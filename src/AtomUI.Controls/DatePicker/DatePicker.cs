using AtomUI.Controls.Internal;
using Avalonia.Controls;

namespace AtomUI.Controls;

public class DatePicker : InfoPickerInput
{

    private DatePickerPresenter? _pickerPresenter;
    
    protected override Flyout CreatePickerFlyout()
    {
        return new DatePickerFlyout();
    }
    
    protected override void NotifyFlyoutPresenterCreated(Control flyoutPresenter)
    {
        if (flyoutPresenter is DatePickerFlyoutPresenter datePickerFlyoutPresenter)
        {
            datePickerFlyoutPresenter.AttachedToVisualTree += (sender, args) =>
            {
                _pickerPresenter = datePickerFlyoutPresenter.DatePickerPresenter;
                ConfigurePickerPresenter(_pickerPresenter);
            };
        }
    }

    private void ConfigurePickerPresenter(DatePickerPresenter? presenter)
    {
        if (presenter is null)
        {
            return;
        }

        presenter.ChoosingStatueChanged += (sender, args) =>
        {
            _isChoosing = args.IsChoosing;
            UpdatePseudoClasses();
        };
    }
}