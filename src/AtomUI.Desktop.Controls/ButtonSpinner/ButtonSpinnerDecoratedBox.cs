using AtomUI.Animations;
using AtomUI.Desktop.Controls.Primitives.Themes;
using AtomUI.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

internal class ButtonSpinnerDecoratedBox : AddOnDecoratedBox
{
    #region 公共属性定义

    public static readonly StyledProperty<object?> SpinnerContentProperty =
        AvaloniaProperty.Register<ButtonSpinnerDecoratedBox, object?>(nameof(SpinnerContent));

    public static readonly StyledProperty<bool> IsShowHandleProperty =
        AvaloniaProperty.Register<ButtonSpinnerDecoratedBox, bool>(nameof(IsShowHandle), true);

    public static readonly StyledProperty<bool> IsHandleFloatableProperty =
        AvaloniaProperty.Register<ButtonSpinnerDecoratedBox, bool>(nameof(IsHandleFloatable), true);

    public static readonly StyledProperty<ButtonSpinnerLocation> ButtonSpinnerLocationProperty =
        ButtonSpinner.ButtonSpinnerLocationProperty.AddOwner<ButtonSpinnerDecoratedBox>();

    public object? SpinnerContent
    {
        get => GetValue(SpinnerContentProperty);
        set => SetValue(SpinnerContentProperty, value);
    }

    public bool IsShowHandle
    {
        get => GetValue(IsShowHandleProperty);
        set => SetValue(IsShowHandleProperty, value);
    }

    public bool IsHandleFloatable
    {
        get => GetValue(IsHandleFloatableProperty);
        set => SetValue(IsHandleFloatableProperty, value);
    }

    public ButtonSpinnerLocation ButtonSpinnerLocation
    {
        get => GetValue(ButtonSpinnerLocationProperty);
        set => SetValue(ButtonSpinnerLocationProperty, value);
    }

    #endregion
    
    #region 内部属性定义

    internal static readonly DirectProperty<ButtonSpinnerDecoratedBox, bool> IsButtonSpinnerVisibleProperty =
        AvaloniaProperty.RegisterDirect<ButtonSpinnerDecoratedBox, bool>(nameof(IsButtonSpinnerVisible),
            o => o.IsButtonSpinnerVisible,
            (o, v) => o.IsButtonSpinnerVisible = v);
    
    internal static readonly DirectProperty<ButtonSpinnerDecoratedBox, Thickness> SpinnerBorderThicknessProperty =
        AvaloniaProperty.RegisterDirect<ButtonSpinnerDecoratedBox, Thickness>(nameof(SpinnerBorderThickness),
            o => o.SpinnerBorderThickness,
            (o, v) => o.SpinnerBorderThickness = v);
    
    public static readonly StyledProperty<double> HandleOpacityProperty =
        AvaloniaProperty.Register<ButtonSpinnerDecoratedBox, double>(nameof(HandleOpacity));

    public static readonly StyledProperty<double> HandleOffsetProperty =
        AvaloniaProperty.Register<ButtonSpinnerDecoratedBox, double>(nameof(HandleOffset));

    public static readonly StyledProperty<double> ContentLeftShiftProperty =
        AvaloniaProperty.Register<ButtonSpinnerDecoratedBox, double>(nameof(ContentLeftShift));

    public static readonly StyledProperty<double> ContentRightShiftProperty =
        AvaloniaProperty.Register<ButtonSpinnerDecoratedBox, double>(nameof(ContentRightShift));

    internal static readonly DirectProperty<ButtonSpinnerDecoratedBox, double> SpinnerHandleWidthProperty =
        AvaloniaProperty.RegisterDirect<ButtonSpinnerDecoratedBox, double>(nameof(SpinnerHandleWidth),
            o => o.SpinnerHandleWidth,
            (o, v) => o.SpinnerHandleWidth = v);
    
    public static readonly StyledProperty<Thickness> ContentPaddingProperty =
        AvaloniaProperty.Register<ButtonSpinnerDecoratedBox, Thickness>(nameof(ContentPadding));
    
    internal static readonly DirectProperty<ButtonSpinnerDecoratedBox, Thickness> EffectiveContentPaddingProperty =
        AvaloniaProperty.RegisterDirect<ButtonSpinnerDecoratedBox, Thickness>(nameof(EffectiveContentPadding),
            o => o.EffectiveContentPadding,
            (o, v) => o.EffectiveContentPadding = v);
    
    internal static readonly DirectProperty<ButtonSpinnerDecoratedBox, bool> IsSpinnerContentHoverProperty =
        AvaloniaProperty.RegisterDirect<ButtonSpinnerDecoratedBox, bool>(nameof(IsSpinnerContentHover),
            o => o.IsSpinnerContentHover,
            (o, v) => o.IsSpinnerContentHover = v);

    private bool _showButtonSpinner;

    internal bool IsButtonSpinnerVisible
    {
        get => _showButtonSpinner;
        set => SetAndRaise(IsButtonSpinnerVisibleProperty, ref _showButtonSpinner, value);
    }
    
    private Thickness _spinnerBorderThickness;

    internal Thickness SpinnerBorderThickness
    {
        get => _spinnerBorderThickness;
        set => SetAndRaise(SpinnerBorderThicknessProperty, ref _spinnerBorderThickness, value);
    }

    internal double HandleOpacity
    {
        get => GetValue(HandleOpacityProperty);
        set => SetValue(HandleOpacityProperty, value);
    }
    
    internal double HandleOffset
    {
        get => GetValue(HandleOffsetProperty);
        set => SetValue(HandleOffsetProperty, value);
    }

    internal double ContentLeftShift
    {
        get => GetValue(ContentLeftShiftProperty);
        set => SetValue(ContentLeftShiftProperty, value);
    }

    internal double ContentRightShift
    {
        get => GetValue(ContentRightShiftProperty);
        set => SetValue(ContentRightShiftProperty, value);
    }

    private double _spinnerHandleWidth;

    internal double SpinnerHandleWidth
    {
        get => _spinnerHandleWidth;
        set => SetAndRaise(SpinnerHandleWidthProperty, ref _spinnerHandleWidth, value);
    }
    
    internal Thickness ContentPadding
    {
        get => GetValue(ContentPaddingProperty);
        set => SetValue(ContentPaddingProperty, value);
    }
    
    private Thickness _effectiveContentPadding;

    internal Thickness EffectiveContentPadding
    {
        get => _effectiveContentPadding;
        set => SetAndRaise(EffectiveContentPaddingProperty, ref _effectiveContentPadding, value);
    }
    
    private bool _isSpinnerContentHover;

    internal bool IsSpinnerContentHover
    {
        get => _isSpinnerContentHover;
        set => SetAndRaise(IsSpinnerContentHoverProperty, ref _isSpinnerContentHover, value);
    }
    
    #endregion
    
    private Panel? _overlayLayout;
    private ContentPresenter? _spinnerHandlePresenter;
    private TranslateTransform? _spinnerHandleOffsetTransform;
    private IDisposable? _pointerTrackingSubscription;
    
    protected void ConfigureEffectiveContentPadding()
    {
        if (IsShowHandle && !IsHandleFloatable)
        {
            var padding = SpinnerHandleWidth * 1.1;
            if (ButtonSpinnerLocation == ButtonSpinnerLocation.Right)
            {
                SetCurrentValue(EffectiveContentPaddingProperty, new Thickness(ContentPadding.Left, ContentPadding.Top, padding,
                    ContentPadding.Bottom));
            }
            else
            {
                SetCurrentValue(EffectiveContentPaddingProperty, new Thickness(padding, ContentPadding.Top, ContentPadding.Right,
                    ContentPadding.Bottom));
            }
        }
        else
        {
            SetCurrentValue(EffectiveContentPaddingProperty, ContentPadding);
        }
        UpdateHandleVisualState();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdateHandleVisualState();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        StopPointerTracking();
        SetSpinnerContentHover(false);
        base.OnDetachedFromVisualTree(e);
    }

    private bool ShouldTrackPointer()
    {
        return this.IsAttachedToVisualTree() &&
               IsEffectivelyEnabled &&
               IsShowHandle &&
               IsHandleFloatable;
    }

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);
        StartPointerTracking();
        UpdatePointerHover(e);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        StartPointerTracking();
        UpdatePointerHover(e);
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        StopPointerTracking();
        SetSpinnerContentHover(false);
    }

    private void UpdatePointerHover(PointerEventArgs e)
    {
        SetSpinnerContentHover(ShouldTrackPointer() && new Rect(Bounds.Size).Contains(e.GetPosition(this)));
    }

    private void StartPointerTracking()
    {
        if (_pointerTrackingSubscription != null || !ShouldTrackPointer())
        {
            return;
        }

        var inputManager = AvaloniaLocator.Current.GetService(typeof(IInputManager)) as IInputManager;
        _pointerTrackingSubscription = inputManager?.Process.Subscribe(HandlePointerTrackingEvent);
    }

    private void StopPointerTracking()
    {
        _pointerTrackingSubscription?.Dispose();
        _pointerTrackingSubscription = null;
    }

    private void HandlePointerTrackingEvent(RawInputEventArgs args)
    {
        if (!ShouldTrackPointer())
        {
            StopPointerTracking();
            SetSpinnerContentHover(false);
            return;
        }

        if (args is not RawPointerEventArgs pointerEventArgs)
        {
            return;
        }

        if (pointerEventArgs.Type != RawPointerEventType.Move &&
            pointerEventArgs.Type != RawPointerEventType.LeftButtonUp &&
            pointerEventArgs.Type != RawPointerEventType.RightButtonDown)
        {
            return;
        }

        var isHover = IsPointerInsideControl(pointerEventArgs.Position);
        SetSpinnerContentHover(isHover);
        if (!isHover)
        {
            StopPointerTracking();
        }
    }

    private bool IsPointerInsideControl(Point topLevelPosition)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null)
        {
            return false;
        }

        var origin = this.TranslatePoint(new Point(0, 0), topLevel);
        if (!origin.HasValue)
        {
            return false;
        }

        return new Rect(origin.Value, Bounds.Size).Contains(topLevelPosition);
    }

    private void SetSpinnerContentHover(bool isHover)
    {
        if (IsSpinnerContentHover == isHover)
        {
            return;
        }

        IsSpinnerContentHover = isHover;
        UpdateHandleVisualState();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsShowHandleProperty ||
            change.Property == IsHandleFloatableProperty)
        {
            if (!ShouldTrackPointer())
            {
                StopPointerTracking();
                SetSpinnerContentHover(false);
            }
            UpdateHandleVisualState();
        }

        if (change.Property == IsEffectivelyEnabledProperty)
        {
            if (!ShouldTrackPointer())
            {
                StopPointerTracking();
                SetSpinnerContentHover(false);
            }
            UpdateHandleVisualState();
        }

        if (change.Property == IsHandleFloatableProperty ||
            change.Property == IsShowHandleProperty ||
            change.Property == SpinnerHandleWidthProperty ||
            change.Property == ButtonSpinnerLocationProperty ||
            change.Property == ContentPaddingProperty)
        {
            ConfigureEffectiveContentPadding();
        }

        if (change.Property == SpinnerHandleWidthProperty ||
            change.Property == ButtonSpinnerLocationProperty)
        {
            if (change.Property == ButtonSpinnerLocationProperty)
            {
                UpdateSpinnerHandlePresenterAlignment();
            }
            UpdateHandleVisualState();
        }

        if (change.Property == SpinnerContentProperty)
        {
            ConfigureSpinnerContentPresenter();
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        ClearSpinnerContentPresenter(clearOverlay: true);
        base.OnApplyTemplate(e);
        _overlayLayout = e.NameScope.Find<Panel>("PART_OverlayLayout");
        ConfigureSpinnerContentPresenter();
        ConfigureEffectiveContentPadding();
        UpdateHandleVisualState();
    }

    private void ConfigureSpinnerContentPresenter()
    {
        if (SpinnerContent == null)
        {
            ClearSpinnerContentPresenter();
            return;
        }

        if (_overlayLayout == null)
        {
            return;
        }

        if (_spinnerHandlePresenter == null)
        {
            _spinnerHandlePresenter = CreateSpinnerHandlePresenter();
            _overlayLayout.Children.Add(_spinnerHandlePresenter);
        }

        _spinnerHandlePresenter.SetCurrentValue(ContentPresenter.ContentProperty, SpinnerContent);
        UpdateSpinnerHandlePresenterAlignment();
    }

    private ContentPresenter CreateSpinnerHandlePresenter()
    {
        _spinnerHandleOffsetTransform = new TranslateTransform();
        _spinnerHandleOffsetTransform[!TranslateTransform.XProperty] = this[!HandleOffsetProperty];

        var presenter = new ContentPresenter
        {
            Name            = "PART_SpinnerHandle",
            RenderTransform = _spinnerHandleOffsetTransform
        };
        presenter.SetTemplatedParent(this);
        presenter[!Visual.OpacityProperty] = this[!HandleOpacityProperty];
        presenter.SetValue(Panel.ZIndexProperty, AddOnDecoratedBoxThemeConstants.ActivatedZIndex);
        return presenter;
    }

    private void ClearSpinnerContentPresenter(bool clearOverlay = false)
    {
        if (_spinnerHandlePresenter == null)
        {
            if (clearOverlay)
            {
                _overlayLayout = null;
            }
            return;
        }

        _spinnerHandlePresenter.ClearValue(ContentPresenter.ContentProperty);
        _spinnerHandlePresenter.ClearValue(Visual.OpacityProperty);
        _spinnerHandlePresenter.RenderTransform = null;
        _spinnerHandlePresenter.SetTemplatedParent(null);
        if (_spinnerHandleOffsetTransform != null)
        {
            _spinnerHandleOffsetTransform.ClearValue(TranslateTransform.XProperty);
            _spinnerHandleOffsetTransform = null;
        }

        if (_spinnerHandlePresenter.GetVisualParent() is Panel parent)
        {
            parent.Children.Remove(_spinnerHandlePresenter);
        }

        _spinnerHandlePresenter = null;
        if (clearOverlay)
        {
            _overlayLayout = null;
        }
    }

    private void UpdateSpinnerHandlePresenterAlignment()
    {
        if (_spinnerHandlePresenter == null)
        {
            return;
        }

        _spinnerHandlePresenter.HorizontalAlignment = ButtonSpinnerLocation == ButtonSpinnerLocation.Left
            ? Avalonia.Layout.HorizontalAlignment.Left
            : Avalonia.Layout.HorizontalAlignment.Right;
    }

    private double GetHiddenOffset()
    {
        if (SpinnerHandleWidth <= 0)
        {
            return 0.0;
        }

        return ButtonSpinnerLocation == ButtonSpinnerLocation.Right ? SpinnerHandleWidth : -SpinnerHandleWidth;
    }

    private void UpdateHandleVisualState()
    {
        if (!IsEffectivelyEnabled)
        {
            IsSpinnerContentHover = false;
            HandleOpacity = IsShowHandle ? 1.0 : 0.0;
            HandleOffset = 0.0;
            ContentLeftShift = 0.0;
            ContentRightShift = 0.0;
            return;
        }

        var visible = IsShowHandle && (!IsHandleFloatable || IsSpinnerContentHover);
        HandleOpacity = visible ? 1.0 : 0.0;
        HandleOffset = visible ? 0.0 : GetHiddenOffset();
        if (!IsHandleFloatable)
        {
            IsSpinnerContentHover = false;
        }

        if (IsHandleFloatable && IsSpinnerContentHover)
        {
            if (ButtonSpinnerLocation == ButtonSpinnerLocation.Right)
            {
                ContentRightShift = -EffectiveContentPadding.Right * 1.5;
                ContentLeftShift = 0.0;
            }
            else
            {
                ContentLeftShift = EffectiveContentPadding.Left * 1.5;
                ContentRightShift = 0.0;
            }
        }
        else
        {
            ContentLeftShift = 0.0;
            ContentRightShift = 0.0;
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.DisableTransitions();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        Dispatcher.Post(this.EnableTransitions);
    }
}
