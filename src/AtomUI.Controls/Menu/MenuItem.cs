using AtomUI.Utils;
using Avalonia;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

using AvaloniaMenuItem = Avalonia.Controls.MenuItem;

public class MenuItem : AvaloniaMenuItem
{
   #region 公共属性定义

   public static readonly StyledProperty<SizeType> SizeTypeProperty =
      Menu.SizeTypeProperty.AddOwner<MenuItem>();

   public SizeType SizeType
   {
      get => GetValue(SizeTypeProperty);
      set => SetValue(SizeTypeProperty, value);
   }

   #endregion
}