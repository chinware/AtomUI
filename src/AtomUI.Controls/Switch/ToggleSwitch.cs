using AtomUI.Controls.Switch;
using AtomUI.Data;
using AtomUI.Media;
using AtomUI.TokenSystem;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Rendering;

namespace AtomUI.Controls;

public partial class ToggleSwitch : ToggleButton, ITokenIdProvider, ISizeTypeAware,
                                    ICustomHitTest
{
   private bool _initialized = false;
   private bool _transitionInitialized = false;
   string ITokenIdProvider.TokenId => ToggleSwitchToken.ID;
   private const double STRETCH_FACTOR = 1.3d;

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
      _controlTokenBinder = new ControlTokenBinder(this);
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
}