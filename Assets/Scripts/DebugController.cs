using UnityEngine;

public class DebugController : MonoBehaviour
{

#if UNITY_EDITOR

    [SerializeField]
    private GameEvent _sendTextToTextBoxEvent;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            var text = $"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam at neque a purus viverra laoreet nec sit amet nisl." +
                $" Duis placerat facilisis fringilla. Quisque volutpat suscipit metus, sed pretium urna auctor in.";
            _sendTextToTextBoxEvent.Invoke(new StringGameEvent(text));
        }
    }

#endif

}
