using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Calendar;

public partial class CalendarShowCase : ReactiveUserControl<CalendarViewModel>
{
    public const string LanguageId = nameof(CalendarShowCase);

    public CalendarShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}
