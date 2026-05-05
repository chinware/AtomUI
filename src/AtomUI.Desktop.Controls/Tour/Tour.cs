using System.Collections.Specialized;
using System.Diagnostics;
using System.Reactive.Disposables;
using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using AtomUI.Data;
using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Desktop.Controls.Themes;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

using ItemCollection = AtomUI.Collections.ItemCollection;

public enum TourStyleType
{
    Default,
    Primary,
}

public class Tour : TemplatedControl, IMotionAwareControl
{
    #region 公共属性定义
    public static readonly StyledProperty<IEnumerable<ITourStepOption>?> StepsSourceProperty =
        AvaloniaProperty.Register<Tour, IEnumerable<ITourStepOption>?>(nameof(StepsSource));
    
    public static readonly StyledProperty<IDataTemplate?> ItemTemplateProperty =
        AvaloniaProperty.Register<Tour, IDataTemplate?>(nameof(ItemTemplate));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Tour>();
    
    public static readonly StyledProperty<bool> IsShowArrowProperty =
        AvaloniaProperty.Register<Tour, bool>(nameof(IsShowArrow), true);
    
    public static readonly StyledProperty<bool> IsPointAtCenterProperty =
        AvaloniaProperty.Register<Tour, bool>(nameof(IsPointAtCenter), false);
    
    public static readonly StyledProperty<IIconTemplate?> CloseIconProperty =
        AvaloniaProperty.Register<Tour, IIconTemplate?>(nameof(CloseIcon));
    
    public static readonly StyledProperty<bool> IsDisabledInteractionProperty =
        AvaloniaProperty.Register<Tour, bool>(nameof(IsDisabledInteraction));
    
    public static readonly StyledProperty<double> GapOffsetXProperty =
        AvaloniaProperty.Register<Tour, double>(nameof(GapOffsetX), 6);
    
    public static readonly StyledProperty<double> GapOffsetYProperty =
        AvaloniaProperty.Register<Tour, double>(nameof(GapOffsetY), 6);
    
    public static readonly StyledProperty<double> GapRadiusProperty =
        AvaloniaProperty.Register<Tour, double>(nameof(GapRadius), 2);
    
    public static readonly StyledProperty<TourPlacementMode> PlacementProperty =
        AvaloniaProperty.Register<Tour, TourPlacementMode>(nameof(Placement));
    
    public static readonly StyledProperty<bool> IsShowMaskProperty =
        AvaloniaProperty.Register<Tour, bool>(nameof(IsShowMask), true);
    
    public static readonly StyledProperty<IBrush?> MaskColorProperty =
        AvaloniaProperty.Register<Tour, IBrush?>(nameof(MaskColor));
    
    public static readonly StyledProperty<TourStyleType> StyleTypeProperty =
        AvaloniaProperty.Register<Tour, TourStyleType>(nameof(StyleType));
    
    public static readonly StyledProperty<bool> IsScrollIntoViewProperty =
        AvaloniaProperty.Register<Tour, bool>(nameof(IsScrollIntoView), true);
    
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<Tour, bool>(nameof(IsOpen));
    
    public static readonly DirectProperty<Tour, int> CurrentIndexProperty =
        AvaloniaProperty.RegisterDirect<Tour, int>(
            nameof(CurrentIndex),
            o => o.CurrentIndex,
            (o, v) => o.CurrentIndex = v,
            unsetValue: -1,
            defaultBindingMode: BindingMode.TwoWay);
    
    public static readonly DirectProperty<Tour, int> StepCountProperty =
        AvaloniaProperty.RegisterDirect<Tour, int>(nameof(StepCount), o => o.StepCount);
    
    public static readonly StyledProperty<TourIndicator?> IndicatorProperty =
        AvaloniaProperty.Register<Tour, TourIndicator?>(nameof(Indicator));
    
    public IEnumerable<ITourStepOption>? StepsSource
    {
        get => GetValue(StepsSourceProperty);
        set => SetValue(StepsSourceProperty, value);
    }
    
    [InheritDataTypeFromItems(nameof(StepsSource))]
    public IDataTemplate? ItemTemplate
    {
        get => GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public bool IsShowArrow
    {
        get => GetValue(IsShowArrowProperty);
        set => SetValue(IsShowArrowProperty, value);
    }

    public bool IsPointAtCenter
    {
        get => GetValue(IsPointAtCenterProperty);
        set => SetValue(IsPointAtCenterProperty, value);
    }
    
    public IIconTemplate? CloseIcon
    {
        get => GetValue(CloseIconProperty);
        set => SetValue(CloseIconProperty, value);
    }
    
    public bool IsDisabledInteraction
    {
        get => GetValue(IsDisabledInteractionProperty);
        set => SetValue(IsDisabledInteractionProperty, value);
    }
    
    public double GapOffsetX
    {
        get => GetValue(GapOffsetXProperty);
        set => SetValue(GapOffsetXProperty, value);
    }
    
    public double GapOffsetY
    {
        get => GetValue(GapOffsetYProperty);
        set => SetValue(GapOffsetYProperty, value);
    }
    
    public double GapRadius
    {
        get => GetValue(GapRadiusProperty);
        set => SetValue(GapRadiusProperty, value);
    }
    
    public TourPlacementMode Placement
    {
        get => GetValue(PlacementProperty);
        set => SetValue(PlacementProperty, value);
    }
    
    public bool IsShowMask
    {
        get => GetValue(IsShowMaskProperty);
        set => SetValue(IsShowMaskProperty, value);
    }
    
    public IBrush? MaskColor
    {
        get => GetValue(MaskColorProperty);
        set => SetValue(MaskColorProperty, value);
    }
    
    public TourStyleType StyleType
    {
        get => GetValue(StyleTypeProperty);
        set => SetValue(StyleTypeProperty, value);
    }
    
    public bool IsScrollIntoView
    {
        get => GetValue(IsScrollIntoViewProperty);
        set => SetValue(IsScrollIntoViewProperty, value);
    }
    
    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }
    
    private int _currentIndex;
    public int CurrentIndex
    {
        get => _currentIndex;
        private set => SetAndRaise(CurrentIndexProperty, ref _currentIndex, value);
    }
    
    private int _stepCount;
    public int StepCount
    {
        get => _stepCount;
        private set => SetAndRaise(StepCountProperty, ref _stepCount, value);
    }
    
    public TourIndicator? Indicator
    {
        get => GetValue(IndicatorProperty);
        set => SetValue(IndicatorProperty, value);
    }
    
    [Content]
    public ItemCollection Steps { get; } = new();
    public ItemCollection CustomActions { get; } = new();
    
    #endregion

    #region 内部属性定义
    internal static readonly DirectProperty<Tour, Rect> TargetClipBoundsProperty =
        AvaloniaProperty.RegisterDirect<Tour, Rect>(nameof(TargetClipBounds),
            o => o.TargetClipBounds,
            (o, v) => o.TargetClipBounds = v);
    
    internal static readonly DirectProperty<Tour, bool> IsPopupHorizontalFlippedProperty =
        AvaloniaProperty.RegisterDirect<Tour, bool>(nameof(IsPopupHorizontalFlipped),
            o => o.IsPopupHorizontalFlipped,
            (o, v) => o.IsPopupHorizontalFlipped = v);

    internal static readonly DirectProperty<Tour, bool> IsPopupVerticalFlippedProperty =
        AvaloniaProperty.RegisterDirect<Tour, bool>(nameof(IsPopupVerticalFlipped),
            o => o.IsPopupVerticalFlipped,
            (o, v) => o.IsPopupVerticalFlipped = v);
    
    internal static readonly StyledProperty<ArrowPosition> ArrowPositionProperty =
        ArrowDecoratedBox.ArrowPositionProperty.AddOwner<Tour>();
    
    internal static readonly DirectProperty<Tour, TourStyleType> CurrentStyleTypeProperty =
        AvaloniaProperty.RegisterDirect<Tour, TourStyleType>(nameof(CurrentStyleType),
            o => o.CurrentStyleType,
            (o, v) => o.CurrentStyleType = v);
    
    internal static readonly DirectProperty<Tour, bool> CurrentShowArrowProperty =
        AvaloniaProperty.RegisterDirect<Tour, bool>(nameof(CurrentShowArrow),
            o => o.CurrentShowArrow,
            (o, v) => o.CurrentShowArrow = v);
    
    internal static readonly DirectProperty<Tour, IBrush?> CurrentMaskColorProperty =
        AvaloniaProperty.RegisterDirect<Tour, IBrush?>(nameof(CurrentMaskColor),
            o => o.CurrentMaskColor,
            (o, v) => o.CurrentMaskColor = v);
    
    private Rect _targetClipBounds;

    internal Rect TargetClipBounds
    {
        get => _targetClipBounds;
        set => SetAndRaise(TargetClipBoundsProperty, ref _targetClipBounds, value);
    }
    
    private bool _isPopupHorizontalFlipped;

    internal bool IsPopupHorizontalFlipped
    {
        get => _isPopupHorizontalFlipped;
        private set => SetAndRaise(IsPopupHorizontalFlippedProperty, ref _isPopupHorizontalFlipped, value);
    }

    private bool _isPopupVerticalFlipped;

    internal bool IsPopupVerticalFlipped
    {
        get => _isPopupVerticalFlipped;
        private set => SetAndRaise(IsPopupVerticalFlippedProperty, ref _isPopupVerticalFlipped, value);
    }
    
    internal ArrowPosition ArrowPosition
    {
        get => GetValue(ArrowPositionProperty);
        set => SetValue(ArrowPositionProperty, value);
    }
    
    private TourStyleType _currentStyleType;

    internal TourStyleType CurrentStyleType
    {
        get => _currentStyleType;
        private set => SetAndRaise(CurrentStyleTypeProperty, ref _currentStyleType, value);
    }

    private bool _currentShowArrow;

    internal bool CurrentShowArrow
    {
        get => _currentShowArrow;
        private set => SetAndRaise(CurrentShowArrowProperty, ref _currentShowArrow, value);
    }
    
    private IBrush? _currentMaskColor;

    internal IBrush? CurrentMaskColor
    {
        get => _currentMaskColor;
        private set => SetAndRaise(CurrentMaskColorProperty, ref _currentMaskColor, value);
    }

    #endregion
    
    private bool _ignorePropertyChanged;
    private bool _isReallyOpened;
    private Popup? _popup;
    private TourLayer? _layer;
    private TourStepsView? _stepsView;
    private CompositeDisposable? _indicatorDisposables;

    static Tour()
    {
        StepsSourceProperty.Changed.AddClassHandler<Tour>((tour, e) => tour.HandleStepsSourcePropertyChanged(e));
    }

    public Tour()
    {
        this.RegisterTokenResourceScope(TourToken.ScopeProvider);
        Steps.CollectionChanged         += HandleItemsViewCollectionChanged;
        CustomActions.CollectionChanged += HandleCustomActionsChanged;
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (Indicator == null)
        {
            SetCurrentValue(IndicatorProperty, new DefaultTourIndicator());
        }
        ConfigureDefaultValues();
    }

    private void HandleItemsViewCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        StepCount = Steps.Count;
        if (IsInitialized)
        {
            ConfigureDefaultValues();
        }
    }

    private void HandleCustomActionsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_stepsView != null)
        {
            var actions = CustomActions.Cast<Control>().ToList();
            foreach (var action in actions)
            {
                if (action is ITourAction tourAction)
                {
                    tourAction.NotifyAttached(this);
                }
            }
            _stepsView.CustomActions = actions;
        }
    }

    private void ConfigureDefaultValues()
    {
        foreach (var item in Steps)
        {
            if (item is ITourStepOption stepOption)
            {
                stepOption.IsShowArrow      ??= IsShowArrow;
                stepOption.IsPointAtCenter  ??= IsPointAtCenter;
                stepOption.IsShowMask       ??= IsShowMask;
                stepOption.StyleType        ??= StyleType;
                stepOption.Placement        ??= Placement;
                stepOption.IsScrollIntoView ??= IsScrollIntoView;
                stepOption.MaskColor        ??= MaskColor;
            }
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == IsOpenProperty)
            {
                if (!_ignorePropertyChanged)
                {
                    if (change.GetNewValue<bool>())
                    {
                        ShowTour();
                    }
                    else
                    {
                        HideTour();
                    }
                }
            }
            else if (change.Property == GapRadiusProperty ||
                     change.Property == GapOffsetXProperty ||
                     change.Property == GapOffsetYProperty)
            {
                CalculateTargetClipBounds();
            }
        }
        
        if (change.Property == GapOffsetXProperty ||
            change.Property == GapOffsetYProperty)
        {
            ConfigurePopupOffset();
        }
        if (change.Property == IsPopupHorizontalFlippedProperty ||
            change.Property == IsPopupVerticalFlippedProperty)
        {
            ConfigureArrowPosition();
        }
        else if (change.Property == CurrentIndexProperty)
        {
            if (!_ignorePropertyChanged)
            {
                HandleCurrentStepChanged();
            }
        }

        if (change.Property == IndicatorProperty)
        {
            if (change.OldValue is TourIndicator)
            {
                _indicatorDisposables?.Dispose();
                _indicatorDisposables = null;
            }

            if (change.NewValue is TourIndicator newIndicator)
            {
                _indicatorDisposables = new CompositeDisposable();
                _indicatorDisposables.Add(BindUtils.RelayBind(this, StepCountProperty, newIndicator, TourIndicator.StepCountProperty));
                _indicatorDisposables.Add(BindUtils.RelayBind(this, CurrentIndexProperty, newIndicator, TourIndicator.ActiveIndexProperty));
                _indicatorDisposables.Add(BindUtils.RelayBind(this, CurrentStyleTypeProperty, newIndicator, TourIndicator.StyleTypeProperty));
                _indicatorDisposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, newIndicator, TourIndicator.IsMotionEnabledProperty));
            }
        }
    }
    
    private void ConfigureArrowPosition()
    {
        if (_popup == null)
        {
            return;
        }
        var arrowPosition = PopupUtils.CalculateArrowPosition(_popup.Placement, null, null);
        if (arrowPosition.HasValue)
        {
            // Handle vertical flipping (Top <-> Bottom)
            if (IsPopupVerticalFlipped)
            {
                if (arrowPosition == ArrowPosition.Top)
                {
                    arrowPosition = ArrowPosition.Bottom;
                }
                else if (arrowPosition == ArrowPosition.Bottom)
                {
                    arrowPosition = ArrowPosition.Top;
                }
                else if (arrowPosition == ArrowPosition.TopEdgeAlignedLeft)
                {
                    arrowPosition = ArrowPosition.BottomEdgeAlignedLeft;
                }
                else if (arrowPosition == ArrowPosition.TopEdgeAlignedRight)
                {
                    arrowPosition = ArrowPosition.BottomEdgeAlignedRight;
                }
                else if (arrowPosition == ArrowPosition.BottomEdgeAlignedLeft)
                {
                    arrowPosition = ArrowPosition.TopEdgeAlignedLeft;
                }
                else if (arrowPosition == ArrowPosition.BottomEdgeAlignedRight)
                {
                    arrowPosition = ArrowPosition.TopEdgeAlignedRight;
                }
            }

            // Handle horizontal flipping (Left <-> Right)
            if (IsPopupHorizontalFlipped)
            {
                if (arrowPosition == ArrowPosition.Left)
                {
                    arrowPosition = ArrowPosition.Right;
                }
                else if (arrowPosition == ArrowPosition.Right)
                {
                    arrowPosition = ArrowPosition.Left;
                }
                else if (arrowPosition == ArrowPosition.LeftEdgeAlignedTop)
                {
                    arrowPosition = ArrowPosition.RightEdgeAlignedTop;
                }
                else if (arrowPosition == ArrowPosition.LeftEdgeAlignedBottom)
                {
                    arrowPosition = ArrowPosition.RightEdgeAlignedBottom;
                }
                else if (arrowPosition == ArrowPosition.RightEdgeAlignedTop)
                {
                    arrowPosition = ArrowPosition.LeftEdgeAlignedTop;
                }
                else if (arrowPosition == ArrowPosition.RightEdgeAlignedBottom)
                {
                    arrowPosition = ArrowPosition.LeftEdgeAlignedBottom;
                }
            }

            SetCurrentValue(ArrowPositionProperty, arrowPosition);
        }
    }

    private void HandleStepsSourcePropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        ResetState();
        if (!Steps.IsReadOnly)
        {
            Steps.Clear();
        }
        Steps.SetItemsSource(change.GetNewValue<IEnumerable<ITourStepOption>?>());
    }
    
    public void ResetState()
    {
        SetCurrentValue(IsOpenProperty, false);
        SetCurrentValue(CurrentIndexProperty, Steps.Count > 0 ? 0 : -1);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _popup = e.NameScope.Find<Popup>("PART_Popup");
        if (_popup != null)
        {
            this[!IsPopupHorizontalFlippedProperty] = _popup[!Popup.IsHorizontalFlippedProperty];
            this[!IsPopupVerticalFlippedProperty] = _popup[!Popup.IsVerticalFlippedProperty];
            _popup.CustomPopupPlacementCallback = parameters =>
            {
                var topLevel = TopLevel.GetTopLevel(this);
                if (topLevel != null)
                {
                    parameters.AnchorRectangle = new Rect(topLevel.DesiredSize);
                }
            };
        }
        if (_stepsView != null)
        {
            _stepsView.CloseRequest -= HandleCloseRequest;
            _stepsView.NavRequest   -= HandleNavRequest;
        }
        _stepsView = e.NameScope.Find<TourStepsView>("StepsView");
        if (_stepsView != null)
        {
            _stepsView.CloseRequest  += HandleCloseRequest;
            _stepsView.NavRequest    += HandleNavRequest;
            var actions =  CustomActions.Cast<Control>().ToList();

            foreach (var action in actions)
            {
                if (action is ITourAction tourAction)
                {
                    tourAction.NotifyAttached(this);
                }
            }

            _stepsView.CustomActions = actions;
        }
    }

    private void HandleCloseRequest(object? sender, EventArgs args)
    {
        HideTour();
    }

    private void HandleNavRequest(object? sender, TourStepNavRequestEventArgs args)
    {
        SetCurrentValue(CurrentIndexProperty, args.Index);
    }

    public void ShowTour()
    {
        if (_isReallyOpened || _popup == null || Steps.Count == 0)
        {
            return;
        }

        PrepareTourLayer();
        _popup.IsOpen = true;
        HandleCurrentStepChanged();
        using (BeginIgnoringPropertyChanged())
        {
            SetCurrentValue(IsOpenProperty, true);
            _isReallyOpened =  true;
        }
    }

    private void PrepareTourLayer()
    {
        _layer = TourLayer.GetTourLayer(this);
        if (_layer != null)
        {
            _layer[!TourLayer.BackgroundProperty]               = this[!CurrentMaskColorProperty];
            _layer[!TourLayer.TargetRegionCornerRadiusProperty] = this[!GapRadiusProperty];
            _layer[!TourLayer.TargetRegionProperty]             = this[!TargetClipBoundsProperty];
        }
    }
    
    public void HideTour()
    {
        if (!_isReallyOpened || _popup == null)
        {
            return;
        }
        if (_layer != null)
        {
            _layer.IsVisible = false;
        }
        _popup.IsOpen = false;
        using (BeginIgnoringPropertyChanged())
        {
            SetCurrentValue(CurrentIndexProperty, 0);
            SetCurrentValue(IsOpenProperty, false);
            _isReallyOpened =  false;
        }
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (IsOpen)
        {
            ShowTour();
        }
        else
        {
            HideTour();
        }

        if (Steps.Count > 0)
        {
            SetCurrentValue(CurrentIndexProperty, 0);
        }
    }

    private void HandleCurrentStepChanged()
    {
        if (CurrentIndex == -1 || _popup == null || !IsOpen)
        {
            return;
        }

        if (CurrentIndex < 0 || CurrentIndex >= Steps.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(CurrentIndex));
        }
        
        var target = GetCurrentTarget();
        _popup.PlacementTarget = target;

        var      step   = Steps[CurrentIndex];
        if (step is ITourStepOption stepOption)
        {
            CurrentStyleType = stepOption.StyleType ?? StyleType;
            CurrentShowArrow = stepOption.IsShowArrow ?? IsShowArrow;
            if (_layer != null)
            {
                CurrentMaskColor = stepOption.MaskColor ?? MaskColor;
                _layer.IsVisible = stepOption.IsShowMask ?? IsShowArrow;
            }

            if (target == null)
            {
                _popup.Placement = GetPopupPlacement(TourPlacementMode.Center);
                CurrentShowArrow = false;;
            }
            else
            {
                _popup.Placement = GetPopupPlacement(stepOption.Placement ?? Placement);
            }
         
            ConfigureArrowPosition();
        }
        
        ConfigurePopupOffset();
        CalculateTargetClipBounds();
    }

    private Control? GetCurrentTarget()
    {
        var      step   = Steps[CurrentIndex];
        Control? target = null;
        if (step is ITourStepOption stepOption)
        {
            target = stepOption.Target;
        }
        return target;
    }

    private void CalculateTargetClipBounds()
    {
        var      step   = Steps[CurrentIndex];
        Control? target = null;
        if (step is ITourStepOption stepOption)
        {
            target = stepOption.Target;
        }
        if (target == null)
        {
            TargetClipBounds = default;
            return;
        }
        var topLevel = TopLevel.GetTopLevel(target);
        if (topLevel != null)
        {
            var offset       = target.TranslatePoint(new Point(0, 0), topLevel) ?? default;
            var targetBounds = new Rect(offset, target.Bounds.Size);
            TargetClipBounds = targetBounds.Inflate(new Thickness(GapOffsetX, GapOffsetY));;
        }
    }

    private PlacementMode GetPopupPlacement(TourPlacementMode placement)
    {
        return placement switch
        {
            TourPlacementMode.Left => PlacementMode.Left,
            TourPlacementMode.LeftTop => PlacementMode.LeftEdgeAlignedTop,
            TourPlacementMode.LeftBottom => PlacementMode.LeftEdgeAlignedBottom,
            TourPlacementMode.Right => PlacementMode.Right,
            TourPlacementMode.RightTop => PlacementMode.RightEdgeAlignedTop,
            TourPlacementMode.RightBottom => PlacementMode.RightEdgeAlignedBottom,
            TourPlacementMode.Top => PlacementMode.Top,
            TourPlacementMode.TopLeft => PlacementMode.TopEdgeAlignedLeft,
            TourPlacementMode.TopRight => PlacementMode.TopEdgeAlignedRight,
            TourPlacementMode.Bottom => PlacementMode.Bottom,
            TourPlacementMode.BottomLeft => PlacementMode.BottomEdgeAlignedLeft,
            TourPlacementMode.BottomRight => PlacementMode.BottomEdgeAlignedRight,
            TourPlacementMode.Center => PlacementMode.Custom,
            _ => PlacementMode.Bottom
        };
    }

    private void ConfigurePopupOffset()
    {
        var target = GetCurrentTarget();
        if (target == null || _popup == null)
        {
            return;
        }
        if (_popup.Child!.DesiredSize == default)
        {
            LayoutHelper.MeasureChild(_popup.Child, Size.Infinity, new Thickness());
        }
        var pointAtCenterOffset =
            CalculatePopupPositionDelta(target, _popup.Child, _popup.Placement, _popup.PlacementAnchor,
                _popup.PlacementGravity);
        AdjustForGap(Placement);
        var offsetX = 0.0d;
        var offsetY = 0.0d;
        if (IsPointAtCenter)
        {
            offsetX += pointAtCenterOffset.X;
            offsetY += pointAtCenterOffset.Y;
        }
        
        _popup.HorizontalOffset = offsetX;
        _popup.VerticalOffset   = offsetY;
    }

    private void AdjustForGap(TourPlacementMode placement)
    {
        Debug.Assert(_popup != null);
        var defaultMarginToAnchor  = (TokenResourceUtils.FindTokenResource(this, TourTokenKind.PopupMarginToAnchor) as double?) ?? 0.0d;
        var value = placement switch
        {
            TourPlacementMode.Left => GapOffsetX,
            TourPlacementMode.LeftTop => GapOffsetX,
            TourPlacementMode.LeftBottom => GapOffsetX,
            TourPlacementMode.Right => GapOffsetX,
            TourPlacementMode.RightTop => GapOffsetX,
            TourPlacementMode.RightBottom => GapOffsetX,
            TourPlacementMode.Top => GapOffsetY,
            TourPlacementMode.TopLeft => GapOffsetY,
            TourPlacementMode.TopRight => GapOffsetY,
            TourPlacementMode.Bottom => GapOffsetY,
            TourPlacementMode.BottomLeft => GapOffsetY,
            TourPlacementMode.BottomRight => GapOffsetY,
            _ => 0.0d
        };
        _popup.MarginToAnchor = value + defaultMarginToAnchor;
    }
    
    private Point CalculatePopupPositionDelta(Control anchorTarget,
                                              Control? flyoutPresenter,
                                              PlacementMode placement,
                                              PopupAnchor? anchor = null,
                                              PopupGravity? gravity = null)
    {
        var offsetX = 0d;
        var offsetY = 0d;
        if (IsShowArrow && IsPointAtCenter)
        {
            if (PopupUtils.CanEnabledArrow(placement, anchor, gravity))
            {
                if (flyoutPresenter is ArrowDecoratedBox arrowDecoratedBox)
                {
                    var arrowVertexPoint = arrowDecoratedBox.ArrowVertexPoint;

                    var anchorSize = anchorTarget.Bounds.Size;
                    var centerX    = anchorSize.Width / 2;
                    var centerY    = anchorSize.Height / 2;
                    // 这里计算不需要全局坐标
                    if (placement == PlacementMode.TopEdgeAlignedLeft ||
                        placement == PlacementMode.BottomEdgeAlignedLeft)
                    {
                        offsetX += centerX - arrowVertexPoint.Item1;
                    }
                    else if (placement == PlacementMode.TopEdgeAlignedRight ||
                             placement == PlacementMode.BottomEdgeAlignedRight)
                    {
                        offsetX -= centerX - arrowVertexPoint.Item2;
                    }
                    else if (placement == PlacementMode.RightEdgeAlignedTop ||
                             placement == PlacementMode.LeftEdgeAlignedTop)
                    {
                        offsetY += centerY - arrowVertexPoint.Item1;
                    }
                    else if (placement == PlacementMode.RightEdgeAlignedBottom ||
                             placement == PlacementMode.LeftEdgeAlignedBottom)
                    {
                        offsetY -= centerY - arrowVertexPoint.Item2;
                    }
                }
            }
        }

        return new Point(offsetX, offsetY);
    }
    
     protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
     {
         base.OnDetachedFromVisualTree(e);
         
         Steps.CollectionChanged -= HandleItemsViewCollectionChanged;
         CustomActions.CollectionChanged -= HandleCustomActionsChanged;
         
         _indicatorDisposables?.Dispose();
         _indicatorDisposables = null;
         
         if (_stepsView != null)
         {
             _stepsView.CloseRequest -= HandleCloseRequest;
             _stepsView.NavRequest -= HandleNavRequest;
         }
     }
     
     private IgnorePropertyChangedScope BeginIgnoringPropertyChanged() => new IgnorePropertyChangedScope(this);
     
     private readonly struct IgnorePropertyChangedScope : IDisposable
     {
         private readonly Tour _owner;
     
         public IgnorePropertyChangedScope(Tour owner)
         {
             _owner                        = owner;
             _owner._ignorePropertyChanged = true;
         }
     
         public void Dispose() => _owner._ignorePropertyChanged = false;
     }
 }
