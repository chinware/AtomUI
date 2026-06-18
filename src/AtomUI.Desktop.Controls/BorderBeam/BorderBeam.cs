using System.Collections.Specialized;
using AtomUI.Controls;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

public class BorderBeam : ContentControl,
                          IMotionAwareControl
{
    #region 公共属性定义

    public static readonly StyledProperty<Color?> ColorProperty =
        AvaloniaProperty.Register<BorderBeam, Color?>(nameof(Color));

    public static readonly DirectProperty<BorderBeam, AvaloniaList<BorderBeamColorStop>> ColorStopsProperty =
        AvaloniaProperty.RegisterDirect<BorderBeam, AvaloniaList<BorderBeamColorStop>>(
            nameof(ColorStops),
            o => o.ColorStops,
            (o, v) => o.ColorStops = v);

    public static readonly StyledProperty<Thickness?> OutsetProperty =
        AvaloniaProperty.Register<BorderBeam, Thickness?>(nameof(Outset));

    public new static readonly StyledProperty<Thickness> BorderThicknessProperty =
        Border.BorderThicknessProperty.AddOwner<BorderBeam>();

    public new static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
        Border.CornerRadiusProperty.AddOwner<BorderBeam>();

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<BorderBeam>(
            new StyledPropertyMetadata<bool>(true, BindingMode.OneWay));

    public static readonly StyledProperty<TimeSpan> DurationProperty =
        AvaloniaProperty.Register<BorderBeam, TimeSpan>(nameof(Duration), TimeSpan.FromSeconds(6));

    public static readonly StyledProperty<double> BeamSizeProperty =
        AvaloniaProperty.Register<BorderBeam, double>(nameof(BeamSize), 100d);

    public Color? Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    private AvaloniaList<BorderBeamColorStop> _colorStops = new();

    public AvaloniaList<BorderBeamColorStop> ColorStops
    {
        get => _colorStops;
        set
        {
            value ??= new AvaloniaList<BorderBeamColorStop>();
            if (ReferenceEquals(_colorStops, value))
            {
                return;
            }

            _colorStops.CollectionChanged -= HandleColorStopsChanged;
            SetAndRaise(ColorStopsProperty, ref _colorStops, value);
            _colorStops.CollectionChanged += HandleColorStopsChanged;
        }
    }

    public Thickness? Outset
    {
        get => GetValue(OutsetProperty);
        set => SetValue(OutsetProperty, value);
    }

    public new Thickness BorderThickness
    {
        get => GetValue(BorderThicknessProperty);
        set => SetValue(BorderThicknessProperty, value);
    }

    public new CornerRadius CornerRadius
    {
        get => GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public TimeSpan Duration
    {
        get => GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    public double BeamSize
    {
        get => GetValue(BeamSizeProperty);
        set => SetValue(BeamSizeProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<BorderBeam, BorderBeamGeometry> EffectiveBorderBeamGeometryProperty =
        AvaloniaProperty.RegisterDirect<BorderBeam, BorderBeamGeometry>(
            nameof(EffectiveBorderBeamGeometry),
            o => o.EffectiveBorderBeamGeometry,
            (o, v) => o.EffectiveBorderBeamGeometry = v);

    internal static readonly StyledProperty<IBrush?> DefaultStartColorProperty =
        AvaloniaProperty.Register<BorderBeam, IBrush?>(nameof(DefaultStartColor));

    internal static readonly StyledProperty<IBrush?> DefaultEndColorProperty =
        AvaloniaProperty.Register<BorderBeam, IBrush?>(nameof(DefaultEndColor));

    internal static readonly StyledProperty<double> BeamOpacityProperty =
        AvaloniaProperty.Register<BorderBeam, double>(nameof(BeamOpacity), 0.95d);

    internal static readonly StyledProperty<double> MaxVisibleStopPercentProperty =
        AvaloniaProperty.Register<BorderBeam, double>(nameof(MaxVisibleStopPercent), 70d);

    private BorderBeamGeometry _effectiveBorderBeamGeometry;

    internal BorderBeamGeometry EffectiveBorderBeamGeometry
    {
        get => _effectiveBorderBeamGeometry;
        set => SetAndRaise(EffectiveBorderBeamGeometryProperty, ref _effectiveBorderBeamGeometry, value);
    }

    internal IBrush? DefaultStartColor
    {
        get => GetValue(DefaultStartColorProperty);
        set => SetValue(DefaultStartColorProperty, value);
    }

    internal IBrush? DefaultEndColor
    {
        get => GetValue(DefaultEndColorProperty);
        set => SetValue(DefaultEndColorProperty, value);
    }

    internal double BeamOpacity
    {
        get => GetValue(BeamOpacityProperty);
        set => SetValue(BeamOpacityProperty, value);
    }

    internal double MaxVisibleStopPercent
    {
        get => GetValue(MaxVisibleStopPercentProperty);
        set => SetValue(MaxVisibleStopPercentProperty, value);
    }

    #endregion

    private IBorderBeamAwareControl? _awareContent;

    static BorderBeam()
    {
        AffectsRender<BorderBeam>(
            ColorProperty,
            ColorStopsProperty,
            OutsetProperty,
            BorderThicknessProperty,
            CornerRadiusProperty,
            IsMotionEnabledProperty,
            DurationProperty,
            BeamSizeProperty);
    }

    public BorderBeam()
    {
        this.RegisterTokenResourceScope(BorderBeamToken.ScopeProvider);
        _colorStops.CollectionChanged += HandleColorStopsChanged;
        ConfigureEffectiveBorderBeamGeometry();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        ConfigureAwareContent(Content);
        ConfigureEffectiveBorderBeamGeometry();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        ConfigureAwareContent(null);
        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ContentProperty)
        {
            ConfigureAwareContent(change.NewValue);
            ConfigureEffectiveBorderBeamGeometry();
        }
        else if (change.Property == BorderThicknessProperty ||
                 change.Property == CornerRadiusProperty)
        {
            ConfigureEffectiveBorderBeamGeometry();
        }
    }

    private void ConfigureAwareContent(object? content)
    {
        if (ReferenceEquals(_awareContent, content))
        {
            return;
        }

        if (_awareContent is not null)
        {
            _awareContent.BorderBeamGeometryChanged -= HandleAwareContentGeometryChanged;
        }

        _awareContent = content as IBorderBeamAwareControl;
        if (_awareContent is not null)
        {
            _awareContent.BorderBeamGeometryChanged += HandleAwareContentGeometryChanged;
        }
    }

    private void ConfigureEffectiveBorderBeamGeometry()
    {
        EffectiveBorderBeamGeometry = _awareContent?.GetBorderBeamGeometry()
                                      ?? new BorderBeamGeometry(BorderThickness, CornerRadius);
    }

    private void HandleAwareContentGeometryChanged(object? sender, EventArgs e)
    {
        ConfigureEffectiveBorderBeamGeometry();
    }

    private void HandleColorStopsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RaisePropertyChanged(ColorStopsProperty, ColorStops, ColorStops);
    }
}
