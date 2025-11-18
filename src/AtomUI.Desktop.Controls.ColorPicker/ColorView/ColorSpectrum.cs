// This source file is adapted from the WinUI project.
// (https://github.com/microsoft/microsoft-ui-xaml)

using AtomUI.Collections.Pooled;
using AtomUI.Desktop.Controls.Primitives;
using AtomUI.Desktop.Controls.Themes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Avalonia.Utilities;

namespace AtomUI.Desktop.Controls;

internal class ColorSpectrum : TemplatedControl
{
    /// <summary>
    /// Defines the <see cref="Color"/> property.
    /// </summary>
    public static readonly StyledProperty<Color> ColorProperty =
        AvaloniaProperty.Register<ColorSpectrum, Color>(
            nameof(Color),
            Colors.White,
            defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// Defines the <see cref="Components"/> property.
    /// </summary>
    public static readonly StyledProperty<ColorSpectrumComponents> ComponentsProperty =
        AvaloniaProperty.Register<ColorSpectrum, ColorSpectrumComponents>(
            nameof(Components),
            ColorSpectrumComponents.HueSaturation);

    /// <summary>
    /// Defines the <see cref="HsvColor"/> property.
    /// </summary>
    public static readonly StyledProperty<HsvColor> HsvColorProperty =
        AvaloniaProperty.Register<ColorSpectrum, HsvColor>(
            nameof(HsvColor),
            Colors.White.ToHsv(),
            defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// Defines the <see cref="MaxHue"/> property.
    /// </summary>
    public static readonly StyledProperty<int> MaxHueProperty =
        AvaloniaProperty.Register<ColorSpectrum, int>(
            nameof(MaxHue),
            359);

    /// <summary>
    /// Defines the <see cref="MaxSaturation"/> property.
    /// </summary>
    public static readonly StyledProperty<int> MaxSaturationProperty =
        AvaloniaProperty.Register<ColorSpectrum, int>(
            nameof(MaxSaturation),
            100);

    /// <summary>
    /// Defines the <see cref="MaxValue"/> property.
    /// </summary>
    public static readonly StyledProperty<int> MaxValueProperty =
        AvaloniaProperty.Register<ColorSpectrum, int>(
            nameof(MaxValue),
            100);

    /// <summary>
    /// Defines the <see cref="MinHue"/> property.
    /// </summary>
    public static readonly StyledProperty<int> MinHueProperty =
        AvaloniaProperty.Register<ColorSpectrum, int>(
            nameof(MinHue),
            0);

    /// <summary>
    /// Defines the <see cref="MinSaturation"/> property.
    /// </summary>
    public static readonly StyledProperty<int> MinSaturationProperty =
        AvaloniaProperty.Register<ColorSpectrum, int>(
            nameof(MinSaturation),
            0);

    /// <summary>
    /// Defines the <see cref="MinValue"/> property.
    /// </summary>
    public static readonly StyledProperty<int> MinValueProperty =
        AvaloniaProperty.Register<ColorSpectrum, int>(
            nameof(MinValue),
            0);

    /// <summary>
    /// Defines the <see cref="Shape"/> property.
    /// </summary>
    public static readonly StyledProperty<ColorSpectrumShape> ShapeProperty =
        AvaloniaProperty.Register<ColorSpectrum, ColorSpectrumShape>(
            nameof(Shape),
            ColorSpectrumShape.Box);

    /// <summary>
    /// Defines the <see cref="ThirdComponent"/> property.
    /// </summary>
    public static readonly DirectProperty<ColorSpectrum, ColorComponent> ThirdComponentProperty =
        AvaloniaProperty.RegisterDirect<ColorSpectrum, ColorComponent>(
            nameof(ThirdComponent),
            o => o.ThirdComponent);

    /// <summary>
    /// Gets or sets the currently selected color in the RGB color model.
    /// </summary>
    /// <remarks>
    /// For control authors, use <see cref="HsvColor"/> instead to avoid loss
    /// of precision and color drifting.
    /// </remarks>
    public Color Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the two HSV color components displayed by the spectrum.
    /// </summary>
    /// <remarks>
    /// Internally, the <see cref="ColorSpectrum"/> uses the HSV color model.
    /// </remarks>
    public ColorSpectrumComponents Components
    {
        get => GetValue(ComponentsProperty);
        set => SetValue(ComponentsProperty, value);
    }

    /// <summary>
    /// Gets or sets the currently selected color in the HSV color model.
    /// </summary>
    /// <remarks>
    /// This should be used in all cases instead of the <see cref="Color"/> property.
    /// Internally, the <see cref="ColorSpectrum"/> uses the HSV color model and using
    /// this property will avoid loss of precision and color drifting.
    /// </remarks>
    public HsvColor HsvColor
    {
        get => GetValue(HsvColorProperty);
        set => SetValue(HsvColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum value of the Hue component in the range from 0..359.
    /// This property must be greater than <see cref="MinHue"/>.
    /// </summary>
    /// <remarks>
    /// Internally, the <see cref="ColorSpectrum"/> uses the HSV color model.
    /// </remarks>
    public int MaxHue
    {
        get => GetValue(MaxHueProperty);
        set => SetValue(MaxHueProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum value of the Saturation component in the range from 0..100.
    /// This property must be greater than <see cref="MinSaturation"/>.
    /// </summary>
    /// <remarks>
    /// Internally, the <see cref="ColorSpectrum"/> uses the HSV color model.
    /// </remarks>
    public int MaxSaturation
    {
        get => GetValue(MaxSaturationProperty);
        set => SetValue(MaxSaturationProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum value of the Value component in the range from 0..100.
    /// This property must be greater than <see cref="MinValue"/>.
    /// </summary>
    /// <remarks>
    /// Internally, the <see cref="ColorSpectrum"/> uses the HSV color model.
    /// </remarks>
    public int MaxValue
    {
        get => GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum value of the Hue component in the range from 0..359.
    /// This property must be less than <see cref="MaxHue"/>.
    /// </summary>
    /// <remarks>
    /// Internally, the <see cref="ColorSpectrum"/> uses the HSV color model.
    /// </remarks>
    public int MinHue
    {
        get => GetValue(MinHueProperty);
        set => SetValue(MinHueProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum value of the Saturation component in the range from 0..100.
    /// This property must be less than <see cref="MaxSaturation"/>.
    /// </summary>
    /// <remarks>
    /// Internally, the <see cref="ColorSpectrum"/> uses the HSV color model.
    /// </remarks>
    public int MinSaturation
    {
        get => GetValue(MinSaturationProperty);
        set => SetValue(MinSaturationProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum value of the Value component in the range from 0..100.
    /// This property must be less than <see cref="MaxValue"/>.
    /// </summary>
    /// <remarks>
    /// Internally, the <see cref="ColorSpectrum"/> uses the HSV color model.
    /// </remarks>
    public int MinValue
    {
        get => GetValue(MinValueProperty);
        set => SetValue(MinValueProperty, value);
    }

    /// <summary>
    /// Gets the third HSV color component that is NOT displayed by the spectrum.
    /// This is automatically calculated from the <see cref="Components"/> property.
    /// </summary>
    /// <remarks>
    /// This property should be used for any external color slider that represents the
    /// third component of the color. Note that this property uses the generic
    /// <see cref="ColorComponent"/> type instead of the more accurate <see cref="HsvComponent"/>
    /// to allow direct usage by the generalized color sliders.
    /// </remarks>
    public ColorComponent ThirdComponent
    {
        get => _thirdComponent;
        private set => SetAndRaise(ThirdComponentProperty, ref _thirdComponent, value);
    }

    /// <summary>
    /// Event for when the selected color changes within the spectrum.
    /// </summary>
    public event EventHandler<ColorChangedEventArgs>? ColorChanged;

    private bool _updatingColor = false;
    private bool _updatingHsvColor = false;
    private bool _isPointerPressed = false;
    private bool _shouldShowLargeSelection = false;
    private List<Hsv> _hsvValues = new List<Hsv>();
    private ColorComponent _thirdComponent = ColorComponent.Component3; // HsvComponent.Value

    private IDisposable? _layoutRootDisposable;
    private IDisposable? _selectionEllipsePanelDisposable;

    // XAML template parts
    private Panel? _layoutRoot;
    private Panel? _sizingPanel;
    private Rectangle? _spectrumRectangle;
    private Rectangle? _spectrumOverlayRectangle;
    private Canvas? _inputTarget;
    private Panel? _selectionEllipsePanel;

    // Put the spectrum images in a bitmap, which is then given to an ImageBrush.
    private Bitmap? _hueRedBitmap;
    private Bitmap? _hueYellowBitmap;
    private Bitmap? _hueGreenBitmap;
    private Bitmap? _hueCyanBitmap;
    private Bitmap? _hueBlueBitmap;
    private Bitmap? _huePurpleBitmap;

    private Bitmap? _saturationMinimumBitmap;
    private Bitmap? _saturationMaximumBitmap;

    private Bitmap? _valueBitmap;
    private Bitmap? _minBitmap;
    private Bitmap? _maxBitmap;

    // Fields used by UpdateEllipse() to ensure that it's using the data
    // associated with the last call to CreateBitmapsAndColorMap(),
    // in order to function properly while the asynchronous bitmap creation
    // is in progress.
    private ColorSpectrumComponents _componentsFromLastBitmapCreation = ColorSpectrumComponents.SaturationValue;
    private double _imageWidthFromLastBitmapCreation = 0.0;
    private double _imageHeightFromLastBitmapCreation = 0.0;
    private int _minHueFromLastBitmapCreation = 0;
    private int _maxHueFromLastBitmapCreation = 0;
    private int _minSaturationFromLastBitmapCreation = 0;
    private int _maxSaturationFromLastBitmapCreation = 0;
    private int _minValueFromLastBitmapCreation = 0;
    private int _maxValueFromLastBitmapCreation = 0;

    private Color _oldColor = Color.FromArgb(255, 255, 255, 255);
    private HsvColor _oldHsvColor = HsvColor.FromAhsv(0.0f, 0.0f, 1.0f, 1.0f);

    /// <summary>
    /// Initializes a new instance of the <see cref="ColorSpectrum"/> class.
    /// </summary>
    public ColorSpectrum()
    {
        _componentsFromLastBitmapCreation    = Components;
        _imageWidthFromLastBitmapCreation    = 0;
        _imageHeightFromLastBitmapCreation   = 0;
        _minHueFromLastBitmapCreation        = MinHue;
        _maxHueFromLastBitmapCreation        = MaxHue;
        _minSaturationFromLastBitmapCreation = MinSaturation;
        _maxSaturationFromLastBitmapCreation = MaxSaturation;
        _minValueFromLastBitmapCreation      = MinValue;
        _maxValueFromLastBitmapCreation      = MaxValue;
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        UnregisterEvents(); // Failsafe

        _inputTarget              = e.NameScope.Find<Canvas>(ColorSpectrumThemeConstants.InputTargetPart);
        _layoutRoot               = e.NameScope.Find<Panel>(ColorSpectrumThemeConstants.LayoutRootPart);
        _selectionEllipsePanel    = e.NameScope.Find<Panel>(ColorSpectrumThemeConstants.SelectionEllipsePanelPart);
        _sizingPanel              = e.NameScope.Find<Panel>(ColorSpectrumThemeConstants.SizingPanelPart);
        _spectrumRectangle        = e.NameScope.Find<Rectangle>(ColorSpectrumThemeConstants.SpectrumRectanglePart);
        _spectrumOverlayRectangle = e.NameScope.Find<Rectangle>(ColorSpectrumThemeConstants.SpectrumOverlayRectanglePart);

        if (_inputTarget != null)
        {
            _inputTarget.PointerEntered  += HandleInputTargetPointerEntered;
            _inputTarget.PointerExited   += HandleInputTargetPointerExited;
            _inputTarget.PointerPressed  += HandleInputTargetPointerPressed;
            _inputTarget.PointerMoved    += HandleInputTargetPointerMoved;
            _inputTarget.PointerReleased += HandleInputTargetPointerReleased;
        }

        if (_layoutRoot != null)
        {
            _layoutRootDisposable = _layoutRoot.GetObservable(BoundsProperty).Subscribe(_ => 
            {
                CreateBitmapsAndColorMap();
            });
        }

        if (_selectionEllipsePanel != null)
        {
            _selectionEllipsePanelDisposable = _selectionEllipsePanel.GetObservable(FlowDirectionProperty).Subscribe(_ => 
            {
                UpdateEllipse();
            });
        }

        if (_selectionEllipsePanel != null &&
            ColorHelper.ToDisplayNameExists)
        {
            ToolTip.SetTip(_selectionEllipsePanel, ColorHelper.ToDisplayName(Color));
        }

        // If we haven't yet created our bitmaps, do so now.
        if (_hsvValues.Count == 0)
        {
            CreateBitmapsAndColorMap();
        }

        UpdateEllipse();
        UpdatePseudoClasses();
    }

    /// <inheritdoc/>
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        // If the color was updated while this ColorSpectrum was not part of the visual tree,
        // the selection ellipse may be in an incorrect position. This is because the spectrum
        // renders based on layout scaling to avoid color banding; however, layout scale is only
        // available when the control is attached to the visual tree. The ColorSpectrum's color
        // may be updated from code-behind or from binding with another control when it's not
        // part of the visual tree.
        //
        //  See discussion: https://github.com/AvaloniaUI/Avalonia/discussions/9077
        //
        // To work-around this issue the selection ellipse is refreshed here.
        Dispatcher.UIThread.Post(UpdateEllipse);

        // OnAttachedToVisualTree is called after OnApplyTemplate so events cannot be connected here
    }
    

    /// <summary>
    /// Explicitly unregisters all events connected in OnApplyTemplate().
    /// </summary>
    private void UnregisterEvents()
    {
        _layoutRootDisposable?.Dispose();
        _layoutRootDisposable = null;

        _selectionEllipsePanelDisposable?.Dispose();
        _selectionEllipsePanelDisposable = null;

        if (_inputTarget != null)
        {
            _inputTarget.PointerEntered  -= HandleInputTargetPointerEntered;
            _inputTarget.PointerExited   -= HandleInputTargetPointerExited;
            _inputTarget.PointerPressed  -= HandleInputTargetPointerPressed;
            _inputTarget.PointerMoved    -= HandleInputTargetPointerMoved;
            _inputTarget.PointerReleased -= HandleInputTargetPointerReleased;
        }
    }

    /// <inheritdoc/>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        var key = e.Key;

        if (key != Key.Left &&
            key != Key.Right &&
            key != Key.Up &&
            key != Key.Down)
        {
            base.OnKeyDown(e);
            return;
        }

        bool isControlDown = e.KeyModifiers.HasFlag(KeyModifiers.Control);

        HsvComponent incrementComponent = HsvComponent.Hue;

        bool isSaturationValue = false;

        if (key == Key.Left || key == Key.Right)
        {
            switch (Components)
            {
                case ColorSpectrumComponents.HueSaturation:
                case ColorSpectrumComponents.HueValue:
                    incrementComponent = HsvComponent.Hue;
                    break;

                case ColorSpectrumComponents.SaturationValue:
                    isSaturationValue = true;
                    goto case ColorSpectrumComponents.SaturationHue;
                case ColorSpectrumComponents.SaturationHue:
                    incrementComponent = HsvComponent.Saturation;
                    break;

                case ColorSpectrumComponents.ValueHue:
                case ColorSpectrumComponents.ValueSaturation:
                    incrementComponent = HsvComponent.Value;
                    break;
            }
        }
        else if (key == Key.Up || key == Key.Down)
        {
            switch (Components)
            {
                case ColorSpectrumComponents.SaturationHue:
                case ColorSpectrumComponents.ValueHue:
                    incrementComponent = HsvComponent.Hue;
                    break;

                case ColorSpectrumComponents.HueSaturation:
                case ColorSpectrumComponents.ValueSaturation:
                    incrementComponent = HsvComponent.Saturation;
                    break;

                case ColorSpectrumComponents.SaturationValue:
                    isSaturationValue = true;
                    goto case ColorSpectrumComponents.HueValue;
                case ColorSpectrumComponents.HueValue:
                    incrementComponent = HsvComponent.Value;
                    break;
            }
        }

        double minBound = 0.0;
        double maxBound = 0.0;

        switch (incrementComponent)
        {
            case HsvComponent.Hue:
                minBound = MinHue;
                maxBound = MaxHue;
                break;

            case HsvComponent.Saturation:
                minBound = MinSaturation;
                maxBound = MaxSaturation;
                break;

            case HsvComponent.Value:
                minBound = MinValue;
                maxBound = MaxValue;
                break;
        }

        // The order of saturation and value in the spectrum is reversed - the max value is at the bottom while the min value is at the top -
        // so we want left and up to be lower for hue, but higher for saturation and value.
        // This will ensure that the icon always moves in the direction of the key press.
        IncrementDirection direction =
            (incrementComponent == HsvComponent.Hue && (key == Key.Left || key == Key.Up)) ||
            (incrementComponent != HsvComponent.Hue && (key == Key.Right || key == Key.Down)) ?
                IncrementDirection.Lower :
                IncrementDirection.Higher;

        // Image is flipped in RightToLeft, so we need to invert direction in that case.
        // The combination saturation and value is also flipped, so we need to invert in that case too.
        // If both are false, we don't need to invert.
        // If both are true, we would invert twice, so not invert at all.
        if ((FlowDirection == FlowDirection.RightToLeft) != isSaturationValue &&
            (key == Key.Left || key == Key.Right))
        {
            if (direction == IncrementDirection.Higher)
            {
                direction = IncrementDirection.Lower;
            }
            else
            {
                direction = IncrementDirection.Higher;
            }
        }

        IncrementAmount amount = isControlDown ? IncrementAmount.Large : IncrementAmount.Small;

        HsvColor hsvColor = HsvColor;
        UpdateColor(ColorPickerHelpers.IncrementColorComponent(
            new Hsv(hsvColor),
            incrementComponent,
            direction,
            amount,
            shouldWrap: true,
            minBound,
            maxBound));

        e.Handled = true;
    }

    /// <inheritdoc/>
    protected override void OnGotFocus(GotFocusEventArgs e)
    {
        // We only want to bother with the color name tool tip if we can provide color names.
        if (_selectionEllipsePanel != null &&
            ColorHelper.ToDisplayNameExists)
        {
            ToolTip.SetIsOpen(_selectionEllipsePanel, true);
        }

        UpdatePseudoClasses();

        base.OnGotFocus(e);
    }

    /// <inheritdoc/>
    protected override void OnLostFocus(RoutedEventArgs e)
    {
        // We only want to bother with the color name tool tip if we can provide color names.
        if (_selectionEllipsePanel != null &&
            ColorHelper.ToDisplayNameExists)
        {
            ToolTip.SetIsOpen(_selectionEllipsePanel, false);
        }

        UpdatePseudoClasses();

        base.OnLostFocus(e);
    }

    /// <inheritdoc/>
    protected override void OnPointerExited(PointerEventArgs e)
    {
        // We only want to bother with the color name tool tip if we can provide color names.
        if (_selectionEllipsePanel != null &&
            ColorHelper.ToDisplayNameExists)
        {
            ToolTip.SetIsOpen(_selectionEllipsePanel, false);
        }

        UpdatePseudoClasses();

        base.OnPointerExited(e);
    }

    /// <inheritdoc/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == ColorProperty)
        {
            // If we're in the process of internally updating the color,
            // then we don't want to respond to the Color property changing.
            if (!_updatingColor)
            {
                Color color = Color;

                _updatingHsvColor = true;
                Hsv newHsv = (new Rgb(color)).ToHsv();
                SetCurrentValue(HsvColorProperty, newHsv.ToHsvColor(color.A / 255.0));
                _updatingHsvColor = false;

                UpdateEllipse();
                UpdateBitmapSources();
            }

            _oldColor = change.GetOldValue<Color>();
        }
        else if (change.Property == HsvColorProperty)
        {
            // If we're in the process of internally updating the HSV color,
            // then we don't want to respond to the HsvColor property changing.
            if (!_updatingHsvColor)
            {
                SetColor();
            }

            _oldHsvColor = change.GetOldValue<HsvColor>();
        }
        else if (change.Property == MinHueProperty ||
                 change.Property == MaxHueProperty)
        {
            int minHue = MinHue;
            int maxHue = MaxHue;

            if (minHue < 0 || minHue > 359)
            {
                throw new ArgumentException("MinHue must be between 0 and 359.");
            }
            if (maxHue < 0 || maxHue > 359)
            {
                throw new ArgumentException("MaxHue must be between 0 and 359.");
            }

            ColorSpectrumComponents components = Components;

            // If hue is one of the axes in the spectrum bitmap, then we'll need to regenerate it
            // if the maximum or minimum value has changed.
            if (components != ColorSpectrumComponents.SaturationValue &&
                components != ColorSpectrumComponents.ValueSaturation)
            {
                CreateBitmapsAndColorMap();
            }
        }
        else if (change.Property == MinSaturationProperty ||
                 change.Property == MaxSaturationProperty)
        {
            int minSaturation = MinSaturation;
            int maxSaturation = MaxSaturation;

            if (minSaturation < 0 || minSaturation > 100)
            {
                throw new ArgumentException("MinSaturation must be between 0 and 100.");
            }
            if (maxSaturation < 0 || maxSaturation > 100)
            {
                throw new ArgumentException("MaxSaturation must be between 0 and 100.");
            }

            ColorSpectrumComponents components = Components;

            // If value is one of the axes in the spectrum bitmap, then we'll need to regenerate it
            // if the maximum or minimum value has changed.
            if (components != ColorSpectrumComponents.HueValue &&
                components != ColorSpectrumComponents.ValueHue)
            {
                CreateBitmapsAndColorMap();
            }
        }
        else if (change.Property == MinValueProperty ||
                 change.Property == MaxValueProperty)
        {
            int minValue = MinValue;
            int maxValue = MaxValue;

            if (minValue < 0 || minValue > 100)
            {
                throw new ArgumentException("MinValue must be between 0 and 100.");
            }
            if (maxValue < 0 || maxValue > 100)
            {
                throw new ArgumentException("MaxValue must be between 0 and 100.");
            }

            ColorSpectrumComponents components = Components;

            // If value is one of the axes in the spectrum bitmap, then we'll need to regenerate it
            // if the maximum or minimum value has changed.
            if (components != ColorSpectrumComponents.HueSaturation &&
                components != ColorSpectrumComponents.SaturationHue)
            {
                CreateBitmapsAndColorMap();
            }
        }
        else if (change.Property == ShapeProperty)
        {
            CreateBitmapsAndColorMap();
        }
        else if (change.Property == ComponentsProperty)
        {
            // Calculate and update the ThirdComponent value
            switch (Components)
            {
                case ColorSpectrumComponents.HueSaturation:
                case ColorSpectrumComponents.SaturationHue:
                    ThirdComponent = (ColorComponent)HsvComponent.Value;
                    break;
                case ColorSpectrumComponents.HueValue:
                case ColorSpectrumComponents.ValueHue:
                    ThirdComponent = (ColorComponent)HsvComponent.Saturation;
                    break;
                case ColorSpectrumComponents.SaturationValue:
                case ColorSpectrumComponents.ValueSaturation:
                    ThirdComponent = (ColorComponent)HsvComponent.Hue;
                    break;
            }

            CreateBitmapsAndColorMap();
        }

        base.OnPropertyChanged(change);
    }

    private void SetColor()
    {
        HsvColor hsvColor = HsvColor;

        _updatingColor = true;
        Rgb newRgb = (new Hsv(hsvColor)).ToRgb();

        SetCurrentValue(ColorProperty, newRgb.ToColor(hsvColor.A));

        _updatingColor = false;
        UpdateEllipse();
        UpdateBitmapSources();
        RaiseColorChanged();
    }

    public void RaiseColorChanged()
    {
        Color newColor = Color;

        bool colorChanged =
            _oldColor.A != newColor.A ||
            _oldColor.R != newColor.R ||
            _oldColor.G != newColor.G ||
            _oldColor.B != newColor.B;

        bool areBothColorsBlack =
            (_oldColor.R == newColor.R && newColor.R == 0) ||
            (_oldColor.G == newColor.G && newColor.G == 0) ||
            (_oldColor.B == newColor.B && newColor.B == 0);

        if (colorChanged || areBothColorsBlack)
        {
            var colorChangedEventArgs = new ColorChangedEventArgs(_oldColor, newColor);
            ColorChanged?.Invoke(this, colorChangedEventArgs);

            if (_selectionEllipsePanel != null &&
                ColorHelper.ToDisplayNameExists)
            {
                ToolTip.SetTip(_selectionEllipsePanel, ColorHelper.ToDisplayName(Color));
            }
        }
    }

    /// <summary>
    /// Updates the visual state of the control by applying latest PseudoClasses.
    /// </summary>
    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(ColorSpectrumPseudoClass.Pressed, _isPointerPressed);
        // Note: The ":pointerover" pseudo class is set in the base Control

        if (_isPointerPressed)
        {
            PseudoClasses.Set(ColorSpectrumPseudoClass.LargeSelector, _shouldShowLargeSelection);
        }
        else
        {
            PseudoClasses.Set(ColorSpectrumPseudoClass.LargeSelector, false);
        }

        if (SelectionEllipseShouldBeLight())
        {
            PseudoClasses.Set(ColorSpectrumPseudoClass.DarkSelector, false);
            PseudoClasses.Set(ColorSpectrumPseudoClass.LightSelector, true);
        }
        else
        {
            PseudoClasses.Set(ColorSpectrumPseudoClass.DarkSelector, true);
            PseudoClasses.Set(ColorSpectrumPseudoClass.LightSelector, false);
        }
    }

    /// <summary>
    /// Changes the currently selected color (always in HSV) and applies all necessary updates.
    /// </summary>
    /// <remarks>
    /// Some additional logic is applied in certain situations to coerce and sync color values.
    /// Use this method instead of update the <see cref="Color"/> or <see cref="HsvColor"/> directly.
    /// </remarks>
    /// <param name="newHsv">The new HSV color to change to.</param>
    private void UpdateColor(Hsv newHsv)
    {
        _updatingColor    = true;
        _updatingHsvColor = true;

        double alpha = HsvColor.A;

        // It is common for the ColorPicker (and therefore the Spectrum) to be initialized
        // with a #00000000 color value in some use cases. This is usually used to indicate
        // that no color has been selected by the user. Note that #00000000 is different than
        // #00FFFFFF (Transparent).
        //
        // In this situation, whenever the user clicks on the spectrum, the third
        // component and alpha component will remain zero. This is because the spectrum only
        // controls two components at any given time.
        //
        // This is very unintuitive from a user-standpoint as after the user clicks on the
        // spectrum they must then increase the alpha and then the third component sliders
        // to the desired value. In fact, until they increase these slider values no color
        // will show at all since it is fully transparent and black. In almost all cases
        // though the desired value is simply full color.
        //
        // To work around this usability issue with an initial #00000000 color, the selected
        // color is coerced into a color with maximum third component value and maximum alpha.
        // This can only happen here in the spectrum if those two components are already zero.
        //
        // In the past this coercion was restricted to occur only one time. However, when
        // ColorPicker controls are re-used or recycled #00000000 can be set multiple times.
        // Each time needs this special logic for usability so now anytime the color is
        // changed on the spectrum this logic will run.
        //
        // Also note this is NOT currently done for #00FFFFFF (Transparent) but based on
        // further usability study that case may need to be handled here as well. Right now
        // Transparent is treated as a normal color value with the alpha intentionally set
        // to zero so the alpha slider must still be adjusted after the spectrum.
        if (IsLoaded)
        {
            bool isAlphaComponentZero = (alpha == 0.0);
            bool isThirdComponentZero = false;

            switch (Components)
            {
                case ColorSpectrumComponents.HueValue:
                case ColorSpectrumComponents.ValueHue:
                    isThirdComponentZero = (newHsv.S == 0.0);
                    break;

                case ColorSpectrumComponents.HueSaturation:
                case ColorSpectrumComponents.SaturationHue:
                    isThirdComponentZero = (newHsv.V == 0.0);
                    break;

                case ColorSpectrumComponents.ValueSaturation:
                case ColorSpectrumComponents.SaturationValue:
                    isThirdComponentZero = (newHsv.H == 0.0);
                    break;
            }

            if (isAlphaComponentZero && isThirdComponentZero)
            {
                alpha = 1.0;

                switch (Components)
                {
                    case ColorSpectrumComponents.HueValue:
                    case ColorSpectrumComponents.ValueHue:
                        newHsv.S = 1.0;
                        break;

                    case ColorSpectrumComponents.HueSaturation:
                    case ColorSpectrumComponents.SaturationHue:
                        newHsv.V = 1.0;
                        break;

                    case ColorSpectrumComponents.ValueSaturation:
                    case ColorSpectrumComponents.SaturationValue:
                        // Hue is mathematically NOT a special case; however, is one conceptually.
                        // It doesn't make sense to change the selected Hue value, so why is it set here?
                        // Setting to 360.0 is equivalent to the max set for other components and is
                        // internally wrapped back to 0.0 (since 360 degrees = 0 degrees).
                        // This means effectively there is no change to the hue component value.
                        newHsv.H = 360.0;
                        break;
                }
            }
        }

        Rgb newRgb = newHsv.ToRgb();

        SetCurrentValue(ColorProperty, newRgb.ToColor(alpha));
        SetCurrentValue(HsvColorProperty, newHsv.ToHsvColor(alpha));

        UpdateEllipse();
        UpdatePseudoClasses();

        _updatingHsvColor = false;
        _updatingColor    = false;

        RaiseColorChanged();
    }

    /// <summary>
    /// Updates the selected <see cref="HsvColor"/> and <see cref="Color"/> based on a point within the color spectrum.
    /// </summary>
    /// <param name="point">The point on the spectrum representing the color.</param>
    private void UpdateColorFromPoint(PointerPoint point)
    {
        // If we haven't initialized our HSV value array yet, then we should just ignore any user input -
        // we don't yet know what to do with it.
        if (_hsvValues.Count == 0)
        {
            return;
        }

        // Remember the bitmap size follows physical device pixels
        var    scale              = LayoutHelper.GetLayoutScale(this);
        double xPosition          = point.Position.X * scale;
        double yPosition          = point.Position.Y * scale;

        // Now we need to find the index into the array of HSL values at each point in the spectrum m_image.
        int x     = (int)Math.Round(xPosition);
        int y     = (int)Math.Round(yPosition);
        int width = (int)Math.Round(_imageWidthFromLastBitmapCreation);

        if (x < 0)
        {
            x = 0;
        }
        else if (x >= _imageWidthFromLastBitmapCreation)
        {
            x = (int)Math.Round(_imageWidthFromLastBitmapCreation) - 1;
        }

        if (y < 0)
        {
            y = 0;
        }
        else if (y >= _imageHeightFromLastBitmapCreation)
        {
            y = (int)Math.Round(_imageHeightFromLastBitmapCreation) - 1;
        }

        // The gradient image contains two dimensions of HSL information, but not the third.
        // We should keep the third where it already was.
        // Note: This can sometimes cause a crash -- possibly due to differences in c# rounding. Therefore, index is now clamped.
        Hsv hsvAtPoint = _hsvValues[MathUtilities.Clamp((y * width + x), 0, _hsvValues.Count - 1)];

        var hsvColor = HsvColor;

        switch (Components)
        {
            case ColorSpectrumComponents.HueValue:
            case ColorSpectrumComponents.ValueHue:
                hsvAtPoint.S = hsvColor.S;
                break;

            case ColorSpectrumComponents.HueSaturation:
            case ColorSpectrumComponents.SaturationHue:
                hsvAtPoint.V = hsvColor.V;
                break;

            case ColorSpectrumComponents.ValueSaturation:
            case ColorSpectrumComponents.SaturationValue:
                hsvAtPoint.H = hsvColor.H;
                break;
        }

        UpdateColor(hsvAtPoint);
    }

    /// <summary>
    /// Updates the position of the selection ellipse on the spectrum which indicates the selected color.
    /// </summary>
    private void UpdateEllipse()
    {
        if (_selectionEllipsePanel == null)
        {
            return;
        }

        // If we don't have an image size yet, we shouldn't be showing the ellipse.
        if (_imageWidthFromLastBitmapCreation == 0 ||
            _imageHeightFromLastBitmapCreation == 0)
        {
            _selectionEllipsePanel.IsVisible = false;
            return;
        }
        _selectionEllipsePanel.IsVisible = true;

        double xPosition;
        double yPosition;

        Hsv hsvColor = new Hsv(HsvColor);

        hsvColor.H = MathUtilities.Clamp(hsvColor.H, _minHueFromLastBitmapCreation, _maxHueFromLastBitmapCreation);
        hsvColor.S = MathUtilities.Clamp(hsvColor.S, _minSaturationFromLastBitmapCreation / 100.0, _maxSaturationFromLastBitmapCreation / 100.0);
        hsvColor.V = MathUtilities.Clamp(hsvColor.V, _minValueFromLastBitmapCreation / 100.0, _maxValueFromLastBitmapCreation / 100.0);

        double xPercent = 0;
        double yPercent = 0;

        double hPercent = (hsvColor.H - _minHueFromLastBitmapCreation) / (_maxHueFromLastBitmapCreation - _minHueFromLastBitmapCreation);
        double sPercent = (hsvColor.S * 100.0 - _minSaturationFromLastBitmapCreation) / (_maxSaturationFromLastBitmapCreation - _minSaturationFromLastBitmapCreation);
        double vPercent = (hsvColor.V * 100.0 - _minValueFromLastBitmapCreation) / (_maxValueFromLastBitmapCreation - _minValueFromLastBitmapCreation);

        // In the case where saturation was an axis in the spectrum with hue, or value is an axis, full stop,
        // we inverted the direction of that axis in order to put more hue on the outside of the ring,
        // so we need to do similarly here when positioning the ellipse.
        if (_componentsFromLastBitmapCreation == ColorSpectrumComponents.HueSaturation ||
            _componentsFromLastBitmapCreation == ColorSpectrumComponents.SaturationHue)
        {
            sPercent = 1 - sPercent;
        }
        else
        {
            vPercent = 1 - vPercent;
        }

        switch (_componentsFromLastBitmapCreation)
        {
            case ColorSpectrumComponents.HueValue:
                xPercent = hPercent;
                yPercent = vPercent;
                break;

            case ColorSpectrumComponents.HueSaturation:
                xPercent = hPercent;
                yPercent = sPercent;
                break;

            case ColorSpectrumComponents.ValueHue:
                xPercent = vPercent;
                yPercent = hPercent;
                break;

            case ColorSpectrumComponents.ValueSaturation:
                xPercent = vPercent;
                yPercent = sPercent;
                break;

            case ColorSpectrumComponents.SaturationHue:
                xPercent = sPercent;
                yPercent = hPercent;
                break;

            case ColorSpectrumComponents.SaturationValue:
                xPercent = sPercent;
                yPercent = vPercent;
                break;
        }

        xPosition = _imageWidthFromLastBitmapCreation * xPercent;
        yPosition = _imageHeightFromLastBitmapCreation * yPercent;

        // Remember the bitmap size follows physical device pixels
        // Warning: LayoutHelper.GetLayoutScale() doesn't work unless the control is visible
        // This will not be true in all cases if the color is updated from another control or code-behind
        var scale = LayoutHelper.GetLayoutScale(this);
        Canvas.SetLeft(_selectionEllipsePanel, (xPosition / scale) - (_selectionEllipsePanel.Width / 2));
        Canvas.SetTop(_selectionEllipsePanel, (yPosition / scale) - (_selectionEllipsePanel.Height / 2));
        // We only want to bother with the color name tool tip if we can provide color names.
        if (IsFocused &&
            _selectionEllipsePanel != null &&
            ColorHelper.ToDisplayNameExists)
        {
            ToolTip.SetIsOpen(_selectionEllipsePanel, true);
        }
     
        UpdatePseudoClasses();
    }

    /// <inheritdoc cref="InputElement.PointerEntered"/>
    private void HandleInputTargetPointerEntered(object? sender, PointerEventArgs args)
    {
        UpdatePseudoClasses();
        args.Handled = true;
    }

    /// <inheritdoc cref="InputElement.PointerExited"/>
    private void HandleInputTargetPointerExited(object? sender, PointerEventArgs args)
    {
        UpdatePseudoClasses();
        args.Handled = true;
    }

    /// <inheritdoc cref="InputElement.PointerPressed"/>
    private void HandleInputTargetPointerPressed(object? sender, PointerPressedEventArgs args)
    {
        var inputTarget = _inputTarget;

        Focus();

        _isPointerPressed = true;
        _shouldShowLargeSelection =
            // TODO: After Pen PR is merged: https://github.com/AvaloniaUI/Avalonia/pull/7412
            // args.Pointer.Type == PointerType.Pen ||
            args.Pointer.Type == PointerType.Touch;

        args.Pointer.Capture(inputTarget);
        UpdateColorFromPoint(args.GetCurrentPoint(inputTarget));
        UpdatePseudoClasses();
        UpdateEllipse();

        args.Handled = true;
    }

    /// <inheritdoc cref="InputElement.PointerMoved"/>
    private void HandleInputTargetPointerMoved(object? sender, PointerEventArgs args)
    {
        if (!_isPointerPressed)
        {
            return;
        }

        UpdateColorFromPoint(args.GetCurrentPoint(_inputTarget));
        args.Handled = true;
    }

    /// <inheritdoc cref="InputElement.PointerReleased"/>
    private void HandleInputTargetPointerReleased(object? sender, PointerReleasedEventArgs args)
    {
        _isPointerPressed         = false;
        _shouldShowLargeSelection = false;

        args.Pointer.Capture(null);
        UpdatePseudoClasses();
        UpdateEllipse();

        args.Handled = true;
    }

    private async void CreateBitmapsAndColorMap()
    {
        if (_layoutRoot == null ||
            _sizingPanel == null ||
            _inputTarget == null ||
            _spectrumRectangle == null ||
            _spectrumOverlayRectangle == null
            /*|| SharedHelpers.IsInDesignMode*/)
        {
            return;
        }

        double width  = _layoutRoot.Bounds.Width;
        double height = _layoutRoot.Bounds.Height;

        if (width == 0 || height == 0)
        {
            return;
        }
        
        _sizingPanel.Width               = width;
        _sizingPanel.Height              = height;
        _inputTarget.Width               = width;
        _inputTarget.Height              = height;
        _spectrumRectangle.Width         = width;
        _spectrumRectangle.Height        = height;
        _spectrumOverlayRectangle.Width  = width;
        _spectrumOverlayRectangle.Height = height;

        HsvColor                hsvColor      = HsvColor;
        int                     minHue        = MinHue;
        int                     maxHue        = MaxHue;
        int                     minSaturation = MinSaturation;
        int                     maxSaturation = MaxSaturation;
        int                     minValue      = MinValue;
        int                     maxValue      = MaxValue;
        ColorSpectrumComponents components    = Components;

        // If min >= max, then by convention, min is the only number that a property can have.
        if (minHue >= maxHue)
        {
            maxHue = minHue;
        }

        if (minSaturation >= maxSaturation)
        {
            maxSaturation = minSaturation;
        }

        if (minValue >= maxValue)
        {
            maxValue = minValue;
        }

        Hsv hsv = new Hsv(hsvColor);
            
        // In Avalonia, Bounds returns the actual device-independent pixel size of a control.
        // However, this is not necessarily the size of the control rendered on a display.
        // A desktop or application scaling factor may be applied which must be accounted for here.
        // Remember bitmaps in Avalonia are rendered mapping to actual device pixels, not the device-
        // independent pixels of controls.
        var scale         = LayoutHelper.GetLayoutScale(this);
        int pixelWidth    = (int)Math.Round(width * scale);
        int pixelHeight   = (int)Math.Round(height * scale);
        var pixelCount    = pixelWidth * pixelHeight;
        var pixelDataSize = pixelCount * 4;
        // We'll only save pixel data for the middle bitmaps if our third dimension is hue.
        var middleBitmapsSize =
            components is ColorSpectrumComponents.ValueSaturation or ColorSpectrumComponents.SaturationValue
                ? pixelDataSize : 0;

        var       newHsvValues     = new List<Hsv>(pixelCount);
        using var bgraMinPixelData = new PooledList<byte>(pixelDataSize, ClearMode.Never);
        using var bgraMaxPixelData = new PooledList<byte>(pixelDataSize, ClearMode.Never);
        // The middle 4 are only needed and used in the case of hue as the third dimension.
        // Saturation and luminosity need only a min and max.
        using var bgraMiddle1PixelData = new PooledList<byte>(middleBitmapsSize, ClearMode.Never);
        using var bgraMiddle2PixelData = new PooledList<byte>(middleBitmapsSize, ClearMode.Never);
        using var bgraMiddle3PixelData = new PooledList<byte>(middleBitmapsSize, ClearMode.Never);
        using var bgraMiddle4PixelData = new PooledList<byte>(middleBitmapsSize, ClearMode.Never);

        await Task.Run(() =>
        {
            // As the user perceives it, every time the third dimension not represented in the ColorSpectrum changes,
            // the ColorSpectrum will visually change to accommodate that value.  For example, if the ColorSpectrum handles hue and luminosity,
            // and the saturation externally goes from 1.0 to 0.5, then the ColorSpectrum will visually change to look more washed out
            // to represent that third dimension's new value.
            // Internally, however, we don't want to regenerate the ColorSpectrum bitmap every single time this happens, since that's very expensive.
            // In order to make it so that we don't have to, we implement an optimization where, rather than having only one bitmap,
            // we instead have multiple that we blend together using opacity to create the effect that we want.
            // In the case where the third dimension is saturation or luminosity, we only need two: one bitmap at the minimum value
            // of the third dimension, and one bitmap at the maximum.  Then we set the second's opacity at whatever the value of
            // the third dimension is - e.g., a saturation of 0.5 implies an opacity of 50%.
            // In the case where the third dimension is hue, we need six: one bitmap corresponding to red, yellow, green, cyan, blue, and purple.
            // We'll then blend between whichever colors our hue exists between - e.g., an orange color would use red and yellow with an opacity of 50%.
            // This optimization does incur slightly more startup time initially since we have to generate multiple bitmaps at once instead of only one,
            // but the running time savings after that are *huge* when we can just set an opacity instead of generating a brand new bitmap.
            for (int y = 0; y < pixelHeight; ++y)
            {
                for (int x = 0; x < pixelWidth; ++x)
                {
                    FillPixelForBox(
                        x, y, hsv, pixelWidth, pixelHeight, components, minHue, maxHue, minSaturation, maxSaturation, minValue, maxValue,
                        bgraMinPixelData, bgraMiddle1PixelData, bgraMiddle2PixelData, bgraMiddle3PixelData, bgraMiddle4PixelData, bgraMaxPixelData,
                        newHsvValues);
                }
            }
        });

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            ColorSpectrumComponents components2 = Components;
            _minBitmap = ColorPickerHelpers.CreateBitmapFromPixelData(bgraMinPixelData, pixelWidth, pixelHeight);
            _maxBitmap = ColorPickerHelpers.CreateBitmapFromPixelData(bgraMaxPixelData, pixelWidth, pixelHeight);

            switch (components2)
            {
                case ColorSpectrumComponents.HueValue:
                case ColorSpectrumComponents.ValueHue:
                    _saturationMinimumBitmap?.Dispose();
                    _saturationMinimumBitmap = _minBitmap;
                    _saturationMaximumBitmap?.Dispose();
                    _saturationMaximumBitmap = _maxBitmap;
                    break;
                case ColorSpectrumComponents.HueSaturation:
                case ColorSpectrumComponents.SaturationHue:
                    _valueBitmap?.Dispose();
                    _valueBitmap = _maxBitmap;
                    break;
                case ColorSpectrumComponents.ValueSaturation:
                case ColorSpectrumComponents.SaturationValue:
                    _hueRedBitmap?.Dispose();
                    _hueRedBitmap = _minBitmap;
                    _hueYellowBitmap?.Dispose();
                    _hueYellowBitmap = ColorPickerHelpers.CreateBitmapFromPixelData(bgraMiddle1PixelData, pixelWidth, pixelHeight);
                    _hueGreenBitmap?.Dispose();
                    _hueGreenBitmap = ColorPickerHelpers.CreateBitmapFromPixelData(bgraMiddle2PixelData, pixelWidth, pixelHeight);
                    _hueCyanBitmap?.Dispose();
                    _hueCyanBitmap = ColorPickerHelpers.CreateBitmapFromPixelData(bgraMiddle3PixelData, pixelWidth, pixelHeight);
                    _hueBlueBitmap?.Dispose();
                    _hueBlueBitmap = ColorPickerHelpers.CreateBitmapFromPixelData(bgraMiddle4PixelData, pixelWidth, pixelHeight);
                    _huePurpleBitmap?.Dispose();
                    _huePurpleBitmap = _maxBitmap;
                    break;
            }
            
            _componentsFromLastBitmapCreation    = Components;
            _imageWidthFromLastBitmapCreation    = pixelWidth;
            _imageHeightFromLastBitmapCreation   = pixelHeight;
            _minHueFromLastBitmapCreation        = MinHue;
            _maxHueFromLastBitmapCreation        = MaxHue;
            _minSaturationFromLastBitmapCreation = MinSaturation;
            _maxSaturationFromLastBitmapCreation = MaxSaturation;
            _minValueFromLastBitmapCreation      = MinValue;
            _maxValueFromLastBitmapCreation      = MaxValue;

            _hsvValues = newHsvValues;

            UpdateBitmapSources();
            UpdateEllipse();
        });
    }

    private static void FillPixelForBox(
        double x,
        double y,
        Hsv baseHsv,
        int pixelWidth,
        int pixelHeight,
        ColorSpectrumComponents components,
        double minHue,
        double maxHue,
        double minSaturation,
        double maxSaturation,
        double minValue,
        double maxValue,
        PooledList<byte> bgraMinPixelData,
        PooledList<byte> bgraMiddle1PixelData,
        PooledList<byte> bgraMiddle2PixelData,
        PooledList<byte> bgraMiddle3PixelData,
        PooledList<byte> bgraMiddle4PixelData,
        PooledList<byte> bgraMaxPixelData,
        List<Hsv> newHsvValues)
    {
        double hMin = minHue;
        double hMax = maxHue;
        double sMin = minSaturation / 100.0;
        double sMax = maxSaturation / 100.0;
        double vMin = minValue / 100.0;
        double vMax = maxValue / 100.0;

        Hsv hsvMin     = baseHsv;
        Hsv hsvMiddle1 = baseHsv;
        Hsv hsvMiddle2 = baseHsv;
        Hsv hsvMiddle3 = baseHsv;
        Hsv hsvMiddle4 = baseHsv;
        Hsv hsvMax     = baseHsv;

        double xPercent = x / (pixelWidth - 1);
        double yPercent = y / (pixelHeight - 1);

        switch (components)
        {
            case ColorSpectrumComponents.HueValue:
                hsvMin.H = hsvMiddle1.H = hsvMiddle2.H = hsvMiddle3.H = hsvMiddle4.H = hsvMax.H = hMin + xPercent * (hMax - hMin);
                hsvMin.V = hsvMiddle1.V = hsvMiddle2.V = hsvMiddle3.V = hsvMiddle4.V = hsvMax.V = vMin + yPercent * (vMax - vMin);
                hsvMin.S = 0;
                hsvMax.S = 1;
                break;

            case ColorSpectrumComponents.HueSaturation:
                hsvMin.H = hsvMiddle1.H = hsvMiddle2.H = hsvMiddle3.H = hsvMiddle4.H = hsvMax.H = hMin + xPercent * (hMax - hMin);
                hsvMin.S = hsvMiddle1.S = hsvMiddle2.S = hsvMiddle3.S = hsvMiddle4.S = hsvMax.S = sMin + yPercent * (sMax - sMin);
                hsvMin.V = 0;
                hsvMax.V = 1;
                break;

            case ColorSpectrumComponents.ValueHue:
                hsvMin.V = hsvMiddle1.V = hsvMiddle2.V = hsvMiddle3.V = hsvMiddle4.V = hsvMax.V = vMin + xPercent * (vMax - vMin);
                hsvMin.H = hsvMiddle1.H = hsvMiddle2.H = hsvMiddle3.H = hsvMiddle4.H = hsvMax.H = hMin + yPercent * (hMax - hMin);
                hsvMin.S = 0;
                hsvMax.S = 1;
                break;

            case ColorSpectrumComponents.ValueSaturation:
                hsvMin.V = hsvMiddle1.V = hsvMiddle2.V = hsvMiddle3.V = hsvMiddle4.V = hsvMax.V = vMin + xPercent * (vMax - vMin);
                hsvMin.S = hsvMiddle1.S = hsvMiddle2.S = hsvMiddle3.S = hsvMiddle4.S = hsvMax.S = sMin + yPercent * (sMax - sMin);
                hsvMin.H = 0;
                hsvMiddle1.H = 60;
                hsvMiddle2.H = 120;
                hsvMiddle3.H = 180;
                hsvMiddle4.H = 240;
                hsvMax.H = 300;
                break;

            case ColorSpectrumComponents.SaturationHue:
                hsvMin.S = hsvMiddle1.S = hsvMiddle2.S = hsvMiddle3.S = hsvMiddle4.S = hsvMax.S = sMin + xPercent * (sMax - sMin);
                hsvMin.H = hsvMiddle1.H = hsvMiddle2.H = hsvMiddle3.H = hsvMiddle4.H = hsvMax.H = hMin + yPercent * (hMax - hMin);
                hsvMin.V = 0;
                hsvMax.V = 1;
                break;

            case ColorSpectrumComponents.SaturationValue:
                hsvMin.S = hsvMiddle1.S = hsvMiddle2.S = hsvMiddle3.S = hsvMiddle4.S = hsvMax.S = sMin + xPercent * (sMax - sMin);
                hsvMin.V = hsvMiddle1.V = hsvMiddle2.V = hsvMiddle3.V = hsvMiddle4.V = hsvMax.V = vMin + yPercent * (vMax - vMin);
                hsvMin.H = 0;
                hsvMiddle1.H = 60;
                hsvMiddle2.H = 120;
                hsvMiddle3.H = 180;
                hsvMiddle4.H = 240;
                hsvMax.H = 300;
                break;
        }

        // If saturation is an axis in the spectrum with hue, or value is an axis, then we want
        // that axis to go from maximum at the top to minimum at the bottom,
        // or maximum at the outside to minimum at the inside in the case of the ring configuration,
        // so we'll invert the number before assigning the HSL value to the array.
        // Otherwise, we'll have a very narrow section in the middle that actually has meaningful hue
        // in the case of the ring configuration.
        if (components == ColorSpectrumComponents.HueSaturation ||
            components == ColorSpectrumComponents.SaturationHue)
        {
            hsvMin.S     = sMax - hsvMin.S + sMin;
            hsvMiddle1.S = sMax - hsvMiddle1.S + sMin;
            hsvMiddle2.S = sMax - hsvMiddle2.S + sMin;
            hsvMiddle3.S = sMax - hsvMiddle3.S + sMin;
            hsvMiddle4.S = sMax - hsvMiddle4.S + sMin;
            hsvMax.S     = sMax - hsvMax.S + sMin;
        }
        else
        {
            hsvMin.V     = vMax - hsvMin.V + vMin;
            hsvMiddle1.V = vMax - hsvMiddle1.V + vMin;
            hsvMiddle2.V = vMax - hsvMiddle2.V + vMin;
            hsvMiddle3.V = vMax - hsvMiddle3.V + vMin;
            hsvMiddle4.V = vMax - hsvMiddle4.V + vMin;
            hsvMax.V     = vMax - hsvMax.V + vMin;
        }

        newHsvValues.Add(hsvMin);

        Rgb rgbMin = hsvMin.ToRgb();
        bgraMinPixelData.Add((byte)Math.Round(rgbMin.B * 255.0)); // b
        bgraMinPixelData.Add((byte)Math.Round(rgbMin.G * 255.0)); // g
        bgraMinPixelData.Add((byte)Math.Round(rgbMin.R * 255.0)); // r
        bgraMinPixelData.Add(255); // a - ignored

        // We'll only save pixel data for the middle bitmaps if our third dimension is hue.
        if (components == ColorSpectrumComponents.ValueSaturation ||
            components == ColorSpectrumComponents.SaturationValue)
        {
            Rgb rgbMiddle1 = hsvMiddle1.ToRgb();
            bgraMiddle1PixelData.Add((byte)Math.Round(rgbMiddle1.B * 255.0)); // b
            bgraMiddle1PixelData.Add((byte)Math.Round(rgbMiddle1.G * 255.0)); // g
            bgraMiddle1PixelData.Add((byte)Math.Round(rgbMiddle1.R * 255.0)); // r
            bgraMiddle1PixelData.Add(255); // a - ignored

            Rgb rgbMiddle2 = hsvMiddle2.ToRgb();
            bgraMiddle2PixelData.Add((byte)Math.Round(rgbMiddle2.B * 255.0)); // b
            bgraMiddle2PixelData.Add((byte)Math.Round(rgbMiddle2.G * 255.0)); // g
            bgraMiddle2PixelData.Add((byte)Math.Round(rgbMiddle2.R * 255.0)); // r
            bgraMiddle2PixelData.Add(255); // a - ignored

            Rgb rgbMiddle3 = hsvMiddle3.ToRgb();
            bgraMiddle3PixelData.Add((byte)Math.Round(rgbMiddle3.B * 255.0)); // b
            bgraMiddle3PixelData.Add((byte)Math.Round(rgbMiddle3.G * 255.0)); // g
            bgraMiddle3PixelData.Add((byte)Math.Round(rgbMiddle3.R * 255.0)); // r
            bgraMiddle3PixelData.Add(255); // a - ignored

            Rgb rgbMiddle4 = hsvMiddle4.ToRgb();
            bgraMiddle4PixelData.Add((byte)Math.Round(rgbMiddle4.B * 255.0)); // b
            bgraMiddle4PixelData.Add((byte)Math.Round(rgbMiddle4.G * 255.0)); // g
            bgraMiddle4PixelData.Add((byte)Math.Round(rgbMiddle4.R * 255.0)); // r
            bgraMiddle4PixelData.Add(255); // a - ignored
        }

        Rgb rgbMax = hsvMax.ToRgb();
        bgraMaxPixelData.Add((byte)Math.Round(rgbMax.B * 255.0)); // b
        bgraMaxPixelData.Add((byte)Math.Round(rgbMax.G * 255.0)); // g
        bgraMaxPixelData.Add((byte)Math.Round(rgbMax.R * 255.0)); // r
        bgraMaxPixelData.Add(255); // a - ignored
    }

    private void UpdateBitmapSources()
    {
        if (_spectrumOverlayRectangle == null ||
            _spectrumRectangle == null)
        {
            return;
        }
 
        HsvColor                hsvColor   = HsvColor;
        ColorSpectrumComponents components = Components;

        // We'll set the base image and the overlay image based on which component is our third dimension.
        // If it's saturation or luminosity, then the base image is that dimension at its minimum value,
        // while the overlay image is that dimension at its maximum value.
        // If it's hue, then we'll figure out where in the color wheel we are, and then use the two
        // colors on either side of our position as our base image and overlay image.
        // For example, if our hue is orange, then the base image would be red and the overlay image yellow.
        switch (components)
        {
            case ColorSpectrumComponents.HueValue:
            case ColorSpectrumComponents.ValueHue:
            {
                if (_saturationMinimumBitmap == null ||
                    _saturationMaximumBitmap == null)
                {
                    return;
                }

                ImageBrush spectrumBrush        = new ImageBrush(_saturationMinimumBitmap);
                ImageBrush spectrumOverlayBrush = new ImageBrush(_saturationMaximumBitmap);

                _spectrumOverlayRectangle.Opacity = hsvColor.S;
                _spectrumRectangle.Fill           = spectrumBrush;
                _spectrumOverlayRectangle.Fill    = spectrumOverlayBrush;
                _spectrumOverlayRectangle.Fill    = spectrumOverlayBrush;
            }
                break;

            case ColorSpectrumComponents.HueSaturation:
            case ColorSpectrumComponents.SaturationHue:
            {
                if (_valueBitmap == null)
                {
                    return;
                }

                ImageBrush spectrumBrush        = new ImageBrush(_valueBitmap);
                ImageBrush spectrumOverlayBrush = new ImageBrush(_valueBitmap);

                _spectrumOverlayRectangle.Opacity = 1.0;
                _spectrumRectangle.Fill           = spectrumBrush;
                _spectrumOverlayRectangle.Fill    = spectrumOverlayBrush;
                _spectrumOverlayRectangle.Fill    = spectrumOverlayBrush;
            }
                break;

            case ColorSpectrumComponents.ValueSaturation:
            case ColorSpectrumComponents.SaturationValue:
            {
                if (_hueRedBitmap == null ||
                    _hueYellowBitmap == null ||
                    _hueGreenBitmap == null ||
                    _hueCyanBitmap == null ||
                    _hueBlueBitmap == null ||
                    _huePurpleBitmap == null)
                {
                    return;
                }

                ImageBrush spectrumBrush;
                ImageBrush spectrumOverlayBrush;

                double sextant = hsvColor.H / 60.0;

                if (sextant < 1)
                {
                    spectrumBrush        = new ImageBrush(_hueRedBitmap);
                    spectrumOverlayBrush = new ImageBrush(_hueYellowBitmap);
                }
                else if (sextant >= 1 && sextant < 2)
                {
                    spectrumBrush        = new ImageBrush(_hueYellowBitmap);
                    spectrumOverlayBrush = new ImageBrush(_hueGreenBitmap);
                }
                else if (sextant >= 2 && sextant < 3)
                {
                    spectrumBrush        = new ImageBrush(_hueGreenBitmap);
                    spectrumOverlayBrush = new ImageBrush(_hueCyanBitmap);
                }
                else if (sextant >= 3 && sextant < 4)
                {
                    spectrumBrush        = new ImageBrush(_hueCyanBitmap);
                    spectrumOverlayBrush = new ImageBrush(_hueBlueBitmap);
                }
                else if (sextant >= 4 && sextant < 5)
                {
                    spectrumBrush        = new ImageBrush(_hueBlueBitmap);
                    spectrumOverlayBrush = new ImageBrush(_huePurpleBitmap);
                }
                else
                {
                    spectrumBrush        = new ImageBrush(_huePurpleBitmap);
                    spectrumOverlayBrush = new ImageBrush(_hueRedBitmap);
                }

                _spectrumOverlayRectangle.Opacity = sextant - (int)sextant;
                _spectrumRectangle.Fill           = spectrumBrush;
                _spectrumOverlayRectangle.Fill    = spectrumOverlayBrush;
                _spectrumOverlayRectangle.Fill    = spectrumOverlayBrush;
            }
                break;
        }
    }

    /// <summary>
    /// Determines whether the selection ellipse should be light based on the relative
    /// luminance of the selected color.
    /// </summary>
    private bool SelectionEllipseShouldBeLight()
    {
        // The selection ellipse should be light if and only if the chosen color
        // contrasts more with black than it does with white.
        // To find how much something contrasts with white, we use the equation
        // for relative luminance.
        //
        // If the third component is value, then we won't be updating the spectrum's displayed colors,
        // so in that case we should use a value of 1 when considering the backdrop
        // for the selection ellipse.
        Color displayedColor;

        if (Components == ColorSpectrumComponents.HueSaturation ||
            Components == ColorSpectrumComponents.SaturationHue)
        {
            HsvColor hsvColor = HsvColor;
            Rgb      color    = (new Hsv(hsvColor.H, hsvColor.S, 1.0)).ToRgb();
            displayedColor = color.ToColor(hsvColor.A);
        }
        else
        {
            displayedColor = Color;
        }

        var lum = ColorHelper.GetRelativeLuminance(displayedColor);

        return lum <= 0.5;
    }
}