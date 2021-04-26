using UniRx;
using UnityEngine;

public class LandedAnimation : MonoBehaviour
{
    //This is used by Unity Animation events. Do not discard. Unity does not detect this as a reference.
    void LandedAnimationComplete() => MessageBroker.Default.Publish(new PlayerStateAnimationCompleteMessage(PlayerState.Land));
}
