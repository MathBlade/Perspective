using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class YouWonLevelPanel : WindowManager
{
    [SerializeField] Button backToMainMenuButton;
    [SerializeField] Button saveAndQuitButton;
    [SerializeField] Button saveAndNextLevelButton;
    [SerializeField] TextMeshProUGUI winningText;

    protected override WindowType WindowType => WindowType.WonLevel;

    protected override void Start()
    {
        base.Start();

        bool hasNextLevel = ApplicationExtensions.HasNextLevel;

        if (hasNextLevel) winningText.text = "You Won The Level! What's the next perspective?";
        else winningText.text = "You Won The Game! What's the next perspective?";

        backToMainMenuButton.OnClickAsObservable().Subscribe(_ => BackToMainMenu()).AddTo(this);
        saveAndQuitButton.OnClickAsObservable().Subscribe(_ => Quit()).AddTo(this);

        saveAndNextLevelButton.gameObject.SetActive(ApplicationExtensions.HasNextLevel);
        if (hasNextLevel) saveAndNextLevelButton.OnClickAsObservable().Subscribe(_ => LoadNextLevel()).AddTo(this);
    }

    protected override void ShowMe()
    {
        base.ShowMe();
        ApplicationExtensions.SaveVictory();
    }

    void BackToMainMenu() => ApplicationExtensions.BackToMainMenu();
    void Quit() => ApplicationExtensions.QuitGame(false);
    void LoadNextLevel() => ApplicationExtensions.LoadNextLevel();
    
}
