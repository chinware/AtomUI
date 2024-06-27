using Avalonia.Controls;

namespace AtomUI.Controls;

public class Flyout : PopupFlyoutBase
{
   protected override Control CreatePresenter()
   {
      return default!;
   }
}