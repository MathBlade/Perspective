using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using static LevelMessage;

public class ResetWindowManager : WindowManager
{
    [SerializeField] Button retryButton;
    protected override WindowType WindowType => WindowType.Reset;

    protected override void Start()
    {
        base.Start();
        retryButton.OnClickAsObservable().Subscribe(_ =>
        {
            MessageBroker.Default.Publish(new ResetThisLevelMessage());
            HideMe();
        });
    }
}
