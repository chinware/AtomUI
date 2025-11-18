using AtomUI.Desktop.Controls.Converters;
using Avalonia.Styling;

namespace AtomUI.Desktop.Controls.Themes;

internal class OverlayDialogHostTheme : ControlTheme
{
    public static OverlayDialogResizerVisibleConverter OverlayDialogResizerVisibleConverter = new OverlayDialogResizerVisibleConverter();
}