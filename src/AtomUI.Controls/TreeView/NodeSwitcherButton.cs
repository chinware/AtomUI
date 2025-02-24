using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.Media;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Controls;

internal enum NodeSwitcherButtonIconMode
{
    Default,
    Rotation,
    Leaf,
    Loading
}

internal class NodeSwitcherButton : ToggleButton
{
    #region 公共属性定义
    
    public static readonly StyledProperty<Icon?> ExpandIconProperty
        = AvaloniaProperty.Register<NodeSwitcherButton, Icon?>(nameof(ExpandIcon));

    public static readonly StyledProperty<Icon?> CollapseIconProperty
        = AvaloniaProperty.Register<NodeSwitcherButton, Icon?>(nameof(CollapseIcon));

    public static readonly StyledProperty<Icon?> LoadingIconProperty
        = AvaloniaProperty.Register<NodeSwitcherButton, Icon?>(nameof(LoadingIcon));

    public static readonly StyledProperty<Icon?> LeafIconProperty
        = AvaloniaProperty.Register<NodeSwitcherButton, Icon?>(nameof(LeafIcon));

    public static readonly StyledProperty<bool> IsLeafProperty
        = AvaloniaProperty.Register<NodeSwitcherButton, bool>(nameof(IsLeaf));
    
    public static readonly StyledProperty<bool> IsLoadingProperty
        = AvaloniaProperty.Register<NodeSwitcherButton, bool>(nameof(IsLoading), false);
    
    public static readonly StyledProperty<Icon?> RotationIconProperty
        = AvaloniaProperty.Register<NodeSwitcherButton, Icon?>(nameof(RotationIcon));

    public Icon? LoadingIcon
    {
        get => GetValue(LoadingIconProperty);
        set => SetValue(LoadingIconProperty, value);
    }

    public Icon? LeafIcon
    {
        get => GetValue(LeafIconProperty);
        set => SetValue(LeafIconProperty, value);
    }

    public bool IsLeaf
    {
        get => GetValue(IsLeafProperty);
        set => SetValue(IsLeafProperty, value);
    }
    
    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }
    
    public Icon? RotationIcon
    {
        get => GetValue(RotationIconProperty);
        set => SetValue(RotationIconProperty, value);
    }
    
    public Icon? ExpandIcon
    {
        get => GetValue(ExpandIconProperty);
        set => SetValue(ExpandIconProperty, value);
    }

    public Icon? CollapseIcon
    {
        get => GetValue(CollapseIconProperty);
        set => SetValue(CollapseIconProperty, value);
    }
    
    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<bool> IsLeafIconVisibleProperty
        = AvaloniaProperty.Register<NodeSwitcherButton, bool>(nameof(IsLeafIconVisible), true);
    
    internal static readonly DirectProperty<NodeSwitcherButton, NodeSwitcherButtonIconMode> IconModeProperty =
        AvaloniaProperty.RegisterDirect<NodeSwitcherButton, NodeSwitcherButtonIconMode>(
            nameof(IconMode),
            o => o.IconMode,
            (o, v) => o.IconMode = v);
    
    internal static readonly DirectProperty<NodeSwitcherButton, bool> ExpandIconVisibleProperty =
        AvaloniaProperty.RegisterDirect<NodeSwitcherButton, bool>(
            nameof(ExpandIconVisible),
            o => o.ExpandIconVisible,
            (o, v) => o.ExpandIconVisible = v);
    
    internal static readonly DirectProperty<NodeSwitcherButton, bool> CollapseIconVisibleProperty =
        AvaloniaProperty.RegisterDirect<NodeSwitcherButton, bool>(
            nameof(CollapseIconVisible),
            o => o.CollapseIconVisible,
            (o, v) => o.CollapseIconVisible = v);
    
    internal static readonly DirectProperty<NodeSwitcherButton, bool> IsMotionEnabledProperty
        = AvaloniaProperty.RegisterDirect<NodeSwitcherButton, bool>(nameof(IsMotionEnabled),
            o => o.IsMotionEnabled,
            (o, v) => o.IsMotionEnabled = v);

    internal bool IsLeafIconVisible
    {
        get => GetValue(IsLeafIconVisibleProperty);
        set => SetValue(IsLeafIconVisibleProperty, value);
    }
    
    private NodeSwitcherButtonIconMode _iconMode;

    internal NodeSwitcherButtonIconMode IconMode
    {
        get => _iconMode;
        set => SetAndRaise(IconModeProperty, ref _iconMode, value);
    }
    
    private bool _expandIconVisible;
    internal bool ExpandIconVisible
    {
        get => _expandIconVisible;
        set => SetAndRaise(ExpandIconVisibleProperty, ref _expandIconVisible, value);
    }
    
    private bool _collapseIconVisible;
    internal bool CollapseIconVisible
    {
        get => _collapseIconVisible;
        set => SetAndRaise(CollapseIconVisibleProperty, ref _collapseIconVisible, value);
    }
    
    private bool _isMotionEnabled;

    internal bool IsMotionEnabled
    {
        get => _isMotionEnabled;
        set => SetAndRaise(IsMotionEnabledProperty, ref _isMotionEnabled, value);
    }

    #endregion

    private readonly BorderRenderHelper _borderRenderHelper;

    static NodeSwitcherButton()
    {
        AffectsRender<NodeSwitcherButton>(BackgroundProperty);
        AffectsMeasure<NodeSwitcherButton>(ExpandIconProperty, CollapseIconProperty, IsCheckedProperty,
            LoadingIconProperty, LeafIconProperty);
    }

    public NodeSwitcherButton()
    {
        _borderRenderHelper = new BorderRenderHelper();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        SetupIconVisibility(IsChecked ?? false);
        SetupTransitions();
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

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == CollapseIconProperty ||
            change.Property == ExpandIconProperty ||
            change.Property == LoadingIconProperty ||
            change.Property == RotationIconProperty)
        {
            if (change.NewValue is Icon icon)
            {
                icon.HorizontalAlignment = HorizontalAlignment.Center;
                icon.VerticalAlignment = VerticalAlignment.Center;
            }
        }
        else if (change.Property == IsCheckedProperty)
        {
            var isChecked = (bool?)change.NewValue ?? false;
            SetupIconVisibility(isChecked);
        }
        else if (change.Property == IconModeProperty)
        {
            SetupTransitions();
        }
    }

    private void SetupIconVisibility(bool isChecked)
    {
        if (_iconMode != NodeSwitcherButtonIconMode.Default)
        {
            ExpandIconVisible = false;
            CollapseIconVisible = false;
        }
        else
        {
            if (isChecked)
            {
                ExpandIconVisible = false;
                CollapseIconVisible = true;
            }
            else
            {
                ExpandIconVisible = true;
                CollapseIconVisible = false;
            }
        }
    }

    public override void Render(DrawingContext context)
    {
        if (_iconMode != NodeSwitcherButtonIconMode.Leaf)
        {
            _borderRenderHelper.Render(context,
                Bounds.Size,
                new Thickness(),
                CornerRadius,
                BackgroundSizing.InnerBorderEdge,
                Background,
                null,
                default);
        }
    }
}