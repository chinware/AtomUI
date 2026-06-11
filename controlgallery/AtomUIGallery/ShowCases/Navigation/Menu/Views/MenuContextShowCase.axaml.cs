using System.Reactive.Disposables.Fluent;

namespace AtomUIGallery.ShowCases.Menu;

public partial class MenuContextShowCase : GalleryReactiveUserControl<MenuViewModel>
{
    public MenuContextShowCase()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            if (ViewModel is not null)
            {
                GalleryBindingUtils.OneWay(ViewModel, nameof(MenuViewModel.ContextMenuItems),
                                           vm => vm.ContextMenuItems, BasicContextMenu,
                                           Avalonia.Controls.ContextMenu.ItemsSourceProperty)
                                   .DisposeWith(disposables);
            }
        });
    }
}
