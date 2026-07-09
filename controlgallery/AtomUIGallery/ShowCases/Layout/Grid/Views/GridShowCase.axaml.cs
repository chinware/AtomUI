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
