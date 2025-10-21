using System;
using UnityEngine;
using UnityEngine.Localization.Settings;
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

    private const string ChoicesLocalizationTableName = "Choices";

    protected override void Awake()
    {
        base.Awake();
        _choicesArea = _root.Q<VisualElement>(ChoicesAreaName);
    }

    public override void Hide()
    {
        _choicesArea.visible = false;
    }

    public override void Show()
    {
        _choicesArea.visible = true;
    }

    public void SetChoices(ChoiceData[] choices, Action<string> onMakeChoice)
    {

        foreach (ChoiceData choice in choices) 
        {

            var choiceElement = _choiceTemplate.Instantiate();
            var choiceLabel = choiceElement.Q<Label>(ChoiceLabelName);

            choiceLabel.text = LocalizationSettings.StringDatabase.GetLocalizedString(ChoicesLocalizationTableName, choice.Text);
            choiceLabel.RegisterCallback<ClickEvent>(ev =>
            {
                onMakeChoice.Invoke(choice.Label);
            });

            _choicesArea.Add(choiceElement);

        }

    }

    public void ClearChoices()
    {
        _choicesArea.Clear();
    }

}
