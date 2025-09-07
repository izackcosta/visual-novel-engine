using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Localization.Settings;
using System.Linq;

public class TextBoxView : View, IClickable
{

    private Label _textArea;

    private const string TextAreaName = "TextArea";

    private Label _nameBox;

    private const string NameBoxName = "NameBox";

    private string _fullText;

    private int _visibleCharacters = 0;

    private const string InvisibleCharacterFormat = "<color=#00000000>{0}</color>";

    protected override void Awake()
    {
        base.Awake();
        _textArea = _root.Q<Label>(TextAreaName);
        _nameBox = _root.Q<Label>(NameBoxName);
    }

    public void SetText(string fullText) 
    {
        _fullText = fullText;
        _visibleCharacters = 0;
        _textArea.text = GenerateText();
    }

    public void SetCharacterName(string characterName) => _nameBox.text = characterName;

    private void ClearCharacterName() => _nameBox.text = string.Empty;

    public override void Show()
    {
        _textArea.visible = true;
        _nameBox.visible = _nameBox.text != string.Empty;
    }

    public void IncrementVisibleCharacters(int amount = 1) 
    {
        _visibleCharacters = Mathf.Min(_visibleCharacters + amount, _fullText.Length);
        _textArea.text = GenerateText();
    }

    public void RevealEntireText() 
    {
        _visibleCharacters = _fullText.Length;
        _textArea.text = GenerateText();
    }

    private string GenerateText()
    {
        var finalText = string.Empty;
        for(int i = 0; i < _fullText.Length; i++)
        {
            finalText += i < _visibleCharacters ? _fullText[i] : string.Format(InvisibleCharacterFormat, _fullText[i]);
        }
        return finalText;
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
