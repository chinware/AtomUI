using SplashControl = AtomUI.Desktop.Controls.Splash;

namespace AtomUIGallery.ShowCases.Splash;

internal sealed partial class GalleryWindowSplash : SplashControl
{
    public GalleryWindowSplash()
    {
        InitializeComponent();
    }

    protected override Type StyleKeyOverride { get; } = typeof(SplashControl);
}
