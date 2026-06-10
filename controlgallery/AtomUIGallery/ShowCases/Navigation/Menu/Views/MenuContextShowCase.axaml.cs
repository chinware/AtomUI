using System.Reactive.Disposables.Fluent;
using ReactiveUI;
using ReactiveUI.Avalonia;

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
