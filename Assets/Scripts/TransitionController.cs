using UnityEngine;

public class TransitionController : MonoBehaviour
{

    private Texture2D _transitionTexture;

    private SpriteRenderer _spriteRenderer;

    private Material _transitionMaterial;

    private const int SCREEN_WIDTH = 1920;
    private const int SCREEN_HEIGHT = 1080;

    private void Awake()
    {
        InitializeTransitionTexture();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _transitionMaterial = _spriteRenderer.material;
    }

    private void Start()
    {
        _spriteRenderer.sprite = Sprite.Create(_transitionTexture, new Rect(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), new Vector2(0.5f, 0.5f));
        _transitionMaterial.SetTexture("_MainTex", _transitionTexture);
    }

    private void InitializeTransitionTexture() 
    {
        _transitionTexture = new Texture2D(SCREEN_WIDTH, SCREEN_HEIGHT, TextureFormat.ARGB32, false);
        _transitionTexture.SetPixels(CreateBlackPixels());
        _transitionTexture.Apply();
    }

    private Color[] CreateBlackPixels()
    {
        var pixels = new Color[SCREEN_WIDTH * SCREEN_HEIGHT];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.black;
        }
        return pixels;
    }

}
