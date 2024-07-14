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
   private ControlTokenBinder _controlTokenBinder;
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
      _controlTokenBinder = new ControlTokenBinder(this, CheckBoxToken.ID);
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
      Cursor = new Cursor(StandardCursorType.Hand);
      _customStyle.CollectStyleState();
      _customStyle.ApplyFixedStyleConfig();
      _customStyle.ApplyVariableStyleConfig();
      _customStyle.SetupTransitions();
   }

   void IControlCustomStyle.CollectStyleState()
   {
      StyleUtils.InitCommonState(this, ref _styleState);
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
      var offsetY = (DesiredSize.Height - Margin.Top - Margin.Bottom - CheckIndicatorSize) / 2;
      return new Rect(0d, offsetY, CheckIndicatorSize, CheckIndicatorSize);
   }

   private Rect TextRect()
   {
      var offsetX = CheckIndicatorSize + PaddingInline;
      return new Rect(offsetX, 0d, DesiredSize.Width - offsetX, DesiredSize.Height);
   }

   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      _controlTokenBinder.AddControlBinding(CheckIndicatorSizeProperty, CheckBoxResourceKey.CheckIndicatorSize);
      _controlTokenBinder.AddControlBinding(PaddingInlineProperty, GlobalResourceKey.PaddingXS);
      _controlTokenBinder.AddControlBinding(IndicatorBorderRadiusProperty, GlobalResourceKey.BorderRadiusSM);
      _controlTokenBinder.AddControlBinding(IndicatorTristateMarkSizeProperty,
                                            CheckBoxResourceKey.IndicatorTristateMarkSize);
      _controlTokenBinder.AddControlBinding(IndicatorTristateMarkBrushProperty, GlobalResourceKey.ColorPrimary);
   }
   
   void IControlCustomStyle.ApplyRenderScalingAwareStyleConfig()
   {
      _controlTokenBinder.AddControlBinding(IndicatorBorderThicknessProperty, GlobalResourceKey.BorderThickness, BindingPriority.Style,
                                            new RenderScaleAwareThicknessConfigure(this));
   }

   void IControlCustomStyle.ApplyVariableStyleConfig()
   {
      _controlTokenBinder.ReleaseTriggerBindings(this);
      if (_styleState.HasFlag(ControlStyleState.Enabled)) {
         _controlTokenBinder.AddControlBinding(ForegroundProperty, GlobalResourceKey.ColorText);
         _controlTokenBinder.AddControlBinding(IndicatorBackgroundProperty, GlobalResourceKey.ColorBgContainer);
         _controlTokenBinder.AddControlBinding(IndicatorCheckedMarkBrushProperty, GlobalResourceKey.ColorBgContainer);
         _controlTokenBinder.AddControlBinding(IndicatorBorderBrushProperty, GlobalResourceKey.ColorBorder);
         if (_styleState.HasFlag(ControlStyleState.On)) {
            IndicatorCheckedMarkEffectSize = CheckIndicatorSize;
            _controlTokenBinder.AddControlBinding(IndicatorBackgroundProperty, GlobalResourceKey.ColorPrimary,
                                                  BindingPriority.StyleTrigger);
            _controlTokenBinder.AddControlBinding(IndicatorBorderBrushProperty, GlobalResourceKey.ColorPrimary,
                                                  BindingPriority.StyleTrigger);
            if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
               _controlTokenBinder.AddControlBinding(IndicatorBackgroundProperty, GlobalResourceKey.ColorPrimaryHover,
                                                     BindingPriority.StyleTrigger);
               _controlTokenBinder.AddControlBinding(IndicatorBorderBrushProperty, GlobalResourceKey.ColorPrimaryHover,
                                                     BindingPriority.StyleTrigger);
            }
         } else if (_styleState.HasFlag(ControlStyleState.Off)) {
            if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
               _controlTokenBinder.AddControlBinding(IndicatorBorderBrushProperty, GlobalResourceKey.ColorPrimaryHover,
                                                     BindingPriority.StyleTrigger);
            }

            IndicatorCheckedMarkEffectSize = CheckIndicatorSize * 0.7;
         } else if (_styleState.HasFlag(ControlStyleState.Indeterminate)) {
            IndicatorCheckedMarkEffectSize = CheckIndicatorSize * 0.7;
            if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
               _controlTokenBinder.AddControlBinding(IndicatorBorderBrushProperty, GlobalResourceKey.ColorPrimaryHover,
                                                     BindingPriority.StyleTrigger);
            }
         }
      } else {
         _controlTokenBinder.AddControlBinding(IndicatorBackgroundProperty, GlobalResourceKey.ColorBgContainerDisabled);
         _controlTokenBinder.AddControlBinding(IndicatorBorderBrushProperty, GlobalResourceKey.ColorBorder);
         _controlTokenBinder.AddControlBinding(ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
         if (_styleState.HasFlag(ControlStyleState.On)) {
            IndicatorCheckedMarkEffectSize = CheckIndicatorSize;
            _controlTokenBinder.AddControlBinding(IndicatorCheckedMarkBrushProperty,
                                                  GlobalResourceKey.ColorTextDisabled,
                                                  BindingPriority.StyleTrigger);
         } else if (_styleState.HasFlag(ControlStyleState.Indeterminate)) {
            _controlTokenBinder.AddControlBinding(IndicatorTristateMarkBrushProperty,
                                                  GlobalResourceKey.ColorTextDisabled,
                                                  BindingPriority.StyleTrigger);
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
         _customStyle.ApplyVariableStyleConfig();
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