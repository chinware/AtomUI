using AtomUI.Toolkits.GalleryBase.Navigation;
using ReactiveUI;

namespace AtomUIGallery.Workspace.ViewModels;

public class CaseNavigationViewModel : GalleryNavigationViewModel
{
    public CaseNavigationViewModel(IScreen hostScreen)
        : base(hostScreen, AtomUIGalleryModule.GetConfiguration())
    {
    }
}
