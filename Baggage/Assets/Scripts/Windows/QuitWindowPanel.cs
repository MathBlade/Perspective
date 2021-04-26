using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class QuitWindowPanel : WindowManager
{
    [SerializeField] Button quitButton;
    protected override WindowType WindowType => WindowType.Quit;

    protected override void Start()
    {
        base.Start();
        quitButton.OnClickAsObservable().Subscribe(_ => ApplicationExtensions.QuitGame(false));        
    }
}
