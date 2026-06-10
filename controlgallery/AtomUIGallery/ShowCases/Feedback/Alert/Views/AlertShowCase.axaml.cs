using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Alert;

public partial class AlertShowCase : GalleryReactiveUserControl<AlertViewModel>
{
    public const string LanguageId = nameof(AlertShowCase);

    public AlertShowCase()
    {
        InitializeComponent();
    }
}
