using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class TextBoxController : Controller<TextBoxView>
{

    [SerializeField]
    private GameEvent _sendTextToTextBoxEvent;

    private const int TextBoxDelay = 50;

    private bool _isWriting = false;

    private CancellationTokenSource _cancelationTokenSource = new CancellationTokenSource();

    private void OnEnable()
    {
        _sendTextToTextBoxEvent.Listeners += OnTextReceived;
        (_view as IClickable).RegisterClickEvent(OnClickTextBox);
    }

    private void OnDisable()
    {
        _sendTextToTextBoxEvent.Listeners -= OnTextReceived;
        (_view as IClickable).UnregisterClickEvent(OnClickTextBox);
    }

    private void OnTextReceived(GameEventType @event) 
    {
        var text = ((SendTextToTextBoxGameEvent)@event).text;
        var characterName = ((SendTextToTextBoxGameEvent)@event).characterName;
        _view.SetCharacterName(characterName);
        _view.Show();
        WriteText(text).Forget();
    }

    private async UniTask WriteText(string fullText) 
    {
        var text = string.Empty;
        var letterIndex = 0;
        _isWriting = true;
        while (text.Length < fullText.Length  && _isWriting)
        {
            text += fullText[letterIndex];
            _view.SetText(text);
            letterIndex++;
            await UniTask.Delay(TextBoxDelay);
            await UniTask.Yield();
        }
        _view.SetText(fullText);
        _isWriting = false;
        //Debug.Log(_view.GetText());
    }

    private void OnClickTextBox(ClickEvent clickEvent) 
    {
        Debug.Log($"TextBox clicked");
        _isWriting = false;
    }

}
