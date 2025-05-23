using System.Reactive.Disposables;
using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Rendering;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

internal enum PaginationItemType
{
    Previous,
    PageIndicator,
    Next,
    Ellipses
}

internal class PaginationNavItem : ContentControl,
                                   ISelectable,
                                   ICustomHitTest,
                                   ITokenResourceConsumer
{
    public static readonly StyledProperty<bool> IsSelectedProperty =
        SelectingItemsControl.IsSelectedProperty.AddOwner<PaginationNavItem>();
    
    public static readonly StyledProperty<PaginationItemType> PaginationItemTypeProperty
        = AvaloniaProperty.Register<PaginationNavItem, PaginationItemType>(nameof(PaginationItemType));
    
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<PaginationNavItem>();

    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<PaginationNavItem>();
    
    public static readonly DirectProperty<PaginationNavItem, bool> IsPressedProperty =
        AvaloniaProperty.RegisterDirect<PaginationNavItem, bool>(nameof(IsPressed), b => b.IsPressed);
    
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

    CompositeDisposable? ITokenResourceConsumer.TokenBindingsDisposable => _tokenBindingsDisposable;
    
    private CompositeDisposable? _tokenBindingsDisposable;

    internal int PageNumber { get; set; } = -1;
    
    static PaginationNavItem()
    {
        SelectableMixin.Attach<PaginationNavItem>(IsSelectedProperty);
        PressedMixin.Attach<PaginationNavItem>();
        FocusableProperty.OverrideDefaultValue<PaginationNavItem>(true);
        AffectsMeasure<PaginationNavItem>(BorderThicknessProperty);
        AffectsRender<PaginationNavItem>(BackgroundProperty, BorderBrushProperty);
    }


    public PaginationNavItem()
    {
        _tokenBindingsDisposable = new CompositeDisposable();
        Classes.CollectionChanged += (sender, args) =>
        {
            if (Content is Icon icon)
            {
                if (Classes.Contains(StdPseudoClass.Disabled))
                {
                    icon.IconMode = IconMode.Disabled;
                }
                else
                {
                    icon.IconMode = IconMode.Normal;
                }
            }
        };
    }
    
    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
    }
    
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        SetupTransitions();
        if (Content is Icon icon)
        {
            SetupIconFillColors(icon);
            SetupIconSizeType(icon);
            SetupIconStatus(icon);
        }
        this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, BorderThicknessProperty,
            SharedTokenKey.BorderThickness,
            BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this)));
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                SetupTransitions();    
            }
            
            if (change.Property == ContentProperty)
            {
                if (change.NewValue is Icon newIcon)
                {
                    SetupIconFillColors(newIcon);
                    SetupIconSizeType(newIcon);
                    SetupIconStatus(newIcon);
                }
            }

            else if (change.Property == SizeTypeProperty)
            {
                if (Content is Icon icon)
                {
                    SetupIconSizeType(icon);
                }
            }
        }
        
        if (change.Property == IsPressedProperty)
        {
            UpdatePseudoClasses();
        }
        else if (change.Property == IsEnabledProperty)
        {
            if (Content is Icon icon)
            {
                SetupIconStatus(icon);
            }
        }
    }

    private void SetupIconFillColors(Icon icon)
    {
        this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(icon, Icon.NormalFilledBrushProperty, SharedTokenKey.ColorText));
        this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(icon, Icon.DisabledFilledBrushProperty, SharedTokenKey.ColorTextDisabled));
    }

    private void SetupIconStatus(Icon icon)
    {
        if (IsEnabled && !Classes.Contains(StdPseudoClass.Disabled))
        {
            icon.IconMode = IconMode.Normal;
        }
        else
        {
            icon.IconMode = IconMode.Disabled;
        }
    }
    
    private void SetupIconSizeType(Icon icon)
    {
        if (SizeType == SizeType.Large)
        {
            this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(icon, WidthProperty, SharedTokenKey.IconSizeLG));
            this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(icon, HeightProperty, SharedTokenKey.IconSizeLG));
        }
        else if (SizeType == SizeType.Middle)
        {
            this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(icon, WidthProperty, SharedTokenKey.IconSize));
            this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(icon, HeightProperty, SharedTokenKey.IconSize));
        }
        else
        {
            this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(icon, WidthProperty, SharedTokenKey.IconSizeSM));
            this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(icon, HeightProperty, SharedTokenKey.IconSizeSM));
        }
    }
    
    private void SetupTransitions()
    {
        if (IsMotionEnabled)
        {
            Transitions ??= new Transitions
            {
                AnimationUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty),
                AnimationUtils.CreateTransition<SolidColorBrushTransition>(BorderBrushProperty),
                AnimationUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty)
            };
        }
        else
        {
            Transitions = null;
        }
    }
    
    public bool HitTest(Point point)
    {
        return true;
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
                OnClick();
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
}