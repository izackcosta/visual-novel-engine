using UnityEngine;

public class ChoicesAreaController : Controller<ChoicesAreaView>
{

    [SerializeField]
    private GameEvent _setChoicesRequestEvent;

    [SerializeField]
    private GameEvent _makeChoiceEvent;

    private void OnEnable()
    {
        _setChoicesRequestEvent.Listeners += OnSetChoicesRequested;
    }

    private void OnDisable()
    {
        _setChoicesRequestEvent.Listeners -= OnSetChoicesRequested;
    }

    private void OnSetChoicesRequested(GameEventType eventType) 
    {
        var e = (ChoiceArrayGameEvent)eventType;
        _view.SetChoices(e.Value, OnChoiceMade);
        _view.Show();
    }

    private void OnChoiceMade(int label) 
    {
        _makeChoiceEvent?.Invoke(new IntegerGameEvent(label));
        _view.ClearChoices();
        _view.Hide();
    }

}