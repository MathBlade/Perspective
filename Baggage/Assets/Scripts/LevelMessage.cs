using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScreenMessage;

public static class LevelMessage 
{
    public class StartLevelMessage
    {
        public float SecondsUntilEndOfLevel { get; private set; }
        public StartLevelMessage(float secondsUntilEndOfLevel)
        {
            SecondsUntilEndOfLevel = secondsUntilEndOfLevel;
        }
    }

    public class EndLevelMessage
    {
        public bool WasVictorious { get; private set; }
        public DefeatReason? Reason { get; private set; }

        public EndLevelMessage(bool victory, DefeatReason? reason)
        {
            WasVictorious = victory;
            Reason = reason;
        }
    }

    public class ResetThisLevelMessage{ }
}
