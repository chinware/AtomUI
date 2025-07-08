using System.Reactive.Disposables;
using AtomUI.Controls.Themes;
using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Media;

namespace AtomUI.Controls;

using AvaloniaTabStrip = Avalonia.Controls.Primitives.TabStrip;

public abstract class BaseTabStrip : AvaloniaTabStrip, 
                                     ISizeTypeAware,
                                     IMotionAwareControl,
                                     IControlSharedTokenResourcesHost,
                                     IResourceBindingManager
{
    private static readonly FuncTemplate<Panel?> DefaultPanel =
        new(() => new StackPanel());

    #region 公共属性定义

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<BaseTabStrip>();

    public static readonly StyledProperty<Dock> TabStripPlacementProperty =
        AvaloniaProperty.Register<BaseTabStrip, Dock>(nameof(TabStripPlacement), Dock.Top);

    public static readonly StyledProperty<bool> TabAlignmentCenterProperty =
        AvaloniaProperty.Register<BaseTabStrip, bool>(nameof(TabAlignmentCenter));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<BaseTabStrip>();
    
    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public Dock TabStripPlacement
    {
        get => GetValue(TabStripPlacementProperty);
        set => SetValue(TabStripPlacementProperty, value);
    }

    public bool TabAlignmentCenter
    {
        get => GetValue(TabAlignmentCenterProperty);
        set => SetValue(TabAlignmentCenterProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    #endregion
    
    #region 内部属性定义
    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => TabControlToken.ID;
    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable
    {
        get => _resourceBindingsDisposable;
        set => _resourceBindingsDisposable = value;
    }
    
    #endregion

    private Border? _frame;
    private CompositeDisposable? _resourceBindingsDisposable;

    static BaseTabStrip()
    {
        ItemsPanelProperty.OverrideDefaultValue<BaseTabStrip>(DefaultPanel);
        AutoScrollToSelectedItemProperty.OverrideDefaultValue<BaseTabStrip>(false);
        AffectsRender<BaseTabStrip>(TabStripPlacementProperty);
    }

    public BaseTabStrip()
    {
        this.RegisterResources();
        this.BindMotionProperties();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _frame        = e.NameScope.Find<Border>(TabStripThemeConstants.FramePart);
        UpdatePseudoClasses();
    }

    private void SetupBorderBinding()
    {
        if (_frame is not null)
        {
            this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, BorderThicknessProperty,
                SharedTokenKey.BorderThickness, BindingPriority.Template,
                new RenderScaleAwareThicknessConfigure(this)));
        }
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is TabStripItem tabStripItem)
        {
            tabStripItem.TabStripPlacement = TabStripPlacement;
            BindUtils.RelayBind(this, SizeTypeProperty, tabStripItem, TabStripItem.SizeTypeProperty);
            BindUtils.RelayBind(this, IsMotionEnabledProperty, tabStripItem, TabStripItem.IsMotionEnabledProperty);
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _resourceBindingsDisposable = new CompositeDisposable();
        SetupBorderBinding();
    }
    
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        this.DisposeTokenBindings();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == TabStripPlacementProperty)
        {
            UpdatePseudoClasses();
            for (var i = 0; i < ItemCount; ++i)
            {
                var itemContainer = ContainerFromIndex(i);
                if (itemContainer is TabStripItem tabStripItem)
                {
                    tabStripItem.TabStripPlacement = TabStripPlacement;
                }
            }
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(TabPseudoClass.Top, TabStripPlacement == Dock.Top);
        PseudoClasses.Set(TabPseudoClass.Right, TabStripPlacement == Dock.Right);
        PseudoClasses.Set(TabPseudoClass.Bottom, TabStripPlacement == Dock.Bottom);
        PseudoClasses.Set(TabPseudoClass.Left, TabStripPlacement == Dock.Left);
    }

    public override void Render(DrawingContext context)
    {
        Point startPoint      = default;
        Point endPoint        = default;
        var   borderThickness = BorderThickness.Left;
        var   offsetDelta     = borderThickness / 2;
        if (TabStripPlacement == Dock.Top)
        {
            startPoint = new Point(0, Bounds.Height - offsetDelta);
            endPoint   = new Point(Bounds.Width, Bounds.Height - offsetDelta);
        }
        else if (TabStripPlacement == Dock.Right)
        {
            startPoint = new Point(offsetDelta, 0);
            endPoint   = new Point(offsetDelta, Bounds.Height);
        }
        else if (TabStripPlacement == Dock.Bottom)
        {
            startPoint = new Point(0, offsetDelta);
            endPoint   = new Point(Bounds.Width, offsetDelta);
        }
        else
        {
            startPoint = new Point(Bounds.Width - offsetDelta, 0);
            endPoint   = new Point(Bounds.Width - offsetDelta, Bounds.Height);
        }

        using var optionState = context.PushRenderOptions(new RenderOptions
        {
            EdgeMode = EdgeMode.Aliased
        });
        context.DrawLine(new Pen(BorderBrush, borderThickness), startPoint, endPoint);
    }
}