using AtomUI.Animations;
using AtomUI.Controls.Primitives;
using AtomUI.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

using ShapePath = Avalonia.Controls.Shapes.Path;

internal enum CheckBoxIndicatorState
{
    Checked,
    Indeterminate,
    Unchecked,
}

internal class CheckBoxIndicator : TemplatedControl
{
    #region 公共属性定义

    public static readonly StyledProperty<CheckBoxIndicatorState> StateProperty =
        AvaloniaProperty.Register<CheckBoxIndicator, CheckBoxIndicatorState>(
            nameof(State),
            CheckBoxIndicatorState.Unchecked);
    
    public static readonly StyledProperty<IBrush?> CheckedMarkBrushProperty =
        AvaloniaProperty.Register<CheckBoxIndicator, IBrush?>(nameof(CheckedMarkBrush));

    public static readonly StyledProperty<IBrush?> TristateMarkBrushProperty =
        AvaloniaProperty.Register<CheckBoxIndicator, IBrush?>(nameof(TristateMarkBrush));

    public static readonly StyledProperty<double> TristateMarkSizeProperty =
        AvaloniaProperty.Register<CheckBoxIndicator, double>(nameof(TristateMarkSize));

    public static readonly StyledProperty<double> CheckedMarkSizeProperty =
        AvaloniaProperty.Register<CheckBoxIndicator, double>(nameof(CheckedMarkSize), 10.0);
    
    public static readonly StyledProperty<ITransform?> CheckedMarkRenderTransformProperty =
        AvaloniaProperty.Register<CheckBoxIndicator, ITransform?>(nameof(CheckedMarkRenderTransform));

    public CheckBoxIndicatorState State
    {
        get => GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }
    
    public IBrush? CheckedMarkBrush
    {
        get => GetValue(CheckedMarkBrushProperty);
        set => SetValue(CheckedMarkBrushProperty, value);
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

    public double CheckedMarkSize
    {
        get => GetValue(CheckedMarkSizeProperty);
        set => SetValue(CheckedMarkSizeProperty, value);
    }
    
    public ITransform? CheckedMarkRenderTransform
    {
        get => GetValue(CheckedMarkRenderTransformProperty);
        set => SetValue(CheckedMarkRenderTransformProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<CheckBoxIndicator>();

    internal static readonly StyledProperty<bool> IsWaveSpiritEnabledProperty =
        WaveSpiritAwareControlProperty.IsWaveSpiritEnabledProperty.AddOwner<CheckBoxIndicator>();

    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    internal bool IsWaveSpiritEnabled
    {
        get => GetValue(IsWaveSpiritEnabledProperty);
        set => SetValue(IsWaveSpiritEnabledProperty, value);
    }

    #endregion

    private static readonly Geometry CheckedMarkGeometry =
        StreamGeometry.Parse("M1014.4 233.2l-84.6-84.6c-6-6-15.8-6-21.8 0L383.6 673.2 115.8 405.4c-6-6-15.8-6-21.8 0L9.6 490c-6 6-6 15.8 0 21.8l362.6 362.6c8.6 7.2 14.6 7.2 22.8 0l619.4-619.4C1020.4 249 1020.4 239.2 1014.4 233.2z");
    private const double CheckedMarkViewBoxSize       = 1024.0;
    private const double CheckedMarkGeometryBoxWidth  = 1016.8;
    private const double CheckedMarkGeometryBoxHeight = 739.0;
    
    private Panel? _rootLayout;
    private WaveSpiritDecorator? _waveSpiritDecorator;
    private ShapePath? _checkedMark;
    private Rectangle? _tristateMark;

    static CheckBoxIndicator()
    {
        AffectsRender<CheckBoxIndicator>(
            StateProperty,
            CheckedMarkBrushProperty,
            TristateMarkBrushProperty);
        AffectsArrange<CheckBoxIndicator>(
            TristateMarkSizeProperty,
            CheckedMarkSizeProperty);
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(StdPseudoClass.Checked, State == CheckBoxIndicatorState.Checked);
        PseudoClasses.Set(StdPseudoClass.UnChecked, State == CheckBoxIndicatorState.Unchecked);
        PseudoClasses.Set(StdPseudoClass.Indeterminate, State == CheckBoxIndicatorState.Indeterminate);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsPointerOverProperty ||
            change.Property == StateProperty ||
            change.Property == IsEnabledProperty)
        {
            UpdatePseudoClasses();
            if (change.Property == StateProperty)
            {
                UpdateStateVisuals();
                if (ShouldPlayWaveSpirit())
                {
                    EnsureWaveSpiritDecorator();
                    Dispatcher.Post(PlayWaveSpirit);
                }
            }
            else if (change.Property == IsEnabledProperty)
            {
                UpdateWaveSpiritDecorator();
            }
        }

        if (change.Property == IsMotionEnabledProperty ||
            change.Property == IsWaveSpiritEnabledProperty)
        {
            UpdateWaveSpiritDecorator();
        }
        if (change.Property == CheckedMarkBrushProperty ||
            change.Property == CheckedMarkRenderTransformProperty ||
            change.Property == CheckedMarkSizeProperty)
        {
            SyncCheckedMark();
        }
        if (change.Property == TristateMarkBrushProperty ||
            change.Property == TristateMarkSizeProperty)
        {
            SyncTristateMark();
        }
        if (change.Property == CornerRadiusProperty)
        {
            SyncWaveSpiritDecorator();
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        DetachWaveSpiritDecorator();
        DetachCheckedMark();
        DetachTristateMark();
        base.OnApplyTemplate(e);

        _rootLayout = e.NameScope.Find<Panel>("PART_RootLayout");
        UpdatePseudoClasses();
        UpdateStateVisuals();
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

    private bool ShouldPlayWaveSpirit()
    {
        return IsWaveSpiritEnabled &&
               IsEnabled &&
               IsMotionEnabled &&
               IsLoaded &&
               State == CheckBoxIndicatorState.Checked;
    }

    private void UpdateStateVisuals()
    {
        if (State == CheckBoxIndicatorState.Checked)
        {
            EnsureCheckedMark();
            DetachTristateMark();
        }
        else if (State == CheckBoxIndicatorState.Indeterminate)
        {
            DetachCheckedMark();
            EnsureTristateMark();
        }
        else
        {
            DetachCheckedMark();
            DetachTristateMark();
        }
    }

    private void EnsureCheckedMark()
    {
        if (_checkedMark != null || _rootLayout == null)
        {
            return;
        }

        _checkedMark = new ShapePath
        {
            Name                  = "CheckedMark",
            Data                  = CheckedMarkGeometry,
            Stretch               = Stretch.Uniform,
            HorizontalAlignment   = HorizontalAlignment.Center,
            VerticalAlignment     = VerticalAlignment.Center,
            RenderTransformOrigin = RelativePoint.Center
        };
        _checkedMark.SetTemplatedParent(this);
        SyncCheckedMark();
        _rootLayout.Children.Add(_checkedMark);
    }

    private void SyncCheckedMark()
    {
        if (_checkedMark == null)
        {
            return;
        }

        _checkedMark.SetCurrentValue(Shape.FillProperty, CheckedMarkBrush);
        _checkedMark.SetCurrentValue(Layoutable.WidthProperty,
            CheckedMarkSize * CheckedMarkGeometryBoxWidth / CheckedMarkViewBoxSize);
        _checkedMark.SetCurrentValue(Layoutable.HeightProperty,
            CheckedMarkSize * CheckedMarkGeometryBoxHeight / CheckedMarkViewBoxSize);
        _checkedMark.SetCurrentValue(RenderTransformProperty, CheckedMarkRenderTransform);
    }

    private void DetachCheckedMark()
    {
        if (_checkedMark == null)
        {
            return;
        }

        if (_checkedMark.GetVisualParent() is Panel parent)
        {
            parent.Children.Remove(_checkedMark);
        }
        else
        {
            _rootLayout?.Children.Remove(_checkedMark);
        }

        _checkedMark.ClearValue(Shape.FillProperty);
        _checkedMark.ClearValue(RenderTransformProperty);
        _checkedMark.SetTemplatedParent(null);
        _checkedMark = null;
    }

    private void EnsureTristateMark()
    {
        if (_tristateMark != null || _rootLayout == null)
        {
            return;
        }

        _tristateMark = new Rectangle
        {
            Name                = "TristateMark",
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment   = VerticalAlignment.Stretch
        };
        _tristateMark.SetTemplatedParent(this);
        SyncTristateMark();
        _rootLayout.Children.Add(_tristateMark);
    }

    private void SyncTristateMark()
    {
        if (_tristateMark == null)
        {
            return;
        }

        _tristateMark.SetCurrentValue(Shape.FillProperty, TristateMarkBrush);
        _tristateMark.SetCurrentValue(Layoutable.WidthProperty, TristateMarkSize);
        _tristateMark.SetCurrentValue(Layoutable.HeightProperty, TristateMarkSize);
    }

    private void DetachTristateMark()
    {
        if (_tristateMark == null)
        {
            return;
        }

        if (_tristateMark.GetVisualParent() is Panel parent)
        {
            parent.Children.Remove(_tristateMark);
        }
        else
        {
            _rootLayout?.Children.Remove(_tristateMark);
        }

        _tristateMark.ClearValue(Shape.FillProperty);
        _tristateMark.SetTemplatedParent(null);
        _tristateMark = null;
    }

    private void EnsureWaveSpiritDecorator()
    {
        if (_waveSpiritDecorator != null || _rootLayout == null)
        {
            return;
        }

        _waveSpiritDecorator = new WaveSpiritDecorator
        {
            Name = WaveSpiritDecorator.WaveSpiritPart
        };
        _waveSpiritDecorator.SetTemplatedParent(this);
        SyncWaveSpiritDecorator();
        _rootLayout.Children.Insert(0, _waveSpiritDecorator);
    }

    private void UpdateWaveSpiritDecorator()
    {
        if (IsWaveSpiritEnabled && IsEnabled && IsMotionEnabled)
        {
            SyncWaveSpiritDecorator();
            return;
        }

        DetachWaveSpiritDecorator();
    }

    private void SyncWaveSpiritDecorator()
    {
        if (_waveSpiritDecorator == null)
        {
            return;
        }

        _waveSpiritDecorator.SetCurrentValue(WaveSpiritDecorator.CornerRadiusProperty, CornerRadius);
        _waveSpiritDecorator.SetCurrentValue(WaveSpiritDecorator.WaveTypeProperty, WaveSpiritType.RoundRectWave);
    }

    private void PlayWaveSpirit()
    {
        _waveSpiritDecorator?.Play();
    }

    private void DetachWaveSpiritDecorator()
    {
        if (_waveSpiritDecorator == null)
        {
            return;
        }

        if (_waveSpiritDecorator.GetVisualParent() is Panel parent)
        {
            parent.Children.Remove(_waveSpiritDecorator);
        }
        else
        {
            _rootLayout?.Children.Remove(_waveSpiritDecorator);
        }

        _waveSpiritDecorator.SetTemplatedParent(null);
        _waveSpiritDecorator = null;
    }
}
