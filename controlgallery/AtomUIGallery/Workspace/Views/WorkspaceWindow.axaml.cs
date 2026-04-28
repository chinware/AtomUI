using AtomUI.Desktop.Controls;
using AtomUIGallery.Workspace.ViewModels;
using ReactiveUI;

namespace AtomUIGallery.Workspace.Views;

public partial class WorkspaceWindow : ReactiveWindow<WorkspaceWindowViewModel>
{
    public const string LanguageId = nameof(WorkspaceWindow);

    public WorkspaceWindow()
    {
        ViewModel = new WorkspaceWindowViewModel();
        InitializeComponent();
        this.WhenActivated(disposables =>
        {
        });
    }
}
