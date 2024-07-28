using AtomUI.Media;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Utilities;

namespace AtomUI.Controls;

[PseudoClasses(StdPseudoClass.Vertical, StdPseudoClass.Horizontal)]
public class SliderTrack : Control, IControlCustomStyle
{
   public static readonly StyledProperty<double> MinimumProperty =
      RangeBase.MinimumProperty.AddOwner<SliderTrack>();

   public static readonly StyledProperty<double> MaximumProperty =
      RangeBase.MaximumProperty.AddOwner<SliderTrack>();

   public static readonly StyledProperty<double> ValueProperty =
      RangeBase.ValueProperty.AddOwner<SliderTrack>();

   public static readonly StyledProperty<SliderRangeValue> RangeValueProperty =
      AvaloniaProperty.Register<SliderTrack, SliderRangeValue>(nameof(RangeValue));

   public static readonly StyledProperty<bool> IsRangeModeProperty =
      AvaloniaProperty.Register<SliderTrack, bool>(nameof(IsRangeMode));

   public static readonly StyledProperty<Orientation> OrientationProperty =
      ScrollBar.OrientationProperty.AddOwner<SliderTrack>();
   
   public static readonly StyledProperty<SliderThumb?> StartSliderThumbProperty =
      AvaloniaProperty.Register<SliderTrack, SliderThumb?>(nameof(StartSliderThumb));
   
   public static readonly StyledProperty<SliderThumb?> EndSliderThumbProperty =
      AvaloniaProperty.Register<SliderTrack, SliderThumb?>(nameof(EndSliderThumb));

   public static readonly StyledProperty<bool> IsDirectionReversedProperty =
      AvaloniaProperty.Register<SliderTrack, bool>(nameof(IsDirectionReversed));

   public static readonly StyledProperty<bool> IgnoreThumbDragProperty =
      AvaloniaProperty.Register<SliderTrack, bool>(nameof(IgnoreThumbDrag));

   public static readonly StyledProperty<bool> DeferThumbDragProperty =
      AvaloniaProperty.Register<SliderTrack, bool>(nameof(DeferThumbDrag));
   
   public static readonly StyledProperty<bool> IncludedProperty =
      AvaloniaProperty.Register<SliderTrack, bool>(nameof(Included), true);

   public static readonly StyledProperty<IBrush?> TrackBarBrushProperty =
      AvaloniaProperty.Register<SliderTrack, IBrush?>(nameof(TrackBarBrush));
   
   public static readonly StyledProperty<IBrush?> TrackGrooveBrushProperty =
      AvaloniaProperty.Register<SliderTrack, IBrush?>(nameof(TrackGrooveBrush));

   public static readonly StyledProperty<AvaloniaList<SliderMark>?> MarksProperty =
      AvaloniaProperty.Register<SliderTrack, AvaloniaList<SliderMark>?>(nameof(Marks));

   public static readonly StyledProperty<double> MarkLabelFontSizeProperty =
      TextElement.FontSizeProperty.AddOwner<SliderTrack>();
   
   public static readonly StyledProperty<FontFamily> MarkLabelFontFamilyProperty =
      TextElement.FontFamilyProperty.AddOwner<SliderTrack>();

   internal static readonly StyledProperty<double> SliderTrackSizeProperty =
      AvaloniaProperty.Register<SliderTrack, double>(nameof(SliderTrackSize));

   internal static readonly StyledProperty<double> SliderRailSizeProperty =
      AvaloniaProperty.Register<SliderTrack, double>(nameof(SliderRailSize));
   
   internal static readonly StyledProperty<Thickness> PaddingProperty = 
      Decorator.PaddingProperty.AddOwner<SliderTrack>();
   
   internal static readonly RoutedEvent<PointerPressedEventArgs> TrailPressedEvent =
      RoutedEvent.Register<SliderThumb, PointerPressedEventArgs>(nameof(TrailPressed), RoutingStrategies.Bubble);
   
   internal static readonly RoutedEvent<PointerReleasedEventArgs> TrailReleasedEvent =
      RoutedEvent.Register<SliderThumb, PointerReleasedEventArgs>(nameof(TrailReleased), RoutingStrategies.Bubble);

   private VectorEventArgs? _deferredThumbDrag;
   private Vector _lastDrag;
   private readonly IControlCustomStyle _customStyle;
   private RenderContextData? _renderContextData;
   
   public double Minimum
   {
      get => GetValue(MinimumProperty);
      set => SetValue(MinimumProperty, value);
   }

   public double Maximum
   {
      get => GetValue(MaximumProperty);
      set => SetValue(MaximumProperty, value);
   }

   public double Value
   {
      get => GetValue(ValueProperty);
      set => SetValue(ValueProperty, value);
   }

   public SliderRangeValue RangeValue
   {
      get => GetValue(RangeValueProperty);
      set => SetValue(RangeValueProperty, value);
   }
   
   /// <summary>
   /// 	Dual thumb mode
   /// </summary>
   public bool IsRangeMode
   {
      get => GetValue(IsRangeModeProperty);
      set => SetValue(IsRangeModeProperty, value);
   }
   
   /// <summary>
   /// Gets the value of the <see cref="SliderThumb"/>'s current position. This can differ from <see cref="Value"/> when <see cref="ScrollViewer.IsDeferredScrollingEnabled"/> is true.
   /// </summary>
   private double ThumbValue => Value + (_deferredThumbDrag == null
      ? 0
      : ValueFromDistance(_deferredThumbDrag.Vector.X, _deferredThumbDrag.Vector.Y));
   
   private double ThumbRangeStartValue => RangeValue.StartValue + (_deferredThumbDrag == null
      ? 0
      : ValueFromDistance(_deferredThumbDrag.Vector.X, _deferredThumbDrag.Vector.Y));
   
   private double ThumbRangeEndValue => RangeValue.EndValue + (_deferredThumbDrag == null
      ? 0
      : ValueFromDistance(_deferredThumbDrag.Vector.X, _deferredThumbDrag.Vector.Y));

   public Orientation Orientation
   {
      get => GetValue(OrientationProperty);
      set => SetValue(OrientationProperty, value);
   }
   
   public SliderThumb? StartSliderThumb
   {
      get => GetValue(StartSliderThumbProperty);
      set => SetValue(StartSliderThumbProperty, value);
   }

   public SliderThumb? EndSliderThumb
   {
      get => GetValue(EndSliderThumbProperty);
      set => SetValue(EndSliderThumbProperty, value);
   }
   
   public bool IsDirectionReversed
   {
      get => GetValue(IsDirectionReversedProperty);
      set => SetValue(IsDirectionReversedProperty, value);
   }

   public bool IgnoreThumbDrag
   {
      get => GetValue(IgnoreThumbDragProperty);
      set => SetValue(IgnoreThumbDragProperty, value);
   }

   public bool DeferThumbDrag
   {
      get => GetValue(DeferThumbDragProperty);
      set => SetValue(DeferThumbDragProperty, value);
   }
   
   public bool Included
   {
      get => GetValue(IncludedProperty);
      set => SetValue(IncludedProperty, value);
   }

   public IBrush? TrackBarBrush
   {
      get => GetValue(TrackBarBrushProperty);
      set => SetValue(TrackBarBrushProperty, value);
   }

   public IBrush? TrackGrooveBrush
   {
      get => GetValue(TrackGrooveBrushProperty);
      set => SetValue(TrackGrooveBrushProperty, value);
   }
   
   public AvaloniaList<SliderMark>? Marks
   {
      get => GetValue(MarksProperty);
      set => SetValue(MarksProperty, value);
   }

   public double MarkLabelFontSize
   {
      get => GetValue(MarkLabelFontSizeProperty);
      set => SetValue(MarkLabelFontSizeProperty, value);
   }

   public FontFamily MarkLabelFontFamily
   {
      get => GetValue(MarkLabelFontFamilyProperty);
      set => SetValue(MarkLabelFontSizeProperty, value);
   }
   
   public double SliderTrackSize
   {
      get => GetValue(SliderTrackSizeProperty);
      set => SetValue(SliderTrackSizeProperty, value);
   }

   public double SliderRailSize
   {
      get => GetValue(SliderRailSizeProperty);
      set => SetValue(SliderRailSizeProperty, value);
   }
   
   public Thickness Padding
   {
      get => GetValue(PaddingProperty);
      set => SetValue(PaddingProperty, value);
   }
   
   public event EventHandler<PointerPressedEventArgs>? TrailPressed
   {
      add => AddHandler(TrailPressedEvent, value);
      remove => RemoveHandler(TrailPressedEvent, value);
   }
   
   public event EventHandler<PointerReleasedEventArgs>? TrailReleased
   {
      add => AddHandler(TrailReleasedEvent, value);
      remove => RemoveHandler(TrailReleasedEvent, value);
   }
   
   private double ThumbCenterOffset { get; set; }
   private Point StartThumbCenterOffset { get; set; }
   private Point EndThumbCenterOffset { get; set; }
   private double Density { get; set; }
   private IDisposable? _focusProcessDisposable;
   
   static SliderTrack()
   {
      StartSliderThumbProperty.Changed.AddClassHandler<SliderTrack>((x, e) => x.ThumbChanged(e));
      EndSliderThumbProperty.Changed.AddClassHandler<SliderTrack>((x, e) => x.ThumbChanged(e));
      AffectsArrange<SliderTrack>(IsDirectionReversedProperty, 
                                  MinimumProperty, 
                                  MaximumProperty, 
                                  ValueProperty,
                                  RangeValueProperty,
                                  OrientationProperty,
                                  IsRangeModeProperty);
      AffectsRender<SliderTrack>(TrackBarBrushProperty,
                                 TrackGrooveBrushProperty);
   }

   public SliderTrack()
   {
      _customStyle = this;
      UpdatePseudoClasses(Orientation);
   }
   
   public override void ApplyTemplate()
   {
      base.ApplyTemplate();
      BindUtils.CreateTokenBinding(this, SliderTrack.SliderTrackSizeProperty, SliderResourceKey.SliderTrackSize);
      BindUtils.CreateTokenBinding(this, SliderTrack.SliderRailSizeProperty, SliderResourceKey.RailSize);

      HandleRangeModeChanged();
   }

   private void HandleRangeModeChanged()
   {
      if (IsRangeMode) {
         if (EndSliderThumb is not null) {
            EndSliderThumb.IsVisible = true;
         }

      } else {
         if (EndSliderThumb is not null) {
            EndSliderThumb.IsVisible = false;
         }
      }
   }

   protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnAttachedToVisualTree(e);
      var inputManager = AvaloniaLocator.Current.GetService<IInputManager>()!;
      _focusProcessDisposable = inputManager.Process.Subscribe((args =>
      {
         if (args is RawPointerEventArgs pointerEventArgs) {
            var eventType = pointerEventArgs.Type;
            switch (eventType) {
               case RawPointerEventType.LeftButtonDown:
               case RawPointerEventType.RightButtonDown:
               case RawPointerEventType.MiddleButtonDown:
               case RawPointerEventType.XButton1Down:
               case RawPointerEventType.XButton2Down:
                  HandleGlobalMousePressed(pointerEventArgs.Position);
                  break;
            }
         }
      }));
   }

   private void HandleGlobalMousePressed(Point point)
   {
      var globalOffset = GetGlobalOffset();
      var trailGlobalBounds = new Rect(globalOffset + _renderContextData!.RailRect.Position, _renderContextData.RailRect.Size);
      if (trailGlobalBounds.Contains(point)) {
         // 点击在轨道上，要不设置值，要不本身就在 Thumb 上，所以不需要处理
         return;
      }
      
      if (StartSliderThumb is not null && StartSliderThumb.IsVisible) {
         HandleThumbFocus(StartSliderThumb, point);
      }

      if (EndSliderThumb is not null && EndSliderThumb.IsVisible) {
         HandleThumbFocus(EndSliderThumb, point);
      }
   }

   private bool IsNeedHandlePressedForValue(Point point)
   {
      if ((StartSliderThumb is not null &&
           StartSliderThumb.IsVisible &&
           StartSliderThumb.Bounds.Contains(point)) ||
          EndSliderThumb is not null &&
          EndSliderThumb.IsVisible &&
          EndSliderThumb.Bounds.Contains(point)) {
         return false;
      }

      return true;
   }

   private Point GetGlobalOffset()
   {
      var topLevel = TopLevel.GetTopLevel(this);
      if (topLevel is null) {
         return default;
      }
      return this.TranslatePoint(Bounds.Position, topLevel) ?? default;
   }

   private void HandleThumbFocus(SliderThumb sliderThumb, Point point)
   {
      var topLevel = TopLevel.GetTopLevel(this);
      if (topLevel is null) {
         return;
      }
      var offset = GetGlobalOffset();
      
      var thumbGOffset = offset + sliderThumb.Bounds.Position;
      var thumbGBounds = new Rect(thumbGOffset, sliderThumb.Bounds.Size);
      if (!thumbGBounds.Contains(point) && sliderThumb.IsFocused) {
         topLevel.FocusManager?.ClearFocus();
      }
   }

   protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnDetachedFromVisualTree(e);
      _focusProcessDisposable?.Dispose();
   }

   /// <summary>
   /// Calculates the distance along the <see cref="SliderThumb"/> of a specified point along the
   /// track.
   /// </summary>
   /// <param name="point">The specified point.</param>
   /// <returns>
   /// The distance between the SliderThumb and the specified pt value.
   /// </returns>
   public virtual double ValueFromPoint(Point point)
   {
      double val;

      // Find distance from center of thumb to given point.
      if (Orientation == Orientation.Horizontal) {
         val = ThumbValue + ValueFromDistance(point.X - ThumbCenterOffset, point.Y - (Bounds.Height * 0.5));
      } else {
         val = ThumbValue + ValueFromDistance(point.X - (Bounds.Width * 0.5), point.Y - ThumbCenterOffset);
      }

      return Math.Max(Minimum, Math.Min(Maximum, val));
   }

   /// <summary>
   /// Calculates the change in the <see cref="Value"/> of the <see cref="SliderTrack"/> when the
   /// <see cref="SliderThumb"/> moves.
   /// </summary>
   /// <param name="horizontal">The horizontal displacement of the thumb.</param>
   /// <param name="vertical">The vertical displacement of the thumb.</param>        
   public virtual double ValueFromDistance(double horizontal, double vertical)
   {
      double scale = IsDirectionReversed ? -1 : 1;

      if (Orientation == Orientation.Horizontal) {
         return scale * horizontal * Density;
      } else {
         // Increases in y cause decreases in Sliders value
         return -1 * scale * vertical * Density;
      }
   }

   protected override Size MeasureOverride(Size availableSize)
   {
      var targetWidth = 0d;
      var targetHeight = 0d;
      var markTextSize = CalculateMaxMarkSize();
      if (StartSliderThumb is not null) {
         StartSliderThumb.Measure(availableSize);
      }

      if (EndSliderThumb is not null) {
         EndSliderThumb.Measure(availableSize);
      }
      // TODO 暂时先不加 Margin 试试
      if (Orientation == Orientation.Horizontal) {
         if (!double.IsInfinity(availableSize.Width)) {
            targetWidth = availableSize.Width;
         }
         targetWidth = Math.Max(targetWidth, SliderTrackSize);
         targetHeight = SliderTrackSize + markTextSize.Height;
         
      } else {
         if (!double.IsInfinity(availableSize.Height)) {
            targetHeight = availableSize.Height;
         }
         targetHeight = Math.Max(targetHeight, SliderTrackSize);
         targetWidth = SliderTrackSize + markTextSize.Width;
      }

      targetWidth += Padding.Left + Padding.Right;
      targetHeight += Padding.Top + Padding.Bottom;

      return new Size(targetWidth, targetHeight);
   }

   protected override Size ArrangeOverride(Size arrangeSize)
   {
      var isVertical = Orientation == Orientation.Vertical;

      ComputeDensity(arrangeSize, isVertical);

      // Layout the pieces of track
      var isDirectionReversed = IsDirectionReversed;

      var thumbCenterPoints = CalculateThumbCenterOffset(arrangeSize);
      StartThumbCenterOffset = thumbCenterPoints.Item1;
      EndThumbCenterOffset = thumbCenterPoints.Item2;
      
      if (IsRangeMode) {
         if (StartSliderThumb != null) {
            var offset = StartThumbCenterOffset - new Point(StartSliderThumb.DesiredSize.Width / 2, StartSliderThumb.DesiredSize.Height / 2);
            var bounds = new Rect(offset, StartSliderThumb.DesiredSize);
            var adjust = CalculateThumbAdjustment(StartSliderThumb, bounds);
            StartSliderThumb.Arrange(bounds);
            StartSliderThumb.AdjustDrag(adjust);
         }
         
         if (EndSliderThumb != null) {
            var offset = EndThumbCenterOffset - new Point(EndSliderThumb.DesiredSize.Width / 2, EndSliderThumb.DesiredSize.Height / 2);
            var bounds = new Rect(offset, EndSliderThumb.DesiredSize);
            var adjust = CalculateThumbAdjustment(EndSliderThumb, bounds);
            EndSliderThumb.Arrange(bounds);
            EndSliderThumb.AdjustDrag(adjust);
         }
      } else {
         if (StartSliderThumb != null) {
            var offset = EndThumbCenterOffset - new Point(StartSliderThumb.DesiredSize.Width / 2, StartSliderThumb.DesiredSize.Height / 2);
            var bounds = new Rect(offset, StartSliderThumb.DesiredSize);
            var adjust = CalculateThumbAdjustment(StartSliderThumb, bounds);
            StartSliderThumb.Arrange(bounds);
            StartSliderThumb.AdjustDrag(adjust);
         }
      }

      _lastDrag = default;
      return arrangeSize;
   }

   private (Point, Point) CalculateThumbCenterOffset(Size arrangeSize)
   {
      CalculateThumbValuePivotOffset(arrangeSize, 
                                     Orientation == Orientation.Vertical, 
                                     out var startThumbPivotOffset,
                                     out var endThumbPivotOffset);
      if (Orientation == Orientation.Horizontal) {
         var offsetY = arrangeSize.Height / 2;
         return (new Point(startThumbPivotOffset, offsetY), new Point(endThumbPivotOffset, offsetY));
      }

      var offsetX = arrangeSize.Width / 2;
      return (new Point(offsetX, startThumbPivotOffset), new Point(offsetX, endThumbPivotOffset));
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
   {
      base.OnPropertyChanged(change);

      if (change.Property == OrientationProperty) {
         UpdatePseudoClasses(change.GetNewValue<Orientation>());
      } else if (change.Property == DeferThumbDragProperty) {
         if (!change.GetNewValue<bool>()) {
            ApplyDeferredThumbDrag();
         }
      } else if (change.Property == IsRangeModeProperty) {
         HandleRangeModeChanged();
      }
   }

   private Vector CalculateThumbAdjustment(SliderThumb thumb, Rect newThumbBounds)
   {
      var thumbDelta = newThumbBounds.Position - thumb.Bounds.Position;
      return _lastDrag - thumbDelta;
   }

   private static void CoerceLength(ref double componentLength, double trackLength)
   {
      if (componentLength < 0) {
         componentLength = 0.0;
      } else if (componentLength > trackLength || double.IsNaN(componentLength)) {
         componentLength = trackLength;
      }
   }

   private void CalculateThumbValuePivotOffset(Size arrangeSize, 
                                               bool isVertical, 
                                               out double startThumbOffset,
                                               out double endThumbOffset)
   {
      double min = Minimum;
      double range = Math.Max(0.0, Maximum - min);
      var thumbSize = StartSliderThumb!.Width;
      var totalSize = arrangeSize.Width;
      if (isVertical) {
         totalSize = arrangeSize.Height;
      }

      totalSize -= thumbSize;
      if (IsRangeMode) {
         var startRatio = Math.Min(range, ThumbRangeStartValue - min) / range;
         var endRatio = Math.Min(range, ThumbRangeEndValue - min) / range;

         startThumbOffset = totalSize * startRatio;
         endThumbOffset = totalSize * endRatio;

      } else {
         startThumbOffset = 0d;
         var ratio = Math.Min(range, ThumbValue - min) / range;
         endThumbOffset = totalSize * ratio;
      }

      startThumbOffset += thumbSize / 2;
      endThumbOffset += thumbSize / 2;
   }

   private void ComputeDensity(Size arrangeSize, bool isVertical)
   {
      double min = Minimum;
      double range = Math.Max(0.0, Maximum - min);
      
      double trackLength;
      // Compute thumb size
      if (isVertical) {
         trackLength = arrangeSize.Height;
      } else {
         trackLength = arrangeSize.Width;
      }
      double remainingTrackLength = trackLength;
      Density = range / remainingTrackLength;
   }

   private void ThumbChanged(AvaloniaPropertyChangedEventArgs e)
   {
      var oldThumb = (SliderThumb?)e.OldValue;
      var newThumb = (SliderThumb?)e.NewValue;

      if (oldThumb != null) {
         oldThumb.DragDelta -= ThumbDragged;
         oldThumb.DragCompleted -= ThumbDragCompleted;
         LogicalChildren.Remove(oldThumb);
         VisualChildren.Remove(oldThumb);
      }

      if (newThumb != null) {
         newThumb.DragDelta += ThumbDragged;
         newThumb.DragCompleted += ThumbDragCompleted;
         LogicalChildren.Add(newThumb);
         VisualChildren.Add(newThumb);
      }
   }

   private void ThumbDragged(object? sender, VectorEventArgs e)
   {
      if (IgnoreThumbDrag) return;

      if (DeferThumbDrag) {
         _deferredThumbDrag = e;
         InvalidateArrange();
      } else {
         ApplyThumbDrag(e);
      }
   }

   private void ApplyThumbDrag(VectorEventArgs e)
   {
      var delta = ValueFromDistance(e.Vector.X, e.Vector.Y);
      var factor = e.Vector / delta;
      var oldValue = Value;

      SetCurrentValue(ValueProperty, MathUtilities.Clamp(
                         Value + delta,
                         Minimum,
                         Maximum));

      // Record the part of the drag that actually had effect as the last drag delta.
      // Due to clamping, we need to compare the two values instead of using the drag delta.
      _lastDrag = (Value - oldValue) * factor;
   }

   private void ThumbDragCompleted(object? sender, EventArgs e) => ApplyDeferredThumbDrag();

   private void ApplyDeferredThumbDrag()
   {
      if (_deferredThumbDrag != null) {
         ApplyThumbDrag(_deferredThumbDrag);
         _deferredThumbDrag = null;
      }
   }

   private void UpdatePseudoClasses(Orientation o)
   {
      PseudoClasses.Set(StdPseudoClass.Vertical, o == Orientation.Vertical);
      PseudoClasses.Set(StdPseudoClass.Horizontal, o == Orientation.Horizontal);
   }

   private Size CalculateMaxMarkSize()
   {
      var targetWidth = 0d;
      var targetHeight = 0d;
      if (Marks is not null) {
         foreach (var mark in Marks) {
            var markTextSize = TextUtils.CalculateTextSize(mark.Label, 
                                                           MarkLabelFontSize,
                                                           MarkLabelFontFamily,
                                                           mark.LabelFontStyle,
                                                           mark.LabelFontWeight);
            targetWidth = Math.Max(targetWidth, markTextSize.Width);
            targetHeight = Math.Max(targetHeight, markTextSize.Height);
         }
      }
      return new Size(targetWidth, targetHeight);
   }

   void IControlCustomStyle.PrepareRenderInfo()
   {
      _renderContextData = new RenderContextData();

      var startThumbPivotOffset = 0d;
      var endThumbPivotOffset = 0d;
      CalculateThumbValuePivotOffset(Bounds.Size, Orientation == Orientation.Vertical, 
                                    out startThumbPivotOffset,
                                    out endThumbPivotOffset);
      var thumbSize = StartSliderThumb!.DesiredSize.Width;
      if (Orientation == Orientation.Horizontal) {
         {
            // 计算轨道位置
            var offsetX = thumbSize / 2;
            var offsetY = (Bounds.Height - SliderRailSize) / 2;
            _renderContextData.RailRect = new Rect(new Point(offsetX, offsetY),
                                                   new Size(Bounds.Width - thumbSize, SliderRailSize));
         }
         {
            // 计算 range bar rect
            var offsetY = (Bounds.Height - SliderRailSize) / 2;
            _renderContextData.TrackRangeRect = new Rect(new Point(startThumbPivotOffset, offsetY), 
                                                         new Size(endThumbPivotOffset - startThumbPivotOffset,SliderRailSize));
         }
      } else {
         {
            // 计算轨道位置
            var offsetX = (Bounds.Width - SliderRailSize) / 2;
            var offsetY = thumbSize;
            _renderContextData.RailRect = new Rect(new Point(offsetX, offsetY), new Size(SliderRailSize, Bounds.Height));
         }
         {
            // 计算 range bar rect
            var offsetX = (Bounds.Width - SliderRailSize) / 2;
            _renderContextData.TrackRangeRect = new Rect(new Point(offsetX, startThumbPivotOffset), 
                                                         new Size(SliderRailSize, endThumbPivotOffset - startThumbPivotOffset));
         }
      }
   }

   public override void Render(DrawingContext context)
   {
      _customStyle.PrepareRenderInfo();
      DrawGroove(context);
      DrawTrackBar(context);
      DrawMark(context);
   }

   private void DrawGroove(DrawingContext context)
   {
      context.DrawPilledRect(TrackGrooveBrush, null, _renderContextData!.RailRect);
   }
   
   private void DrawMark(DrawingContext context)
   {
      
   }
   
   private void DrawTrackBar(DrawingContext context)
   {
      context.DrawPilledRect(TrackBarBrush, null, _renderContextData!.TrackRangeRect);
   }
   
   // 跟渲染相关的数据
   private class RenderContextData
   {
      public Rect RailRect { get; set; }
      public Rect TrackRangeRect { get; set; }
   }
}