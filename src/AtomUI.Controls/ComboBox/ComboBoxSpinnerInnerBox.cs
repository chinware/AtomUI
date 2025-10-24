using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

internal class ComboBoxSpinnerInnerBox : AddOnDecoratedInnerBox
{
    #region 公共属性定义

    public static readonly StyledProperty<object?> SpinnerContentProperty =
        AvaloniaProperty.Register<ComboBoxSpinnerInnerBox, object?>(nameof(SpinnerContent));

    public object? SpinnerContent
    {
        get => GetValue(SpinnerContentProperty);
        set => SetValue(SpinnerContentProperty, value);
    }

    #endregion
}