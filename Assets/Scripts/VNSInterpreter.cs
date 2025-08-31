using Cysharp.Threading.Tasks;
using UnityEngine;

public class VNSInterpreter : MonoBehaviour
{

    [SerializeField]
    private VNSAsset _entryScript;

    [SerializeField]
    private bool _autoStart = true;

    private int _programCounter = 0;

    private const string ERROR_FORMAT = "Error at line {0}: {1}";

    private const string INVALID_ARGUMENT_ERROR = "Invalid argument";

    private const string WAIT_COMMAND = "wait";

    private void Start()
    {
        if (_autoStart)
            Interpret(_entryScript).Forget();
    }

    private async UniTask Interpret(VNSAsset script) 
    {

        if (_programCounter >= script.Instructions.Length)
            return;

        var currentLine = script.Instructions[_programCounter];

        //WAIT
        if (currentLine[0] == WAIT_COMMAND) 
        {
            if(!int.TryParse(currentLine[1], out int waitTime)) 
            {
                Debug.LogError(CreateErrorLog(INVALID_ARGUMENT_ERROR));
                return;
            }
            await UniTask.Delay(waitTime);
        }

        _programCounter++;
        Interpret(script).Forget();

    }

    private string CreateErrorLog(string error) => string.Format(ERROR_FORMAT, _programCounter, error);

}
