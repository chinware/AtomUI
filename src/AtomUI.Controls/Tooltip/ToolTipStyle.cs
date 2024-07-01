using AtomUI.Data;
using AtomUI.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Controls;

public partial class ToolTip : IControlCustomStyle
{
   private bool _initialized = false;
   private IControlCustomStyle _customStyle;
   private ControlTokenBinder _controlTokenBinder;
   private ArrowDecoratedBox? _arrowDecoratedBox;

   void IControlCustomStyle.SetupUi()
   {
      Background = new SolidColorBrush(Colors.Transparent);
      _arrowDecoratedBox = new ArrowDecoratedBox();
      if (Content is string text) {
         _arrowDecoratedBox.Child = new TextBlock
         {
            Text = text,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            TextWrapping = TextWrapping.Wrap,
         };
      } else if (Content is Control control) {
         _arrowDecoratedBox.Child = control;
      }

      ((ISetLogicalParent)_arrowDecoratedBox).SetParent(this);
      VisualChildren.Add(_arrowDecoratedBox);
      _customStyle.ApplyFixedStyleConfig();
   }

   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      if (_arrowDecoratedBox is not null) {
         _controlTokenBinder.AddControlBinding(_arrowDecoratedBox, FontSizeProperty, GlobalResourceKey.FontSize);
         _controlTokenBinder.AddControlBinding(_arrowDecoratedBox, MaxWidthProperty, ToolTipResourceKey.ToolTipMaxWidth);
         _controlTokenBinder.AddControlBinding(_arrowDecoratedBox, BackgroundProperty, ToolTipResourceKey.ToolTipBackground);
         _controlTokenBinder.AddControlBinding(_arrowDecoratedBox, ForegroundProperty, ToolTipResourceKey.ToolTipColor);
         _controlTokenBinder.AddControlBinding(_arrowDecoratedBox, MinHeightProperty, GlobalResourceKey.ControlHeight);
         _controlTokenBinder.AddControlBinding(_arrowDecoratedBox, PaddingProperty, ToolTipResourceKey.ToolTipPadding);
         _controlTokenBinder.AddControlBinding(MarginXXSTokenProperty, GlobalResourceKey.MarginXXS);
         _controlTokenBinder.AddControlBinding(_arrowDecoratedBox, ArrowDecoratedBox.CornerRadiusProperty, ToolTipResourceKey.BorderRadiusOuter);
      }
   }
   
   public CornerRadius GetMaskCornerRadius()
   {
      return _arrowDecoratedBox!.GetMaskCornerRadius();
   }

   public Rect GetMaskBounds()
   {
      return _arrowDecoratedBox!.GetMaskBounds();
   }
   
   private void SetupArrowPosition(PlacementMode placement, PopupAnchor? anchor = null, PopupGravity? gravity = null)
   {
      var arrowPosition = PopupUtils.CalculateArrowPosition(placement, anchor, gravity);
      if (_arrowDecoratedBox is not null && arrowPosition is not null) {
         _arrowDecoratedBox.ArrowPosition = arrowPosition.Value;
      }
   }

   private void SetupPointCenterOffset()
   {
      var offset =
         CalculatePopupPositionDelta(_popup!.PlacementTarget!, _popup.Placement, _popup.PlacementAnchor, _popup.PlacementGravity);
      _popup.HorizontalOffset += offset.X;
      _popup.VerticalOffset += offset.Y;
   }
   
   private Point CalculatePopupPositionDelta(Control anchorTarget, PlacementMode placement, PopupAnchor? anchor = null,
                                             PopupGravity? gravity = null)
   {
      var offsetX = 0d;
      var offsetY = 0d;
      if (GetIsShowArrow(AdornedControl!) && GetIsPointAtCenter(AdornedControl!)) {
         if (PopupUtils.CanEnabledArrow(placement, anchor, gravity)) {
            var arrowVertexPoint = _arrowDecoratedBox!.ArrowVertexPoint;
            var anchorSize = anchorTarget.Bounds.Size;
            var centerX = anchorSize.Width / 2;
            var centerY = anchorSize.Height / 2;
            // 这里计算不需要全局坐标
            if (placement == PlacementMode.TopEdgeAlignedLeft ||
                placement == PlacementMode.BottomEdgeAlignedLeft) {
               offsetX += centerX - arrowVertexPoint.Item1;
            } else if (placement == PlacementMode.TopEdgeAlignedRight ||
                       placement == PlacementMode.BottomEdgeAlignedRight) {
               offsetX -= centerX - arrowVertexPoint.Item2;
            } else if (placement == PlacementMode.RightEdgeAlignedTop ||
                       placement == PlacementMode.LeftEdgeAlignedTop) {
               offsetY += centerY - arrowVertexPoint.Item1;
            } else if (placement == PlacementMode.RightEdgeAlignedBottom ||
                       placement == PlacementMode.LeftEdgeAlignedBottom) {
               offsetY -= centerY - arrowVertexPoint.Item2;
            }
         }
      }
      return new Point(offsetX, offsetY);
   }
}