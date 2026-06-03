using System.Globalization;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using AtomUI.Theme.Language;
using Avalonia;
using Avalonia.Interactivity;
using AtomUIGallery.Localization;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.TabControl;

public class MyTabItemData : TabItemData
{
    public object? Content { get; init; }
}

public partial class TabControlShowCase : ReactiveUserControl<TabControlViewModel>
{
    public const string LanguageId = nameof(TabControlShowCase);

    public TabControlShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is TabControlViewModel viewModel)
            {
                RefreshItemsSourceData(viewModel);

                PositionTabStripOptionGroup.OptionCheckedChanged     += viewModel.HandleTabStripPlacementOptionCheckedChanged;
                PositionCardTabStripOptionGroup.OptionCheckedChanged += viewModel.HandleCardTabStripPlacementOptionCheckedChanged;
                SizeTypeTabStripOptionGroup.OptionCheckedChanged     += viewModel.HandleTabStripSizeTypeOptionCheckedChanged;
                AddTabDemoStrip.AddTabRequest                        += HandleTabStripAddTabRequest;

                PositionTabControlOptionGroup.OptionCheckedChanged     += viewModel.HandleTabControlPlacementOptionCheckedChanged;
                PositionCardTabControlOptionGroup.OptionCheckedChanged += viewModel.HandleCardTabControlPlacementOptionCheckedChanged;
                SizeTypeTabControlOptionGroup.OptionCheckedChanged     += viewModel.HandleTabControlSizeTypeOptionCheckedChanged;
                AddTabDemoTabControl.AddTabRequest                     += HandleTabControlAddTabRequest;

                var themeManager = Application.Current?.GetThemeManager();
                if (themeManager != null)
                {
                    EventHandler<LanguageVariantChangedEventArgs> handler = (_, _) =>
                    {
                        RefreshItemsSourceData(viewModel);
                        RefreshDynamicAddedTabs();
                    };
                    themeManager.LanguageVariantChanged += handler;
                    Disposable.Create(() => themeManager.LanguageVariantChanged -= handler)
                        .DisposeWith(disposables);
                }

                Disposable.Create(() =>
                {
                    PositionTabStripOptionGroup.OptionCheckedChanged     -= viewModel.HandleTabStripPlacementOptionCheckedChanged;
                    PositionCardTabStripOptionGroup.OptionCheckedChanged -= viewModel.HandleCardTabStripPlacementOptionCheckedChanged;
                    SizeTypeTabStripOptionGroup.OptionCheckedChanged     -= viewModel.HandleTabStripSizeTypeOptionCheckedChanged;
                    AddTabDemoStrip.AddTabRequest                        -= HandleTabStripAddTabRequest;

                    PositionTabControlOptionGroup.OptionCheckedChanged     -= viewModel.HandleTabControlPlacementOptionCheckedChanged;
                    PositionCardTabControlOptionGroup.OptionCheckedChanged -= viewModel.HandleCardTabControlPlacementOptionCheckedChanged;
                    SizeTypeTabControlOptionGroup.OptionCheckedChanged     -= viewModel.HandleTabControlSizeTypeOptionCheckedChanged;
                    AddTabDemoTabControl.AddTabRequest                     -= HandleTabControlAddTabRequest;

                    viewModel.TabItemDataSource      = new();
                    viewModel.TabStripItemDataSource = new();
                }).DisposeWith(disposables);
            }
        });
        InitializeComponent();
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

        viewModel.TabStripItemDataSource.Clear();
        viewModel.TabStripItemDataSource.Add(new TabItemData()
        {
            Header = Lang(TabControlShowCaseLangResourceKind.P2ContentTabN1, "Tab 1")
        });
        viewModel.TabStripItemDataSource.Add(new TabItemData()
        {
            Header = Lang(TabControlShowCaseLangResourceKind.P2ContentTabN2, "Tab 2")
        });
    }

    private void HandleTabStripAddTabRequest(object? sender, RoutedEventArgs args)
    {
        var index = AddTabDemoStrip.ItemCount;
        AddTabDemoStrip.Items.Add(new TabStripItem
        {
            Content    = Format(TabControlShowCaseLangResourceKind.P2ContentNewTabFormat, "new tab {0}", index),
            IsClosable = true,
            Tag        = index
        });
    }

    private void HandleTabControlAddTabRequest(object? sender, RoutedEventArgs args)
    {
        var index = AddTabDemoTabControl.ItemCount;
        AddTabDemoTabControl.Items.Add(new TabItem
        {
            Header     = Format(TabControlShowCaseLangResourceKind.P2HeaderNewTabFormat, "new tab {0}", index),
            Content    = Format(TabControlShowCaseLangResourceKind.P2ContentNewTabContentFormat, "new tab content {0}", index),
            IsClosable = true,
            Tag        = index
        });
    }

    private void RefreshDynamicAddedTabs()
    {
        foreach (var item in AddTabDemoStrip.Items)
        {
            if (item is TabStripItem { Tag: int index } tabStripItem)
            {
                tabStripItem.Content = Format(TabControlShowCaseLangResourceKind.P2ContentNewTabFormat, "new tab {0}", index);
            }
        }

        foreach (var item in AddTabDemoTabControl.Items)
        {
            if (item is TabItem { Tag: int index } tabItem)
            {
                tabItem.Header  = Format(TabControlShowCaseLangResourceKind.P2HeaderNewTabFormat, "new tab {0}", index);
                tabItem.Content = Format(TabControlShowCaseLangResourceKind.P2ContentNewTabContentFormat, "new tab content {0}", index);
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
