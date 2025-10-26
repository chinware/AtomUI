using Avalonia;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

internal class SelectHandle : TemplatedControl
{
    public static readonly StyledProperty<bool> IsInputHoverProperty =
        AvaloniaProperty.Register<SelectHandle, bool>(nameof(IsInputHover));
    
    public static readonly StyledProperty<bool> IsSearchEnabledProperty =
        AvaloniaProperty.Register<SelectHandle, bool>(nameof(IsSearchEnabled));
        
    public static readonly StyledProperty<bool> IsClearableProperty =
        AvaloniaProperty.Register<SelectHandle, bool>(nameof(IsClearable));
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty = 
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<SelectHandle>();
    
    public bool IsInputHover
    {
        get => GetValue(IsInputHoverProperty);
        set => SetValue(IsInputHoverProperty, value);
    }
    
    public bool IsSearchEnabled
    {
        get => GetValue(IsSearchEnabledProperty);
        set => SetValue(IsSearchEnabledProperty, value);
    }
    
    public bool IsClearable
    {
        get => GetValue(IsClearableProperty);
        set => SetValue(IsClearableProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
}