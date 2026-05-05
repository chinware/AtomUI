using AtomUI.Animations;
using AtomUI.Controls;
using Avalonia;
using Avalonia.Interactivity;

namespace AtomUI.Desktop.Controls;

using AvaloniaListBoxItem = Avalonia.Controls.ListBoxItem;

public class TimeViewCell : AvaloniaListBoxItem
{
    #region 内部属性定义
    
    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<TimeViewCell>();

    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    #endregion

    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.DisableTransitions();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        this.EnableTransitions();
    }
}