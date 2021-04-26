using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static TabButton;

public class TabGroup : MonoBehaviour
{
    List<TabButton> tabButtons;
    public void Subscribe(TabButton tabButton)
    {
        if (tabButtons == null) tabButtons = new List<TabButton>();
        tabButtons.Add(tabButton);
    }

    public void OnTabEnter(TabButton tabButton)
    {
        ResetTabs();
        if (selectedTab == null ||selectedTab != tabButton) tabButton.SetButtonState(TabButtonState.Hover);
    }
    public void OnTabExit(TabButton tabButton) 
    {
        ResetTabs();
    }

    public void OnTabSelected(TabButton tabButton) 
    {
        selectedTab = tabButton;
        ResetTabs();
        tabButton.SetButtonState(TabButtonState.Selected);
        tabButtons.Where(aButton => aButton != selectedTab).ToList().ForEach(aButton => aButton.SetButtonState(TabButtonState.UnSelected));
    }

    void ResetTabs() => tabButtons.Where(aButton => aButton != selectedTab).ToList().ForEach(button => button.SetButtonState(TabButtonState.Idle));
    TabButton selectedTab;
}
