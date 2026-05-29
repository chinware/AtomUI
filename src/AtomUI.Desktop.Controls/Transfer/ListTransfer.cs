using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace AtomUI.Desktop.Controls;

public class ListTransfer : AbstractTransfer
{
    #region 公共属性定义
    
    public static readonly StyledProperty<int> PageSizeProperty =
        AvaloniaProperty.Register<ListTransfer, int>(nameof(PageSize), 0);
    
    public static readonly StyledProperty<ITransferView?> SourceViewProperty =
        AvaloniaProperty.Register<ListTransfer, ITransferView?>(nameof(SourceView));
    
    public static readonly StyledProperty<ITransferView?> TargetViewProperty =
        AvaloniaProperty.Register<ListTransfer, ITransferView?>(nameof(TargetView));
    
    public int PageSize
    {
        get => GetValue(PageSizeProperty);
        set => SetValue(PageSizeProperty, value);
    }
    
    public ITransferView? SourceView
    {
        get => GetValue(SourceViewProperty);
        set => SetValue(SourceViewProperty, value);
    }
    
    public ITransferView? TargetView
    {
        get => GetValue(TargetViewProperty);
        set => SetValue(TargetViewProperty, value);
    }
    #endregion

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        
        if (SourceView == null)
        {
            SetCurrentValue(SourceViewProperty, new TransferListView());
        }

        if (TargetView == null)
        {
            SetCurrentValue(TargetViewProperty, new TransferListView());
        }
        SetCurrentValue(IsPaginationEnabledProperty, PageSize > 0);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == PageSizeProperty)
        {
            SetCurrentValue(IsPaginationEnabledProperty, PageSize > 0);
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
            var sourcePanelSource = ItemsSource?
                                    .Where(item => !(targetKeySet?.Contains(item.ItemKey ?? default) ?? false))
                                    .Where(item => !IsFilterEnabled || string.IsNullOrEmpty(SourceFilterValue) || 
                                                   (Filter?.Filter(FilterValueSelector != null ? FilterValueSelector(item) : item,
                                                       SourceFilterValue) ?? false))
                                    .ToArray();
            sourcePanelSourceChanged = SourceViewSource != sourcePanelSource;
            SourceViewSource         = sourcePanelSource;
            sourceItemKeys           = BuildItemKeyList(sourcePanelSource);
        }

        if (changeType.HasFlag(FilterChangeType.Target))
        {
            IEnumerable<IItemKey>? targetPanelSource = null;
            if (ItemsSource != null)
            {
                targetPanelSource = targetKeySet == null
                    ? Array.Empty<IItemKey>()
                    : ItemsSource
                        .Where(item => targetKeySet.Contains(item.ItemKey ?? default))
                        .Where(item => !IsFilterEnabled || string.IsNullOrEmpty(TargetFilterValue) ||
                                       (Filter?.Filter(FilterValueSelector != null ? FilterValueSelector(item) : item,
                                           TargetFilterValue) ?? false))
                        .ToArray();
            }
            targetPanelSourceChanged = TargetViewSource != targetPanelSource;
            TargetViewSource         = targetPanelSource;
            targetItemKeys           = BuildItemKeyList(targetPanelSource);
        }

        if (sourcePanelSourceChanged || targetPanelSourceChanged)
        {
            NotifySelectionChanged(sourceItemKeys, targetItemKeys);
        }
    }
}
