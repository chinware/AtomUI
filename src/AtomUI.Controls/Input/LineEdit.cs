using AtomUI.Controls.Utils;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Controls;

public class LineEdit : TextBox,
                        IAnimationAwareControl
{
    #region 公共属性定义

    public static readonly StyledProperty<object?> LeftAddOnProperty =
        AvaloniaProperty.Register<LineEdit, object?>(nameof(LeftAddOn));

    public static readonly StyledProperty<object?> RightAddOnProperty =
        AvaloniaProperty.Register<LineEdit, object?>(nameof(RightAddOn));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = AvaloniaProperty.Register<LineEdit, bool>(nameof(IsMotionEnabled), true);

    public static readonly StyledProperty<bool> IsWaveAnimationEnabledProperty
        = AvaloniaProperty.Register<LineEdit, bool>(nameof(IsWaveAnimationEnabled), true);
    
    public object? LeftAddOn
    {
        get => GetValue(LeftAddOnProperty);
        set => SetValue(LeftAddOnProperty, value);
    }

    public object? RightAddOn
    {
        get => GetValue(RightAddOnProperty);
        set => SetValue(RightAddOnProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public bool IsWaveAnimationEnabled
    {
        get => GetValue(IsWaveAnimationEnabledProperty);
        set => SetValue(IsWaveAnimationEnabledProperty, value);
    }
    
    #endregion

    #region 内部属性定义

    Control IAnimationAwareControl.PropertyBindTarget => this;

    #endregion

    public LineEdit()
    {
        this.BindAnimationProperties(IsMotionEnabledProperty, IsWaveAnimationEnabledProperty);
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        // TODO 到底是否需要这样，这些控件的管辖区理论上不应该我们控制
        if (change.Property == LeftAddOnProperty ||
            change.Property == RightAddOnProperty)
        {
            if (change.OldValue is Control oldControl)
            {
                UIStructureUtils.SetTemplateParent(oldControl, null);
            }

            if (change.NewValue is Control newControl)
            {
                UIStructureUtils.SetTemplateParent(newControl, this);
            }
        }
    }
}