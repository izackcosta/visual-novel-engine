using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AddressableAssets;
using PrimeTween;

public class VNSInterpreter : MonoBehaviour
{

    [Header("Script")]
    [SerializeField]
    private VNSAsset _entryScript;
    [SerializeField]
    private bool _autoStart = true;

    private List<string[]> _script;

    private int _programCounter = 0;

    private bool _waitingTextBox = false;

    private bool _waitingFade = false;

    [Header("Game Events")]
    [SerializeField]
    private GameEvent _sendTextToTextBox;
    [SerializeField]
    private GameEvent _TextBoxContinueInterpretingRequest;
    [SerializeField]
    private GameEvent _changeBackgroundEvent;
    [SerializeField]
    private GameEvent _createCharacterEvent;
    [SerializeField]
    private GameEvent _startFadeOutEvent;
    [SerializeField]
    private GameEvent _startFadeInEvent;
    [SerializeField]
    private GameEvent _fadeCompletedEvent;

    //COMMANDS
    private const string WAIT_COMMAND = "wait";
    private const string SAY_COMMAND = "say";
    private const string BACKGROUND_COMMAND = "background";
    private const string CREATE_CHARACTER_COMMAND = "create-character";
    private const string FADE_OUT_SCREEN_COMMAND = "fade-out";
    private const string FADE_IN_SCREEN_COMMAND = "fade-in";

    //DEFAULTS
    private const CharacterPosition CHARACTER_DEFAULT_POSITION = CharacterPosition.Middle;
    private const float FADE_DEFAULT_DURATION = 1f;
    private const TransitionMode FADE_DEFAULT_TRANSITION = TransitionMode.Normal;
    private const Ease FADE_DEFAULT_EASE = Ease.Linear;

    //PATHS
    private const string BACKGROUNDS_PATH_FORMAT = "Assets/Images/Backgrounds/{0}.png";
    private const string CHARACTERS_PATH_FORMAT = "Assets/Images/Characters/{0}.png";

    //ERRORS
    private const string ERROR_FORMAT = "Error at line {0}: {1}";
    private const string INVALID_ARGUMENT_ERROR = "Invalid argument";
    private const string INVALID_ARGUMENT_NUMBER_ERROR = "Invalid number of arguments";

    private void OnEnable()
    {
        _TextBoxContinueInterpretingRequest.Listeners += OnTextBoxContinueInterpretingRequest;
        _fadeCompletedEvent.Listeners += OnFadeCompleted;
    }

    private void OnDisable()
    {
        _TextBoxContinueInterpretingRequest.Listeners -= OnTextBoxContinueInterpretingRequest;
        _fadeCompletedEvent.Listeners -= OnFadeCompleted;
    }

    private void Start()
    {
        if (_autoStart)
            Interpret(_entryScript).Forget();
    }

    private async UniTask Interpret(VNSAsset script) 
    {

        _script = CreateScript(script);
        _programCounter = 0;

        while(_programCounter < _script.Count) 
        {
            await ReadNextInstruction(_script);
        }

    }

    private async UniTask ReadNextInstruction(List<string[]> script) 
    {

        var currentInstruction = script[_programCounter];

        //WAIT
        if (currentInstruction[0] == WAIT_COMMAND)
        {
            if (currentInstruction.Length < 2)
            {
                Debug.LogError(CreateErrorLog(INVALID_ARGUMENT_NUMBER_ERROR));
                return;
            }
            if (!int.TryParse(currentInstruction[1], out int waitTime))
            {
                Debug.LogError(CreateErrorLog(INVALID_ARGUMENT_ERROR));
                return;
            }
            Debug.Log($"waiting {waitTime / 1000} seconds");
            await UniTask.Delay(waitTime);
        }

        //SAY
        if (currentInstruction[0] == SAY_COMMAND)
        {
            if (currentInstruction.Length < 2)
            {
                Debug.LogError(CreateErrorLog(INVALID_ARGUMENT_NUMBER_ERROR));
                return;
            }
            _waitingTextBox = true;
            var name = currentInstruction.Length > 2 ? currentInstruction[2] : null;
            _sendTextToTextBox.Invoke(new SendTextToTextBoxGameEvent(currentInstruction[1], name));
            await UniTask.WaitWhile(() => _waitingTextBox);
        }

        //BACKGROUNG
        if (currentInstruction[0] == BACKGROUND_COMMAND)
        {
            if (currentInstruction.Length < 2)
            {
                Debug.LogError(CreateErrorLog(INVALID_ARGUMENT_NUMBER_ERROR));
                return;
            }
            var backgroundSprite = await Addressables.LoadAssetAsync<Sprite>(string.Format(BACKGROUNDS_PATH_FORMAT, currentInstruction[1]));
            _changeBackgroundEvent.Invoke(new SpriteGameEvent(backgroundSprite));
        }

        //CREATE CHARACTER
        if (currentInstruction[0] == CREATE_CHARACTER_COMMAND) 
        {

            if(currentInstruction.Length < 2)
            {
                Debug.LogError(CreateErrorLog(INVALID_ARGUMENT_NUMBER_ERROR));
                return;
            }

            var name = currentInstruction[1];

            var sprite = currentInstruction.Length > 2 ? await Addressables.LoadAssetAsync<Sprite>(string.Format(CHARACTERS_PATH_FORMAT, currentInstruction[2])) : null;

            var position = currentInstruction.Length > 3 && int.TryParse(currentInstruction[3], out int pos) ? (CharacterPosition)pos : CHARACTER_DEFAULT_POSITION;

            var offsetX = currentInstruction.Length > 4 && float.TryParse(currentInstruction[4], out float x) ? x : 0;

            var offsetY = currentInstruction.Length > 5 && float.TryParse(currentInstruction[5], out float y) ? y : 0;

            var inverted = currentInstruction.Length > 6 && int.TryParse(currentInstruction[6], out int inv) ? inv > 0 : false;

            _createCharacterEvent.Invoke(new CreateCharacterGameEvent(name, sprite, position, offsetX, offsetY, inverted));

        }

        //FADE IN / FADE OUT SCREEN
        if (currentInstruction[0] == FADE_IN_SCREEN_COMMAND || currentInstruction[0] == FADE_OUT_SCREEN_COMMAND)
        {

            var duration = currentInstruction.Length > 1 && float.TryParse(currentInstruction[1], out float dur) ? dur : FADE_DEFAULT_DURATION;

            var transitionMode = currentInstruction.Length > 2 && int.TryParse(currentInstruction[2], out int mode) ? (TransitionMode)mode : FADE_DEFAULT_TRANSITION;

            var ease = currentInstruction.Length > 3 && int.TryParse(currentInstruction[3], out int easeInt) ? (Ease)easeInt : FADE_DEFAULT_EASE;

            _waitingFade = true;

            var fadeEvent = currentInstruction[0] == FADE_IN_SCREEN_COMMAND ? _startFadeInEvent : _startFadeOutEvent;

            fadeEvent.Invoke(new FadeGameEvent(duration, transitionMode, ease));

            await UniTask.WaitWhile(() => _waitingFade);

        }

        _programCounter++;

    }

    private string CreateErrorLog(string error) => string.Format(ERROR_FORMAT, _programCounter, error);

    private List<string[]> CreateScript(VNSAsset script) => new List<string[]>(script.Instructions
        .Select(line => line.Split(' ')
        .Where(x => x != string.Empty)
        .ToArray()));

    private void OnTextBoxContinueInterpretingRequest(GameEventType eventType)
    {
        if(_waitingTextBox)
            _waitingTextBox = false;
    }

    private void OnFadeCompleted(GameEventType eventType) 
    {
        if(_waitingFade)
            _waitingFade = false;
    }

}
