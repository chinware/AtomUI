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
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Transfer;

public partial class TransferShowCase : GalleryReactiveUserControl<TransferViewModel>
{
    public const string LanguageId = nameof(TransferShowCase);

    private const string BasicScenario      = "Basic";
    private const string AdvancedScenario   = "Advanced";
    private const string TreeStatusScenario = "TreeStatus";

    private readonly Dictionary<string, Control> _scenarioCache = new(StringComparer.Ordinal);

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
            BasicScenario      => new TransferBasicShowCase(),
            AdvancedScenario   => new TransferAdvancedShowCase(),
            TreeStatusScenario => new TransferTreeStatusShowCase(),
            _                  => throw new InvalidOperationException($"Unknown Transfer scenario: {scenario}")
        };
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
