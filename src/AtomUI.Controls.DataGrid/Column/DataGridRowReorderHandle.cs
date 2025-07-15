using Avalonia;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

internal class DataGridRowReorderHandle : TemplatedControl
{
    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<DataGridRowReorderHandle>();
    
    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    internal void NotifyLoadingRow(DataGridRow row)
    {
       
    }
    
    internal void NotifyUnLoadingRow(DataGridRow row)
    {
        
    }
}