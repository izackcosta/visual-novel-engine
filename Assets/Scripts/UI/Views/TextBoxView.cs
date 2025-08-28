using UnityEngine;
using UnityEngine.UIElements;

public class TextBoxView : View, IClickable
{

    private Label _textArea;

    private const string TextAreaName = "TextArea";

    protected override void Awake()
    {
        base.Awake();
        if (_textArea == null)
            _textArea = _root.Q<Label>(TextAreaName);
    }

    public void SetText(string text) => _textArea.text = text;

    public void SetTextBoxVisibility(bool visible) => _textArea.visible = visible;

    public void RegisterClickEvent(EventCallback<ClickEvent> clickEvent) 
    {
        if(_root == null)
            _root = GetComponent<UIDocument>().rootVisualElement;
        if (_textArea == null)
            _textArea = _root.Q<Label>(TextAreaName);
        _root.RegisterCallback<ClickEvent>(clickEvent);
        _textArea.RegisterCallback<ClickEvent>(clickEvent);
    }

    public void UnregisterClickEvent(EventCallback<ClickEvent> clickEvent) 
    {
        _root.UnregisterCallback<ClickEvent>(clickEvent);
        _textArea.UnregisterCallback<ClickEvent>(clickEvent);
    } 

}
