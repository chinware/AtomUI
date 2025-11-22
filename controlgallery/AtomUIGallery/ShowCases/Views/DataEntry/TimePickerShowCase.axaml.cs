using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class TimePickerShowCase : ReactiveUserControl<TimePickerViewModel>
{
    public TimePickerShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}