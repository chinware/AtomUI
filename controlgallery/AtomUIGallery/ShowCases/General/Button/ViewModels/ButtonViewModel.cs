using System.Collections.ObjectModel;
using AtomUI;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Button;

public class ButtonViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    public static EntityKey ID = "Button";

    public IScreen HostScreen { get; }
    public ViewModelActivator Activator { get; }

    public string? UrlPathSegment => ID.ToString();

    private CustomizableSizeType _buttonSizeType;
    private ButtonIconPlacement _buttonIconPlacement;

    public CustomizableSizeType ButtonSizeType
    {
        get => _buttonSizeType;
        set => this.RaiseAndSetIfChanged(ref _buttonSizeType, value);
    }

    public ButtonIconPlacement ButtonIconPlacement
    {
        get => _buttonIconPlacement;
        set => this.RaiseAndSetIfChanged(ref _buttonIconPlacement, value);
    }

    public ButtonViewModel(IScreen screen)
    {
        Activator  = new ViewModelActivator();
        HostScreen = screen;
    }

}
