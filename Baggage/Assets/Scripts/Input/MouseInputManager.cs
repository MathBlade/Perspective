using CameraTools;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class MouseInputManager : MonoBehaviour
{

    Vector2 screen;
    float mousePositionOnRotateStart;

    private void Awake() => screen = new Vector2Int(Screen.width, Screen.height);
    
    // Start is called before the first frame update
    void Start()
    {
        this.UpdateAsObservable().Subscribe(_ => CheckMovement());

        this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonDown((int)MouseButtons.Right))
            .Subscribe(_ => SetMousePositionOnRotateStart());

        this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButton((int)MouseButtons.Right))
            .Subscribe(_ => DoRotation());

        this.UpdateAsObservable()
            .Where(_ => Input.mouseScrollDelta.y > 0)
            .Subscribe(_ => MessageBroker.Default.Publish(CameraMessage.MouseZoomIn));

        this.UpdateAsObservable()
            .Where(_ => Input.mouseScrollDelta.y < 0)
            .Subscribe(_ => MessageBroker.Default.Publish(CameraMessage.MouseZoomOut));

    }

    void CheckMovement()
    {
        mousePosition = Input.mousePosition;
        bool mouseInYBounds = (mousePosition.y <= screen.y * (1f + yBoundsPercent)) && (mousePosition.y >= screen.y * (0f - yBoundsPercent));
        bool mouseInXBounds = (mousePosition.x <= screen.x * (1f + xBoundsPercent)) && (mousePosition.x >= screen.x * (0f - xBoundsPercent));
        bool mouseValid = mouseInXBounds && mouseInYBounds;
        
        if (!mouseValid) return;

        bool wantToMoveUp = false;
        bool wantToMoveDown = false;
        bool wantToMoveLeft = false;
        bool wantToMoveRight = false;
        if (mousePosition.y > screen.y * (1 - yBoundsPercent)) wantToMoveUp = true;
        if (mousePosition.y < screen.y * (0f + yBoundsPercent)) wantToMoveDown = true;
        if (mousePosition.x > screen.x * (1 - xBoundsPercent)) wantToMoveRight = true;
        if (mousePosition.x < screen.x * (0f + xBoundsPercent)) wantToMoveLeft = true;

        if (wantToMoveUp && !wantToMoveLeft && !wantToMoveRight) MessageBroker.Default.Publish(CameraMessage.MoveUp);
        if (wantToMoveDown && !wantToMoveLeft && !wantToMoveRight) MessageBroker.Default.Publish(CameraMessage.MoveDown);
        if (wantToMoveLeft && !wantToMoveUp && !wantToMoveDown) MessageBroker.Default.Publish(CameraMessage.MoveLeft);
        if (wantToMoveRight && !wantToMoveUp && !wantToMoveDown) MessageBroker.Default.Publish(CameraMessage.MoveRight);
    }

    void SetMousePositionOnRotateStart() => mousePositionOnRotateStart = mousePosition.x;

    void DoRotation()
    {
        if (mousePosition.x < mousePositionOnRotateStart) MessageBroker.Default.Publish(CameraMessage.RotateCounterClockwise);
        else if (mousePosition.x > mousePositionOnRotateStart) MessageBroker.Default.Publish(CameraMessage.RotateClockwise);
    }
    

    Vector3 mousePosition;
    const float yBoundsPercent = 0.05f;
    const float xBoundsPercent = 0.05f;

    enum MouseButtons
    {
        Left = 0,
        Right = 1,
        Middle = 2
    }
}
