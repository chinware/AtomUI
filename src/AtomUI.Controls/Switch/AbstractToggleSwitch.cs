using System.Reactive.Disposables;
using AtomUI.Animations;
using AtomUI.Controls.Primitives;
using AtomUI.Data;
using AtomUI.Media;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Rendering;

namespace AtomUI.Controls;

[TemplatePart("PART_SwitchKnob", typeof(SwitchKnob))]
public abstract class AbstractToggleSwitch : ToggleButton,
                                             ISizeTypeAware,
                                             ICustomHitTest,
                                             IWaveSpiritAwareControl,
                                             IFormItemAware
{
    #region 公共属性定义
    
    public static readonly StyledProperty<IBrush?> GrooveBackgroundProperty =
        AvaloniaProperty.Register<AbstractToggleSwitch, IBrush?>(
            nameof(GrooveBackground));

    public static readonly StyledProperty<object?> OnContentProperty =
        AvaloniaProperty.Register<AbstractToggleSwitch, object?>(nameof(OnContent));
    
    public static readonly StyledProperty<IDataTemplate?> OnContentTemplateProperty = 
        AvaloniaProperty.Register<AbstractToggleSwitch, IDataTemplate?>(nameof(OnContentTemplate));
    
    public static readonly StyledProperty<object?> OffContentProperty =
        AvaloniaProperty.Register<AbstractToggleSwitch, object?>(nameof(OffContent));

    public static readonly StyledProperty<IDataTemplate?> OffContentTemplateProperty = 
        AvaloniaProperty.Register<AbstractToggleSwitch, IDataTemplate?>(nameof(OffContentTemplate));
    
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<AbstractToggleSwitch>();
    
    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<AbstractToggleSwitch, bool>(nameof(IsLoading));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<AbstractToggleSwitch>();

    public static readonly StyledProperty<bool> IsWaveSpiritEnabledProperty =
        WaveSpiritAwareControlProperty.IsWaveSpiritEnabledProperty.AddOwner<AbstractToggleSwitch>();
    
    [DependsOn(nameof(OnContentTemplate))]
    public object? OnContent
    {
        get => GetValue(OnContentProperty);
        set => SetValue(OnContentProperty, value);
    }
    
    public IDataTemplate? OnContentTemplate
    {
        get => GetValue(OnContentTemplateProperty);
        set => SetValue(OnContentTemplateProperty, value);
    }
    
    [DependsOn(nameof(OffContentTemplate))]
    public object? OffContent
    {
        get => GetValue(OffContentProperty);
        set => SetValue(OffContentProperty, value);
    }
    
    public IDataTemplate? OffContentTemplate
    {
        get => GetValue(OffContentTemplateProperty);
        set => SetValue(OffContentTemplateProperty, value);
    }

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
    
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

    #region 内部属性定义

    internal static readonly StyledProperty<double> InnerMaxMarginProperty =
        AvaloniaProperty.Register<AbstractToggleSwitch, double>(nameof(InnerMaxMargin));

    internal static readonly StyledProperty<double> InnerMinMarginProperty =
        AvaloniaProperty.Register<AbstractToggleSwitch, double>(nameof(InnerMinMargin));

    internal static readonly StyledProperty<double> TrackHeightProperty =
        AvaloniaProperty.Register<AbstractToggleSwitch, double>(nameof(TrackHeight));

    internal static readonly StyledProperty<double> IconSizeProperty =
        AvaloniaProperty.Register<AbstractToggleSwitch, double>(nameof(IconSize));

    internal static readonly StyledProperty<double> TrackMinWidthProperty =
        AvaloniaProperty.Register<AbstractToggleSwitch, double>(nameof(TrackMinWidth));

    internal static readonly StyledProperty<double> TrackPaddingProperty =
        AvaloniaProperty.Register<AbstractToggleSwitch, double>(nameof(TrackPadding));

    // 这几个属性跟动画相关
    internal static readonly StyledProperty<Rect> KnobRectProperty = 
        AvaloniaProperty.Register<AbstractToggleSwitch, Rect>(nameof(KnobRect));

    internal static readonly StyledProperty<Size> KnobSizeProperty =
        AvaloniaProperty.Register<AbstractToggleSwitch, Size>(nameof(KnobSize));

    internal static readonly StyledProperty<Rect> KnobMovingRectProperty = 
        AvaloniaProperty.Register<AbstractToggleSwitch, Rect>(nameof(KnobMovingRect));

    internal static readonly StyledProperty<Point> OnContentOffsetProperty =
        AvaloniaProperty.Register<AbstractToggleSwitch, Point>(nameof(OnContentOffset));

    internal static readonly StyledProperty<Point> OffContentOffsetProperty = 
        AvaloniaProperty.Register<AbstractToggleSwitch, Point>(nameof(OffContentOffset));

    internal static readonly StyledProperty<double> SwitchOpacityProperty = 
        AvaloniaProperty.Register<AbstractToggleSwitch, double>(nameof(SwitchOpacity), 1d);

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

    internal Size KnobSize
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
    
    #endregion

    private const double STRETCH_FACTOR = 1.3d;
    private ContentPresenter? _onContentPresenter;
    private ContentPresenter? _offContentPresenter;
    
    private bool _isCheckedChanged = false;
    private SwitchKnob? _switchKnob;
    private CompositeDisposable? _onBindingDisposables;
    private CompositeDisposable? _offBindingDisposables;
    private WaveSpiritDecorator? _waveSpiritDecorator;
    private Canvas? _mainLayout;
    
    static AbstractToggleSwitch()
    {
        AffectsMeasure<AbstractToggleSwitch>(SizeTypeProperty, IsCheckedProperty);
        AffectsArrange<AbstractToggleSwitch>(
            IsPressedProperty,
            KnobRectProperty,
            KnobMovingRectProperty,
            OnContentOffsetProperty,
            OffContentOffsetProperty);
        AffectsRender<AbstractToggleSwitch>(GrooveBackgroundProperty, SwitchOpacityProperty);
        IsCheckedProperty.Changed.AddClassHandler<AbstractToggleSwitch>((toggleSwitch, args) => toggleSwitch.NotifyFormValueChanged(args.NewValue as bool?));
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        base.MeasureOverride(availableSize);
        double extraInfoWidth = 0;

        if (_offContentPresenter != null)
        {
            _offContentPresenter.Measure(availableSize);
            extraInfoWidth = Math.Max(extraInfoWidth, _offContentPresenter.DesiredSize.Width);
        }

        if (_onContentPresenter != null)
        {
            _onContentPresenter.Measure(availableSize);
            extraInfoWidth = Math.Max(extraInfoWidth, _onContentPresenter.DesiredSize.Width);
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
        _mainLayout?.Arrange(new Rect(DesiredSize));
        if (!_isCheckedChanged)
        {
            _switchKnob?.Arrange(KnobRect);
        }
        else
        {
            _switchKnob?.Arrange(KnobMovingRect);
        }
        
        _offContentPresenter?.Arrange(new Rect(new Point(OffContentOffset.X, OffContentOffset.Y), _offContentPresenter.DesiredSize));
        _onContentPresenter?.Arrange(new Rect(new Point(OnContentOffset.X, OnContentOffset.Y), _onContentPresenter.DesiredSize));
        _waveSpiritDecorator?.Arrange(new Rect(new Point(0, 0), DesiredSize.Deflate(Margin)));
        
        return finalSize;
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
        CalculateElementsOffset(GrooveRect().Size);

        if (_switchKnob is not null)
        {
            // 延迟更新 KnobSize，让 Transition 动画有时间完成
            this.Dispatcher.InvokeAsync(() =>
            {
                if (_switchKnob is not null)
                {
                    _switchKnob.KnobSize = KnobSize;
                }
            });
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

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsLoadingProperty)
        {
            HandleLoadingState(IsLoading);
        }
        else if ((change.Property == IsPointerOverProperty && !IsLoading) ||
                 change.Property == IsCheckedProperty ||
                 change.Property == IsEnabledProperty)
        {
            if (change.Property == IsCheckedProperty && IsMotionEnabled)
            {
                CalculateElementsOffset(GrooveRect().Size);
                _waveSpiritDecorator?.Play();
            }
        }
        else if (change.Property == KnobSizeProperty)
        {
            if (_switchKnob is not null)
            {
                _switchKnob.KnobSize = KnobSize;
            }
        }

        if (change.Property == OffContentProperty ||
            change.Property == OnContentProperty)
        {
            SetupContent(change.OldValue, change.NewValue, change.Property == OnContentProperty);
        }
        else if (change.Property == IsCheckedProperty)
        {
            _isCheckedChanged = true;
        }

    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        var scope = e.NameScope;
        _switchKnob  = scope.Find<SwitchKnob>("PART_SwitchKnob");
        if (_switchKnob is not null)
        {
            _switchKnob.KnobSize = KnobSize;
        }

        _onContentPresenter  = scope.Find<ContentPresenter>("PART_OnContentPresenter");
        _offContentPresenter = scope.Find<ContentPresenter>("PART_OffContentPresenter");
        _waveSpiritDecorator = scope.Find<WaveSpiritDecorator>("PART_WaveSpirit");
        _mainLayout          = scope.Find<Canvas>("PART_MainContainer");
        HandleLoadingState(IsLoading);
    }

    public sealed override void Render(DrawingContext context)
    {
        using var state = context.PushOpacity(SwitchOpacity);
        var       size  = DesiredSize.Deflate(Margin);
        context.DrawPilledRect(GrooveBackground, null, new Rect(new Point(0, 0), size));
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

    private void SetupContent(object? oldContent, object? newContent, bool isOnContent)
    {
        if (oldContent != null)
        {
            if (isOnContent)
            {
                _onBindingDisposables?.Dispose();
                _onBindingDisposables = null;
            }
            else
            {
                _offBindingDisposables?.Dispose();
                _offBindingDisposables = null;
            }
        }
        if (newContent is PathIcon newPathIcon)
        {
            var disposables = new CompositeDisposable(4);
            disposables.Add(BindUtils.RelayBind(this, IconSizeProperty, newPathIcon, WidthProperty));
            disposables.Add(BindUtils.RelayBind(this, IconSizeProperty, newPathIcon, HeightProperty));
            disposables.Add(BindUtils.RelayBind(this, ForegroundProperty, newPathIcon, ForegroundProperty));
            if (newPathIcon is Icon icon)
            {
                disposables.Add(BindUtils.RelayBind(this, ForegroundProperty, icon, Icon.FillBrushProperty));
                disposables.Add(BindUtils.RelayBind(this, ForegroundProperty, icon, Icon.StrokeBrushProperty));
            }
            if (isOnContent)
            {
                _onBindingDisposables = disposables;
            }
            else
            {
                _offBindingDisposables = disposables;
            }
        }
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
        return new Rect(new Point(0, 0), DesiredSize.Deflate(Margin));
    }

    private Rect HandleRect()
    {
        return HandleRect(IsChecked.HasValue && IsChecked.Value, GrooveRect().Size);
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
            if (_offContentPresenter != null)
            {
                yAdjustValue = (controlSize.Height - _offContentPresenter.DesiredSize.Height) / 2;
            }

            targetRect = targetRect.Inflate(new Thickness(-innerMinMargin, -yAdjustValue, innerMaxMargin, 0));
        }
        else
        {
            if (_onContentPresenter != null)
            {
                yAdjustValue = (controlSize.Height - _onContentPresenter.DesiredSize.Height) / 2;
            }

            targetRect = targetRect.Inflate(new Thickness(-innerMaxMargin, -yAdjustValue, innerMinMargin, 0));
        }

        return targetRect;
    }
    
    #region 实现 FormItem 接口
    
    private EventHandler? _formValueChanged;
    event EventHandler? IFormItemAware.ValueChanged
    {
        add => _formValueChanged += value;
        remove => _formValueChanged -= value;
    }

    void IFormItemAware.SetFormValue(object? value) => NotifySetFormValue(value as bool?);

    object? IFormItemAware.GetFormValue() => NotifyGetFormValue();
    void IFormItemAware.ClearFormValue() => NotifyClearFormValue();
    void IFormItemAware.NotifyValidateStatus(FormValidateStatus status) => NotifyValidateStatus(status);
    
    protected virtual void NotifyFormValueChanged(object? value)
    {
        _formValueChanged?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void NotifySetFormValue(bool? value)
    {
        SetCurrentValue(IsCheckedProperty, value);
    }

    protected virtual object? NotifyGetFormValue()
    {
        return IsChecked;
    }

    protected virtual void NotifyClearFormValue()
    {
        SetCurrentValue(IsCheckedProperty, null);
    }

    protected virtual void NotifyValidateStatus(FormValidateStatus status)
    {
    }
    #endregion

    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.DisableTransitions();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        this.Dispatcher.Post(this.EnableTransitions);
    }
}