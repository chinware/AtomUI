using AtomUI.Controls.Utils;
using AtomUI.Media;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;

namespace AtomUI.Controls;

internal class CheckBoxIndicator : Control, IWaveAdornerInfoProvider
{
    public static readonly StyledProperty<bool?> IsCheckedProperty = 
        ToggleButton.IsCheckedProperty.AddOwner<CheckBoxIndicator>();
    
    public static readonly StyledProperty<double> SizeProperty =
        AvaloniaProperty.Register<CheckBoxIndicator, double>(nameof(Size));
    
    public static readonly StyledProperty<IBrush?> BorderBrushProperty =
        Border.BorderBrushProperty.AddOwner<CheckBoxIndicator>();

    public static readonly StyledProperty<IBrush?> CheckedMarkBrushProperty =
        AvaloniaProperty.Register<CheckBoxIndicator, IBrush?>(nameof(CheckedMarkBrush));

    public static readonly StyledProperty<double> CheckedMarkEffectSizeProperty =
        AvaloniaProperty.Register<CheckBoxIndicator, double>(nameof(CheckedMarkEffectSize));

    public static readonly StyledProperty<IBrush?> TristateMarkBrushProperty =
        AvaloniaProperty.Register<CheckBoxIndicator, IBrush?>(nameof(TristateMarkBrush));

    public static readonly StyledProperty<double> TristateMarkSizeProperty =
        AvaloniaProperty.Register<CheckBoxIndicator, double>(nameof(TristateMarkSize));

    public static readonly StyledProperty<IBrush?> BackgroundProperty =
        Border.BackgroundProperty.AddOwner<CheckBoxIndicator>();

    public static readonly StyledProperty<Thickness> BorderThicknessProperty =
        Border.BorderThicknessProperty.AddOwner<CheckBoxIndicator>();

    public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
        Border.CornerRadiusProperty.AddOwner<CheckBoxIndicator>();
    
    public bool? IsChecked
    {
        get => GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    public double Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }
    
    public IBrush? BorderBrush
    {
        get => GetValue(BorderBrushProperty);
        set => SetValue(BorderBrushProperty, value);
    }

    public IBrush? CheckedMarkBrush
    {
        get => GetValue(CheckedMarkBrushProperty);
        set => SetValue(CheckedMarkBrushProperty, value);
    }

    public double CheckedMarkEffectSize
    {
        get => GetValue(CheckedMarkEffectSizeProperty);
        set => SetValue(CheckedMarkEffectSizeProperty, value);
    }

    public IBrush? TristateMarkBrush
    {
        get => GetValue(TristateMarkBrushProperty);
        set => SetValue(TristateMarkBrushProperty, value);
    }

    public double TristateMarkSize
    {
        get => GetValue(TristateMarkSizeProperty);
        set => SetValue(TristateMarkSizeProperty, value);
    }

    public IBrush? Background
    {
        get => GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    public Thickness BorderThickness
    {
        get => GetValue(BorderThicknessProperty);
        set => SetValue(BorderThicknessProperty, value);
    }

    public CornerRadius CornerRadius
    {
        get => GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    private ControlStyleState _styleState;
    private readonly BorderRenderHelper _borderRenderHelper;

    static CheckBoxIndicator()
    {
        AffectsRender<CheckBoxIndicator>(
            IsCheckedProperty,
            CheckedMarkEffectSizeProperty,
            BorderBrushProperty,
            CheckedMarkBrushProperty,
            TristateMarkBrushProperty,
            BackgroundProperty,
            BorderThicknessProperty,
            CornerRadiusProperty);
    }

    public CheckBoxIndicator()
    {
        _borderRenderHelper = new BorderRenderHelper();
    }

    public override void ApplyTemplate()
    {
        base.ApplyTemplate();
        Transitions ??= new Transitions
        {
            AnimationUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty),
            AnimationUtils.CreateTransition<SolidColorBrushTransition>(BorderBrushProperty),
            AnimationUtils.CreateTransition<SolidColorBrushTransition>(TristateMarkBrushProperty),
            AnimationUtils.CreateTransition<DoubleTransition>(CheckedMarkEffectSizeProperty,
                SharedTokenKey.MotionDurationMid, new BackEaseOut())
        };
        TokenResourceBinder.CreateSharedResourceBinding(this, BorderThicknessProperty,
            SharedTokenKey.BorderThickness, BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this));
    }

    private void CollectStyleState()
    {
        ControlStateUtils.InitCommonState(this, ref _styleState);
        switch (IsChecked)
        {
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
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == IsPointerOverProperty ||
            e.Property == IsCheckedProperty ||
            e.Property == IsEnabledProperty)
        {
            CollectStyleState();
            SetupIndicatorCheckedMarkEffectSize();
            if (e.Property == IsCheckedProperty &&
                _styleState.HasFlag(ControlStyleState.Enabled) &&
                _styleState.HasFlag(ControlStyleState.On))
            {
                WaveSpiritAdorner.ShowWaveAdorner(this, WaveType.RoundRectWave);
            }
        }

        if (e.Property == SizeProperty)
        {
            CollectStyleState();
            SetupIndicatorCheckedMarkEffectSize();
        }
    }
    
    private void SetupIndicatorCheckedMarkEffectSize()
    {
        if (_styleState.HasFlag(ControlStyleState.Enabled))
        {
            if (_styleState.HasFlag(ControlStyleState.On))
            {
                CheckedMarkEffectSize = Size;
            }
            else if (_styleState.HasFlag(ControlStyleState.Off))
            {
                CheckedMarkEffectSize = Size * 0.7;
            }
            else if (_styleState.HasFlag(ControlStyleState.Indeterminate))
            {
                CheckedMarkEffectSize = Size * 0.7;
            }
        }
        else
        {
            if (_styleState.HasFlag(ControlStyleState.On))
            {
                CheckedMarkEffectSize = Size;
            }
        }
    }
    
    public sealed override void Render(DrawingContext context)
    {
        var penWidth      = BorderThickness.Top;
        var borderRadius  = GeometryUtils.CornerRadiusScalarValue(CornerRadius);
        {
            _borderRenderHelper.Render(context, Bounds.Size,
                new Thickness(penWidth),
                new CornerRadius(borderRadius),
                BackgroundSizing.OuterBorderEdge,
                Background,
                BorderBrush,
                new BoxShadows());
        }
        if (_styleState.HasFlag(ControlStyleState.On))
        {
            var checkMarkGeometry =
                CommonShapeBuilder.BuildCheckMark(new Size(CheckedMarkEffectSize,
                    CheckedMarkEffectSize));
            var checkMarkPen = new Pen(CheckedMarkBrush, 2);
            context.DrawGeometry(null, checkMarkPen, checkMarkGeometry);
        }
        else if (_styleState.HasFlag(ControlStyleState.Indeterminate))
        {
            var deltaSize = (Size - TristateMarkSize) / 2.0;
            var offsetX   = deltaSize;
            var offsetY   = deltaSize;
            var indicatorTristateRect =
                new Rect(offsetX, offsetY, TristateMarkSize, TristateMarkSize);
            context.FillRectangle(TristateMarkBrush!, indicatorTristateRect);
        }
    }
    
    public Rect WaveGeometry()
    {
        return new Rect(Bounds.Size);
    }

    public CornerRadius WaveBorderRadius()
    {
        return CornerRadius;
    }
}