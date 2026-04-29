using Avalonia;
using Avalonia.Controls.Metadata;

namespace AtomUI.Controls.Commons;

using AvaloniaScrollBar = Avalonia.Controls.Primitives.ScrollBar;

[PseudoClasses(StdPseudoClass.Vertical, StdPseudoClass.Horizontal)]
public abstract class AbstractScrollBar : AvaloniaScrollBar, IMotionAwareControl
{
    #region 公共属性定义
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<AbstractScrollViewer>();

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    #endregion
    
    #region 内部属性定义

    internal static readonly StyledProperty<bool> IsEffectiveExpandedProperty =
        AvaloniaProperty.Register<AbstractScrollBar, bool>(nameof(IsEffectiveExpanded));
    
    internal bool IsEffectiveExpanded
    {
        get => GetValue(IsEffectiveExpandedProperty);
        set => SetValue(IsEffectiveExpandedProperty, value);
    }

    #endregion
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == AllowAutoHideProperty)
        {
            UpdateIsExpandedState();
        }
        else
        {
            base.OnPropertyChanged(change);
            if (change.Property == IsEffectiveExpandedProperty)
            {
                this.SetIsExpanded(IsEffectiveExpanded);
            }
        }
    }
    
    protected virtual void UpdateIsExpandedState()
    {
        if (!AllowAutoHide)
        {
            var timer = this.GetTimer();
            timer?.Stop();
            IsEffectiveExpanded = false;
        }
    }
}