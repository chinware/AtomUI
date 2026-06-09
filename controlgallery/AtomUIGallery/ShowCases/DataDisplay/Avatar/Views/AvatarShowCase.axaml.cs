using System.Reactive.Disposables.Fluent;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Avatar;

public partial class AvatarShowCase : ReactiveUserControl<AvatarViewModel>
{
    public const string LanguageId = nameof(AvatarShowCase);

    public AvatarShowCase()
    {
        InitializeComponent();
        this.WhenActivated(disposables =>
        {
            if (DataContext is AvatarViewModel viewModel)
            {
                this.BindCommand(viewModel, vm => vm.ChangeUserCommand, v => v.ChangeUserButton)
                    .DisposeWith(disposables);
                this.BindCommand(viewModel, vm => vm.ChangeGapCommand, v => v.ChangeGapButton)
                    .DisposeWith(disposables);
            }
        });
    }
}
