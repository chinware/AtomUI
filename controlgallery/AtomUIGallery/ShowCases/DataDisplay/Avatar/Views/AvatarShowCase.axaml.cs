using System.Reactive.Disposables.Fluent;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Avatar;

public partial class AvatarShowCase : GalleryReactiveUserControl<AvatarViewModel>
{
    public const string LanguageId = nameof(AvatarShowCase);

    public AvatarShowCase()
    {
        InitializeComponent();
        this.WhenActivated(disposables =>
        {
            if (DataContext is AvatarViewModel viewModel)
            {
                GalleryBindingUtils.BindCommand(ChangeUserButton, viewModel.ChangeUserCommand).DisposeWith(disposables);
                GalleryBindingUtils.BindCommand(ChangeGapButton, viewModel.ChangeGapCommand).DisposeWith(disposables);
            }
        });
    }
}
