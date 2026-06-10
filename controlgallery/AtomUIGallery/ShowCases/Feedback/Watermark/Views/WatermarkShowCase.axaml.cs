using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Watermark;

public partial class WatermarkShowCase : GalleryReactiveUserControl<WatermarkViewModel>
{
    public const string LanguageId = nameof(WatermarkShowCase);

    public WatermarkShowCase()
    {
        InitializeComponent();
    }
}
