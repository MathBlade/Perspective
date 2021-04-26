using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ScreenMessage;

public static class ApplicationExtensions 
{
    const char LIST_SEPARATOR = ';';
    const char KEY_SEPARATOR = ':';
    const string ALL_PROFILES_KEY = "AllProfiles";
    const string CURRENT_PLAYER_PROFILE_KEY = "CurrentProfile";
    
    const int MAIN_MENU_SCENE = 0;
    const int NUM_TECHNICAL_SCENES = 1;

    //Player specific keys
    static readonly string[] PLAYER_SPECIFIC_KEYS = new string[] { LAST_COMPLETED_LEVEL, DEATH_COUNT_KEY, DEATH_REASONS_KEY };
    const string LAST_COMPLETED_LEVEL = "LastCompletedLevel";
    const string DEATH_COUNT_KEY = "DeathCountKey";
    const string DEATH_REASONS_KEY = "DeathReasonsKey";

    public static bool HasNextLevel => SceneManager.sceneCountInBuildSettings - NUM_TECHNICAL_SCENES > currentSceneLoaded;

    public static bool HasActivePlayer => PlayerPrefs.HasKey(CURRENT_PLAYER_PROFILE_KEY);

    public static void SaveVictory()
    {
        string key = currentPlayerPrefName + KEY_SEPARATOR + LAST_COMPLETED_LEVEL;
        IfHasKeyIncrementElseOne(key);
    }

    public static void SaveDefeat(DefeatReason reason)
    {
        string key = currentPlayerPrefName + KEY_SEPARATOR + DEATH_COUNT_KEY;
        IfHasKeyIncrementElseOne(key);
        string deathReasonsKey = currentPlayerPrefName + KEY_SEPARATOR + DEATH_REASONS_KEY;
        if (!PlayerPrefs.HasKey(deathReasonsKey)) PlayerPrefs.SetString(deathReasonsKey, reason.ToString());
        else PlayerPrefs.SetString(deathReasonsKey, PlayerPrefs.GetString(deathReasonsKey) + LIST_SEPARATOR + reason.ToString());
    }

    public static int GetLevelSaved(string profileName)
    {
        if (PlayerPrefs.HasKey(profileName + KEY_SEPARATOR + LAST_COMPLETED_LEVEL)) return PlayerPrefs.GetInt(profileName + KEY_SEPARATOR + LAST_COMPLETED_LEVEL);
        else return 0;
    }

    public static void DeleteProfile(string thisProfileName)
    {
        var profileNames = new List<String>(ProfileNames);
        if (currentPlayerPrefName == thisProfileName) PlayerPrefs.DeleteKey(CURRENT_PLAYER_PROFILE_KEY);
        foreach (var key in PLAYER_SPECIFIC_KEYS)
        {
            string keyName = thisProfileName + KEY_SEPARATOR + key;
            if (PlayerPrefs.HasKey(keyName)) PlayerPrefs.DeleteKey(keyName);
        }

        profileNames.RemoveAll(name => name.Equals(thisProfileName));
        PlayerPrefs.SetString(ALL_PROFILES_KEY, string.Join(LIST_SEPARATOR.ToString(), profileNames.ToArray()));
    }

    public static void LoadProfile(string thisProfileName)
    {
        PlayerPrefs.SetString(CURRENT_PLAYER_PROFILE_KEY, thisProfileName);
        ContinueGame();
    }

    public static void ContinueGame()
    {
        int sceneToLoad = PlayerPrefs.GetInt(currentPlayerPrefName + KEY_SEPARATOR + LAST_COMPLETED_LEVEL) + 1;
        LoadScene(sceneToLoad);
    }

    public static void LoadNextLevel()
    {
        if (!HasNextLevel) return;
        LoadScene(currentSceneLoaded + 1);
    }

    public static void BackToMainMenu() => LoadScene(MAIN_MENU_SCENE);

    public static void QuitGame(bool showConfirmationPrompt)
    {
        if (showConfirmationPrompt)
        {
            MessageBroker.Default.Publish(new ShowWindowIntent(WindowType.Quit));
            return;
        }

#if UNITY_EDITOR
        if (Application.isPlaying) UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    public static int CurrentLevel => currentSceneLoaded;

    static int DeathsByReason(string profileName, DefeatReason reason)
    {
        string deathReasonsKey = currentPlayerPrefName + KEY_SEPARATOR + DEATH_REASONS_KEY;
        if (!PlayerPrefs.HasKey(deathReasonsKey)) return 0;
        return PlayerPrefs.GetString(deathReasonsKey).Split(LIST_SEPARATOR).Count(str => str.Equals(reason.ToString()));
    }

    static void IfHasKeyIncrementElseOne(string key)
    {
        if (PlayerPrefs.HasKey(key)) PlayerPrefs.SetInt(key, PlayerPrefs.GetInt(key) + 1);
        else PlayerPrefs.SetInt(key, 1);
    }

    static void LoadScene(int sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    static List<string> ProfileNames
    {
        get
        {
            if (!PlayerPrefs.HasKey(ALL_PROFILES_KEY)) return new List<string>();
            return PlayerPrefs.GetString(ALL_PROFILES_KEY).Split(LIST_SEPARATOR).ToList();
        }
    }

    public static List<string> ProfileNamesWithLevelCanBeBeat
    {
        get
        {

            return ProfileNames.Where(p => HasLevelToBeat(p)).ToList();
        }
    }

    static bool HasLevelToBeat(string profileName)
    {
        string key = profileName + KEY_SEPARATOR + LAST_COMPLETED_LEVEL;
        if (!PlayerPrefs.HasKey(key)) return true;

        var lastLevelBeat = PlayerPrefs.GetInt(key);

        return SceneManager.sceneCountInBuildSettings - NUM_TECHNICAL_SCENES > lastLevelBeat;
    }

    public static void CreateNewProfile(string text)
    {
        PlayerPrefs.SetString(CURRENT_PLAYER_PROFILE_KEY, text);
        if (!PlayerPrefs.HasKey(ALL_PROFILES_KEY)) PlayerPrefs.SetString(ALL_PROFILES_KEY, text);
        else PlayerPrefs.SetString(ALL_PROFILES_KEY, PlayerPrefs.GetString(ALL_PROFILES_KEY) + LIST_SEPARATOR + text);

        PlayerPrefs.SetInt(currentPlayerPrefName + KEY_SEPARATOR + LAST_COMPLETED_LEVEL, 0);
    }

    public static bool ProfileExists(string text)
    {
        if (!PlayerPrefs.HasKey(ALL_PROFILES_KEY)) return false;
        var allNames = PlayerPrefs.GetString(ALL_PROFILES_KEY).Split(LIST_SEPARATOR);
        return allNames.Any(name => name.Equals(text));
    }

    static int currentSceneLoaded => SceneManager.GetActiveScene().buildIndex;

    static string currentPlayerPrefName => PlayerPrefs.GetString(CURRENT_PLAYER_PROFILE_KEY);
}
