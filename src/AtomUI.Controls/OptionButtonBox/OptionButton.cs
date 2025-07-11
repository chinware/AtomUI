using System.Diagnostics;
using AtomUI.Animations;
using AtomUI.Controls.Utils;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

using AvaloniaRadioButton = Avalonia.Controls.RadioButton;

public enum OptionButtonStyle
{
    Outline,
    Solid
}

public enum OptionButtonPositionTrait
{
    First,
    Last,
    Middle,
    OnlyOne
}

public class OptionButtonPointerEventArgs : EventArgs
{
    public OptionButton? Button { get; }
    public bool IsPressed { get; set; }
    public bool IsHovering { get; set; }

    public OptionButtonPointerEventArgs(OptionButton button)
    {
        Button = button;
    }
}

public class OptionButton : AvaloniaRadioButton,
                            ISizeTypeAware,
                            IWaveAdornerInfoProvider
{
    #region 公共属性定义

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<OptionButton>();

    public static readonly StyledProperty<OptionButtonStyle> ButtonStyleProperty =
        AvaloniaProperty.Register<OptionButton, OptionButtonStyle>(nameof(ButtonStyle));

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public OptionButtonStyle ButtonStyle
    {
        get => GetValue(ButtonStyleProperty);
        set => SetValue(ButtonStyleProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<OptionButton, OptionButtonPositionTrait> GroupPositionTraitProperty =
        AvaloniaProperty.RegisterDirect<OptionButton, OptionButtonPositionTrait>(
            nameof(GroupPositionTrait),
            o => o.GroupPositionTrait,
            (o, v) => o.GroupPositionTrait = v,
            OptionButtonPositionTrait.OnlyOne);

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty
        = MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<OptionButton>();

    internal static readonly StyledProperty<bool> IsWaveSpiritEnabledProperty
        = WaveSpiritAwareControlProperty.IsWaveSpiritEnabledProperty.AddOwner<OptionButton>();

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

    private OptionButtonPositionTrait _groupPositionTrait = OptionButtonPositionTrait.OnlyOne;

    internal OptionButtonPositionTrait GroupPositionTrait
    {
        get => _groupPositionTrait;
        set => SetAndRaise(GroupPositionTraitProperty, ref _groupPositionTrait, value);
    }

    internal event EventHandler<OptionButtonPointerEventArgs>? OptionButtonPointerEvent;

    #endregion

    private CornerRadius? _originCornerRadius;
    private readonly BorderRenderHelper _borderRenderHelper;

    static OptionButton()
    {
        AffectsMeasure<OptionButton>(SizeTypeProperty, ButtonStyleProperty);
        AffectsRender<OptionButton>(IsCheckedProperty, CornerRadiusProperty, ForegroundProperty, BackgroundProperty);
    }

    public OptionButton()
    {
        _borderRenderHelper = new BorderRenderHelper();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var size         = base.MeasureOverride(availableSize);
        var targetWidth  = size.Width;
        var targetHeight = size.Height;
        targetHeight += Padding.Top + Padding.Bottom;
        targetWidth  += Padding.Left + Padding.Right;
        return new Size(targetWidth, targetHeight);
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        Debug.Assert(Parent is OptionButtonGroup, "OptionButton parent must be type of OptionButtonGroup");
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        HandleSizeTypeChanged();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == IsPointerOverProperty ||
            e.Property == IsPressedProperty ||
            e.Property == IsCheckedProperty)
        {
            if (e.Property == IsPressedProperty && e.OldValue as bool? == true && IsWaveSpiritEnabled)
            {
                WaveSpiritAdorner.ShowWaveAdorner(this, WaveType.RoundRectWave);
            }
        }

        if (e.Property == GroupPositionTraitProperty)
        {
            if (_originCornerRadius.HasValue)
            {
                CornerRadius = BuildCornerRadius(GroupPositionTrait, _originCornerRadius!.Value);
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

    private void HandleSizeTypeChanged()
    {
        _originCornerRadius = CornerRadius;
        CornerRadius        = BuildCornerRadius(GroupPositionTrait, _originCornerRadius!.Value);
    }

    public Rect WaveGeometry()
    {
        return new Rect(0, 0, Bounds.Width, Bounds.Height);
    }

    public CornerRadius WaveBorderRadius()
    {
        return CornerRadius;
    }

    private void ConfigureTransitions()
    {
        if (IsMotionEnabled)
        {
            var transitions = new Transitions();
            if (ButtonStyle == OptionButtonStyle.Solid)
            {
                transitions.Add(TransitionUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty));
            }
            else if (ButtonStyle == OptionButtonStyle.Outline)
            {
                transitions.Add(TransitionUtils.CreateTransition<SolidColorBrushTransition>(BorderBrushProperty));
            }

            transitions.Add(TransitionUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty));
            Transitions = transitions;
        }
        else
        {
            Transitions = null;
        }
    }

    private CornerRadius BuildCornerRadius(OptionButtonPositionTrait positionTrait, CornerRadius cornerRadius)
    {
        if (positionTrait == OptionButtonPositionTrait.First)
        {
            return new CornerRadius(cornerRadius.TopLeft,
                0,
                0,
                cornerRadius.BottomLeft);
        }

        if (positionTrait == OptionButtonPositionTrait.Last)
        {
            return new CornerRadius(0,
                cornerRadius.TopRight,
                cornerRadius.BottomRight,
                0);
        }

        if (positionTrait == OptionButtonPositionTrait.Middle)
        {
            return new CornerRadius(0);
        }

        return cornerRadius;
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        OptionButtonPointerEvent?.Invoke(this, new OptionButtonPointerEventArgs(this)
        {
            IsHovering = true,
            IsPressed  = true
        });
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        OptionButtonPointerEvent?.Invoke(this, new OptionButtonPointerEventArgs(this)
        {
            IsHovering = true,
            IsPressed  = false
        });
    }

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);
        OptionButtonPointerEvent?.Invoke(this, new OptionButtonPointerEventArgs(this)
        {
            IsHovering = true,
            IsPressed  = false
        });
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        OptionButtonPointerEvent?.Invoke(this, new OptionButtonPointerEventArgs(this)
        {
            IsHovering = false,
            IsPressed  = false
        });
    }

    public override void Render(DrawingContext context)
    {
        _borderRenderHelper.Render(context,
            Bounds.Size,
            BorderUtils.BuildRenderScaleAwareThickness(BorderThickness, VisualRoot?.RenderScaling ?? 1.0),
            CornerRadius,
            BackgroundSizing.InnerBorderEdge,
            Background,
            BorderBrush,
            default);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        ConfigureTransitions();
    }
}