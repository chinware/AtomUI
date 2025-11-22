using ReactiveUI;

namespace AtomUIGallery.Workspace.ViewModels;

public class WorkspaceWindowViewModel : ReactiveObject, IScreen
{
    public RoutingState Router { get; } = new RoutingState();
}