using AtomUI.Toolkits.GalleryBase.Shell;

namespace AtomUIGallery.Workspace.ViewModels;

public class WorkspaceWindowViewModel : GalleryWorkspaceViewModel
{
    public CaseNavigationViewModel CaseNavigation => (CaseNavigationViewModel)Navigation;

    public WorkspaceWindowViewModel()
        : base(AtomUIGalleryModule.GetConfiguration(), screen => new CaseNavigationViewModel(screen))
    {
    }
}
