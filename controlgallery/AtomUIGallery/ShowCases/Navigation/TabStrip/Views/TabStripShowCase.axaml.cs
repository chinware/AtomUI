using System.Globalization;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUI.Theme.Language;
using Avalonia;
using Avalonia.Interactivity;
using AtomUIGallery.Localization;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.TabStrip;

public partial class TabStripShowCase : GalleryReactiveUserControl<TabStripViewModel>
{
    public const string LanguageId = nameof(TabStripShowCase);

    public TabStripShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is TabStripViewModel viewModel)
            {
                RefreshItemsSourceData(viewModel);

                PositionTabStripOptionGroup.OptionCheckedChanged     += viewModel.HandleTabStripPlacementOptionCheckedChanged;
                PositionCardTabStripOptionGroup.OptionCheckedChanged += viewModel.HandleCardTabStripPlacementOptionCheckedChanged;
                SizeTypeTabStripOptionGroup.OptionCheckedChanged     += viewModel.HandleTabStripSizeTypeOptionCheckedChanged;
                AddTabDemoStrip.AddTabRequest                        += HandleTabStripAddTabRequest;

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

                    viewModel.TabStripItemDataSource = new();
                }).DisposeWith(disposables);
            }
        });
        InitializeComponent();
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

    private void HandleTabStripAddTabRequest(object? sender, RoutedEventArgs args)
    {
        var index = AddTabDemoStrip.ItemCount;
        AddTabDemoStrip.Items.Add(new TabStripItem
        {
            Content    = Format(TabStripShowCaseLangResourceKind.P2ContentNewTabFormat, "new tab {0}", index),
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
                tabStripItem.Content = Format(TabStripShowCaseLangResourceKind.P2ContentNewTabFormat, "new tab {0}", index);
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
