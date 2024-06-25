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
      // context.FillRectangle(new SolidColorBrush(Colors.Aqua), new Rect(default, DesiredSize));
      var arrowRect = GetArrowRect(DesiredSize);
      if (GetIsShowArrow(AdornedControl!)) {
         var direction = GetDirection(GetPlacement(AdornedControl!));
         var arrowSize = _toolTipArrowSize;
         var matrix = Matrix.CreateTranslation(-arrowSize / 2, -arrowSize / 2);
         if (direction == Direction.Right) {
            matrix *= Matrix.CreateRotation(MathUtils.Deg2Rad(-90));
            matrix *= Matrix.CreateTranslation(0, arrowSize / 2);
         } else if (direction == Direction.Top) {
            matrix *= Matrix.CreateRotation(MathUtils.Deg2Rad(180));
            matrix *= Matrix.CreateTranslation(arrowSize / 2, arrowSize / 2);
         } else if (direction == Direction.Left) {
            matrix *= Matrix.CreateRotation(MathUtils.Deg2Rad(90));
            matrix *= Matrix.CreateTranslation(arrowSize / 2, arrowSize / 2);
         } else {
            matrix *= Matrix.CreateTranslation(arrowSize / 2, 0);
         }
  
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
      if (GetIsShowArrow(adornedControl)) {
         var arrowSize = Math.Min(_arrowGeometry!.Bounds.Size.Height, _arrowGeometry!.Bounds.Size.Width);
         var direction = GetDirection(GetPlacement(adornedControl));
         if (direction == Direction.Left || direction == Direction.Right) {
            targetWidth += arrowSize;
         } else {
            targetHeight += arrowSize;
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
      if (GetIsShowArrow(adornedControl)) {
         var arrowSize = Math.Min(_arrowGeometry!.Bounds.Size.Height, _arrowGeometry!.Bounds.Size.Width) + 0.5;
         var direction = GetDirection(GetPlacement(adornedControl));
         if (direction == Direction.Left || direction == Direction.Right) {
            targetWidth -= arrowSize;
         } else {
            targetHeight -= arrowSize;
         }

         if (direction == Direction.Right) {
            offsetX = arrowSize - 0.5;
         } else if (direction == Direction.Bottom) {
            offsetY = arrowSize - 0.5;
         } else if (direction == Direction.Top) {
            offsetY = 0.5;
         } else {
            offsetX = 0.5;
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
      var targetWidth = 0d;
      var targetHeight = 0d;
      if (GetIsShowArrow(adornedControl)) {
         var minValue = Math.Min(size.Width, size.Height);
         var maxValue = Math.Max(size.Width, size.Height);
         var placement = GetPlacement(adornedControl);
         if (placement == PlacementType.Left ||
             placement == PlacementType.LeftEdgeAlignedTop ||
             placement == PlacementType.LeftEdgeAlignedBottom) {
            offsetX = finalSize.Width - minValue;
            if (placement == PlacementType.Left) {
               offsetY = (finalSize.Height - maxValue) / 2;
            } else if (placement == PlacementType.LeftEdgeAlignedTop) {
               if (maxValue * 2 > finalSize.Height / 2) {
                  offsetY = minValue;
               } else {
                  offsetY = maxValue;
               }
            } else {
               if (maxValue * 2 > finalSize.Height / 2) {
                  offsetY = finalSize.Height - minValue - maxValue;
               } else {
                  offsetY = finalSize.Height - maxValue * 2;
               }
            }
          
            targetWidth = minValue;
            targetHeight = maxValue;
         } else if (placement == PlacementType.Top ||
                    placement == PlacementType.TopEdgeAlignedLeft ||
                    placement == PlacementType.TopEdgeAlignedRight) {
            offsetY = finalSize.Height - minValue;
            targetWidth = maxValue;
            targetHeight = minValue;
            if (placement == PlacementType.TopEdgeAlignedLeft) {
               offsetX = maxValue;
            } else if (placement == PlacementType.Top) {
               offsetX = (finalSize.Width - maxValue) / 2;
            } else {
               offsetX = finalSize.Width - maxValue * 2;
            }
         } else if (placement == PlacementType.Right ||
                    placement == PlacementType.RightEdgeAlignedTop ||
                    placement == PlacementType.RightEdgeAlignedBottom) {
            targetWidth = minValue;
            targetHeight = maxValue;
            if (placement == PlacementType.Right) {
               offsetY = (finalSize.Height - maxValue) / 2;
            } else if (placement == PlacementType.RightEdgeAlignedTop) {
               if (maxValue * 2 > finalSize.Height / 2) {
                  offsetY = minValue;
               } else {
                  offsetY = maxValue;
               }
            } else {
               if (maxValue * 2 > finalSize.Height / 2) {
                  offsetY = finalSize.Height - minValue - maxValue;
               } else {
                  offsetY = finalSize.Height - maxValue * 2;
               }
           
            }
         } else {
            if (placement == PlacementType.BottomEdgeAlignedLeft) {
               offsetX = maxValue;
            } else if (placement == PlacementType.Bottom) {
               offsetX = (finalSize.Width - maxValue) / 2;
            } else {
               offsetX = finalSize.Width - maxValue * 2;
            }
            targetWidth = maxValue;
            targetHeight = minValue;
         }
      }

      return new Rect(offsetX, offsetY, targetWidth, targetHeight);
   }
   
   private void BuildGeometry(Direction direction)
   {
      if (_lastDirection != direction) {
         var arrowSize = _toolTipArrowSize;
         _arrowGeometry = CommonShapeBuilder.BuildArrow(arrowSize, 1.5);
         _lastDirection = direction;
      }
   }
   
   void IControlCustomStyle.HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
   {
      if (e.Property == BackgroundProperty) {
         //Console.WriteLine(e.GetNewValue<IBrush>());
      }
   }
}