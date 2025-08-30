using UnityEngine;

public class DebugController : MonoBehaviour
{

#if UNITY_EDITOR

    [SerializeField]
    private GameEvent _sendTextToTextBoxEvent;

    [SerializeField]
    private GameEvent _changeBackgroundEvent;

    [SerializeField]
    private Sprite _newBackground;

    private void Update()
    {

        if(Input.GetKeyDown(KeyCode.D))
        {
            var text = $"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam at neque a purus viverra laoreet nec sit amet nisl." +
                $" Duis placerat facilisis fringilla. Quisque volutpat suscipit metus, sed pretium urna auctor in.";
            var characterName = "Maria";
            _sendTextToTextBoxEvent.Invoke(new SendTextToTextBoxGameEvent(text, characterName));
        }

        if(Input.GetKeyDown(KeyCode.B))
        {
            _changeBackgroundEvent.Invoke(new SpriteGameEvent(_newBackground));
        }

    }

#endif

}
