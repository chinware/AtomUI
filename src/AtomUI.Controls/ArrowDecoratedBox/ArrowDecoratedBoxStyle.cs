using AtomUI.Data;
using AtomUI.Media;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Controls;

public partial class ArrowDecoratedBox : IControlCustomStyle
{
   private bool _initialized = false;
   private IControlCustomStyle _customStyle;
   private ControlTokenBinder<ArrowDecoratedBox> _controlTokenBinder;
   private Geometry? _arrowGeometry;
   private Direction? _lastDirection;
   private Rect _contentRect;
   private Rect _arrowRect;
   private Border? _container;

   // 组件的 Token 绑定属性
   private double _arrowSize;

   private static readonly DirectProperty<ArrowDecoratedBox, double> ArrowSizeTokenProperty
      = AvaloniaProperty.RegisterDirect<ArrowDecoratedBox, double>(nameof(_arrowSize),
                                                                   (o) => o._arrowSize,
                                                                   (o, v) => o._arrowSize = v);
   // 组件的 Token 绑定属性

   void IControlCustomStyle.SetupUi()
   {
      _container = new Border();
      NotifyCreateUi();
      _customStyle.ApplyFixedStyleConfig();
      if (IsShowArrow) {
         BuildGeometry(GetDirection(ArrowPosition));
      }
      LogicalChildren.Add(_container);
      VisualChildren.Add(_container);
      // 生命周期一样，可以不用管理
      BindUtils.RelayBind(this, BackgroundSizingProperty, _container);
      BindUtils.RelayBind(this, CornerRadiusProperty, _container);
      BindUtils.RelayBind(this, ChildProperty, _container);
      BindUtils.RelayBind(this, PaddingProperty, _container);
      BindUtils.RelayBind(this, ChildProperty, _container);
      _controlTokenBinder.AddControlBinding(_container, BackgroundProperty, GlobalResourceKey.ColorBgContainer);
      
      _initialized = true;
   }

   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      NotifyApplyFixedStyleConfig();
   }

   void IControlCustomStyle.HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
   {
      if (e.Property == IsShowArrowProperty ||
          e.Property == ArrowPositionProperty) {
         BuildGeometry(GetDirection(ArrowPosition));
      } 
   }

   private void BuildGeometry(Direction direction)
   {
      if (_lastDirection != direction) {
         var arrowSize = _arrowSize;
         _arrowGeometry = CommonShapeBuilder.BuildArrow(arrowSize, 1.5);
         _lastDirection = direction;
      }
   }

   protected virtual void NotifyApplyFixedStyleConfig()
   {
      _controlTokenBinder.AddControlBinding(MinHeightProperty, GlobalResourceKey.ControlHeight);
      _controlTokenBinder.AddControlBinding(PaddingProperty, GlobalResourceKey.PaddingXS);
      _controlTokenBinder.AddControlBinding(ArrowSizeTokenProperty, ArrowDecoratedBoxResourceKey.ArrowSize);
      _controlTokenBinder.AddControlBinding(CornerRadiusProperty, GlobalResourceKey.BorderRadius);
   }

   public sealed override void Render(DrawingContext context)
   {
      if (IsShowArrow) {
         var direction = GetDirection(ArrowPosition);
         var matrix = Matrix.CreateTranslation(-_arrowSize / 2, -_arrowSize / 2);
         if (direction == Direction.Right) {
            matrix *= Matrix.CreateRotation(MathUtils.Deg2Rad(-90));
            matrix *= Matrix.CreateTranslation(0, _arrowSize / 2);
         } else if (direction == Direction.Top) {
            matrix *= Matrix.CreateRotation(MathUtils.Deg2Rad(180));
            matrix *= Matrix.CreateTranslation(_arrowSize / 2, _arrowSize / 2);
         } else if (direction == Direction.Left) {
            matrix *= Matrix.CreateRotation(MathUtils.Deg2Rad(90));
            matrix *= Matrix.CreateTranslation(_arrowSize / 2, _arrowSize / 2);
         } else {
            matrix *= Matrix.CreateTranslation(_arrowSize / 2, 0);
         }

         matrix *= Matrix.CreateTranslation(_arrowRect.X, _arrowRect.Y);
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
      if (IsShowArrow) {
         var realArrowSize = Math.Min(_arrowGeometry!.Bounds.Size.Height, _arrowGeometry!.Bounds.Size.Width);
         var direction = GetDirection(ArrowPosition);
         if (direction == Direction.Left || direction == Direction.Right) {
            targetWidth += realArrowSize;
         } else {
            targetHeight += realArrowSize;
         }
      }

      return new Size(targetWidth, targetHeight);
   }

   protected override Size ArrangeOverride(Size finalSize)
   {
      var visualChildren = VisualChildren;
      var visualCount = visualChildren.Count;
      _contentRect = GetContentRect(finalSize);
      _arrowRect = GetArrowRect(finalSize);
      for (int i = 0; i < visualCount; ++i) {
         var child = visualChildren[i];
         if (child is Layoutable layoutable) {
            layoutable.Arrange(_contentRect);
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
      if (IsShowArrow) {
         var arrowSize = Math.Min(_arrowGeometry!.Bounds.Size.Height, _arrowGeometry!.Bounds.Size.Width) + 0.5;
         var direction = GetDirection(ArrowPosition);
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
      var size = _arrowGeometry!.Bounds.Size;
      var targetWidth = 0d;
      var targetHeight = 0d;
      var position = ArrowPosition;
      if (IsShowArrow) {
         var minValue = Math.Min(size.Width, size.Height);
         var maxValue = Math.Max(size.Width, size.Height);
         if (position == ArrowPosition.Left ||
             position == ArrowPosition.LeftEdgeAlignedTop ||
             position == ArrowPosition.LeftEdgeAlignedBottom) {
            offsetX = finalSize.Width - minValue;
            if (position == ArrowPosition.Left) {
               offsetY = (finalSize.Height - maxValue) / 2;
            } else if (position == ArrowPosition.LeftEdgeAlignedTop) {
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
         } else if (position == ArrowPosition.Top ||
                    position == ArrowPosition.TopEdgeAlignedLeft ||
                    position == ArrowPosition.TopEdgeAlignedRight) {
            offsetY = finalSize.Height - minValue;
            targetWidth = maxValue;
            targetHeight = minValue;
            if (position == ArrowPosition.TopEdgeAlignedLeft) {
               offsetX = maxValue;
            } else if (position == ArrowPosition.Top) {
               offsetX = (finalSize.Width - maxValue) / 2;
            } else {
               offsetX = finalSize.Width - maxValue * 2;
            }
         } else if (position == ArrowPosition.Right ||
                    position == ArrowPosition.RightEdgeAlignedTop ||
                    position == ArrowPosition.RightEdgeAlignedBottom) {
            targetWidth = minValue;
            targetHeight = maxValue;
            if (position == ArrowPosition.Right) {
               offsetY = (finalSize.Height - maxValue) / 2;
            } else if (position == ArrowPosition.RightEdgeAlignedTop) {
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
            if (position == ArrowPosition.BottomEdgeAlignedLeft) {
               offsetX = maxValue;
            } else if (position == ArrowPosition.Bottom) {
               offsetX = (finalSize.Width - maxValue) / 2;
            } else {
               offsetX = finalSize.Width - maxValue * 2;
            }

            targetWidth = maxValue;
            targetHeight = minValue;
         }
      }

      var targetRect = new Rect(offsetX, offsetY, targetWidth, targetHeight);
      var center = targetRect.Center;
      // 计算中点
      var direction = GetDirection(position);
      if (direction == Direction.Left || direction == Direction.Right) {
         ArrowVertexPoint = (center.Y, finalSize.Height - center.Y);
      } else if (direction == Direction.Top || direction == Direction.Bottom) {
         ArrowVertexPoint = (center.X, finalSize.Width - center.X);
      }

      return targetRect;
   }
}