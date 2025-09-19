using System.Collections.Generic;
using UnityEngine;

public enum CharacterPosition
{
    Left,
    Middle,
    Right
}

public class CharacterManager : MonoBehaviour
{

    [SerializeField]
    private GameObject _characterPrefab;

    [SerializeField]
    private List<Transform> _positions;

    private Dictionary<string, Character> _characters = new Dictionary<string, Character>();

    [SerializeField]
    private GameEvent _createCharacterEvent;

    private void OnEnable()
    {
        _createCharacterEvent.Listeners += OnCreateCharacterRequest;
    }

    private void OnDisable()
    {
        _createCharacterEvent.Listeners -= OnCreateCharacterRequest;
    }

    public void CreateCharacter(string key, Sprite initialSprite, CharacterPosition position, float offsetX = 0, float offsetY = 0, bool inverted = false)
    {

        Character character = Instantiate(_characterPrefab, _positions[(int)position]).GetComponent<Character>();

        character.SetSprite(initialSprite);

        character.transform.localPosition = new Vector3(offsetX, offsetY, 0);

        if(inverted)
            character.InvertSprite();

    }

    private void OnCreateCharacterRequest(GameEventType @event)
    {
        var e = (CreateCharacterGameEvent)@event;
        CreateCharacter(e.key, e.initialSprite, e.position, e.offsetX, e.offsetY, e.inverted);
    }

}
