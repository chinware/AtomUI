using Avalonia;

namespace AtomUI.Controls;

using AvaloniaTabStripItem = Avalonia.Controls.Primitives.TabStripItem;

public class TabStripItem : AvaloniaTabStripItem
{
   #region 公共属性定义
   public static readonly StyledProperty<SizeType> SizeTypeProperty =
      TabStrip.SizeTypeProperty.AddOwner<TabStrip>();

   public static readonly StyledProperty<PathIcon?> IconProperty =
      AvaloniaProperty.Register<TabStripItem, PathIcon?>(nameof(Icon));
   
   public static readonly StyledProperty<PathIcon?> CloseIconProperty =
      AvaloniaProperty.Register<TabStripItem, PathIcon?>(nameof(CloseIcon));
   
   public static readonly StyledProperty<bool> ClosableProperty =
      AvaloniaProperty.Register<TabStripItem, bool>(nameof(Closable));
   
   public SizeType SizeType
   {
      get => GetValue(SizeTypeProperty);
      set => SetValue(SizeTypeProperty, value);
   }
   
   public PathIcon? Icon
   {
      get => GetValue(IconProperty);
      set => SetValue(IconProperty, value);
   }
   
   public PathIcon? CloseIcon
   {
      get => GetValue(CloseIconProperty);
      set => SetValue(CloseIconProperty, value);
   }
   
   public bool Closable
   {
      get => GetValue(ClosableProperty);
      set => SetValue(ClosableProperty, value);
   }
   #endregion
   
   #region 内部属性定义
   internal static readonly StyledProperty<TabSharp> ShapeProperty =
      AvaloniaProperty.Register<TabStripItem, TabSharp>(nameof(Shape));
   
   public TabSharp Shape
   {
      get => GetValue(ShapeProperty);
      set => SetValue(ShapeProperty, value);
   }
   #endregion
}