using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectSwapTabButton : TabButton
{
    [SerializeField] GameObject gameObjectToToggle;

    protected override void OnHover()
    {
        button.image.color = Color.blue;
    }

    protected override void OnIdle()
    {
        button.image.color = Color.white;
    }

    protected override void OnUnSelected()
    {
        OnIdle();
        if (gameObjectToToggle != null) gameObjectToToggle.SetActive(false);
    }

    protected override void OnSelected()
    {
        button.image.color = Color.red;
        if (gameObjectToToggle != null) gameObjectToToggle.SetActive(true);
    }
}
