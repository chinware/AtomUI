using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Watermark;

public partial class WatermarkShowCase : ReactiveUserControl<WatermarkViewModel>
{
    public const string LanguageId = nameof(WatermarkShowCase);

    public WatermarkShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}
