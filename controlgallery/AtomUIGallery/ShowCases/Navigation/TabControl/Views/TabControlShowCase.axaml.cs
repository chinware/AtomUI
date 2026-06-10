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

namespace AtomUIGallery.ShowCases.TabControl;

public class MyTabItemData : TabItemData
{
    public object? Content { get; init; }
}

public partial class TabControlShowCase : GalleryReactiveUserControl<TabControlViewModel>
{
    public const string LanguageId = nameof(TabControlShowCase);

    public TabControlShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is TabControlViewModel viewModel)
            {
                RefreshItemsSourceData(viewModel);

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
                    PositionTabControlOptionGroup.OptionCheckedChanged     -= viewModel.HandleTabControlPlacementOptionCheckedChanged;
                    PositionCardTabControlOptionGroup.OptionCheckedChanged -= viewModel.HandleCardTabControlPlacementOptionCheckedChanged;
                    SizeTypeTabControlOptionGroup.OptionCheckedChanged     -= viewModel.HandleTabControlSizeTypeOptionCheckedChanged;
                    AddTabDemoTabControl.AddTabRequest                     -= HandleTabControlAddTabRequest;

                    viewModel.TabItemDataSource = new();
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
