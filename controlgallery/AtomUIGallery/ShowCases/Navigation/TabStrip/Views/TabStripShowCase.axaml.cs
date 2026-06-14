using System.Globalization;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUI.Theme.Language;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AtomTabStripItem = AtomUI.Desktop.Controls.TabStripItem;
using ScenarioTabStripItem = AtomUI.Desktop.Controls.TabStripItem;

namespace AtomUIGallery.ShowCases.TabStrip;

public partial class TabStripShowCase : GalleryReactiveUserControl<TabStripViewModel>
{
    public const string LanguageId = nameof(TabStripShowCase);
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;
    private readonly List<WeakReference<CardTabStrip>> _dynamicAddTabStrips = [];
    private EventHandler<LanguageVariantChangedEventArgs>? _languageVariantChangedHandler;

    public TabStripShowCase()
    {
        InitializeComponent();
        _scenarioController = new GalleryShowCaseScenarioController(ScenarioTabs, ScenarioContentHost, CreateScenarioContent, ExamplesContent);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        RefreshViewModelData();
        SubscribeLanguageVariantChanged();
        _scenarioController.Attach(DataContext);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        UnsubscribeLanguageVariantChanged();
        _scenarioController.Detach();
        _dynamicAddTabStrips.Clear();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        RefreshViewModelData();
        _scenarioController.UpdateDataContext(DataContext);
    }

    private static Control CreateScenarioContent(string scenario)
    {
        return scenario switch
        {
            ApiScenario         => new TabStripApiDataGrid(),
            DesignTokenScenario => new TabStripDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown TabStrip scenario: {scenario}")
        };
    }

    private void HandleTabStripPlacementOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (DataContext is TabStripViewModel viewModel)
        {
            viewModel.HandleTabStripPlacementOptionCheckedChanged(sender, args);
        }
    }

    private void HandleCardTabStripPlacementOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (DataContext is TabStripViewModel viewModel)
        {
            viewModel.HandleCardTabStripPlacementOptionCheckedChanged(sender, args);
        }
    }

    private void HandleTabStripSizeTypeOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (DataContext is TabStripViewModel viewModel)
        {
            viewModel.HandleTabStripSizeTypeOptionCheckedChanged(sender, args);
        }
    }

    private void HandleTabStripAddTabRequest(object? sender, RoutedEventArgs args)
    {
        if (sender is not CardTabStrip tabStrip)
        {
            return;
        }

        TrackDynamicAddTabStrip(tabStrip);
        var index = tabStrip.ItemCount;
        tabStrip.Items.Add(new AtomTabStripItem
        {
            Content    = Format(TabStripShowCaseLangResourceKind.P2ContentNewTabFormat, "new tab {0}", index),
            IsClosable = true,
            Tag        = index
        });
    }

    private void TrackDynamicAddTabStrip(CardTabStrip tabStrip)
    {
        foreach (var reference in _dynamicAddTabStrips)
        {
            if (reference.TryGetTarget(out var tracked) &&
                ReferenceEquals(tracked, tabStrip))
            {
                return;
            }
        }

        _dynamicAddTabStrips.Add(new WeakReference<CardTabStrip>(tabStrip));
    }

    private void RefreshViewModelData()
    {
        if (DataContext is TabStripViewModel viewModel)
        {
            RefreshItemsSourceData(viewModel);
        }
    }

    private void SubscribeLanguageVariantChanged()
    {
        if (_languageVariantChangedHandler is not null)
        {
            return;
        }

        var themeManager = Application.Current?.GetThemeManager();
        if (themeManager is null)
        {
            return;
        }

        _languageVariantChangedHandler = (_, _) =>
        {
            RefreshViewModelData();
            RefreshDynamicAddedTabs();
        };
        themeManager.LanguageVariantChanged += _languageVariantChangedHandler;
    }

    private void UnsubscribeLanguageVariantChanged()
    {
        if (_languageVariantChangedHandler is null)
        {
            return;
        }

        var themeManager = Application.Current?.GetThemeManager();
        if (themeManager is not null)
        {
            themeManager.LanguageVariantChanged -= _languageVariantChangedHandler;
        }
        _languageVariantChangedHandler = null;
    }

    private static void RefreshItemsSourceData(TabStripViewModel viewModel)
    {
        viewModel.TabStripItemDataSource.Clear();
        viewModel.TabStripItemDataSource.Add(new TabItemData()
        {
            Header = Lang(TabStripShowCaseLangResourceKind.P2ContentTabN1, "Tab 1")
        });
        viewModel.TabStripItemDataSource.Add(new TabItemData()
        {
            Header = Lang(TabStripShowCaseLangResourceKind.P2ContentTabN2, "Tab 2")
        });
    }

    private void RefreshDynamicAddedTabs()
    {
        for (var i = _dynamicAddTabStrips.Count - 1; i >= 0; i--)
        {
            if (!_dynamicAddTabStrips[i].TryGetTarget(out var tabStrip))
            {
                _dynamicAddTabStrips.RemoveAt(i);
                continue;
            }

            foreach (var item in tabStrip.Items)
            {
                if (item is AtomTabStripItem { Tag: int index } tabStripItem)
                {
                    tabStripItem.Content = Format(TabStripShowCaseLangResourceKind.P2ContentNewTabFormat, "new tab {0}", index);
                }
            }
        }
    }

    private static string Lang(TabStripShowCaseLangResourceKind resourceKind, string fallback)
    {
        return LanguageResourceBinder.GetLangResource(resourceKind) ?? fallback;
    }

    private static string Format(TabStripShowCaseLangResourceKind resourceKind, string fallback, params object?[] args)
    {
        return string.Format(CultureInfo.CurrentCulture, Lang(resourceKind, fallback), args);
    }
}
