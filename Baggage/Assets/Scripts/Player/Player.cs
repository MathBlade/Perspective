using CameraTools;
using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using static HeartMessage;
using static LevelMessage;
using static ScreenMessage;

[RequireComponent(typeof(PlayerMovementController))]
[RequireComponent(typeof(PlayerAnimator))]
public class Player : MonoBehaviour, IHurtOnColllision
{
    void Awake()
    {
        movementController = GetComponent<PlayerMovementController>();
        playerAnimator = GetComponent<PlayerAnimator>();
    }

    void Start()
    {
        startingPosition = transform.position;
        startingRotation = transform.rotation;
        playerState.Subscribe(PublishNewState).AddTo(this);
        dateTimeStarted = DateTime.Now;

        MessageBroker.Default.Receive<PlayerStateAnimationCompleteMessage>()
            .Subscribe(m => HandleAnimationComplete(m));

        MessageBroker.Default.Receive<PlayerArrivedAtGoalMessage>()
            .Where(_ => isAlive == true)
            .Subscribe(_ =>
                {
                    levelIsOver = true;
                    MessageBroker.Default.Publish(new EndLevelMessage(true, null));
                }
            );

        MessageBroker.Default.Receive<StartLevelMessage>()
            .Subscribe(m => maxSecondsForLevel = m.SecondsUntilEndOfLevel)
            .AddTo(this);

        MessageBroker.Default.Receive<ResetThisLevelMessage>()
            .Subscribe(_ => DoReset())
            .AddTo(this);

        MessageBroker.Default.Receive<HeartMessage.OutOfHeartsMessage>()
            .Where(_ => isAlive == true)
            .Subscribe(_ => KillMe(DefeatReason.OutOfHearts))
            .AddTo(this);

        movementController.JustLanded
            .Where(justLanded => justLanded == true)
            .Where(_ => isAlive == true)
            .Subscribe(_ => {
                if (Input.anyKey) playerState.Value = PlayerState.Run;
                else playerState.Value = PlayerState.Land; })
            .AddTo(this);

        movementController.MovedThisFrame
            .Where(movedThisFrame => movedThisFrame == true)
            .Subscribe(_ => playerState.Value = PlayerState.Run)
            .AddTo(this);

        movementController.MovedOrRotatedThisFrame
            .Where(_ => PlayerCanSwapToIdle)
            .Where(changedThisFrame => changedThisFrame == false)
            .Subscribe(_ => playerState.Value = PlayerState.Idle)
            .AddTo(this);

        this.UpdateAsObservable()
            .Where(_ => !IsOnScreen)
            .Subscribe(_ => KillMe(DefeatReason.FellOffScreen))
            .AddTo(this);

        this.UpdateAsObservable()
            .TakeWhile(_ => !levelIsOver && maxSecondsForLevel != float.MaxValue)
            .Where(_ => DateTime.Now.Subtract(dateTimeStarted) > TimeSpan.FromSeconds(maxSecondsForLevel))
            .Subscribe(_ => KillMe(DefeatReason.TimeElapsed))
            .AddTo(this);
    }

    void Update()
    {
        //Tired it's midnight. Should unirx this.
        if (movementController.PlayerMovementMessageIsPaused)
        {
            if (framesToWaitBeforeUnpausing > 0)
            {
                framesToWaitBeforeUnpausing--;
                if (framesToWaitBeforeUnpausing == 0) movementController.ResumePlayerMoveMessage();
            }
        }
    }

    void DoReset()
    {
        framesToWaitBeforeUnpausing = 5;
        movementController.PausePlayerMoveMessage();
        isAlive = true;
        transform.position = startingPosition;
        transform.rotation = startingRotation;
        dateTimeStarted = DateTime.Now;
        MessageBroker.Default.Publish(new ResetHeartsMessage());
        playerState.Value = PlayerState.Idle;
    }

    public bool IsAlive => isAlive;

    public void HitDeadlyObstacle()
    {
        if (isAlive) KillMe(DefeatReason.HitDeadlyObstacle);
    }

    public void TookDamage(int damage)
    {
        if (isAlive) MessageBroker.Default.Publish(new DamageHeartsMessage(damage));
    }

    void HandleAnimationComplete(PlayerStateAnimationCompleteMessage message)
    {
        switch(message.StateFinished)
        {
            case PlayerState.Land:
                playerState.Value = PlayerState.Idle;
                return;
            default:
                throw new NotImplementedException($"Did not implement what to do on animation complete for state {message.StateFinished}");
        }
    }

    public bool IsOnScreen
    {
        get
        {
            if (playerAnimator == null) return true;
            if (!playerAnimator.HasInitialized.Value) return true;
            else
            {
                return playerAnimator.IsVisible;
            }
        }
    }

    bool PlayerCanSwapToIdle => playerState.Value != PlayerState.Idle && isAlive;

    void KillMe(DefeatReason reason)
    {
        MessageBroker.Default.Publish(PlayerMovementEnabledMessage.Disabled);
        isAlive = false;
        playerState.Value = PlayerState.Dead;
        levelIsOver = true;
        MessageBroker.Default.Publish(new EndLevelMessage(false, reason));
    }

    void PublishNewState(PlayerState state) => MessageBroker.Default.Publish<PlayerState>(state);

    ReactiveProperty<PlayerState> playerState = new ReactiveProperty<PlayerState>();
    PlayerMovementController movementController;
    PlayerAnimator playerAnimator;
    DateTime dateTimeStarted;
    bool isAlive = true;
    float maxSecondsForLevel = float.MaxValue;
    bool levelIsOver;
    Vector3 startingPosition;
    Quaternion startingRotation;
    int framesToWaitBeforeUnpausing = 0;
}
