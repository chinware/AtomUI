namespace AtomUI;

using AvaloniaControlTheme = Avalonia.Styling.ControlTheme;

public class ControlTheme : AvaloniaControlTheme
{
   public ControlTheme() { }
   public ControlTheme(Type targetType) : base(targetType) {}
   
   public void Build()
   {
      BuildDefaultStyles();
      BuildTriggerStyles();
      BuildControlTemplate();
   }
   
   public virtual void BuildControlTemplate() {}
   public virtual void BuildDefaultStyles() {}
   public virtual void BuildTriggerStyles() {}
}