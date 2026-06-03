using System.Reactive.Disposables.Fluent;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Menu;

public partial class MenuItemsSourceShowCase : ReactiveUserControl<MenuViewModel>
{
    public MenuItemsSourceShowCase()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, vm => vm.MenuItems, v => v.BasicItemsSourceMenu.ItemsSource)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.InlineNavMenuNodes, v => v.InlineModeMenu.ItemsSource)
                .DisposeWith(disposables);
            this.OneWayBind(ViewModel, vm => vm.ItemsSourceDemoNavMenuNodes, v => v.ItemsSourceDemoNavMenu.ItemsSource)
                .DisposeWith(disposables);
        });
    }
}
