using AtomUI.Animations;
using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

internal enum PaginationItemType
{
    Previous,
    PageIndicator,
    Next,
    Ellipses
}

internal class PaginationNavItem : ContentControl, ISelectable
{
    public static readonly StyledProperty<bool> IsSelectedProperty =
        SelectingItemsControl.IsSelectedProperty.AddOwner<PaginationNavItem>();
    
    public static readonly StyledProperty<PaginationItemType> PaginationItemTypeProperty =
        AvaloniaProperty.Register<PaginationNavItem, PaginationItemType>(nameof(PaginationItemType));
    
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<PaginationNavItem>();

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<PaginationNavItem>();
    
    public static readonly DirectProperty<PaginationNavItem, bool> IsPressedProperty =
        AvaloniaProperty.RegisterDirect<PaginationNavItem, bool>(nameof(IsPressed), b => b.IsPressed);
    
    public static readonly StyledProperty<Icon?> IconProperty =
        AvaloniaProperty.Register<PaginationNavItem, Icon?>(nameof(Icon));
    
    public static readonly RoutedEvent<RoutedEventArgs> ClickEvent =
        RoutedEvent.Register<PaginationNavItem, RoutedEventArgs>(nameof(Click), RoutingStrategies.Bubble);

    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }
    
    public PaginationItemType PaginationItemType
    {
        get => GetValue(PaginationItemTypeProperty);
        set => SetValue(PaginationItemTypeProperty, value);
    }
    
    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public Icon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
    
    public event EventHandler<RoutedEventArgs>? Click
    {
        add => AddHandler(ClickEvent, value);
        remove => RemoveHandler(ClickEvent, value);
    }
    
    private bool _isPressed = false;
    public bool IsPressed
    {
        get => _isPressed;
        private set => SetAndRaise(IsPressedProperty, ref _isPressed, value);
    }

    internal int PageNumber { get; set; } = -1;
    
    static PaginationNavItem()
    {
        SelectableMixin.Attach<PaginationNavItem>(IsSelectedProperty);
        PressedMixin.Attach<PaginationNavItem>();
        FocusableProperty.OverrideDefaultValue<PaginationNavItem>(true);
        AffectsMeasure<PaginationNavItem>(BorderThicknessProperty);
        AffectsRender<PaginationNavItem>(BackgroundProperty, BorderBrushProperty);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (IsLoaded)
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions(true);    
            }
        }
        
        if (change.Property == IsPressedProperty)
        {
            UpdatePseudoClasses();
        }
    }
    
    private void ConfigureTransitions(bool force)
    {
        if (IsMotionEnabled)
        {
            if (force || Transitions == null)
            {
                Transitions = [
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty),
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(BorderBrushProperty),
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty)
                ];
            }
        }
        else
        {
            Transitions = null;
        }
    }

    protected virtual void OnClick()
    {
        var e = new RoutedEventArgs(ClickEvent);
        RaiseEvent(e);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            IsPressed = true;
            e.Handled = true;
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (IsPressed && e.InitialPressMouseButton == MouseButton.Left)
        {
            IsPressed = false;
            e.Handled = true;
            if (this.GetVisualsAt(e.GetPosition(this)).Any(c => this == c || this.IsVisualAncestorOf(c)))
            {
                if (PaginationItemType != PaginationItemType.Ellipses)
                {
                    OnClick();
                }
            }
        }
    }
    
    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);

        IsPressed = false;
    }
    
    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        base.OnPointerCaptureLost(e);

        IsPressed = false;
    }
    
    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(StdPseudoClass.Pressed, IsPressed);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        ConfigureTransitions(false);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        Transitions = null;
    }
}