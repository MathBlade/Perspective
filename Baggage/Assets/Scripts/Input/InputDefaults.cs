using System.Linq;
using UnityEngine;
public static class InputDefaults
{
    public enum KeyAction
    {
        //Character
        MovePlayerUp,
        MovePlayerLeft,
        MovePlayerRight,
        MovePlayerDown,
        RotatePlayerClockwise,
        RotatePlayerCounterclockwise,

        //Camera
        MoveCameraLeft,
        MoveCameraRight,
        MoveCameraUp,
        MoveCameraDown,
        RotateCameraCounterclockwise,
        RotateCameraClockwise,
        RecenterCamera,
        ZoomInCamera,
        ZoomOutCamera,
        DisableCameraMovementInput,
        EnableCameraMovementInput
    }

    public static readonly KeyMap[] DEFAULT_KEY_MAPS = new KeyMap[]
     {
        new KeyMap(KeyAction.MoveCameraUp, KeyCode.UpArrow),
        new KeyMap(KeyAction.MoveCameraDown, KeyCode.DownArrow),
        new KeyMap(KeyAction.MoveCameraLeft, KeyCode.LeftArrow),
        new KeyMap(KeyAction.MoveCameraRight, KeyCode.RightArrow),
        new KeyMap(KeyAction.RotateCameraClockwise, KeyCode.LeftShift),
        new KeyMap(KeyAction.RotateCameraCounterclockwise, KeyCode.C),
        new KeyMap(KeyAction.RecenterCamera, KeyCode.R),
        new KeyMap(KeyAction.ZoomInCamera, KeyCode.Z),
        new KeyMap(KeyAction.ZoomOutCamera, KeyCode.X),
        new KeyMap(KeyAction.MovePlayerUp, KeyCode.W),
        new KeyMap(KeyAction.MovePlayerDown, KeyCode.S),
        new KeyMap(KeyAction.MovePlayerLeft, KeyCode.A),
        new KeyMap(KeyAction.MovePlayerRight, KeyCode.D),
        new KeyMap(KeyAction.RotatePlayerClockwise, KeyCode.Q),
        new KeyMap(KeyAction.RotatePlayerCounterclockwise, KeyCode.E),
        new KeyMap(KeyAction.DisableCameraMovementInput, KeyCode.V, false),
        new KeyMap(KeyAction.EnableCameraMovementInput, KeyCode.B, false)
     };

    public static KeyMap GetDefaultKeyMap(KeyAction action) => DEFAULT_KEY_MAPS.Where(map => map.keyAction == action).FirstOrDefault();
    public static KeyCode GetDefaultKeyCode(KeyAction action) => GetDefaultKeyMap(action).keyCode;
}
