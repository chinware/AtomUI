using System.Collections.Specialized;
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
using Avalonia.Metadata;

namespace AtomUI.Controls;

using AvaloniaTabControl = Avalonia.Controls.TabControl;

public class BaseTabControl : AvaloniaTabControl,
                              IMotionAwareControl,
                              IControlSharedTokenResourcesHost
{
    private static readonly FuncTemplate<Panel?> DefaultPanel =
        new(() => new StackPanel());

    #region 公共属性定义

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<BaseTabControl>();

    public static readonly StyledProperty<bool> TabAlignmentCenterProperty =
        AvaloniaProperty.Register<BaseTabControl, bool>(nameof(TabAlignmentCenter));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<BaseTabControl>();
    
    public static readonly StyledProperty<double> HeaderStartEdgePaddingProperty = 
        AvaloniaProperty.Register<BaseTabControl, double>(nameof (HeaderStartEdgePadding));
    
    public static readonly StyledProperty<double> HeaderEndEdgePaddingProperty = 
        AvaloniaProperty.Register<BaseTabControl, double>(nameof (HeaderEndEdgePadding));
    
    public static readonly StyledProperty<Thickness> ContentPaddingProperty = 
        AvaloniaProperty.Register<BaseTabControl, Thickness>(nameof (ContentPadding));
    
    public static readonly StyledProperty<double> TabAndContentGutterProperty =
        AvaloniaProperty.Register<BaseTabControl, double>(nameof(TabAndContentGutter));
    
    public static readonly StyledProperty<object?> HeaderStartExtraContentProperty = 
        AvaloniaProperty.Register<BaseTabControl, object?>(nameof (HeaderStartExtraContent));
    
    public static readonly StyledProperty<IDataTemplate?> HeaderStartExtraContentTemplateProperty =
        AvaloniaProperty.Register<ContentControl, IDataTemplate?>(nameof(HeaderStartExtraContentTemplate));
    
    public static readonly StyledProperty<object?> HeaderEndExtraContentProperty = 
        AvaloniaProperty.Register<BaseTabControl, object?>(nameof (HeaderEndExtraContent));
    
    public static readonly StyledProperty<IDataTemplate?> HeaderEndExtraContentTemplateProperty =
        AvaloniaProperty.Register<ContentControl, IDataTemplate?>(nameof(HeaderEndExtraContentTemplate));

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
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
    
    public Thickness ContentPadding
    {
        get => GetValue(ContentPaddingProperty);
        set => SetValue(ContentPaddingProperty, value);
    }
    
    public double TabAndContentGutter
    {
        get => GetValue(TabAndContentGutterProperty);
        set => SetValue(TabAndContentGutterProperty, value);
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

    #region 内部属性实现
    
    internal static readonly DirectProperty<BaseTabControl, Thickness> TabStripMarginProperty =
        AvaloniaProperty.RegisterDirect<BaseTabControl, Thickness>(nameof(TabStripMargin),
            o => o.TabStripMargin,
            (o, v) => o.TabStripMargin = v);
    
    internal static readonly DirectProperty<BaseTabControl, Thickness> EffectiveHeaderPaddingProperty =
        AvaloniaProperty.RegisterDirect<BaseTabControl, Thickness>(nameof(EffectiveHeaderPadding),
            o => o.EffectiveHeaderPadding,
            (o, v) => o.EffectiveHeaderPadding = v);

    private Thickness _tabStripMargin;

    internal Thickness TabStripMargin
    {
        get => _tabStripMargin;
        set => SetAndRaise(TabStripMarginProperty, ref _tabStripMargin, value);
    }
    
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
    
    private Panel? _alignWrapper;
    private Point _tabStripBorderStartPoint;
    private Point _tabStripBorderEndPoint;
    private IDisposable? _borderThicknessDisposable;
    private protected readonly Dictionary<TabItem, CompositeDisposable> ItemsBindingDisposables = new();

    static BaseTabControl()
    {
        AutoScrollToSelectedItemProperty.OverrideDefaultValue<BaseTabControl>(false);
        ItemsPanelProperty.OverrideDefaultValue<BaseTabControl>(DefaultPanel);
        AffectsRender<BaseTabControl>(BorderBrushProperty);
        AffectsMeasure<BaseTabControl>(TabStripMarginProperty, TabAndContentGutterProperty);
    }

    public BaseTabControl()
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
                    if (item is TabItem tabItem)
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

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _alignWrapper   = e.NameScope.Find<Panel>(TabControlThemeConstants.AlignWrapperPart);
        HandlePlacementChanged();
        ConfigureEffectiveHeaderPadding();
    }
    
    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is TabItem tabItem)
        {
            var disposables = new CompositeDisposable(4);
            
            if (item != null && item is not Visual)
            {
                if (!tabItem.IsSet(TabItem.ContentProperty))
                {
                    tabItem.SetCurrentValue(TabItem.ContentProperty, item);
                }
            }

            if (ItemTemplate != null)
            {
                disposables.Add(BindUtils.RelayBind(this, ItemTemplateProperty, tabItem, TabItem.ContentTemplateProperty));
            }
            
            disposables.Add(BindUtils.RelayBind(this, SizeTypeProperty, tabItem, TabItem.SizeTypeProperty));
            disposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, tabItem, TabItem.IsMotionEnabledProperty));
            
            PrepareTabItem(tabItem, item, index, disposables);
            
            if (ItemsBindingDisposables.TryGetValue(tabItem, out var oldDisposables))
            {
                oldDisposables.Dispose();
                ItemsBindingDisposables.Remove(tabItem);
            }
            ItemsBindingDisposables.Add(tabItem, disposables);
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(container), "The container type is incorrect, it must be type TabItem.");
        }
    }
    
    protected virtual void PrepareTabItem(TabItem tabItem, object? item, int index, CompositeDisposable compositeDisposable)
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
        if (change.Property == TabStripPlacementProperty || change.Property == TabAndContentGutterProperty)
        {
            UpdatePseudoClasses();
            HandlePlacementChanged();
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

    private void HandlePlacementChanged()
    {
        if (TabStripPlacement == Dock.Top)
        {
            SetCurrentValue(TabStripMarginProperty, new Thickness(0, 0, 0, TabAndContentGutter));
        }
        else if (TabStripPlacement == Dock.Right)
        {
            SetCurrentValue(TabStripMarginProperty, new Thickness(TabAndContentGutter, 0, 0, 0));
        }
        else if (TabStripPlacement == Dock.Bottom)
        {
            SetCurrentValue(TabStripMarginProperty, new Thickness(0, TabAndContentGutter, 0, 0));
        }
        else
        {
            SetCurrentValue(TabStripMarginProperty, new Thickness(0, 0, TabAndContentGutter, 0));
        }

        ConfigureEffectiveHeaderPadding();
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

    private void SetupTabStripBorderPoints()
    {
        if (_alignWrapper is not null)
        {
            var offset          = _alignWrapper.TranslatePoint(new Point(0, 0), this) ?? default;
            var size            = _alignWrapper.Bounds.Size;
            var borderThickness = BorderThickness.Left;
            var offsetDelta     = borderThickness / 2;
            if (TabStripPlacement == Dock.Top)
            {
                _tabStripBorderStartPoint = new Point(0, size.Height - offsetDelta);
                _tabStripBorderEndPoint   = new Point(size.Width, size.Height - offsetDelta);
            }
            else if (TabStripPlacement == Dock.Right)
            {
                _tabStripBorderStartPoint = new Point(offsetDelta, 0);
                _tabStripBorderEndPoint   = new Point(offsetDelta, size.Height);
            }
            else if (TabStripPlacement == Dock.Bottom)
            {
                _tabStripBorderStartPoint = new Point(0, offsetDelta);
                _tabStripBorderEndPoint   = new Point(size.Width, offsetDelta);
            }
            else
            {
                _tabStripBorderStartPoint = new Point(size.Width - offsetDelta, 0);
                _tabStripBorderEndPoint   = new Point(size.Width - offsetDelta, size.Height);
            }

            _tabStripBorderStartPoint += offset;
            _tabStripBorderEndPoint   += offset;
        }
    }

    public override void Render(DrawingContext context)
    {
        SetupTabStripBorderPoints();
        var borderThickness = BorderThickness.Left;
        using var optionState = context.PushRenderOptions(new RenderOptions
        {
            EdgeMode = EdgeMode.Aliased
        });
        context.DrawLine(new Pen(BorderBrush, borderThickness), _tabStripBorderStartPoint, _tabStripBorderEndPoint);
    }
    
}