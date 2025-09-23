using AtomUI.Controls.Themes;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;

namespace AtomUI.Controls;

public class Carousel : SelectingItemsControl, 
                        IControlSharedTokenResourcesHost,
                        IMotionAwareControl
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
    
    public static readonly StyledProperty<bool> IsDraggableProperty = 
        AvaloniaProperty.Register<Carousel, bool>(nameof(IsDraggable));
    
    public static readonly StyledProperty<bool> IsInfiniteProperty = 
        AvaloniaProperty.Register<Carousel, bool>(nameof(IsInfinite));
    
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

    public bool IsDraggable
    {
        get => GetValue(IsDraggableProperty);
        set => SetValue(IsDraggableProperty, value);
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

    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => CarouselToken.ID;

    #endregion
    
    private IScrollable? _scroller;
    
    static Carousel()
    {
        SelectionModeProperty.OverrideDefaultValue<Carousel>(SelectionMode.AlwaysSelected);
        ItemsPanelProperty.OverrideDefaultValue<Carousel>(DefaultPanel);
    }
    
    public void Next()
    {
        if (SelectedIndex < ItemCount - 1)
        {
            ++SelectedIndex;
        }
    }
    
    public void Previous()
    {
        if (SelectedIndex > 0)
        {
            --SelectedIndex;
        }
    }
    
    protected override Size ArrangeOverride(Size finalSize)
    {
        var result = base.ArrangeOverride(finalSize);

        if (_scroller is not null)
        {
            _scroller.Offset = new(SelectedIndex, 0);
        }

        return result;
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _scroller = e.NameScope.Find<IScrollable>(CarouselThemeConstants.ScrollViewerPart);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == SelectedIndexProperty && _scroller is not null)
        {
            var value = change.GetNewValue<int>();
            _scroller.Offset = new(value, 0);
        }
        else if (change.Property == IsInfiniteProperty ||
                 change.Property == IsShowNavButtonsProperty ||
                 change.Property == SelectedIndexProperty ||
                 change.Property == ItemCountProperty)
        {
            ConfigureNavButtons();
        }
    }

    private void ConfigureNavButtons()
    {
        if (IsShowNavButtons)
        {
            if (IsInfinite)
            {
                SetCurrentValue(PreviousNavButtonVisibleProperty, false);
                SetCurrentValue(NextNavButtonVisibleProperty, false);
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
    }
}