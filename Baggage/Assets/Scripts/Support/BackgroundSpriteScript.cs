using UniRx;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundSpriteScript : MonoBehaviour
{
    [SerializeField] FloatReactiveProperty _backgroundScaleMultiplier = new FloatReactiveProperty(1f);
    private void Awake() => spriteRenderer = GetComponent<SpriteRenderer>();
    
    void Start()
    {
        ResizeBackground();
        backgroundScaleMultiplier.Subscribe(_ => ResizeBackground());
        
    }

    void ResizeBackground()
    {
        float cameraHeight = Camera.main.orthographicSize * 2; //This is * 2 as the camera is half size in orthographic mode;
        Vector2 cameraSize = new Vector2(Camera.main.aspect * cameraHeight, cameraHeight); //Aspect is width/height so width/height*height = width;
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;


        if (CameraIsLandscape(cameraSize)) { ResizeBasedOnDimension(cameraSize, spriteSize, Axis.X); }
        else ResizeBasedOnDimension(cameraSize, spriteSize, Axis.Y);

        transform.position = Vector2.zero;
    }

    Vector3 ResizeBasedOnDimension(Vector2 camSize, Vector2 spriteSize, Axis axis)
    {
        int index = (int)axis;
        Vector2 scale = transform.localScale;
        return transform.localScale = scale *= (camSize[index] / spriteSize[index] / scale[index]) * backgroundScaleMultiplier.Value;
    }

    bool CameraIsLandscape(Vector2 camSize) => camSize.x >= camSize.y;

    static readonly Vector2 screen = new Vector2(Screen.width, Screen.height);
    IReadOnlyReactiveProperty<float> backgroundScaleMultiplier => _backgroundScaleMultiplier;
    SpriteRenderer spriteRenderer;

    enum Axis
    {
        X = 0,
        Y = 1
    }
}