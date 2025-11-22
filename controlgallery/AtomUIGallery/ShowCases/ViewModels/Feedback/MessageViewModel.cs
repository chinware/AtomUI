using AtomUI.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ViewModels;

public class MessageViewModel : ReactiveObject, IRoutableViewModel
{
    public static TreeNodeKey ID = "Message";
    
    public IScreen HostScreen { get; }
    
    public string UrlPathSegment { get; } = ID.ToString();

    public MessageViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}