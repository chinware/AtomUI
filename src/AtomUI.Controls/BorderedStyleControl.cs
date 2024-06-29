using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Metadata;

namespace AtomUI.Controls;

public class BorderedStyleControl : StyledControl
{
   /// <summary>
   /// Defines the <see cref="Child"/> property.
   /// </summary>
   public static readonly StyledProperty<Control?> ChildProperty =
      Border.ChildProperty.AddOwner<BorderedStyleControl>();

   /// <summary>
   /// Defines the <see cref="BorderBrush"/> property.
   /// </summary>
   public static readonly StyledProperty<IBrush?> BorderBrushProperty =
      Border.BorderBrushProperty.AddOwner<BorderedStyleControl>();

   /// <summary>
   /// Defines the <see cref="BorderThickness"/> property.
   /// </summary>
   public static readonly StyledProperty<Thickness> BorderThicknessProperty =
      Border.BorderThicknessProperty.AddOwner<BorderedStyleControl>();

   /// <summary>
   /// Defines the <see cref="CornerRadius"/> property.
   /// </summary>
   public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
      Border.CornerRadiusProperty.AddOwner<BorderedStyleControl>();

   /// <summary>
   /// Defines the <see cref="BoxShadow"/> property.
   /// </summary>
   public static readonly StyledProperty<BoxShadows> BoxShadowProperty =
      Border.BoxShadowProperty.AddOwner<BorderedStyleControl>();

   /// <summary>
   /// Gets or sets a brush with which to paint the border.
   /// </summary>
   public IBrush? BorderBrush
   {
      get => GetValue(BorderBrushProperty);
      set => SetValue(BorderBrushProperty, value);
   }

   /// <summary>
   /// Gets or sets the thickness of the border.
   /// </summary>
   public Thickness BorderThickness
   {
      get => GetValue(BorderThicknessProperty);
      set => SetValue(BorderThicknessProperty, value);
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
   /// Gets or sets the box shadow effect parameters
   /// </summary>
   public BoxShadows BoxShadow
   {
      get => GetValue(BoxShadowProperty);
      set => SetValue(BoxShadowProperty, value);
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

   private Border? _border;

   static BorderedStyleControl()
   {
      AffectsMeasure<BorderedStyleControl>(ChildProperty, PaddingProperty, BorderThicknessProperty);
      AffectsRender<BorderedStyleControl>(BorderBrushProperty);
   }

   public BorderedStyleControl()
   {
      _border = new Border();
      LogicalChildren.Add(_border);
      VisualChildren.Add(_border);
      // TODO 这些资源要管理起来
      BindUtils.RelayBind(this, BackgroundProperty, _border);
      BindUtils.RelayBind(this, BackgroundSizingProperty, _border);
      BindUtils.RelayBind(this, BorderBrushProperty, _border);
      BindUtils.RelayBind(this, BorderThicknessProperty, _border);
      BindUtils.RelayBind(this, CornerRadiusProperty, _border);
      BindUtils.RelayBind(this, ChildProperty, _border);
      BindUtils.RelayBind(this, PaddingProperty, _border);
   }

   protected override Size ArrangeOverride(Size finalSize)
   {
      var targetRect = new Rect(new Point(BorderThickness.Left, BorderThickness.Top), 
                                finalSize.Deflate(BorderThickness));
      _border!.Arrange(targetRect);
      return finalSize;
   }
}