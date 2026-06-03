using System.Reactive.Disposables.Fluent;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Menu;

public partial class MenuContextShowCase : ReactiveUserControl<MenuViewModel>
{
    public MenuContextShowCase()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, vm => vm.ContextMenuItems, v => v.BasicContextMenu.ItemsSource)
                .DisposeWith(disposables);
        });
    }
}
