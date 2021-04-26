using UnityEngine;

namespace CameraTools
{
    public class CameraMessage
    {
        const float MOUSE_MAGNITUDE = 10;
        public static CameraMessage MoveLeft => moveLeftMessage ??= new CameraMessage(CameraDirection.MovementDirection.Left);
        public static CameraMessage MoveRight => moveRightMessage ??= new CameraMessage(CameraDirection.MovementDirection.Right);
        public static CameraMessage MoveUp => moveUpMessage ??= new CameraMessage(CameraDirection.MovementDirection.Up);
        public static CameraMessage MoveDown => moveDownMessage ??= new CameraMessage(CameraDirection.MovementDirection.Down);

        public static CameraMessage RotateClockwise => rotateClockwiseMessage ??= new CameraMessage(CameraDirection.RotationDirection.Clockwise);
        public static CameraMessage RotateCounterClockwise => rotateCounterClockwiseMessage ??= new CameraMessage(CameraDirection.RotationDirection.Counterclockwise);

        public static CameraMessage RecenterCamera => recenterCamera ??= new CameraMessage(true);

        public static CameraMessage ZoomIn => zoomInMessage ??= new CameraMessage(CameraDirection.ZoomDirection.In);
        public static CameraMessage ZoomOut => zoomOutMessage ??= new CameraMessage(CameraDirection.ZoomDirection.Out);

        public static CameraMessage MouseZoomIn => mouseZoomInMessage ??= new CameraMessage(CameraDirection.ZoomDirection.MouseZoomIn);
        public static CameraMessage MouseZoomOut => mouseZoomOutMessage ??= new CameraMessage(CameraDirection.ZoomDirection.MouseZoomOut);

        public Vector3 MovementVector { get; private set; }
        public float RotationAmount { get; private set; }
        public float ZoomAmount { get; private set; }

        public bool Recenter { get; private set; }

        private CameraMessage(bool recenter) : this(CameraDirection.MovementDirection.None, CameraDirection.RotationDirection.None, CameraDirection.ZoomDirection.None)
        {
            Recenter = recenter;
        }

        private CameraMessage(CameraDirection.MovementDirection movementDirection) : this(movementDirection, CameraDirection.RotationDirection.None, CameraDirection.ZoomDirection.None) { }
        private CameraMessage(CameraDirection.RotationDirection rotationDirection) : this(CameraDirection.MovementDirection.None, rotationDirection, CameraDirection.ZoomDirection.None) { }
        private CameraMessage(CameraDirection.ZoomDirection zoomDirection) : this(CameraDirection.MovementDirection.None, CameraDirection.RotationDirection.None, zoomDirection) { }

        private CameraMessage(CameraDirection.MovementDirection movementDirection, CameraDirection.RotationDirection rotationDirection, CameraDirection.ZoomDirection zoomDirection)
        {
            Movement = movementDirection;
            Rotation = rotationDirection;
            Zoom = zoomDirection;

            SetMovementAmount();
            SetRotationAmount();
            SetZoomAmount();  
        }

        void SetMovementAmount()
        {
            switch (Movement)
            {
                case CameraDirection.MovementDirection.None:
                    MovementVector = Vector3.zero;
                    break;
                case CameraDirection.MovementDirection.Down:
                    MovementVector = Vector3.back;
                    break;
                case CameraDirection.MovementDirection.Up:
                    MovementVector = Vector3.forward;
                    break;
                case CameraDirection.MovementDirection.Left:
                    MovementVector = Vector3.left;
                    break;
                case CameraDirection.MovementDirection.Right:
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
                case CameraDirection.RotationDirection.None:
                    RotationAmount = 0f;
                    break;
                case CameraDirection.RotationDirection.Counterclockwise:
                    RotationAmount = CounterclockwiseFloat;
                    break;
                case CameraDirection.RotationDirection.Clockwise:
                    RotationAmount = ClockwiseFloat;
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException("No vector provided for rotation");

            }
        }

        void SetZoomAmount()
        {
            switch (Zoom)
            {
                case CameraDirection.ZoomDirection.None:
                    ZoomAmount = 0f;
                    break;
                case CameraDirection.ZoomDirection.In:
                    ZoomAmount = ZoomInFloat;
                    break;
                case CameraDirection.ZoomDirection.Out:
                    ZoomAmount = ZoomOutFloat;
                    break;
                case CameraDirection.ZoomDirection.MouseZoomIn:
                    ZoomAmount = MOUSE_MAGNITUDE * ZoomInFloat;
                    break;
                case CameraDirection.ZoomDirection.MouseZoomOut:
                    ZoomAmount = MOUSE_MAGNITUDE * ZoomOutFloat;
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException("No vector provided for zoom");

            }
        }

        CameraDirection.MovementDirection Movement;
        CameraDirection.RotationDirection Rotation;
        CameraDirection.ZoomDirection Zoom;

        const float ClockwiseFloat = 1f;
        const float CounterclockwiseFloat = -1f;
        const float ZoomInFloat = -1f;
        const float ZoomOutFloat = 1f;

        static CameraMessage moveLeftMessage;
        static CameraMessage moveRightMessage;
        static CameraMessage moveUpMessage;
        static CameraMessage moveDownMessage;
        static CameraMessage recenterCamera;

        static CameraMessage rotateClockwiseMessage;
        static CameraMessage rotateCounterClockwiseMessage;

        static CameraMessage zoomInMessage;
        static CameraMessage zoomOutMessage;
        static CameraMessage mouseZoomInMessage;
        static CameraMessage mouseZoomOutMessage;
    }

    public class CameraEnabledMessage
    {
        public static CameraEnabledMessage Enabled => enableCamerasMessage ??= new CameraEnabledMessage(true);
        public static CameraEnabledMessage Disabled => disableCamerasMessage ??= new CameraEnabledMessage(false);

        public bool enabledState { get; private set; }
        private CameraEnabledMessage(bool enabled)
        {
            this.enabledState = enabled;
        }

        static CameraEnabledMessage enableCamerasMessage;
        static CameraEnabledMessage disableCamerasMessage;
    }
}


