using System.Globalization;
using AtomUI.Animations;
using AtomUI.Controls;
using AtomUI.Media;
using AtomUI.Reflection;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[PseudoClasses(StdPseudoClass.Vertical, StdPseudoClass.Horizontal)]
public class SliderTrack : TemplatedControl
{
    #region 公共属性定义

    public static readonly StyledProperty<double> MinimumProperty =
        RangeBase.MinimumProperty.AddOwner<SliderTrack>();

    public static readonly StyledProperty<double> MaximumProperty =
        RangeBase.MaximumProperty.AddOwner<SliderTrack>();

    public static readonly StyledProperty<double> ValueProperty =
        RangeBase.ValueProperty.AddOwner<SliderTrack>();

    public static readonly StyledProperty<IReadOnlyList<double>?> RangeValuesProperty =
        AvaloniaProperty.Register<SliderTrack, IReadOnlyList<double>?>(
            nameof(RangeValues),
            defaultBindingMode: BindingMode.TwoWay,
            enableDataValidation: true,
            coerce: CoerceRangeValues);

    public static readonly StyledProperty<IReadOnlyList<bool>?> DisabledHandlesProperty =
        AvaloniaProperty.Register<SliderTrack, IReadOnlyList<bool>?>(nameof(DisabledHandles));

    public static readonly StyledProperty<bool> IsDraggableTrackProperty =
        AvaloniaProperty.Register<SliderTrack, bool>(nameof(IsDraggableTrack));

    public static readonly StyledProperty<bool> IsRangeModeProperty =
        AvaloniaProperty.Register<SliderTrack, bool>(nameof(IsRangeMode));

    public static readonly StyledProperty<Orientation> OrientationProperty =
        ScrollBar.OrientationProperty.AddOwner<SliderTrack>();

    public static readonly StyledProperty<bool> IsDirectionReversedProperty =
        AvaloniaProperty.Register<SliderTrack, bool>(nameof(IsDirectionReversed));

    public static readonly StyledProperty<bool> IgnoreThumbDragProperty =
        AvaloniaProperty.Register<SliderTrack, bool>(nameof(IgnoreThumbDrag));

    public static readonly StyledProperty<bool> DeferThumbDragProperty =
        AvaloniaProperty.Register<SliderTrack, bool>(nameof(DeferThumbDrag));

    public static readonly StyledProperty<bool> IsIncludedProperty =
        AvaloniaProperty.Register<SliderTrack, bool>(nameof(IsIncluded), true);

    public static readonly StyledProperty<IBrush?> TrackBarBrushProperty =
        AvaloniaProperty.Register<SliderTrack, IBrush?>(nameof(TrackBarBrush));

    public static readonly StyledProperty<IBrush?> TracksBrushProperty =
        AvaloniaProperty.Register<SliderTrack, IBrush?>(nameof(TracksBrush));

    public static readonly StyledProperty<IBrush?> TrackGrooveBrushProperty =
        AvaloniaProperty.Register<SliderTrack, IBrush?>(nameof(TrackGrooveBrush));

    public static readonly StyledProperty<List<SliderMark>?> MarksProperty =
        AvaloniaProperty.Register<SliderTrack, List<SliderMark>?>(nameof(Marks));

    public static readonly StyledProperty<double> MarkLabelFontSizeProperty =
        TextElement.FontSizeProperty.AddOwner<SliderTrack>();

    public static readonly StyledProperty<FontFamily> MarkLabelFontFamilyProperty =
        TextElement.FontFamilyProperty.AddOwner<SliderTrack>();

    public static readonly StyledProperty<IBrush?> MarkLabelBrushProperty =
        AvaloniaProperty.Register<SliderTrack, IBrush?>(nameof(MarkLabelBrush));

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

    public IReadOnlyList<double>? RangeValues
    {
        get => GetValue(RangeValuesProperty);
        set => SetValue(RangeValuesProperty, value);
    }

    public IReadOnlyList<bool>? DisabledHandles
    {
        get => GetValue(DisabledHandlesProperty);
        set => SetValue(DisabledHandlesProperty, value);
    }

    public bool IsDraggableTrack
    {
        get => GetValue(IsDraggableTrackProperty);
        set => SetValue(IsDraggableTrackProperty, value);
    }

    public bool IsRangeMode
    {
        get => GetValue(IsRangeModeProperty);
        set => SetValue(IsRangeModeProperty, value);
    }

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
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

    public bool IsIncluded
    {
        get => GetValue(IsIncludedProperty);
        set => SetValue(IsIncludedProperty, value);
    }

    public IBrush? TrackBarBrush
    {
        get => GetValue(TrackBarBrushProperty);
        set => SetValue(TrackBarBrushProperty, value);
    }

    public IBrush? TracksBrush
    {
        get => GetValue(TracksBrushProperty);
        set => SetValue(TracksBrushProperty, value);
    }

    public IBrush? TrackGrooveBrush
    {
        get => GetValue(TrackGrooveBrushProperty);
        set => SetValue(TrackGrooveBrushProperty, value);
    }

    public List<SliderMark>? Marks
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
        set => SetValue(MarkLabelFontFamilyProperty, value);
    }

    public IBrush? MarkLabelBrush
    {
        get => GetValue(MarkLabelBrushProperty);
        set => SetValue(MarkLabelBrushProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<double> SliderTrackSizeProperty =
        AvaloniaProperty.Register<SliderTrack, double>(nameof(SliderTrackSize));

    internal static readonly StyledProperty<double> SliderRailSizeProperty =
        AvaloniaProperty.Register<SliderTrack, double>(nameof(SliderRailSize));

    internal static readonly StyledProperty<double> SliderMarkSizeProperty =
        AvaloniaProperty.Register<SliderTrack, double>(nameof(SliderMarkSize));

    internal static readonly StyledProperty<IBrush?> MarkBorderBrushProperty =
        AvaloniaProperty.Register<SliderTrack, IBrush?>(nameof(MarkBorderBrush));

    internal static readonly StyledProperty<IBrush?> MarkBorderActiveBrushProperty =
        AvaloniaProperty.Register<SliderTrack, IBrush?>(nameof(MarkBorderActiveBrush));

    internal static readonly StyledProperty<IBrush?> MarkBackgroundBrushProperty =
        AvaloniaProperty.Register<SliderTrack, IBrush?>(nameof(MarkBackgroundBrush));

    internal static readonly StyledProperty<Thickness> MarkBorderThicknessProperty =
        AvaloniaProperty.Register<SliderTrack, Thickness>(nameof(MarkBorderThickness));

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<SliderTrack>();

    internal double SliderTrackSize
    {
        get => GetValue(SliderTrackSizeProperty);
        set => SetValue(SliderTrackSizeProperty, value);
    }

    internal double SliderRailSize
    {
        get => GetValue(SliderRailSizeProperty);
        set => SetValue(SliderRailSizeProperty, value);
    }

    internal double SliderMarkSize
    {
        get => GetValue(SliderMarkSizeProperty);
        set => SetValue(SliderMarkSizeProperty, value);
    }

    internal IBrush? MarkBorderBrush
    {
        get => GetValue(MarkBorderBrushProperty);
        set => SetValue(MarkBorderBrushProperty, value);
    }

    internal IBrush? MarkBorderActiveBrush
    {
        get => GetValue(MarkBorderActiveBrushProperty);
        set => SetValue(MarkBorderActiveBrushProperty, value);
    }

    internal IBrush? MarkBackgroundBrush
    {
        get => GetValue(MarkBackgroundBrushProperty);
        set => SetValue(MarkBackgroundBrushProperty, value);
    }

    internal Thickness MarkBorderThickness
    {
        get => GetValue(MarkBorderThicknessProperty);
        set => SetValue(MarkBorderThicknessProperty, value);
    }

    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    #endregion

    #region 内部协作 API

    internal IReadOnlyList<SliderThumb> Thumbs => _thumbs;

    internal event EventHandler? ThumbsChanged;

    internal IReadOnlyList<double> EffectiveRangeValues => IsRangeMode
        ? RangeValues ?? [Minimum, Minimum]
        : [Value];

    internal SliderThumb? FocusedThumb => _thumbs.FirstOrDefault(thumb => thumb.IsFocused);

    internal double FocusedThumbValue => FocusedThumb is { } thumb ? GetThumbValue(thumb.HandleIndex) : Value;

    internal double GetThumbValue(int index)
    {
        var values = EffectiveRangeValues;
        return index >= 0 && index < values.Count ? values[index] : Value;
    }

    internal double ValueFromPoint(Point point)
    {
        var railRect = GetRailRect(Bounds.Size);
        double ratio;
        if (Orientation == Orientation.Horizontal)
        {
            ratio = railRect.Width <= 0 ? 0 : (point.X - railRect.X) / railRect.Width;
        }
        else
        {
            ratio = railRect.Height <= 0 ? 0 : 1 - (point.Y - railRect.Y) / railRect.Height;
        }

        if (IsDirectionReversed)
        {
            ratio = 1 - ratio;
        }

        return SliderRangeMath.RatioToValue(ratio, Minimum, Maximum);
    }

    public virtual double ValueFromDistance(double horizontal, double vertical)
    {
        var railRect = GetRailRect(Bounds.Size);
        var range = Maximum - Minimum;
        if (Orientation == Orientation.Horizontal)
        {
            var scale = IsDirectionReversed ? -1 : 1;
            return railRect.Width <= 0 ? 0 : scale * horizontal / railRect.Width * range;
        }

        var verticalScale = IsDirectionReversed ? 1 : -1;
        return railRect.Height <= 0 ? 0 : verticalScale * vertical / railRect.Height * range;
    }

    internal SliderMark? GetMarkForPosition(Point point)
    {
        if (Marks is not null && _renderContextData?.MarkTextRects is not null)
        {
            var entries = _renderContextData.MarkTextRects;
            for (var i = 0; i < entries.Count; i++)
            {
                if (entries[i].Item1.Contains(point))
                {
                    return Marks[i];
                }
            }
        }

        return null;
    }

    internal bool CanDragRangeTrackAt(Point point)
    {
        var values = EffectiveRangeValues;
        return IsRangeMode &&
               IsDraggableTrack &&
               values.Count >= 2 &&
               !SliderRangeMath.HasDisabledHandle(values.Count, DisabledHandles) &&
               _thumbs.All(thumb => !thumb.Bounds.Contains(point)) &&
               _renderContextData?.TrackRangeRect.Contains(point) == true;
    }

    #endregion

    private readonly List<SliderThumb> _thumbs = [];
    private VectorEventArgs? _deferredThumbDrag;
    private Vector _lastDrag;
    private RenderContextData? _renderContextData;
    private IDisposable? _focusProcessDisposable;
    private Size _markLabelSize;
    private IPen? _markBorderPen;
    private IPen? _markBorderActivePen;

    static SliderTrack()
    {
        AffectsMeasure<SliderTrack>(MarksProperty,
            MarkLabelFontSizeProperty,
            MarkLabelFontFamilyProperty,
            RangeValuesProperty,
            IsRangeModeProperty);
        AffectsArrange<SliderTrack>(IsDirectionReversedProperty,
            MinimumProperty,
            MaximumProperty,
            ValueProperty,
            RangeValuesProperty,
            DisabledHandlesProperty,
            OrientationProperty,
            IsRangeModeProperty);
        AffectsRender<SliderTrack>(TrackBarBrushProperty,
            TracksBrushProperty,
            TrackGrooveBrushProperty,
            IsIncludedProperty,
            MarkBorderBrushProperty,
            MarkLabelBrushProperty,
            ValueProperty,
            RangeValuesProperty,
            IsRangeModeProperty);
    }

    public SliderTrack()
    {
        UpdatePseudoClasses(Orientation);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        var inputManager = AvaloniaLocator.Current.GetService<IInputManager>()!;
        _focusProcessDisposable = inputManager.Process.Subscribe(args =>
        {
            if (args is RawPointerEventArgs pointerEventArgs)
            {
                var eventType = pointerEventArgs.Type;
                switch (eventType)
                {
                    case RawPointerEventType.LeftButtonDown:
                    case RawPointerEventType.RightButtonDown:
                    case RawPointerEventType.MiddleButtonDown:
                    case RawPointerEventType.XButton1Down:
                    case RawPointerEventType.XButton2Down:
                        HandleGlobalMousePressed(pointerEventArgs.Position);
                        break;
                }
            }
        });
        EnsureThumbs();
        CalculateMaxMarkSize();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _focusProcessDisposable?.Dispose();
        _focusProcessDisposable = null;
        ClearThumbs();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        EnsureThumbs();
        foreach (var thumb in _thumbs)
        {
            thumb.Measure(availableSize);
        }

        var targetWidth  = 0d;
        var targetHeight = 0d;
        if (Orientation == Orientation.Horizontal)
        {
            targetWidth  = Math.Max(0, SliderTrackSize);
            targetHeight = SliderTrackSize + _markLabelSize.Height;
        }
        else
        {
            targetHeight = Math.Max(0, SliderTrackSize);
            targetWidth  = SliderTrackSize + _markLabelSize.Width;
        }

        targetWidth  += Padding.Left + Padding.Right;
        targetHeight += Padding.Top + Padding.Bottom;

        return new Size(targetWidth, targetHeight);
    }

    protected override Size ArrangeOverride(Size arrangeSize)
    {
        EnsureThumbs();
        var values = EffectiveRangeValues;

        for (var i = 0; i < _thumbs.Count; i++)
        {
            var thumb = _thumbs[i];
            var center = ValueToCenterPoint(arrangeSize, values[i]);
            var offset = center - new Point(thumb.DesiredSize.Width / 2, thumb.DesiredSize.Height / 2);

            var bounds = new Rect(offset, thumb.DesiredSize);
            var adjust = CalculateThumbAdjustment(thumb, bounds);
            thumb.Arrange(bounds);
            thumb.AdjustDrag(adjust);
        }

        _lastDrag = default;
        return arrangeSize;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == OrientationProperty)
        {
            UpdatePseudoClasses(change.GetNewValue<Orientation>());
        }
        else if (change.Property == DeferThumbDragProperty)
        {
            if (!change.GetNewValue<bool>())
            {
                ApplyDeferredThumbDrag();
            }
        }
        else if (change.Property == IsMotionEnabledProperty)
        {
            var isMotionEnabled = change.GetNewValue<bool>();
            foreach (var thumb in _thumbs)
            {
                thumb.SetCurrentValue(SliderThumb.IsMotionEnabledProperty, isMotionEnabled);
            }
        }

        if (change.Property == RangeValuesProperty ||
            change.Property == IsRangeModeProperty ||
            change.Property == DisabledHandlesProperty)
        {
            EnsureThumbs();
        }

        if (IsMarkTextProperty(change.Property))
        {
            RefreshMarkTextMetrics();
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.DisableTransitions();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        Dispatcher.Post(this.EnableTransitions);
    }

    public override void Render(DrawingContext context)
    {
        PrepareRenderInfo();
        DrawGroove(context);
        DrawTrackBars(context);
        DrawMark(context);
    }

    private static IReadOnlyList<double>? CoerceRangeValues(AvaloniaObject sender, IReadOnlyList<double>? values)
    {
        var normalized = SliderRangeMath.NormalizeRangeValues(
            values,
            sender.GetValue(MinimumProperty),
            sender.GetValue(MaximumProperty));
        return normalized.Count == 0 ? sender.GetValue(RangeValuesProperty) : normalized;
    }

    private void EnsureThumbs()
    {
        var handleCount = IsRangeMode ? EffectiveRangeValues.Count : 1;
        while (_thumbs.Count < handleCount)
        {
            AddThumb(_thumbs.Count);
        }

        while (_thumbs.Count > handleCount)
        {
            RemoveThumb(_thumbs[^1]);
        }

        for (var i = 0; i < _thumbs.Count; i++)
        {
            var thumb = _thumbs[i];
            thumb.HandleIndex = i;
            thumb.IsEnabled = IsEnabled && !SliderRangeMath.IsHandleDisabled(DisabledHandles, i);
        }
    }

    private void AddThumb(int index)
    {
        var thumb = new SliderThumb
        {
            HandleIndex = index,
            IsMotionEnabled = IsMotionEnabled
        };
        thumb.SetTemplatedParent(TemplatedParent ?? this);
        ToolTip.SetShowDelay(thumb, 20);
        thumb.DragDelta += ThumbDragged;
        thumb.DragCompleted += ThumbDragCompleted;
        _thumbs.Add(thumb);
        LogicalChildren.Add(thumb);
        VisualChildren.Add(thumb);
        ThumbsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void RemoveThumb(SliderThumb thumb)
    {
        thumb.DragDelta -= ThumbDragged;
        thumb.DragCompleted -= ThumbDragCompleted;
        LogicalChildren.Remove(thumb);
        VisualChildren.Remove(thumb);
        thumb.SetTemplatedParent(null);
        _thumbs.Remove(thumb);
        ThumbsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void ClearThumbs()
    {
        foreach (var thumb in _thumbs.ToArray())
        {
            RemoveThumb(thumb);
        }
    }

    private void HandleGlobalMousePressed(Point point)
    {
        if (_renderContextData is null)
        {
            return;
        }

        var globalOffset = GetGlobalOffset();
        var trailGlobalBounds = new Rect(globalOffset + _renderContextData.RailRect.Position,
            _renderContextData.RailRect.Size);
        if (trailGlobalBounds.Contains(point))
        {
            return;
        }

        foreach (var thumb in _thumbs)
        {
            HandleThumbFocus(thumb, point);
        }
    }

    private Point GetGlobalOffset()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null)
        {
            return default;
        }

        return this.TranslatePoint(Bounds.Position, topLevel) ?? default;
    }

    private void HandleThumbFocus(SliderThumb sliderThumb, Point point)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null)
        {
            return;
        }

        var offset = GetGlobalOffset();
        var thumbGOffset = offset + sliderThumb.Bounds.Position;
        var thumbGBounds = new Rect(thumbGOffset, sliderThumb.Bounds.Size);
        if (!thumbGBounds.Contains(point) && sliderThumb.IsFocused)
        {
            topLevel.FocusManager?.Focus(null);
        }
    }

    private Vector CalculateThumbAdjustment(SliderThumb thumb, Rect newThumbBounds)
    {
        var thumbDelta = newThumbBounds.Position - thumb.Bounds.Position;
        return _lastDrag - thumbDelta;
    }

    private Point ValueToCenterPoint(Size arrangeSize, double value)
    {
        var railRect = GetRailRect(arrangeSize);
        var ratio = SliderRangeMath.ValueToRatio(value, Minimum, Maximum);
        if (IsDirectionReversed)
        {
            ratio = 1 - ratio;
        }

        if (Orientation == Orientation.Horizontal)
        {
            return new Point(railRect.X + railRect.Width * ratio, railRect.Center.Y);
        }

        return new Point(railRect.Center.X, railRect.Y + railRect.Height * (1 - ratio));
    }

    private Rect GetRailRect(Size size)
    {
        var thumbSize = GetThumbSize();
        if (Orientation == Orientation.Horizontal)
        {
            var offsetX = thumbSize / 2;
            var offsetY = Marks?.Count > 0
                ? Math.Max(Padding.Top, (thumbSize - SliderRailSize) / 2)
                : (size.Height - SliderRailSize) / 2;
            return new Rect(new Point(offsetX, offsetY),
                new Size(Math.Max(0, size.Width - thumbSize), SliderRailSize));
        }

        var offsetXVertical = Marks?.Count > 0
            ? Math.Max(Padding.Left, (thumbSize - SliderRailSize) / 2)
            : (size.Width - SliderRailSize) / 2;
        var offsetYVertical = thumbSize / 2;
        return new Rect(new Point(offsetXVertical, offsetYVertical),
            new Size(SliderRailSize, Math.Max(0, size.Height - thumbSize)));
    }

    private double GetThumbSize()
    {
        if (_thumbs.Count > 0)
        {
            var desired = Orientation == Orientation.Horizontal
                ? _thumbs[0].DesiredSize.Width
                : _thumbs[0].DesiredSize.Height;
            if (desired > 0)
            {
                return desired;
            }

            var explicitSize = Orientation == Orientation.Horizontal ? _thumbs[0].Width : _thumbs[0].Height;
            if (explicitSize > 0 && !double.IsNaN(explicitSize))
            {
                return explicitSize;
            }
        }

        return SliderTrackSize;
    }

    private void ThumbDragged(object? sender, VectorEventArgs e)
    {
        if (IgnoreThumbDrag || sender is not SliderThumb thumb || !thumb.IsEnabled)
        {
            return;
        }

        if (DeferThumbDrag)
        {
            _deferredThumbDrag = e;
            InvalidateArrange();
        }
        else
        {
            ApplyThumbDrag(thumb, e);
        }
    }

    private void ApplyThumbDrag(SliderThumb thumb, VectorEventArgs e)
    {
        var delta = ValueFromDistance(e.Vector.X, e.Vector.Y);
        if (delta == 0)
        {
            return;
        }

        var factor = e.Vector / delta;
        if (!IsRangeMode)
        {
            var oldValue = Value;
            SetCurrentValue(ValueProperty, Math.Clamp(Value + delta, Minimum, Maximum));
            _lastDrag = (Value - oldValue) * factor;
            return;
        }

        var values = EffectiveRangeValues;
        var oldHandleValue = values[thumb.HandleIndex];
        var updated = SliderRangeMath.MoveHandle(
            values,
            DisabledHandles,
            thumb.HandleIndex,
            oldHandleValue + delta,
            Minimum,
            Maximum);
        SetCurrentValue(RangeValuesProperty, updated);
        _lastDrag = (updated[thumb.HandleIndex] - oldHandleValue) * factor;
    }

    private void ThumbDragCompleted(object? sender, EventArgs e)
    {
        ApplyDeferredThumbDrag();
    }

    private void ApplyDeferredThumbDrag()
    {
        if (_deferredThumbDrag != null && FocusedThumb is { } thumb)
        {
            ApplyThumbDrag(thumb, _deferredThumbDrag);
            _deferredThumbDrag = null;
        }
    }

    private void UpdatePseudoClasses(Orientation o)
    {
        PseudoClasses.Set(StdPseudoClass.Vertical, o == Orientation.Vertical);
        PseudoClasses.Set(StdPseudoClass.Horizontal, o == Orientation.Horizontal);
    }

    private void CalculateMaxMarkSize(bool force = false)
    {
        if (_markLabelSize != default && !force)
        {
            return;
        }

        var targetWidth = 0d;
        var targetHeight = 0d;
        if (Marks is not null)
        {
            foreach (var mark in Marks)
            {
                var markTextSize = TextUtils.CalculateTextSize(mark.Label,
                    MarkLabelFontSize,
                    MarkLabelFontFamily,
                    mark.LabelFontStyle,
                    mark.LabelFontWeight);
                mark.LabelSize = markTextSize;
                targetWidth = Math.Max(targetWidth, markTextSize.Width);
                targetHeight = Math.Max(targetHeight, markTextSize.Height);

                var typeface = new Typeface(MarkLabelFontFamily, mark.LabelFontStyle, mark.LabelFontWeight);
                var formattedText = new FormattedText(mark.Label, CultureInfo.CurrentUICulture,
                    GetFlowDirection(this),
                    typeface,
                    1,
                    mark.LabelBrush is not null && IsEnabled ? mark.LabelBrush : MarkLabelBrush);
                formattedText.SetFontSize(MarkLabelFontSize);
                formattedText.TextAlignment = TextAlignment.Left;
                mark.FormattedText = formattedText;
            }
        }

        _markLabelSize = new Size(targetWidth, targetHeight);
    }

    private static bool IsMarkTextProperty(AvaloniaProperty property)
    {
        return property == IsEnabledProperty ||
               property == MarksProperty ||
               property == MarkLabelFontSizeProperty ||
               property == MarkLabelFontFamilyProperty ||
               property == MarkLabelBrushProperty;
    }

    private void RefreshMarkTextMetrics()
    {
        CalculateMaxMarkSize(true);
        InvalidateMeasure();
        InvalidateVisual();
    }

    private void PrepareRenderInfo()
    {
        var railRect = GetRailRect(Bounds.Size);
        var values = EffectiveRangeValues;
        _renderContextData = new RenderContextData
        {
            RailRect = railRect,
            SegmentRects = CreateSegmentRects(railRect, values),
            TrackRangeRect = CreateTrackRangeRect(railRect, values)
        };

        if (Marks?.Count > 0)
        {
            PrepareMarkRenderInfo(railRect);
        }
    }

    private List<Rect> CreateSegmentRects(Rect railRect, IReadOnlyList<double> values)
    {
        var rects = new List<Rect>();
        if (!IsRangeMode || values.Count < 2)
        {
            var startRatio = SliderRangeMath.ValueToRatio(Minimum, Minimum, Maximum);
            var endRatio = SliderRangeMath.ValueToRatio(Value, Minimum, Maximum);
            if (IsDirectionReversed)
            {
                startRatio = 1 - startRatio;
                endRatio = 1 - endRatio;
            }

            rects.Add(SliderRangeMath.CreateSegmentRect(railRect, Orientation, startRatio, endRatio));
            return rects;
        }

        for (var i = 0; i < values.Count - 1; i++)
        {
            var startRatio = SliderRangeMath.ValueToRatio(values[i], Minimum, Maximum);
            var endRatio = SliderRangeMath.ValueToRatio(values[i + 1], Minimum, Maximum);
            if (IsDirectionReversed)
            {
                startRatio = 1 - startRatio;
                endRatio = 1 - endRatio;
            }

            rects.Add(SliderRangeMath.CreateSegmentRect(railRect, Orientation, startRatio, endRatio));
        }

        return rects;
    }

    private Rect CreateTrackRangeRect(Rect railRect, IReadOnlyList<double> values)
    {
        if (!IsRangeMode || values.Count == 0)
        {
            return default;
        }

        var startRatio = SliderRangeMath.ValueToRatio(values[0], Minimum, Maximum);
        var endRatio = SliderRangeMath.ValueToRatio(values[^1], Minimum, Maximum);
        if (IsDirectionReversed)
        {
            startRatio = 1 - startRatio;
            endRatio = 1 - endRatio;
        }

        return SliderRangeMath.CreateSegmentRect(railRect, Orientation, startRatio, endRatio);
    }

    private void PrepareMarkRenderInfo(Rect railRect)
    {
        if (Marks is null || _renderContextData is null)
        {
            return;
        }

        _renderContextData.MarkRects = new List<(Rect, int, bool)>(Marks.Count);
        _renderContextData.MarkTextRects = new List<(Rect, int, bool, FormattedText)>(Marks.Count);
        var thumbSize = GetThumbSize();
        var rangeValues = EffectiveRangeValues;

        for (var i = 0; i < Marks.Count; i++)
        {
            var mark = Marks[i];
            var markIncluded = IsMarkIncluded(mark.Value, rangeValues);
            var center = MarkCenterPoint(railRect, mark.Value);
            var markRect = new Rect(
                new Point(center.X - SliderMarkSize / 2, center.Y - SliderMarkSize / 2),
                new Size(SliderMarkSize, SliderMarkSize));
            _renderContextData.MarkRects.Add((markRect, i, markIncluded));

            Point textPosition;
            if (Orientation == Orientation.Horizontal)
            {
                var textOffsetX = center.X - mark.LabelSize.Width / 2;
                var textOffsetY = railRect.Center.Y + thumbSize / 4;
                if (textOffsetX + mark.LabelSize.Width > Bounds.Width)
                {
                    textOffsetX = Bounds.Width - mark.LabelSize.Width;
                }

                textPosition = new Point(textOffsetX, textOffsetY);
            }
            else
            {
                var textOffsetX = railRect.Center.X + thumbSize / 2;
                var textOffsetY = i == 0
                    ? markRect.Y - Padding.Bottom
                    : markRect.Y - mark.LabelSize.Height / 2;
                textPosition = new Point(textOffsetX, textOffsetY);
            }

            _renderContextData.MarkTextRects.Add((new Rect(textPosition, mark.LabelSize), i, markIncluded, mark.FormattedText!));
        }
    }

    private Point MarkCenterPoint(Rect railRect, double value)
    {
        var ratio = SliderRangeMath.ValueToRatio(value, Minimum, Maximum);
        if (IsDirectionReversed)
        {
            ratio = 1 - ratio;
        }

        if (Orientation == Orientation.Horizontal)
        {
            return new Point(railRect.X + railRect.Width * ratio, railRect.Center.Y);
        }

        return new Point(railRect.Center.X, railRect.Y + railRect.Height * (1 - ratio));
    }

    private bool IsMarkIncluded(double value, IReadOnlyList<double> values)
    {
        if (!IsIncluded)
        {
            return false;
        }

        if (!IsRangeMode)
        {
            return MathUtils.LessThanOrClose(value, Value);
        }

        return values.Count > 0 &&
               MathUtils.GreaterThanOrClose(value, values[0]) &&
               MathUtils.LessThanOrClose(value, values[^1]);
    }

    private void DrawGroove(DrawingContext context)
    {
        context.DrawPilledRect(TrackGrooveBrush, null, _renderContextData!.RailRect, Orientation);
    }

    private void DrawMark(DrawingContext context)
    {
        if (_renderContextData?.MarkRects is not null)
        {
            foreach (var markRectEntry in _renderContextData.MarkRects)
            {
                var centerPos = markRectEntry.Item1.Center;
                var radius = SliderMarkSize / 2;
                if (markRectEntry.Item3)
                {
                    PenUtils.TryModifyOrCreate(ref _markBorderActivePen,
                        MarkBorderActiveBrush,
                        MarkBorderThickness.Left);
                    context.DrawEllipse(MarkBackgroundBrush, _markBorderActivePen, centerPos, radius, radius);
                }
                else
                {
                    PenUtils.TryModifyOrCreate(ref _markBorderPen,
                        MarkBorderBrush,
                        MarkBorderThickness.Left);
                    context.DrawEllipse(MarkBackgroundBrush, _markBorderPen, centerPos, radius, radius);
                }
            }
        }

        if (_renderContextData?.MarkTextRects is not null)
        {
            foreach (var markTextRectEntry in _renderContextData.MarkTextRects)
            {
                context.DrawText(markTextRectEntry.Item4, markTextRectEntry.Item1.Position);
            }
        }
    }

    private void DrawTrackBars(DrawingContext context)
    {
        if (!IsIncluded || _renderContextData is null)
        {
            return;
        }

        if (TracksBrush is not null &&
            _renderContextData.TrackRangeRect.Width > 0 &&
            _renderContextData.TrackRangeRect.Height > 0)
        {
            context.DrawPilledRect(TracksBrush, null, _renderContextData.TrackRangeRect, Orientation);
        }

        foreach (var segmentRect in _renderContextData.SegmentRects)
        {
            context.DrawPilledRect(TrackBarBrush, null, segmentRect, Orientation);
        }
    }

    private class RenderContextData
    {
        public Rect RailRect { get; set; }
        public Rect TrackRangeRect { get; set; }
        public List<Rect> SegmentRects { get; set; } = [];
        public List<(Rect, int, bool)>? MarkRects { get; set; }
        public List<(Rect, int, bool, FormattedText)>? MarkTextRects { get; set; }
    }
}
