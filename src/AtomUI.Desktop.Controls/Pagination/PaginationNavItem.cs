using AtomUI.Animations;
using Avalonia.Threading;
using AtomUI.Controls;
using AtomUI.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
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
    
    public static readonly StyledProperty<PathIcon?> IconProperty =
        AvaloniaProperty.Register<PaginationNavItem, PathIcon?>(nameof(Icon));
    
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
    
    public PathIcon? Icon
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

    private Panel? _contentLayout;
    private IconPresenter? _iconPresenter;
    private Avalonia.Controls.TextBlock? _contentTextBlock;
    
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
        
        if (change.Property == IsPressedProperty)
        {
            UpdatePseudoClasses();
        }
        else if (change.Property == PaginationItemTypeProperty)
        {
            ConfigureDisplaySlot();
        }
        else if (change.Property == IconProperty)
        {
            SyncIconPresenter();
        }
        else if (change.Property == ContentProperty)
        {
            SyncContentTextBlock();
        }
        else if (change.Property == IsEnabledProperty)
        {
            SyncDisplaySlotEnabled();
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        ReleaseDisplaySlot();
        base.OnApplyTemplate(e);
        _contentLayout = e.NameScope.Find<Panel>("PART_RootLayout");
        ConfigureDisplaySlot();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        ReleaseDisplaySlot();
        base.OnDetachedFromVisualTree(e);
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

    protected override void OnLostFocus(FocusChangedEventArgs e)
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

    private void ConfigureDisplaySlot()
    {
        if (_contentLayout is null)
        {
            return;
        }

        if (PaginationItemType == PaginationItemType.PageIndicator)
        {
            DetachIconPresenter();
            EnsureContentTextBlock();
        }
        else
        {
            DetachContentTextBlock();
            EnsureIconPresenter();
        }
    }

    private void EnsureIconPresenter()
    {
        if (_contentLayout is null)
        {
            return;
        }

        if (_iconPresenter is null)
        {
            _iconPresenter = new IconPresenter
            {
                Name                = "IconPresenter",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center
            };
            _iconPresenter.SetTemplatedParent(this);
            _contentLayout.Children.Add(_iconPresenter);
        }

        SyncIconPresenter();
    }

    private void EnsureContentTextBlock()
    {
        if (_contentLayout is null)
        {
            return;
        }

        if (_contentTextBlock is null)
        {
            _contentTextBlock = new Avalonia.Controls.TextBlock
            {
                Name                = "PART_ContentTextBlock",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center
            };
            _contentTextBlock.SetTemplatedParent(this);
            _contentLayout.Children.Add(_contentTextBlock);
        }

        SyncContentTextBlock();
    }

    private void SyncIconPresenter()
    {
        if (_iconPresenter is null)
        {
            return;
        }

        _iconPresenter.SetCurrentValue(IconPresenter.IconProperty, Icon);
        _iconPresenter.SetCurrentValue(IsEnabledProperty, IsEnabled);
    }

    private void SyncContentTextBlock()
    {
        if (_contentTextBlock is null)
        {
            return;
        }

        _contentTextBlock.SetCurrentValue(Avalonia.Controls.TextBlock.TextProperty, Content?.ToString());
        _contentTextBlock.SetCurrentValue(IsEnabledProperty, IsEnabled);
    }

    private void SyncDisplaySlotEnabled()
    {
        if (_iconPresenter is not null)
        {
            _iconPresenter.SetCurrentValue(IsEnabledProperty, IsEnabled);
        }

        if (_contentTextBlock is not null)
        {
            _contentTextBlock.SetCurrentValue(IsEnabledProperty, IsEnabled);
        }
    }

    private void ReleaseDisplaySlot()
    {
        DetachIconPresenter();
        DetachContentTextBlock();
        _contentLayout = null;
    }

    private void DetachIconPresenter()
    {
        if (_iconPresenter is null)
        {
            return;
        }

        RemoveDisplayChild(_iconPresenter);
        _iconPresenter.SetCurrentValue(IconPresenter.IconProperty, null);
        _iconPresenter.SetTemplatedParent(null);
        _iconPresenter = null;
    }

    private void DetachContentTextBlock()
    {
        if (_contentTextBlock is null)
        {
            return;
        }

        RemoveDisplayChild(_contentTextBlock);
        _contentTextBlock.SetCurrentValue(Avalonia.Controls.TextBlock.TextProperty, null);
        _contentTextBlock.SetTemplatedParent(null);
        _contentTextBlock = null;
    }

    private void RemoveDisplayChild(Control child)
    {
        if (child.GetVisualParent() is Panel parent)
        {
            parent.Children.Remove(child);
            return;
        }

        _contentLayout?.Children.Remove(child);
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
