using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using static LevelMessage;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] LayerMask jumpCollisionLayerMask;
    [SerializeField] LayerMask victoryLayerMask;

    [Range(1, 4)]
    [SerializeField] float playerMovementSpeed = 2.5f;
    [Range(100, 200)]
    [SerializeField] float playerRotateSpeed = 150f;

    [SerializeField] bool playerLooksRightOnLoad = false;
    public bool MovementEnabled => playerMovementEnabled;
    public IReadOnlyReactiveProperty<bool> MovedThisFrame => movedThisFrame;
    public IReadOnlyReactiveProperty<bool> RotatedThisFrame => rotatedThisFrame;
    public IReadOnlyReactiveProperty<bool> JustLanded => justLanded;
    public IReadOnlyReactiveProperty<bool> MovedOrRotatedThisFrame => movedThisFrame.Merge(rotatedThisFrame).ToReadOnlyReactiveProperty();

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        priorPosition = transform.localPosition;
        broker.Receive<ShowWindowIntent>().Subscribe(_ => SetMovementEnabled(false));
        broker.Receive<HideWindowIntent>().Subscribe(_ => SetMovementEnabled(true));

        broker.Receive<PlayerMessage>()
            .Where(_ => playerMovementEnabled)
            .Subscribe(m => PreparePlayerMovement(m))
            .AddTo(this);

        broker.Receive<PlayerMovementEnabledMessage>()
            .Subscribe(m => SetMovementEnabled(m.enabledState))
            .AddTo(this);

        this.LateUpdateAsObservable()
            .Where(_ => playerMovementEnabled)
            .Subscribe(_ => HandlePlayerMovement())
            .AddTo(this);

        broker.Receive<PlayerState>()
            .Where(state => state == PlayerState.Dead)
            .Where(_ => !IsOnScreen)
            .Select(_ => PlaneOutsideOfBounds)
            .Subscribe(p => DisableRigidBody(p))
            .AddTo(this);

        playerMovementEnabled = true;
    }

    public bool PlayerMovementMessageIsPaused => isResetting;
    public void PausePlayerMoveMessage() => isResetting = true;
    public void ResumePlayerMoveMessage() => isResetting = false;

    bool IsOnScreen => GetComponent<Player>().IsOnScreen;
    
    Plane? PlaneOutsideOfBounds
    {
        get
        {
            var cam = FindObjectOfType<CameraManager>();
            if (cam == null) return null;
            else return cam.PlaneTargetIsOutsideOf(this.gameObject);
        }
    }

    void DisableRigidBody(Plane? plane = null)
    {
        if (PlaneIsTopOfScreen(plane)) return;
        rigidBody.velocity = Vector2.zero;
        rigidBody.isKinematic = true;
    }

    bool PlaneIsTopOfScreen(Plane? plane) => plane != null && plane.HasValue && plane.Value.normal.y == -1f;

    void SetMovementEnabled(bool enabled) => playerMovementEnabled = enabled;

    void PreparePlayerMovement(PlayerMessage message)
    {
        frameMove += message.MovementVector;
        frameRotate += message.RotationAmount;
    }

    void HandlePlayerMovement()
    {
        Move();
        Rotate();
        justLanded.Value = !wasGroundedPreviousFrame && IsGrounded && priorPosition.y > transform.localPosition.y;

        var differenceBetweenPositions = transform.localPosition - priorPosition;

        if (!isResetting && (differenceBetweenPositions.x != 0 || differenceBetweenPositions.y != 0 || differenceBetweenPositions.z != 0)) MessageBroker.Default.Publish(new PlayerMoveMessage(transform.localPosition - priorPosition));
        priorPosition = transform.localPosition;

        //Done for performance reasons. Checking entirely overlapping colliders is a lot of processing. Only check when absolutely needed.
        if (IsPossiblyVictorious)
        {
            if (IsVictorious)
            {
                SetMovementEnabled(false);
                MessageBroker.Default.Publish(new PlayerArrivedAtGoalMessage());
            }
        }
    }

    bool IsGrounded => IsCollidingWithLayer(jumpCollisionLayerMask);
    bool IsPossiblyVictorious => IsCollidingWithLayer(victoryLayerMask);

    bool IsVictorious
    {
        get
        {
            
            var points = GetBoxPoints2D(boxCollider).Concat(new Vector2[] { new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.center.y) });

            foreach(var point in points)
            {
                if(!Physics2D.OverlapPoint(point, victoryLayerMask)) return false;
            }

            return true;
        }
    }

    public Vector2[] GetBoxPoints2D(BoxCollider2D box)
    {
        var size = box.size * 0.5f;

        var mtx = Matrix4x4.TRS(box.bounds.center, box.transform.localRotation, box.transform.localScale);
        var points = new Vector2[4];
        points[0] = mtx.MultiplyPoint3x4(new Vector3(-size.x, size.y));
        points[1] = mtx.MultiplyPoint3x4(new Vector3(-size.x, -size.y));
        points[2] = mtx.MultiplyPoint3x4(new Vector3(size.x, -size.y));
        points[3] = mtx.MultiplyPoint3x4(new Vector3(size.x, size.y));

        return points;
    }

    bool IsCollidingWithLayer(LayerMask layerMask)
    {
        var rayCastCollider = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, layerMask);
        return rayCastCollider.collider != null;
    }

    float jumpVelocity = 5;

    void Move()
    {
        movedThisFrame.Value = frameMove != Vector3.zero;
        if (movedThisFrame.Value)
        {
            if ((frameMove.x > 0 && (playerLooksRightOnLoad ? transform.localScale.x < 0 : transform.localScale.x > 0)) || (frameMove.x < 0 && (playerLooksRightOnLoad ? transform.localScale.x > 0 : transform.localScale.x < 0)))
                transform.localScale = new Vector3(-1f * transform.localScale.x, transform.localScale.y, transform.localScale.z);

            var moveSpeed = playerMovementSpeed * frameMove * Time.deltaTime;
            if (frameMove.y == Vector3.up.y && IsGrounded)
            {
                rigidBody.velocity = Vector2.up * jumpVelocity;
            }
            else if (!IsGrounded && frameMove.y != Vector3.up.y && frameMove.y != Vector3.down.y)
            {
                rigidBody.velocity = new Vector2(moveSpeed.x * 100, rigidBody.velocity.y);
            }

            transform.localPosition += moveSpeed;
            frameMove = Vector3.zero;
        }
    }

    void Rotate()
    {
        rotatedThisFrame.Value = frameRotate != 0f;
        if (rotatedThisFrame.Value)
        {
            transform.Rotate(Vector3.forward, frameRotate * Time.deltaTime * playerRotateSpeed);
            frameRotate = 0f;
        }
    }

    IMessageBroker broker = MessageBroker.Default;
    bool playerMovementEnabled;
    Vector3 frameMove;
    float frameRotate;
    ReactiveProperty<bool> movedThisFrame = new ReactiveProperty<bool>();
    ReactiveProperty<bool> rotatedThisFrame = new ReactiveProperty<bool>();
    ReactiveProperty<bool> justLanded = new ReactiveProperty<bool>();
    Rigidbody2D rigidBody;
    BoxCollider2D boxCollider;
    bool wasGroundedPreviousFrame = false;
    Vector3 priorPosition;
    bool isResetting;
}


public struct PlayerArrivedAtGoalMessage { } 