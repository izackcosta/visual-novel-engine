using UnityEngine;
using UnityEngine.UIElements;

public class TextBoxController : MonoBehaviour
{

    [SerializeField]
    private GameEvent _sendTextToTextBoxEvent;

    private string _currentText = string.Empty;

    private TextBoxView _view;

    private void OnEnable()
    {
        _sendTextToTextBoxEvent.Listeners += OnTextReceived;
    }

    private void OnDisable()
    {
        _sendTextToTextBoxEvent.Listeners -= OnTextReceived;
    }

    private void Awake()
    {
        _view = GetComponent<TextBoxView>();
    }

    private void OnTextReceived(GameEventType @event) 
    {
        var text = ((StringGameEvent)@event).Value;
        _currentText = text;
        _view.SetText(_currentText);
    }

}
