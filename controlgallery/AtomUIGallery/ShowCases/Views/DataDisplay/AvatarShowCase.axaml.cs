using System.Reactive.Disposables.Fluent;
using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class AvatarShowCase : ReactiveUserControl<AvatarViewModel>
{
    public AvatarShowCase()
    {
        InitializeComponent();
        this.WhenActivated(disposables =>
        {
            this.BindCommand(ViewModel!, vm => vm.ChangeUserCommand, v => v.ChangeUserButton)
                .DisposeWith(disposables);
            this.BindCommand(ViewModel!, vm => vm.ChangeGapCommand, v => v.ChangeGapButton)
                .DisposeWith(disposables);
        });
    }
}
