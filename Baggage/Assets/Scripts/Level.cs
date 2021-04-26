using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using static LevelMessage;
using static ScreenMessage;

public class Level : MonoBehaviour
{
    const string LEVEL_PREFIX = "Level: ";

    [SerializeField] float secondsToCompleteLevel = 15*60;
    [SerializeField] TextMeshProUGUI levelText;

    void Start()
    {
        MessageBroker.Default.Publish(new StartLevelMessage(secondsToCompleteLevel));

        MessageBroker.Default.Receive<EndLevelMessage>()
            .Where(endLevel => endLevel.WasVictorious)
            .First()
            .Subscribe(_ => ShowVictoryScreen())
            .AddTo(this);

        MessageBroker.Default.Receive<EndLevelMessage>()
            .Where(endLevel => !endLevel.WasVictorious)
            .First()
            .Subscribe(e => ShowDefeatScreen(e.Reason))
            .AddTo(this);

        levelText.text = LEVEL_PREFIX + ApplicationExtensions.CurrentLevel;
    }

    void ShowDefeatScreen(DefeatReason? reason)
    {
        MessageBroker.Default.Publish(reason);
        MessageBroker.Default.Publish(new ShowWindowIntent(WindowType.Defeat));
    }

    void ShowVictoryScreen()
    {
        MessageBroker.Default.Publish(new ShowWindowIntent(WindowType.WonLevel));
    }

   

    bool foundActiveScene;
    int maxLevelNumberForAllScenes;
    int activeLevelNumber;
}