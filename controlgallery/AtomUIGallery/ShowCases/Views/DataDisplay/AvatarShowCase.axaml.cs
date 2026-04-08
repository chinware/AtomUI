using System.Reactive.Disposables;
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
            if (DataContext is AvatarViewModel viewModel)
            {
                ChangeUserButton.Click += viewModel.HandleChangeUserClicked;
                ChangeGapButton.Click  += viewModel.HandleChangeGapClicked;
                Disposable.Create(() =>
                {
                    ChangeUserButton.Click -= viewModel.HandleChangeUserClicked;
                    ChangeGapButton.Click  -= viewModel.HandleChangeGapClicked;
                }).DisposeWith(disposables);
            }
        });
    }
}