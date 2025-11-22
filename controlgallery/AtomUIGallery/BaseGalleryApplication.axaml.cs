using AtomUIGallery.ShowCases;
using AtomUIGallery.Workspace.Views;
using Avalonia;

namespace AtomUIGallery;

public partial class BaseGalleryAtomApplication : Application
{
    protected WorkspaceWindow CreateWorkspaceWindow()
    {
        return new WorkspaceWindow();
    }

    public override void RegisterServices()
    {
        base.RegisterServices();
        ShowCaseRegister.Register();
    }
}