using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class TabButton : MonoBehaviour
{
    public TabGroup tabGroup;

    public enum TabButtonState
    {
        Hover,
        Idle,
        Selected,
        UnSelected
    }

    void Start()
    {
        tabGroup.Subscribe(this);
        button = GetComponent<Button>();
        button.OnClickAsObservable().Subscribe(_ => tabGroup.OnTabSelected(this));
        button.OnPointerEnterAsObservable().Subscribe(_ => tabGroup.OnTabEnter(this));
        button.OnPointerExitAsObservable().Subscribe(_ => tabGroup.OnTabExit(this));
    }

    public void SetButtonState(TabButtonState state)
    {
        switch(state)
        {
            case TabButtonState.Hover:
                OnHover();
                break;
            case TabButtonState.Selected:
                OnSelected();
                break;
            case TabButtonState.UnSelected:
                OnUnSelected();
                break;
            case TabButtonState.Idle:
            default:
                OnIdle();
                break;
        }
    }

    protected abstract void OnUnSelected();
    protected abstract void OnHover();
    protected abstract void OnSelected();
    protected abstract void OnIdle();

    protected Button button;
}
