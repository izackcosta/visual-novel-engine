using UnityEngine;
using UnityEngine.UIElements;

public class TextBoxView : View, IClickable
{

    private Label _textArea;

    private const string TextAreaName = "TextArea";

    private Label _nameBox;

    private const string NameBoxName = "NameBox";

    protected override void Awake()
    {
        base.Awake();
        _textArea = _root.Q<Label>(TextAreaName);
        _nameBox = _root.Q<Label>(NameBoxName);
    }

    public void SetText(string text) => _textArea.text = text;

    public string GetText() => _textArea.text;

    public void SetCharacterName(string characterName) => _nameBox.text = characterName;

    private void ClearCharacterName() => _nameBox.text = string.Empty;

    public override void Show()
    {
        _textArea.visible = true;
        _nameBox.visible = _nameBox.text != string.Empty;
    }

    public override void Hide()
    {
        _textArea.visible = false;
        _nameBox.visible = false;
        ClearCharacterName();
    }

    public void RegisterClickEvent(EventCallback<ClickEvent> clickEvent) 
    {
        if(_root == null)
            _root = GetComponent<UIDocument>().rootVisualElement;
        _root.RegisterCallback<ClickEvent>(clickEvent);
    }

    public void UnregisterClickEvent(EventCallback<ClickEvent> clickEvent) 
    {
        _root.UnregisterCallback<ClickEvent>(clickEvent);
    } 

}
