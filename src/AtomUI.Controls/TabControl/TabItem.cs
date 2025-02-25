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

using AvaloniaTabItem = Avalonia.Controls.TabItem;

public class TabItem : AvaloniaTabItem, ICustomHitTest
{
    #region 公共属性定义

    public static readonly StyledProperty<Icon?> IconProperty =
        AvaloniaProperty.Register<TabItem, Icon?>(nameof(Icon));

    public static readonly StyledProperty<Icon?> CloseIconProperty =
        AvaloniaProperty.Register<TabItem, Icon?>(nameof(CloseIcon));

    public static readonly StyledProperty<bool> IsClosableProperty =
        AvaloniaProperty.Register<TabItem, bool>(nameof(IsClosable));

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

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<SizeType> SizeTypeProperty =
        BaseTabControl.SizeTypeProperty.AddOwner<TabItem>();

    internal static readonly StyledProperty<TabSharp> ShapeProperty =
        AvaloniaProperty.Register<TabItem, TabSharp>(nameof(Shape));
    
    internal static readonly DirectProperty<TabItem, bool> IsMotionEnabledProperty
        = AvaloniaProperty.RegisterDirect<TabItem, bool>(nameof(IsMotionEnabled), 
            o => o.IsMotionEnabled,
            (o, v) => o.IsMotionEnabled = v);

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

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
            VisualAndLogicalUtils.SetTemplateParent(Icon, this);
            Icon.Name = BaseTabItemTheme.ItemIconPart;
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

        VisualAndLogicalUtils.SetTemplateParent(CloseIcon, this);
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
        _contentLayout = scope.Find<StackPanel>(BaseTabItemTheme.ContentLayoutPart);
        _closeButton   = scope.Find<IconButton>(BaseTabItemTheme.ItemCloseButtonPart);

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
            Transitions ??= new Transitions()
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
        if (Parent is BaseTabControl tabControl)
        {
            if (tabControl.SelectedItem is TabItem selectedItem)
            {
                if (selectedItem == this)
                {
                    var     selectedIndex   = tabControl.SelectedIndex;
                    object? newSelectedItem = null;
                    if (selectedIndex != 0)
                    {
                        newSelectedItem = tabControl.Items[--selectedIndex];
                    }

                    tabControl.Items.Remove(this);
                    tabControl.SelectedItem = newSelectedItem;
                }
                else
                {
                    tabControl.Items.Remove(this);
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
                    VisualAndLogicalUtils.SetTemplateParent(oldIcon, null);
                }

                SetupItemIcon();
            }
        }

        if (change.Property == CloseIconProperty)
        {
            var oldIcon = change.GetOldValue<Icon?>();
            if (oldIcon != null)
            {
                VisualAndLogicalUtils.SetTemplateParent(oldIcon, null);
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
            TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, TabItemTheme.ID);
        }
        else
        {
            TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, CardTabItemTheme.ID);
        }
    }

    public bool HitTest(Point point)
    {
        return true;
    }
}