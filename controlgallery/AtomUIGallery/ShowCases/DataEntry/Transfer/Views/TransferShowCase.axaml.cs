using System.Collections.Generic;
using System.Globalization;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using AtomUI.Controls.Data;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUI.Theme.Language;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using AtomUIGallery.Localization;
using ScenarioTabStripItem = AtomUI.Desktop.Controls.TabStripItem;

namespace AtomUIGallery.ShowCases.Transfer;

public partial class TransferShowCase : GalleryReactiveUserControl<TransferViewModel>
{
    public const string LanguageId = nameof(TransferShowCase);

    private const string ExamplesScenario    = "Examples";
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly Dictionary<string, Control> _lazyScenarioContentCache = new(StringComparer.Ordinal);

    public TransferShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is TransferViewModel viewModel)
            {
                RefreshLocalizedTransferItems(viewModel);
                viewModel.TransferFilterValueSelector = record =>
                {
                    if (record is ListItemData listItemData)
                    {
                        return listItemData.Content;
                    }
                    return record?.ToString();
                };

                var themeManager = Application.Current?.GetThemeManager();
                if (themeManager != null)
                {
                    EventHandler<LanguageVariantChangedEventArgs> handler = (_, _) => RefreshLocalizedTransferItems(viewModel);
                    themeManager.LanguageVariantChanged += handler;
                    Disposable.Create(() => themeManager.LanguageVariantChanged -= handler)
                              .DisposeWith(disposables);
                }

                Disposable.Create(() =>
                {
                    viewModel.BasicTransferItems                  = null;
                    viewModel.OneWayTransferItems                 = null;
                    viewModel.SearchTransferItems                 = null;
                    viewModel.PaginationTransferItems             = null;
                    viewModel.PaginationTransferDefaultTargetKeys = null;
                    viewModel.GridDataTransformItems              = null;
                    viewModel.AdvanceTransferItems                = null;
                    viewModel.AdvanceTransferDefaultTargetKeys    = null;
                    viewModel.TransferTreeNodes                   = null;

                }).DisposeWith(disposables);
            }
        });
        InitializeComponent();
        ScenarioTabs.SelectionChanged += HandleScenarioSelectionChanged;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        EnsureSelectedScenarioContent();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        ClearLazyScenarioContent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        ExamplesContent.DataContext = DataContext;
        foreach (var content in _lazyScenarioContentCache.Values)
        {
            content.DataContext = DataContext;
        }
        EnsureSelectedScenarioContent();
    }

    private void HandleScenarioSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        EnsureSelectedScenarioContent();
    }

    private void EnsureSelectedScenarioContent()
    {
        if (ScenarioTabs.SelectedItem is not ScenarioTabStripItem tabStripItem ||
            tabStripItem.Tag is not string scenario)
        {
            return;
        }

        var content = ResolveScenarioContent(scenario);
        if (!ReferenceEquals(ScenarioContentHost.Content, content))
        {
            ScenarioContentHost.Content = content;
        }
    }

    private void ClearLazyScenarioContent()
    {
        if (ScenarioContentHost.Content is not null &&
            !ReferenceEquals(ScenarioContentHost.Content, ExamplesContent))
        {
            ScenarioContentHost.Content = null;
        }
        _lazyScenarioContentCache.Clear();
    }

    private Control ResolveScenarioContent(string scenario)
    {
        if (scenario == ExamplesScenario)
        {
            ExamplesContent.DataContext = DataContext;
            return ExamplesContent;
        }

        if (!_lazyScenarioContentCache.TryGetValue(scenario, out var content))
        {
            content             = CreateScenarioContent(scenario);
            content.DataContext = DataContext;
            _lazyScenarioContentCache.Add(scenario, content);
        }

        return content;
    }

    private static Control CreateScenarioContent(string scenario)
    {
        return scenario switch
        {
            ApiScenario         => new TransferApiDataGrid(),
            DesignTokenScenario => new TransferDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown Transfer scenario: {scenario}")
        };
    }

    private void ReloadAdvancedTransferItems(object? sender, RoutedEventArgs e)
    {
        if (sender is Control control &&
            control.FindAncestorOfType<ListTransfer>() is { } transfer &&
            DataContext is TransferViewModel viewModel)
        {
            transfer.TargetKeys = viewModel.AdvanceTransferDefaultTargetKeys;
        }
    }

    private void RefreshLocalizedTransferItems(TransferViewModel viewModel)
    {
        InitBasicTransferItems(viewModel);
        InitOneWayTransferItems(viewModel);
        InitSearchTransferItems(viewModel);
        InitPaginationTransferItems(viewModel);
        InitDataGridTransferItems(viewModel);
        InitAdvanceTransferItems(viewModel);
        InitTreeViewTransferItems(viewModel);
    }

    private void InitBasicTransferItems(TransferViewModel vm)
    {
        var items = new List<IListItemData>();
        for (var i = 0; i < 20; i++)
        {
            items.Add(new ListItemData()
            {
                ItemKey = $"{i}",
                Content = TransferShowCaseLanguage.Format(
                    TransferShowCaseLangResourceKind.P2ItemContentFormat,
                    "content{0}",
                    i + 1)
            });
        }

        vm.BasicTransferItems = items;
    }

    private void InitOneWayTransferItems(TransferViewModel vm)
    {
        var items = new List<IListItemData>();
        for (var i = 0; i < 20; i++)
        {
            items.Add(new ListItemData()
            {
                ItemKey = $"{i}",
                Content = TransferShowCaseLanguage.Format(
                    TransferShowCaseLangResourceKind.P2ItemContentFormat,
                    "content{0}",
                    i + 1),
                IsEnabled = !(i % 3 < 1)
            });
        }

        vm.OneWayTransferItems = items;
    }

    private void InitSearchTransferItems(TransferViewModel vm)
    {
        var items = new List<IListItemData>();
        for (var i = 0; i < 20; i++)
        {
            items.Add(new SearchCaseItemData()
            {
                ItemKey   = $"{i}",
                Content   = TransferShowCaseLanguage.Format(
                    TransferShowCaseLangResourceKind.P2ItemContentFormat,
                    "content{0}",
                    i + 1),
                Description = TransferShowCaseLanguage.Format(
                    TransferShowCaseLangResourceKind.P2ItemDescriptionFormat,
                    "description of content{0}",
                    i + 1)
            });
        }

        vm.SearchTransferItems = items;
    }

    private void InitAdvanceTransferItems(TransferViewModel vm)
    {
        var items      = new List<IListItemData>();
        var targetKeys = new List<EntityKey>();
        for (var i = 0; i < 20; i++)
        {
            var item = new SearchCaseItemData()
            {
                ItemKey     = $"{i}",
                Content     = TransferShowCaseLanguage.Format(
                    TransferShowCaseLangResourceKind.P2ItemContentFormat,
                    "content{0}",
                    i + 1),
                Description = TransferShowCaseLanguage.Format(
                    TransferShowCaseLangResourceKind.P2ItemDescriptionFormat,
                    "description of content{0}",
                    i + 1)
            };
            items.Add(item);
            if (i % 2 == 0)
            {
                targetKeys.Add(item.ItemKey!.Value);
            }
        }
        vm.AdvanceTransferItems             = items;
        vm.AdvanceTransferDefaultTargetKeys = targetKeys;
    }

    private void InitPaginationTransferItems(TransferViewModel vm)
    {
        var items      = new List<IListItemData>();
        var targetKeys = new List<EntityKey>();
        for (var i = 0; i < 2000; i++)
        {
            var item = new SearchCaseItemData()
            {
                ItemKey     = $"{i}",
                Content     = TransferShowCaseLanguage.Format(
                    TransferShowCaseLangResourceKind.P2ItemContentFormat,
                    "content{0}",
                    i + 1),
                Description = TransferShowCaseLanguage.Format(
                    TransferShowCaseLangResourceKind.P2ItemDescriptionFormat,
                    "description of content{0}",
                    i + 1)
            };
            items.Add(item);
            if (i % 2 == 0)
            {
                targetKeys.Add(item.ItemKey!.Value);
            }
        }
        vm.PaginationTransferItems             = items;
        vm.PaginationTransferDefaultTargetKeys = targetKeys;
    }

    private void InitDataGridTransferItems(TransferViewModel vm)
    {
        var          items = new List<DataGridTransferData>();
        List<string> tags =
        [
            TransferShowCaseLanguage.Get(TransferShowCaseLangResourceKind.P2TagCat, "cat"),
            TransferShowCaseLanguage.Get(TransferShowCaseLangResourceKind.P2TagDog, "dog"),
            TransferShowCaseLanguage.Get(TransferShowCaseLangResourceKind.P2TagBird, "bird")
        ];
        for (var i = 0; i < 20; i++)
        {
            items.Add(new DataGridTransferData()
            {
                ItemKey     = $"{i}",
                Title       = TransferShowCaseLanguage.Format(
                    TransferShowCaseLangResourceKind.P2ItemContentFormat,
                    "content{0}",
                    i + 1),
                Description = TransferShowCaseLanguage.Format(
                    TransferShowCaseLangResourceKind.P2ItemDescriptionFormat,
                    "description of content{0}",
                    i + 1),
                Tag         = tags[i % 3]
            });
        }
        vm.GridDataTransformItems = items;
    }

    private void InitTreeViewTransferItems(TransferViewModel vm)
    {
        vm.TransferTreeNodes = [
            new TreeItemNode()
            {
                ItemKey = "0-0",
                Header = "0-0"
            },
            new TreeItemNode()
            {
                ItemKey = "0-1",
                Header = "0-1",
                Children = [
                    new TreeItemNode()
                    {
                        ItemKey = "0-1-0",
                        Header = "0-1-0",
                    },
                    new TreeItemNode()
                    {
                        ItemKey = "0-1-1",
                        Header = "0-1-1",
                    }
                ]
            },
            new TreeItemNode()
            {
                ItemKey = "0-2",
                Header = "0-2"
            },
            new TreeItemNode()
            {
                ItemKey = "0-3",
                Header = "0-3"
            },
            new TreeItemNode()
            {
                ItemKey = "0-4",
                Header = "0-4"
            }
        ];
    }

}

internal static class TransferShowCaseLanguage
{
    public static string Get(TransferShowCaseLangResourceKind resourceKind, string fallback)
    {
        return LanguageResourceBinder.GetLangResource(resourceKind) ?? fallback;
    }

    public static string Format(TransferShowCaseLangResourceKind resourceKind, string fallback, params object?[] args)
    {
        return string.Format(CultureInfo.CurrentCulture, Get(resourceKind, fallback), args);
    }
}
