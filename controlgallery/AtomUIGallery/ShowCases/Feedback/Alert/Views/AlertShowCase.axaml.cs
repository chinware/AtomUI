
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.Alert;

public partial class AlertShowCase : GalleryReactiveUserControl<AlertViewModel>
{
    public const string LanguageId = nameof(AlertShowCase);

    public AlertShowCase()
    {
        InitializeComponent();
    }

}
