using UnityEngine;

public class DebugController : MonoBehaviour
{

    [SerializeField]
    private GameEvent _sendTextToTextBoxEvent;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            var text = $"Text sent to TextBox";
            _sendTextToTextBoxEvent.Invoke(new StringGameEvent(text));
        }
    }

}
