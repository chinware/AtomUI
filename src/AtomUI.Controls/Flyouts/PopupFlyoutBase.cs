using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace AtomUI.Controls;

using AvaloniaPopupFlyoutBase = Avalonia.Controls.Primitives.PopupFlyoutBase;
using PopupControl = Popup;

/// <summary>
/// 最基本得弹窗 Flyout，在这里不处理那种带箭头得
/// </summary>
public abstract class PopupFlyoutBase : AvaloniaPopupFlyoutBase
{
   /// <summary>
   /// 距离 anchor 的边距，根据垂直和水平进行设置
   /// 但是对某些组合无效，比如跟随鼠标的情况
   /// 还有些 anchor 和 gravity 的组合也没有用 
   /// </summary>
   public static readonly StyledProperty<double> MarginToAnchorProperty =
      PopupControl.MarginToAnchorProperty.AddOwner<PopupFlyoutBase>();
   
   public double MarginToAnchor
   {
      get => GetValue(MarginToAnchorProperty);
      set => SetValue(MarginToAnchorProperty, value);
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

   protected internal virtual void NotifyPopupCreated(Popup popup)
   {
      BindUtils.RelayBind(this, MarginToAnchorProperty, popup);
   }
   
   protected internal virtual void NotifyPositionPopup(bool showAtPointer)
   {
      Size sz;
      // Popup.Child can't be null here, it was set in ShowAtCore.
      if (Popup.Child!.DesiredSize == default) {
         // Popup may not have been shown yet. Measure content
         sz = LayoutHelper.MeasureChild(Popup.Child, Size.Infinity, new Thickness());
      } else {
         sz = Popup.Child.DesiredSize;
      }

      Popup.VerticalOffset = VerticalOffset;
      Popup.HorizontalOffset = HorizontalOffset;
      
      Popup.PlacementAnchor = PlacementAnchor;
      Popup.PlacementGravity = PlacementGravity;
      
      if (showAtPointer) {
         Popup.Placement = PlacementMode.Pointer;
      } else {
         Popup.Placement = Placement;
         Popup.PlacementConstraintAdjustment = PlacementConstraintAdjustment;
      }
   }
}