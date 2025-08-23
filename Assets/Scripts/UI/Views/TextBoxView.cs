using UnityEngine;
using UnityEngine.UIElements;

public class TextBoxView : MonoBehaviour
{

    private UIDocument _textBox;

    private VisualElement _root;

    private Label _textArea;

    private const string TextAreaName = "TextArea";

    private void Awake()
    {
        _textBox = GetComponent<UIDocument>();
        _root = _textBox.rootVisualElement;
        _textArea = _root.Q<Label>(TextAreaName);
    }

    public void SetText(string text) => _textArea.text = text;

}
