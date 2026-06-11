using System.Reactive.Disposables.Fluent;

namespace AtomUIGallery.ShowCases.Menu;

public partial class MenuItemsSourceShowCase : GalleryReactiveUserControl<MenuViewModel>
{
    public MenuItemsSourceShowCase()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            if (ViewModel is not null)
            {
                GalleryBindingUtils.OneWay(ViewModel, nameof(MenuViewModel.MenuItems), vm => vm.MenuItems,
                                           BasicItemsSourceMenu, Avalonia.Controls.ItemsControl.ItemsSourceProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(ViewModel, nameof(MenuViewModel.InlineNavMenuNodes),
                                           vm => vm.InlineNavMenuNodes, InlineModeMenu,
                                           Avalonia.Controls.ItemsControl.ItemsSourceProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(ViewModel, nameof(MenuViewModel.ItemsSourceDemoNavMenuNodes),
                                           vm => vm.ItemsSourceDemoNavMenuNodes, ItemsSourceDemoNavMenu,
                                           Avalonia.Controls.ItemsControl.ItemsSourceProperty)
                                   .DisposeWith(disposables);
            }
        });
    }
}
