using PrimeTween;
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

public struct IntegerGameEvent : SingleArgumentGameEventType<int>
{

    public int Value { get; private set; }

    public IntegerGameEvent(int value)
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

public struct ChoiceArrayGameEvent : SingleArgumentGameEventType<ChoiceData[]>
{

    public ChoiceData[] Value { get; private set; }

    public ChoiceArrayGameEvent(ChoiceData[] value) 
    {
        Value = value;
    }

}

public struct SendTextToTextBoxGameEvent : GameEventType
{

    public string textKey;

    public string characterNameKey;

    public SendTextToTextBoxGameEvent(string textKey, string characterNameKey)
    {
        this.textKey = textKey;
        this.characterNameKey = characterNameKey;
    }

}

public struct CreateCharacterGameEvent : GameEventType
{

    public string key;
    public Sprite initialSprite;
    public CharacterPosition position;
    public float offsetX;
    public float offsetY;
    public bool inverted;

    public CreateCharacterGameEvent(string key, Sprite initialSprite, CharacterPosition position, float offsetX, float offsetY, bool inverted)
    {
        this.key = key;
        this.initialSprite = initialSprite;
        this.position = position;
        this.offsetX = offsetX;
        this.offsetY = offsetY;
        this.inverted = inverted;
    }

}

public struct  FadeGameEvent : GameEventType
{
    
    public float duration;

    public TransitionMode mode;

    public Ease ease;

    public FadeGameEvent(float duration, TransitionMode mode, Ease ease)
    {
        this.duration = duration;
        this.mode = mode;
        this.ease = ease;
    }

}