using System.Diagnostics;
using AtomUI.Animations;
using AtomUI.Controls.Themes;
using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Reflection;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.VisualTree;

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
    
    internal static readonly StyledProperty<bool> IsMotionEnabledProperty
        = MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<NodeSwitcherButton>();

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
    
    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    internal bool IsNodeAnimating { get; set; } = false;
    
    #endregion

    private readonly BorderRenderHelper _borderRenderHelper;
    private IconPresenter? _rotationIconPresenter;

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
    
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        SetupDefaultIcons();
    }

    private void ConfigureTransitions()
    {
        if (IsMotionEnabled)
        {
            Transitions ??= new Transitions
            {
                TransitionUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty),
            };
            if (_rotationIconPresenter != null)
            {
                _rotationIconPresenter.Transitions = new Transitions()
                {
                    TransitionUtils.CreateTransition<TransformOperationsTransition>(IconPresenter.RenderTransformProperty),
                };
            }
        }
        else
        {
            Transitions = null;
            if (_rotationIconPresenter != null)
            {
                _rotationIconPresenter.Transitions = null;
            }
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == CollapseIconProperty ||
                change.Property == ExpandIconProperty ||
                change.Property == LoadingIconProperty ||
                change.Property == RotationIconProperty)
            {
                if (change.OldValue is Icon oldIcon)
                {
                    oldIcon.SetTemplatedParent(null);
                }

                if (change.NewValue is Icon newIcon)
                {
                    newIcon.SetTemplatedParent(this);
                }

                SetupDefaultIcons();
            }
            else if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions();
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

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _rotationIconPresenter = e.NameScope.Find<IconPresenter>(NodeSwitcherButtonThemeConstants.RotationIconPresenterPart);
        ConfigureTransitions();
    }

    private void SetupDefaultIcons()
    {
        if (ExpandIcon == null)
        {
            ClearValue(ExpandIconProperty);
            SetValue(ExpandIconProperty, AntDesignIconPackage.PlusSquareOutlined(), BindingPriority.Template);
        }
        
        if (CollapseIcon == null)
        {
            ClearValue(CollapseIconProperty);
            SetValue(CollapseIconProperty, AntDesignIconPackage.MinusSquareOutlined(), BindingPriority.Template);
        }
        
        if (RotationIcon == null)
        {
            ClearValue(RotationIconProperty);
            SetValue(RotationIconProperty, AntDesignIconPackage.CaretRightOutlined(), BindingPriority.Template);
        }
        
        if (LeafIcon == null)
        {
            ClearValue(LeafIconProperty);
            SetValue(LeafIconProperty, AntDesignIconPackage.FileOutlined(), BindingPriority.Template);
        }
        Debug.Assert(ExpandIcon != null);
        ExpandIcon.SetTemplatedParent(this);
        
        Debug.Assert(CollapseIcon != null);
        CollapseIcon.SetTemplatedParent(this);
        
        Debug.Assert(RotationIcon != null);
        RotationIcon.SetTemplatedParent(this);
        
        Debug.Assert(LeafIcon != null);
        LeafIcon.SetTemplatedParent(this);
    }

    protected override void Toggle()
    {
        if (IsNodeAnimating)
        {
            return;
        }
        base.Toggle();
    }
}