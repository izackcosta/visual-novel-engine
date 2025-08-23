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