using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class CalendarShowCase : ReactiveUserControl<CalendarViewModel>
{
    public CalendarShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}