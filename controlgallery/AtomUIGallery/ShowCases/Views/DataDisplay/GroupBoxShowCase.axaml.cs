using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class GroupBoxShowCase : ReactiveUserControl<GroupBoxViewModel>
{
    public GroupBoxShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}