using AtomUI.Controls.Utils;
using AtomUI.Media;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Rendering;

namespace AtomUI.Controls;

using AvaloniaCheckBox = Avalonia.Controls.CheckBox;

public class CheckBox : AvaloniaCheckBox,
                        ICustomHitTest,
                        IWaveAdornerInfoProvider,
                        IControlCustomStyle
{
   internal static readonly StyledProperty<double> CheckIndicatorSizeProperty =
      AvaloniaProperty.Register<CheckBox, double>(nameof(CheckIndicatorSize), 0);

   internal double CheckIndicatorSize
   {
      get => GetValue(CheckIndicatorSizeProperty);
      set => SetValue(CheckIndicatorSizeProperty, value);
   }

   internal static readonly StyledProperty<double> PaddingInlineProperty =
      AvaloniaProperty.Register<CheckBox, double>(nameof(PaddingInline), 0);

   internal double PaddingInline
   {
      get => GetValue(PaddingInlineProperty);
      set => SetValue(PaddingInlineProperty, value);
   }

   internal static readonly StyledProperty<IBrush?> IndicatorBorderBrushProperty =
      AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(IndicatorBorderBrush));

   internal IBrush? IndicatorBorderBrush
   {
      get => GetValue(IndicatorBorderBrushProperty);
      set => SetValue(IndicatorBorderBrushProperty, value);
   }

   internal static readonly StyledProperty<IBrush?> IndicatorCheckedMarkBrushProperty =
      AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(IndicatorCheckedMarkBrush));

   internal IBrush? IndicatorCheckedMarkBrush
   {
      get => GetValue(IndicatorCheckedMarkBrushProperty);
      set => SetValue(IndicatorCheckedMarkBrushProperty, value);
   }

   internal static readonly StyledProperty<double> IndicatorCheckedMarkEffectSizeProperty =
      AvaloniaProperty.Register<CheckBox, double>(nameof(IndicatorCheckedMarkEffectSize));

   internal double IndicatorCheckedMarkEffectSize
   {
      get => GetValue(IndicatorCheckedMarkEffectSizeProperty);
      set => SetValue(IndicatorCheckedMarkEffectSizeProperty, value);
   }

   internal static readonly StyledProperty<IBrush?> IndicatorTristateMarkBrushProperty =
      AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(IndicatorTristateMarkBrush));

   internal IBrush? IndicatorTristateMarkBrush
   {
      get => GetValue(IndicatorTristateMarkBrushProperty);
      set => SetValue(IndicatorTristateMarkBrushProperty, value);
   }

   internal static readonly StyledProperty<double> IndicatorTristateMarkSizeProperty =
      AvaloniaProperty.Register<CheckBox, double>(nameof(IndicatorTristateMarkSize));

   internal double IndicatorTristateMarkSize
   {
      get => GetValue(IndicatorTristateMarkSizeProperty);
      set => SetValue(IndicatorTristateMarkSizeProperty, value);
   }

   internal static readonly StyledProperty<IBrush?> IndicatorBackgroundProperty =
      AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(IndicatorBackground));

   internal IBrush? IndicatorBackground
   {
      get => GetValue(IndicatorBackgroundProperty);
      set => SetValue(IndicatorBackgroundProperty, value);
   }

   internal static readonly StyledProperty<Thickness> IndicatorBorderThicknessProperty =
      AvaloniaProperty.Register<CheckBox, Thickness>(nameof(IndicatorBorderThickness));

   internal Thickness IndicatorBorderThickness
   {
      get => GetValue(IndicatorBorderThicknessProperty);
      set => SetValue(IndicatorBorderThicknessProperty, value);
   }

   internal static readonly StyledProperty<CornerRadius> IndicatorBorderRadiusProperty =
      AvaloniaProperty.Register<CheckBox, CornerRadius>(nameof(IndicatorBorderRadius));

   internal CornerRadius IndicatorBorderRadius
   {
      get => GetValue(IndicatorBorderRadiusProperty);
      set => SetValue(IndicatorBorderRadiusProperty, value);
   }

   private IControlCustomStyle _customStyle;
   private ControlStyleState _styleState;
   private BorderRenderHelper _borderRenderHelper;

   static CheckBox()
   {
      AffectsRender<CheckBox>(
         IsCheckedProperty,
         IndicatorCheckedMarkEffectSizeProperty,
         PaddingInlineProperty,
         IndicatorBorderBrushProperty,
         IndicatorCheckedMarkBrushProperty,
         IndicatorTristateMarkBrushProperty,
         IndicatorBackgroundProperty,
         IndicatorBorderThicknessProperty,
         IndicatorBorderRadiusProperty);
   }

   public CheckBox()
   {
      _customStyle = this;
      _borderRenderHelper = new BorderRenderHelper();
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);
      _customStyle.HandlePropertyChangedForStyle(e);
   }

   protected override Size MeasureOverride(Size availableSize)
   {
      var size = base.MeasureOverride(availableSize);
      var targetWidth = size.Width + CheckIndicatorSize + PaddingInline;
      var targetHeight = Math.Max(size.Height, CheckIndicatorSize);
      return new Size(targetWidth, targetHeight);
   }

   protected override Size ArrangeOverride(Size finalSize)
   {
      var arrangeRect = TextRect();

      var visualChildren = VisualChildren;
      var visualCount = visualChildren.Count;

      for (var i = 0; i < visualCount; i++) {
         Visual visual = visualChildren[i];
         if (visual is Layoutable layoutable) {
            layoutable.Arrange(arrangeRect);
         }
      }

      return finalSize;
   }

   public sealed override void Render(DrawingContext context)
   {
      var indicatorRect = IndicatorRect();
      var penWidth = IndicatorBorderThickness.Top;
      var borderRadius = GeometryUtils.CornerRadiusScalarValue(IndicatorBorderRadius);
      {
         _borderRenderHelper.Render(context, indicatorRect.Size,
                                    borderThickness: new Thickness(penWidth),
                                    new CornerRadius(borderRadius),
                                    BackgroundSizing.OuterBorderEdge,
                                    IndicatorBackground,
                                    IndicatorBorderBrush,
                                    new BoxShadows());
      }
      if (_styleState.HasFlag(ControlStyleState.On)) {
         var checkMarkGeometry =
            CommonShapeBuilder.BuildCheckMark(new Size(IndicatorCheckedMarkEffectSize, IndicatorCheckedMarkEffectSize));
         var checkMarkPenWidth = 2;
         var checkMarkPen = new Pen(IndicatorCheckedMarkBrush, 2);
         var checkMarkBounds = checkMarkGeometry.GetRenderBounds(checkMarkPen);
         var deltaWidth = (CheckIndicatorSize - checkMarkBounds.Width) / 2;
         var deltaHeight = (CheckIndicatorSize - checkMarkBounds.Height) / 2;
         var offsetX = indicatorRect.X + deltaWidth - checkMarkPenWidth - penWidth;
         var offsetY = indicatorRect.Y + deltaHeight - checkMarkPenWidth - penWidth * 3;
         checkMarkGeometry.Transform = new TranslateTransform(offsetX, offsetY);
         context.DrawGeometry(null, checkMarkPen, checkMarkGeometry);
      } else if (_styleState.HasFlag(ControlStyleState.Indeterminate)) {
         double deltaSize = (CheckIndicatorSize - IndicatorTristateMarkSize) / 2.0;
         var offsetX = indicatorRect.X + deltaSize;
         var offsetY = indicatorRect.Y + deltaSize;
         var indicatorTristateRect = new Rect(offsetX, offsetY, IndicatorTristateMarkSize, IndicatorTristateMarkSize);
         context.FillRectangle(IndicatorTristateMarkBrush!, indicatorTristateRect);
      }
   }

   public bool HitTest(Point point)
   {
      return true;
   }

   #region IControlCustomStyle 实现

   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      _customStyle.HandleTemplateApplied(e.NameScope);
   }

   void IControlCustomStyle.HandleTemplateApplied(INameScope scope)
   {
      TokenResourceBinder.CreateGlobalResourceBinding(this, IndicatorBorderThicknessProperty, GlobalResourceKey.BorderThickness, BindingPriority.Template,
         new RenderScaleAwareThicknessConfigure(this));
      HorizontalAlignment = HorizontalAlignment.Left;
      Cursor = new Cursor(StandardCursorType.Hand);
      _customStyle.CollectStyleState();
      SetupIndicatorCheckedMarkEffectSize();
      _customStyle.SetupTransitions();
   }

   void IControlCustomStyle.CollectStyleState()
   {
      ControlStateUtils.InitCommonState(this, ref _styleState);
      switch (IsChecked) {
         case true:
            _styleState |= ControlStyleState.On;
            break;
         case false:
            _styleState |= ControlStyleState.Off;
            break;
         default:
            _styleState |= ControlStyleState.Indeterminate;
            break;
      }

      if (IsPressed) {
         _styleState |= ControlStyleState.Sunken;
      } else {
         _styleState |= ControlStyleState.Raised;
      }
   }

   // Measure 之后才有值
   private Rect IndicatorRect()
   {
      // var offsetY = Math.Ceiling((DesiredSize.Height - Margin.Top - Margin.Bottom - CheckIndicatorSize) / 2);
      return new Rect(0d, 0d, CheckIndicatorSize, CheckIndicatorSize);
   }

   private Rect TextRect()
   {
      var offsetX = CheckIndicatorSize + PaddingInline;
      return new Rect(offsetX, 0d, DesiredSize.Width - offsetX, DesiredSize.Height);
   }

   private void SetupIndicatorCheckedMarkEffectSize()
   {
      if (_styleState.HasFlag(ControlStyleState.Enabled)) {
         if (_styleState.HasFlag(ControlStyleState.On)) {
            IndicatorCheckedMarkEffectSize = CheckIndicatorSize;
         } else if (_styleState.HasFlag(ControlStyleState.Off)) {
            IndicatorCheckedMarkEffectSize = CheckIndicatorSize * 0.7;
         } else if (_styleState.HasFlag(ControlStyleState.Indeterminate)) {
            IndicatorCheckedMarkEffectSize = CheckIndicatorSize * 0.7;
         }
      } else {
         if (_styleState.HasFlag(ControlStyleState.On)) {
            IndicatorCheckedMarkEffectSize = CheckIndicatorSize;
         }
      }
   }

   void IControlCustomStyle.SetupTransitions()
   {
      Transitions = new Transitions()
      {
         AnimationUtils.CreateTransition<SolidColorBrushTransition>(IndicatorBackgroundProperty),
         AnimationUtils.CreateTransition<SolidColorBrushTransition>(IndicatorBorderBrushProperty),
         AnimationUtils.CreateTransition<SolidColorBrushTransition>(IndicatorTristateMarkBrushProperty),
         AnimationUtils.CreateTransition<DoubleTransition>(IndicatorCheckedMarkEffectSizeProperty,
                                                           GlobalResourceKey.MotionDurationMid, new BackEaseOut())
      };
   }

   void IControlCustomStyle.HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
   {
      if (e.Property == IsPointerOverProperty ||
          e.Property == IsCheckedProperty ||
          e.Property == IsEnabledProperty) {
         _customStyle.CollectStyleState();
         SetupIndicatorCheckedMarkEffectSize();
         if (e.Property == IsCheckedProperty &&
             _styleState.HasFlag(ControlStyleState.Enabled) &&
             _styleState.HasFlag(ControlStyleState.On)) {
            WaveSpiritAdorner.ShowWaveAdorner(this, WaveType.RoundRectWave);
         }
      }
   }

   public Rect WaveGeometry()
   {
      return IndicatorRect();
   }

   public CornerRadius WaveBorderRadius()
   {
      return IndicatorBorderRadius;
   }

   #endregion
}