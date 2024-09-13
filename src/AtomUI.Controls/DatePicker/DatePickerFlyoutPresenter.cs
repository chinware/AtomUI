using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

internal class DatePickerFlyoutPresenter : FlyoutPresenter
{
    protected override Type StyleKeyOverride => typeof(DatePickerFlyoutPresenter);

    internal DatePickerPresenter? DatePickerPresenter;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        DatePickerPresenter = e.NameScope.Get<DatePickerPresenter>(DatePickerFlyoutPresenterTheme.DatePickerPresenterPart);
    }
}