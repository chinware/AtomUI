using AtomUI.Controls.Utils;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;

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
   
   internal CornerRadius EditKernelCornerRadius
   {
      get => GetValue(EditKernelCornerRadiusProperty);
      set => SetValue(EditKernelCornerRadiusProperty, value);
   }
   
   internal static readonly StyledProperty<CornerRadius> LeftAddOnCornerRadiusProperty =
      AvaloniaProperty.Register<LineEdit, CornerRadius>(nameof(LeftAddOnCornerRadius));
   
   internal CornerRadius LeftAddOnCornerRadius
   {
      get => GetValue(LeftAddOnCornerRadiusProperty);
      set => SetValue(LeftAddOnCornerRadiusProperty, value);
   }
   
   internal static readonly StyledProperty<CornerRadius> RightAddOnCornerRadiusProperty =
      AvaloniaProperty.Register<LineEdit, CornerRadius>(nameof(RightAddOnCornerRadius));
   
   internal CornerRadius RightAddOnCornerRadius
   {
      get => GetValue(RightAddOnCornerRadiusProperty);
      set => SetValue(RightAddOnCornerRadiusProperty, value);
   }
   
   #endregion
   
   private readonly BorderRenderHelper _borderRenderHelper;
   private ContentPresenter? _leftAddOnPresenter;
   private ContentPresenter? _rightAddOnPresenter;
   private LineEditKernel? _lineEditKernel;


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
      _lineEditKernel = e.NameScope.Find<LineEditKernel>(LineEditTheme.LineEditKernelPart);
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

      if (change.Property == CornerRadiusProperty) {
         SetupAddOnCornerRadius();
      }
   }

   private void SetupAddOnCornerRadius()
   {
      var topLeftRadius = CornerRadius.TopLeft;
      var topRightRadius = CornerRadius.TopRight;
      var bottomLeftRadius = CornerRadius.BottomLeft;
      var bottomRightRadius = CornerRadius.BottomRight;

      LeftAddOnCornerRadius = new CornerRadius(topLeft: topLeftRadius,
                                               topRight: 0,
                                               bottomLeft:bottomLeftRadius,
                                               bottomRight:0);
      RightAddOnCornerRadius = new CornerRadius(topLeft: 0,
                                               topRight: topRightRadius,
                                               bottomLeft:0,
                                               bottomRight:bottomRightRadius);
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

   protected override Size ArrangeOverride(Size finalSize)
   {
      var offsetLeft = 0d;
      var offsetRight = finalSize.Width;
      var controlRect = new Rect(new Point(0, 0), finalSize);
      if (_leftAddOnPresenter is not null && _leftAddOnPresenter.IsVisible) {
         offsetLeft += _leftAddOnPresenter.DesiredSize.Width - BorderThickness.Left;
         _leftAddOnPresenter.Arrange(controlRect);
      }

      if (_rightAddOnPresenter is not null && _rightAddOnPresenter.IsVisible) {
         offsetRight -= _rightAddOnPresenter.DesiredSize.Width - BorderThickness.Right;
         _rightAddOnPresenter.Arrange(controlRect);
      }

      if (_lineEditKernel is not null) {
         var width = offsetRight - offsetLeft;
         if (_leftAddOnPresenter is not null && _leftAddOnPresenter.IsVisible) {
            offsetLeft -= BorderThickness.Left;
            width += BorderThickness.Left;
         }

         if (_rightAddOnPresenter is not null && _rightAddOnPresenter.IsVisible) {
            width += BorderThickness.Right;
         }
         _lineEditKernel.Arrange(new Rect(new Point(offsetLeft, 0), new Size(width, finalSize.Height)));
      }

      return finalSize;
   }
}