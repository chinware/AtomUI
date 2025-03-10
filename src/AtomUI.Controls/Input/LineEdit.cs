using AtomUI.Controls.Utils;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Controls;

public class LineEdit : TextBox,
                        IMotionAwareControl
{
    #region 公共属性定义

    public static readonly StyledProperty<object?> LeftAddOnProperty =
        AvaloniaProperty.Register<LineEdit, object?>(nameof(LeftAddOn));

    public static readonly StyledProperty<object?> RightAddOnProperty =
        AvaloniaProperty.Register<LineEdit, object?>(nameof(RightAddOn));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<LineEdit>();

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

    #endregion

    #region 内部属性定义

    Control IMotionAwareControl.PropertyBindTarget => this;

    #endregion

    public LineEdit()
    {
        this.BindMotionProperties();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == LeftAddOnProperty ||
            change.Property == RightAddOnProperty)
        {
            if (change.OldValue is Control oldControl)
            {
                oldControl.SetTemplatedParent(null);
            }

            if (change.NewValue is Control newControl)
            {
                newControl.SetTemplatedParent(this);
            }
        }
    }
}