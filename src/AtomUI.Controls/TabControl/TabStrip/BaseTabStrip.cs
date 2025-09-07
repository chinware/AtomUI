using System.Collections.Specialized;
using System.Reactive.Disposables;
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
using Avalonia.Metadata;

namespace AtomUI.Controls;

using AvaloniaTabStrip = Avalonia.Controls.Primitives.TabStrip;

public abstract class BaseTabStrip : AvaloniaTabStrip, 
                                     ISizeTypeAware,
                                     IMotionAwareControl,
                                     IControlSharedTokenResourcesHost
{
    private static readonly FuncTemplate<Panel?> DefaultPanel =
        new(() => new StackPanel());
    
    private protected readonly Dictionary<TabStripItem, CompositeDisposable> ItemsBindingDisposables = new();

    #region 公共属性定义

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<BaseTabStrip>();

    public static readonly StyledProperty<Dock> TabStripPlacementProperty =
        AvaloniaProperty.Register<BaseTabStrip, Dock>(nameof(TabStripPlacement), Dock.Top);

    public static readonly StyledProperty<bool> TabAlignmentCenterProperty =
        AvaloniaProperty.Register<BaseTabStrip, bool>(nameof(TabAlignmentCenter));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<BaseTabStrip>();
    
    public static readonly StyledProperty<double> HeaderStartEdgePaddingProperty = 
        AvaloniaProperty.Register<BaseTabStrip, double>(nameof (HeaderStartEdgePadding));
    
    public static readonly StyledProperty<double> HeaderEndEdgePaddingProperty = 
        AvaloniaProperty.Register<BaseTabStrip, double>(nameof (HeaderEndEdgePadding));
    
    public static readonly StyledProperty<object?> HeaderStartExtraContentProperty = 
        AvaloniaProperty.Register<BaseTabStrip, object?>(nameof (HeaderStartExtraContent));
    
    public static readonly StyledProperty<IDataTemplate?> HeaderStartExtraContentTemplateProperty =
        AvaloniaProperty.Register<BaseTabStrip, IDataTemplate?>(nameof(HeaderStartExtraContentTemplate));
    
    public static readonly StyledProperty<object?> HeaderEndExtraContentProperty = 
        AvaloniaProperty.Register<BaseTabStrip, object?>(nameof (HeaderEndExtraContent));
    
    public static readonly StyledProperty<IDataTemplate?> HeaderEndExtraContentTemplateProperty =
        AvaloniaProperty.Register<BaseTabStrip, IDataTemplate?>(nameof(HeaderEndExtraContentTemplate));
    
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

    public double HeaderStartEdgePadding
    {
        get => GetValue(HeaderStartEdgePaddingProperty);
        set => SetValue(HeaderStartEdgePaddingProperty, value);
    }
    
    public double HeaderEndEdgePadding
    {
        get => GetValue(HeaderEndEdgePaddingProperty);
        set => SetValue(HeaderEndEdgePaddingProperty, value);
    }
    
    [DependsOn(nameof(HeaderStartExtraContentTemplate))]
    public object? HeaderStartExtraContent
    {
        get => GetValue(HeaderStartExtraContentProperty);
        set => SetValue(HeaderStartExtraContentProperty, value);
    }
    
    public IDataTemplate? HeaderStartExtraContentTemplate
    {
        get => GetValue(HeaderStartExtraContentTemplateProperty);
        set => SetValue(HeaderStartExtraContentTemplateProperty, value);
    }
    
    [DependsOn(nameof(HeaderEndExtraContentTemplate))]
    public object? HeaderEndExtraContent
    {
        get => GetValue(HeaderEndExtraContentProperty);
        set => SetValue(HeaderEndExtraContentProperty, value);
    }
    
    public IDataTemplate? HeaderEndExtraContentTemplate
    {
        get => GetValue(HeaderEndExtraContentTemplateProperty);
        set => SetValue(HeaderEndExtraContentTemplateProperty, value);
    }

    #endregion
    
    #region 内部属性定义
    internal static readonly DirectProperty<BaseTabStrip, Thickness> EffectiveHeaderPaddingProperty =
        AvaloniaProperty.RegisterDirect<BaseTabStrip, Thickness>(nameof(EffectiveHeaderPadding),
            o => o.EffectiveHeaderPadding,
            (o, v) => o.EffectiveHeaderPadding = v);
        
    private Thickness _effectiveHeaderPadding;

    internal Thickness EffectiveHeaderPadding
    {
        get => _effectiveHeaderPadding;
        set => SetAndRaise(EffectiveHeaderPaddingProperty, ref _effectiveHeaderPadding, value);
    }
    
    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => TabControlToken.ID;
    
    #endregion
    
    private IDisposable? _borderThicknessDisposable;

    static BaseTabStrip()
    {
        ItemsPanelProperty.OverrideDefaultValue<BaseTabStrip>(DefaultPanel);
        AutoScrollToSelectedItemProperty.OverrideDefaultValue<BaseTabStrip>(false);
        AffectsRender<BaseTabStrip>(TabStripPlacementProperty, BorderBrushProperty);
        AffectsMeasure<BaseTabStrip>(TabStripPlacementProperty);
    }

    public BaseTabStrip()
    {
        this.RegisterResources();
        Items.CollectionChanged += HandleCollectionChanged;
    }
    
    private void HandleCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems.Count > 0)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is TabStripItem tabItem)
                    {
                        if (ItemsBindingDisposables.TryGetValue(tabItem, out var disposable))
                        {
                            disposable.Dispose();
                            ItemsBindingDisposables.Remove(tabItem);
                        }
                    }
                }
            }
        }
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is TabStripItem tabStripItem)
        {
            var disposables = new CompositeDisposable(4);
            tabStripItem.TabStripPlacement = TabStripPlacement;
            
            if (item != null && item is not Visual)
            {
                if (!tabStripItem.IsSet(TabStripItem.ContentProperty))
                {
                    tabStripItem.SetCurrentValue(TabStripItem.ContentProperty, item);
                }
            }

            if (ItemTemplate != null)
            {
                disposables.Add(BindUtils.RelayBind(this, ItemTemplateProperty, tabStripItem, TabItem.ContentTemplateProperty));
            }
            
            disposables.Add(BindUtils.RelayBind(this, SizeTypeProperty, tabStripItem, TabStripItem.SizeTypeProperty));
            disposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, tabStripItem, TabStripItem.IsMotionEnabledProperty));
            
            PrepareTabStripItem(tabStripItem, item, index, disposables);
            
            if (ItemsBindingDisposables.TryGetValue(tabStripItem, out var oldDisposables))
            {
                oldDisposables.Dispose();
                ItemsBindingDisposables.Remove(tabStripItem);
            }
            ItemsBindingDisposables.Add(tabStripItem, disposables);
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(container), "The container type is incorrect, it must be type TabStripItem.");
        }
    }

    protected virtual void PrepareTabStripItem(TabStripItem tabStripItem, object? item, int index, CompositeDisposable compositeDisposable)
    {
    }
    
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _borderThicknessDisposable = TokenResourceBinder.CreateTokenBinding(this, BorderThicknessProperty,
            SharedTokenKey.BorderThickness,
            BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this));
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _borderThicknessDisposable?.Dispose();
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

            ConfigureEffectiveHeaderPadding();
        }
        else if (change.Property == HeaderStartEdgePaddingProperty || change.Property == HeaderEndEdgePaddingProperty)
        {
            ConfigureEffectiveHeaderPadding();
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
    
    private void ConfigureEffectiveHeaderPadding()
    {
        if (TabStripPlacement == Dock.Top)
        {
            SetCurrentValue(EffectiveHeaderPaddingProperty, new  Thickness(HeaderStartEdgePadding, 0, HeaderEndEdgePadding, 0));
        }
        else if (TabStripPlacement == Dock.Right)
        {
            SetCurrentValue(EffectiveHeaderPaddingProperty, new Thickness(0, HeaderStartEdgePadding, 0, HeaderEndEdgePadding));
        }
        else if (TabStripPlacement == Dock.Bottom)
        {
            SetCurrentValue(EffectiveHeaderPaddingProperty, new  Thickness(HeaderStartEdgePadding, 0, HeaderEndEdgePadding, 0));
        }
        else
        {
            SetCurrentValue(EffectiveHeaderPaddingProperty, new Thickness(0, HeaderStartEdgePadding, 0, HeaderEndEdgePadding));
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        ConfigureEffectiveHeaderPadding();
    }
}