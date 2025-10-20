using UnityEngine;
using UnityEngine.UIElements;

public struct ChoiceData
{

    public string Text;
    public string Label;

    public ChoiceData(string text, string label)
    {
        Text = text;
        Label = label;
    }

}

public class ChoicesAreaView : View
{

    [SerializeField]
    private VisualTreeAsset _choiceTemplate;

    private const string ChoiceLabelName = "Choice";

    private VisualElement _choicesArea;

    private const string ChoicesAreaName = "ChoicesArea";

    protected override void Awake()
    {
        base.Awake();
        _choicesArea = _root.Q<VisualElement>(ChoicesAreaName);
        Show();
        SetChoices(new ChoiceData[] 
        {
            new ChoiceData("Choice 1", "choice_1"),
            new ChoiceData("Choice 2", "choice_2"),
            new ChoiceData("Choice 3", "choice_3"),
            new ChoiceData("Choice 4", "choice_4"),
        });
    }

    public override void Hide()
    {
        _choicesArea.visible = false;
    }

    public override void Show()
    {
        _choicesArea.visible = true;
    }

    public void SetChoices(ChoiceData[] choices)
    {

        foreach (ChoiceData choice in choices) 
        {

            var choiceElement = _choiceTemplate.Instantiate();
            var choiceLabel = choiceElement.Q<Label>(ChoiceLabelName);

            choiceLabel.text = choice.Text;
            choiceLabel.RegisterCallback<ClickEvent>(ev =>
            {
                Debug.Log($"Choice selected: {choice.Label}");
            });

            _choicesArea.Add(choiceElement);

        }

    }

}
