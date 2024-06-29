using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;


namespace AtomUI.Controls;

using AvaloniaPopupFlyoutBase = Avalonia.Controls.Primitives.PopupFlyoutBase;

/// <summary>
/// 最基本得弹窗 Flyout，在这里不处理那种带箭头得
/// </summary>
public abstract class PopupFlyoutBase : AvaloniaPopupFlyoutBase
{
   /// <summary>
   /// 是否显示指示箭头
   /// </summary>
   public static readonly StyledProperty<bool> IsShowArrowProperty =
      ArrowDecoratedBox.IsShowArrowProperty.AddOwner<PopupFlyoutBase>();

   /// <summary>
   /// 箭头是否始终指向中心
   /// </summary>
   public static readonly StyledProperty<bool> IsPointAtCenterProperty =
      AvaloniaProperty.Register<PopupFlyoutBase, bool>(nameof(IsPointAtCenter), false);
   
   /// <summary>
   /// 距离 anchor 的边距，根据垂直和水平进行设置
   /// 但是对某些组合无效，比如跟随鼠标的情况
   /// 还有些 anchor 和 gravity 的组合也没有用 
   /// </summary>
   public static readonly StyledProperty<double> MarginToAnchorProperty =
      AvaloniaProperty.Register<PopupFlyoutBase, double>(nameof(MarginToAnchor), 0);
   
   public bool IsShowArrow
   {
      get => GetValue(IsShowArrowProperty);
      set => SetValue(IsShowArrowProperty, value);
   }

   public bool IsPointAtCenter
   {
      get => GetValue(IsPointAtCenterProperty);
      set => SetValue(IsPointAtCenterProperty, value);
   }
   
   public double MarginToAnchor
   {
      get => GetValue(MarginToAnchorProperty);
      set => SetValue(MarginToAnchorProperty, value);
   }
   
   public PopupFlyoutBase()
   {
   }
   
   internal static void SetPresenterClasses(Control? presenter, Classes classes)
   {
      if (presenter is null) {
         return;
      }

      //Remove any classes no longer in use, ignoring pseudo classes
      for (int i = presenter.Classes.Count - 1; i >= 0; i--) {
         if (!classes.Contains(presenter.Classes[i]) &&
             !presenter.Classes[i].Contains(':')) {
            presenter.Classes.RemoveAt(i);
         }
      }

      //Add new classes
      presenter.Classes.AddRange(classes);
   }

   internal virtual void NotifyPopupCreated(Popup popup) { }
}