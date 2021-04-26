using UnityEngine;

public class PlayerMessage
{
    const float MOUSE_MAGNITUDE = 10;
    public static PlayerMessage MoveLeft => moveLeftMessage ??= new PlayerMessage(PlayerDirection.MovementDirection.Left);
    public static PlayerMessage MoveRight => moveRightMessage ??= new PlayerMessage(PlayerDirection.MovementDirection.Right);
    public static PlayerMessage MoveUp => moveUpMessage ??= new PlayerMessage(PlayerDirection.MovementDirection.Up);
    public static PlayerMessage MoveDown => moveDownMessage ??= new PlayerMessage(PlayerDirection.MovementDirection.Down);

    public static PlayerMessage RotateClockwise => rotateClockwiseMessage ??= new PlayerMessage(PlayerDirection.RotationDirection.Clockwise);
    public static PlayerMessage RotateCounterClockwise => rotateCounterClockwiseMessage ??= new PlayerMessage(PlayerDirection.RotationDirection.Counterclockwise);

    public Vector3 MovementVector { get; private set; }
    public float RotationAmount { get; private set; }

    private PlayerMessage(PlayerDirection.MovementDirection movementDirection) : this(movementDirection, PlayerDirection.RotationDirection.None) { }
    private PlayerMessage(PlayerDirection.RotationDirection rotationDirection) : this(PlayerDirection.MovementDirection.None, rotationDirection) { }
    
    private PlayerMessage(PlayerDirection.MovementDirection movementDirection, PlayerDirection.RotationDirection rotationDirection)
    {
        Movement = movementDirection;
        Rotation = rotationDirection;

        SetMovementAmount();
        SetRotationAmount();
    }

    void SetMovementAmount()
    {
        switch (Movement)
        {
            case PlayerDirection.MovementDirection.None:
                MovementVector = Vector3.zero;
                break;
            case PlayerDirection.MovementDirection.Down:
                MovementVector = Vector3.down;
                break;
            case PlayerDirection.MovementDirection.Up:
                MovementVector = Vector3.up;
                break;
            case PlayerDirection.MovementDirection.Left:
                MovementVector = Vector3.left;
                break;
            case PlayerDirection.MovementDirection.Right:
                MovementVector = Vector3.right;
                break;
            default:
                throw new System.ArgumentOutOfRangeException("No vector provided for movement");
        }
    }

    void SetRotationAmount()
    {
        switch (Rotation)
        {
            case PlayerDirection.RotationDirection.None:
                RotationAmount = 0f;
                break;
            case PlayerDirection.RotationDirection.Counterclockwise:
                RotationAmount = CounterclockwiseFloat;
                break;
            case PlayerDirection.RotationDirection.Clockwise:
                RotationAmount = ClockwiseFloat;
                break;
            default:
                throw new System.ArgumentOutOfRangeException("No vector provided for rotation");

        }
    }

    public PlayerDirection.MovementDirection Movement { get; private set; }
    PlayerDirection.RotationDirection Rotation;

    const float ClockwiseFloat = 1f;
    const float CounterclockwiseFloat = -1f;

    static PlayerMessage moveLeftMessage;
    static PlayerMessage moveRightMessage;
    static PlayerMessage moveUpMessage;
    static PlayerMessage moveDownMessage;

    static PlayerMessage rotateClockwiseMessage;
    static PlayerMessage rotateCounterClockwiseMessage;
}

public class PlayerMovementEnabledMessage
{
    public static PlayerMovementEnabledMessage Enabled => enableMessage ??= new PlayerMovementEnabledMessage(true);
    public static PlayerMovementEnabledMessage Disabled => disableMessage ??= new PlayerMovementEnabledMessage(false);

    public bool enabledState { get; private set; }
    private PlayerMovementEnabledMessage(bool enabled)
    {
        this.enabledState = enabled;
    }

    static PlayerMovementEnabledMessage enableMessage;
    static PlayerMovementEnabledMessage disableMessage;
}

public class PlayerMoveMessage
{
    public Vector3 AmountMoved { get; private set; }
    public PlayerMoveMessage(Vector3 amountMoved)
    {
        AmountMoved = amountMoved;
    }
}


