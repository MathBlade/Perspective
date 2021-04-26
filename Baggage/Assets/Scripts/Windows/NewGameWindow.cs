using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class NewGameWindow : WindowManager
{
    const string INSTRUCTIONS_TEXT = "Enter new profile name";
    [SerializeField] TMP_InputField newNameText;
    [SerializeField] TextMeshProUGUI instructionsText;
    [SerializeField] Button submitButton;

    protected override WindowType WindowType => WindowType.NewGame;

    protected override void Start()
    {
        base.Start();
        submitButton.OnClickAsObservable().Subscribe(_ => StartNewGame());
    }

    void StartNewGame()
    {
        if (newNameText == null || string.IsNullOrEmpty(newNameText.text))
        {
            instructionsText.text = INSTRUCTIONS_TEXT + "\n" + "Please enter a valid name";
            return;
        }
        if (ApplicationExtensions.ProfileExists(newNameText.text))
        {
            instructionsText.text = INSTRUCTIONS_TEXT + "\n" + "Profile exists. Please enter another.";
            return;
        }

        ApplicationExtensions.CreateNewProfile(newNameText.text);
        ApplicationExtensions.LoadNextLevel();
    }

    protected override void HideMe()
    {
        instructionsText.text = INSTRUCTIONS_TEXT;
        base.HideMe();
    }
}
