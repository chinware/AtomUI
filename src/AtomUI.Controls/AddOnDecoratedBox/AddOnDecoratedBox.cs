using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Metadata;

namespace AtomUI.Controls;

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

   #region 公共属性定义

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
   //
   // internal static readonly StyledProperty<Thickness> InnerBoxPaddingProperty =
   //    AvaloniaProperty.Register<AddOnDecoratedBox, Thickness>(nameof(InnerBoxPadding));
   //
   // internal static readonly DirectProperty<AddOnDecoratedBox, Thickness> EffectiveInnerBoxPaddingProperty =
   //    AvaloniaProperty.RegisterDirect<AddOnDecoratedBox, Thickness>(nameof(EffectiveInnerBoxPadding),
   //                                                                  o => o.EffectiveInnerBoxPadding,
   //                                                                  (o, v) => o.EffectiveInnerBoxPadding = v);

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
   //
   // public Thickness InnerBoxPadding
   // {
   //    get => GetValue(InnerBoxPaddingProperty);
   //    set => SetValue(InnerBoxPaddingProperty, value);
   // }
   
   // private Thickness _effectiveInnerBoxPadding;
   //
   // internal Thickness EffectiveInnerBoxPadding
   // {
   //    get => _effectiveInnerBoxPadding;
   //    set => SetAndRaise(EffectiveInnerBoxPaddingProperty, ref _effectiveInnerBoxPadding, value);
   // }
   
   #endregion

   protected Control? _leftAddOnPresenter;
   protected Control? _rightAddOnPresenter;

   static AddOnDecoratedBox()
   {
      AffectsRender<AddOnDecoratedBox>(BorderBrushProperty, BackgroundProperty);
      AffectsMeasure<AddOnDecoratedBox>(LeftAddOnProperty, RightAddOnProperty);
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
   {
      base.OnPropertyChanged(change);

      if (VisualRoot is not null) {
         if (change.Property == LeftAddOnProperty || change.Property == RightAddOnProperty) {
            SetupInnerBoxCornerRadius();
         }
      }

      if (change.Property == CornerRadiusProperty || change.Property == BorderThicknessProperty) {
         SetupAddOnBorderInfo();
      }
      
      if (change.Property == StatusProperty) {
         UpdatePseudoClasses();
      }

      if (change.Property == LeftAddOnProperty || change.Property == RightAddOnProperty) {
         if (change.NewValue is PathIcon icon) {
            SetupIconTypeAddOnSize(icon);
         }
      } else if (change.Property == ContentProperty) {
         if (Content is AddOnDecoratedInnerBox innerBox) {
            BindUtils.RelayBind(this, InnerBoxCornerRadiusProperty, innerBox, AddOnDecoratedInnerBox.CornerRadiusProperty);
            BindUtils.RelayBind(this, BorderThicknessProperty, innerBox, AddOnDecoratedInnerBox.BorderThicknessProperty);
         }
      }

      if (change.Property == SizeTypeProperty) {
         if (LeftAddOn is PathIcon leftIconAddOn) {
            SetupIconTypeAddOnSize(leftIconAddOn);
         }

         if (RightAddOn is PathIcon rightIconAddOn) {
            SetupIconTypeAddOnSize(rightIconAddOn);
         }
      }
   }

   private void SetupIconTypeAddOnSize(PathIcon icon)
   {
      if (SizeType == SizeType.Large) {
         TokenResourceBinder.CreateGlobalTokenBinding(icon, PathIcon.WidthProperty, GlobalResourceKey.IconSizeLG);
         TokenResourceBinder.CreateGlobalTokenBinding(icon, PathIcon.HeightProperty, GlobalResourceKey.IconSizeLG);
      } else if (SizeType == SizeType.Middle) {
         TokenResourceBinder.CreateGlobalTokenBinding(icon, PathIcon.WidthProperty, GlobalResourceKey.IconSize);
         TokenResourceBinder.CreateGlobalTokenBinding(icon, PathIcon.HeightProperty, GlobalResourceKey.IconSize);
      } else {
         TokenResourceBinder.CreateGlobalTokenBinding(icon, PathIcon.WidthProperty, GlobalResourceKey.IconSizeSM);
         TokenResourceBinder.CreateGlobalTokenBinding(icon, PathIcon.HeightProperty, GlobalResourceKey.IconSizeSM);
      }
   }
   
   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      TokenResourceBinder.CreateGlobalResourceBinding(this, BorderThicknessProperty, GlobalResourceKey.BorderThickness,
                                                      BindingPriority.Template,
                                                      new RenderScaleAwareThicknessConfigure(this));
      _leftAddOnPresenter = e.NameScope.Find<Control>(AddOnDecoratedBoxTheme.LeftAddOnPart);
      _rightAddOnPresenter = e.NameScope.Find<Control>(AddOnDecoratedBoxTheme.RightAddOnPart);
      SetupInnerBoxCornerRadius();
   }
   
   private void SetupAddOnBorderInfo()
   {
      var topLeftRadius = CornerRadius.TopLeft;
      var topRightRadius = CornerRadius.TopRight;
      var bottomLeftRadius = CornerRadius.BottomLeft;
      var bottomRightRadius = CornerRadius.BottomRight;
      
      var topThickness = BorderThickness.Top;
      var rightThickness = BorderThickness.Right;
      var bottomThickness = BorderThickness.Bottom;
      var leftThickness = BorderThickness.Left;

      LeftAddOnCornerRadius = new CornerRadius(topLeft: topLeftRadius,
                                               topRight: 0,
                                               bottomLeft:bottomLeftRadius,
                                               bottomRight:0);
      RightAddOnCornerRadius = new CornerRadius(topLeft: 0,
                                                topRight: topRightRadius,
                                                bottomLeft:0,
                                                bottomRight:bottomRightRadius);

      LeftAddOnBorderThickness = new Thickness(top: topThickness, right:0, bottom:bottomThickness, left: leftThickness);
      RightAddOnBorderThickness = new Thickness(top: topThickness, right:rightThickness, bottom:bottomThickness, left: 0);

      NotifyAddOnBorderInfoCalculated();
   }

   protected virtual void NotifyAddOnBorderInfoCalculated()
   {
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