using AtomUIGallery.Workspace.Views;
using Avalonia;

namespace AtomUIGallery;

public abstract partial class BaseGalleryApplication : Application
{
    public BaseGalleryApplication()
    {
    }

    protected WorkspaceWindow CreateWorkspaceWindow()
    {
        return new WorkspaceWindow();
    }
}
