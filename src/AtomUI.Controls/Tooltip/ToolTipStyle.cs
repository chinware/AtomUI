using AtomUI.Data;
using AtomUI.Media;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Controls;

public partial class ToolTip : IControlCustomStyle
{
   private bool _initialized = false;
   private IControlCustomStyle _customStyle;
   private TokenResourceBinder _tokenResourceBinder;
   private Geometry? _arrowGeometry;
   private Direction? _lastDirection;

   void IControlCustomStyle.SetupUi()
   {
      
      if (Content is string text) {
         Child = new TextBlock
         {
            Text = text,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            TextWrapping = TextWrapping.Wrap
         };
      } else if (Content is Control control) {
         Child = control;
      }

      _customStyle.ApplyFixedStyleConfig();
      BuildGeometry(GetDirection(GetPlacement(AdornedControl!)));
   }

   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      _tokenResourceBinder.AddBinding(FontSizeProperty, GlobalResourceKey.FontSize);
      _tokenResourceBinder.AddBinding(MaxWidthProperty, ToolTipResourceKey.ToolTipMaxWidth);
      _tokenResourceBinder.AddBinding(BackgroundProperty, ToolTipResourceKey.ToolTipBackground);
      _tokenResourceBinder.AddBinding(ForegroundProperty, ToolTipResourceKey.ToolTipColor);
      _tokenResourceBinder.AddBinding(CornerRadiusProperty, ToolTipResourceKey.BorderRadiusOuter);
      _tokenResourceBinder.AddBinding(MinHeightProperty, GlobalResourceKey.ControlHeight);
      _tokenResourceBinder.AddBinding(PaddingProperty, ToolTipResourceKey.ToolTipPadding);
      _tokenResourceBinder.AddBinding(ToolTipArrowSizeTokenProperty, ToolTipResourceKey.ToolTipArrowSize);
      _tokenResourceBinder.AddBinding(MarginXXSTokenProperty, GlobalResourceKey.MarginXXS);
   }

   public sealed override void Render(DrawingContext context)
   {
      var arrowRect = GetArrowRect(DesiredSize);
      if (IsShowArrow(AdornedControl!)) {
         var direction = GetDirection(GetPlacement(AdornedControl!));
         var arrowSize = _toolTipArrowSize;
         var matrix = Matrix.CreateTranslation(-arrowSize/2, -arrowSize/2);
         if (direction == Direction.Right) {
            matrix *= Matrix.CreateRotation(90);
         } else if (direction == Direction.Top) {
            matrix *= Matrix.CreateRotation(MathUtils.Deg2Rad(180));
         } else if (direction == Direction.Left) {
            matrix *= Matrix.CreateRotation(270);
         }
         matrix *= Matrix.CreateTranslation(arrowSize/2, arrowSize/2);
         matrix *= Matrix.CreateTranslation(arrowRect.X, arrowRect.Y);
         _arrowGeometry!.Transform = new MatrixTransform(matrix);
         context.DrawGeometry(Background, null, _arrowGeometry);
      }
   }
   
   protected override Size MeasureOverride(Size availableSize)
   {
      var size = base.MeasureOverride(availableSize);
      var targetWidth = size.Width;
      var targetHeight = size.Height;
      targetHeight = Math.Max(MinHeight, targetHeight);
      var adornedControl = AdornedControl!;
      if (IsShowArrow(adornedControl)) {
         var arrowSize = _arrowGeometry!.Bounds.Size;
         var direction = GetDirection(GetPlacement(adornedControl));
         if (direction == Direction.Left || direction == Direction.Right) {
            targetWidth += arrowSize.Width;
         } else {
            targetHeight += arrowSize.Height;
         }
      }

      return new Size(targetWidth, targetHeight);
   }

   protected override Size ArrangeOverride(Size finalSize)
   {
      var visualChildren = VisualChildren;
      var visualCount = visualChildren.Count;
      var contentRect = GetContentRect(finalSize);

      for (int i = 0; i < visualCount; ++i) {
         var child = visualChildren[i];
         if (child is Layoutable layoutable) {
            layoutable.Arrange(contentRect);
         }
      }
      
      return finalSize;
   }

   private Rect GetContentRect(Size finalSize)
   {
      var offsetX = 0d;
      var offsetY = 0d;
      var targetWidth = finalSize.Width;
      var targetHeight = finalSize.Height;
      var adornedControl = AdornedControl!;
      if (IsShowArrow(adornedControl)) {
         var size = _arrowGeometry!.Bounds.Size;
         var direction = GetDirection(GetPlacement(adornedControl));
         if (direction == Direction.Left || direction == Direction.Right) {
            targetWidth -= size.Width;
         } else {
            targetHeight -= size.Height;
         }

         if (direction == Direction.Right) {
            offsetX = size.Width;
         } else if (direction == Direction.Bottom) {
            offsetY = size.Height;
         }
      }

      return new Rect(offsetX, offsetY, targetWidth, targetHeight);
   }

   private Rect GetArrowRect(Size finalSize)
   {
      var offsetX = 0d;
      var offsetY = 0d;
      var adornedControl = AdornedControl!;
      var size = _arrowGeometry!.Bounds.Size;
      if (IsShowArrow(adornedControl)) {
         var direction = GetDirection(GetPlacement(adornedControl));
         if (direction == Direction.Left) {
            offsetY = (finalSize.Height - size.Height) / 2;
         } else if (direction == Direction.Top) {
            offsetX = (finalSize.Width - size.Width) / 2;
            offsetY = finalSize.Height - size.Height;
         } else if (direction == Direction.Right) {
            offsetX = finalSize.Width - size.Width;
            offsetY = (finalSize.Height - size.Height) / 2;
         } else {
            offsetX = (finalSize.Width - size.Width) / 2;
         }
      }

      return new Rect(offsetX, offsetY, size.Width, size.Height);
   }

   
   private void BuildGeometry(Direction direction)
   {
      if (_lastDirection != direction) {
         var arrowSize = _toolTipArrowSize;
         _arrowGeometry = CommonShapeBuilder.BuildArrow(arrowSize, 1.5);
         _lastDirection = direction;
      }
   }
}