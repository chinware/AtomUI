using AtomUI.Controls;
using AtomUI.Controls.Data;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace AtomUI.Desktop.Controls;

public class TreeTransfer : AbstractTransfer
{
    #region 公共属性定义
    public static readonly StyledProperty<ITransferTreeView?> SourceViewProperty =
        AvaloniaProperty.Register<TreeTransfer, ITransferTreeView?>(nameof(SourceView));
    
    public ITransferTreeView? SourceView
    {
        get => GetValue(SourceViewProperty);
        set => SetValue(SourceViewProperty, value);
    }
    #endregion

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (SourceView == null)
        {
            SetCurrentValue(SourceViewProperty, new TransferTreeView());
        }
    }

    protected override void ConfigurePanelItemsSourceForFilter(FilterChangeType changeType)
    {
        var               sourcePanelSourceChanged = false;
        var               targetPanelSourceChanged = false;
        IList<EntityKey>? sourceItemKeys           = null;
        IList<EntityKey>? targetItemKeys           = null;
        var               targetKeySet             = BuildTargetKeySet(TargetKeys);
        if (changeType.HasFlag(FilterChangeType.Source))
        {
            var sourcePanelSource = ItemsSource;
            sourcePanelSourceChanged = SourceViewSource != sourcePanelSource;
            SourceViewSource         = sourcePanelSource;
            sourceItemKeys           = BuildItemKeyList(sourcePanelSource);
            SourceView?.SetMaskedItems(TargetKeys);
        }

        if (changeType.HasFlag(FilterChangeType.Target))
        {
            IEnumerable<IListItemData> targetPanelSource = targetKeySet == null
                ? Array.Empty<IListItemData>()
                : CalculateTargetItemsSource(ItemsSource?.Cast<ITreeItemNode>(), targetKeySet);
            targetPanelSourceChanged = TargetViewSource != targetPanelSource;
            TargetViewSource         = targetPanelSource;
            targetItemKeys           = BuildItemKeyList(targetPanelSource);
        }

        if (sourcePanelSourceChanged || targetPanelSourceChanged)
        {
            NotifySelectionChanged(sourceItemKeys, targetItemKeys);
        }
    }

    private List<IListItemData> CalculateTargetItemsSource(IEnumerable<ITreeItemNode>? itemNodes, ISet<EntityKey> targetKeySet)
    {
        var results = new List<IListItemData>(targetKeySet.Count);
        CollectTargetItemsSource(itemNodes, targetKeySet, results);
        return results;
    }

    private void CollectTargetItemsSource(
        IEnumerable<ITreeItemNode>? itemNodes,
        ISet<EntityKey> targetKeySet,
        IList<IListItemData> results)
    {
        if (itemNodes != null)
        {
            foreach (var node in itemNodes)
            {
                if (targetKeySet.Contains(node.ItemKey ?? default))
                {
                    var item = new ListItemData
                    {
                        ItemKey = node.ItemKey ?? default,
                        Content = node.Header
                    };
                    if (IsTargetFilterMatched(item))
                    {
                        results.Add(item);
                    }
                }
                CollectTargetItemsSource(node.Children, targetKeySet, results);
            }
        }
    }

    private bool IsTargetFilterMatched(IListItemData item)
    {
        return !IsFilterEnabled ||
               string.IsNullOrEmpty(TargetFilterValue) ||
               (Filter?.Filter(FilterValueSelector != null ? FilterValueSelector(item) : item,
                   TargetFilterValue) ?? false);
    }
    
}
