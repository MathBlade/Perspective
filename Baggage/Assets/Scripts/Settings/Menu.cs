using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class Menu : Singleton<Menu>
{
    [SerializeField] Button settingsButton;
    [SerializeField] Button restartButton;
    [SerializeField] Button quitButton;
    [SerializeField] Button homeButton;

    void Start()
    {
        MessageBroker.Default.Receive<HideWindowIntent>().Subscribe(_ => EnableClickInputs());

        settingsButton.OnClickAsObservable()
            .Where(_ => allowClickInputs)
            .Subscribe(_ => ShowSettingsWindow());

        restartButton.OnClickAsObservable()
            .Where(_ => allowClickInputs)
            .Subscribe(_ => Reset());

        quitButton.OnClickAsObservable()
            .Where(_ => allowClickInputs)
            .Subscribe(_ =>
                {
                    DisableClickInputs();
                    ApplicationExtensions.QuitGame(true);
                }
            )
            .AddTo(this);

        homeButton.OnClickAsObservable()
            .Subscribe(_ => ApplicationExtensions.BackToMainMenu())
            .AddTo(this);
    }

    private void Reset()
    {
        DisableClickInputs();
        MessageBroker.Default.Publish(new ShowWindowIntent(WindowType.Reset));
    }

    void ShowSettingsWindow()
    {
        DisableClickInputs();
        SendSettingsWindowIntent();
    }

    void SendSettingsWindowIntent() => MessageBroker.Default.Publish(new ShowWindowIntent(WindowType.Settings));

    void EnableClickInputs() => allowClickInputs = true;
    void DisableClickInputs() => allowClickInputs = false;

    bool allowClickInputs = true;
}

