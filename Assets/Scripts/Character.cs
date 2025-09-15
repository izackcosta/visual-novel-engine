using UnityEngine;

public class Character : MonoBehaviour
{

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetSprite(Sprite sprite) => _spriteRenderer.sprite = sprite;

    public void InvertSprite() => _spriteRenderer.flipX = !_spriteRenderer.flipX;

}
