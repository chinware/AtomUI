using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.Watermark;

public partial class WatermarkShowCase : GalleryReactiveUserControl<WatermarkViewModel>
{
    public const string LanguageId = nameof(WatermarkShowCase);

    public WatermarkShowCase()
    {
        InitializeComponent();
    }

}
