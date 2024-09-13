using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;

namespace AtomUI.Controls;

internal class DatePickerPresenter : PickerPresenterBase
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsNeedConfirmProperty =
        AvaloniaProperty.Register<DatePickerPresenter, bool>(nameof(IsNeedConfirm));

    public bool IsNeedConfirm
    {
        get => GetValue(IsNeedConfirmProperty);
        set => SetValue(IsNeedConfirmProperty, value);
    }

    #endregion

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        TokenResourceBinder.CreateGlobalTokenBinding(this, BorderThicknessProperty, GlobalTokenResourceKey.BorderThickness, BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this, thickness => new Thickness(0, thickness.Top, 0, 0)));
    }
}