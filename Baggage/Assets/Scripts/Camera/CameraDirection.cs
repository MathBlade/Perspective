namespace CameraTools
{
    public static class CameraDirection
    {
        public enum MovementDirection
        {
            None,
            Up,
            Down,
            Left,
            Right
        }

        public enum RotationDirection
        {
            None,
            Clockwise,
            Counterclockwise
        }

        public enum ZoomDirection
        {
            None,
            In,
            Out,
            MouseZoomIn,
            MouseZoomOut
        }
    }
}
