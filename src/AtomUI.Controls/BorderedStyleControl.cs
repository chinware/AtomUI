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
      AvaloniaProperty.Register<BorderedStyleControl, Control?>(nameof(Child));

   /// <summary>
   /// Defines the <see cref="BorderBrush"/> property.
   /// </summary>
   public static readonly StyledProperty<IBrush?> BorderBrushProperty =
      AvaloniaProperty.Register<BorderedStyleControl, IBrush?>(nameof(BorderBrush));

   /// <summary>
   /// Defines the <see cref="BorderThickness"/> property.
   /// </summary>
   public static readonly StyledProperty<Thickness> BorderThicknessProperty =
      AvaloniaProperty.Register<BorderedStyleControl, Thickness>(nameof(BorderThickness));

   /// <summary>
   /// Defines the <see cref="CornerRadius"/> property.
   /// </summary>
   public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
      AvaloniaProperty.Register<BorderedStyleControl, CornerRadius>(nameof(CornerRadius));

   /// <summary>
   /// Defines the <see cref="BoxShadow"/> property.
   /// </summary>
   public static readonly StyledProperty<BoxShadows> BoxShadowProperty =
      AvaloniaProperty.Register<BorderedStyleControl, BoxShadows>(nameof(BoxShadow));

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
   protected Control? Child
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

      BindUtils.RelayBind(this, "Background", _border);
      BindUtils.RelayBind(this, "BackgroundSizing", _border);
      BindUtils.RelayBind(this, "BorderBrush", _border);
      BindUtils.RelayBind(this, "BorderThickness", _border);
      BindUtils.RelayBind(this, "CornerRadius", _border);
      BindUtils.RelayBind(this, "Child", _border);
      BindUtils.RelayBind(this, "Padding", _border);
   }

   protected override Size ArrangeOverride(Size finalSize)
   {
      var targetRect = new Rect(new Point(BorderThickness.Left, BorderThickness.Top), 
                                finalSize.Deflate(BorderThickness));
      _border!.Arrange(targetRect);
      return finalSize;
   }
}