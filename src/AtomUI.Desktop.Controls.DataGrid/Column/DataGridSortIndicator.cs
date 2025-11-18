using System.ComponentModel;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace AtomUI.Desktop.Controls;

internal class DataGridSortIndicator : TemplatedControl
{
    #region 公共属性定义

    public static readonly StyledProperty<DataGridSortDirections> SupportedSortDirectionsProperty =
        DataGridColumn.SupportedSortDirectionsProperty.AddOwner<DataGridSortIndicator>();

    public static readonly StyledProperty<ListSortDirection?> CurrentSortDirectionProperty =
        AvaloniaProperty.Register<DataGridSortIndicator, ListSortDirection?>(nameof(CurrentSortDirection));

    public static readonly StyledProperty<bool> IsHoverModeProperty =
        AvaloniaProperty.Register<DataGridSortIndicator, bool>(nameof(IsHoverMode));

    public DataGridSortDirections SupportedSortDirections
    {
        get => GetValue(SupportedSortDirectionsProperty);
        set => SetValue(SupportedSortDirectionsProperty, value);
    }

    public ListSortDirection? CurrentSortDirection
    {
        get => GetValue(CurrentSortDirectionProperty);
        set => SetValue(CurrentSortDirectionProperty, value);
    }

    public bool IsHoverMode
    {
        get => GetValue(IsHoverModeProperty);
        set => SetValue(IsHoverModeProperty, value);
    }

    #endregion

    #region 内部属性定义
    
    public static readonly DirectProperty<DataGridSortIndicator, bool> AscendingIndicatorVisibleProperty =
        AvaloniaProperty.RegisterDirect<DataGridSortIndicator, bool>(
            nameof(AscendingIndicatorVisible),
            o => o.AscendingIndicatorVisible,
            (o, v) => o.AscendingIndicatorVisible = v);
    
    public static readonly DirectProperty<DataGridSortIndicator, bool> DescendingIndicatorVisibleProperty =
        AvaloniaProperty.RegisterDirect<DataGridSortIndicator, bool>(
            nameof(DescendingIndicatorVisible),
            o => o.DescendingIndicatorVisible,
            (o, v) => o.DescendingIndicatorVisible = v);
    
    private bool _ascendingIndicatorVisible;
    public bool AscendingIndicatorVisible
    {
        get => _ascendingIndicatorVisible;
        set => SetAndRaise(AscendingIndicatorVisibleProperty, ref _ascendingIndicatorVisible, value);
    }
    
    private bool _descendingIndicatorVisible;
    public bool DescendingIndicatorVisible
    {
        get => _descendingIndicatorVisible;
        set => SetAndRaise(DescendingIndicatorVisibleProperty, ref _descendingIndicatorVisible, value);
    }

    #endregion

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SupportedSortDirectionsProperty)
        {
            ConfigureIndicatorVisible();
        }
    }

    private void ConfigureIndicatorVisible()
    {
        DescendingIndicatorVisible = (SupportedSortDirections & DataGridSortDirections.Descending) != 0;
        AscendingIndicatorVisible  = (SupportedSortDirections & DataGridSortDirections.Ascending) != 0;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        ConfigureIndicatorVisible();
    }
}