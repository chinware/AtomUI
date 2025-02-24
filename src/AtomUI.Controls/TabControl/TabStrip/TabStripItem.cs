using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Media;
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

public class TabStripItem : AvaloniaTabStripItem, ICustomHitTest
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
    
    #endregion

    private StackPanel? _contentLayout;
    private IconButton? _closeButton;

    private void SetupItemIcon()
    {
        if (Icon is not null)
        {
            UIStructureUtils.SetTemplateParent(Icon, this);
            Icon.Name = BaseTabStripItemTheme.ItemIconPart;
            if (Icon.ThemeType != IconThemeType.TwoTone)
            {
                TokenResourceBinder.CreateTokenBinding(Icon, Icon.NormalFilledBrushProperty,
                    TabControlTokenKey.ItemColor);
                TokenResourceBinder.CreateTokenBinding(Icon, Icon.ActiveFilledBrushProperty,
                    TabControlTokenKey.ItemHoverColor);
                TokenResourceBinder.CreateTokenBinding(Icon, Icon.SelectedFilledBrushProperty,
                    TabControlTokenKey.ItemSelectedColor);
                TokenResourceBinder.CreateTokenBinding(Icon, Icon.DisabledFilledBrushProperty,
                    SharedTokenKey.ColorTextDisabled);
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
            TokenResourceBinder.CreateTokenBinding(CloseIcon, WidthProperty,
                SharedTokenKey.IconSizeSM);
            TokenResourceBinder.CreateTokenBinding(CloseIcon, HeightProperty,
                SharedTokenKey.IconSizeSM);
        }

        CloseIcon.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);

        UIStructureUtils.SetTemplateParent(CloseIcon, this);
        if (CloseIcon.ThemeType != IconThemeType.TwoTone)
        {
            TokenResourceBinder.CreateTokenBinding(CloseIcon, Icon.NormalFilledBrushProperty,
                SharedTokenKey.ColorIcon);
            TokenResourceBinder.CreateTokenBinding(CloseIcon, Icon.ActiveFilledBrushProperty,
                SharedTokenKey.ColorIconHover);
            TokenResourceBinder.CreateTokenBinding(CloseIcon, Icon.DisabledFilledBrushProperty,
                SharedTokenKey.ColorTextDisabled);
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        HandleTemplateApplied(e.NameScope);
    }

    private void HandleTemplateApplied(INameScope scope)
    {
        _contentLayout = scope.Find<StackPanel>(BaseTabStripItemTheme.ContentLayoutPart);
        _closeButton   = scope.Find<IconButton>(BaseTabStripItemTheme.ItemCloseButtonPart);

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
        if (VisualRoot is not null)
        {
            if (change.Property == IconProperty)
            {
                var oldIcon = change.GetOldValue<Icon?>();
                if (oldIcon != null)
                {
                    UIStructureUtils.SetTemplateParent(oldIcon, null);
                }

                SetupItemIcon();
            }
        }

        if (change.Property == CloseIconProperty)
        {
            var oldIcon = change.GetOldValue<Icon?>();
            if (oldIcon != null)
            {
                UIStructureUtils.SetTemplateParent(oldIcon, null);
            }

            SetupCloseIcon();
        } 
        else if (change.Property == ShapeProperty)
        {
            HandleShapeChanged();
        }
        else if (change.Property == IsMotionEnabledProperty)
        {
            SetupTransitions();
        }
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        HandleShapeChanged();
    }

    private void HandleShapeChanged()
    {
        if (Shape == TabSharp.Line)
        {
            TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, TabStripItemTheme.ID);
        }
        else
        {
            TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, CardTabStripItemTheme.ID);
        }
    }

    public bool HitTest(Point point)
    {
        return true;
    }
}