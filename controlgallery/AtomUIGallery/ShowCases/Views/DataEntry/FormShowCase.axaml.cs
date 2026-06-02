using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUIGallery.ShowCases.ShowCaseControls;
using AtomUIGallery.ShowCases.ViewModels;
using Avalonia.Controls;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class FormShowCase : ReactiveUserControl<FormViewModel>
{
    private const string BasicScenario      = "Basic";
    private const string LayoutScenario     = "Layout";
    private const string StatesScenario     = "States";
    private const string ValidationScenario = "Validation";
    private const string DynamicScenario    = "Dynamic";
    private const string PresetsScenario    = "Presets";
    private const string ControlsScenario   = "Controls";

    private readonly Dictionary<string, Control> _scenarioCache = new(StringComparer.Ordinal);

    public FormShowCase()
    {
        InitializeComponent();
        ScenarioTabs.SelectionChanged += HandleScenarioSelectionChanged;
        EnsureSelectedScenarioContent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        foreach (var content in _scenarioCache.Values)
        {
            content.DataContext = DataContext;
        }
    }

    private void HandleScenarioSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        EnsureSelectedScenarioContent();
    }

    private void EnsureSelectedScenarioContent()
    {
        if (ScenarioTabs.SelectedItem is not AtomUI.Desktop.Controls.TabItem tabItem ||
            tabItem.Tag is not string scenario)
        {
            return;
        }

        if (!_scenarioCache.TryGetValue(scenario, out var content))
        {
            content             = CreateScenarioContent(scenario);
            content.DataContext = DataContext;
            _scenarioCache.Add(scenario, content);
        }

        if (tabItem.Content != content)
        {
            tabItem.Content = content;
        }
    }

    private static Control CreateScenarioContent(string scenario)
    {
        return scenario switch
        {
            BasicScenario      => new FormBasicShowCase(),
            LayoutScenario     => new FormLayoutShowCase(),
            StatesScenario     => new FormStateShowCase(),
            ValidationScenario => new FormValidationShowCase(),
            DynamicScenario    => new FormDynamicShowCase(),
            PresetsScenario    => new FormPresetShowCase(),
            ControlsScenario   => new FormControlsShowCase(),
            _                  => throw new InvalidOperationException($"Unknown form scenario: {scenario}")
        };
    }
}

public class NoteFormItem : FormItem
{
    protected override void NotifyFormItemChanged(IFormItem formItem)
    {
        if (formItem.FieldName == "gender")
        {
            if (formItem.Content is Select select && select.Mode == SelectMode.Single)
            {
                if (formItem.GetItemValue() is ISelectOption selectOption)
                {
                    SetItemValue($"Hi, {selectOption.Content}!");
                }
            }
        }
    }
}

public class CustomizeGenderFormItem : FormItem
{
    protected override void NotifyFormItemChanged(IFormItem formItem)
    {
        if (formItem.FieldName == "gender")
        {
            if (formItem.Content is Select select && select.Mode == SelectMode.Single)
            {
                var option = formItem.GetItemValue() as ISelectOption;
                if (option?.Content?.ToString() == "other")
                {
                    IsVisible = true;
                }
                else
                {
                    IsVisible = false;
                }
            }
        }
    }
}

public class PriceValidator : AbstractFormValidator
{
    protected override async Task<bool> ValidateCoreAsync(string fieldName, object? value, CancellationToken cancellationToken)
    {
        var price = value as PriceInfo;
        return await Task.FromResult(price != null && price.Value > 0);
    }
}
