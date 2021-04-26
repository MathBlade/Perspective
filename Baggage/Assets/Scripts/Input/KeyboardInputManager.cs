using CameraTools;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using static InputDefaults;

public class KeyboardInputManager : Singleton<KeyboardInputManager>
{
    private void Start()
    {
        foreach(var keyActionMethodPair in KeyActionDictionary)
        {
            this.UpdateAsObservable()
                .Where(_ => Input.GetKey(CurrentInputs.GetKeyMap(keyActionMethodPair.Key).keyCode))
                .Subscribe(_ => keyActionMethodPair.Value.Invoke())
                .AddTo(this);
        }
    }

    static readonly Dictionary<KeyAction, Action> KeyActionDictionary = new Dictionary<KeyAction, Action>()
    {
        //Player
        { KeyAction.MovePlayerDown, () => MessageBroker.Default.Publish(PlayerMessage.MoveDown) },
        { KeyAction.MovePlayerUp, () => MessageBroker.Default.Publish(PlayerMessage.MoveUp) },
        { KeyAction.MovePlayerLeft, () => MessageBroker.Default.Publish(PlayerMessage.MoveLeft) },
        { KeyAction.MovePlayerRight, () => MessageBroker.Default.Publish(PlayerMessage.MoveRight) },
        { KeyAction.RotatePlayerClockwise, () => MessageBroker.Default.Publish(PlayerMessage.RotateClockwise) },
        { KeyAction.RotatePlayerCounterclockwise, () => MessageBroker.Default.Publish(PlayerMessage.RotateCounterClockwise) },

        //Camera section
        { KeyAction.MoveCameraUp, () => MessageBroker.Default.Publish(CameraMessage.MoveUp) },
        { KeyAction.MoveCameraDown, () => MessageBroker.Default.Publish(CameraMessage.MoveDown) },
        { KeyAction.MoveCameraLeft, () => MessageBroker.Default.Publish(CameraMessage.MoveLeft) },
        { KeyAction.MoveCameraRight, () => MessageBroker.Default.Publish(CameraMessage.MoveRight) },
        { KeyAction.RotateCameraClockwise, () => MessageBroker.Default.Publish(CameraMessage.RotateClockwise) },
        { KeyAction.RotateCameraCounterclockwise, () => MessageBroker.Default.Publish(CameraMessage.RotateCounterClockwise) },
        { KeyAction.RecenterCamera, () => MessageBroker.Default.Publish(CameraMessage.RecenterCamera) },
        { KeyAction.ZoomInCamera, () => MessageBroker.Default.Publish(CameraMessage.ZoomIn) },
        { KeyAction.ZoomOutCamera, () => MessageBroker.Default.Publish(CameraMessage.ZoomOut) },
        { KeyAction.DisableCameraMovementInput, () => MessageBroker.Default.Publish(CameraEnabledMessage.Disabled) },
        { KeyAction.EnableCameraMovementInput, () => MessageBroker.Default.Publish(CameraEnabledMessage.Enabled) },
    };
}
