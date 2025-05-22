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

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<PaginationNavItem>();

    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<PaginationNavItem>();
    
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

    CompositeDisposable? ITokenResourceConsumer.TokenBindingsDisposable => _tokenBindingsDisposable;
    
    private CompositeDisposable? _tokenBindingsDisposable;
    
    static PaginationNavItem()
    {
        SelectableMixin.Attach<PaginationNavItem>(IsSelectedProperty);
        PressedMixin.Attach<PaginationNavItem>();
        FocusableProperty.OverrideDefaultValue<PaginationNavItem>(true);
        AffectsRender<PaginationNavItem>(BackgroundProperty);
    }


    public PaginationNavItem()
    {
        _tokenBindingsDisposable = new CompositeDisposable();
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
            else if (change.Property == IsEnabledProperty)
            {
                if (Content is Icon icon)
                {
                    SetupIconStatus(icon);
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
    }

    private void SetupIconFillColors(Icon icon)
    {
        this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(icon, Icon.NormalFilledBrushProperty, SharedTokenKey.ColorText));
        this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(icon, Icon.DisabledFilledBrushProperty, SharedTokenKey.ColorTextDisabled));
    }

    private void SetupIconStatus(Icon icon)
    {
        if (IsEnabled)
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
                AnimationUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty)
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
}