using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Calendar;

public partial class CalendarShowCase : ReactiveUserControl<CalendarViewModel>
{
    public CalendarShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}
