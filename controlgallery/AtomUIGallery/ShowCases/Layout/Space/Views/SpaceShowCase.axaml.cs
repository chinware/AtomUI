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

namespace AtomUIGallery.ShowCases.Space;

public partial class SpaceShowCase : ReactiveUserControl<SpaceViewModel>
{
    public const string LanguageId = nameof(SpaceShowCase);

    private const string BasicScenario         = "Basic";
    private const string SizeScenario          = "Size";
    private const string AlignScenario         = "Align";
    private const string CompactFormScenario   = "CompactForm";
    private const string CompactButtonScenario = "CompactButton";

    private readonly Dictionary<string, Control> _scenarioCache = new(StringComparer.Ordinal);

    public SpaceShowCase()
    {
        InitializeComponent();
        ScenarioTabs.SelectionChanged += HandleScenarioSelectionChanged;
        EnsureSelectedScenarioContent();

        this.WhenActivated(disposables =>
        {
            if (DataContext is SpaceViewModel viewModel)
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
            BasicScenario         => new SpaceBasicShowCase(),
            SizeScenario          => new SpaceSizeShowCase(),
            AlignScenario         => new SpaceAlignShowCase(),
            CompactFormScenario   => new SpaceCompactFormShowCase(),
            CompactButtonScenario => new SpaceCompactButtonShowCase(),
            _                     => throw new InvalidOperationException($"Unknown Space scenario: {scenario}")
        };
    }

    private static void RefreshLocalizedOptionData(SpaceViewModel viewModel)
    {
        viewModel.ProvinceOptions =
        [
            SelectOption(SpaceShowCaseLangResourceKind.P2HeaderZhejiang, "Zhejiang", "Zhejiang"),
            SelectOption(SpaceShowCaseLangResourceKind.P2HeaderJiangsu, "Jiangsu", "Jiangsu")
        ];
        viewModel.BasicOptions =
        [
            SelectOption(SpaceShowCaseLangResourceKind.P2HeaderOption1, "Option1", "Option1"),
            SelectOption(SpaceShowCaseLangResourceKind.P2HeaderOption2, "Option2", "Option2")
        ];
        viewModel.FirstNestedOptions =
        [
            SelectOption(SpaceShowCaseLangResourceKind.P2HeaderOption1N1, "Option1-1", "Option1-1"),
            SelectOption(SpaceShowCaseLangResourceKind.P2HeaderOption2N1, "Option2-1", "Option2-1")
        ];
        viewModel.SecondNestedOptions =
        [
            SelectOption(SpaceShowCaseLangResourceKind.P2HeaderOption2N1, "Option2-1", "Option2-1"),
            SelectOption(SpaceShowCaseLangResourceKind.P2HeaderOption2N2, "Option2-2", "Option2-2")
        ];
        viewModel.ConditionOptions =
        [
            SelectOption(SpaceShowCaseLangResourceKind.P2HeaderBetween, "Between", "1"),
            SelectOption(SpaceShowCaseLangResourceKind.P2HeaderExcept, "Except", "2")
        ];
        viewModel.AuthActionOptions =
        [
            SelectOption(SpaceShowCaseLangResourceKind.P2HeaderSignUp, "Sign Up", "Sign Up"),
            SelectOption(SpaceShowCaseLangResourceKind.P2HeaderSignIn, "Sign In", "Sign In")
        ];
        viewModel.AutoCompleteTextOptions =
        [
            AutoCompleteOption(SpaceShowCaseLangResourceKind.P2HeaderTextN1, "text 1", "text 1"),
            AutoCompleteOption(SpaceShowCaseLangResourceKind.P2HeaderTextN2, "text 2", "text 2")
        ];
        viewModel.AddressCascaderOptions = BuildAddressCascaderOptions();
        viewModel.TreeSelectNodes = BuildTreeSelectNodes();
    }

    private static List<ICascaderOption> BuildAddressCascaderOptions()
    {
        return
        [
            CascaderOption(SpaceShowCaseLangResourceKind.P2HeaderZhejiang, "Zhejiang", "zhejiang",
            [
                CascaderOption(SpaceShowCaseLangResourceKind.P2HeaderHangzhou, "Hangzhou", "hangzhou",
                [
                    CascaderOption(SpaceShowCaseLangResourceKind.P2HeaderWestLake, "West Lake", "xihu"),
                    CascaderOption(SpaceShowCaseLangResourceKind.P2HeaderLingyinShi, "Lingyin Temple", "lingyinshi")
                ])
            ]),
            CascaderOption(SpaceShowCaseLangResourceKind.P2HeaderJiangsu, "Jiangsu", "jiangsu",
            [
                CascaderOption(SpaceShowCaseLangResourceKind.P2HeaderNanjing, "Nanjing", "nanjing",
                [
                    CascaderOption(SpaceShowCaseLangResourceKind.P2HeaderZhongHuaMen, "Zhong Hua Men", "zhonghuamen")
                ])
            ])
        ];
    }

    private static List<ITreeItemNode> BuildTreeSelectNodes()
    {
        return
        [
            TreeNode(SpaceShowCaseLangResourceKind.P2HeaderParentN1, "parent 1", "parent 1",
            [
                TreeNode(SpaceShowCaseLangResourceKind.P2HeaderParentN1N0, "parent 1-0", "parent 1-0",
                [
                    TreeNode(SpaceShowCaseLangResourceKind.P2HeaderLeaf1, "leaf1", "leaf1"),
                    TreeNode(SpaceShowCaseLangResourceKind.P2HeaderLeaf2, "leaf2", "leaf2")
                ]),
                TreeNode(SpaceShowCaseLangResourceKind.P2HeaderParentN1N1, "parent 1-1", "parent 1-1",
                [
                    TreeNode(SpaceShowCaseLangResourceKind.P2HeaderLeaf3, "leaf3", "leaf3")
                ])
            ])
        ];
    }

    private static SelectOption SelectOption(SpaceShowCaseLangResourceKind header, string fallback, string content)
    {
        return new SelectOption
        {
            Header  = SpaceShowCaseLanguage.Get(header, fallback),
            Content = content
        };
    }

    private static AutoCompleteOption AutoCompleteOption(SpaceShowCaseLangResourceKind header, string fallback, string content)
    {
        return new AutoCompleteOption
        {
            Header  = SpaceShowCaseLanguage.Get(header, fallback),
            Content = content
        };
    }

    private static CascaderOption CascaderOption(
        SpaceShowCaseLangResourceKind header,
        string fallback,
        string value,
        IList<ICascaderOption>? children = null)
    {
        return new CascaderOption
        {
            Header   = SpaceShowCaseLanguage.Get(header, fallback),
            Value    = value,
            Children = children ?? []
        };
    }

    private static TreeItemNode TreeNode(
        SpaceShowCaseLangResourceKind header,
        string fallback,
        string value,
        IList<ITreeItemNode>? children = null)
    {
        return new TreeItemNode
        {
            Header   = SpaceShowCaseLanguage.Get(header, fallback),
            Value    = value,
            Children = children ?? []
        };
    }
}

internal static class SpaceShowCaseLanguage
{
    public static string Get(SpaceShowCaseLangResourceKind resourceKind, string fallback)
    {
        return LanguageResourceBinder.GetLangResource(resourceKind) ?? fallback;
    }
}
