using AtomUI.Theme;
using AtomUI.Theme.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class WindowNotificationManagerTheme : BaseControlTheme
{
   public const string ItemsPart = "PART_Items";
   
   public WindowNotificationManagerTheme() : base(typeof(WindowNotificationManager))
   {
   }
   
   
}