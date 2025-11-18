using Avalonia.Controls;
using Avalonia.Input;

namespace AtomUI.Desktop.Controls;

internal class GlobalDialogManager : Panel
{
    public GlobalDialogManager()
    {
        Cursor = new Cursor(StandardCursorType.Arrow);
    }
}