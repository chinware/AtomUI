using AtomUI.Data;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

internal class RangeDatePickerFlyoutPresenter : FlyoutPresenter
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsShowTimeProperty =
        DatePicker.IsShowTimeProperty.AddOwner<RangeDatePickerFlyoutPresenter>();
    
    public bool IsShowTime
    {
        get => GetValue(IsShowTimeProperty);
        set => SetValue(IsShowTimeProperty, value);
    }

    #endregion
    
    internal RangeDatePickerPresenter? DatePickerPresenter;

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        SetupDatePickerPresenter();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == IsShowTimeProperty)
        {
            SetupDatePickerPresenter();
        }
    }

    private void SetupDatePickerPresenter()
    {
        if (IsShowTime)
        {
            DatePickerPresenter = new TimedRangeDatePickerPresenter()
            {
                IsShowTime = true
            };
        }
        else
        {
            DatePickerPresenter = new DualMonthRangeDatePickerPresenter();
        }

        BindUtils.RelayBind(this, IsMotionEnabledProperty, DatePickerPresenter, IsMotionEnabledProperty);

        Content = DatePickerPresenter;
    }
}