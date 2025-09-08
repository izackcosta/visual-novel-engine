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

    public void CreateCharacter(string key, Sprite initialSprite, CharacterPosition position)
    {



    }

}
