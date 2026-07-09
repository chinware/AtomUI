namespace AtomUIGallery.ShowCases.CustomizeTheme;

public partial class CustomizeThemeShowCase : GalleryReactiveUserControl<CustomizeThemeViewModel>
{
    public const string LanguageId = nameof(CustomizeThemeShowCase);

    public CustomizeThemeShowCase()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

    }
}
