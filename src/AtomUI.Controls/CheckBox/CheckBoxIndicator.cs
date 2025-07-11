﻿using System.Reactive.Disposables;
using AtomUI.Animations;
using AtomUI.Controls.Utils;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

internal class CheckBoxIndicator : Control, 
                                   IWaveAdornerInfoProvider,
                                   IResourceBindingManager
{
    #region 公共属性定义

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

    private readonly BorderRenderHelper _borderRenderHelper;
    private CompositeDisposable? _resourceBindingsDisposable;

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

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _resourceBindingsDisposable = new CompositeDisposable();
        this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, BorderThicknessProperty,
            SharedTokenKey.BorderThickness, BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this)));
        ConfigureTransitions();
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
            Transitions ??= new Transitions
            {
                TransitionUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty),
                TransitionUtils.CreateTransition<SolidColorBrushTransition>(BorderBrushProperty),
                TransitionUtils.CreateTransition<SolidColorBrushTransition>(TristateMarkBrushProperty),
                TransitionUtils.CreateTransition<DoubleTransition>(CheckedMarkEffectSizeProperty,
                    SharedTokenKey.MotionDurationMid, new BackEaseOut())
            };
        }
        else
        {
            Transitions = null;
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(StdPseudoClass.Checked, IsChecked.HasValue && IsChecked.Value);
        PseudoClasses.Set(StdPseudoClass.UnChecked, IsChecked.HasValue && !IsChecked.Value);
        PseudoClasses.Set(StdPseudoClass.Indeterminate, !IsChecked.HasValue);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == IsPointerOverProperty ||
            e.Property == IsCheckedProperty ||
            e.Property == IsEnabledProperty)
        {
            UpdatePseudoClasses();
            SetupIndicatorCheckedMarkEffectSize();
            if (e.Property == IsCheckedProperty &&
                IsWaveSpiritEnabled &&
                !PseudoClasses.Contains(StdPseudoClass.Disabled) &&
                PseudoClasses.Contains(StdPseudoClass.Checked))
            {
                WaveSpiritAdorner.ShowWaveAdorner(this, WaveType.RoundRectWave);
            }
        }

        if (e.Property == SizeProperty)
        {
            UpdatePseudoClasses();
            SetupIndicatorCheckedMarkEffectSize();
        }

        if (this.IsAttachedToVisualTree())
        {
            if (e.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions();
            }
        }
    }

    private void SetupIndicatorCheckedMarkEffectSize()
    {
        if (!PseudoClasses.Contains(StdPseudoClass.Disabled))
        {
            if (PseudoClasses.Contains(StdPseudoClass.Checked))
            {
                CheckedMarkEffectSize = Size;
            }
            else if (PseudoClasses.Contains(StdPseudoClass.UnChecked))
            {
                CheckedMarkEffectSize = Size * 0.7;
            }
            else if (PseudoClasses.Contains(StdPseudoClass.Indeterminate))
            {
                CheckedMarkEffectSize = Size * 0.7;
            }
        }
        else
        {
            if (PseudoClasses.Contains(StdPseudoClass.Checked))
            {
                CheckedMarkEffectSize = Size;
            }
        }
    }

    public sealed override void Render(DrawingContext context)
    {
        var penWidth     = BorderThickness.Top;
        var borderRadius = GeometryUtils.CornerRadiusScalarValue(CornerRadius);
        {
            _borderRenderHelper.Render(context, Bounds.Size,
                new Thickness(penWidth),
                new CornerRadius(borderRadius),
                BackgroundSizing.OuterBorderEdge,
                Background,
                BorderBrush,
                new BoxShadows());
        }
        if (PseudoClasses.Contains(StdPseudoClass.Checked))
        {
            var checkMarkGeometry =
                CommonShapeBuilder.BuildCheckMark(new Size(CheckedMarkEffectSize,
                    CheckedMarkEffectSize));
            var checkMarkPen = new Pen(CheckedMarkBrush, 2);
            context.DrawGeometry(null, checkMarkPen, checkMarkGeometry);
        }
        else if (PseudoClasses.Contains(StdPseudoClass.Indeterminate))
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