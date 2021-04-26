using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using static LevelMessage;
using static ScreenMessage;

public class GameOverWindow : WindowManager
{
    [SerializeField] Button backToMainMenuButton;
    [SerializeField] Button saveAndQuitButton;
    [SerializeField] Button saveAndRetryButton;

    protected override WindowType WindowType => WindowType.Defeat;

    protected override void Start()
    {
        base.Start();
        MessageBroker.Default.Receive<DefeatReason>().Subscribe(r => AddDefeatReasonToList(r));
        backToMainMenuButton.OnClickAsObservable().Subscribe(_ => BackToMainMenu()).AddTo(this);
        saveAndQuitButton.OnClickAsObservable().Subscribe(_ => Quit()).AddTo(this);
        saveAndRetryButton.OnClickAsObservable().Subscribe(_ => Retry()).AddTo(this);
    }

    void AddDefeatReasonToList(DefeatReason reason) => ApplicationExtensions.SaveDefeat(reason);

    void BackToMainMenu() => ApplicationExtensions.BackToMainMenu();

    void Quit() => ApplicationExtensions.QuitGame(false);

    void Retry()
    {
        MessageBroker.Default.Publish(new ResetThisLevelMessage());
        HideMe();
    }
}
