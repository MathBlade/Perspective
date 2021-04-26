using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class ActionKeyCodeCombo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI actionText;
    [SerializeField] TextMeshProUGUI keyCodeText;
    [SerializeField] Button keyCodeButton;

    private void Start()
    {
        foreach(KeyCode keycode in System.Enum.GetValues(typeof(KeyCode)))
            this.UpdateAsObservable()
                .Where(_ => rebindingAllowed && Input.GetKeyDown(keycode))
                .Subscribe(_ => AttemptRebind(keycode))
                .AddTo(this);
    }

    public void Initialize(KeyMap mapInput)
    {
        map = mapInput;
        actionText.text = map.keyAction.ToString();
        keyCodeText.text = map.keyCode.ToString();
        keyCodeButton.OnClickAsObservable().Subscribe(_ => SetKeyForRebinding());
    }

    public void DisableRebinding()
    {
        keyCodeText.text = map.keyCode.ToString();
        rebindingAllowed = false;
    }

    void SetKeyForRebinding()
    {
        rebindingAllowed = true;
        MessageBroker.Default.Publish(this);
        keyCodeText.text = "???";
    }

    void AttemptRebind(KeyCode keycode)
    {
        var unavailableKeys = CurrentInputs.UsedKeyCodes.Where(code => code != map.keyCode);
        if (unavailableKeys.Contains(keycode))
            Debug.LogError("No can do");
        else
        {
            rebindingAllowed = false;
            map = CurrentInputs.ChangeKeybinding(map.keyAction, keycode);
            keyCodeText.text = map.keyCode.ToString();
        }

    }


    KeyMap map;
    bool rebindingAllowed;
}
