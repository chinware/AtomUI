using AtomUI.Collections.Pooled;
using AtomUI.Controls.Primitives;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Utilities;
using Thumb = AtomUI.Controls.Primitives.Thumb;

namespace AtomUI.Controls;

[PseudoClasses(StdPseudoClass.Pressed, ColorSliderPseudoClass.DarkSelector, ColorSliderPseudoClass.LightSelector)]
internal class AbstractColorSlider : RangeBase
{
    #region 公共属性定义

    public static readonly StyledProperty<Color> ColorProperty =
        AvaloniaProperty.Register<AbstractColorSlider, Color>(
            nameof(Color),
            Colors.White,
            defaultBindingMode: BindingMode.TwoWay);
    
    public static readonly StyledProperty<ColorComponent> ColorComponentProperty =
        AvaloniaProperty.Register<AbstractColorSlider, ColorComponent>(
            nameof(ColorComponent),
            ColorComponent.Component1);
    
    public static readonly StyledProperty<ColorModel> ColorModelProperty =
        AvaloniaProperty.Register<AbstractColorSlider, ColorModel>(
            nameof(ColorModel),
            ColorModel.Rgba);
    
    public static readonly StyledProperty<HsvColor> HsvColorProperty =
        AvaloniaProperty.Register<AbstractColorSlider, HsvColor>(
            nameof(HsvColor),
            Colors.White.ToHsv(),
            defaultBindingMode: BindingMode.TwoWay);
    
    public static readonly StyledProperty<bool> IsAlphaVisibleProperty =
        AvaloniaProperty.Register<AbstractColorSlider, bool>(
            nameof(IsAlphaVisible),
            false);
    
    public static readonly StyledProperty<bool> IsPerceptiveProperty =
        AvaloniaProperty.Register<AbstractColorSlider, bool>(
            nameof(IsPerceptive),
            true);
    
    public static readonly StyledProperty<bool> IsRoundingEnabledProperty =
        AvaloniaProperty.Register<AbstractColorSlider, bool>(
            nameof(IsRoundingEnabled),
            false);
    
    public Color Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public ColorComponent ColorComponent
    {
        get => GetValue(ColorComponentProperty);
        set => SetValue(ColorComponentProperty, value);
    }
    
    public ColorModel ColorModel
    {
        get => GetValue(ColorModelProperty);
        set => SetValue(ColorModelProperty, value);
    }
    
    public HsvColor HsvColor
    {
        get => GetValue(HsvColorProperty);
        set => SetValue(HsvColorProperty, value);
    }
    
    public bool IsAlphaVisible
    {
        get => GetValue(IsAlphaVisibleProperty);
        set => SetValue(IsAlphaVisibleProperty, value);
    }
    
    /// <summary>
    /// Gets or sets a value indicating whether the slider adapts rendering to improve user-perception
    /// over exactness.
    /// </summary>
    /// <remarks>
    /// When true in the HSVA color model, this ensures that the gradient is always visible and
    /// never washed out regardless of the actual color. When true in the RGBA color model, this ensures
    /// the gradient always appears as red, green or blue.
    /// <br/><br/>
    /// For example, with Hue in the HSVA color model, the Saturation and Value components are always forced
    /// to maximum values during rendering. In the RGBA color model, all components other than
    /// <see cref="ColorComponent"/> are forced to minimum values during rendering.
    /// <br/><br/>
    /// Note this property will only adjust components other than <see cref="ColorComponent"/> during rendering.
    /// This also doesn't change the values of any components in the actual color – it is only for display.
    /// </remarks>
    public bool IsPerceptive
    {
        get => GetValue(IsPerceptiveProperty);
        set => SetValue(IsPerceptiveProperty, value);
    }
    
    /// <summary>
    /// Gets or sets a value indicating whether rounding of color component values is enabled.
    /// </summary>
    /// <remarks>
    /// This is applicable for the HSV color model only. The <see cref="Avalonia.Media.HsvColor"/> struct uses double
    /// values while the <see cref="Media.Color"/> struct uses byte. Only double types need rounding.
    /// </remarks>
    public bool IsRoundingEnabled
    {
        get => GetValue(IsRoundingEnabledProperty);
        set => SetValue(IsRoundingEnabledProperty, value);
    }

    #endregion

    #region 内部属性定义

    public static readonly DirectProperty<AbstractColorSlider, double> ThumbSizeProperty =
        AvaloniaProperty.RegisterDirect<AbstractColorSlider, double>(
            nameof(ThumbSize),
            o => o.ThumbSize,
            (o, v) => o.ThumbSize = v);
    
    private double _thumbSize = 0.0d;

    public double ThumbSize
    {
        get => _thumbSize;
        set => SetAndRaise(ThumbSizeProperty, ref _thumbSize, value);
    }
    
    #endregion
    
    public event EventHandler<ColorChangedEventArgs>? ColorChanged;
    
    /// <summary>
    /// Defines the maximum hue component value
    /// (other components are always 0..100 or 0.255).
    /// </summary>
    /// <remarks>
    /// This should match the default <see cref="ColorSpectrum.MaxHue"/> property.
    /// </remarks>
    private const double MaxHue = 359;
    
    protected bool IgnorePropertyChanged = false;

    private Bitmap? _backgroundBitmap;
    
    // Slider required parts
    protected internal bool IsDragging;
    protected internal bool IsFocusEngaged;
    protected internal AbstractColorPickerSliderTrack? Track;
    private IDisposable? _pointerMovedDispose;

    private const double Tolerance = 0.0001;

    static AbstractColorSlider()
    {
        PressedMixin.Attach<AbstractColorSlider>();
        FocusableProperty.OverrideDefaultValue<AbstractColorSlider>(true);
        Thumb.DragStartedEvent.AddClassHandler<AbstractColorSlider>((x, e) => x.OnThumbDragStarted(e), RoutingStrategies.Bubble);
        Thumb.DragCompletedEvent.AddClassHandler<AbstractColorSlider>((x, e) => x.OnThumbDragCompleted(e),
            RoutingStrategies.Bubble);

        ValueProperty.OverrideMetadata<AbstractColorSlider>(new(enableDataValidation: true));
    }

    protected void ConfigureCornerRadius()
    {
        CornerRadius = new CornerRadius(Height / 2);
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
            MoveToPoint(e.GetCurrentPoint(Track));
        }
    }
    
    protected void TrackReleased(object? sender, PointerReleasedEventArgs e)
    {
        IsDragging = false;
    }
    
    protected void TrackPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            MoveToPoint(e.GetCurrentPoint(Track));
            IsDragging = true;
        }
    }

    protected void MoveToPoint(PointerPoint posOnTrack)
    {
        if (Track is null)
        {
            return;
        }
        var thumbLength = ThumbSize + double.Epsilon;
        var trackLength = Track.Bounds.Width - thumbLength;
        var trackPos    = posOnTrack.Position.X;
        var logicalPos  = MathUtilities.Clamp((trackPos - thumbLength * 0.5) / trackLength, 0.0d, 1.0d);
        var calcVal     = Math.Abs(-logicalPos);
        var range       = Maximum - Minimum;
        var finalValue  = calcVal * range + Minimum;

        SetCurrentValue(ValueProperty, finalValue);
    }
    
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
    
    protected virtual void OnThumbDragStarted(VectorEventArgs e)
    {
        IsDragging = true;
    }
    
    protected virtual void OnThumbDragCompleted(VectorEventArgs e)
    {
        IsDragging = false;
    }
    
    /// <summary>
    /// Updates the visual state of the control by applying latest PseudoClasses.
    /// </summary>
    private void UpdatePseudoClasses()
    {
        // The slider itself can be transparent for certain color values.
        // This causes an issue where a white selector thumb over a light window background or
        // a black selector thumb over a dark window background is not visible.
        // This means under a certain alpha threshold, neither a white or black selector thumb
        // should be shown and instead the default slider thumb color should be used instead.
        if (Color.A < 128 &&
            (IsAlphaVisible || ColorComponent == ColorComponent.Alpha))
        {
            PseudoClasses.Set(ColorSliderPseudoClass.DarkSelector, false);
            PseudoClasses.Set(ColorSliderPseudoClass.LightSelector, false);
        }
        else
        {
            Color perceivedColor;

            if (ColorModel == ColorModel.Hsva)
            {
                perceivedColor = GetPerceptiveBackgroundColor(HsvColor).ToRgb();
            }
            else
            {
                perceivedColor = GetPerceptiveBackgroundColor(Color);
            }

            if (ColorHelper.GetRelativeLuminance(perceivedColor) <= 0.5)
            {
                PseudoClasses.Set(ColorSliderPseudoClass.DarkSelector, false);
                PseudoClasses.Set(ColorSliderPseudoClass.LightSelector, true);
            }
            else
            {
                PseudoClasses.Set(ColorSliderPseudoClass.DarkSelector, true);
                PseudoClasses.Set(ColorSliderPseudoClass.LightSelector, false);
            }
        }
    }
    
    /// <summary>
    /// Generates a new background image for the color slider and applies it.
    /// </summary>
    private async void UpdateBackground()
    {
        // In Avalonia, Bounds returns the actual device-independent pixel size of a control.
        // However, this is not necessarily the size of the control rendered on a display.
        // A desktop or application scaling factor may be applied which must be accounted for here.
        // Remember bitmaps in Avalonia are rendered mapping to actual device pixels, not the device-
        // independent pixels of controls.

        var scale = LayoutHelper.GetLayoutScale(this);
        int pixelWidth;
        int pixelHeight;

        if (Track != null)
        {
            pixelWidth  = Convert.ToInt32(Track.Bounds.Width * scale);
            pixelHeight = Convert.ToInt32(Track.Bounds.Height * scale);
        }
        else
        {
            // As a fallback, attempt to calculate using the overall control size
            // This shouldn't happen as a track is a required template part of a slider
            // However, if it does, the spectrum gradient will still be shown
            pixelWidth  = Convert.ToInt32(Bounds.Width * scale);
            pixelHeight = Convert.ToInt32(Bounds.Height * scale);
        }

        if (pixelWidth != 0 && pixelHeight != 0)
        {
            // siteToCapacity = true, because CreateComponentBitmapAsync sets bytes via indexer over pre-allocated buffer. 
            using var bgraPixelData = new PooledList<byte>(pixelWidth * pixelHeight * 4, ClearMode.Never, true);
            await ColorPickerHelpers.CreateComponentBitmapAsync(
                bgraPixelData,
                pixelWidth,
                pixelHeight,
                Orientation.Horizontal,
                ColorModel,
                ColorComponent,
                HsvColor,
                IsAlphaVisible,
                IsPerceptive);

            _backgroundBitmap?.Dispose();
            _backgroundBitmap = ColorPickerHelpers.CreateBitmapFromPixelData(bgraPixelData, pixelWidth, pixelHeight);

            Background = new ImageBrush(_backgroundBitmap);
        }
    }
    
    /// <summary>
    /// Rounds the component values of the given <see cref="HsvColor"/>.
    /// This is useful for user-display and to ensure a color matches user selection exactly.
    /// </summary>
    /// <param name="hsvColor">The <see cref="HsvColor"/> to round component values for.</param>
    /// <returns>A new <see cref="HsvColor"/> with rounded component values.</returns>
    private static HsvColor RoundComponentValues(HsvColor hsvColor)
    {
        return new HsvColor(
            Math.Round(hsvColor.A, 2, MidpointRounding.AwayFromZero),
            Math.Round(hsvColor.H, 0, MidpointRounding.AwayFromZero),
            Math.Round(hsvColor.S, 2, MidpointRounding.AwayFromZero),
            Math.Round(hsvColor.V, 2, MidpointRounding.AwayFromZero));
    }

    /// <summary>
    /// Updates the slider property values by applying the current color.
    /// </summary>
    /// <remarks>
    /// Warning: This will trigger property changed updates.
    /// Consider using <see cref="ignorePropertyChanged"/> externally.
    /// </remarks>
    private void SetColorToSliderValues()
    {
        var component = ColorComponent;

        if (ColorModel == ColorModel.Hsva)
        {
            var hsvColor = HsvColor;

            if (IsRoundingEnabled)
            {
                hsvColor = RoundComponentValues(hsvColor);
            }

            // Note: Components converted into a usable range for the user
            switch (component)
            {
                case ColorComponent.Alpha:
                    Minimum = 0;
                    Maximum = 100;
                    Value   = hsvColor.A * 100;
                    break;
                case ColorComponent.Component1: // Hue
                    Minimum = 0;
                    Maximum = MaxHue;
                    Value   = hsvColor.H;
                    break;
                case ColorComponent.Component2: // Saturation
                    Minimum = 0;
                    Maximum = 100;
                    Value   = hsvColor.S * 100;
                    break;
                case ColorComponent.Component3: // Value
                    Minimum = 0;
                    Maximum = 100;
                    Value   = hsvColor.V * 100;
                    break;
            }
        }
        else
        {
            var rgbColor = Color;

            switch (component)
            {
                case ColorComponent.Alpha:
                    Minimum = 0;
                    Maximum = 255;
                    Value   = Convert.ToDouble(rgbColor.A);
                    break;
                case ColorComponent.Component1: // Red
                    Minimum = 0;
                    Maximum = 255;
                    Value   = Convert.ToDouble(rgbColor.R);
                    break;
                case ColorComponent.Component2: // Green
                    Minimum = 0;
                    Maximum = 255;
                    Value   = Convert.ToDouble(rgbColor.G);
                    break;
                case ColorComponent.Component3: // Blue
                    Minimum = 0;
                    Maximum = 255;
                    Value   = Convert.ToDouble(rgbColor.B);
                    break;
            }
        }
    }

    /// <summary>
    /// Gets the current color determined by the slider values.
    /// </summary>
    private (Color, HsvColor) GetColorFromSliderValues()
    {
        HsvColor hsvColor      = new HsvColor();
        Color    rgbColor      = new Color();
        double   sliderPercent = Value / (Maximum - Minimum);
        var      component     = ColorComponent;

        if (ColorModel == ColorModel.Hsva)
        {
            var baseHsvColor = HsvColor;

            switch (component)
            {
                case ColorComponent.Alpha:
                {
                    hsvColor = new HsvColor(sliderPercent, baseHsvColor.H, baseHsvColor.S, baseHsvColor.V);
                    break;
                }
                case ColorComponent.Component1:
                {
                    hsvColor = new HsvColor(baseHsvColor.A, sliderPercent * MaxHue, baseHsvColor.S, baseHsvColor.V);
                    break;
                }
                case ColorComponent.Component2:
                {
                    hsvColor = new HsvColor(baseHsvColor.A, baseHsvColor.H, sliderPercent, baseHsvColor.V);
                    break;
                }
                case ColorComponent.Component3:
                {
                    hsvColor = new HsvColor(baseHsvColor.A, baseHsvColor.H, baseHsvColor.S, sliderPercent);
                    break;
                }
            }

            rgbColor = hsvColor.ToRgb();
        }
        else
        {
            var baseRgbColor = Color;

            byte componentValue = Convert.ToByte(MathUtilities.Clamp(sliderPercent * 255, 0, 255));

            switch (component)
            {
                case ColorComponent.Alpha:
                    rgbColor = new Color(componentValue, baseRgbColor.R, baseRgbColor.G, baseRgbColor.B);
                    break;
                case ColorComponent.Component1:
                    rgbColor = new Color(baseRgbColor.A, componentValue, baseRgbColor.G, baseRgbColor.B);
                    break;
                case ColorComponent.Component2:
                    rgbColor = new Color(baseRgbColor.A, baseRgbColor.R, componentValue, baseRgbColor.B);
                    break;
                case ColorComponent.Component3:
                    rgbColor = new Color(baseRgbColor.A, baseRgbColor.R, baseRgbColor.G, componentValue);
                    break;
            }

            hsvColor = rgbColor.ToHsv();
        }

        if (IsRoundingEnabled)
        {
            hsvColor = RoundComponentValues(hsvColor);
        }

        return (rgbColor, hsvColor);
    }

    /// <summary>
    /// Gets the actual background color displayed for the given HSV color.
    /// This can differ due to the effects of certain properties intended to improve perception.
    /// </summary>
    /// <param name="hsvColor">The actual color to get the equivalent background color for.</param>
    /// <returns>The equivalent, perceived background color.</returns>
    private HsvColor GetPerceptiveBackgroundColor(HsvColor hsvColor)
    {
        var component      = ColorComponent;
        var isAlphaVisible = IsAlphaVisible;
        var isPerceptive   = IsPerceptive;

        if (isAlphaVisible == false &&
            component != ColorComponent.Alpha)
        {
            hsvColor = new HsvColor(1.0, hsvColor.H, hsvColor.S, hsvColor.V);
        }

        if (isPerceptive)
        {
            switch (component)
            {
                case ColorComponent.Component1:
                    return new HsvColor(hsvColor.A, hsvColor.H, 1.0, 1.0);
                case ColorComponent.Component2:
                    return new HsvColor(hsvColor.A, hsvColor.H, hsvColor.S, 1.0);
                case ColorComponent.Component3:
                    return new HsvColor(hsvColor.A, hsvColor.H, 1.0, hsvColor.V);
                default:
                    return hsvColor;
            }
        }
        return hsvColor;
    }

    /// <summary>
    /// Gets the actual background color displayed for the given RGB color.
    /// This can differ due to the effects of certain properties intended to improve perception.
    /// </summary>
    /// <param name="rgbColor">The actual color to get the equivalent background color for.</param>
    /// <returns>The equivalent, perceived background color.</returns>
    private Color GetPerceptiveBackgroundColor(Color rgbColor)
    {
        var component      = ColorComponent;
        var isAlphaVisible = IsAlphaVisible;
        var isPerceptive   = IsPerceptive;

        if (isAlphaVisible == false &&
            component != ColorComponent.Alpha)
        {
            rgbColor = new Color(255, rgbColor.R, rgbColor.G, rgbColor.B);
        }

        if (isPerceptive)
        {
            switch (component)
            {
                case ColorComponent.Component1:
                    return new Color(rgbColor.A, rgbColor.R, 0, 0);
                case ColorComponent.Component2:
                    return new Color(rgbColor.A, 0, rgbColor.G, 0);
                case ColorComponent.Component3:
                    return new Color(rgbColor.A, 0, 0, rgbColor.B);
                default:
                    return rgbColor;
            }
        }
        else
        {
            return rgbColor;
        }
    }
        
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (IgnorePropertyChanged)
        {
            base.OnPropertyChanged(change);
            return;
        }

        if (change.Property == ColorProperty)
        {
            IgnorePropertyChanged = true;

            // Always keep the two color properties in sync
            SetCurrentValue(HsvColorProperty, Color.ToHsv());

            SetColorToSliderValues();
            UpdateBackground();
            UpdatePseudoClasses();

            NotifyColorChanged(new ColorChangedEventArgs(
                change.GetOldValue<Color>(),
                change.GetNewValue<Color>()));

            IgnorePropertyChanged = false;
        }
        else if (change.Property == ColorComponentProperty ||
                 change.Property == ColorModelProperty ||
                 change.Property == IsAlphaVisibleProperty ||
                 change.Property == IsPerceptiveProperty)
        {
            IgnorePropertyChanged = true;

            SetColorToSliderValues();
            UpdateBackground();
            UpdatePseudoClasses();

            IgnorePropertyChanged = false;
        }
        else if (change.Property == HsvColorProperty)
        {
            IgnorePropertyChanged = true;

            // Always keep the two color properties in sync
            SetCurrentValue(ColorProperty, HsvColor.ToRgb());

            SetColorToSliderValues();
            UpdateBackground();
            UpdatePseudoClasses();

            NotifyColorChanged(new ColorChangedEventArgs(
                change.GetOldValue<HsvColor>().ToRgb(),
                change.GetNewValue<HsvColor>().ToRgb()));

            IgnorePropertyChanged = false;
        }
        else if (change.Property == IsRoundingEnabledProperty)
        {
            SetColorToSliderValues();
        }
        else if (change.Property == BoundsProperty)
        {
            // If the control's overall dimensions have changed the background bitmap size also needs to change.
            // This means the existing bitmap must be released to be recreated correctly in UpdateBackground().
            _backgroundBitmap?.Dispose();
            _backgroundBitmap = null;

            UpdateBackground();
            UpdatePseudoClasses();
        }
        else if (change.Property == ValueProperty ||
                 change.Property == MinimumProperty ||
                 change.Property == MaximumProperty)
        {
            IgnorePropertyChanged = true;

            Color oldColor = Color;
            (var color, var hsvColor) = GetColorFromSliderValues();

            if (ColorModel == ColorModel.Hsva)
            {
                SetCurrentValue(HsvColorProperty, hsvColor);
                SetCurrentValue(ColorProperty, hsvColor.ToRgb());
            }
            else
            {
                SetCurrentValue(ColorProperty, color);
                SetCurrentValue(HsvColorProperty, color.ToHsv());
            }

            UpdatePseudoClasses();
            NotifyColorChanged(new ColorChangedEventArgs(oldColor, Color));

            IgnorePropertyChanged = false;
        }
        else if (change.Property == HeightProperty ||
                 change.Property == WidthProperty)
        {
            ConfigureCornerRadius();
        }

        base.OnPropertyChanged(change);
    }
          
    protected virtual void NotifyColorChanged(ColorChangedEventArgs e)
    {
        ColorChanged?.Invoke(this, e);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _pointerMovedDispose?.Dispose();
        _pointerMovedDispose = this.AddDisposableHandler(PointerMovedEvent, TrackMoved, RoutingStrategies.Tunnel);
    }
}