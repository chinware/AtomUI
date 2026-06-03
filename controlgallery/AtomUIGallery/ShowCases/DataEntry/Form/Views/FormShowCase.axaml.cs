using System.Globalization;
using System.Reactive.Disposables;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUI.Theme;
using AtomUI.Theme.Language;
using Avalonia;
using Avalonia.Controls;
using AtomUIGallery.Localization;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Form;

public partial class FormShowCase : ReactiveUserControl<FormViewModel>
{
    public const string LanguageId = nameof(FormShowCase);

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

        this.WhenActivated(disposables =>
        {
            if (DataContext is FormViewModel viewModel)
            {
                RefreshLocalizedOptionData(viewModel);
                var themeManager = Application.Current?.GetThemeManager();
                if (themeManager != null)
                {
                    EventHandler<LanguageVariantChangedEventArgs> handler = (_, _) => RefreshLocalizedOptionData(viewModel);
                    themeManager.LanguageVariantChanged += handler;
                    disposables.Add(Disposable.Create(() => themeManager.LanguageVariantChanged -= handler));
                }
            }
        });
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

    private static void RefreshLocalizedOptionData(FormViewModel viewModel)
    {
        viewModel.GenderOptions =
        [
            SelectOption(FormShowCaseLangResourceKind.P2HeaderMale, "Male", "male"),
            SelectOption(FormShowCaseLangResourceKind.P2HeaderFemale, "Female", "female"),
            SelectOption(FormShowCaseLangResourceKind.P2HeaderOther, "Other", "other")
        ];
        viewModel.PresetGenderOptions =
        [
            SelectOption(FormShowCaseLangResourceKind.P2HeaderMale2, "Male", "male"),
            SelectOption(FormShowCaseLangResourceKind.P2HeaderFemale2, "Female", "female"),
            SelectOption(FormShowCaseLangResourceKind.P2HeaderOther2, "Other", "other")
        ];
        viewModel.CountryOptions =
        [
            SelectOption(FormShowCaseLangResourceKind.P2HeaderChina, "China", "china"),
            SelectOption(FormShowCaseLangResourceKind.P2HeaderUSA, "USA", "usa")
        ];
        viewModel.ColorOptions =
        [
            SelectOption(FormShowCaseLangResourceKind.P2HeaderRed, "Red", "red"),
            SelectOption(FormShowCaseLangResourceKind.P2HeaderGreen, "Green", "green"),
            SelectOption(FormShowCaseLangResourceKind.P2HeaderBlue, "Blue", "blue")
        ];
        viewModel.DemoSelectOptions = [SelectOption(FormShowCaseLangResourceKind.P2HeaderDemo, "Demo", "demo")];
        viewModel.RequiredStyleSelectOptions =
        [
            SelectOption(FormShowCaseLangResourceKind.P2HeaderBbb, "Bbb", "bbb"),
            SelectOption(FormShowCaseLangResourceKind.P2HeaderAaa, "Aaa", "aaa")
        ];
        viewModel.ValidationSelectOptions =
        [
            SelectOption(FormShowCaseLangResourceKind.P2HeaderOptionN1, "Option 1", "1"),
            SelectOption(FormShowCaseLangResourceKind.P2HeaderOptionN2, "Option 2", "2"),
            SelectOption(FormShowCaseLangResourceKind.P2HeaderOptionN3, "Option 3", "3")
        ];
        viewModel.PresetCascaderOptions = BuildAddressCascaderOptions();
        viewModel.DemoCascaderOptions =
        [
            CascaderOption(FormShowCaseLangResourceKind.P2HeaderZhejiang, "Zhejiang", "zhejiang",
            [
                CascaderOption(FormShowCaseLangResourceKind.P2HeaderHangzhou, "Hangzhou", "hangzhou")
            ])
        ];
        viewModel.ValidationCascaderOptions =
        [
            CascaderOption(FormShowCaseLangResourceKind.P2HeaderXx, "xx", "xx")
        ];
        viewModel.DemoTreeNodes =
        [
            TreeNode(FormShowCaseLangResourceKind.P2HeaderLight, "Light", "light",
            [
                TreeNode(FormShowCaseLangResourceKind.P2HeaderBamboo, "Bamboo", "bamboo")
            ])
        ];
        viewModel.ValidationTreeNodes =
        [
            TreeNode(FormShowCaseLangResourceKind.P2HeaderXx, "xx", "xx")
        ];
    }

    private static List<ICascaderOption> BuildAddressCascaderOptions()
    {
        var hangzhouChildren = new List<ICascaderOption>
        {
            CascaderOption(FormShowCaseLangResourceKind.P2HeaderWestLake, "West Lake", "xihu")
        };

        return
        [
            CascaderOption(FormShowCaseLangResourceKind.P2HeaderZhejiang, "Zhejiang", "zhejiang",
            [
                CascaderOption(FormShowCaseLangResourceKind.P2HeaderHangzhou, "Hangzhou", "hangzhou", hangzhouChildren)
            ]),
            CascaderOption(FormShowCaseLangResourceKind.P2HeaderJiangsu, "Jiangsu", "jiangsu",
            [
                CascaderOption(FormShowCaseLangResourceKind.P2HeaderNanjing, "Nanjing", "nanjing",
                [
                    CascaderOption(FormShowCaseLangResourceKind.P2HeaderZhongHuaMen, "Zhong Hua Men", "zhonghuamen")
                ])
            ])
        ];
    }

    private static SelectOption SelectOption(FormShowCaseLangResourceKind header, string fallback, string content)
    {
        return new SelectOption
        {
            Header  = FormShowCaseLanguage.Get(header, fallback),
            Content = content
        };
    }

    private static CascaderOption CascaderOption(
        FormShowCaseLangResourceKind header,
        string fallback,
        string value,
        IList<ICascaderOption>? children = null)
    {
        return new CascaderOption
        {
            Header = FormShowCaseLanguage.Get(header, fallback),
            Value  = value,
            Children = children ?? []
        };
    }

    private static TreeItemNode TreeNode(
        FormShowCaseLangResourceKind header,
        string fallback,
        string value,
        IList<ITreeItemNode>? children = null)
    {
        return new TreeItemNode
        {
            Header   = FormShowCaseLanguage.Get(header, fallback),
            Value    = value,
            Children = children ?? []
        };
    }
}

internal static class FormShowCaseLanguage
{
    public static string Get(FormShowCaseLangResourceKind resourceKind, string fallback)
    {
        return LanguageResourceBinder.GetLangResource(resourceKind) ?? fallback;
    }

    public static string Format(FormShowCaseLangResourceKind resourceKind, string fallback, params object?[] args)
    {
        var format = Get(resourceKind, fallback);
        return string.Format(CultureInfo.CurrentCulture, format, args);
    }
}

public class NoteFormItem : FormItem
{
    protected override void NotifyFormItemChanged(IFormItem formItem)
    {
        if (formItem.FieldName == "gender")
        {
            if (formItem.Content is AtomUISelect select && select.Mode == SelectMode.Single)
            {
                if (formItem.GetItemValue() is ISelectOption selectOption)
                {
                    SetItemValue(FormShowCaseLanguage.Format(FormShowCaseLangResourceKind.P3GenderNoteFormat,
                        "Hi, {0}!",
                        selectOption.Content));
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
            if (formItem.Content is AtomUISelect select && select.Mode == SelectMode.Single)
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
