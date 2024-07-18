using System.Reactive.Disposables;
using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Media;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Metadata;

namespace AtomUI.Controls;

public enum ArrowPosition
{
   /// <summary>
   /// Preferred location is below the target element.
   /// </summary>
   Bottom,

   /// <summary>
   /// Preferred location is to the right of the target element.
   /// </summary>
   Right,

   /// <summary>
   /// Preferred location is to the left of the target element.
   /// </summary>
   Left,

   /// <summary>
   /// Preferred location is above the target element.
   /// </summary>
   Top,

   /// <summary>
   /// Preferred location is above the target element, with the left edge of the popup
   /// aligned with the left edge of the target element.
   /// </summary>
   TopEdgeAlignedLeft,

   /// <summary>
   /// Preferred location is above the target element, with the right edge of popup aligned with right edge of the target element.
   /// </summary>
   TopEdgeAlignedRight,

   /// <summary>
   /// Preferred location is below the target element, with the left edge of popup aligned with left edge of the target element.
   /// </summary>
   BottomEdgeAlignedLeft,

   /// <summary>
   /// Preferred location is below the target element, with the right edge of popup aligned with right edge of the target element.
   /// </summary>
   BottomEdgeAlignedRight,

   /// <summary>
   /// Preferred location is to the left of the target element, with the top edge of popup aligned with top edge of the target element.
   /// </summary>
   LeftEdgeAlignedTop,

   /// <summary>
   /// Preferred location is to the left of the target element, with the bottom edge of popup aligned with bottom edge of the target element.
   /// </summary>
   LeftEdgeAlignedBottom,

   /// <summary>
   /// Preferred location is to the right of the target element, with the top edge of popup aligned with top edge of the target element.
   /// </summary>
   RightEdgeAlignedTop,

   /// <summary>
   /// Preferred location is to the right of the target element, with the bottom edge of popup aligned with bottom edge of the target element.
   /// </summary>
   RightEdgeAlignedBottom
}

public class ArrowDecoratedBox : StyledControl,
                                 IShadowMaskInfoProvider,
                                 IControlCustomStyle
{
   public static readonly StyledProperty<bool> IsShowArrowProperty =
      AvaloniaProperty.Register<ArrowDecoratedBox, bool>(nameof(IsShowArrow), true);

   public static readonly StyledProperty<ArrowPosition> ArrowPositionProperty =
      AvaloniaProperty.Register<ArrowDecoratedBox, ArrowPosition>(
         nameof(ArrowPosition), defaultValue: ArrowPosition.Bottom);

   /// <summary>
   /// Defines the <see cref="Child"/> property.
   /// </summary>
   public static readonly StyledProperty<Control?> ChildProperty =
      Border.ChildProperty.AddOwner<ArrowDecoratedBox>();

   /// <summary>
   /// Defines the <see cref="CornerRadius"/> property.
   /// </summary>
   public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
      Border.CornerRadiusProperty.AddOwner<ArrowDecoratedBox>();

   // 指针最顶点位置
   // 相对坐标
   private (double, double) _arrowVertexPoint;
   internal (double, double) ArrowVertexPoint => GetArrowVertexPoint();

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

   /// <summary>
   /// Gets or sets the radius of the border rounded corners.
   /// </summary>
   public CornerRadius CornerRadius
   {
      get => GetValue(CornerRadiusProperty);
      set => SetValue(CornerRadiusProperty, value);
   }

   /// <summary>
   /// Gets or sets the decorated control.
   /// </summary>
   [Content]
   public Control? Child
   {
      get => GetValue(ChildProperty);
      set => SetValue(ChildProperty, value);
   }

   private bool _initialized = false;
   private IControlCustomStyle _customStyle;
   private Geometry? _arrowGeometry;
   private Rect _contentRect;
   private Rect _arrowRect;
   private Border? _container;
   private CompositeDisposable? _compositeDisposable;
   private bool _needGenerateArrowVertexPoint = true;

   static ArrowDecoratedBox()
   {
      AffectsMeasure<ArrowDecoratedBox>(ArrowPositionProperty, IsShowArrowProperty);
   }

   public ArrowDecoratedBox()
   {
      _customStyle = this;
   }

   public static Direction GetDirection(ArrowPosition arrowPosition)
   {
      return arrowPosition switch
      {
         ArrowPosition.Left => Direction.Left,
         ArrowPosition.LeftEdgeAlignedBottom => Direction.Left,
         ArrowPosition.LeftEdgeAlignedTop => Direction.Left,

         ArrowPosition.Top => Direction.Top,
         ArrowPosition.TopEdgeAlignedLeft => Direction.Top,
         ArrowPosition.TopEdgeAlignedRight => Direction.Top,

         ArrowPosition.Right => Direction.Right,
         ArrowPosition.RightEdgeAlignedBottom => Direction.Right,
         ArrowPosition.RightEdgeAlignedTop => Direction.Right,

         ArrowPosition.Bottom => Direction.Bottom,
         ArrowPosition.BottomEdgeAlignedLeft => Direction.Bottom,
         ArrowPosition.BottomEdgeAlignedRight => Direction.Bottom,
         _ => throw new ArgumentOutOfRangeException(nameof(arrowPosition), arrowPosition,
                                                    "Invalid value for ArrowPosition")
      };
   }

   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (!_initialized) {
         _customStyle.SetupUi();
         SetupRelayProperties();
      }
   }

   protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnAttachedToVisualTree(e);
      SetupRelayProperties();
   }

   protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnDetachedFromVisualTree(e);
      _compositeDisposable?.Dispose();
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);
      _customStyle.HandlePropertyChangedForStyle(e);
   }

   public CornerRadius GetMaskCornerRadius()
   {
      return CornerRadius;
   }

   public Rect GetMaskBounds()
   {
      return GetContentRect(DesiredSize).Deflate(0.5);
   }

   #region IControlCustomStyle 实现

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
      _customStyle.ApplyFixedStyleConfig();
      if (IsShowArrow) {
         BuildGeometry(true);
      }

      LogicalChildren.Add(_container);
      VisualChildren.Add(_container);

      BindUtils.CreateTokenBinding(this, BackgroundProperty, GlobalResourceKey.ColorBgContainer);
      _initialized = true;
   }

   private void SetupRelayProperties()
   {
      _compositeDisposable = new CompositeDisposable();
      // 生命周期一样，可以不用管理
      if (_container is not null) {
         if (Child?.Parent is not null) {
            UIStructureUtils.ClearLogicalParentRecursive(Child, null);
            UIStructureUtils.ClearVisualParentRecursive(Child, null);
         }

         _compositeDisposable.Add(BindUtils.RelayBind(this, BackgroundSizingProperty, _container));
         _compositeDisposable.Add(BindUtils.RelayBind(this, BackgroundProperty, _container));
         _compositeDisposable.Add(BindUtils.RelayBind(this, CornerRadiusProperty, _container));
         _compositeDisposable.Add(BindUtils.RelayBind(this, ChildProperty, _container));
         _compositeDisposable.Add(BindUtils.RelayBind(this, PaddingProperty, _container));
      }
   }

   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      NotifyApplyFixedStyleConfig();
   }

   private (double, double) GetArrowVertexPoint()
   {
      if (_needGenerateArrowVertexPoint) {
         BuildGeometry(true);
         _arrowRect = GetArrowRect(DesiredSize);
         _needGenerateArrowVertexPoint = false;
      }

      return _arrowVertexPoint;
   }

   void IControlCustomStyle.HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
   {
      if (e.Property == IsShowArrowProperty ||
          e.Property == ArrowPositionProperty ||
          e.Property == ArrowSizeTokenProperty ||
          e.Property == VisualParentProperty) {
         if (e.Property == IsShowArrowProperty && VisualRoot is null) {
            // 当开启的时候，但是还没有加入的渲染树，这个时候我们取不到 Token 需要在取值的时候重新生成一下
            _needGenerateArrowVertexPoint = true;
         }

         if (_initialized && VisualRoot is not null) {
            BuildGeometry(true);
            _arrowRect = GetArrowRect(DesiredSize);
         }
      }
   }

   private void BuildGeometry(bool force = false)
   {
      if (_arrowGeometry is null || force) {
         _arrowGeometry = CommonShapeBuilder.BuildArrow(_arrowSize, 1.5);
      }
   }

   protected virtual void NotifyApplyFixedStyleConfig()
   {
      BindUtils.CreateTokenBinding(this, MinHeightProperty, GlobalResourceKey.ControlHeight);
      BindUtils.CreateTokenBinding(this, PaddingProperty, GlobalResourceKey.PaddingXS);
      BindUtils.CreateTokenBinding(this, ArrowSizeTokenProperty, ArrowDecoratedBoxResourceKey.ArrowSize);
      BindUtils.CreateTokenBinding(this, CornerRadiusProperty, GlobalResourceKey.BorderRadius);
   }

   public sealed override void Render(DrawingContext context)
   {
      if (IsShowArrow) {
         var direction = GetDirection(ArrowPosition);
         var matrix = Matrix.CreateTranslation(-_arrowSize / 2, -_arrowSize / 2);

         if (direction == Direction.Right) {
            matrix *= Matrix.CreateRotation(MathUtils.Deg2Rad(90));
            matrix *= Matrix.CreateTranslation(_arrowSize / 2, _arrowSize / 2);
         } else if (direction == Direction.Top) {
            matrix *= Matrix.CreateTranslation(_arrowSize / 2, 0);
         } else if (direction == Direction.Left) {
            matrix *= Matrix.CreateRotation(MathUtils.Deg2Rad(-90));
            matrix *= Matrix.CreateTranslation(0, _arrowSize / 2);
         } else {
            matrix *= Matrix.CreateRotation(MathUtils.Deg2Rad(180));
            matrix *= Matrix.CreateTranslation(_arrowSize / 2, _arrowSize / 2);
         }

         matrix *= Matrix.CreateTranslation(_arrowRect.X, _arrowRect.Y);
         _arrowGeometry!.Transform = new MatrixTransform(matrix);
         context.DrawGeometry(_container?.Background, null, _arrowGeometry);
      }
   }

   protected override Size MeasureOverride(Size availableSize)
   {
      var size = base.MeasureOverride(availableSize);
      var targetWidth = size.Width;
      var targetHeight = size.Height;
      targetHeight = Math.Max(MinHeight, targetHeight);

      if (IsShowArrow) {
         BuildGeometry();
         var realArrowSize = Math.Min(_arrowGeometry!.Bounds.Size.Height, _arrowGeometry!.Bounds.Size.Width);
         var direction = GetDirection(ArrowPosition);
         if (direction == Direction.Left || direction == Direction.Right) {
            targetWidth += realArrowSize;
         } else {
            targetHeight += realArrowSize;
         }
      }

      var targetSize = new Size(targetWidth, targetHeight);
      _arrowRect = GetArrowRect(targetSize);
      return targetSize;
   }

   protected override Size ArrangeOverride(Size finalSize)
   {
      var visualChildren = VisualChildren;
      var visualCount = visualChildren.Count;
      _contentRect = GetContentRect(finalSize);
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
            offsetX = 0.5;
         } else if (direction == Direction.Bottom) {
            offsetY = 0.5;
         } else if (direction == Direction.Top) {
            offsetY = arrowSize - 0.5;
         } else {
            offsetX = arrowSize - 0.5;
         }
      }

      return new Rect(offsetX, offsetY, targetWidth, targetHeight);
   }

   private Rect GetArrowRect(Size finalSize)
   {
      var offsetX = 0d;
      var offsetY = 0d;
      var targetWidth = 0d;
      var targetHeight = 0d;
      var position = ArrowPosition;
      if (IsShowArrow) {
         var size = _arrowGeometry!.Bounds.Size;

         var minValue = Math.Min(size.Width, size.Height);
         var maxValue = Math.Max(size.Width, size.Height);
         if (position == ArrowPosition.Left ||
             position == ArrowPosition.LeftEdgeAlignedTop ||
             position == ArrowPosition.LeftEdgeAlignedBottom) {
            targetWidth = minValue;
            targetHeight = maxValue;
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
         } else if (position == ArrowPosition.Top ||
                    position == ArrowPosition.TopEdgeAlignedLeft ||
                    position == ArrowPosition.TopEdgeAlignedRight) {
            if (position == ArrowPosition.TopEdgeAlignedLeft) {
               offsetX = maxValue;
            } else if (position == ArrowPosition.Top) {
               offsetX = (finalSize.Width - maxValue) / 2;
            } else {
               offsetX = finalSize.Width - maxValue * 2;
            }

            targetWidth = maxValue;
            targetHeight = minValue;
         } else if (position == ArrowPosition.Right ||
                    position == ArrowPosition.RightEdgeAlignedTop ||
                    position == ArrowPosition.RightEdgeAlignedBottom) {
            offsetX = finalSize.Width - minValue;
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

            targetWidth = minValue;
            targetHeight = maxValue;
         } else {
            offsetY = finalSize.Height - minValue;
            targetWidth = maxValue;
            targetHeight = minValue;
            if (position == ArrowPosition.BottomEdgeAlignedLeft) {
               offsetX = maxValue;
            } else if (position == ArrowPosition.Bottom) {
               offsetX = (finalSize.Width - maxValue) / 2;
            } else {
               offsetX = finalSize.Width - maxValue * 2;
            }
         }
      }

      var targetRect = new Rect(offsetX, offsetY, targetWidth, targetHeight);
      var center = targetRect.Center;

      // 计算中点
      var direction = GetDirection(position);
      if (direction == Direction.Left || direction == Direction.Right) {
         _arrowVertexPoint = (center.Y, finalSize.Height - center.Y);
      } else if (direction == Direction.Top || direction == Direction.Bottom) {
         _arrowVertexPoint = (center.X, finalSize.Width - center.X);
      }

      return targetRect;
   }

   #endregion
}