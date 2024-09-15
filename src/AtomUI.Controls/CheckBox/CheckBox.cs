using AtomUI.Controls.Utils;
using AtomUI.Media;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Rendering;

namespace AtomUI.Controls;

using AvaloniaCheckBox = Avalonia.Controls.CheckBox;

public class CheckBox : AvaloniaCheckBox,
                        ICustomHitTest,
                        IWaveAdornerInfoProvider
{
    #region 内部属性定义

    internal static readonly StyledProperty<double> CheckIndicatorSizeProperty =
        AvaloniaProperty.Register<CheckBox, double>(nameof(CheckIndicatorSize));

    internal static readonly StyledProperty<double> PaddingInlineProperty =
        AvaloniaProperty.Register<CheckBox, double>(nameof(PaddingInline));

    internal static readonly StyledProperty<IBrush?> IndicatorBorderBrushProperty =
        AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(IndicatorBorderBrush));

    internal static readonly StyledProperty<IBrush?> IndicatorCheckedMarkBrushProperty =
        AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(IndicatorCheckedMarkBrush));

    internal static readonly StyledProperty<double> IndicatorCheckedMarkEffectSizeProperty =
        AvaloniaProperty.Register<CheckBox, double>(nameof(IndicatorCheckedMarkEffectSize));

    internal static readonly StyledProperty<IBrush?> IndicatorTristateMarkBrushProperty =
        AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(IndicatorTristateMarkBrush));

    internal static readonly StyledProperty<double> IndicatorTristateMarkSizeProperty =
        AvaloniaProperty.Register<CheckBox, double>(nameof(IndicatorTristateMarkSize));

    internal static readonly StyledProperty<IBrush?> IndicatorBackgroundProperty =
        AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(IndicatorBackground));

    internal static readonly StyledProperty<Thickness> IndicatorBorderThicknessProperty =
        AvaloniaProperty.Register<CheckBox, Thickness>(nameof(IndicatorBorderThickness));

    internal static readonly StyledProperty<CornerRadius> IndicatorBorderRadiusProperty =
        AvaloniaProperty.Register<CheckBox, CornerRadius>(nameof(IndicatorBorderRadius));

    internal double CheckIndicatorSize
    {
        get => GetValue(CheckIndicatorSizeProperty);
        set => SetValue(CheckIndicatorSizeProperty, value);
    }

    internal double PaddingInline
    {
        get => GetValue(PaddingInlineProperty);
        set => SetValue(PaddingInlineProperty, value);
    }

    internal IBrush? IndicatorBorderBrush
    {
        get => GetValue(IndicatorBorderBrushProperty);
        set => SetValue(IndicatorBorderBrushProperty, value);
    }

    internal IBrush? IndicatorCheckedMarkBrush
    {
        get => GetValue(IndicatorCheckedMarkBrushProperty);
        set => SetValue(IndicatorCheckedMarkBrushProperty, value);
    }

    internal double IndicatorCheckedMarkEffectSize
    {
        get => GetValue(IndicatorCheckedMarkEffectSizeProperty);
        set => SetValue(IndicatorCheckedMarkEffectSizeProperty, value);
    }

    internal IBrush? IndicatorTristateMarkBrush
    {
        get => GetValue(IndicatorTristateMarkBrushProperty);
        set => SetValue(IndicatorTristateMarkBrushProperty, value);
    }

    internal double IndicatorTristateMarkSize
    {
        get => GetValue(IndicatorTristateMarkSizeProperty);
        set => SetValue(IndicatorTristateMarkSizeProperty, value);
    }

    internal IBrush? IndicatorBackground
    {
        get => GetValue(IndicatorBackgroundProperty);
        set => SetValue(IndicatorBackgroundProperty, value);
    }

    internal Thickness IndicatorBorderThickness
    {
        get => GetValue(IndicatorBorderThicknessProperty);
        set => SetValue(IndicatorBorderThicknessProperty, value);
    }

    internal CornerRadius IndicatorBorderRadius
    {
        get => GetValue(IndicatorBorderRadiusProperty);
        set => SetValue(IndicatorBorderRadiusProperty, value);
    }

    #endregion

    private ControlStyleState _styleState;
    private readonly BorderRenderHelper _borderRenderHelper;

    static CheckBox()
    {
        AffectsRender<CheckBox>(
            IsCheckedProperty,
            IndicatorCheckedMarkEffectSizeProperty,
            PaddingInlineProperty,
            IndicatorBorderBrushProperty,
            IndicatorCheckedMarkBrushProperty,
            IndicatorTristateMarkBrushProperty,
            IndicatorBackgroundProperty,
            IndicatorBorderThicknessProperty,
            IndicatorBorderRadiusProperty);
    }

    public CheckBox()
    {
        _borderRenderHelper = new BorderRenderHelper();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        HandlePropertyChangedForStyle(e);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var size         = base.MeasureOverride(availableSize);
        var targetWidth  = size.Width + CheckIndicatorSize + PaddingInline;
        var targetHeight = Math.Max(size.Height, CheckIndicatorSize);
        return new Size(targetWidth, targetHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var arrangeRect = TextRect();

        var visualChildren = VisualChildren;
        var visualCount    = visualChildren.Count;

        for (var i = 0; i < visualCount; i++)
        {
            var visual = visualChildren[i];
            if (visual is Layoutable layoutable)
            {
                layoutable.Arrange(arrangeRect);
            }
        }

        return finalSize;
    }

    public sealed override void Render(DrawingContext context)
    {
        var indicatorRect = IndicatorRect();
        var penWidth      = IndicatorBorderThickness.Top;
        var borderRadius  = GeometryUtils.CornerRadiusScalarValue(IndicatorBorderRadius);
        {
            _borderRenderHelper.Render(context, indicatorRect.Size,
                new Thickness(penWidth),
                new CornerRadius(borderRadius),
                BackgroundSizing.OuterBorderEdge,
                IndicatorBackground,
                IndicatorBorderBrush,
                new BoxShadows());
        }
        if (_styleState.HasFlag(ControlStyleState.On))
        {
            var checkMarkGeometry =
                CommonShapeBuilder.BuildCheckMark(new Size(IndicatorCheckedMarkEffectSize,
                    IndicatorCheckedMarkEffectSize));
            var checkMarkPen = new Pen(IndicatorCheckedMarkBrush, 2);
            context.DrawGeometry(null, checkMarkPen, checkMarkGeometry);
        }
        else if (_styleState.HasFlag(ControlStyleState.Indeterminate))
        {
            var deltaSize = (CheckIndicatorSize - IndicatorTristateMarkSize) / 2.0;
            var offsetX   = indicatorRect.X + deltaSize;
            var offsetY   = indicatorRect.Y + deltaSize;
            var indicatorTristateRect =
                new Rect(offsetX, offsetY, IndicatorTristateMarkSize, IndicatorTristateMarkSize);
            context.FillRectangle(IndicatorTristateMarkBrush!, indicatorTristateRect);
        }
    }

    public bool HitTest(Point point)
    {
        return true;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        TokenResourceBinder.CreateGlobalResourceBinding(this, IndicatorBorderThicknessProperty,
            GlobalTokenResourceKey.BorderThickness, BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this));
        HorizontalAlignment = HorizontalAlignment.Left;
        Cursor              = new Cursor(StandardCursorType.Hand);
        CollectStyleState();
        SetupIndicatorCheckedMarkEffectSize();
        SetupTransitions();
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

        if (IsPressed)
        {
            _styleState |= ControlStyleState.Sunken;
        }
        else
        {
            _styleState |= ControlStyleState.Raised;
        }
    }

    // Measure 之后才有值
    private Rect IndicatorRect()
    {
        return new Rect(0d, 0d, CheckIndicatorSize, CheckIndicatorSize);
    }

    private Rect TextRect()
    {
        var offsetX = CheckIndicatorSize + PaddingInline;
        return new Rect(offsetX, 0d, DesiredSize.Width - offsetX, DesiredSize.Height);
    }

    private void SetupIndicatorCheckedMarkEffectSize()
    {
        if (_styleState.HasFlag(ControlStyleState.Enabled))
        {
            if (_styleState.HasFlag(ControlStyleState.On))
            {
                IndicatorCheckedMarkEffectSize = CheckIndicatorSize;
            }
            else if (_styleState.HasFlag(ControlStyleState.Off))
            {
                IndicatorCheckedMarkEffectSize = CheckIndicatorSize * 0.7;
            }
            else if (_styleState.HasFlag(ControlStyleState.Indeterminate))
            {
                IndicatorCheckedMarkEffectSize = CheckIndicatorSize * 0.7;
            }
        }
        else
        {
            if (_styleState.HasFlag(ControlStyleState.On))
            {
                IndicatorCheckedMarkEffectSize = CheckIndicatorSize;
            }
        }
    }

    private void SetupTransitions()
    {
        Transitions = new Transitions
        {
            AnimationUtils.CreateTransition<SolidColorBrushTransition>(IndicatorBackgroundProperty),
            AnimationUtils.CreateTransition<SolidColorBrushTransition>(IndicatorBorderBrushProperty),
            AnimationUtils.CreateTransition<SolidColorBrushTransition>(IndicatorTristateMarkBrushProperty),
            AnimationUtils.CreateTransition<DoubleTransition>(IndicatorCheckedMarkEffectSizeProperty,
                GlobalTokenResourceKey.MotionDurationMid, new BackEaseOut())
        };
    }

    private void HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
    {
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
    }

    public Rect WaveGeometry()
    {
        return IndicatorRect();
    }

    public CornerRadius WaveBorderRadius()
    {
        return IndicatorBorderRadius;
    }
}