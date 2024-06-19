using Avalonia.Styling;

namespace AtomUI.Styling;

public interface IControlThemeProvider
{
   public object Key { get; }
   public Type TargetType { get; }

   public ControlTheme BuildControlTheme();
}