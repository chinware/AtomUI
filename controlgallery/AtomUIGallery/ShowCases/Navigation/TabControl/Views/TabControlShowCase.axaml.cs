using System.Globalization;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using AtomUI.Theme.Language;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AtomTabItem = AtomUI.Desktop.Controls.TabItem;
using TabStripItem = AtomUI.Desktop.Controls.TabStripItem;

namespace AtomUIGallery.ShowCases.TabControl;

public class MyTabItemData : TabItemData
{
    public object? Content { get; init; }
}

public partial class TabControlShowCase : GalleryReactiveUserControl<TabControlViewModel>
{
    public const string LanguageId = nameof(TabControlShowCase);

    private readonly List<WeakReference<CardTabControl>> _dynamicAddTabControls = [];
    private EventHandler<LanguageVariantChangedEventArgs>? _languageVariantChangedHandler;

    public TabControlShowCase()
    {
        InitializeComponent();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        RefreshViewModelData();
        SubscribeLanguageVariantChanged();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        UnsubscribeLanguageVariantChanged();
        _dynamicAddTabControls.Clear();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        RefreshViewModelData();
    }

    private void HandleTabControlPlacementOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (DataContext is TabControlViewModel viewModel)
        {
            viewModel.HandleTabControlPlacementOptionCheckedChanged(sender, args);
        }
    }

    private void HandleCardTabControlPlacementOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (DataContext is TabControlViewModel viewModel)
        {
            viewModel.HandleCardTabControlPlacementOptionCheckedChanged(sender, args);
        }
    }

    private void HandleTabControlReorderPlacementOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (DataContext is TabControlViewModel viewModel)
        {
            viewModel.HandleTabControlReorderPlacementOptionCheckedChanged(sender, args);
        }
    }

    private void HandleTabControlSizeTypeOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (DataContext is TabControlViewModel viewModel)
        {
            viewModel.HandleTabControlSizeTypeOptionCheckedChanged(sender, args);
        }
    }

    private void HandleTabControlAddTabRequest(object? sender, RoutedEventArgs args)
    {
        if (sender is not CardTabControl tabControl)
        {
            return;
        }

        TrackDynamicAddTabControl(tabControl);
        var index = tabControl.ItemCount;
        tabControl.Items.Add(new AtomTabItem
        {
            Header     = Format(TabControlShowCaseLangResourceKind.P2HeaderNewTabFormat, "new tab {0}", index),
            Content    = Format(TabControlShowCaseLangResourceKind.P2ContentNewTabContentFormat, "new tab content {0}", index),
            IsClosable = true,
            Tag        = index
        });
    }

    private void TrackDynamicAddTabControl(CardTabControl tabControl)
    {
        foreach (var reference in _dynamicAddTabControls)
        {
            if (reference.TryGetTarget(out var tracked) &&
                ReferenceEquals(tracked, tabControl))
            {
                return;
            }
        }

        _dynamicAddTabControls.Add(new WeakReference<CardTabControl>(tabControl));
    }

    private void RefreshViewModelData()
    {
        if (DataContext is TabControlViewModel viewModel)
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

    private static void RefreshItemsSourceData(TabControlViewModel viewModel)
    {
        viewModel.TabItemDataSource.Clear();
        viewModel.TabItemDataSource.Add(new MyTabItemData()
        {
            Header  = Lang(TabControlShowCaseLangResourceKind.P2HeaderTabN1, "Tab 1"),
            Content = Lang(TabControlShowCaseLangResourceKind.P2ContentDynamicTabContentN1, "Tab Content 1"),
            Icon    = new WechatFilled()
        });

        viewModel.TabItemDataSource.Add(new MyTabItemData()
        {
            Header     = Lang(TabControlShowCaseLangResourceKind.P2HeaderTabN2, "Tab 2"),
            Content    = Lang(TabControlShowCaseLangResourceKind.P2ContentDynamicTabContentN2, "Tab Content 2"),
            IsClosable = true,
            Icon       = new LinuxOutlined()
        });
    }

    private void RefreshDynamicAddedTabs()
    {
        for (var i = _dynamicAddTabControls.Count - 1; i >= 0; i--)
        {
            if (!_dynamicAddTabControls[i].TryGetTarget(out var tabControl))
            {
                _dynamicAddTabControls.RemoveAt(i);
                continue;
            }

            foreach (var item in tabControl.Items)
            {
                if (item is AtomTabItem { Tag: int index } tabItem)
                {
                    tabItem.Header  = Format(TabControlShowCaseLangResourceKind.P2HeaderNewTabFormat, "new tab {0}", index);
                    tabItem.Content = Format(TabControlShowCaseLangResourceKind.P2ContentNewTabContentFormat, "new tab content {0}", index);
                }
            }
        }
    }

    private static string Lang(TabControlShowCaseLangResourceKind resourceKind, string fallback)
    {
        return LanguageResourceBinder.GetLangResource(resourceKind) ?? fallback;
    }

    private static string Format(TabControlShowCaseLangResourceKind resourceKind, string fallback, params object?[] args)
    {
        return string.Format(CultureInfo.CurrentCulture, Lang(resourceKind, fallback), args);
    }
}
