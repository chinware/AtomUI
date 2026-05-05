using AtomUI.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ViewModels;

public class TimePickerViewModel : ReactiveObject, IRoutableViewModel
{
    public const string ID = "TimePicker";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID;

    public TimePickerViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}
