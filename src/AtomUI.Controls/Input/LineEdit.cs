using AtomUI.Controls.Utils;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

public class LineEdit : TextBox
{
    #region 公共属性定义

    public static readonly StyledProperty<object?> LeftAddOnProperty =
        AvaloniaProperty.Register<LineEdit, object?>(nameof(LeftAddOn));

    public static readonly StyledProperty<object?> RightAddOnProperty =
        AvaloniaProperty.Register<LineEdit, object?>(nameof(RightAddOn));

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

    #endregion

    #region 内部属性定义

    private ControlTokenResourceRegister _controlTokenResourceRegister;
    private bool _initialized;
    #endregion

    public LineEdit()
    {
        _controlTokenResourceRegister = new ControlTokenResourceRegister(this, LineEditToken.ID);
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        if (!_initialized)
        {
            _controlTokenResourceRegister.RegisterResources();
            _initialized = true;
        }
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