using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

using AvaloniaMenu = Avalonia.Controls.Menu;

public class Menu : AvaloniaMenu,
                    ISizeTypeAware
{
   #region 公共属性定义

   public static readonly StyledProperty<SizeType> SizeTypeProperty =
      AvaloniaProperty.Register<Button, SizeType>(nameof(SizeType), SizeType.Middle);

   public SizeType SizeType
   {
      get => GetValue(SizeTypeProperty);
      set => SetValue(SizeTypeProperty, value);
   }

   #endregion
   
   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (ItemContainerTheme is null) {
         BindUtils.CreateGlobalResourceBinding(this, ItemContainerThemeProperty, TopLevelMenuItemTheme.ID);
      }
   }
}