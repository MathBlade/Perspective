using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[RequireComponent(typeof(PlayableDirector))]
public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] PlayerStateAnimation[] animationsByState = new PlayerStateAnimation[System.Enum.GetValues(typeof(PlayerState)).Length];
    void Awake()
    {
        playableDirector = GetComponent<PlayableDirector>();
    }

    void Start()
    {
        MessageBroker.Default.Receive<PlayerState>().Subscribe(SetState).AddTo(this);
        this.UpdateAsObservable()
            .Where(_ => activeRenderer != null && activeRenderer.isVisible == true)
            .Subscribe(_ => hasInitialized.Value = true)
            .AddTo(this);
    }

    public IReadOnlyReactiveProperty<bool> HasInitialized => hasInitialized;
    public bool IsVisible => HasInitialized.Value && activeRenderer != null && activeRenderer.isVisible;

    void SetState(PlayerState state)
    {
        if (animationsByState == null || !animationsByState.Where(a => a.PlayerState == state).Any()) return;

        var animationState = animationsByState.Where(a => a.PlayerState == state).FirstOrDefault();
        if (animationState.Animation == null) return;

        if (animationState.Animation == currentAnimation) return;

        if (currentAnimation != null)
            foreach (var binding in currentAnimation.outputs.Where(b => b.streamName.Contains("Activation")))
            {
                    var gameObject = (GameObject)playableDirector.GetGenericBinding(binding.sourceObject);
                    hasInitialized.Value = false;
                    gameObject.SetActive(false);
            }

        playableDirector.playableAsset = animationState.Animation;
        currentAnimation = animationState.Animation;

        if (currentAnimation != null)
            foreach (var binding in currentAnimation.outputs.Where(b => b.streamName.Contains("Activation")))
            {
                var gameObject = (GameObject)playableDirector.GetGenericBinding(binding.sourceObject);
                activeRenderer = gameObject.GetComponent<Renderer>();
            }

        playableDirector.Play();
    }

    Renderer activeRenderer;
    PlayableAsset currentAnimation;
    PlayableDirector playableDirector;
    ReactiveProperty<bool> hasInitialized = new ReactiveProperty<bool>();
}
