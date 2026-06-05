using Avalonia;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

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

    static AbstractScrollBar()
    {
        Thumb.DragStartedEvent.AddClassHandler<AbstractScrollBar>(
            (x, e) => x.NotifyThumbDragStarted(e),
            RoutingStrategies.Bubble);
        Thumb.DragCompletedEvent.AddClassHandler<AbstractScrollBar>(
            (x, e) => x.NotifyThumbDragCompleted(e),
            RoutingStrategies.Bubble);
    }
    
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

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        SetOwnerScrollBarDragging(false);
        base.OnDetachedFromVisualTree(e);
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

    protected virtual void NotifyThumbDragStarted(VectorEventArgs e)
    {
        SetOwnerScrollBarDragging(true);
    }

    protected virtual void NotifyThumbDragCompleted(VectorEventArgs e)
    {
        SetOwnerScrollBarDragging(false);
    }

    private void SetOwnerScrollBarDragging(bool isDragging)
    {
        if (TemplatedParent is AbstractScrollViewer scrollViewer)
        {
            scrollViewer.IsScrollBarDragging = isDragging;
        }
    }
}
