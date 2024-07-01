using AtomUI.Data;
using AtomUI.TokenSystem;
using Avalonia;
using Avalonia.Controls;
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

public partial class ArrowDecoratedBox : StyledControl, IShadowMaskInfoProvider
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
   internal (double, double) ArrowVertexPoint { get; private set; }

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
   
   static ArrowDecoratedBox()
   {
      AffectsArrange<ArrowDecoratedBox>(ArrowPositionProperty, IsShowArrowProperty);
   }
   
   public ArrowDecoratedBox()
   {
      _customStyle = this;
      _controlTokenBinder = new ControlTokenBinder(this, ArrowDecoratedBoxToken.ID);
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
      }
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);
      _customStyle.HandlePropertyChangedForStyle(e);
   }

   protected virtual void NotifyCreateUi() { }

   public CornerRadius GetMaskCornerRadius()
   {
      return CornerRadius;
   }

   public Rect GetMaskBounds()
   {
      return GetContentRect(DesiredSize);
   }
}