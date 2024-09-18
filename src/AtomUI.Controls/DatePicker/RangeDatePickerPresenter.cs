using Avalonia;

namespace AtomUI.Controls;

internal class RangeDatePickerPresenter : DatePickerPresenter
{
    #region 公共属性定义

    public static readonly StyledProperty<DateTime?> SecondarySelectedDateTimeProperty =
        AvaloniaProperty.Register<RangeDatePickerPresenter, DateTime?>(nameof(SecondarySelectedDateTime));
    
    public DateTime? SecondarySelectedDateTime
    {
        get => GetValue(SecondarySelectedDateTimeProperty);
        set => SetValue(SecondarySelectedDateTimeProperty, value);
    }

    #endregion
}