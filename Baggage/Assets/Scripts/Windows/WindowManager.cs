using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using static ScreenMessage;

public abstract class WindowManager : MonoBehaviour
{
    [SerializeField] GameObject windowObject;
    [SerializeField] Button closeButton;

    protected abstract WindowType WindowType { get; }
    protected virtual void Start()
    {
        MessageBroker.Default.Receive<ShowWindowIntent>()
            .Where(w => w.WindowType == WindowType)
            .Subscribe(_ => ShowMe())
            .AddTo(this);

        if (closeButton != null) closeButton.OnClickAsObservable().Subscribe(_ => HideMe()).AddTo(this);
    }

    protected virtual void HideMe()
    {
        windowObject.SetActive(false);
        MessageBroker.Default.Publish(new HideWindowIntent(WindowType));
    }
    protected virtual void ShowMe() => windowObject.SetActive(true);
}
public enum WindowType
{
    Settings,
    Quit,
    Reset,
    WonLevel,
    Defeat,
    Profile,
    NewGame
}

public class ShowWindowIntent
{
    public WindowType WindowType { get; private set; }
    public ShowWindowIntent(WindowType windowType)
    {
        WindowType = windowType;
    }
}

public class HideWindowIntent
{
    public WindowType WindowType { get; private set; }
    public HideWindowIntent(WindowType windowType)
    {
        WindowType = windowType;
    }
}


