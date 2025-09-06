using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VNSInterpreter : MonoBehaviour
{

    [Header("Script")]
    [SerializeField]
    private VNSAsset _entryScript;
    [SerializeField]
    private bool _autoStart = true;

    [Header("Game Events")]
    [SerializeField]
    private GameEvent _sendTextToTextBox;
    [SerializeField]
    private GameEvent _TextBoxContinueInterpretingRequest;

    private List<string[]> _script;

    private int _programCounter = 0;

    private bool _waitingTextBox = false;

    private const string WAIT_COMMAND = "wait";

    private const string SAY_COMMAND = "say";

    private const string ERROR_FORMAT = "Error at line {0}: {1}";

    private const string INVALID_ARGUMENT_ERROR = "Invalid argument";

    private void OnEnable()
    {
        _TextBoxContinueInterpretingRequest.Listeners +=OnTextBoxContinueInterpretingRequest;
    }

    private void OnDisable()
    {
        _TextBoxContinueInterpretingRequest.Listeners -= OnTextBoxContinueInterpretingRequest;
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
            _waitingTextBox = true;
            _sendTextToTextBox.Invoke(new SendTextToTextBoxGameEvent(currentInstruction[1], "Maria"));
            await UniTask.WaitWhile(() => _waitingTextBox);
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

}
