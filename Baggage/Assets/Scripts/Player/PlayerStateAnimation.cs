using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class PlayerStateAnimation 
{
    [SerializeField] PlayerState state;
    [SerializeField] PlayableAsset animation;

    public PlayerState PlayerState => state;
    public PlayableAsset Animation => animation;
}

public class PlayerStateAnimationCompleteMessage
{
    public PlayerState StateFinished { get; private set; }
    public PlayerStateAnimationCompleteMessage(PlayerState state)
    {
        StateFinished = state;
    }
}
