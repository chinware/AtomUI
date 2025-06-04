using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;

namespace AtomUI.Controls;

internal class DataGridFilterIndicator : TemplatedControl
{
    #region 公共属性定义

    public static readonly StyledProperty<Icon?> IconProperty
        = AvaloniaProperty.Register<DataGridFilterIndicator, Icon?>(nameof(Icon));
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<DataGridFilterIndicator>();

    public static readonly StyledProperty<bool> IsFilterActivatedProperty
        = AvaloniaProperty.Register<DataGridFilterIndicator, bool>(nameof(IsFilterActivated));
    
    public Icon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public bool IsFilterActivated
    {
        get => GetValue(IsFilterActivatedProperty);
        set => SetValue(IsFilterActivatedProperty, value);
    }
    
    #endregion

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (Icon is null)
        {
            SetValue(IconProperty, AntDesignIconPackage.FilterFilled(), BindingPriority.Template);
        }
        base.OnApplyTemplate(e);
    }
}