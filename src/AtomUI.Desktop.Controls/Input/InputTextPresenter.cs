using Avalonia;
using Avalonia.Controls.Presenters;

namespace AtomUI.Desktop.Controls;

internal class InputTextPresenter : TextPresenter
{
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        // Avalonia 12 keeps shaped text runs cached across selection changes, so selected text can keep the old foreground.
        if (change.Property == SelectionStartProperty ||
            change.Property == SelectionEndProperty ||
            change.Property == SelectionForegroundBrushProperty ||
            change.Property == ShowSelectionHighlightProperty)
        {
            InvalidateTextLayout();
        }
    }
}
