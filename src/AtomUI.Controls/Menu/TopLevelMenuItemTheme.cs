namespace AtomUI.Controls;

public class TopLevelMenuItemTheme : ControlTheme
{
   public const string ID = "TopLevelMenuItem";
   public TopLevelMenuItemTheme() : base(typeof(MenuItem)) {}
   
   public override string? ThemeResourceKey()
   {
      return ID;
   }
   
}