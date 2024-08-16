using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.Data;

namespace AtomUI.Controls.AddOnDecoratedBox;

public enum AddOnDecoratedVariant
{
   Outline,
   Filled,
   Borderless
}

public enum AddOnDecoratedStatus
{
   Default,
   Warning,
   Error
}

[TemplatePart(AddOnDecoratedBoxTheme.LeftAddOnPart, typeof(ContentPresenter))]
[TemplatePart(AddOnDecoratedBoxTheme.RightAddOnPart, typeof(ContentPresenter))]
[TemplatePart(AddOnDecoratedBoxTheme.InnerBoxContentPart, typeof(ContentPresenter), IsRequired = true)]
public class AddOnDecoratedBox : ContentControl
{
   public const string ErrorPC = ":error";
   public const string WarningPC = ":warning";

   #region 功能属性定义

   public static readonly StyledProperty<object?> LeftAddOnProperty =
      AvaloniaProperty.Register<AddOnDecoratedBox, object?>(nameof(LeftAddOn));

   public static readonly StyledProperty<object?> RightAddOnProperty =
      AvaloniaProperty.Register<AddOnDecoratedBox, object?>(nameof(RightAddOn));

   public static readonly StyledProperty<SizeType> SizeTypeProperty =
      AvaloniaProperty.Register<AddOnDecoratedBox, SizeType>(nameof(SizeType), SizeType.Middle);

   public static readonly StyledProperty<AddOnDecoratedVariant> StyleVariantProperty =
      AvaloniaProperty.Register<AddOnDecoratedBox, AddOnDecoratedVariant>(
         nameof(StyleVariant), AddOnDecoratedVariant.Outline);

   public static readonly StyledProperty<AddOnDecoratedStatus> StatusProperty =
      AvaloniaProperty.Register<AddOnDecoratedBox, AddOnDecoratedStatus>(nameof(Status), AddOnDecoratedStatus.Default);

   public object? LeftAddOn
   {
      get => GetValue(LeftAddOnProperty);
      set => SetValue(LeftAddOnProperty, value);
   }

   public object? RightAddOn
   {
      get => GetValue(RightAddOnProperty);
      set => SetValue(RightAddOnProperty, value);
   }

   public SizeType SizeType
   {
      get => GetValue(SizeTypeProperty);
      set => SetValue(SizeTypeProperty, value);
   }

   public AddOnDecoratedVariant StyleVariant
   {
      get => GetValue(StyleVariantProperty);
      set => SetValue(StyleVariantProperty, value);
   }

   public AddOnDecoratedStatus Status
   {
      get => GetValue(StatusProperty);
      set => SetValue(StatusProperty, value);
   }

   #endregion

   #region 内部属性定义

   internal static readonly DirectProperty<AddOnDecoratedBox, CornerRadius> InnerBoxCornerRadiusProperty =
      AvaloniaProperty.RegisterDirect<AddOnDecoratedBox, CornerRadius>(nameof(InnerBoxCornerRadius),
                                                                       o => o.InnerBoxCornerRadius,
                                                                       (o, v) => o.InnerBoxCornerRadius = v);

   internal static readonly DirectProperty<AddOnDecoratedBox, CornerRadius> LeftAddOnCornerRadiusProperty =
      AvaloniaProperty.RegisterDirect<AddOnDecoratedBox, CornerRadius>(nameof(LeftAddOnCornerRadius),
                                                                       o => o.LeftAddOnCornerRadius,
                                                                       (o, v) => o.LeftAddOnCornerRadius = v);

   internal static readonly DirectProperty<AddOnDecoratedBox, CornerRadius> RightAddOnCornerRadiusProperty =
      AvaloniaProperty.RegisterDirect<AddOnDecoratedBox, CornerRadius>(nameof(RightAddOnCornerRadius),
                                                                       o => o.RightAddOnCornerRadius,
                                                                       (o, v) => o.RightAddOnCornerRadius = v);

   internal static readonly DirectProperty<AddOnDecoratedBox, Thickness> LeftAddOnBorderThicknessProperty =
      AvaloniaProperty.RegisterDirect<AddOnDecoratedBox, Thickness>(nameof(LeftAddOnBorderThickness),
                                                                    o => o.LeftAddOnBorderThickness,
                                                                    (o, v) => o.LeftAddOnBorderThickness = v);

   internal static readonly DirectProperty<AddOnDecoratedBox, Thickness> RightAddOnBorderThicknessProperty =
      AvaloniaProperty.RegisterDirect<AddOnDecoratedBox, Thickness>(nameof(RightAddOnBorderThickness),
                                                                    o => o.RightAddOnBorderThickness,
                                                                    (o, v) => o.RightAddOnBorderThickness = v);

   private CornerRadius _innerBoxCornerRadius;

   internal CornerRadius InnerBoxCornerRadius
   {
      get => _innerBoxCornerRadius;
      set => SetAndRaise(InnerBoxCornerRadiusProperty, ref _innerBoxCornerRadius, value);
   }

   private CornerRadius _leftAddOnCornerRadius;

   internal CornerRadius LeftAddOnCornerRadius
   {
      get => _leftAddOnCornerRadius;
      set => SetAndRaise(LeftAddOnCornerRadiusProperty, ref _leftAddOnCornerRadius, value);
   }

   private CornerRadius _rightAddOnCornerRadius;

   internal CornerRadius RightAddOnCornerRadius
   {
      get => _rightAddOnCornerRadius;
      set => SetAndRaise(RightAddOnCornerRadiusProperty, ref _rightAddOnCornerRadius, value);
   }

   private Thickness _leftAddOnBorderThickness;

   internal Thickness LeftAddOnBorderThickness
   {
      get => _leftAddOnBorderThickness;
      set => SetAndRaise(LeftAddOnBorderThicknessProperty, ref _leftAddOnBorderThickness, value);
   }

   private Thickness _rightAddOnBorderThickness;

   internal Thickness RightAddOnBorderThickness
   {
      get => _rightAddOnBorderThickness;
      set => SetAndRaise(RightAddOnBorderThicknessProperty, ref _rightAddOnBorderThickness, value);
   }

   #endregion

   protected Control? _leftAddOnPresenter;
   protected Control? _rightAddOnPresenter;
   protected Border? _innerBoxDecorator;

   static AddOnDecoratedBox()
   {
      AffectsRender<AddOnDecoratedBox>(BorderBrushProperty, BackgroundProperty);
      AffectsMeasure<AddOnDecoratedBox>(LeftAddOnProperty, RightAddOnProperty);
   }

   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      TokenResourceBinder.CreateGlobalResourceBinding(this, BorderThicknessProperty, GlobalResourceKey.BorderThickness,
                                                      BindingPriority.Template,
                                                      new RenderScaleAwareThicknessConfigure(this));
      _leftAddOnPresenter = e.NameScope.Find<Control>(AddOnDecoratedBoxTheme.LeftAddOnPart);
      _rightAddOnPresenter = e.NameScope.Find<Control>(AddOnDecoratedBoxTheme.RightAddOnPart);
      _innerBoxDecorator = e.NameScope.Find<Border>(AddOnDecoratedBoxTheme.InnerBoxDecoratorPart);
      SetupInnerBoxCornerRadius();
   }

   private void SetupInnerBoxCornerRadius()
   {
      var topLeftRadius = CornerRadius.TopLeft;
      var topRightRadius = CornerRadius.TopRight;
      var bottomLeftRadius = CornerRadius.BottomLeft;
      var bottomRightRadius = CornerRadius.BottomRight;

      if (_leftAddOnPresenter is not null && _leftAddOnPresenter.IsVisible) {
         topLeftRadius = 0;
         bottomLeftRadius = 0;
      }

      if (_rightAddOnPresenter is not null && _rightAddOnPresenter.IsVisible) {
         topRightRadius = 0;
         bottomRightRadius = 0;
      }

      InnerBoxCornerRadius = new CornerRadius(topLeft: topLeftRadius,
                                              topRight: topRightRadius,
                                              bottomLeft: bottomLeftRadius,
                                              bottomRight: bottomRightRadius);
   }

   protected override Size MeasureOverride(Size availableSize)
   {
      return base.MeasureOverride(new Size(availableSize.Width - BorderThickness.Left - BorderThickness.Right, availableSize.Height));
   }
   
   protected override Size ArrangeOverride(Size finalSize)
   {
      return base.ArrangeOverride(new Size(finalSize.Width - BorderThickness.Left - BorderThickness.Right, finalSize.Height));
   }
   
   protected virtual void UpdatePseudoClasses()
   {
      PseudoClasses.Set(ErrorPC, Status == AddOnDecoratedStatus.Error);
      PseudoClasses.Set(WarningPC, Status == AddOnDecoratedStatus.Warning);
   }
}