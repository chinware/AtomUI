using AtomUI.TokenSystem;
using Avalonia.Media;

namespace AtomUI.Controls.Window;

[ControlDesignToken]
public class WindowToken : AbstractControlDesignToken
{
   public const string ID = "Window";
   /// <summary>
   /// 窗口默认的背景色
   /// </summary>
   public Color DefaultBackground { get; set; }
   
   /// <summary>
   /// 窗口默认的前景色
   /// </summary>
   public Color DefaultForeground { get; set; }
   
   public WindowToken()
      : base("Window")
   {
   }
}