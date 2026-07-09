namespace AtomUIGallery.ShowCases.PopupConfirm;

public partial class PopupConfirmShowCase : GalleryReactiveUserControl<PopupConfirmViewModel>
{
    public const string LanguageId = nameof(PopupConfirmShowCase);

    public PopupConfirmShowCase()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

    }
}
