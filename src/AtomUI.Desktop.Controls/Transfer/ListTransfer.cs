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
        var               sourceChanged            = (changeType & FilterChangeType.Source) == FilterChangeType.Source;
        var               targetChanged            = (changeType & FilterChangeType.Target) == FilterChangeType.Target;
        if (sourceChanged)
        {
            var sourcePanelSource = BuildSourcePanelSource(targetKeySet);
            sourcePanelSourceChanged = SourceViewSource != sourcePanelSource;
            SourceViewSource         = sourcePanelSource;
            sourceItemKeys           = BuildItemKeyList(sourcePanelSource);
        }

        if (targetChanged)
        {
            var targetPanelSource = BuildTargetPanelSource(targetKeySet);
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
