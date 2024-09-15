﻿using System.Globalization;
using AtomUI.Input;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Utilities;

namespace AtomUI.Controls;

public record struct SliderRangeValue
{
    public double StartValue { get; set; }
    public double EndValue { get; set; }

    public static SliderRangeValue Parse(string expr)
    {
        // 这里只负责解析到 double
        const string exceptionMessage = "Parse value expression for SliderRangeValue failed";
        using (var tokenizer = new StringTokenizer(expr, CultureInfo.InvariantCulture, exceptionMessage))
        {
            try
            {
                var startValue = 0d;
                var endValue   = 0d;
                if (tokenizer.TryReadString(out var startValueStr))
                {
                    startValue = double.Parse(startValueStr);
                    if (tokenizer.TryReadString(out var endValueStr))
                    {
                        endValue = double.Parse(endValueStr);
                    }
                    else
                    {
                        // 至少要两个
                        throw new FormatException($"{exceptionMessage}, must have two value.");
                    }
                }

                // 检查顺序
                if (startValue > endValue)
                {
                    throw new ArgumentException($"{exceptionMessage}, start value must less or equal to end value.");
                }

                return new SliderRangeValue
                {
                    StartValue = startValue,
                    EndValue   = endValue
                };
            }
            catch (Exception e)
            {
                if (e is not FormatException)
                {
                    throw new FormatException(exceptionMessage, e);
                }

                throw;
            }
        }
    }
}

public record SliderMark(string Label, double Value)
{
    public IBrush? LabelBrush { get; set; }
    public FontStyle LabelFontStyle { get; set; } = FontStyle.Normal;
    public FontWeight LabelFontWeight { get; set; } = FontWeight.Normal;
    internal Size LabelSize { get; set; }
    internal FormattedText? FormattedText { get; set; }
}

/// <summary>
/// A control that lets the user select from a range of values by moving a SliderThumb control along a SliderTrack.
/// </summary>
[TemplatePart(SliderTheme.TrackPart, typeof(SliderTrack))]
[PseudoClasses(StdPseudoClass.Vertical, StdPseudoClass.Horizontal, StdPseudoClass.Pressed)]
public class Slider : RangeBase
{
   /// <summary>
   /// Defines the <see cref="Orientation" /> property.
   /// </summary>
   public static readonly StyledProperty<Orientation> OrientationProperty =
        ScrollBar.OrientationProperty.AddOwner<Slider>();

   /// <summary>
   /// Defines the <see cref="IsDirectionReversed" /> property.
   /// </summary>
   public static readonly StyledProperty<bool> IsDirectionReversedProperty =
        SliderTrack.IsDirectionReversedProperty.AddOwner<Slider>();

   /// <summary>
   /// Defines the <see cref="IsSnapToTickEnabled" /> property.
   /// </summary>
   public static readonly StyledProperty<bool> IsSnapToTickEnabledProperty =
        AvaloniaProperty.Register<Slider, bool>(nameof(IsSnapToTickEnabled));

   /// <summary>
   /// Defines the <see cref="TickFrequency" /> property.
   /// </summary>
   public static readonly StyledProperty<double> TickFrequencyProperty =
        AvaloniaProperty.Register<Slider, double>(nameof(TickFrequency));

    public static readonly StyledProperty<SliderRangeValue> RangeValueProperty =
        SliderTrack.RangeValueProperty.AddOwner<Slider>();

    public static readonly StyledProperty<bool> IsRangeModeProperty =
        SliderTrack.IsRangeModeProperty.AddOwner<Slider>();

    public static readonly StyledProperty<AvaloniaList<SliderMark>?> MarksProperty =
        SliderTrack.MarksProperty.AddOwner<Slider>();

    public static readonly StyledProperty<string> ValueFormatTemplateProperty =
        AvaloniaProperty.Register<Slider, string>(nameof(ValueFormatTemplate), "{0:0.00}");

    public static readonly StyledProperty<bool> IncludedProperty =
        SliderTrack.IncludedProperty.AddOwner<Slider>();

    /// <summary>
    /// Gets or sets the orientation of a <see cref="Slider" />.
    /// </summary>
    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// Gets or sets the direction of increasing value.
    /// </summary>
    /// <value>
    /// true if the direction of increasing value is to the left for a horizontal slider or
    /// down for a vertical slider; otherwise, false. The default is false.
    /// </value>
    public bool IsDirectionReversed
    {
        get => GetValue(IsDirectionReversedProperty);
        set => SetValue(IsDirectionReversedProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the <see cref="Slider" /> automatically moves the
    /// <see cref="SliderThumb" /> to the closest tick mark.
    /// </summary>
    public bool IsSnapToTickEnabled
    {
        get => GetValue(IsSnapToTickEnabledProperty);
        set => SetValue(IsSnapToTickEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets the interval between tick marks.
    /// </summary>
    public double TickFrequency
    {
        get => GetValue(TickFrequencyProperty);
        set => SetValue(TickFrequencyProperty, value);
    }

    public SliderRangeValue RangeValue
    {
        get => GetValue(RangeValueProperty);
        set => SetValue(RangeValueProperty, value);
    }

    public bool IsRangeMode
    {
        get => GetValue(IsRangeModeProperty);
        set => SetValue(IsRangeModeProperty, value);
    }

    public AvaloniaList<SliderMark>? Marks
    {
        get => GetValue(MarksProperty);
        set => SetValue(MarksProperty, value);
    }

    public string ValueFormatTemplate
    {
        get => GetValue(ValueFormatTemplateProperty);
        set => SetValue(ValueFormatTemplateProperty, value);
    }

    public bool Included
    {
        get => GetValue(IncludedProperty);
        set => SetValue(IncludedProperty, value);
    }

    // Slider required parts
    private bool _isFocusEngaged;
    private SliderThumb? _graspedThumb;
    private IDisposable? _pointerMovedDispose;
    private IDisposable? _pointerPressDispose;
    private IDisposable? _pointerReleaseDispose;

    private const double Tolerance = 0.0001;

    /// <summary>
    /// Initializes static members of the <see cref="Slider" /> class.
    /// </summary>
    static Slider()
    {
        PressedMixin.Attach<Slider>();
        FocusableProperty.OverrideDefaultValue<Slider>(true);
        OrientationProperty.OverrideDefaultValue(typeof(Slider), Orientation.Horizontal);
        SliderThumb.DragStartedEvent.AddClassHandler<Slider>((x, e) => x.OnThumbDragStarted(e),
            RoutingStrategies.Bubble);
        SliderThumb.DragCompletedEvent.AddClassHandler<Slider>((x, e) => x.OnThumbDragCompleted(e),
            RoutingStrategies.Bubble);

        ValueProperty.OverrideMetadata<Slider>(new StyledPropertyMetadata<double>(enableDataValidation: true));
        AutomationProperties.ControlTypeOverrideProperty.OverrideDefaultValue<Slider>(AutomationControlType.Slider);
    }

    /// <summary>
    /// Instantiates a new instance of the <see cref="Slider" /> class.
    /// </summary>
    public Slider()
    {
        UpdatePseudoClasses(Orientation);
    }

    /// <summary>
    /// Gets a value indicating whether the <see cref="Slider" /> is currently being dragged.
    /// </summary>
    protected bool IsDragging { get; private set; }

    /// <summary>
    /// Gets the <see cref="SliderTrack" /> part of the <see cref="Slider" />.
    /// </summary>
    protected SliderTrack? SliderTrack { get; private set; }

    /// <inheritdoc />
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _pointerMovedDispose?.Dispose();
        _pointerPressDispose?.Dispose();
        _pointerReleaseDispose?.Dispose();

        SliderTrack = e.NameScope.Find<SliderTrack>(SliderTheme.TrackPart);

        if (SliderTrack != null)
        {
            SliderTrack.IgnoreThumbDrag = true;
        }

        _pointerPressDispose = this.AddDisposableHandler(PointerPressedEvent, TrackPressed, RoutingStrategies.Tunnel);
        _pointerReleaseDispose =
            this.AddDisposableHandler(PointerReleasedEvent, TrackReleased, RoutingStrategies.Tunnel);
        _pointerMovedDispose = this.AddDisposableHandler(PointerMovedEvent, TrackMoved, RoutingStrategies.Tunnel);

        if (SliderTrack is not null)
        {
            if (!IsRangeMode)
            {
                if (SliderTrack.StartSliderThumb is not null)
                {
                    ToolTip.SetTip(SliderTrack.StartSliderThumb, FormatValue(Value));
                }
            }
            else
            {
                if (SliderTrack.StartSliderThumb is not null)
                {
                    ToolTip.SetTip(SliderTrack.StartSliderThumb, FormatValue(RangeValue.StartValue));
                }

                if (SliderTrack.EndSliderThumb is not null)
                {
                    ToolTip.SetTip(SliderTrack.EndSliderThumb, FormatValue(RangeValue.EndValue));
                }
            }
        }

        SetupSliderThumbPlacement();
    }

    // TODO 在 rangemode 下可能没有用
    /// <inheritdoc />
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Handled || e.KeyModifiers != KeyModifiers.None)
        {
            return;
        }

        var usingXyNavigation = this.IsAllowedXYNavigationMode(e.KeyDeviceType);
        var allowArrowKeys    = _isFocusEngaged || !usingXyNavigation;

        var handled = true;

        switch (e.Key)
        {
            case Key.Enter when usingXyNavigation:
                _isFocusEngaged = !_isFocusEngaged;
                handled         = true;
                break;
            case Key.Escape when usingXyNavigation:
                _isFocusEngaged = false;
                handled         = true;
                break;

            case Key.Down when allowArrowKeys:
            case Key.Left when allowArrowKeys:
                MoveToNextTick(IsDirectionReversed ? SmallChange : -SmallChange);
                break;

            case Key.Up when allowArrowKeys:
            case Key.Right when allowArrowKeys:
                MoveToNextTick(IsDirectionReversed ? -SmallChange : SmallChange);
                break;

            case Key.PageUp:
                MoveToNextTick(IsDirectionReversed ? -LargeChange : LargeChange);
                break;

            case Key.PageDown:
                MoveToNextTick(IsDirectionReversed ? LargeChange : -LargeChange);
                break;

            case Key.Home:
                SetCurrentValue(ValueProperty, Minimum);
                break;

            case Key.End:
                SetCurrentValue(ValueProperty, Maximum);
                break;

            default:
                handled = false;
                break;
        }

        e.Handled = handled;
    }

    private void MoveToNextTick(double direction)
    {
        if (direction == 0.0)
        {
            return;
        }

        var value = Value;

        // Find the next value by snapping
        var next = SnapToTick(Math.Max(Minimum, Math.Min(Maximum, value + direction)));

        var greaterThan = direction > 0; //search for the next tick greater than value?

        // If the snapping brought us back to value, find the next tick point
        if (Math.Abs(next - value) < Tolerance
            && !(greaterThan && Math.Abs(value - Maximum) < Tolerance) // Stop if searching up if already at Max
            && !(!greaterThan && Math.Abs(value - Minimum) < Tolerance)) // Stop if searching down if already at Min
        {
            // var ticks = Ticks;

            // If ticks collection is available, use it.
            // // Note that ticks may be unsorted.
            // if (ticks != null && ticks.Count > 0) {
            //    foreach (var tick in ticks) {
            //       // Find the smallest tick greater than value or the largest tick less than value
            //       if (greaterThan && MathUtilities.GreaterThan(tick, value) &&
            //           (MathUtilities.LessThan(tick, next) || Math.Abs(next - value) < Tolerance)
            //           || !greaterThan && MathUtilities.LessThan(tick, value) &&
            //           (MathUtilities.GreaterThan(tick, next) || Math.Abs(next - value) < Tolerance)) {
            //          next = tick;
            //       }
            //    }
            // } else

            if (MathUtilities.GreaterThan(TickFrequency, 0.0))
            {
                // Find the current tick we are at
                var tickNumber = Math.Round((value - Minimum) / TickFrequency);

                if (greaterThan)
                {
                    tickNumber += 1.0;
                }
                else
                {
                    tickNumber -= 1.0;
                }

                next = Minimum + tickNumber * TickFrequency;
            }
        }

        // Update if we've found a better value
        if (Math.Abs(next - value) > Tolerance)
        {
            SetCurrentValue(ValueProperty, next);
        }
    }

    private void TrackMoved(object? sender, PointerEventArgs e)
    {
        if (!IsEnabled)
        {
            IsDragging = false;
            return;
        }

        if (IsDragging)
        {
            MoveToPoint(e.GetCurrentPoint(SliderTrack));
        }
    }

    private void TrackReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_graspedThumb is not null)
        {
            ToolTip.SetIsCustomHide(_graspedThumb, false);
            if (!_graspedThumb.Bounds.Contains(e.GetPosition(SliderTrack)))
            {
                ToolTip.SetIsOpen(_graspedThumb, false);
            }
        }

        IsDragging    = false;
        _graspedThumb = null;
    }

    private void TrackPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            var posOnTrack = e.GetCurrentPoint(SliderTrack);

            _graspedThumb = GetEffectiveMoveThumb(posOnTrack.Position);

            var mark = SliderTrack?.GetMarkForPosition(posOnTrack.Position);
            if (mark is not null)
            {
                // 修正坐标
                if (!IsRangeMode)
                {
                    Value = mark.Value;
                }
                else
                {
                    if (_graspedThumb == SliderTrack?.StartSliderThumb)
                    {
                        var endValue = RangeValue.EndValue;
                        RangeValue = new SliderRangeValue
                        {
                            StartValue = mark.Value,
                            EndValue   = endValue
                        };
                    }
                    else
                    {
                        var startValue = RangeValue.StartValue;
                        RangeValue = new SliderRangeValue
                        {
                            StartValue = startValue,
                            EndValue   = mark.Value
                        };
                    }
                }

                return;
            }

            MoveToPoint(posOnTrack);
            if (_graspedThumb is not null)
            {
                ToolTip.SetIsCustomHide(_graspedThumb, true);
            }

            IsDragging = true;
        }
    }

    private void MoveToPoint(PointerPoint posOnTrack)
    {
        if (SliderTrack is null)
        {
            return;
        }

        var orient      = Orientation == Orientation.Horizontal;
        var sliderThumb = _graspedThumb;
        var thumbLength = (orient
            ? sliderThumb?.Bounds.Width ?? 0.0
            : sliderThumb?.Bounds.Height ?? 0.0) + double.Epsilon;
        var trackLength = (orient
            ? SliderTrack.Bounds.Width
            : SliderTrack.Bounds.Height) - thumbLength;
        var trackPos   = orient ? posOnTrack.Position.X : posOnTrack.Position.Y;
        var logicalPos = MathUtilities.Clamp((trackPos - thumbLength * 0.5) / trackLength, 0.0d, 1.0d);
        var invert = orient ? IsDirectionReversed ? 1 : 0 :
            IsDirectionReversed ? 0 : 1;
        var calcVal    = Math.Abs(invert - logicalPos);
        var range      = Maximum - Minimum;
        var finalValue = calcVal * range + Minimum;
        finalValue = IsSnapToTickEnabled ? SnapToTick(finalValue) : finalValue;
        if (!IsRangeMode)
        {
            SetCurrentValue(ValueProperty, finalValue);
        }
        else
        {
            var currentRangeValue = RangeValue;
            if (sliderThumb == SliderTrack.StartSliderThumb)
            {
                currentRangeValue.StartValue = finalValue;
            }
            else
            {
                currentRangeValue.EndValue = finalValue;
            }

            SetCurrentValue(RangeValueProperty, currentRangeValue);
        }

        if (sliderThumb is not null && !sliderThumb.IsFocused)
        {
            sliderThumb.Focus();
        }
    }

    private SliderThumb? GetEffectiveMoveThumb(Point point)
    {
        if (SliderTrack is null)
        {
            return null;
        }

        if (!IsRangeMode)
        {
            return SliderTrack.StartSliderThumb;
        }

        // 看谁离的近
        var startThumbCenter = SliderTrack.StartSliderThumb!.Bounds.Center;
        var endThumbCenter   = SliderTrack.EndSliderThumb!.Bounds.Center;
        var startThumbDelta  = 0d;
        var endThumbDelta    = 0d;
        if (Orientation == Orientation.Horizontal)
        {
            startThumbDelta = Math.Abs(startThumbCenter.X - point.X);
            endThumbDelta   = Math.Abs(endThumbCenter.X - point.X);
        }
        else
        {
            startThumbDelta = Math.Abs(startThumbCenter.Y - point.Y);
            endThumbDelta   = Math.Abs(endThumbCenter.Y - point.Y);
        }

        if (startThumbDelta < endThumbDelta)
        {
            return SliderTrack.StartSliderThumb;
        }

        return SliderTrack.EndSliderThumb;
    }

    /// <inheritdoc />
    protected override void UpdateDataValidation(
        AvaloniaProperty property,
        BindingValueType state,
        Exception? error)
    {
        if (property == ValueProperty)
        {
            DataValidationErrors.SetError(this, error);
        }
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new SliderAutomationPeer(this);
    }

    private void SetupSliderThumbPlacement()
    {
        if (SliderTrack is not null)
        {
            if (Orientation == Orientation.Horizontal)
            {
                if (SliderTrack.StartSliderThumb is not null)
                {
                    ToolTip.SetPlacement(SliderTrack.StartSliderThumb, PlacementMode.Top);
                }

                if (SliderTrack.EndSliderThumb is not null)
                {
                    ToolTip.SetPlacement(SliderTrack.EndSliderThumb, PlacementMode.Top);
                }
            }
            else
            {
                if (SliderTrack.StartSliderThumb is not null)
                {
                    ToolTip.SetPlacement(SliderTrack.StartSliderThumb, PlacementMode.Right);
                }

                if (SliderTrack.EndSliderThumb is not null)
                {
                    ToolTip.SetPlacement(SliderTrack.EndSliderThumb, PlacementMode.Right);
                }
            }
        }
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == OrientationProperty)
        {
            UpdatePseudoClasses(change.GetNewValue<Orientation>());
            SetupSliderThumbPlacement();
        }
        else if (change.Property == ValueProperty)
        {
            if (SliderTrack is not null && SliderTrack.StartSliderThumb is not null)
            {
                ToolTip.SetTip(SliderTrack.StartSliderThumb, FormatValue(Value));
            }
        }
        else if (change.Property == RangeValueProperty)
        {
            if (SliderTrack is not null)
            {
                if (SliderTrack.StartSliderThumb is not null)
                {
                    ToolTip.SetTip(SliderTrack.StartSliderThumb, FormatValue(RangeValue.StartValue));
                }

                if (SliderTrack.EndSliderThumb is not null)
                {
                    ToolTip.SetTip(SliderTrack.EndSliderThumb, FormatValue(RangeValue.EndValue));
                }
            }
        }
    }

    private string FormatValue(double value)
    {
        return string.Format(ValueFormatTemplate, value);
    }

    /// <summary>
    /// Called when user start dragging the <see cref="SliderThumb" />.
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnThumbDragStarted(VectorEventArgs e)
    {
        IsDragging = true;
    }

    /// <summary>
    /// Called when user stop dragging the <see cref="SliderThumb" />.
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnThumbDragCompleted(VectorEventArgs e)
    {
        IsDragging = false;
    }

    /// <summary>
    /// Snap the input 'value' to the closest tick.
    /// </summary>
    /// <param name="value">Value that want to snap to closest Tick.</param>
    private double SnapToTick(double value)
    {
        if (IsSnapToTickEnabled)
        {
            var previous = Minimum;
            var next     = Maximum;

            // This property is rarely set so let's try to avoid the GetValue
            // var ticks = Ticks;

            // If ticks collection is available, use it.
            // Note that ticks may be unsorted.
            // if (ticks != null && ticks.Count > 0) {
            //    foreach (var tick in ticks) {
            //       if (MathUtilities.AreClose(tick, value)) {
            //          return value;
            //       }
            //
            //       if (MathUtilities.LessThan(tick, value) && MathUtilities.GreaterThan(tick, previous)) {
            //          previous = tick;
            //       } else if (MathUtilities.GreaterThan(tick, value) && MathUtilities.LessThan(tick, next)) {
            //          next = tick;
            //       }
            //    }
            // } else 

            if (MathUtilities.GreaterThan(TickFrequency, 0.0))
            {
                previous = Minimum + Math.Round((value - Minimum) / TickFrequency) * TickFrequency;
                next     = Math.Min(Maximum, previous + TickFrequency);
            }

            // Choose the closest value between previous and next. If tie, snap to 'next'.
            value = MathUtilities.GreaterThanOrClose(value, (previous + next) * 0.5) ? next : previous;
        }

        return value;
    }

    private void UpdatePseudoClasses(Orientation o)
    {
        PseudoClasses.Set(StdPseudoClass.Vertical, o == Orientation.Vertical);
        PseudoClasses.Set(StdPseudoClass.Horizontal, o == Orientation.Horizontal);
    }
}