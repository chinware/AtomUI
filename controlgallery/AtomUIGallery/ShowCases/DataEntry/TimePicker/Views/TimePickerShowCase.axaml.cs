using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.TimePicker;

public partial class TimePickerShowCase : ReactiveUserControl<TimePickerViewModel>
{
    public TimePickerShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}
