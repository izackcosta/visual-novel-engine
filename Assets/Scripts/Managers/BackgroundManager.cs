using UnityEngine;

public class BackgroundManager : MonoBehaviour
{

    private SpriteRenderer _spriteRenderer;

    [SerializeField]
    private GameEvent _changeBackgroundEvent;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        _changeBackgroundEvent.Listeners += OnChangeBackgroundRequested;
    }

    private void OnDisable()
    {
        _changeBackgroundEvent.Listeners -= OnChangeBackgroundRequested;
    }

    public void SetBackground(Sprite sprite) => _spriteRenderer.sprite = sprite;

    private void OnChangeBackgroundRequested(GameEventType @event) 
    {
        var sprite = ((SpriteGameEvent)@event).Value;
        SetBackground(sprite);
    }

}
