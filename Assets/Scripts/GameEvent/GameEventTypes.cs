using UnityEngine;

public interface GameEventType{}

public interface SingleArgumentGameEventType<T> : GameEventType
{
    public T Value { get; }
}

public struct NoArgGameEvent : GameEventType{}

public struct StringGameEvent : SingleArgumentGameEventType<string>
{

    public string Value { get; private set; }

    public StringGameEvent(string value)
    {
        Value = value;
    }

}

public struct SpriteGameEvent : SingleArgumentGameEventType<Sprite>
{

    public Sprite Value { get; private set; }

    public SpriteGameEvent(Sprite value)
    {
        Value = value;
    }

}

public struct SendTextToTextBoxGameEvent : GameEventType
{

    public string textKey;

    public string characterName;

    public SendTextToTextBoxGameEvent(string textKey, string characterName)
    {
        this.textKey = textKey;
        this.characterName = characterName;
    }
}