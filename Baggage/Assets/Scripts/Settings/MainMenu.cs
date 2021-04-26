using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Button newGameButton;
    [SerializeField] Button loadGameButton;
    [SerializeField] Button optionsButton;
    [SerializeField] Button quitButton;
    [SerializeField] Button continueButton;

    void Start()
    {
        if (!ApplicationExtensions.HasActivePlayer) continueButton.enabled = false;

        newGameButton.OnClickAsObservable().Subscribe(_ => NewGame());
        continueButton.OnClickAsObservable().Subscribe(_ => ContinueGame());
        loadGameButton.OnClickAsObservable().Subscribe(_ => LoadGame());
        optionsButton.OnClickAsObservable().Subscribe(_ => ShowOptions());
        quitButton.OnClickAsObservable().Subscribe(_ => ApplicationExtensions.QuitGame(false));
    }

    void NewGame() => MessageBroker.Default.Publish(new ShowWindowIntent(WindowType.NewGame));
    void ContinueGame() => ApplicationExtensions.ContinueGame();

    void LoadGame()
    {
        MessageBroker.Default.Publish(new ShowWindowIntent(WindowType.Profile));
    }

    void ShowOptions()
    {
        MessageBroker.Default.Publish(new ShowWindowIntent(WindowType.Settings));
    }
}