using AtomUI.Controls.Utils;
using AtomUI.Media;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;

namespace AtomUI.Controls;

using AvaloniaRadioButton = Avalonia.Controls.RadioButton;
using ButtonSizeType = SizeType;


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
    public OptionButtonPointerEventArgs(OptionButton button)
    {
        Button = button;
    }

    public OptionButton? Button { get; }
    public bool IsPressed { get; set; }
    public bool IsHovering { get; set; }
}


public class OptionButton : AvaloniaRadioButton,
    ISizeTypeAware,
    IWaveAdornerInfoProvider,
    IControlCustomStyle
{
    public static readonly StyledProperty<ButtonSizeType> SizeTypeProperty =
        AvaloniaProperty.Register<OptionButton, ButtonSizeType>(nameof(SizeType), ButtonSizeType.Middle);

    public static readonly StyledProperty<OptionButtonStyle> ButtonStyleProperty =
        AvaloniaProperty.Register<OptionButton, OptionButtonStyle>(nameof(ButtonStyle));

    public static readonly StyledProperty<string?> TextProperty
        = AvaloniaProperty.Register<OptionButton, string?>(nameof(Text));

    internal static readonly DirectProperty<OptionButton, bool> InOptionGroupProperty =
        AvaloniaProperty.RegisterDirect<OptionButton, bool>(
            nameof(InOptionGroup),
            o => o.InOptionGroup,
            (o, v) => o.InOptionGroup = v);

    private static readonly DirectProperty<OptionButton, OptionButtonPositionTrait> GroupPositionTraitProperty =
        AvaloniaProperty.RegisterDirect<OptionButton, OptionButtonPositionTrait>(
            nameof(GroupPositionTrait),
            o => o.GroupPositionTrait,
            (o, v) => o.GroupPositionTrait = v,
            OptionButtonPositionTrait.OnlyOne);

    private readonly BorderRenderHelper _borderRenderHelper;
    private readonly IControlCustomStyle _customStyle;

    private OptionButtonPositionTrait _groupPositionTrait = OptionButtonPositionTrait.OnlyOne;

    private bool _inOptionGroup;
    private CornerRadius? _originCornerRadius;
    private ControlStyleState _styleState;

    static OptionButton()
    {
        AffectsMeasure<OptionButton>(SizeTypeProperty, ButtonStyleProperty, InOptionGroupProperty);
        AffectsRender<OptionButton>(IsCheckedProperty, CornerRadiusProperty, ForegroundProperty, BackgroundProperty);
    }

    public OptionButton()
    {
        _customStyle        = this;
        _borderRenderHelper = new BorderRenderHelper();
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

    /// <summary>
    ///     是否在 Group 中渲染
    /// </summary>
    internal bool InOptionGroup
    {
        get => _inOptionGroup;
        set => SetAndRaise(InOptionGroupProperty, ref _inOptionGroup, value);
    }

    internal OptionButtonPositionTrait GroupPositionTrait
    {
        get => _groupPositionTrait;
        set => SetAndRaise(GroupPositionTraitProperty, ref _groupPositionTrait, value);
    }

    void IControlCustomStyle.HandleTemplateApplied(INameScope scope)
    {
        if (Text is null && Content is string content) Text = content;

        Cursor = new Cursor(StandardCursorType.Hand);
        HandleSizeTypeChanged();
        _customStyle.CollectStyleState();
        _customStyle.SetupTransitions();
    }

    public ButtonSizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    internal event EventHandler<OptionButtonPointerEventArgs>? OptionButtonPointerEvent;

    protected override Size MeasureOverride(Size availableSize)
    {
        var size         = base.MeasureOverride(availableSize);
        var targetWidth  = size.Width;
        var targetHeight = size.Height;
        targetHeight += Padding.Top  + Padding.Bottom;
        targetWidth  += Padding.Left + Padding.Right;
        return new Size(targetWidth, targetHeight);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        _customStyle.HandlePropertyChangedForStyle(e);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _customStyle.HandleTemplateApplied(e.NameScope);
    }

    private void HandleSizeTypeChanged()
    {
        _originCornerRadius = CornerRadius;
        CornerRadius        = BuildCornerRadius(GroupPositionTrait, _originCornerRadius!.Value);
    }



    #region IControlCustomStyle 实现

    public Rect WaveGeometry()
    {
        return new Rect(0, 0, Bounds.Width, Bounds.Height);
    }

    public CornerRadius WaveBorderRadius()
    {
        return CornerRadius;
    }

    void IControlCustomStyle.SetupTransitions()
    {
        var transitions = new Transitions();
        if (ButtonStyle == OptionButtonStyle.Solid)
            transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty));
        else if (ButtonStyle == OptionButtonStyle.Outline)
            transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(BorderBrushProperty));

        transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty));
        Transitions = transitions;
    }

    void IControlCustomStyle.CollectStyleState()
    {
        ControlStateUtils.InitCommonState(this, ref _styleState);
        if (IsPressed)
            _styleState |= ControlStyleState.Sunken;
        else
            _styleState |= ControlStyleState.Raised;

        if (IsChecked.HasValue && IsChecked.Value) _styleState |= ControlStyleState.Selected;
    }

    void IControlCustomStyle.HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == IsPointerOverProperty ||
            e.Property == IsPressedProperty     ||
            e.Property == IsCheckedProperty)
        {
            _customStyle.CollectStyleState();
            if (e.Property == IsPressedProperty)
                if (_styleState.HasFlag(ControlStyleState.Raised))
                    WaveSpiritAdorner.ShowWaveAdorner(this, WaveType.RoundRectWave);
        }

        if (e.Property == GroupPositionTraitProperty)
            if (_originCornerRadius.HasValue)
                CornerRadius = BuildCornerRadius(GroupPositionTrait, _originCornerRadius!.Value);
    }

    private CornerRadius BuildCornerRadius(OptionButtonPositionTrait positionTrait, CornerRadius cornerRadius)
    {
        if (positionTrait == OptionButtonPositionTrait.First)
            return new CornerRadius(cornerRadius.TopLeft,
                0,
                0,
                cornerRadius.BottomLeft);
        if (positionTrait == OptionButtonPositionTrait.Last)
            return new CornerRadius(0,
                cornerRadius.TopRight,
                cornerRadius.BottomRight,
                0);
        if (positionTrait == OptionButtonPositionTrait.Middle) return new CornerRadius(0);

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

    #endregion
}