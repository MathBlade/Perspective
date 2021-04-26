using CameraTools;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using static LevelMessage;

public class CameraManager : Singleton<CameraManager>
{
    [Header("Camera Positioning")]
    public Vector3 cameraOffset = new Vector3(0,4f,0);
    public bool is3D = false;

    [Header("3D only values")]
    public float lookAtOffset = 0.5f;

    [Header("Move Controls")]
    public float inOutSpeed = 5f;
    public float lateralSpeed = 5f;
    public float rotateSpeed = 45f;

    [Header("Movement Bounds")]
    public Vector2 minBounds = new Vector2();
    public Vector2 maxBounds = new Vector2();

    [Header("Zoom Controls")]
    public float zoomSpeed = 4f;
    public float nearZoomLimit = 2f;
    public float farZoomLimit = 8f;
    public float startingZoom = 4f;

    [SerializeField] bool usePlayerLocationForReset = true;

    void Awake()
    {
        cam = GetComponentInChildren<Camera>();
        cam.transform.localPosition = new Vector3(cameraOffset.x, cameraOffset.y, cameraOffset.z);
        zoomStrategy = cam.orthographic ? (IZoomStrategy)new OrthographicZoomStrategy(cam,startingZoom) : new PerspectiveZoomStrategy(cam, cameraOffset, startingZoom);
        cam.transform.LookAt(transform.position + (is3D ? Vector3.up : Vector3.zero) * lookAtOffset);
    }

    public Plane? PlaneTargetIsOutsideOf(GameObject go)
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(cam);
        var point = go.transform.position;
        foreach (var plane in planes)
        {
            if (plane.GetDistanceToPoint(point) < 0)
                return plane;
        }
        return null;
    }

    void Start()
    {
        startingPosition = transform.position;
        startingRotation = transform.rotation;

        broker.Receive<ShowWindowIntent>().Subscribe(_ => SetCameraEnabled(false));
        broker.Receive<HideWindowIntent>().Subscribe(_ => SetCameraEnabled(true));

        broker.Receive<CameraMessage>()
            .Where(_ => cameraMovementEnabled)
            .Subscribe(m => PrepareCamera(m))
            .AddTo(this);

        broker.Receive<CameraEnabledMessage>()
            .Subscribe(m => SetCameraEnabled(m.enabledState))
            .AddTo(this);

        MessageBroker.Default.Receive<ResetThisLevelMessage>()
            .Subscribe(_ => DoReset())
            .AddTo(this);

        this.LateUpdateAsObservable()
            .Subscribe(_ => HandleCameraMovement());

        broker.Receive<PlayerMoveMessage>()
            .Where(move => move.AmountMoved != Vector3.zero)
            .Subscribe(m => HandlePlayerMovement(m.AmountMoved))
            .AddTo(this);
    }

    void DoReset()
    {
        transform.position = startingPosition;
        cam.transform.localPosition = new Vector3(cameraOffset.x, cameraOffset.y, cameraOffset.z);
        cam.transform.LookAt(transform.position + (is3D ? Vector3.up : Vector3.zero) * lookAtOffset);
        transform.rotation = startingRotation;
        playerShiftsSince = Vector2.zero;
    }

    void HandlePlayerMovement(Vector3 movement)
    {
        transform.position += movement;
        playerShiftsSince += movement;
        LockPositionInBounds();
    }

    void SetCameraEnabled(bool enabled) => cameraMovementEnabled = enabled;

    void PrepareCamera(CameraMessage message)
    {
        if (message.Recenter)
        {
            transform.position = startingPosition + (usePlayerLocationForReset ? playerShiftsSince : Vector3.zero);
            LockPositionInBounds();
            transform.rotation = startingRotation;
            zoomStrategy.Reset(cam);
            return;
        }
        frameMove += is3D ? message.MovementVector : new Vector3(message.MovementVector.x, message.MovementVector.z, message.MovementVector.y);
        frameRotate += message.RotationAmount;
        frameZoom += message.ZoomAmount;
    }

    void HandleCameraMovement()
    {
        Move();
        Rotate();
        Zoom();
    }

    void Move()
    {
        if (frameMove != Vector3.zero)
        {
            Vector3 speedModFrameMove = is3D ?  new Vector3(frameMove.x * lateralSpeed, frameMove.y, frameMove.z * inOutSpeed) : new Vector3(frameMove.x * lateralSpeed, frameMove.y * inOutSpeed);
            transform.position += transform.TransformDirection(speedModFrameMove) * Time.deltaTime;
            LockPositionInBounds();
            frameMove = Vector3.zero;
        }
    }

    void Rotate()
    {
        if (frameRotate != 0f)
        {
            if (is3D) transform.Rotate(Vector3.up, frameRotate * Time.deltaTime * rotateSpeed);
            else transform.Rotate(Vector3.forward, frameRotate * Time.deltaTime * rotateSpeed);
            frameRotate = 0f;
        }
    }

    void Zoom()
    {
        if (frameZoom < 0f)
        {
            zoomStrategy.ZoomIn(cam, Time.deltaTime * Mathf.Abs(frameZoom), nearZoomLimit);
            frameZoom = 0f;
        }

        if (frameZoom > 0f)
        {
            zoomStrategy.ZoomOut(cam, Time.deltaTime * frameZoom, farZoomLimit);
            frameZoom = 0f;
        }
    }

    void LockPositionInBounds()
    {
        if (is3D)
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, minBounds.x, maxBounds.x),
                transform.position.y,
                Mathf.Clamp(transform.position.z, minBounds.y, maxBounds.y)
            );
        else
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, minBounds.x, maxBounds.x),
                Mathf.Clamp(transform.position.y, minBounds.y, maxBounds.y),
                transform.position.z
            );
    }

    Vector3 startingPosition;
    Quaternion startingRotation;
    IZoomStrategy zoomStrategy;
    Vector3 frameMove;
    float frameRotate;
    float frameZoom;
    Camera cam;
    IMessageBroker broker = MessageBroker.Default;
    bool cameraMovementEnabled = true;
    Vector3 playerShiftsSince = Vector3.zero;
}
