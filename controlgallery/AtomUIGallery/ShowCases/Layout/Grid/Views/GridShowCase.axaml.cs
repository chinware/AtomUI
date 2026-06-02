using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Grid;

public partial class GridShowCase : ReactiveUserControl<GridViewModel>
{
    public GridShowCase()
    {
        this.WhenActivated(disposables =>
        {
        });
        InitializeComponent();
    }
}
