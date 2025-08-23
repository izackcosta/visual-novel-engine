using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "GameEvent", menuName = "Scriptable Objects/GameEvent")]
public class GameEvent : ScriptableObject
{

    public event UnityAction<GameEventType> Listeners = delegate { };

    public void Invoke(GameEventType @event) => Listeners.Invoke(@event);

}
