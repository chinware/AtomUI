using AtomUI.Controls.Switch;
using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.Media;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Rendering;

namespace AtomUI.Controls;

public class ToggleSwitch : ToggleButton,
                            ISizeTypeAware,
                            ICustomHitTest,
                            IWaveAdornerInfoProvider
{
    #region 公共属性定义

    /// <summary>
    /// Defines the <see cref="GrooveBackground" /> property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> GrooveBackgroundProperty
        = AvaloniaProperty.Register<ToggleSwitch, IBrush?>(
            nameof(GrooveBackground));

    /// <summary>
    /// Defines the <see cref="OffContent" /> property.
    /// </summary>
    public static readonly StyledProperty<object?> OffContentProperty =
        AvaloniaProperty.Register<ToggleSwitch, object?>(nameof(OffContent));

    /// <summary>
    /// Defines the <see cref="OnContent" /> property.
    /// </summary>
    public static readonly StyledProperty<object?> OnContentProperty =
        AvaloniaProperty.Register<ToggleSwitch, object?>(nameof(OnContent));

    /// <summary>
    /// 设置预置的大小类型
    /// </summary>
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        AvaloniaProperty.Register<Button, SizeType>(nameof(SizeType), SizeType.Middle);

    /// <summary>
    /// 是否处于加载状态
    /// </summary>
    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<Button, bool>(nameof(IsLoading));

    /// <summary>
    /// Gets or Sets the Content that is displayed when in the On State.
    /// </summary>
    public object? OnContent
    {
        get => GetValue(OnContentProperty);
        set => SetValue(OnContentProperty, value);
    }

    /// <summary>
    /// Gets or Sets the Content that is displayed when in the Off State.
    /// </summary>
    public object? OffContent
    {
        get => GetValue(OffContentProperty);
        set => SetValue(OffContentProperty, value);
    }

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    /// <summary>
    /// 是否处于加载状态
    /// </summary>
    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public IBrush? GrooveBackground
    {
        get => GetValue(GrooveBackgroundProperty);
        set => SetValue(GrooveBackgroundProperty, value);
    }

    #endregion

    #region 私有属性定义

    internal static readonly StyledProperty<double> InnerMaxMarginProperty
        = AvaloniaProperty.Register<ToggleSwitch, double>(nameof(InnerMaxMargin));

    internal static readonly StyledProperty<double> InnerMinMarginProperty
        = AvaloniaProperty.Register<ToggleSwitch, double>(nameof(InnerMinMargin));

    internal static readonly StyledProperty<double> TrackHeightProperty
        = AvaloniaProperty.Register<ToggleSwitch, double>(nameof(TrackHeight));

    internal static readonly StyledProperty<double> IconSizeProperty
        = AvaloniaProperty.Register<ToggleSwitch, double>(nameof(IconSize));

    internal static readonly StyledProperty<double> TrackMinWidthProperty
        = AvaloniaProperty.Register<ToggleSwitch, double>(nameof(TrackMinWidth));

    internal static readonly StyledProperty<double> TrackPaddingProperty
        = AvaloniaProperty.Register<ToggleSwitch, double>(nameof(TrackPadding));

    // 这几个属性跟动画相关
    internal static readonly StyledProperty<Point> KnobOffsetProperty
        = AvaloniaProperty.Register<ToggleSwitch, Point>(nameof(KnobOffset));

    internal double InnerMaxMargin
    {
        get => GetValue(InnerMaxMarginProperty);
        set => SetValue(InnerMaxMarginProperty, value);
    }

    internal double InnerMinMargin
    {
        get => GetValue(InnerMinMarginProperty);
        set => SetValue(InnerMinMarginProperty, value);
    }

    internal double TrackHeight
    {
        get => GetValue(TrackHeightProperty);
        set => SetValue(TrackHeightProperty, value);
    }

    internal double IconSize
    {
        get => GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    internal double TrackMinWidth
    {
        get => GetValue(TrackMinWidthProperty);
        set => SetValue(TrackMinWidthProperty, value);
    }

    internal double TrackPadding
    {
        get => GetValue(TrackPaddingProperty);
        set => SetValue(TrackPaddingProperty, value);
    }

    internal Point KnobOffset
    {
        get => GetValue(KnobOffsetProperty);
        set => SetValue(KnobOffsetProperty, value);
    }

    internal static readonly StyledProperty<Point> OnContentOffsetProperty
        = AvaloniaProperty.Register<ToggleSwitch, Point>(nameof(OnContentOffset));

    internal Point OnContentOffset
    {
        get => GetValue(OnContentOffsetProperty);
        set => SetValue(OnContentOffsetProperty, value);
    }

    internal static readonly StyledProperty<Point> OffContentOffsetProperty
        = AvaloniaProperty.Register<ToggleSwitch, Point>(nameof(OffContentOffset));

    internal Point OffContentOffset
    {
        get => GetValue(OffContentOffsetProperty);
        set => SetValue(OffContentOffsetProperty, value);
    }

    internal static readonly StyledProperty<double> SwitchOpacityProperty
        = AvaloniaProperty.Register<ToggleSwitch, double>(nameof(SwitchOpacity), 1d);

    internal double SwitchOpacity
    {
        get => GetValue(SwitchOpacityProperty);
        set => SetValue(SwitchOpacityProperty, value);
    }

    #endregion

    private const double STRETCH_FACTOR = 1.3d;
    private ControlStyleState _styleState;
    private Canvas? _togglePanel;

    private SwitchKnob? _switchKnob;

    static ToggleSwitch()
    {
        AffectsMeasure<ToggleSwitch>(SizeTypeProperty);
        AffectsArrange<ToggleSwitch>(
            IsPressedProperty,
            KnobOffsetProperty,
            OnContentOffsetProperty,
            OffContentOffsetProperty);
        AffectsRender<ToggleSwitch>(GrooveBackgroundProperty,
            SwitchOpacityProperty);
    }

    public ToggleSwitch()
    {
        LayoutUpdated += HandleLayoutUpdated;
    }

    private void HandleLayoutUpdated(object? sender, EventArgs args)
    {
        if (Transitions is null)
        {
            SetupTransitions();
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        double extraInfoWidth = 0;

        if (OffContent is Layoutable offLayoutable)
        {
            offLayoutable.Measure(availableSize);
            extraInfoWidth = Math.Max(extraInfoWidth, offLayoutable.DesiredSize.Width);
        }

        if (OnContent is Layoutable onLayoutable)
        {
            onLayoutable.Measure(availableSize);
            extraInfoWidth = Math.Max(extraInfoWidth, onLayoutable.DesiredSize.Width);
        }

        var switchHeight  = TrackHeight;
        var switchWidth   = extraInfoWidth;
        var trackMinWidth = TrackMinWidth;
        switchWidth += InnerMinMargin + InnerMaxMargin;
        switchWidth =  Math.Max(switchWidth, trackMinWidth);
        var targetSize = new Size(switchWidth, switchHeight);
        CalculateElementsOffset(targetSize);
        return targetSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (_switchKnob is not null)
        {
            Canvas.SetLeft(_switchKnob, KnobOffset.X);
            Canvas.SetTop(_switchKnob, KnobOffset.Y);
        }

        base.ArrangeOverride(finalSize);
        AdjustExtraInfoOffset();
        return finalSize;
    }

    private void AdjustExtraInfoOffset()
    {
        if (OffContent is Control offControl)
        {
            Canvas.SetLeft(offControl, OffContentOffset.X);
            Canvas.SetTop(offControl, OffContentOffset.Y);
        }

        if (OnContent is Control onControl)
        {
            Canvas.SetLeft(onControl, OnContentOffset.X);
            Canvas.SetTop(onControl, OnContentOffset.Y);
        }
    }

    private void AdjustOffsetOnPressed()
    {
        var handleRect = HandleRect();
        KnobOffset = handleRect.TopLeft;
        var handleSize = _switchKnob?.OriginKnobSize.Width ?? 0d;
        var delta      = handleRect.Width - handleSize;

        var contentOffsetDelta = handleSize * (STRETCH_FACTOR - 1);

        if (IsChecked.HasValue && IsChecked.Value)
        {
            // 点击的时候如果是选中，需要调整坐标
            KnobOffset      = new Point(KnobOffset.X - delta, KnobOffset.Y);
            OnContentOffset = new Point(OnContentOffset.X - contentOffsetDelta, OffContentOffset.Y);
        }
        else
        {
            OffContentOffset = new Point(OffContentOffset.X + contentOffsetDelta, OffContentOffset.Y);
        }

        var handleWidth = handleSize * STRETCH_FACTOR;
        if (_switchKnob is not null)
        {
            _switchKnob.KnobSize = new Size(handleWidth, handleSize);
        }
    }

    private void AdjustOffsetOnReleased()
    {
        var handleSize = _switchKnob?.OriginKnobSize.Width ?? 0d;
        if (_switchKnob is not null)
        {
            _switchKnob.KnobSize = new Size(handleSize, handleSize);
        }

        CalculateElementsOffset(Bounds.Size);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        if (!IsLoading)
        {
            base.OnPointerPressed(e);
            AdjustOffsetOnPressed();
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        if (!IsLoading)
        {
            base.OnPointerReleased(e);
            AdjustOffsetOnReleased();
            InvalidateArrange();
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        HandlePropertyChangedForStyle(e);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        HandleTemplateApplied(e.NameScope);
    }

    private void HandleTemplateApplied(INameScope scope)
    {
        _togglePanel = scope.Find<Canvas>(ToggleSwitchTheme.MainContainerPart);
        _switchKnob  = scope.Find<SwitchKnob>(ToggleSwitchTheme.SwitchKnobPart);

        if (!IsLoading)
        {
            Cursor = new Cursor(StandardCursorType.Hand);
        }

        HorizontalAlignment = HorizontalAlignment.Left;

        var offControl = SetupContent(OffContent);
        var onControl  = SetupContent(OnContent);

        // 这里会调用属性函数进行添加到布局
        if (offControl == OffContent && offControl is not null)
        {
            _togglePanel?.Children.Add(offControl);
        }

        if (onControl == OnContent && onControl is not null)
        {
            _togglePanel?.Children.Add(onControl);
        }

        OffContent = offControl;
        OnContent  = onControl;

        HandleLoadingState(IsLoading);
        CollectStyleState();
    }

    public sealed override void Render(DrawingContext context)
    {
        using var state = context.PushOpacity(SwitchOpacity);
        context.DrawPilledRect(GrooveBackground, null, GrooveRect());
    }

    public bool HitTest(Point point)
    {
        if (!IsEnabled || IsLoading)
        {
            return false;
        }

        var grooveRect = GrooveRect();
        return grooveRect.Contains(point);
    }

    private Control? SetupContent(object? content)
    {
        Control? result = default;
        if (content is Control offControl)
        {
            if (content is TemplatedControl templatedControl)
            {
                templatedControl.Padding = new Thickness(0);
            }
            else if (content is Icon iconControl)
            {
                if (iconControl.ThemeType != IconThemeType.TwoTone)
                {
                    iconControl.NormalFilledBrush ??= Foreground;
                }
            }

            offControl.Width = offControl.Height = IconSize;
            result           = offControl;
        }
        else if (content is string offStr)
        {
            var label = new TextBlock
            {
                Text = offStr,
                VerticalAlignment = VerticalAlignment.Center,
            };
            result = label;
        }

        return result;
    }

    private void CalculateElementsOffset(Size controlSize)
    {
        var isChecked  = IsChecked.HasValue && IsChecked.Value;
        var handleRect = HandleRect(isChecked, controlSize);
        KnobOffset = handleRect.TopLeft;

        var onExtraInfoRect  = ExtraInfoRect(true, controlSize);
        var offExtraInfoRect = ExtraInfoRect(false, controlSize);
        if (isChecked)
        {
            OnContentOffset  = onExtraInfoRect.TopLeft;
            OffContentOffset = new Point(controlSize.Width + 1, onExtraInfoRect.Top);
        }
        else
        {
            OffContentOffset = offExtraInfoRect.TopLeft;
            OnContentOffset  = new Point(-offExtraInfoRect.Width, offExtraInfoRect.Top);
        }
    }

    private void SetupTransitions()
    {
        Transitions = new Transitions
        {
            AnimationUtils.CreateTransition<PointTransition>(KnobOffsetProperty),
            AnimationUtils.CreateTransition<PointTransition>(OnContentOffsetProperty),
            AnimationUtils.CreateTransition<PointTransition>(OffContentOffsetProperty),
            AnimationUtils.CreateTransition<SolidColorBrushTransition>(GrooveBackgroundProperty),
            AnimationUtils.CreateTransition<DoubleTransition>(SwitchOpacityProperty)
        };
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

    private void HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == IsLoadingProperty)
        {
            HandleLoadingState(IsLoading);
        }
        else if (e.Property == OffContentProperty || e.Property == OnContentProperty)
        {
            if (VisualRoot is not null)
            {
                if (e.OldValue is Control oldChild)
                {
                    _togglePanel?.Children.Remove(oldChild);
                }

                var newControl = SetupContent(e.NewValue);

                if (e.Property == OffContentProperty)
                {
                    OffContent = newControl;
                }
                else
                {
                    OnContent = newControl;
                }

                if (newControl is not null)
                {
                    _togglePanel?.Children.Add(newControl);
                }
            }
        }

        if ((e.Property == IsPointerOverProperty && !IsLoading) ||
            e.Property == IsCheckedProperty ||
            e.Property == IsEnabledProperty)
        {
            CollectStyleState();
            if (e.Property == IsCheckedProperty)
            {
                CalculateElementsOffset(Bounds.Size);
                WaveSpiritAdorner.ShowWaveAdorner(this, WaveType.PillWave);
            }
        }
    }

    private void HandleLoadingState(bool isLoading)
    {
        if (isLoading)
        {
            Cursor = new Cursor(StandardCursorType.Arrow);
            _switchKnob?.NotifyStartLoading();
        }
        else
        {
            Cursor = new Cursor(StandardCursorType.Hand);
            _switchKnob?.NotifyStopLoading();
        }
    }

    private Rect GrooveRect()
    {
        return new Rect(new Point(0, 0), Bounds.Size);
    }

    private Rect HandleRect()
    {
        return HandleRect(IsChecked.HasValue && IsChecked.Value, Bounds.Size);
    }

    private Rect HandleRect(bool isChecked, Size controlSize)
    {
        double handlePosX;
        double handlePosY;
        var    handleSize = _switchKnob?.OriginKnobSize.Width ?? 0d;
        var    offsetX    = TrackPadding;
        var    offsetY    = TrackPadding;
        if (!isChecked)
        {
            handlePosX = offsetX;
            handlePosY = offsetY;
        }
        else
        {
            if (IsPressed)
            {
                handleSize *= STRETCH_FACTOR;
            }

            handlePosX = controlSize.Width - offsetX - handleSize;
            handlePosY = offsetY;
        }

        return new Rect(handlePosX, handlePosY, handleSize, handleSize);
    }

    private Rect ExtraInfoRect(bool isChecked, Size controlSize)
    {
        var    innerMinMargin = InnerMinMargin;
        var    innerMaxMargin = InnerMaxMargin;
        double yAdjustValue   = 0;
        var    targetRect     = new Rect(new Point(0, 0), controlSize);
        if (isChecked)
        {
            if (OffContent is Control offControl)
            {
                yAdjustValue = (controlSize.Height - offControl.DesiredSize.Height) / 2;
            }

            targetRect = targetRect.Inflate(new Thickness(-innerMinMargin, -yAdjustValue, innerMaxMargin, 0));
        }
        else
        {
            if (OnContent is Control onControl)
            {
                yAdjustValue = (controlSize.Height - onControl.DesiredSize.Height) / 2;
            }

            targetRect = targetRect.Inflate(new Thickness(-innerMaxMargin, -yAdjustValue, innerMinMargin, 0));
        }

        return targetRect;
    }

    public Rect WaveGeometry()
    {
        return GrooveRect();
    }

    public CornerRadius WaveBorderRadius()
    {
        return new CornerRadius(DesiredSize.Height / 2);
    }
}