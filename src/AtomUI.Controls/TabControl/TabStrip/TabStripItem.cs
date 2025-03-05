using System.Reactive.Disposables;
using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Rendering;

namespace AtomUI.Controls;

using AvaloniaTabStripItem = Avalonia.Controls.Primitives.TabStripItem;

public enum TabSharp
{
    Line,
    Card
}

public class TabStripItem : AvaloniaTabStripItem,
                            ICustomHitTest,
                            ITokenResourceConsumer
{
    #region 公共属性定义

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        BaseTabStrip.SizeTypeProperty.AddOwner<TabStripItem>();

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
    
    internal static readonly DirectProperty<TabStripItem, bool> IsMotionEnabledProperty
        = AvaloniaProperty.RegisterDirect<TabStripItem, bool>(nameof(IsMotionEnabled), 
            o => o.IsMotionEnabled,
            (o, v) => o.IsMotionEnabled = v);

    public TabSharp Shape
    {
        get => GetValue(ShapeProperty);
        set => SetValue(ShapeProperty, value);
    }

    private bool _isMotionEnabled;

    internal bool IsMotionEnabled
    {
        get => _isMotionEnabled;
        set => SetAndRaise(IsMotionEnabledProperty, ref _isMotionEnabled, value);
    }
    
    CompositeDisposable? ITokenResourceConsumer.TokenBindingsDisposable => _tokenBindingsDisposable;
    
    #endregion

    private StackPanel? _contentLayout;
    private IconButton? _closeButton;
    private CompositeDisposable? _tokenBindingsDisposable;

    private void SetupItemIcon()
    {
        if (Icon is not null)
        {
            Icon.SetTemplatedParent(this);
            Icon.Name = BaseTabStripItemTheme.ItemIconPart;
            if (Icon.ThemeType != IconThemeType.TwoTone)
            {
                this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(Icon, Icon.NormalFilledBrushProperty,
                    TabControlTokenKey.ItemColor));
                this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(Icon, Icon.ActiveFilledBrushProperty,
                    TabControlTokenKey.ItemHoverColor));
                this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(Icon, Icon.SelectedFilledBrushProperty,
                    TabControlTokenKey.ItemSelectedColor));
                this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(Icon, Icon.DisabledFilledBrushProperty,
                    SharedTokenKey.ColorTextDisabled));
            }

            if (_contentLayout is not null)
            {
                _contentLayout.Children.Insert(0, Icon);
            }
        }
    }

    private void SetupCloseIcon()
    {
        if (CloseIcon is null)
        {
            CloseIcon = AntDesignIconPackage.CloseOutlined();
            this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(CloseIcon, WidthProperty,
                SharedTokenKey.IconSizeSM));
            this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(CloseIcon, HeightProperty,
                SharedTokenKey.IconSizeSM));
        }

        CloseIcon.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);

        CloseIcon.SetTemplatedParent(this);
        if (CloseIcon.ThemeType != IconThemeType.TwoTone)
        {
            this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(CloseIcon, Icon.NormalFilledBrushProperty,
                SharedTokenKey.ColorIcon));
            this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(CloseIcon, Icon.ActiveFilledBrushProperty,
                SharedTokenKey.ColorIconHover));
            this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(CloseIcon, Icon.DisabledFilledBrushProperty,
                SharedTokenKey.ColorTextDisabled));
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _contentLayout = e.NameScope.Find<StackPanel>(BaseTabStripItemTheme.ContentLayoutPart);
        _closeButton   = e.NameScope.Find<IconButton>(BaseTabStripItemTheme.ItemCloseButtonPart);

        SetupItemIcon();
        SetupCloseIcon();
        SetupTransitions();
        if (_closeButton is not null)
        {
            _closeButton.Click += HandleCloseRequest;
        }
    }

    private void SetupTransitions()
    {
        if (IsMotionEnabled)
        {
            Transitions ??= new Transitions
            {
                AnimationUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty)
            };
        }
        else
        {
            Transitions = null;
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
            if (change.Property == IconProperty)
            {
                var oldIcon = change.GetOldValue<Icon?>();
                if (oldIcon != null)
                {
                    oldIcon.SetTemplatedParent(null);
                }

                SetupItemIcon();
            }
            else if (change.Property == ShapeProperty)
            {
                SetupShapeThemeBindings();
            }
            else if (change.Property == CloseIconProperty)
            {
                var oldIcon = change.GetOldValue<Icon?>();
                oldIcon?.SetTemplatedParent(null);

                SetupCloseIcon();
            }
        }
       
        if (change.Property == IsMotionEnabledProperty)
        {
            SetupTransitions();
        }
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        _tokenBindingsDisposable = new CompositeDisposable();
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
            this.AddTokenBindingDisposable(
                TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, TabStripItemTheme.ID));
        }
        else
        {
            this.AddTokenBindingDisposable(
                TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, CardTabStripItemTheme.ID));
        }
    }

    public bool HitTest(Point point)
    {
        return true;
    }
}