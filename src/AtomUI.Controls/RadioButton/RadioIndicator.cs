﻿using System.Reactive.Disposables;
using AtomUI.Controls.Utils;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

internal class RadioIndicator : Control,
                                IWaveAdornerInfoProvider,
                                ITokenResourceConsumer
{
    #region 公共属性定义

    public static readonly StyledProperty<bool?> IsCheckedProperty =
        ToggleButton.IsCheckedProperty.AddOwner<CheckBoxIndicator>();

    public static readonly StyledProperty<double> RadioSizeProperty =
        AvaloniaProperty.Register<RadioIndicator, double>(nameof(RadioSize));

    public static readonly StyledProperty<double> PaddingInlineProperty =
        AvaloniaProperty.Register<RadioIndicator, double>(nameof(PaddingInline));

    public static readonly StyledProperty<IBrush?> RadioBorderBrushProperty =
        AvaloniaProperty.Register<RadioIndicator, IBrush?>(nameof(RadioBorderBrush));

    public static readonly StyledProperty<IBrush?> RadioInnerBackgroundProperty =
        AvaloniaProperty.Register<RadioIndicator, IBrush?>(nameof(RadioInnerBackground));

    public static readonly StyledProperty<IBrush?> RadioBackgroundProperty =
        AvaloniaProperty.Register<RadioIndicator, IBrush?>(nameof(RadioBackground));

    public static readonly StyledProperty<Thickness> RadioBorderThicknessProperty =
        AvaloniaProperty.Register<RadioIndicator, Thickness>(nameof(RadioBorderThickness));

    public static readonly StyledProperty<double> RadioDotEffectSizeProperty =
        AvaloniaProperty.Register<RadioIndicator, double>(nameof(RadioDotEffectSize));

    public static readonly StyledProperty<double> DotSizeValueProperty =
        AvaloniaProperty.Register<RadioIndicator, double>(
            nameof(DotSizeValue));

    public static readonly StyledProperty<double> DotPaddingProperty =
        AvaloniaProperty.Register<RadioIndicator, double>(
            nameof(DotPadding));

    public bool? IsChecked
    {
        get => GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    public double RadioSize
    {
        get => GetValue(RadioSizeProperty);
        set => SetValue(RadioSizeProperty, value);
    }

    public double PaddingInline
    {
        get => GetValue(PaddingInlineProperty);
        set => SetValue(PaddingInlineProperty, value);
    }

    public IBrush? RadioBorderBrush
    {
        get => GetValue(RadioBorderBrushProperty);
        set => SetValue(RadioBorderBrushProperty, value);
    }

    public IBrush? RadioInnerBackground
    {
        get => GetValue(RadioInnerBackgroundProperty);
        set => SetValue(RadioInnerBackgroundProperty, value);
    }

    public IBrush? RadioBackground
    {
        get => GetValue(RadioBackgroundProperty);
        set => SetValue(RadioBackgroundProperty, value);
    }

    public Thickness RadioBorderThickness
    {
        get => GetValue(RadioBorderThicknessProperty);
        set => SetValue(RadioBorderThicknessProperty, value);
    }

    public double RadioDotEffectSize
    {
        get => GetValue(RadioDotEffectSizeProperty);
        set => SetValue(RadioDotEffectSizeProperty, value);
    }

    public double DotSizeValue
    {
        get => GetValue(DotSizeValueProperty);
        set => SetValue(DotSizeValueProperty, value);
    }

    public double DotPadding
    {
        get => GetValue(DotPaddingProperty);
        set => SetValue(DotPaddingProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty
        = WaveSpiritAwareControlProperty.IsMotionEnabledProperty.AddOwner<RadioIndicator>();

    internal static readonly StyledProperty<bool> IsWaveAnimationEnabledProperty
        = WaveSpiritAwareControlProperty.IsWaveSpiritEnabledProperty.AddOwner<RadioIndicator>();

    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    internal bool IsWaveAnimationEnabled
    {
        get => GetValue(IsWaveAnimationEnabledProperty);
        set => SetValue(IsWaveAnimationEnabledProperty, value);
    }

    CompositeDisposable? ITokenResourceConsumer.TokenBindingsDisposable => _tokenBindingsDisposable;

    #endregion

    private IPen? _cachedPen;
    private CompositeDisposable? _tokenBindingsDisposable;

    static RadioIndicator()
    {
        AffectsRender<RadioIndicator>(
            IsCheckedProperty,
            RadioBorderBrushProperty,
            RadioInnerBackgroundProperty,
            RadioBackgroundProperty,
            RadioBorderThicknessProperty,
            RadioDotEffectSizeProperty);
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        _tokenBindingsDisposable = new CompositeDisposable();
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, RadioBorderThicknessProperty,
            SharedTokenKey.BorderThickness, BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this)));
        RadioDotEffectSize = CalculateDotSize(IsEnabled, IsChecked.HasValue && IsChecked.Value);
        SetupTransitions();
        UpdatePseudoClasses();
    }

    private void SetupTransitions()
    {
        if (IsMotionEnabled)
        {
            Transitions ??= new Transitions
            {
                AnimationUtils.CreateTransition<SolidColorBrushTransition>(RadioBorderBrushProperty),
                AnimationUtils.CreateTransition<DoubleTransition>(RadioDotEffectSizeProperty),
                AnimationUtils.CreateTransition<SolidColorBrushTransition>(RadioBackgroundProperty,
                    SharedTokenKey.MotionDurationFast)
            };
        }
        else
        {
            Transitions = null;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == IsPointerOverProperty ||
            e.Property == IsCheckedProperty ||
            e.Property == IsEnabledProperty)
        {
            UpdatePseudoClasses();

            if (e.Property == IsCheckedProperty &&
                IsWaveAnimationEnabled &&
                !PseudoClasses.Contains(StdPseudoClass.Disabled) &&
                PseudoClasses.Contains(StdPseudoClass.Checked))
            {
                WaveSpiritAdorner.ShowWaveAdorner(this, WaveType.CircleWave);
            }
        }

        if (this.IsAttachedToVisualTree())
        {
            if (e.Property == IsMotionEnabledProperty)
            {
                SetupTransitions();
            }
            else if (e.Property == IsPointerOverProperty ||
                e.Property == IsCheckedProperty ||
                e.Property == IsEnabledProperty)
            {
                RadioDotEffectSize = CalculateDotSize(IsEnabled, IsChecked.HasValue && IsChecked.Value);
            }
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(StdPseudoClass.Checked, IsChecked.HasValue && IsChecked.Value);
        PseudoClasses.Set(StdPseudoClass.UnChecked, (IsChecked.HasValue && !IsChecked.Value) || !IsChecked.HasValue);
    }

    private double CalculateDotSize(bool isEnabled, bool isChecked)
    {
        double targetValue;
        if (isChecked)
        {
            if (isEnabled)
            {
                targetValue = DotSizeValue;
            }
            else
            {
                targetValue = RadioSize - DotPadding * 2;
            }
        }
        else
        {
            targetValue = DotSizeValue * 0.6;
        }

        return targetValue;
    }

    public sealed override void Render(DrawingContext context)
    {
        var penWidth = RadioBorderThickness.Top;
        PenUtils.TryModifyOrCreate(ref _cachedPen, RadioBorderBrush, RadioBorderThickness.Top);
        var targetRect = new Rect(0, 0, Bounds.Width, Bounds.Height);
        context.DrawEllipse(RadioBackground, _cachedPen, targetRect.Deflate(penWidth / 2));
        if (IsChecked.HasValue && IsChecked.Value)
        {
            var dotDiameter = RadioDotEffectSize / 2;
            context.DrawEllipse(RadioInnerBackground, null, targetRect.Center, dotDiameter, dotDiameter);
        }
    }

    public Rect WaveGeometry()
    {
        return new Rect(Bounds.Size);
    }

    public CornerRadius WaveBorderRadius()
    {
        return new CornerRadius(RadioSize / 2);
    }
}