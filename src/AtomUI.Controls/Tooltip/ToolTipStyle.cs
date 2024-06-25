using AtomUI.Data;
using AtomUI.Media;
using AtomUI.Styling;
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
      HorizontalContentAlignment = HorizontalAlignment.Center;
      VerticalContentAlignment = VerticalAlignment.Center;
      
      if (Content is string text) {
         Content = new TextBlock
         {
            Text = text,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            TextWrapping = TextWrapping.Wrap
         };
      }

      _customStyle.ApplyFixedStyleConfig();
   }

   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      _tokenResourceBinder.AddBinding(FontSizeProperty, GlobalResourceKey.FontSize);
      _tokenResourceBinder.AddBinding(MaxWidthProperty, ToolTipResourceKey.TooltipMaxWidth);
      _tokenResourceBinder.AddBinding(DefaultBgTokenProperty, ToolTipResourceKey.TooltipBackground);
      _tokenResourceBinder.AddBinding(ForegroundProperty, ToolTipResourceKey.TooltipColor);
      _tokenResourceBinder.AddBinding(CornerRadiusProperty, ToolTipResourceKey.BorderRadiusOuter);
      _tokenResourceBinder.AddBinding(MinHeightProperty, GlobalResourceKey.ControlHeight);
      _tokenResourceBinder.AddBinding(PaddingProperty, ToolTipResourceKey.ToolTipPadding);
      _tokenResourceBinder.AddBinding(SizePopupArrowTokenProperty, GlobalResourceKey.SizePopupArrow);
   }

   void IControlCustomStyle.HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
   {
      // if (e.Property == SizePopupArrowTokenProperty || 
      //     e.Property == PlacementProperty ||
      //     e.Property == IsShowArrowProperty ||
      //     e.Property == IsPointAtCenterProperty) {
      //    if (AdornedControl is not null && IsShowArrow(AdornedControl)) {
      //       BuildGeometry(true, GetDirection(GetPlacement(AdornedControl)));
      //    }
      // }
   }

   public sealed override void Render(DrawingContext context)
   {
      var contentRect = GetContentRect(DesiredSize);
      // context.FillRectangle(new SolidColorBrush(Colors.Crimson),
      //    new Rect(new Point(0, 0), DesiredSize));
      context.FillRectangle(_defaultBackground!,
                            contentRect,
                            (float)GeometryUtils.CornerRadiusScalarValue(CornerRadius));
      var arrowRect = GetArrowRect(DesiredSize);
      if (IsShowArrow(AdornedControl!)) {
         var direction = GetDirection(GetPlacement(AdornedControl!));
         var geoRect = _arrowGeometry!.Bounds;
         var matrix = Matrix.CreateTranslation(-geoRect.X, -geoRect.Y);
         if (direction == Direction.Right) {
            matrix *= Matrix.CreateRotation(90);
         } else if (direction == Direction.Top) {
           matrix *= Matrix.CreateRotation(180 * Math.PI / 180);
         } else if (direction == Direction.Left) {
            matrix *= Matrix.CreateRotation(270);
         }
         Console.WriteLine(_arrowGeometry!.Bounds);
         matrix *= Matrix.CreateTranslation(arrowRect.X, arrowRect.Y);
         _arrowGeometry!.Transform = new MatrixTransform(matrix);
         context.DrawGeometry(new SolidColorBrush(Colors.OrangeRed), null, _arrowGeometry);
      }
   }
   
   protected override Size MeasureOverride(Size availableSize)
   {
      var size = base.MeasureOverride(availableSize);
      var targetWidth = size.Width;
      var targetHeight = size.Height;
      targetHeight = Math.Max(MinHeight, targetHeight);
      var adornedControl = AdornedControl!;
      var delta = _sizePopupArrow / 2;
      if (IsShowArrow(adornedControl)) {
         var direction = GetDirection(GetPlacement(adornedControl));
         if (direction == Direction.Left || direction == Direction.Right) {
            targetWidth += delta;
         } else {
            targetHeight += delta;
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
      var delta = _sizePopupArrow / 2;
      if (IsShowArrow(adornedControl)) {
         var direction = GetDirection(GetPlacement(adornedControl));
         if (direction == Direction.Left || direction == Direction.Right) {
            targetWidth -= delta;
         } else {
            targetHeight -= delta;
         }

         if (direction == Direction.Right) {
            offsetX = delta;
         } else if (direction == Direction.Bottom) {
            offsetY = delta;
         }
      }

      return new Rect(offsetX, offsetY, targetWidth, targetHeight);
   }

   private Rect GetArrowRect(Size finalSize)
   {
      var offsetX = 0d;
      var offsetY = 0d;
      var size = _sizePopupArrow / 2;
      var adornedControl = AdornedControl!;
      if (IsShowArrow(adornedControl)) {
         var direction = GetDirection(GetPlacement(adornedControl));
         if (direction == Direction.Left) {
            offsetY = (finalSize.Height - size) / 2;
         } else if (direction == Direction.Top) {
            offsetX = (finalSize.Width - size) / 2;
            offsetY = finalSize.Height - size;
         } else if (direction == Direction.Right) {
            offsetX = finalSize.Width - size;
            offsetY = (finalSize.Height - size) / 2;
         } else {
            offsetX = (finalSize.Width - size) / 2;
         }
      }

      return new Rect(offsetX, offsetY, size, size);
   }

   
   private void BuildGeometry(Direction direction)
   {
      if (_lastDirection != direction) {
         var arrowSize = _sizePopupArrow / 1.3;
         _arrowGeometry = CommonShapeBuilder.BuildArrow(arrowSize, 1.5);
         _lastDirection = direction;
      }
   }
}