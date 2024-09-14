using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

internal class TimePickerFlyoutPresenter : FlyoutPresenter
{
    protected override Type StyleKeyOverride => typeof(TimePickerFlyoutPresenter);
    internal TimePickerPresenter? TimePickerPresenter;
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        TimePickerPresenter = e.NameScope.Get<TimePickerPresenter>(TimePickerFlyoutPresenterTheme.TimePickerPresenterPart);
    }
}