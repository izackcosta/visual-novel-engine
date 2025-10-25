using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AddressableAssets;
using PrimeTween;
using System;
using System.Globalization;

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

    private bool _waitingChoice = false;

    private bool _haltSignal = false;

    private Dictionary<string, Sprite> _spriteAssets = new();

    private Dictionary<string, int> _labels = new();

    private Dictionary<string, float> _numbers = new();

    private Dictionary<string, string> _strings = new();

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
    [SerializeField]
    private GameEvent _hideTextBoxRequestEvent;
    [SerializeField]
    private GameEvent _setChoicesRequestEvent;
    [SerializeField]
    private GameEvent _makeChoiceEvent;

    //COMMANDS
    private const string WAIT_COMMAND = "wait";
    private const string SAY_COMMAND = "say";
    private const string BACKGROUND_COMMAND = "background";
    private const string CREATE_CHARACTER_COMMAND = "create-character";
    private const string FADE_OUT_SCREEN_COMMAND = "fade-out";
    private const string FADE_IN_SCREEN_COMMAND = "fade-in";
    private const string HIDE_TEXTBOX_COMMAND = "hide-text";
    private const string GOTO_COMMAND = "goto";
    private const string SET_CHOICES_COMMAND = "set-choices";
    private const string VARIABLE_COMMAND = "var";
    private const string PRINT_COMMAND = "print";
    private const string HALT_COMMAND = "halt";

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
    private const string VARIABLE_ALREADY_DEFINED_ERROR = "Variable already defined";
    private const string VARIABLE_NOT_DEFINED_ERROR = "Variable not defined";
    private readonly string INVALID_VARIABLE_NAME_ERROR = $"Invalid variable name. Variable names must start with '{VARIABLE_NUMBER_PREFIX}'(number) or '{VARIABLE_STRING_PREFIX}'(string)";

    //MISC
    private const char LABEL_PREFIX = ':';
    private const char VARIABLE_NUMBER_PREFIX = '#';
    private const char VARIABLE_STRING_PREFIX = '$';

    private void OnEnable()
    {
        _TextBoxContinueInterpretingRequest.Listeners += OnTextBoxContinueInterpretingRequest;
        _fadeCompletedEvent.Listeners += OnFadeCompleted;
        _makeChoiceEvent.Listeners += OnChoiceMade;
    }

    private void OnDisable()
    {
        _TextBoxContinueInterpretingRequest.Listeners -= OnTextBoxContinueInterpretingRequest;
        _fadeCompletedEvent.Listeners -= OnFadeCompleted;
        _makeChoiceEvent.Listeners -= OnChoiceMade;
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
        _haltSignal = false;

        await PreLoad(_script);

        while (_programCounter < _script.Count && !_haltSignal) 
        {
            await ReadNextInstruction(_script);
        }

    }

    private async UniTask ReadNextInstruction(List<string[]> script) 
    {

        var currentInstruction = script[_programCounter];

        if(currentInstruction.Length < 1) 
        {
            _programCounter++;
            return;
        }

        //WAIT
        if (currentInstruction[0] == WAIT_COMMAND)
        {
            if (currentInstruction.Length < 2)
            {
                Debug.LogError(CreateErrorLog(INVALID_ARGUMENT_NUMBER_ERROR));
                _haltSignal = true;
                return;
            }

            var value = ResolveNumber(currentInstruction[1]);
            await UniTask.Delay((int)value);
        }

        //SAY
        if (currentInstruction[0] == SAY_COMMAND)
        {
            if (currentInstruction.Length < 2)
            {
                Debug.LogError(CreateErrorLog(INVALID_ARGUMENT_NUMBER_ERROR));
                _haltSignal = true;
                return;
            }

            _waitingTextBox = true;
            
            var textKey = ResolveString(currentInstruction[1]);

            var name = currentInstruction.Length > 2 ? ResolveString(currentInstruction[2]) : null;

            _sendTextToTextBox.Invoke(new SendTextToTextBoxGameEvent((string)textKey, (string)name));
            
            await UniTask.WaitWhile(() => _waitingTextBox);

        }

        //BACKGROUNG
        if (currentInstruction[0] == BACKGROUND_COMMAND)
        {

            if (currentInstruction.Length < 2)
            {
                Debug.LogError(CreateErrorLog(INVALID_ARGUMENT_NUMBER_ERROR));
                _haltSignal = true;
                return;
            }

            var backgroundSprite = _spriteAssets[string.Format(BACKGROUNDS_PATH_FORMAT, currentInstruction[1])];

            _changeBackgroundEvent.Invoke(new SpriteGameEvent(backgroundSprite));

        }

        //CREATE CHARACTER
        if (currentInstruction[0] == CREATE_CHARACTER_COMMAND) 
        {

            if(currentInstruction.Length < 2)
            {
                Debug.LogError(CreateErrorLog(INVALID_ARGUMENT_NUMBER_ERROR));
                _haltSignal = true;
                return;
            }

            var name = ResolveString(currentInstruction[1]);

            var sprite = currentInstruction.Length > 2 ? _spriteAssets[string.Format(CHARACTERS_PATH_FORMAT, currentInstruction[2])] : null;

            var position = currentInstruction.Length > 3 ? (CharacterPosition)(int)ResolveNumber(currentInstruction[3]) : CHARACTER_DEFAULT_POSITION;

            var offsetX = currentInstruction.Length > 4 ? ResolveNumber(currentInstruction[4]) : 0;

            var offsetY = currentInstruction.Length > 5 ? ResolveNumber(currentInstruction[5]) : 0;

            var inverted = currentInstruction.Length > 6 ? ResolveBoolean(currentInstruction[6]) : false;

            _createCharacterEvent.Invoke(new CreateCharacterGameEvent(name, sprite, position, offsetX, offsetY, inverted));

        }

        //FADE IN / FADE OUT SCREEN
        if (currentInstruction[0] == FADE_IN_SCREEN_COMMAND || currentInstruction[0] == FADE_OUT_SCREEN_COMMAND)
        {

            var duration = currentInstruction.Length > 1 ? ResolveNumber(currentInstruction[1]) : FADE_DEFAULT_DURATION;

            var transitionMode = currentInstruction.Length > 2 ? (TransitionMode)(int)ResolveNumber(currentInstruction[2]) : FADE_DEFAULT_TRANSITION;

            var ease = currentInstruction.Length > 3 ? (Ease)(int)ResolveNumber(currentInstruction[3]) : FADE_DEFAULT_EASE;

            _waitingFade = true;

            var fadeEvent = currentInstruction[0] == FADE_IN_SCREEN_COMMAND ? _startFadeInEvent : _startFadeOutEvent;

            fadeEvent.Invoke(new FadeGameEvent(duration, transitionMode, ease));

            await UniTask.WaitWhile(() => _waitingFade);

        }

        //HIDE TEXTBOX
        if (currentInstruction[0] == HIDE_TEXTBOX_COMMAND)
        {
            _hideTextBoxRequestEvent.Invoke(new NoArgGameEvent());
        }

        //GOTO
        if (currentInstruction[0] == GOTO_COMMAND)
        {

            if (currentInstruction.Length < 2)
            {
                Debug.LogError(CreateErrorLog(INVALID_ARGUMENT_NUMBER_ERROR));
                return;
            }

            var line = ResolveNumber(currentInstruction[1]);

            _programCounter = (int)line;

        }

        //SET CHOICES
        if (currentInstruction[0] == SET_CHOICES_COMMAND)
        {

            if (currentInstruction.Length < 3 || currentInstruction.Length % 2 == 0)
            {
                Debug.LogError(CreateErrorLog(INVALID_ARGUMENT_NUMBER_ERROR));
                _haltSignal = true;
                return;
            }

            var choices = new List<ChoiceData>();

            for(int i = 1; i < currentInstruction.Length; i += 2) 
            {

                var choiceText = ResolveString(currentInstruction[i]);

                var choiceLabel = ResolveNumber(currentInstruction[i + 1]);

                choices.Add(new ChoiceData(choiceText, (int)choiceLabel));

            }

            _setChoicesRequestEvent.Invoke(new ChoiceArrayGameEvent(choices.ToArray()));

            _waitingChoice = true;

            await UniTask.WaitUntil(() => !_waitingChoice);

        }

        //CREATE VARIABLE
        if (currentInstruction[0] == VARIABLE_COMMAND)
        {

            if (currentInstruction.Length < 3)
            {
                Debug.LogError(CreateErrorLog(INVALID_ARGUMENT_NUMBER_ERROR));
                _haltSignal = true;
                return;
            }

            var varKey = currentInstruction[1];

            if (varKey.StartsWith(VARIABLE_NUMBER_PREFIX)) 
            {
                var varValue = ResolveNumber(currentInstruction[2]);
                if (!_numbers.ContainsKey(varKey))
                    _numbers.TryAdd(varKey, varValue);
                else
                {
                    Debug.LogError(CreateErrorLog(VARIABLE_ALREADY_DEFINED_ERROR));
                    _haltSignal = true;
                    return;
                }
            }

            else if(varKey.StartsWith(VARIABLE_STRING_PREFIX))
            {
                var varValue = ResolveString(currentInstruction[2]);
                if (!_strings.ContainsKey(varKey))
                    _strings.TryAdd(varKey, varValue);
                else
                {
                    Debug.LogError(CreateErrorLog(VARIABLE_ALREADY_DEFINED_ERROR));
                    _haltSignal = true;
                    return;
                }
            }

            else 
            {
                Debug.LogError(CreateErrorLog(INVALID_VARIABLE_NAME_ERROR));
                _haltSignal = true;
                return;
            }

        }

        //SET VARIABLE
        if (currentInstruction[0].StartsWith(VARIABLE_NUMBER_PREFIX)) 
        {

            if (currentInstruction.Length < 2)
            {
                Debug.LogError(CreateErrorLog(INVALID_ARGUMENT_NUMBER_ERROR));
                _haltSignal = true;
                return;
            }

            var varKey = currentInstruction[0];

            var varValue = ResolveNumber(currentInstruction[1]);

            if (_numbers.ContainsKey(varKey))
                _numbers[varKey] = varValue;
            else 
            {
                Debug.LogError(CreateErrorLog(VARIABLE_NOT_DEFINED_ERROR));
                _haltSignal = true;
                return;
            }

        }

        if (currentInstruction[0].StartsWith(VARIABLE_STRING_PREFIX))
        {

            if (currentInstruction.Length < 2)
            {
                Debug.LogError(CreateErrorLog(INVALID_ARGUMENT_NUMBER_ERROR));
                _haltSignal = true;
                return;
            }

            var varKey = currentInstruction[0];

            var varValue = ResolveString(currentInstruction[1]);

            if (_strings.ContainsKey(varKey))
                _strings[varKey] = varValue;
            else
            {
                Debug.LogError(CreateErrorLog(VARIABLE_NOT_DEFINED_ERROR));
                _haltSignal = true;
                return;
            }

        }

        //PRINT
        if (currentInstruction[0] == PRINT_COMMAND)
        {

            if (currentInstruction.Length < 2)
            {
                Debug.LogError(CreateErrorLog(INVALID_ARGUMENT_NUMBER_ERROR));
                _haltSignal = true;
                return;
            }

            var valueToPrint = string.Empty;

            for(int i= 1; i < currentInstruction.Length; i++) 
            {
                valueToPrint += GetPrintable(currentInstruction[i]) + ' ';
            }

            Debug.Log(valueToPrint);

        }

        //HALT
        if (currentInstruction[0] == HALT_COMMAND)
        {
            _haltSignal = true;
            return;
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

    private void OnChoiceMade(GameEventType eventType) 
    {
        var e = (IntegerGameEvent)eventType;
        _programCounter = e.Value;
        _waitingChoice = false;
    }

    private async UniTask PreLoad(List<string[]> script) 
    {

        foreach (string[] line in script) 
        {

            if (line.Length < 1)
                continue;

            //GET LABELS
            if (line[0].Length > 1 && line[0].StartsWith(LABEL_PREFIX))
                _labels.Add(line[0], script.IndexOf(line));

            //GET CHARACTER SPRITES
            if (line[0] == CREATE_CHARACTER_COMMAND)
            {

                if (line.Length > 2)
                {
                    var path = string.Format(CHARACTERS_PATH_FORMAT, line[2]);
                    var sprite = await Addressables.LoadAssetAsync<Sprite>(path);
                    _spriteAssets.Add(path, sprite);
                }

            }

            //GET BACKGROUND SPRITES
            if (line[0] == BACKGROUND_COMMAND)
            {

                if (line.Length > 1)
                {
                    var path = string.Format(BACKGROUNDS_PATH_FORMAT, line[1]);
                    var sprite = await Addressables.LoadAssetAsync<Sprite>(path);
                    _spriteAssets.Add(path, sprite);
                }

            }

        }

    }

    private float ResolveNumber(string value)
    {

        if (float.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var f)) 
            return f;

        if(_numbers.ContainsKey(value))
            return _numbers[value];

        if(_strings.ContainsKey(value) && _labels.ContainsKey(_strings[value]))
            return _labels[_strings[value]];

        if (_labels.ContainsKey(value))
            return _labels[value];

        _haltSignal = true;
        throw new Exception(CreateErrorLog(INVALID_ARGUMENT_ERROR));

        return 0;

    }

    private string ResolveString(string value) 
    {

        if (_strings.ContainsKey(value))
            return _strings[value];

        return value;

    }

    private bool ResolveBoolean(string value) => ResolveNumber(value) > 0;

    private string GetPrintable(string parameter) 
    {

        if (_numbers.ContainsKey(parameter))
            return _numbers[parameter].ToString();

        if (_strings.ContainsKey(parameter))
            return _strings[parameter];

        if (_labels.ContainsKey(parameter))
            return _labels[parameter].ToString();

        return parameter;

    }

}
