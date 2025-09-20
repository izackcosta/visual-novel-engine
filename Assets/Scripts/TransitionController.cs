using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;

public enum TransitionMode 
{
    Normal,
    Shutters
}

public class TransitionController : MonoBehaviour
{

    private Texture2D _transitionTexture;

    private SpriteRenderer _spriteRenderer;

    private Material _transitionMaterial;

    private const int SCREEN_WIDTH = 1920;
    private const int SCREEN_HEIGHT = 1080;
    private const string FADE_AMOUNT_PROPERTY_NAME = "_FadeAmount";

    private readonly Dictionary<TransitionMode, string> _transitionModes = new Dictionary<TransitionMode, string>()
    {
        { TransitionMode.Normal, "_TRANSITIONMODE_NORMAL" },
        { TransitionMode.Shutters, "_TRANSITIONMODE_SHUTTERS" }
    };

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
        StartFadeOut(2, TransitionMode.Shutters, Ease.OutQuad);
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

    public void StartFadeIn(float duration, TransitionMode mode, Ease ease) => FadeIn(duration, mode, ease).Forget();

    private async UniTask FadeIn(float duration, TransitionMode mode, Ease ease)
    {
        await Tween.Custom(1, 0, duration, value => _transitionMaterial.SetFloat(FADE_AMOUNT_PROPERTY_NAME, value), ease);
    }

    public void StartFadeOut(float duration, TransitionMode mode, Ease ease) => FadeOut(duration, mode, ease).Forget();

    private async UniTask FadeOut(float duration, TransitionMode mode, Ease ease)
    {
        SetTransitionMode(mode);
        await Tween.Custom(0, 1, duration, value => _transitionMaterial.SetFloat(FADE_AMOUNT_PROPERTY_NAME, value), ease);
    }

    private void SetTransitionMode(TransitionMode mode)
    {
        foreach (var kvp in _transitionModes)
        {
            if (kvp.Key == mode)
            {
                _transitionMaterial.EnableKeyword(kvp.Value);
            }
            else
            {
                _transitionMaterial.DisableKeyword(kvp.Value);
            }
        }
    }

}
