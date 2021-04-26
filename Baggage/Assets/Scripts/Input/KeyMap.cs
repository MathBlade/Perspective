using UnityEngine;
using static InputDefaults;

[System.Serializable]
public class KeyMap 
{
    public KeyAction keyAction { get; private set; }
    public KeyCode keyCode { get; private set; }

    public bool isRemappable { get; private set; }

    public KeyMap(KeyAction keyActionInput, KeyCode keyCodeInput, bool isRemappableInput = true)
    {
        this.keyCode = keyCodeInput;
        this.keyAction = keyActionInput;
        this.isRemappable = isRemappableInput;
    }
}
