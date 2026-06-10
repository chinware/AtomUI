using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Calendar;

public partial class CalendarShowCase : GalleryReactiveUserControl<CalendarViewModel>
{
    public const string LanguageId = nameof(CalendarShowCase);

    public CalendarShowCase()
    {
        InitializeComponent();
    }
}
