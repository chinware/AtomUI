using Avalonia;
using Avalonia.Controls;
using ScenarioTabStripItem = AtomUI.Desktop.Controls.TabStripItem;

namespace AtomUIGallery.ShowCases.Grid;

public partial class GridShowCase : GalleryReactiveUserControl<GridViewModel>
{
    public const string LanguageId = nameof(GridShowCase);

    public GridShowCase()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

    }
}
