using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static InputDefaults;

public class CurrentInputs : Singleton<CurrentInputs>
{
    public static KeyMap GetKeyMap(KeyAction action) => CurrentKeyMaps.Where(map => map.keyAction == action).FirstOrDefault();

    public static List<KeyMap> RemappableKeyMaps => CurrentKeyMaps.Where(map => map.isRemappable).ToList();

    public static List<KeyCode> UsedKeyCodes => CurrentKeyMaps.Select(map => map.keyCode).ToList();

    static List<KeyMap> CurrentKeyMaps
    {
        get
        {
            if (currentKeyMaps?.Any() ?? false) return currentKeyMaps;
            var keyActions = DEFAULT_KEY_MAPS.Select(map => map.keyAction);
            if (keyActions.Count() != DEFAULT_KEY_MAPS.Count()) throw new System.ArgumentOutOfRangeException("Duplicate key actions exist. Check defaults");

            foreach(var keyAction in keyActions)
            {
                var defaultKeyMap = GetDefaultKeyMap(keyAction);
                if (PlayerPrefs.HasKey(keyAction.ToString()))
                    currentKeyMaps.Add(new KeyMap(keyAction, (KeyCode)PlayerPrefs.GetInt(keyAction.ToString()), defaultKeyMap.isRemappable));
                else
                {
                    currentKeyMaps.Add(defaultKeyMap);
                    SaveKeyActionKeyCode(keyAction, defaultKeyMap.keyCode);
                }
            }
           
            return currentKeyMaps;
        }
    }

    public static KeyMap ChangeKeybinding(KeyAction keyAction, KeyCode newKeyCode)
    {
        SaveKeyActionKeyCode(keyAction, newKeyCode);
        var isRemappable = currentKeyMaps.Where(map => map.keyAction == keyAction).FirstOrDefault().isRemappable;
        currentKeyMaps.Remove(currentKeyMaps.Where(map => map.keyAction == keyAction).FirstOrDefault());
        var newMap = new KeyMap(keyAction, newKeyCode, isRemappable);
        currentKeyMaps.Add(newMap);
        return newMap;
    }

    static void SaveKeyActionKeyCode(KeyAction action, KeyCode code)
    {
        PlayerPrefs.SetInt(action.ToString(), (int)code);
        PlayerPrefs.Save();
    }

    static List<KeyMap> currentKeyMaps = new List<KeyMap>();
}
