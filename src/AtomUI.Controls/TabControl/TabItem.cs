using System.Diagnostics;
using System.Reactive.Disposables;
using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Media;
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

using AvaloniaTabItem = Avalonia.Controls.TabItem;

public class TabItem : AvaloniaTabItem,
                       ICustomHitTest,
                       ITokenResourceConsumer
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
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<TabItem>();

    internal static readonly StyledProperty<TabSharp> ShapeProperty =
        AvaloniaProperty.Register<TabItem, TabSharp>(nameof(Shape));

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty
        = MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<TabItem>();

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
    
    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    CompositeDisposable? ITokenResourceConsumer.TokenBindingsDisposable => _tokenBindingsDisposable;

    #endregion

    private IconButton? _closeButton;
    private CompositeDisposable? _tokenBindingsDisposable;
    

    private void SetupDefaultCloseIcon()
    {
        if (CloseIcon is null)
        {
            ClearValue(CloseIconProperty);
            SetValue(CloseIconProperty, AntDesignIconPackage.CloseOutlined(), BindingPriority.Template);
        }
        Debug.Assert(CloseIcon is not null);
        CloseIcon.SetTemplatedParent(this);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _closeButton   = e.NameScope.Find<IconButton>(BaseTabItemTheme.ItemCloseButtonPart);

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
                SetupTransitions();
            }
        }
        
        if (change.Property == IconProperty ||
            change.Property == CloseIconProperty)
        {
            if (change.OldValue is Icon oldIcon)
            {
                oldIcon.SetTemplatedParent(null);
            }
            if (change.NewValue is Icon newIcon)
            {
                newIcon.SetTemplatedParent(this);
            }

            if (change.Property == CloseIconProperty && CloseIcon is null)
            {
                SetupDefaultCloseIcon();
            }
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

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        SetupTransitions();
        SetupDefaultCloseIcon();
    }

    private void SetupShapeThemeBindings()
    {
        if (Shape == TabSharp.Line)
        {
            this.AddTokenBindingDisposable(
                TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, TabItemTheme.ID));
        }
        else
        {
            this.AddTokenBindingDisposable(
                TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, CardTabItemTheme.ID));
        }
    }

    public bool HitTest(Point point)
    {
        return true;
    }
}