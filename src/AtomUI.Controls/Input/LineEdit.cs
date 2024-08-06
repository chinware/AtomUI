using AtomUI.Controls.Utils;
using AtomUI.Media;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;

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

   private readonly BorderRenderHelper _borderRenderHelper;
   private ContentPresenter? _leftAddOnPresenter;
   private ContentPresenter? _rightAddOnPresenter;

   static LineEdit()
   {
      AffectsRender<LineEdit>(BorderBrushProperty);
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

      if (Transitions is null) {
         Transitions = new Transitions();
         Transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(BorderBrushProperty));
      }
   }

   public override void Render(DrawingContext context)
   {
      var borderRect = CalculateInputBoxRect();
      var borderRadius = SetupBorderRadius(CornerRadius);
      using var state = context.PushTransform(Matrix.CreateTranslation(borderRect.X, borderRect.Y));
      _borderRenderHelper.Render(context,
                                 borderThickness: BorderThickness,
                                 backgroundSizing: BackgroundSizing.OuterBorderEdge,
                                 finalSize: borderRect.Size,
                                 cornerRadius: borderRadius,
                                 background: null,
                                 borderBrush: BorderBrush,
                                 boxShadows: new BoxShadows());
   }

   private CornerRadius SetupBorderRadius(in CornerRadius cornerRadius)
   {
      var topLeftRadius = cornerRadius.TopLeft;
      var topRightRadius = cornerRadius.TopRight;
      
      var bottomLeftRadius = cornerRadius.BottomLeft;
      var bottomRightRadius = cornerRadius.BottomRight;

      if (_leftAddOnPresenter is not null && _leftAddOnPresenter.IsVisible) {
         topLeftRadius = 0;
         bottomLeftRadius = 0;
      }
      if (_rightAddOnPresenter is not null && _rightAddOnPresenter.IsVisible) {
         topRightRadius = 0;
         bottomRightRadius = 0;
      }

      return new CornerRadius(topLeft: topLeftRadius,
                              topRight: topRightRadius,
                              bottomLeft:bottomLeftRadius,
                              bottomRight:bottomRightRadius);
   }

   private Rect CalculateInputBoxRect()
   {
      var offsetStart = 0d;
      var offsetEnd = 0d;
      if (_leftAddOnPresenter is not null && _leftAddOnPresenter.IsVisible) {
         offsetStart += _leftAddOnPresenter.DesiredSize.Width;
      }
      if (_rightAddOnPresenter is not null && _rightAddOnPresenter.IsVisible) {
         offsetEnd += _rightAddOnPresenter.DesiredSize.Width;
      }

      return new Rect(new Point(offsetStart, Bounds.Y),
                      new Size(Bounds.Width - offsetStart + offsetEnd, Bounds.Height));
   }
}