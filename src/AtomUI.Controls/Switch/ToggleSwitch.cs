using AtomUI.Animations;
using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Rendering;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public class ToggleSwitch : ToggleButton,
                            ISizeTypeAware,
                            ICustomHitTest,
                            IWaveAdornerInfoProvider,
                            IWaveSpiritAwareControl,
                            IControlSharedTokenResourcesHost
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
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<ToggleSwitch>();

    /// <summary>
    /// 是否处于加载状态
    /// </summary>
    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<ToggleSwitch, bool>(nameof(IsLoading));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<ToggleSwitch>();

    public static readonly StyledProperty<bool> IsWaveSpiritEnabledProperty =
        WaveSpiritAwareControlProperty.IsWaveSpiritEnabledProperty.AddOwner<ToggleSwitch>();

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

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public bool IsWaveSpiritEnabled
    {
        get => GetValue(IsWaveSpiritEnabledProperty);
        set => SetValue(IsWaveSpiritEnabledProperty, value);
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
    internal static readonly StyledProperty<Rect> KnobRectProperty
        = AvaloniaProperty.Register<ToggleSwitch, Rect>(nameof(KnobRect));

    public static readonly StyledProperty<Size> KnobSizeProperty
        = AvaloniaProperty.Register<ToggleSwitch, Size>(nameof(KnobSize));

    internal static readonly StyledProperty<Rect> KnobMovingRectProperty
        = AvaloniaProperty.Register<ToggleSwitch, Rect>(nameof(KnobMovingRect));

    internal static readonly StyledProperty<Point> OnContentOffsetProperty
        = AvaloniaProperty.Register<ToggleSwitch, Point>(nameof(OnContentOffset));

    internal static readonly StyledProperty<Point> OffContentOffsetProperty
        = AvaloniaProperty.Register<ToggleSwitch, Point>(nameof(OffContentOffset));

    internal static readonly StyledProperty<double> SwitchOpacityProperty
        = AvaloniaProperty.Register<ToggleSwitch, double>(nameof(SwitchOpacity), 1d);

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

    internal Rect KnobRect
    {
        get => GetValue(KnobRectProperty);
        set => SetValue(KnobRectProperty, value);
    }

    public Size KnobSize
    {
        get => GetValue(KnobSizeProperty);
        set => SetValue(KnobSizeProperty, value);
    }

    internal Rect KnobMovingRect
    {
        get => GetValue(KnobMovingRectProperty);
        set => SetValue(KnobMovingRectProperty, value);
    }

    internal Point OnContentOffset
    {
        get => GetValue(OnContentOffsetProperty);
        set => SetValue(OnContentOffsetProperty, value);
    }

    internal Point OffContentOffset
    {
        get => GetValue(OffContentOffsetProperty);
        set => SetValue(OffContentOffsetProperty, value);
    }

    internal double SwitchOpacity
    {
        get => GetValue(SwitchOpacityProperty);
        set => SetValue(SwitchOpacityProperty, value);
    }

    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => ToggleSwitchToken.ID;

    #endregion

    private const double STRETCH_FACTOR = 1.3d;
    private Canvas? _togglePanel;
    private bool _isCheckedChanged = false;
    private SwitchKnob? _switchKnob;

    static ToggleSwitch()
    {
        AffectsMeasure<ToggleSwitch>(SizeTypeProperty, IsCheckedProperty);
        AffectsArrange<ToggleSwitch>(
            IsPressedProperty,
            KnobRectProperty,
            KnobMovingRectProperty,
            OnContentOffsetProperty,
            OffContentOffsetProperty);
        AffectsRender<ToggleSwitch>(GrooveBackgroundProperty,
            SwitchOpacityProperty);
    }

    public ToggleSwitch()
    {
        LayoutUpdated += HandleLayoutUpdated;
        this.RegisterResources();
        this.BindWaveSpiritProperties();
    }

    private void HandleLayoutUpdated(object? sender, EventArgs args)
    {
        if (IsMotionEnabled)
        {
            Transitions ??= new Transitions
            {
                TransitionUtils.CreateTransition<RectTransition>(KnobMovingRectProperty),
                TransitionUtils.CreateTransition<PointTransition>(OnContentOffsetProperty),
                TransitionUtils.CreateTransition<PointTransition>(OffContentOffsetProperty),
                TransitionUtils.CreateTransition<SolidColorBrushTransition>(GrooveBackgroundProperty),
                TransitionUtils.CreateTransition<DoubleTransition>(SwitchOpacityProperty)
            };
        }
        else
        {
            Transitions = null;
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        base.MeasureOverride(availableSize);
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
        if (_switchKnob is not null)
        {
            _switchKnob.Measure(KnobRect.Size);
        }

        return targetSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (_switchKnob is not null)
        {
            if (!_isCheckedChanged)
            {
                _switchKnob.Arrange(KnobRect);
            }
            else
            {
                _switchKnob.Arrange(KnobMovingRect);
            }
        }

        AdjustExtraInfoOffset();
        return finalSize;
    }

    private void AdjustExtraInfoOffset()
    {
        if (OffContent is Control offControl)
        {
            offControl.Arrange(new Rect(new Point(OffContentOffset.X, OffContentOffset.Y), offControl.DesiredSize));
        }

        if (OnContent is Control onControl)
        {
            onControl.Arrange(new Rect(new Point(OnContentOffset.X, OnContentOffset.Y), onControl.DesiredSize));
        }
    }

    private void AdjustOffsetOnPressed()
    {
        var handleRect = HandleRect();
        var handleSize = handleRect.Width;

        var contentOffsetDelta = handleSize * (STRETCH_FACTOR - 1);

        if (IsChecked.HasValue && IsChecked.Value)
        {
            // 点击的时候如果是选中，需要调整坐标
            OnContentOffset = new Point(OnContentOffset.X - contentOffsetDelta, OffContentOffset.Y);
        }
        else
        {
            OffContentOffset = new Point(OffContentOffset.X + contentOffsetDelta, OffContentOffset.Y);
        }

        if (_switchKnob is not null)
        {
            _switchKnob.KnobSize = new Size(handleSize, KnobSize.Height);
        }

        KnobRect       = handleRect;
        KnobMovingRect = KnobRect;
    }

    private void AdjustOffsetOnReleased()
    {
        CalculateElementsOffset(Bounds.Size);

        if (_switchKnob is not null)
        {
            _switchKnob.KnobSize = KnobSize;
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        if (!IsLoading)
        {
            base.OnPointerPressed(e);
            if (!MathUtils.AreClose(KnobMovingRect.X, KnobRect.X))
            {
                return;
            }

            _isCheckedChanged = false;
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
        if (e.Property == IsLoadingProperty)
        {
            HandleLoadingState(IsLoading);
        }
        else if ((e.Property == IsPointerOverProperty && !IsLoading) ||
                 e.Property == IsCheckedProperty ||
                 e.Property == IsEnabledProperty)
        {
            if (e.Property == IsCheckedProperty && IsMotionEnabled)
            {
                CalculateElementsOffset(Bounds.Size);
                WaveSpiritAdorner.ShowWaveAdorner(this, WaveType.PillWave);
            }
        }
        else if (e.Property == KnobSizeProperty)
        {
            if (_switchKnob is not null)
            {
                _switchKnob.KnobSize = KnobSize;
            }
        }

        if (e.Property == IsCheckedProperty)
        {
            _isCheckedChanged = true;
        }

        if (this.IsAttachedToVisualTree())
        {
            if (e.Property == OffContentProperty || e.Property == OnContentProperty)
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
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        var scope = e.NameScope;
        _togglePanel = scope.Find<Canvas>(ToggleSwitchThemeConstants.MainContainerPart);
        _switchKnob  = scope.Find<SwitchKnob>(ToggleSwitchThemeConstants.SwitchKnobPart);
        if (_switchKnob is not null)
        {
            _switchKnob.KnobSize = KnobSize;
        }

        if (!IsLoading)
        {
            Cursor = new Cursor(StandardCursorType.Hand);
        }

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
    }

    public sealed override void Render(DrawingContext context)
    {
        using var state = context.PushOpacity(SwitchOpacity);
        context.DrawPilledRect(GrooveBackground, null, new Rect(0, 0, Bounds.Width, Bounds.Height));
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
        Control? result = null;
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
                    iconControl.NormalFilledBrush = Foreground;
                }
            }

            offControl.Width = offControl.Height = IconSize;
            result           = offControl;
        }
        else if (content is string offStr)
        {
            var label = new TextBlock()
            {
                Text              = offStr,
                VerticalAlignment = VerticalAlignment.Center,
            };
            result = label;
        }

        return result;
    }

    private void CalculateElementsOffset(Size controlSize)
    {
        var isChecked = IsChecked.HasValue && IsChecked.Value;
        KnobRect       = HandleRect(isChecked, controlSize);
        KnobMovingRect = KnobRect;

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
        var    handleSize = KnobSize.Width;
        var    offsetX    = TrackPadding;
        var    offsetY    = TrackPadding;
        if (IsPressed)
        {
            handleSize *= STRETCH_FACTOR;
        }

        if (!isChecked)
        {
            handlePosX = offsetX;
            handlePosY = offsetY;
        }
        else
        {
            handlePosX = controlSize.Width - offsetX - handleSize;
            handlePosY = offsetY;
        }

        return new Rect(handlePosX, handlePosY, handleSize, KnobSize.Height);
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