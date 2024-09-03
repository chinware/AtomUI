using Avalonia;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace AtomUI.Controls.Utils;

public static class FocusUtils
{
   public static FocusManager? GetFocusManager(IInputElement? element)
   {
      // Element might not be a visual, and not attached to the root.
      // But IFocusManager is always expected to be a FocusManager. 
      return (FocusManager?)((element as Visual)?.GetVisualRoot() as IInputRoot)?.FocusManager
             // In our unit tests some elements might not have a root. Remove when we migrate to headless tests.
             ?? (FocusManager?)AvaloniaLocator.Current.GetService<IFocusManager>();
   }
}