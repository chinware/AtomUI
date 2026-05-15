using AtomUI.Controls;
using AtomUI.Icons.AntDesign;
using AtomUI.Reflection;
using AtomUI.Theme;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

public class Carousel : SelectingItemsControl, IMotionAwareControl
{
    #region 公共属性定义
    public static readonly StyledProperty<bool> IsShowNavButtonsProperty = 
        AvaloniaProperty.Register<Carousel, bool>(nameof(IsShowNavButtons), false);
    
    public static readonly StyledProperty<bool> IsAutoPlayProperty = 
        AvaloniaProperty.Register<Carousel, bool>(nameof(IsAutoPlay));
    
    public static readonly StyledProperty<TimeSpan> AutoPlaySpeedProperty = 
        AvaloniaProperty.Register<Carousel, TimeSpan>(nameof(AutoPlaySpeed), TimeSpan.FromMilliseconds(3000));
    
    public static readonly StyledProperty<CarouselPaginationPosition> PaginationPositionProperty = 
        AvaloniaProperty.Register<Carousel, CarouselPaginationPosition>(nameof(PaginationPosition), CarouselPaginationPosition.Bottom);
    
    public static readonly StyledProperty<bool> IsShowPaginationProperty = 
        AvaloniaProperty.Register<Carousel, bool>(nameof(IsShowPagination), true);
    
    public static readonly StyledProperty<bool> IsShowTransitionProgressProperty = 
        AvaloniaProperty.Register<Carousel, bool>(nameof(IsShowTransitionProgress), false);
    
    public static readonly StyledProperty<bool> IsInfiniteProperty = 
        AvaloniaProperty.Register<Carousel, bool>(nameof(IsInfinite), true);
    
    public static readonly StyledProperty<TimeSpan> PageTransitionDurationProperty = 
        AvaloniaProperty.Register<Carousel, TimeSpan>(nameof(PageTransitionDuration));
    
    public static readonly StyledProperty<Easing> PageInEasingProperty = 
        AvaloniaProperty.Register<Carousel, Easing>(nameof(PageInEasing));
    
    public static readonly StyledProperty<Easing> PageOutEasingProperty = 
        AvaloniaProperty.Register<Carousel, Easing>(nameof(PageOutEasing));
    
    public static readonly StyledProperty<CarouselTransitionEffect> TransitionEffectProperty = 
        AvaloniaProperty.Register<Carousel, CarouselTransitionEffect>(nameof(TransitionEffect));
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Carousel>();

    public static readonly StyledProperty<bool> IsSwipeEnabledProperty =
        AvaloniaProperty.Register<Carousel, bool>(nameof(IsSwipeEnabled));
    
    public bool IsShowNavButtons
    {
        get => GetValue(IsShowNavButtonsProperty);
        set => SetValue(IsShowNavButtonsProperty, value);
    }
    
    public bool IsAutoPlay
    {
        get => GetValue(IsAutoPlayProperty);
        set => SetValue(IsAutoPlayProperty, value);
    }
    
    public TimeSpan AutoPlaySpeed
    {
        get => GetValue(AutoPlaySpeedProperty);
        set => SetValue(AutoPlaySpeedProperty, value);
    }
    
    public CarouselPaginationPosition PaginationPosition
    {
        get => GetValue(PaginationPositionProperty);
        set => SetValue(PaginationPositionProperty, value);
    }
    
    public bool IsShowPagination
    {
        get => GetValue(IsShowPaginationProperty);
        set => SetValue(IsShowPaginationProperty, value);
    }
    
    public bool IsShowTransitionProgress
    {
        get => GetValue(IsShowTransitionProgressProperty);
        set => SetValue(IsShowTransitionProgressProperty, value);
    }
    
    public bool IsInfinite
    {
        get => GetValue(IsInfiniteProperty);
        set => SetValue(IsInfiniteProperty, value);
    }
    
    public TimeSpan PageTransitionDuration
    {
        get => GetValue(PageTransitionDurationProperty);
        set => SetValue(PageTransitionDurationProperty, value);
    }

    public Easing PageInEasing
    {
        get => GetValue(PageInEasingProperty);
        set => SetValue(PageInEasingProperty, value);
    }

    public Easing PageOutEasing
    {
        get => GetValue(PageOutEasingProperty);
        set => SetValue(PageOutEasingProperty, value);
    }

    public CarouselTransitionEffect TransitionEffect
    {
        get => GetValue(TransitionEffectProperty);
        set => SetValue(TransitionEffectProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public bool IsSwipeEnabled
    {
        get => GetValue(IsSwipeEnabledProperty);
        set => SetValue(IsSwipeEnabledProperty, value);
    }
    #endregion

    #region 内部属性定义
    
    internal static readonly StyledProperty<IPageTransition?> PageTransitionProperty =
        AvaloniaProperty.Register<Carousel, IPageTransition?>(nameof(PageTransition));
    
    internal static readonly DirectProperty<Carousel, bool> PreviousNavButtonVisibleProperty =
        AvaloniaProperty.RegisterDirect<Carousel, bool>(
            nameof(PreviousNavButtonVisible),
            o => o.PreviousNavButtonVisible,
            (o, v) => o.PreviousNavButtonVisible = v);
    
    internal static readonly DirectProperty<Carousel, bool> NextNavButtonVisibleProperty =
        AvaloniaProperty.RegisterDirect<Carousel, bool>(
            nameof(NextNavButtonVisible),
            o => o.NextNavButtonVisible,
            (o, v) => o.NextNavButtonVisible = v);
    
    internal static readonly DirectProperty<Carousel, Thickness> EffectivePaginationMarginProperty =
        AvaloniaProperty.RegisterDirect<Carousel, Thickness>(
            nameof(EffectivePaginationMargin),
            o => o.EffectivePaginationMargin,
            (o, v) => o.EffectivePaginationMargin = v);
    
    internal static readonly DirectProperty<Carousel, Thickness> EffectivePreviousButtonMarginProperty =
        AvaloniaProperty.RegisterDirect<Carousel, Thickness>(
            nameof(EffectivePreviousButtonMargin),
            o => o.EffectivePreviousButtonMargin,
            (o, v) => o.EffectivePreviousButtonMargin = v);
    
    internal static readonly DirectProperty<Carousel, Thickness> EffectiveNextButtonMarginProperty =
        AvaloniaProperty.RegisterDirect<Carousel, Thickness>(
            nameof(EffectiveNextButtonMargin),
            o => o.EffectiveNextButtonMargin,
            (o, v) => o.EffectiveNextButtonMargin = v);
    
    internal static readonly StyledProperty<double> PaginationOffsetProperty =
        AvaloniaProperty.Register<Carousel, double>(nameof(PaginationOffset));
    
    internal static readonly DirectProperty<Carousel, bool> IsEffectiveShowTransitionProgressProperty =
        AvaloniaProperty.RegisterDirect<Carousel, bool>(
            nameof(IsEffectiveShowTransitionProgress),
            o => o.IsEffectiveShowTransitionProgress,
            (o, v) => o.IsEffectiveShowTransitionProgress = v);
    
    internal static readonly DirectProperty<Carousel, IList<object>?> IndicatorItemsProperty =
        AvaloniaProperty.RegisterDirect<Carousel, IList<object>?>(
            nameof(IndicatorItems),
            o => o.IndicatorItems,
            (o, v) => o.IndicatorItems = v);
    
    private static readonly FuncTemplate<Panel?> DefaultPanel =
        new(() => new VirtualizingCarouselPanel());
    
    public IPageTransition? PageTransition
    {
        get => GetValue(PageTransitionProperty);
        set => SetValue(PageTransitionProperty, value);
    }

    private bool _previousNavButtonVisible;

    internal bool PreviousNavButtonVisible
    {
        get => _previousNavButtonVisible;
        set => SetAndRaise(PreviousNavButtonVisibleProperty, ref _previousNavButtonVisible, value);
    }
    
    private bool _nextNavButtonVisible;

    internal bool NextNavButtonVisible
    {
        get => _nextNavButtonVisible;
        set => SetAndRaise(NextNavButtonVisibleProperty, ref _nextNavButtonVisible, value);
    }
    
    private Thickness _effectivePaginationMargin;

    internal Thickness EffectivePaginationMargin
    {
        get => _effectivePaginationMargin;
        set => SetAndRaise(EffectivePaginationMarginProperty, ref _effectivePaginationMargin, value);
    }
    
    private Thickness _effectivePreviousButtonMargin;

    internal Thickness EffectivePreviousButtonMargin
    {
        get => _effectivePreviousButtonMargin;
        set => SetAndRaise(EffectivePreviousButtonMarginProperty, ref _effectivePreviousButtonMargin, value);
    }
    
    private Thickness _effectiveNextButtonMargin;

    internal Thickness EffectiveNextButtonMargin
    {
        get => _effectiveNextButtonMargin;
        set => SetAndRaise(EffectiveNextButtonMarginProperty, ref _effectiveNextButtonMargin, value);
    }
    
    private bool _isEffectiveShowTransitionProgress;

    internal bool IsEffectiveShowTransitionProgress
    {
        get => _isEffectiveShowTransitionProgress;
        set => SetAndRaise(IsEffectiveShowTransitionProgressProperty, ref _isEffectiveShowTransitionProgress, value);
    }
    
    internal double PaginationOffset
    {
        get => GetValue(PaginationOffsetProperty);
        set => SetValue(PaginationOffsetProperty, value);
    }
    
    private IList<object>? _indicatorItems;

    internal IList<object>? IndicatorItems
    {
        get => _indicatorItems;
        set => SetAndRaise(IndicatorItemsProperty, ref _indicatorItems, value);
    }

    #endregion
    
    private Panel? _rootLayout;
    private IScrollable? _scroller;
    private CarouselPagination? _pagination;
    private LayoutTransformControl? _paginationLayoutTransform;
    private DispatcherTimer? _autoPlayTimer;
    private CarouselNavButton? _previousButton;
    private CarouselNavButton? _nextButton;
    private bool _isPointerGestureActive;
    private Point _pointerPressPoint;
    private const double SwipeGestureThreshold = 30;
    private bool _isSwipeCursorActive;
    private bool _isSyncingPaginationSelection;
    
    static Carousel()
    {
        SelectionModeProperty.OverrideDefaultValue<Carousel>(SelectionMode.AlwaysSelected);
        ItemsPanelProperty.OverrideDefaultValue<Carousel>(DefaultPanel);
        AffectsArrange<Carousel>(SelectedIndexProperty);
        AutoScrollToSelectedItemProperty.OverrideDefaultValue<Carousel>(false);
        ItemCountProperty.Changed.AddClassHandler<Carousel>((carousel, args) => carousel.HandleItemCountChanged(args.GetNewValue<int>()));
    }

    public Carousel()
    {
        this.RegisterTokenResourceScope(CarouselToken.ScopeProvider);
    }
    
    private void HandleItemCountChanged(int count)
    {
        var items = new List<object>();
        for (var i = 0; i < count; i++)
        {
            items.Add(new object());
        }
        SetCurrentValue(IndicatorItemsProperty, items);
    }
    
    public void Next()
    {
        if (!IsInfinite)
        {
            if (SelectedIndex < ItemCount - 1)
            {
                ++SelectedIndex;
            }
        }
        else
        {
            SelectedIndex = (SelectedIndex + 1) % ItemCount;
        }
    }
    
    public void Previous()
    {
        if (SelectedIndex > 0)
        {
            --SelectedIndex;
        }
        else if (IsInfinite)
        {
            SelectedIndex = ItemCount - 1;
        }
    }
    
    protected override Size ArrangeOverride(Size finalSize)
    {
        var result = base.ArrangeOverride(finalSize);

        if (_scroller is not null && !MathUtils.AreClose(_scroller.Offset.X, SelectedIndex))
        {
            _scroller.Offset = new(SelectedIndex, 0);
        }

        return result;
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        ReleaseTemplateChildren();
        base.OnApplyTemplate(e);
        _rootLayout = e.NameScope.Find<Panel>("PART_RootLayout");
        _scroller   = e.NameScope.Find<IScrollable>("PART_ScrollViewer");
        ConfigurePaginationMargin();
        ConfigureNavButtonsMargin();
        UpdateEffectiveTransitionProgress();
        UpdatePagination();
        ConfigureNavButtons();
        ConfigureEffectivePageTransition();
    }

    private void HandlePreviousButtonClick(object? sender, RoutedEventArgs args)
    {
        Previous();
    }

    private void HandleNextButtonClick(object? sender, RoutedEventArgs args)
    {
        Next();
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (!IsSwipeEnabled)
        {
            return;
        }

        var point = e.GetCurrentPoint(this);
        var isPrimaryPointer = e.Pointer.Type != PointerType.Mouse || point.Properties.IsLeftButtonPressed;

        if (!isPrimaryPointer)
        {
            return;
        }

        if (ShouldIgnorePointerSource(e.Source))
        {
            return;
        }

        _isPointerGestureActive = true;
        _pointerPressPoint      = point.Position;
        e.Pointer.Capture(this);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (!IsSwipeEnabled || !_isPointerGestureActive)
        {
            return;
        }

        var handled = TryHandleSwipe(e.GetPosition(this));
        _isPointerGestureActive = false;
        e.Pointer.Capture(null);
        if (handled)
        {
            e.Handled = true;
        }
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        base.OnPointerCaptureLost(e);
        _isPointerGestureActive = false;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == SelectedIndexProperty)
        {
            EnsurePageTransitionForSelectionChange();
            if (_scroller is not null)
            {
                var value = change.GetNewValue<int>();
                if (!MathUtils.AreClose(_scroller.Offset.X, value))
                {
                    _scroller.Offset = new(value, 0);
                }
            }
            SyncPaginationSelection();
            ConfigureNavButtons();
        }
        else if (change.Property == IsInfiniteProperty ||
                 change.Property == IsShowNavButtonsProperty ||
                 change.Property == ItemCountProperty)
        {
            ConfigureNavButtons();
        }
        else if (change.Property == PaginationPositionProperty ||
                 change.Property == PaginationOffsetProperty)
        {
            ConfigurePaginationMargin();
            ConfigureNavButtonsMargin();
            UpdatePaginationPlacement();
            SyncNavButtonProperties();
        }
        else if (change.Property == TransitionEffectProperty)
        {
            RebuildExistingPageTransition();
        }
        else if (change.Property == PageTransitionDurationProperty ||
                 change.Property == PageInEasingProperty ||
                 change.Property == PageOutEasingProperty)
        {
            ConfigureEffectivePageTransition();
        }
        else if (change.Property == IsShowPaginationProperty)
        {
            UpdatePagination();
        }
        else if (change.Property == IndicatorItemsProperty ||
                 change.Property == IsEffectiveShowTransitionProgressProperty)
        {
            SyncPaginationProperties();
        }

        if (change.Property == PaginationPositionProperty)
        {
            ConfigureEffectivePageTransition();
            if (IsSwipeEnabled)
            {
                UpdateSwipeCursor();
            }
        }
        else if (change.Property == IsAutoPlayProperty)
        {
            BuildAutoPlayTimer();
        }
        else if (change.Property == AutoPlaySpeedProperty)
        {
            ConfigureAutoPlayTimer();
            SyncPaginationProperties();
        }
        else if (change.Property == IsMotionEnabledProperty)
        {
            if (!IsMotionEnabled)
            {
                SetCurrentValue(PageTransitionProperty, null);
            }
            SyncPaginationProperties();
        }
        else if (change.Property == IsSwipeEnabledProperty)
        {
            UpdateSwipeCursor();
        }
        else if (change.Property == SelectedIndexProperty)
        {
            if (_autoPlayTimer != null && _autoPlayTimer.IsEnabled)
            {
                _autoPlayTimer?.Stop();
                _autoPlayTimer?.Start();
            }
        }

        if (change.Property == IsAutoPlayProperty ||
            change.Property == IsShowTransitionProgressProperty)
        {
            UpdateEffectiveTransitionProgress();
        }
    }

    private bool TryHandleSwipe(Point releasePoint)
    {
        var delta       = releasePoint - _pointerPressPoint;
        var orientation = GetSwipeOrientation();

        if (orientation == Orientation.Horizontal)
        {
            if (Math.Abs(delta.X) >= SwipeGestureThreshold &&
                Math.Abs(delta.X) > Math.Abs(delta.Y))
            {
                if (delta.X < 0)
                {
                    Next();
                }
                else
                {
                    Previous();
                }

                return true;
            }
        }
        else
        {
            if (Math.Abs(delta.Y) >= SwipeGestureThreshold &&
                Math.Abs(delta.Y) > Math.Abs(delta.X))
            {
                if (delta.Y < 0)
                {
                    Next();
                }
                else
                {
                    Previous();
                }

                return true;
            }
        }

        return false;
    }

    private Orientation GetSwipeOrientation()
    {
        if (PaginationPosition == CarouselPaginationPosition.Left ||
            PaginationPosition == CarouselPaginationPosition.Right)
        {
            return Orientation.Vertical;
        }

        return Orientation.Horizontal;
    }

    private bool ShouldIgnorePointerSource(object? source)
    {
        if (source is not Visual visual)
        {
            return false;
        }

        if (_previousButton != null &&
            (_previousButton == visual || _previousButton.IsVisualAncestorOf(visual)))
        {
            return true;
        }

        if (_nextButton != null &&
            (_nextButton == visual || _nextButton.IsVisualAncestorOf(visual)))
        {
            return true;
        }

        if (_pagination != null &&
            (_pagination == visual || _pagination.IsVisualAncestorOf(visual)))
        {
            return true;
        }

        return false;
    }

    private void UpdateSwipeCursor()
    {
        if (IsSwipeEnabled)
        {
            SetCurrentValue(CursorProperty, new Cursor(StandardCursorType.DragMove));
            _isSwipeCursorActive = true;
        }
        else if (_isSwipeCursorActive)
        {
            SetCurrentValue(CursorProperty, Cursor.Default);
            _isSwipeCursorActive = false;
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (IsAutoPlay)
        {
            _autoPlayTimer?.Start();
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _autoPlayTimer?.Stop();
    }

    private void ReleaseTemplateChildren()
    {
        ReleaseNavButtons();
        ReleasePagination();
        _rootLayout = null;
        _scroller   = null;
    }

    private void UpdateNavButtons()
    {
        if (!IsShowNavButtons)
        {
            ReleaseNavButtons();
            return;
        }

        if (_rootLayout is null)
        {
            return;
        }

        EnsureNavButtons();
        EnsureOverlayOrder();
    }

    private void EnsureNavButtons()
    {
        if (_previousButton is null)
        {
            _previousButton = new CarouselNavButton
            {
                Name = "PART_PreviousButton"
            };
            _previousButton.SetValue(IconButton.IconProperty, new LeftOutlined(), BindingPriority.Template);
            _previousButton.SetTemplatedParent(this);
            _previousButton.Click += HandlePreviousButtonClick;
        }

        if (_nextButton is null)
        {
            _nextButton = new CarouselNavButton
            {
                Name = "PART_NextButton"
            };
            _nextButton.SetValue(IconButton.IconProperty, new RightOutlined(), BindingPriority.Template);
            _nextButton.SetTemplatedParent(this);
            _nextButton.Click += HandleNextButtonClick;
        }

        if (!_rootLayout!.Children.Contains(_previousButton))
        {
            _rootLayout.Children.Add(_previousButton);
        }
        if (!_rootLayout.Children.Contains(_nextButton))
        {
            _rootLayout.Children.Add(_nextButton);
        }
    }

    private void ReleaseNavButtons()
    {
        ReleaseNavButton(ref _previousButton, HandlePreviousButtonClick);
        ReleaseNavButton(ref _nextButton, HandleNextButtonClick);
    }

    private void ReleaseNavButton(ref CarouselNavButton? button, EventHandler<RoutedEventArgs> clickHandler)
    {
        if (button is null)
        {
            return;
        }

        button.Click -= clickHandler;
        RemoveFromVisualParent(button);
        button.ClearValue(IconButton.IconProperty);
        button.SetTemplatedParent(null);
        button = null;
    }

    private void SyncNavButtonProperties()
    {
        if (_previousButton is null || _nextButton is null)
        {
            return;
        }

        _previousButton.SetValue(MarginProperty, EffectivePreviousButtonMargin, BindingPriority.Template);
        _previousButton.SetValue(IsVisibleProperty, PreviousNavButtonVisible, BindingPriority.Template);
        _nextButton.SetValue(MarginProperty, EffectiveNextButtonMargin, BindingPriority.Template);
        _nextButton.SetValue(IsVisibleProperty, NextNavButtonVisible, BindingPriority.Template);

        if (IsVerticalPaginationPosition())
        {
            _previousButton.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
            _previousButton.SetValue(VerticalAlignmentProperty, VerticalAlignment.Top, BindingPriority.Template);
            _previousButton.SetValue(RenderTransformProperty, new RotateTransform(90), BindingPriority.Template);
            _nextButton.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
            _nextButton.SetValue(VerticalAlignmentProperty, VerticalAlignment.Bottom, BindingPriority.Template);
            _nextButton.SetValue(RenderTransformProperty, new RotateTransform(90), BindingPriority.Template);
        }
        else
        {
            _previousButton.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Left, BindingPriority.Template);
            _previousButton.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
            _previousButton.SetValue(RenderTransformProperty, null, BindingPriority.Template);
            _nextButton.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Right, BindingPriority.Template);
            _nextButton.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
            _nextButton.SetValue(RenderTransformProperty, null, BindingPriority.Template);
        }
    }

    private void UpdatePagination()
    {
        if (!IsShowPagination)
        {
            ReleasePagination();
            return;
        }

        if (_rootLayout is null)
        {
            return;
        }

        EnsurePagination();
        UpdatePaginationPlacement();
        SyncPaginationProperties();
        EnsureOverlayOrder();
    }

    private void EnsurePagination()
    {
        if (_pagination is not null)
        {
            return;
        }

        _pagination = new CarouselPagination
        {
            Name = "PART_Pagination"
        };
        _pagination.SetTemplatedParent(this);
        _pagination.SelectionChanged += HandlePaginationSelectionChanged;
    }

    private void UpdatePaginationPlacement()
    {
        if (_rootLayout is null || _pagination is null)
        {
            return;
        }

        if (IsVerticalPaginationPosition())
        {
            EnsurePaginationLayoutTransform();
            if (_pagination.GetVisualParent() is Panel directParent)
            {
                directParent.Children.Remove(_pagination);
            }
            if (!ReferenceEquals(_paginationLayoutTransform!.Child, _pagination))
            {
                _paginationLayoutTransform.Child = _pagination;
            }
            if (!_rootLayout.Children.Contains(_paginationLayoutTransform))
            {
                _rootLayout.Children.Add(_paginationLayoutTransform);
            }
        }
        else
        {
            if (_paginationLayoutTransform is not null)
            {
                if (ReferenceEquals(_paginationLayoutTransform.Child, _pagination))
                {
                    _paginationLayoutTransform.Child = null;
                }
                RemoveFromVisualParent(_paginationLayoutTransform);
                _paginationLayoutTransform.SetTemplatedParent(null);
                _paginationLayoutTransform = null;
            }
            if (!_rootLayout.Children.Contains(_pagination))
            {
                _rootLayout.Children.Add(_pagination);
            }
        }

        SyncPaginationHostPlacement(GetPaginationHost());
        EnsureOverlayOrder();
    }

    private Control GetPaginationHost()
    {
        return _paginationLayoutTransform ?? (Control)_pagination!;
    }

    private void EnsurePaginationLayoutTransform()
    {
        if (_paginationLayoutTransform is not null)
        {
            return;
        }

        _paginationLayoutTransform = new LayoutTransformControl
        {
            Name = "PaginationLayoutTransform"
        };
        _paginationLayoutTransform.SetTemplatedParent(this);
    }

    private void ReleasePagination()
    {
        if (_pagination is not null)
        {
            _pagination.SelectionChanged -= HandlePaginationSelectionChanged;
            if (_paginationLayoutTransform is not null &&
                ReferenceEquals(_paginationLayoutTransform.Child, _pagination))
            {
                _paginationLayoutTransform.Child = null;
            }
            RemoveFromVisualParent(_pagination);
            _pagination.ClearValue(ItemsSourceProperty);
            _pagination.SetTemplatedParent(null);
            _pagination = null;
        }

        if (_paginationLayoutTransform is not null)
        {
            _paginationLayoutTransform.Child = null;
            RemoveFromVisualParent(_paginationLayoutTransform);
            _paginationLayoutTransform.SetTemplatedParent(null);
            _paginationLayoutTransform = null;
        }
    }

    private void SyncPaginationProperties()
    {
        if (_pagination is null)
        {
            return;
        }

        _pagination.SetValue(CarouselPagination.IsMotionEnabledProperty, IsMotionEnabled, BindingPriority.Template);
        _pagination.SetValue(CarouselPagination.IsShowTransitionProgressProperty, IsEffectiveShowTransitionProgress, BindingPriority.Template);
        _pagination.SetValue(CarouselPagination.AutoPlaySpeedProperty, AutoPlaySpeed, BindingPriority.Template);
        _pagination.SetValue(ItemsSourceProperty, IndicatorItems, BindingPriority.Template);
        SyncPaginationSelection();
    }

    private void SyncPaginationSelection()
    {
        if (_pagination is null || _isSyncingPaginationSelection)
        {
            return;
        }

        if (_pagination.SelectedIndex == SelectedIndex)
        {
            return;
        }

        _isSyncingPaginationSelection = true;
        _pagination.SetCurrentValue(SelectedIndexProperty, SelectedIndex);
        _isSyncingPaginationSelection = false;
    }

    private void HandlePaginationSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        if (_pagination is null || _isSyncingPaginationSelection)
        {
            return;
        }

        if (_pagination.SelectedIndex >= 0 && _pagination.SelectedIndex != SelectedIndex)
        {
            SetCurrentValue(SelectedIndexProperty, _pagination.SelectedIndex);
        }
    }

    private void SyncPaginationHostPlacement(Control host)
    {
        host.SetValue(MarginProperty, EffectivePaginationMargin, BindingPriority.Template);
        host.SetValue(IsVisibleProperty, IsShowPagination, BindingPriority.Template);
        if (host is LayoutTransformControl layoutTransform)
        {
            layoutTransform.SetValue(LayoutTransformControl.LayoutTransformProperty, new RotateTransform(90), BindingPriority.Template);
        }

        switch (PaginationPosition)
        {
            case CarouselPaginationPosition.Top:
                host.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
                host.SetValue(VerticalAlignmentProperty, VerticalAlignment.Top, BindingPriority.Template);
                break;
            case CarouselPaginationPosition.Left:
                host.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Left, BindingPriority.Template);
                host.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
                break;
            case CarouselPaginationPosition.Right:
                host.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Right, BindingPriority.Template);
                host.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center, BindingPriority.Template);
                break;
            default:
                host.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center, BindingPriority.Template);
                host.SetValue(VerticalAlignmentProperty, VerticalAlignment.Bottom, BindingPriority.Template);
                break;
        }
    }

    private void EnsureOverlayOrder()
    {
        if (_rootLayout is null)
        {
            return;
        }

        MoveChildToEnd(_previousButton);
        MoveChildToEnd(_nextButton);
        MoveChildToEnd(_paginationLayoutTransform ?? (Control?)_pagination);
    }

    private void MoveChildToEnd(Control? child)
    {
        if (child is null || _rootLayout is null)
        {
            return;
        }

        var index = _rootLayout.Children.IndexOf(child);
        if (index < 0 || index == _rootLayout.Children.Count - 1)
        {
            return;
        }

        _rootLayout.Children.RemoveAt(index);
        _rootLayout.Children.Add(child);
    }

    private bool IsVerticalPaginationPosition()
    {
        return PaginationPosition == CarouselPaginationPosition.Left ||
               PaginationPosition == CarouselPaginationPosition.Right;
    }

    private static void RemoveFromVisualParent(Control control)
    {
        var parent = control.GetVisualParent();
        if (parent is Panel panel)
        {
            panel.Children.Remove(control);
        }
        else if (parent is Decorator decorator && ReferenceEquals(decorator.Child, control))
        {
            decorator.Child = null;
        }
    }

    private void ConfigureNavButtons()
    {
        UpdateNavButtons();
        if (IsShowNavButtons)
        {
            if (IsInfinite)
            {
                SetCurrentValue(PreviousNavButtonVisibleProperty, true);
                SetCurrentValue(NextNavButtonVisibleProperty, true);
            }
            else
            {
                SetCurrentValue(PreviousNavButtonVisibleProperty, SelectedIndex != 0);
                SetCurrentValue(NextNavButtonVisibleProperty, SelectedIndex != ItemCount - 1);
            }
        }
        else
        {
            SetCurrentValue(PreviousNavButtonVisibleProperty, false);
            SetCurrentValue(NextNavButtonVisibleProperty, false);
        }
        SyncNavButtonProperties();
    }

    private void ConfigurePaginationMargin()
    {
        if (PaginationPosition == CarouselPaginationPosition.Bottom)
        {
            SetCurrentValue(EffectivePaginationMarginProperty, new Thickness(0, 0, 0, PaginationOffset));
        }
        else if (PaginationPosition == CarouselPaginationPosition.Top)
        {
            SetCurrentValue(EffectivePaginationMarginProperty, new Thickness(0, PaginationOffset, 0, 0));
        }
        else if (PaginationPosition == CarouselPaginationPosition.Left)
        {
            SetCurrentValue(EffectivePaginationMarginProperty, new Thickness(PaginationOffset, 0, 0, 0));
        }
        else if (PaginationPosition == CarouselPaginationPosition.Right)
        {
            SetCurrentValue(EffectivePaginationMarginProperty, new Thickness(0, 0, PaginationOffset, 0));
        }
    }

    private void ConfigureNavButtonsMargin()
    {
        if (PaginationPosition == CarouselPaginationPosition.Bottom ||
            PaginationPosition == CarouselPaginationPosition.Top)
        {
            SetCurrentValue(EffectivePreviousButtonMarginProperty, new Thickness(PaginationOffset, 0, 0, 0));
            SetCurrentValue(EffectiveNextButtonMarginProperty, new Thickness(0, 0, PaginationOffset, 0));
        }
        else if (PaginationPosition == CarouselPaginationPosition.Left ||
                 PaginationPosition == CarouselPaginationPosition.Right)
        {
            SetCurrentValue(EffectivePreviousButtonMarginProperty, new Thickness(0, PaginationOffset, 0, 0));
            SetCurrentValue(EffectiveNextButtonMarginProperty, new Thickness(0, 0, 0, PaginationOffset));
        }
    }

    private void EnsurePageTransitionForSelectionChange()
    {
        if (_scroller is null || !IsMotionEnabled)
        {
            if (!IsMotionEnabled)
            {
                SetCurrentValue(PageTransitionProperty, null);
            }
            return;
        }

        BuildEffectivePageTransition(PageTransition is null);
    }

    private void RebuildExistingPageTransition()
    {
        if (PageTransition is null)
        {
            return;
        }

        BuildEffectivePageTransition(true);
    }

    private void BuildEffectivePageTransition(bool force)
    {
        if (!IsMotionEnabled)
        {
            SetCurrentValue(PageTransitionProperty, null);
            return;
        }

        if (PageTransition is null || force)
        {
            if (TransitionEffect == CarouselTransitionEffect.Fade)
            {
                SetCurrentValue(PageTransitionProperty, new CrossFade());
            }
            else
            {
                SetCurrentValue(PageTransitionProperty, new PageSlide());
            }
        }
        ConfigureEffectivePageTransition();
    }

    private void ConfigureEffectivePageTransition()
    {
        if (PageTransition is CrossFade crossFade)
        {
            crossFade.FadeInEasing  = PageInEasing;
            crossFade.FadeOutEasing = PageOutEasing;
            crossFade.Duration      = PageTransitionDuration;
        }
        else if (PageTransition is PageSlide pageSlide)
        {
            pageSlide.SlideInEasing  = PageInEasing;
            pageSlide.SlideOutEasing = PageOutEasing;
            pageSlide.Duration      = PageTransitionDuration;
            if (PaginationPosition == CarouselPaginationPosition.Bottom ||
                PaginationPosition == CarouselPaginationPosition.Top)
            {
                pageSlide.Orientation = PageSlide.SlideAxis.Horizontal;
            }
            else if (PaginationPosition == CarouselPaginationPosition.Left ||
                     PaginationPosition == CarouselPaginationPosition.Right)
            {
                pageSlide.Orientation = PageSlide.SlideAxis.Vertical;
            }
        }
    }

    private void BuildAutoPlayTimer()
    {
        if (IsAutoPlay)
        {
            if (_autoPlayTimer != null)
            {
                _autoPlayTimer.Stop();
                _autoPlayTimer.Tick -= HandleAutoPlayTick;
            }
            _autoPlayTimer      =  new DispatcherTimer();
            _autoPlayTimer.Tick += HandleAutoPlayTick;
            ConfigureAutoPlayTimer();
            if (this.IsAttachedToVisualTree())
            {
                _autoPlayTimer.Start();
            }
        }
        else
        {
            if (_autoPlayTimer != null)
            {
                _autoPlayTimer.Stop();
                _autoPlayTimer.Tick -= HandleAutoPlayTick;
            }
            _autoPlayTimer = null;
        }
    }

    private void HandleAutoPlayTick(object? sender, EventArgs e)
    {
        Next();
    }

    private void ConfigureAutoPlayTimer()
    {
        if (_autoPlayTimer != null)
        {
            _autoPlayTimer.Interval = AutoPlaySpeed;
        }
    }

    private void UpdateEffectiveTransitionProgress()
    {
        SetCurrentValue(IsEffectiveShowTransitionProgressProperty, IsAutoPlay && IsShowTransitionProgress);
    }
}
