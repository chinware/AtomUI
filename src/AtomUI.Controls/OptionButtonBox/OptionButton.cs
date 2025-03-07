using System.Diagnostics;
using AtomUI.Controls.Utils;
using AtomUI.Media;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Media;

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

    public static readonly StyledProperty<string?> TextProperty
        = AvaloniaProperty.Register<OptionButton, string?>(nameof(Text));

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

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
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
        = AnimationAwareControlProperty.IsMotionEnabledProperty.AddOwner<OptionButton>();

    internal static readonly DirectProperty<OptionButton, bool> IsWaveAnimationEnabledProperty
        = AvaloniaProperty.RegisterDirect<OptionButton, bool>(nameof(IsWaveAnimationEnabled),
            o => o.IsWaveAnimationEnabled,
            (o, v) => o.IsWaveAnimationEnabled = v);

    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    private bool _isWaveAnimationEnabled = true;

    internal bool IsWaveAnimationEnabled
    {
        get => _isWaveAnimationEnabled;
        set => SetAndRaise(IsWaveAnimationEnabledProperty, ref _isWaveAnimationEnabled, value);
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
        SetupTransitions();
        Debug.Assert(Parent is OptionButtonGroup, "OptionButton parent must be type of OptionButtonGroup");
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == IsPointerOverProperty ||
            e.Property == IsPressedProperty ||
            e.Property == IsCheckedProperty)
        {
            if (e.Property == IsPressedProperty && e.OldValue as bool? == true && IsWaveAnimationEnabled)
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

        if (this.IsAttachedToLogicalTree())
        {
            if (e.Property == IsMotionEnabledProperty)
            {
                SetupTransitions();
            }
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (Text is null && Content is string content)
        {
            Text = content;
        }

        HandleSizeTypeChanged();
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

    private void SetupTransitions()
    {
        if (IsMotionEnabled)
        {
            var transitions = new Transitions();
            if (ButtonStyle == OptionButtonStyle.Solid)
            {
                transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty));
            }
            else if (ButtonStyle == OptionButtonStyle.Outline)
            {
                transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(BorderBrushProperty));
            }

            transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty));
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
}