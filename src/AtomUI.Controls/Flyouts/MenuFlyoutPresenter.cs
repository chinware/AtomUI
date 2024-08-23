using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

public class MenuFlyoutPresenter : MenuBase
{
   #region 公共属性定义

   public static readonly StyledProperty<bool> IsShowArrowProperty =
      ArrowDecoratedBox.IsShowArrowProperty.AddOwner<MenuFlyoutPresenter>();

   public static readonly StyledProperty<ArrowPosition> ArrowPositionProperty =
      ArrowDecoratedBox.ArrowPositionProperty.AddOwner<MenuFlyoutPresenter>();
   
   /// <summary>
   /// 是否显示指示箭头
   /// </summary>
   public bool IsShowArrow
   {
      get => GetValue(IsShowArrowProperty);
      set => SetValue(IsShowArrowProperty, value);
   }

   /// <summary>
   /// 箭头渲染的位置
   /// </summary>
   public ArrowPosition ArrowPosition
   {
      get => GetValue(ArrowPositionProperty);
      set => SetValue(ArrowPositionProperty, value);
   }

   #endregion
   
   public MenuFlyoutPresenter()
      : base(new DefaultMenuInteractionHandler(true)) { }

   public MenuFlyoutPresenter(IMenuInteractionHandler menuInteractionHandler)
      : base(menuInteractionHandler) { }


   public override void Close()
   {
      // DefaultMenuInteractionHandler calls this
      var host = this.FindLogicalAncestorOfType<Popup>();
      if (host != null) {
         SelectedIndex = -1;
         host.IsOpen = false;
      }
   }

   public override void Open()
   {
      throw new NotSupportedException("Use MenuFlyout.ShowAt(Control) instead");
   }

   protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnDetachedFromVisualTree(e);

      foreach (var i in LogicalChildren) {
         if (i is MenuItem menuItem) {
            menuItem.IsSubMenuOpen = false;
         }
      }
   }
}