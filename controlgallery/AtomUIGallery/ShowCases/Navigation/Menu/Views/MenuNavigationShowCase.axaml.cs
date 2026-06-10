using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Menu;

public partial class MenuNavigationShowCase : GalleryReactiveUserControl<MenuViewModel>
{
    public MenuNavigationShowCase()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            if (DataContext is MenuViewModel viewModel)
            {
                ChangeModeSwitch.IsCheckedChanged  += viewModel.HandleChangeModeCheckChanged;
                ChangeStyleSwitch.IsCheckedChanged += viewModel.HandleChangeStyleCheckChanged;

                Disposable.Create(() =>
                {
                    ChangeModeSwitch.IsCheckedChanged  -= viewModel.HandleChangeModeCheckChanged;
                    ChangeStyleSwitch.IsCheckedChanged -= viewModel.HandleChangeStyleCheckChanged;
                }).DisposeWith(disposables);
            }
        });
    }
}
