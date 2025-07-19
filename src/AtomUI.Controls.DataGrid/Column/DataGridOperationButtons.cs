using Avalonia;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

internal class DataGridOperationButtons : TemplatedControl
{
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<DataGridOperationButtons>();
    
    public static readonly StyledProperty<bool> IsEditEnabledProperty =
        AvaloniaProperty.Register<DataGridOperationButtons, bool>(nameof(IsEditEnabled));
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public bool IsEditEnabled
    {
        get => GetValue(IsEditEnabledProperty);
        set => SetValue(IsEditEnabledProperty, value);
    }
    
}