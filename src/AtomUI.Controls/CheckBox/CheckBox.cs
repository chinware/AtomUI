using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Media;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Rendering;

namespace AtomUI.Controls;

using AvaloniaCheckBox = Avalonia.Controls.CheckBox;

public partial class CheckBox : AvaloniaCheckBox,
                                ICustomHitTest,
                                IWaveAdornerInfoProvider,
                                IControlCustomStyle
{
   private IControlCustomStyle _customStyle;
   private ControlStyleState _styleState;
   private BorderRenderHelper _borderRenderHelper;
   private bool _initialized = false;
   
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
   
   protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnAttachedToVisualTree(e);
      _customStyle.ApplyRenderScalingAwareStyleConfig();
   }

   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (!_initialized) {
         _customStyle.SetupUi();
         _initialized = true;
      }
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
         var originTransform = Matrix.CreateTranslation(indicatorRect.X, indicatorRect.Y);
         using var transformState = context.PushTransform(originTransform);
         _borderRenderHelper.Render(context, indicatorRect.Size,
                                    borderThickness:new Thickness(penWidth),
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
         var deltaSize = (CheckIndicatorSize - checkMarkBounds.Width) / 2;
         var offsetX = deltaSize - checkMarkPenWidth - penWidth;
         var offsetY = deltaSize - checkMarkPenWidth - penWidth;
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

   void IControlCustomStyle.SetupUi()
   {
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
      var offsetY = Math.Ceiling((DesiredSize.Height - Margin.Top - Margin.Bottom - CheckIndicatorSize) / 2);
      return new Rect(0d, offsetY, CheckIndicatorSize, CheckIndicatorSize);
   }

   private Rect TextRect()
   {
      var offsetX = CheckIndicatorSize + PaddingInline;
      return new Rect(offsetX, 0d, DesiredSize.Width - offsetX, DesiredSize.Height);
   }
   
   void IControlCustomStyle.ApplyRenderScalingAwareStyleConfig()
   {
      BindUtils.CreateTokenBinding(this, IndicatorBorderThicknessProperty, GlobalResourceKey.BorderThickness, BindingPriority.Template,
                                   new RenderScaleAwareThicknessConfigure(this));
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