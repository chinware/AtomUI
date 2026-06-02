using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Grid;

public partial class GridShowCase : ReactiveUserControl<GridViewModel>
{
    public const string LanguageId = nameof(GridShowCase);

    public GridShowCase()
    {
        this.WhenActivated(disposables =>
        {
        });
        InitializeComponent();
    }
}
