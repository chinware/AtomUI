using AtomUI.Controls.Window;
using AtomUI.Styling;
using Avalonia.Styling;

namespace AtomUI.Controls.Controls.Window;

public class CaptionButtonThemeProvider
{
   public object Key { get; } = typeof(CaptionButton);
   public Type TargetType { get; } = typeof(Button);

   public ControlTheme BuildControlTheme()
   {
      return default!;
   }
}