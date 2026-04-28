using AtomUIGallery.Workspace.Views;
using Avalonia;

namespace AtomUIGallery;

public partial class BaseGalleryApplication : Application
{
    protected WorkspaceWindow CreateWorkspaceWindow()
    {
        return new WorkspaceWindow();
    }
}