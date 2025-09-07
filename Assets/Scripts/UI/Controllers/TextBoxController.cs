using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;

public class TextBoxController : Controller<TextBoxView>
{

    [SerializeField]
    private GameEvent _sendTextToTextBoxEvent;

    [SerializeField]
    private GameEvent _TextBoxContinueInterpretingRequest;

    private const int TextBoxDelay = 10;

    private bool _isWriting = false;

    private CancellationTokenSource _cancelationTokenSource = new CancellationTokenSource();

    private const string DialogueLocalizationTableName = "Dialogues";

    private const string NameLocalizationTableName = "Names";

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

        var textKey = ((SendTextToTextBoxGameEvent)@event).textKey;
        var fullText = LocalizationSettings.StringDatabase.GetLocalizedString(DialogueLocalizationTableName, textKey);
        
        var characterNameKey = ((SendTextToTextBoxGameEvent)@event).characterNameKey;
        var characterName = characterNameKey != null ? LocalizationSettings.StringDatabase.GetLocalizedString(NameLocalizationTableName, characterNameKey) : string.Empty;
        _view.SetCharacterName(characterName);

        _view.Show();
        
        WriteText(fullText).Forget();

    }

    private async UniTask WriteText(string fullText) 
    {
        var letterIndex = 0;
        _isWriting = true;
        _view.SetText(fullText);
        while (letterIndex < fullText.Length  && _isWriting)
        {
            _view.IncrementVisibleCharacters();
            letterIndex++;
            await UniTask.Delay(TextBoxDelay);
            await UniTask.Yield();
        }
        _view.RevealEntireText();
        _isWriting = false;
    }

    private void OnClickTextBox(ClickEvent clickEvent) 
    {
        if (_isWriting)
            _isWriting = false;
        else
            _TextBoxContinueInterpretingRequest.Invoke(new NoArgGameEvent());
    }

}
