using Avalonia;

namespace AtomUI.Controls;

using AvaloniaTabStrip = Avalonia.Controls.Primitives.TabStrip;

public enum TabSharp
{
   Line,
   Card
}

public abstract class BaseTabStrip : AvaloniaTabStrip, ISizeTypeAware
{
   #region 公共属性定义
   public static readonly StyledProperty<SizeType> SizeTypeProperty =
      AvaloniaProperty.Register<TabStrip, SizeType>(nameof(SizeType), SizeType.Middle);
   
   public SizeType SizeType
   {
      get => GetValue(SizeTypeProperty);
      set => SetValue(SizeTypeProperty, value);
   }
   #endregion
}