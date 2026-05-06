using AtomUI.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ViewModels;

public class FloatButtonViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "FloatButton";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();
    
    private bool _isOpened;

    public bool IsOpened
    {
        get => _isOpened;
        set => this.RaiseAndSetIfChanged(ref _isOpened, value);
    }

    public FloatButtonViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}
