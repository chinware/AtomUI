using System.ComponentModel;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

internal class DataGridSortIndicator : TemplatedControl
{
    public static readonly StyledProperty<DataGridSupportedDirections> SupportedDirectionsProperty =
        DataGridColumn.SupportedDirectionsProperty.AddOwner<DataGridSortIndicator>();
    
    public static readonly StyledProperty<ListSortDirection?> CurrentSortDirectionProperty =
        AvaloniaProperty.Register<DataGridSortIndicator, ListSortDirection?>(nameof(CurrentSortDirection));
    
    public static readonly StyledProperty<bool> IsHoverModeProperty =
        AvaloniaProperty.Register<DataGridSortIndicator, bool>(nameof(IsHoverMode));
    
    public DataGridSupportedDirections SupportedDirections
    {
        get => GetValue(SupportedDirectionsProperty);
        set => SetValue(SupportedDirectionsProperty, value);
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
}