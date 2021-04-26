using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ProfileEntry : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Button loadButton;
    [SerializeField] Button deleteButton;
    // Start is called before the first frame update
    void Start()
    {
        loadButton.OnClickAsObservable().Subscribe(_ => LoadProfile());
        deleteButton.OnClickAsObservable().Subscribe(_ => DeleteProfile());
    }

    public void Initialize(string profileName)
    {
        nameText.text = profileName;
        levelText.text = ApplicationExtensions.GetLevelSaved(profileName).ToString();
        loadButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Load Game for {profileName}";
        thisProfileName = profileName;
    }

    void LoadProfile() => ApplicationExtensions.LoadProfile(thisProfileName);

    void DeleteProfile()
    {
        ApplicationExtensions.DeleteProfile(thisProfileName);
        MessageBroker.Default.Publish(new ProfileWindowRefreshList());
    }

    string thisProfileName;
}
