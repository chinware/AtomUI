using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI;
using AtomUI.Controls;
using AtomUI.Controls.Data;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUI.Theme.Language;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ReactiveUI;
using ReactiveUI.Avalonia;
using IListItemData = AtomUI.Controls.Data.IListItemData;
using ListItemData = AtomUI.Controls.Data.ListItemData;

namespace AtomUIGallery.ShowCases.List;

public partial class ListShowCase : GalleryReactiveUserControl<ListViewModel>
{
    public const string LanguageId = nameof(ListShowCase);

    private const string BasicScenario    = "Basic";
    private const string AdvancedScenario = "Advanced";

    private readonly Dictionary<string, Control> _scenarioCache = new(StringComparer.Ordinal);

    public ListShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is ListViewModel viewModel)
            {
                RefreshLocalizedListItems(viewModel);
                viewModel.SelectionMode = SelectionMode.Single;

                var themeManager = Application.Current?.GetThemeManager();
                if (themeManager != null)
                {
                    EventHandler<LanguageVariantChangedEventArgs> handler = (_, _) => RefreshLocalizedListItems(viewModel);
                    themeManager.LanguageVariantChanged += handler;
                    Disposable.Create(() => themeManager.LanguageVariantChanged -= handler)
                              .DisposeWith(disposables);
                }

                Disposable.Create(() =>
                {
                    viewModel.ListItems              = null;
                    viewModel.SelectionListItems     = null;
                    viewModel.GroupListItems         = null;
                    viewModel.ListItemsWidthDisabled = null;
                    viewModel.EmptyDemoItems         = null;
                    viewModel.FilteredGroupListItems = null;
                    viewModel.OrderedGroupListItems  = null;
                    viewModel.BasicListBoxItems      = null;
                    viewModel.PaginationListItems    = null;
                }).DisposeWith(disposables);
            }
        });
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
            BasicScenario    => new ListBasicShowCase(),
            AdvancedScenario => new ListAdvancedShowCase(),
            _                => throw new InvalidOperationException($"Unknown List scenario: {scenario}")
        };
    }

    private void RefreshLocalizedListItems(ListViewModel viewModel)
    {
        var emptyDemoItemCount = viewModel.EmptyDemoItems?.Count ?? 0;
        InitBasicListItems(viewModel);
        InitSelectionListItems(viewModel);
        InitializeGroupItems(viewModel);
        InitializeDisabledItems(viewModel);
        InitializeFilteredGroupItems(viewModel);
        InitializeOrderedGroupItems(viewModel);
        InitializeEmptyDemoItems(viewModel, emptyDemoItemCount);
        InitializeBasicListBoxItems(viewModel);
        InitializePaginationListBoxItems(viewModel);
    }

    internal static string Lang(ListShowCaseLangResourceKind resourceKind, string fallback)
    {
        return ListShowCaseLanguage.Get(resourceKind, fallback);
    }

    private List<IListItemData> BuildBasicListItems()
    {
        return [
            new ListItemData()
            {
                Content = Lang(ListShowCaseLangResourceKind.P2ColorBlue, "Blue")
            },
            new ListItemData()
            {
                Content = Lang(ListShowCaseLangResourceKind.P2ColorGreen, "Green")
            },
            new ListItemData()
            {
                Content = Lang(ListShowCaseLangResourceKind.P2ColorRed, "Red")
            },
            new ListItemData()
            {
                Content = Lang(ListShowCaseLangResourceKind.P2ColorYellow, "Yellow")
            }
        ];
    }

    private void InitBasicListItems(ListViewModel viewModel)
    {
        viewModel.ListItems = BuildBasicListItems();
    }

    private void InitSelectionListItems(ListViewModel viewModel)
    {
        viewModel.SelectionListItems = BuildBasicListItems();
    }

    private void InitializeDisabledItems(ListViewModel viewModel)
    {
        viewModel.ListItemsWidthDisabled = [
            new ListItemData()
            {
                Content = Lang(ListShowCaseLangResourceKind.P2ColorBlue, "Blue")
            },
            new ListItemData()
            {
                Content = Lang(ListShowCaseLangResourceKind.P2ColorGreen, "Green")
            },
            new ListItemData()
            {
                Content = Lang(ListShowCaseLangResourceKind.P2ColorRed, "Red")
            },
            new ListItemData()
            {
                Content   = Lang(ListShowCaseLangResourceKind.P2ColorYellow, "Yellow"),
                IsEnabled = false
            }
        ];
    }

    private List<IListItemData> BuildGroupItems()
    {
        var basicColors   = Lang(ListShowCaseLangResourceKind.P2GroupBasicColors, "Basic Colors");
        var neutralColors = Lang(ListShowCaseLangResourceKind.P2GroupNeutralColors, "Neutral Colors");
        var specificShades = Lang(ListShowCaseLangResourceKind.P2GroupSpecificShades,
            "Specific Shades");

        return [
            new ListItemData()
            {
                Content = Lang(ListShowCaseLangResourceKind.P2ColorRed, "Red"),
                Group   = basicColors
            },
            new ListItemData()
            {
                Content = Lang(ListShowCaseLangResourceKind.P2ColorOrange, "Orange"),
                Group   = basicColors
            },

            new ListItemData()
            {
                Content = Lang(ListShowCaseLangResourceKind.P2ColorGreen, "Green"),
                Group   = basicColors
            },
            new ListItemData()
            {
                Content = Lang(ListShowCaseLangResourceKind.P2ColorBlue, "Blue"),
                Group   = basicColors
            },
            new ListItemData()
            {
                Content = Lang(ListShowCaseLangResourceKind.P2ColorPurple, "Purple"),
                Group   = basicColors
            },
            new ListItemData()
            {
                Content = Lang(ListShowCaseLangResourceKind.P2ColorPink, "Pink"),
                Group   = basicColors
            },
            new ListItemData()
            {
                Content = Lang(ListShowCaseLangResourceKind.P2ColorYellow, "Yellow"),
                Group   = basicColors
            },
            new ListItemData()
            {
                Content = Lang(ListShowCaseLangResourceKind.P2ColorBrown, "Brown"),
                Group   = neutralColors
            },
            new ListItemData()
            {
                Content = Lang(ListShowCaseLangResourceKind.P2ColorWhite, "White"),
                Group   = neutralColors
            },
            new ListItemData()
            {
                Content = Lang(ListShowCaseLangResourceKind.P2ColorBlack, "Black"),
                Group   = neutralColors
            },

            new ListItemData()
            {
                Content = Lang(ListShowCaseLangResourceKind.P2ColorGray, "Gray"),
                Group   = neutralColors
            },
            new ListItemData()
            {
                Content = Lang(ListShowCaseLangResourceKind.P2ColorTurquoise, "Turquoise"),
                Group   = specificShades
            },
            new ListItemData()
            {
                Content = Lang(ListShowCaseLangResourceKind.P2ColorViolet, "Violet"),
                Group   = specificShades
            },
            new ListItemData()
            {
                Content = Lang(ListShowCaseLangResourceKind.P2ColorMagenta, "Magenta"),
                Group   = specificShades
            },
            new ListItemData()
            {
                Content = Lang(ListShowCaseLangResourceKind.P2ColorMaroon, "Maroon"),
                Group   = specificShades
            },
            new ListItemData()
            {
                Content = Lang(ListShowCaseLangResourceKind.P2ColorNavy, "Navy"),
                Group   = specificShades
            },
            new ListItemData()
            {
                Content = Lang(ListShowCaseLangResourceKind.P2ColorBeige, "Beige"),
                Group   = specificShades
            },
            new ListItemData()
            {
                Content = Lang(ListShowCaseLangResourceKind.P2ColorCyan, "Cyan"),
                Group   = specificShades
            },
            new ListItemData()
            {
                Content = Lang(ListShowCaseLangResourceKind.P2ColorLavender, "Lavender"),
                Group   = specificShades
            },
            new ListItemData()
            {
                Content = Lang(ListShowCaseLangResourceKind.P2ColorOlive, "Olive"),
                Group   = specificShades
            },
        ];
    }

    private void InitializeGroupItems(ListViewModel viewModel)
    {
        viewModel.GroupListItems = BuildGroupItems();
    }

    private void InitializeFilteredGroupItems(ListViewModel viewModel)
    {
        viewModel.FilteredGroupListItems = BuildGroupItems();
    }

    private void InitializeOrderedGroupItems(ListViewModel viewModel)
    {
        viewModel.OrderedGroupListItems = BuildGroupItems();
    }

    private void InitializeEmptyDemoItems(ListViewModel viewModel, int itemCount)
    {
        var items = new List<IListItemData>(itemCount);
        for (var i = 0; i < itemCount; i++)
        {
            items.Add(CreateDynamicItem());
        }

        viewModel.EmptyDemoItems = items;
    }

    private void InitializeBasicListBoxItems(ListViewModel viewModel)
    {
        viewModel.BasicListBoxItems = [
            new ListItemData()
            {
                Content = Lang(
                    ListShowCaseLangResourceKind.P2ContentRacingCarSpraysBurningFuelIntoCrowd,
                    "Racing car sprays burning fuel into crowd.")
            },
            new ListItemData()
            {
                Content = Lang(
                    ListShowCaseLangResourceKind.P2ContentJapanesePrincessToWedCommoner,
                    "Japanese princess to wed commoner.")
            },
            new ListItemData()
            {
                Content = Lang(
                    ListShowCaseLangResourceKind.P2ContentAustralianWalksN100kmAfterOutbackCrash,
                    "Australian walks 100km after outback crash.")
            },
            new ListItemData()
            {
                Content = Lang(
                    ListShowCaseLangResourceKind.P2ContentManChargedOverMissingWeddingGirl,
                    "Man charged over missing wedding girl.")
            },
            new ListItemData()
            {
                Content = Lang(
                    ListShowCaseLangResourceKind.P2ContentLosAngelesBattlesHugeWildfires,
                    "Los Angeles battles huge wildfires.")
            },
        ];
    }

    internal static ListItemData CreateDynamicItem()
    {
        return new ListItemData()
        {
            Content = Lang(ListShowCaseLangResourceKind.P2ContentDynamicItem, "Dynamic item")
        };
    }

    private void InitializePaginationListBoxItems(ListViewModel viewModel)
    {
        var list = new List<IListItemData>();
        for (var i = 0; i < 2000; i++)
        {
            list.Add(new ListItemData()
            {
                ItemKey = $"{i}",
                Content = string.Format(
                    Lang(ListShowCaseLangResourceKind.P2ContentPaginationItemFormat, "Content {0}"),
                    i)
            });
        }

        viewModel.PaginationListItems = list;
    }
}

internal static class ListShowCaseLanguage
{
    public static string Get(ListShowCaseLangResourceKind resourceKind, string fallback)
    {
        if (Application.Current is null)
        {
            return fallback;
        }

        return LanguageResourceBinder.GetLangResource(resourceKind) ?? fallback;
    }
}
