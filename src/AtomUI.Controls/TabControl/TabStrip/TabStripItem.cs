using System.Diagnostics;
using System.Reactive.Disposables;
using AtomUI.Animations;
using AtomUI.Controls.Themes;
using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Rendering;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

using AvaloniaTabStripItem = Avalonia.Controls.Primitives.TabStripItem;

public enum TabSharp
{
    Line,
    Card
}

public class TabStripItem : AvaloniaTabStripItem,
                            ICustomHitTest,
                            IResourceBindingManager
{
    #region 公共属性定义

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<TabStripItem>();

    public static readonly StyledProperty<Icon?> IconProperty =
        AvaloniaProperty.Register<TabStripItem, Icon?>(nameof(Icon));

    public static readonly StyledProperty<Icon?> CloseIconProperty =
        AvaloniaProperty.Register<TabStripItem, Icon?>(nameof(CloseIcon));

    public static readonly StyledProperty<bool> IsClosableProperty =
        AvaloniaProperty.Register<TabStripItem, bool>(nameof(IsClosable));

    public static readonly DirectProperty<TabStripItem, Dock?> TabStripPlacementProperty =
        AvaloniaProperty.RegisterDirect<TabStripItem, Dock?>(nameof(TabStripPlacement), o => o.TabStripPlacement);

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public Icon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public Icon? CloseIcon
    {
        get => GetValue(CloseIconProperty);
        set => SetValue(CloseIconProperty, value);
    }

    public bool IsClosable
    {
        get => GetValue(IsClosableProperty);
        set => SetValue(IsClosableProperty, value);
    }

    private Dock? _tabStripPlacement;

    public Dock? TabStripPlacement
    {
        get => _tabStripPlacement;
        internal set => SetAndRaise(TabStripPlacementProperty, ref _tabStripPlacement, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<TabSharp> ShapeProperty =
        AvaloniaProperty.Register<TabStripItem, TabSharp>(nameof(Shape));
    
    internal static readonly StyledProperty<bool> IsMotionEnabledProperty
        = MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<TabStripItem>();

    public TabSharp Shape
    {
        get => GetValue(ShapeProperty);
        set => SetValue(ShapeProperty, value);
    }

    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable
    {
        get => _resourceBindingsDisposable;
        set => _resourceBindingsDisposable = value;
    }
    
    #endregion
    
    private IconButton? _closeButton;
    private CompositeDisposable? _resourceBindingsDisposable;
    private Border? _decorator;
    
    private void SetupDefaultCloseIcon()
    {
        if (CloseIcon is null)
        {
            ClearValue(CloseIconProperty);
            SetValue(CloseIconProperty, AntDesignIconPackage.CloseOutlined(), BindingPriority.Template);
        }
        Debug.Assert(CloseIcon is not null);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        SetupDefaultCloseIcon();

        base.OnApplyTemplate(e);
        _closeButton   = e.NameScope.Find<IconButton>(TabStripItemThemeConstants.ItemCloseButtonPart);
        
        if (_closeButton is not null)
        {
            _closeButton.Click += HandleCloseRequest;
        }

        _decorator = e.NameScope.Find<Border>(TabStripItemThemeConstants.DecoratorPart);
        ConfigureTransitions();
    }

    private void ConfigureTransitions()
    {
        if (IsMotionEnabled)
        {
            Transitions = new Transitions
            {
                TransitionUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty)
            };
            if (_decorator != null)
            {
                _decorator.Transitions = new Transitions()
                {
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(Border.BackgroundProperty)
                };
            }
        }
        else
        {
            Transitions = null;
            if (_decorator != null)
            {
                _decorator.Transitions?.Clear();
                _decorator.Transitions = null;
            }
        }
    }
    
    private void HandleCloseRequest(object? sender, RoutedEventArgs args)
    {
        if (Parent is BaseTabStrip tabStrip)
        {
            if (tabStrip.SelectedItem is TabStripItem selectedItem)
            {
                if (selectedItem == this)
                {
                    var     selectedIndex   = tabStrip.SelectedIndex;
                    object? newSelectedItem = null;
                    if (selectedIndex != 0)
                    {
                        newSelectedItem = tabStrip.Items[--selectedIndex];
                    }

                    tabStrip.Items.Remove(this);
                    tabStrip.SelectedItem = newSelectedItem;
                }
                else
                {
                    tabStrip.Items.Remove(this);
                }
            }
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (this.IsAttachedToLogicalTree())
        {
            if (change.Property == ShapeProperty)
            {
                SetupShapeThemeBindings();
            }
        }

        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions();
            }
        }
        
        if (change.Property == IconProperty ||
            change.Property == CloseIconProperty)
        {
            if (change.Property == CloseIconProperty)
            {
                SetupDefaultCloseIcon();
            }
        }
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        _resourceBindingsDisposable = new CompositeDisposable();
        SetupShapeThemeBindings();
        base.OnAttachedToLogicalTree(e);
    }
    
    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
    }

    private void SetupShapeThemeBindings()
    {
        if (Shape == TabSharp.Line)
        {
            this.AddResourceBindingDisposable(
                TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, TabStripThemeConstants.TabStripItemThemeId));
        }
        else
        {
            this.AddResourceBindingDisposable(
                TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, TabStripThemeConstants.CardTabStripItemThemeId));
        }
    }

    public bool HitTest(Point point)
    {
        return true;
    }
}