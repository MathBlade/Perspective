using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class TransphobiaWarning : MonoBehaviour
{
    const string WARNING_ACKNOWLEDGED = "WARNING ACKNOWLEDGED";
    [SerializeField] Button okayButton;
    [SerializeField] GameObject menu;
    [SerializeField] GameObject settingsPanel;

    void Start()
    {
        if (PlayerPrefs.HasKey(WARNING_ACKNOWLEDGED) && PlayerPrefs.GetInt(WARNING_ACKNOWLEDGED) == 1)
        {
            WarningAccepted();
            return;
        }

        okayButton.OnClickAsObservable().Subscribe(_ => WarningAccepted()).AddTo(this);
    }

    void WarningAccepted()
    {
        if (menu != null) menu.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(true);
        PlayerPrefs.SetInt(WARNING_ACKNOWLEDGED, 1);
        Destroy(gameObject);
    }
}
