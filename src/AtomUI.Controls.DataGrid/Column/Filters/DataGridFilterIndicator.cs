using AtomUI.IconPkg.AntDesign;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;

namespace AtomUI.Controls;

internal class DataGridFilterIndicator : IconButton
{
    #region 公共属性定义
    
    public static readonly StyledProperty<bool> IsFilterActivatedProperty
        = AvaloniaProperty.Register<DataGridFilterIndicator, bool>(nameof(IsFilterActivated));
    
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