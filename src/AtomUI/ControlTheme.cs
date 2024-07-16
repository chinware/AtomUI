namespace AtomUI;

using AvaloniaControlTheme = Avalonia.Styling.ControlTheme;

public class ControlTheme : AvaloniaControlTheme
{
   public ControlTheme() { }
   public ControlTheme(Type targetType) : base(targetType) {}
   
   public void Build()
   {
      BuildStyles();
      BuildControlTemplate();
   }
   
   public virtual void BuildControlTemplate() {}
   public virtual void BuildStyles() {}
}