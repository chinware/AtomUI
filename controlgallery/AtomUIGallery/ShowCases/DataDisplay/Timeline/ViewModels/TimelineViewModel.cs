using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Timeline;

public class TimelineViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Timeline";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    private bool _reverseTimelineIsReverse;
    private TimelineMode _selectedTimelineMode = TimelineMode.Left;

    public bool ReverseTimelineIsReverse
    {
        get => _reverseTimelineIsReverse;
        set => this.RaiseAndSetIfChanged(ref _reverseTimelineIsReverse, value);
    }

    public TimelineMode SelectedTimelineMode
    {
        get => _selectedTimelineMode;
        set => this.RaiseAndSetIfChanged(ref _selectedTimelineMode, value);
    }

    public TimelineViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

}
