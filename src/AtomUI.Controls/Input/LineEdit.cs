using AtomUI.Controls.Utils;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;

namespace AtomUI.Controls;

public enum TextBoxVariant
{
   Outline,
   Filled,
   Borderless
}

public enum TextBoxStatus
{
   Default,
   Warning,
   Error
}

public class LineEdit : TextBox
{
   #region 功能属性定义

   public static readonly StyledProperty<object?> LeftAddOnProperty =
      AvaloniaProperty.Register<LineEdit, object?>(nameof(LeftAddOn));

   public static readonly StyledProperty<object?> RightAddOnProperty =
      AvaloniaProperty.Register<LineEdit, object?>(nameof(RightAddOn));
   
   public static readonly StyledProperty<SizeType> SizeTypeProperty =
      AvaloniaProperty.Register<LineEdit, SizeType>(nameof(SizeType), SizeType.Middle);
   
   public static readonly StyledProperty<TextBoxVariant> StyleVariantProperty =
      AvaloniaProperty.Register<LineEdit, TextBoxVariant>(nameof(StyleVariant), TextBoxVariant.Outline);
   
   public static readonly StyledProperty<TextBoxStatus> StatusProperty =
      AvaloniaProperty.Register<LineEdit, TextBoxStatus>(nameof(Status), TextBoxStatus.Default);
   
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
   
   public TextBoxVariant StyleVariant
   {
      get => GetValue(StyleVariantProperty);
      set => SetValue(StyleVariantProperty, value);
   }
   
   public TextBoxStatus Status
   {
      get => GetValue(StatusProperty);
      set => SetValue(StatusProperty, value);
   }

   #endregion

   #region 内部属性定义

   internal static readonly StyledProperty<CornerRadius> EditKernelCornerRadiusProperty =
      AvaloniaProperty.Register<LineEdit, CornerRadius>(nameof(EditKernelCornerRadius));
   
   internal static readonly StyledProperty<CornerRadius> LeftAddOnCornerRadiusProperty =
      AvaloniaProperty.Register<LineEdit, CornerRadius>(nameof(LeftAddOnCornerRadius));
   
   internal static readonly StyledProperty<CornerRadius> RightAddOnCornerRadiusProperty =
      AvaloniaProperty.Register<LineEdit, CornerRadius>(nameof(RightAddOnCornerRadius));
   
   internal static readonly StyledProperty<Thickness> LeftAddOnBorderThicknessProperty =
      AvaloniaProperty.Register<LineEdit, Thickness>(nameof(LeftAddOnBorderThickness));
   
   internal static readonly StyledProperty<Thickness> RightAddOnBorderThicknessProperty =
      AvaloniaProperty.Register<LineEdit, Thickness>(nameof(RightAddOnBorderThickness));
   
   internal CornerRadius EditKernelCornerRadius
   {
      get => GetValue(EditKernelCornerRadiusProperty);
      set => SetValue(EditKernelCornerRadiusProperty, value);
   }
   
   internal CornerRadius LeftAddOnCornerRadius
   {
      get => GetValue(LeftAddOnCornerRadiusProperty);
      set => SetValue(LeftAddOnCornerRadiusProperty, value);
   }
   
   internal CornerRadius RightAddOnCornerRadius
   {
      get => GetValue(RightAddOnCornerRadiusProperty);
      set => SetValue(RightAddOnCornerRadiusProperty, value);
   }
   
   internal Thickness LeftAddOnBorderThickness
   {
      get => GetValue(LeftAddOnBorderThicknessProperty);
      set => SetValue(LeftAddOnBorderThicknessProperty, value);
   }
   
   internal Thickness RightAddOnBorderThickness
   {
      get => GetValue(RightAddOnBorderThicknessProperty);
      set => SetValue(RightAddOnBorderThicknessProperty, value);
   }
   
   #endregion
   
   private readonly BorderRenderHelper _borderRenderHelper;
   private ContentPresenter? _leftAddOnPresenter;
   private ContentPresenter? _rightAddOnPresenter;
   private Border? _lineEditKernelDecorator;


   static LineEdit()
   {
      AffectsRender<LineEdit>(BorderBrushProperty, BackgroundProperty);
      AffectsMeasure<LineEdit>(LeftAddOnProperty, RightAddOnProperty);
   }

   public LineEdit()
   {
      _borderRenderHelper = new BorderRenderHelper();
   }

   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      TokenResourceBinder.CreateGlobalResourceBinding(this, BorderThicknessProperty, GlobalResourceKey.BorderThickness, 
                                                      BindingPriority.Template, new RenderScaleAwareThicknessConfigure(this));
      _leftAddOnPresenter = e.NameScope.Find<ContentPresenter>(LineEditTheme.LeftAddOnPart);
      _rightAddOnPresenter = e.NameScope.Find<ContentPresenter>(LineEditTheme.RightAddOnPart);
      _lineEditKernelDecorator = e.NameScope.Find<Border>(LineEditTheme.LineEditKernelDecoratorPart);
      SetupEditKernelCornerRadius();
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
   {
      base.OnPropertyChanged(change);

      if (VisualRoot is not null) {
         if (change.Property == LeftAddOnProperty || change.Property == RightAddOnProperty) {
            SetupEditKernelCornerRadius();
         }
      }

      if (change.Property == CornerRadiusProperty || change.Property == BorderThicknessProperty) {
         SetupAddOnBorderInfo();
      }
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
   }

   private void SetupEditKernelCornerRadius()
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

      EditKernelCornerRadius = new CornerRadius(topLeft: topLeftRadius,
                              topRight: topRightRadius,
                              bottomLeft:bottomLeftRadius,
                              bottomRight:bottomRightRadius);
   }

   protected override Size MeasureOverride(Size availableSize)
   {
      return base.MeasureOverride(new Size(availableSize.Width - BorderThickness.Left - BorderThickness.Right, availableSize.Height));
   }
   
   protected override Size ArrangeOverride(Size finalSize)
   {
      return base.ArrangeOverride(new Size(finalSize.Width - BorderThickness.Left - BorderThickness.Right, finalSize.Height));
   }

   protected override void OnPointerPressed(PointerPressedEventArgs e)
   {
      if (_lineEditKernelDecorator is null) {
         return;
      }
      var targetRect = new Rect(_lineEditKernelDecorator.DesiredSize);
      if (!targetRect.Contains(e.GetPosition(_lineEditKernelDecorator))) {
         return;
      }
      base.OnPointerPressed(e);
   }
}