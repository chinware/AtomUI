using System.Diagnostics;
using System.Reactive.Disposables;
using AtomUI.Animations;
using AtomUI.Controls.Themes;
using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

internal enum CheckBoxIndicatorState
{
    Checked,
    Indeterminate,
    Unchecked,
}

internal class CheckBoxIndicator : TemplatedControl,
                                    IWaveAdornerInfoProvider,
                                    IResourceBindingManager
{
    #region 公共属性定义

    public static readonly StyledProperty<CheckBoxIndicatorState> StateProperty =
        AvaloniaProperty.Register<CheckBoxIndicator, CheckBoxIndicatorState>(nameof(State));
    
    public static readonly StyledProperty<IBrush?> CheckedMarkBrushProperty =
        AvaloniaProperty.Register<CheckBoxIndicator, IBrush?>(nameof(CheckedMarkBrush));

    public static readonly StyledProperty<IBrush?> TristateMarkBrushProperty =
        AvaloniaProperty.Register<CheckBoxIndicator, IBrush?>(nameof(TristateMarkBrush));

    public static readonly StyledProperty<double> TristateMarkSizeProperty =
        AvaloniaProperty.Register<CheckBoxIndicator, double>(nameof(TristateMarkSize));

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

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty
        = MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<CheckBoxIndicator>();

    internal static readonly StyledProperty<bool> IsWaveSpiritEnabledProperty
        = WaveSpiritAwareControlProperty.IsWaveSpiritEnabledProperty.AddOwner<CheckBoxIndicator>();

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
    
    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable
    {
        get => _resourceBindingsDisposable;
        set => _resourceBindingsDisposable = value;
    }

    #endregion

    private Icon? _checkedMark;
    private CompositeDisposable? _resourceBindingsDisposable;

    static CheckBoxIndicator()
    {
        AffectsRender<CheckBoxIndicator>(
            StateProperty,
            CheckedMarkBrushProperty,
            TristateMarkBrushProperty);
        AffectsArrange<CheckBoxIndicator>(TristateMarkSizeProperty);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _resourceBindingsDisposable = new CompositeDisposable();
        this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, BorderThicknessProperty,
            SharedTokenKey.BorderThickness, BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this)));
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        this.DisposeTokenBindings();
    }

    private void ConfigureTransitions()
    {
        if (IsMotionEnabled)
        {
            Transitions = new Transitions
            {
                TransitionUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty),
                TransitionUtils.CreateTransition<SolidColorBrushTransition>(BorderBrushProperty),
                TransitionUtils.CreateTransition<SolidColorBrushTransition>(TristateMarkBrushProperty),
            };
            if (_checkedMark != null)
            {
                _checkedMark.Transitions = new Transitions
                {
                    TransitionUtils.CreateTransition<TransformOperationsTransition>(RenderTransformProperty, SharedTokenKey.MotionDurationMid,
                        new BackEaseOut()),
                };
            }
        }
        else
        {
            Transitions = null;
            if (_checkedMark != null)
            {
                _checkedMark.Transitions = null;
            }
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(StdPseudoClass.Checked, State == CheckBoxIndicatorState.Checked);
        PseudoClasses.Set(StdPseudoClass.UnChecked, State == CheckBoxIndicatorState.Unchecked);
        PseudoClasses.Set(StdPseudoClass.Indeterminate, State == CheckBoxIndicatorState.Indeterminate);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == IsPointerOverProperty ||
            e.Property == StateProperty ||
            e.Property == IsEnabledProperty)
        {
            UpdatePseudoClasses();
            if (e.Property == StateProperty &&
                IsWaveSpiritEnabled &&
                IsEnabled &&
                PseudoClasses.Contains(StdPseudoClass.Checked))
            {
                WaveSpiritAdorner.ShowWaveAdorner(this, WaveType.RoundRectWave);
            }
        }
        
        if (this.IsAttachedToVisualTree())
        {
            if (e.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions();
            }
         
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _checkedMark = e.NameScope.Find<Icon>(CheckBoxIndicatorThemeConstants.CheckedMarkPart);
        ConfigureTransitions();
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