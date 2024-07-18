using AtomUI.Controls.Switch;
using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Icon;
using AtomUI.Media;
using AtomUI.Styling;
using AtomUI.TokenSystem;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Rendering;

namespace AtomUI.Controls;

public partial class ToggleSwitch : ToggleButton,
                                    ISizeTypeAware, 
                                    ICustomHitTest, 
                                    IWaveAdornerInfoProvider,
                                    IControlCustomStyle
{
   private bool _initialized = false;
   private bool _transitionInitialized = false;
   private const double STRETCH_FACTOR = 1.3d;
   private IControlCustomStyle _customStyle;
   private ControlStyleState _styleState;
   private Canvas? _togglePanel;

   private SwitchKnob _switchKnob;

   static ToggleSwitch()
   {
      OffContentProperty.Changed.AddClassHandler<ToggleSwitch>((toggleSwitch, args) =>
      {
         toggleSwitch.OffContentChanged(args);
      });
      OnContentProperty.Changed.AddClassHandler<ToggleSwitch>((toggleSwitch, args) =>
      {
         toggleSwitch.OnContentChanged(args);
      });

      AffectsMeasure<ToggleSwitch>(SizeTypeProperty,
         HandleSizeTokenProperty,
         HandleSizeSMTokenProperty,
         InnerMaxMarginTokenProperty,
         InnerMaxMarginSMTokenProperty,
         InnerMinMarginTokenProperty,
         InnerMinMarginSMTokenProperty,
         TrackHeightTokenProperty,
         TrackHeightSMTokenProperty,
         TrackMinWidthTokenProperty,
         TrackMinWidthSMTokenProperty,
         TrackPaddingTokenProperty
      );
      AffectsArrange<ToggleSwitch>(
         IsPressedProperty,
         KnobOffsetProperty,
         OnContentOffsetProperty,
         OffContentOffsetProperty);
      AffectsRender<ToggleSwitch>(GrooveBackgroundProperty,
         SwitchOpacityProperty);
   }

   public ToggleSwitch()
   {
      _customStyle = this;
      _switchKnob = new SwitchKnob();
      LayoutUpdated += HandleLayoutUpdated;
   }
   
   private void HandleLayoutUpdated(object? sender, EventArgs args)
   {
      if (!_transitionInitialized) {
         _transitionInitialized = true;
         _customStyle.SetupTransitions();
      }
   }

   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (!_initialized) {
         _customStyle.SetupUi();
         _initialized = true;
      }
   }

   protected override Size MeasureOverride(Size availableSize)
   {
      double extraInfoWidth = 0;

      if (OffContent is Layoutable offLayoutable) {
         offLayoutable.Measure(availableSize);
         extraInfoWidth = Math.Max(extraInfoWidth, offLayoutable.DesiredSize.Width);
      }

      if (OnContent is Layoutable onLayoutable) {
         onLayoutable.Measure(availableSize);
         extraInfoWidth = Math.Max(extraInfoWidth, onLayoutable.DesiredSize.Width);
      }

      double switchHeight = TrackHeight();
      double switchWidth = extraInfoWidth;
      double trackMinWidth = TrackMinWidth();
      switchWidth += InnerMinMargin() + InnerMaxMargin();
      switchWidth = Math.Max(switchWidth, trackMinWidth);
      var targetSize = new Size(switchWidth, switchHeight);
      CalculateElementsOffset(targetSize);
      return targetSize;
   }

   protected override Size ArrangeOverride(Size finalSize)
   {
      base.ArrangeOverride(finalSize);
      Canvas.SetLeft(_switchKnob, KnobOffset.X);
      Canvas.SetTop(_switchKnob, KnobOffset.Y);
      AdjustExtraInfoOffset();
      return finalSize;
   }

   private void AdjustExtraInfoOffset()
   {
      if (OffContent is Control offControl) {
         Canvas.SetLeft(offControl, OffContentOffset.X);
         Canvas.SetTop(offControl, OffContentOffset.Y);
      }

      if (OnContent is Control onControl) {
         Canvas.SetLeft(onControl, OnContentOffset.X);
         Canvas.SetTop(onControl, OnContentOffset.Y);
      }
   }

   private void AdjustOffsetOnPressed()
   {
      var handleRect = HandleRect();
      KnobOffset = handleRect.TopLeft;
      var handleSize = HandleSize();
      var delta = handleRect.Width - handleSize;

      var contentOffsetDelta = handleSize * (STRETCH_FACTOR - 1);

      if (IsChecked.HasValue && IsChecked.Value) {
         // 点击的时候如果是选中，需要调整坐标
         KnobOffset = new Point(KnobOffset.X - delta, KnobOffset.Y);
         OnContentOffset = new Point(OnContentOffset.X - contentOffsetDelta, OffContentOffset.Y);
      } else {
         OffContentOffset = new Point(OffContentOffset.X + contentOffsetDelta, OffContentOffset.Y);
      }
      
      var handleWidth = handleSize * STRETCH_FACTOR;
      _switchKnob.KnobSize = new Size(handleWidth, handleSize);
   }

   private void AdjustOffsetOnReleased()
   {
      var handleSize = HandleSize();
      _switchKnob.KnobSize = new Size(handleSize, handleSize);
      CalculateElementsOffset(DesiredSize);
   }

   protected override void OnPointerPressed(PointerPressedEventArgs e)
   {
      if (!IsLoading) {
         base.OnPointerPressed(e);
         AdjustOffsetOnPressed();
      }
   }

   protected override void OnPointerReleased(PointerReleasedEventArgs e)
   {
      if (!IsLoading) {
         base.OnPointerReleased(e);
         AdjustOffsetOnReleased();
         InvalidateArrange();
      }
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);
      _customStyle.HandlePropertyChangedForStyle(e);
   }

   private void OffContentChanged(AvaloniaPropertyChangedEventArgs e)
   {
      if (e.OldValue is ILogical oldChild) {
         LogicalChildren.Remove(oldChild);
      }

      if (e.NewValue is ILogical newChild) {
         LogicalChildren.Add(newChild);
      }
   }

   private void OnContentChanged(AvaloniaPropertyChangedEventArgs e)
   {
      if (e.OldValue is ILogical oldChild) {
         LogicalChildren.Remove(oldChild);
      }

      if (e.NewValue is ILogical newChild) {
         LogicalChildren.Add(newChild);
      }
   }

   public sealed override void Render(DrawingContext context)
   {
      using var state = context.PushOpacity(SwitchOpacity);
      context.DrawPilledRect(GrooveBackground, null, GrooveRect());
   }
   
   public bool HitTest(Point point)
   {
      if (!IsEnabled || IsLoading) {
         return false;
      }

      var grooveRect = GrooveRect();
      return grooveRect.Contains(point);
   }

   #region IControlCustomStyle 实现

   void IControlCustomStyle.SetupUi()
   {
      if (!IsLoading) {
         Cursor = new Cursor(StandardCursorType.Hand);
      }
      
      _customStyle.CollectStyleState();
      _customStyle.ApplyFixedStyleConfig();
      _customStyle.ApplyVariableStyleConfig();
      _customStyle.ApplySizeTypeStyleConfig();

      var handleSize = HandleSize();
      _switchKnob.KnobSize = new Size(handleSize, handleSize);
      
      BindUtils.CreateTokenBinding(_switchKnob, SwitchKnob.KnobBackgroundColorProperty, ToggleSwitchResourceKey.HandleBg);
      BindUtils.CreateTokenBinding(_switchKnob, SwitchKnob.KnobBoxShadowProperty, ToggleSwitchResourceKey.HandleShadow);
     
      _togglePanel = new Canvas();
      _togglePanel.Children.Add(_switchKnob);
      if (OffContent is Control offControl) {
         _togglePanel.Children.Add(offControl);
         if (OffContent is TemplatedControl templatedControl) {
            templatedControl.Padding = new Thickness(0);
         } else if (OffContent is PathIcon iconControl) {
            if (iconControl.ThemeType != IconThemeType.TwoTone) {
               iconControl.NormalFilledBrush ??= Foreground;
            }
         }
         offControl.Width = offControl.Height = IconSize();
      } else if (OffContent is string offStr) {
         var label = new Label
         {
            Content = offStr,
            Padding = new Thickness(0)
         };
         _togglePanel.Children.Add(label);
         OffContent = label;
      }
      
      // TODO 这里要不要限定支持的 control 类型呢？
      if (OnContent is Control onControl) {
         _togglePanel.Children.Add(onControl);
         if (OnContent is TemplatedControl templatedControl) {
            templatedControl.Padding = new Thickness(0);
         } else if (OnContent is PathIcon iconControl) {
            if (iconControl.ThemeType != IconThemeType.TwoTone) {
               iconControl.NormalFilledBrush ??= Foreground;
            }
         }

         onControl.Width = onControl.Height = IconSize();
      } else if (OnContent is string onStr) {
         var label = new Label
         {
            Content = onStr,
            Padding = new Thickness(0)
         };
         _togglePanel.Children.Add(label);
         OnContent = label;
      }

      HandleLoadingState(IsLoading);
      
      Content = _togglePanel;
   }

   private void CalculateElementsOffset(Size controlSize)
   {
      var isChecked = IsChecked.HasValue && IsChecked.Value;
      var handleRect = HandleRect(isChecked, controlSize);
      KnobOffset = handleRect.TopLeft;

      var onExtraInfoRect = ExtraInfoRect(true, controlSize);
      var offExtraInfoRect = ExtraInfoRect(false, controlSize);
      if (isChecked) {
         OnContentOffset = onExtraInfoRect.TopLeft;
         OffContentOffset = new Point(controlSize.Width + 1, onExtraInfoRect.Top);
      } else {
         OffContentOffset = offExtraInfoRect.TopLeft;
         OnContentOffset = new Point(-offExtraInfoRect.Width, offExtraInfoRect.Top);
      }
   }

   void IControlCustomStyle.SetupTransitions()
   { 
      Transitions = new Transitions()
      {
         AnimationUtils.CreateTransition<PointTransition>(KnobOffsetProperty),
         AnimationUtils.CreateTransition<PointTransition>(OnContentOffsetProperty),
         AnimationUtils.CreateTransition<PointTransition>(OffContentOffsetProperty),
         AnimationUtils.CreateTransition<SolidColorBrushTransition>(GrooveBackgroundProperty),
         AnimationUtils.CreateTransition<DoubleTransition>(SwitchOpacityProperty),
      };
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

   void IControlCustomStyle.ApplyVariableStyleConfig()
   {
      if (_styleState.HasFlag(ControlStyleState.Enabled)) {
         SwitchOpacity = 1.0;
         if (_styleState.HasFlag(ControlStyleState.On)) {
            BindUtils.CreateTokenBinding(this, GrooveBackgroundProperty, ToggleSwitchResourceKey.SwitchColor);
            if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
               BindUtils.CreateTokenBinding(this, GrooveBackgroundProperty, GlobalResourceKey.ColorPrimaryHover, BindingPriority.StyleTrigger);
            }
         } else {
            BindUtils.CreateTokenBinding(this, GrooveBackgroundProperty, GlobalResourceKey.ColorTextQuaternary);
            if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
               BindUtils.CreateTokenBinding(this, GrooveBackgroundProperty, GlobalResourceKey.ColorTextTertiary, BindingPriority.StyleTrigger);
            }
         }
      } else {
         SwitchOpacity = _switchDisabledOpacity;
      }
   }

   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      BindUtils.CreateTokenBinding(this, HandleSizeTokenProperty, ToggleSwitchResourceKey.HandleSize);
      BindUtils.CreateTokenBinding(this, HandleSizeSMTokenProperty, ToggleSwitchResourceKey.HandleSizeSM);
      BindUtils.CreateTokenBinding(this, InnerMaxMarginTokenProperty, ToggleSwitchResourceKey.InnerMaxMargin);
      BindUtils.CreateTokenBinding(this, InnerMaxMarginSMTokenProperty, ToggleSwitchResourceKey.InnerMaxMarginSM);
      BindUtils.CreateTokenBinding(this, InnerMinMarginTokenProperty, ToggleSwitchResourceKey.InnerMinMargin);
      BindUtils.CreateTokenBinding(this, InnerMinMarginSMTokenProperty, ToggleSwitchResourceKey.InnerMinMarginSM);
      BindUtils.CreateTokenBinding(this, TrackHeightTokenProperty, ToggleSwitchResourceKey.TrackHeight);
      BindUtils.CreateTokenBinding(this, TrackHeightSMTokenProperty, ToggleSwitchResourceKey.TrackHeightSM);
      BindUtils.CreateTokenBinding(this, TrackMinWidthTokenProperty, ToggleSwitchResourceKey.TrackMinWidth);
      BindUtils.CreateTokenBinding(this, TrackMinWidthSMTokenProperty, ToggleSwitchResourceKey.TrackMinWidthSM);
      BindUtils.CreateTokenBinding(this, TrackPaddingTokenProperty, ToggleSwitchResourceKey.TrackPadding);
      BindUtils.CreateTokenBinding(this, SwitchDisabledOpacityTokenProperty, ToggleSwitchResourceKey.SwitchDisabledOpacity);
      
      BindUtils.CreateTokenBinding(this, ForegroundProperty, GlobalResourceKey.ColorTextLightSolid);

      SwitchOpacity = 1d;
   }
   
   void IControlCustomStyle.ApplySizeTypeStyleConfig()
   {
      if (IsDefaultSize()) {
         BindUtils.CreateTokenBinding(this, FontSizeProperty, ToggleSwitchResourceKey.ExtraInfoFontSize);
      } else {
         BindUtils.CreateTokenBinding(this, FontSizeProperty, ToggleSwitchResourceKey.ExtraInfoFontSizeSM);
      }
   }

   void IControlCustomStyle.HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
   {
      if (e.Property == DesiredSizeProperty) {
         var handleSize = HandleSize();
         _switchKnob.KnobSize = new Size(handleSize, handleSize);
      }
      if ((e.Property == IsPointerOverProperty && !IsLoading) ||
          e.Property == IsCheckedProperty ||
          e.Property == IsEnabledProperty) {
         
         _customStyle.CollectStyleState();
         _customStyle.ApplyVariableStyleConfig();
         if (e.Property == IsCheckedProperty) {
            CalculateElementsOffset(DesiredSize);
            WaveSpiritAdorner.ShowWaveAdorner(this, WaveType.PillWave);
            _switchKnob.IsCheckedState = IsChecked.HasValue && IsChecked.Value;
         }
      }
      
      if (e.Property == IsLoadingProperty) {
         HandleLoadingState(IsLoading);
      }
      
      if (e.Property == SizeTypeProperty) {
         _customStyle.ApplySizeTypeStyleConfig();
      }
   }

   private bool IsDefaultSize()
   {
      return SizeType == SizeType.Middle || SizeType == SizeType.Large;
   }

   private void HandleLoadingState(bool isLoading)
   {
      if (isLoading) {
         Cursor = new Cursor(StandardCursorType.Arrow);
         SwitchOpacity = _switchDisabledOpacity;
         _switchKnob.NotifyStartLoading();
      } else {
         Cursor = new Cursor(StandardCursorType.Hand);
         SwitchOpacity = 1d;
         _switchKnob.NotifyStopLoading();
      }
   }

   private double HandleSize()
   {
      if (IsDefaultSize()) {
         return _handleSize;
      }
      return _handleSizeSM;
   }

   private double InnerMaxMargin()
   {
      if (IsDefaultSize()) {
         return _innerMaxMargin;
      } 
      return _innerMaxMarginSM;
   }
   
   private double InnerMinMargin()
   {
      if (IsDefaultSize()) {
         return _innerMinMargin;
      }
      return _innerMinMarginSM;
   }

   private double TrackHeight()
   {
      if (IsDefaultSize()) {
         return _trackHeight;
      } 
      return _trackHeightSM;
   }

   private double TrackMinWidth()
   {
      if (IsDefaultSize()) {
         return _trackMinWidth;
      } 
      return _trackMinWidthSM;
   }

   private double IconSize()
   {
      if (IsDefaultSize()) {
         return _trackHeightSM;
      }
      return _trackHeightSM - 4;
   }

   private double PressOffsetRange()
   {
      double value;
      if (IsDefaultSize()) {
         value = _handleSize;
      } else {
         value = _handleSizeSM;
      }

      value /= 3;
      return value;
   }

   private Rect GrooveRect()
   {
      return new Rect(new Point(0, 0), DesiredSize);
   }

   private Rect HandleRect()
   {
      return HandleRect(IsChecked.HasValue && IsChecked.Value, DesiredSize);
   }

   private Rect HandleRect(bool isChecked, Size controlSize)
   {
      double handlePosX;
      double handlePosY;
      double handleSize = HandleSize();
      double offsetX = _trackPadding;
      double offsetY = _trackPadding;
      if (!isChecked) {
         handlePosX = offsetX;
         handlePosY = offsetY;
      } else {
         if (IsPressed) {
            handleSize *= STRETCH_FACTOR;
         }
         handlePosX = controlSize.Width - offsetX - handleSize;
         handlePosY = offsetY;
      }

      return new Rect(handlePosX, handlePosY, handleSize, handleSize);
   }

   private Rect ExtraInfoRect()
   {
      return ExtraInfoRect(IsChecked.HasValue && IsChecked.Value, DesiredSize);
   }

   private Rect ExtraInfoRect(bool isChecked, Size controlSize)
   {
      double innerMinMargin = InnerMinMargin();
      double innerMaxMargin = InnerMaxMargin();
      double yAdjustValue = 0;
      Rect targetRect = new Rect(new Point(0, 0), controlSize);
      if (isChecked) {
         if (OffContent is Control offControl) {
            yAdjustValue = (controlSize.Height - offControl.DesiredSize.Height) / 2;
         }
         targetRect = targetRect.Inflate(new Thickness(-innerMinMargin, -yAdjustValue, innerMaxMargin, 0));
      } else {
         if (OnContent is Control onControl) {
            yAdjustValue = (controlSize.Height - onControl.DesiredSize.Height) / 2;
         }
         targetRect = targetRect.Inflate(new Thickness(-innerMaxMargin, -yAdjustValue, innerMinMargin, 0));
      }

      return targetRect;
   }

   public Rect WaveGeometry()
   {
      return GrooveRect();
   }

   public CornerRadius WaveBorderRadius()
   {
      return new CornerRadius(DesiredSize.Height / 2);
   }

   #endregion
}