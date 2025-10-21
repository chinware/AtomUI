using System.Diagnostics;
using AtomUI.Animations;
using AtomUI.Controls.Themes;
using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace AtomUI.Controls;

[PseudoClasses(TreeViewPseudoClass.NodeToggleTypeCheckBox, TreeViewPseudoClass.NodeToggleTypeRadio)]
internal class TreeViewItemHeader : ContentControl
{
    public static readonly StyledProperty<bool> IsExpandedProperty =
        TreeViewItem.IsExpandedProperty.AddOwner<TreeViewItemHeader>();
    
    public static readonly StyledProperty<Icon?> IconProperty =
        TreeViewItem.IconProperty.AddOwner<TreeViewItemHeader>();

    public static readonly StyledProperty<bool?> IsCheckedProperty =
        TreeViewItem.IsCheckedProperty.AddOwner<TreeViewItemHeader>();
    
    public static readonly StyledProperty<bool> IsSelectedProperty =
        TreeViewItem.IsSelectedProperty.AddOwner<TreeViewItemHeader>();

    public static readonly StyledProperty<Icon?> SwitcherExpandIconProperty =
        TreeViewItem.SwitcherExpandIconProperty.AddOwner<TreeViewItemHeader>();

    public static readonly StyledProperty<Icon?> SwitcherCollapseIconProperty =
        TreeViewItem.SwitcherCollapseIconProperty.AddOwner<TreeViewItemHeader>();

    public static readonly StyledProperty<Icon?> SwitcherRotationIconProperty =
        TreeViewItem.SwitcherRotationIconProperty.AddOwner<TreeViewItemHeader>();

    public static readonly StyledProperty<Icon?> SwitcherLoadingIconProperty =
        TreeViewItem.SwitcherLoadingIconProperty.AddOwner<TreeViewItemHeader>();

    public static readonly StyledProperty<Icon?> SwitcherLeafIconProperty =
        TreeViewItem.SwitcherLeafIconProperty.AddOwner<TreeViewItemHeader>();

    public static readonly DirectProperty<TreeViewItemHeader, bool> IsLeafProperty =
        AvaloniaProperty.RegisterDirect<TreeViewItemHeader, bool>(nameof(IsLeaf),
            o => o.IsLeaf,
            (o, v) => o.IsLeaf = v);

    public static readonly StyledProperty<bool> IsLoadingProperty =
        TreeViewItem.IsLoadingProperty.AddOwner<TreeViewItemHeader>();
    
    public static readonly StyledProperty<bool> IsIndicatorEnabledProperty =
        TreeViewItem.IsIndicatorEnabledProperty.AddOwner<TreeViewItemHeader>();
    
    public static readonly StyledProperty<string?> GroupNameProperty =
        TreeViewItem.GroupNameProperty.AddOwner<TreeViewItemHeader>();
    
    public bool IsExpanded
    {
        get => GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }
    
    public Icon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public Icon? SwitcherExpandIcon
    {
        get => GetValue(SwitcherExpandIconProperty);
        set => SetValue(SwitcherExpandIconProperty, value);
    }

    public Icon? SwitcherCollapseIcon
    {
        get => GetValue(SwitcherCollapseIconProperty);
        set => SetValue(SwitcherCollapseIconProperty, value);
    }

    public Icon? SwitcherRotationIcon
    {
        get => GetValue(SwitcherRotationIconProperty);
        set => SetValue(SwitcherRotationIconProperty, value);
    }

    public Icon? SwitcherLoadingIcon
    {
        get => GetValue(SwitcherLoadingIconProperty);
        set => SetValue(SwitcherLoadingIconProperty, value);
    }

    public Icon? SwitcherLeafIcon
    {
        get => GetValue(SwitcherLeafIconProperty);
        set => SetValue(SwitcherLeafIconProperty, value);
    }

    public bool? IsChecked
    {
        get => GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }
    
    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    private bool _isLeaf;

    public bool IsLeaf
    {
        get => _isLeaf;
        internal set => SetAndRaise(IsLeafProperty, ref _isLeaf, value);
    }

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public string? GroupName
    {
        get => GetValue(GroupNameProperty);
        set => SetValue(GroupNameProperty, value);
    }
    
    public bool IsIndicatorEnabled
    {
        get => GetValue(IsIndicatorEnabledProperty);
        set => SetValue(IsIndicatorEnabledProperty, value);
    }
    
    #region 内部属性定义

    internal static readonly StyledProperty<bool> IsHoverProperty =
        AvaloniaProperty.Register<TreeViewItemHeader, bool>(nameof(IsHover), false);
    
    public static readonly StyledProperty<IBrush?> ContentFrameBackgroundProperty =
        AvaloniaProperty.Register<TreeViewItemHeader, IBrush?>(nameof (ContentFrameBackground));

    internal static readonly DirectProperty<TreeViewItemHeader, TreeItemHoverMode> NodeHoverModeProperty =
        AvaloniaProperty.RegisterDirect<TreeViewItemHeader, TreeItemHoverMode>(nameof(NodeHoverMode),
            o => o.NodeHoverMode,
            (o, v) => o.NodeHoverMode = v);

    internal static readonly DirectProperty<TreeViewItemHeader, bool> IsShowLineProperty =
        AvaloniaProperty.RegisterDirect<TreeViewItemHeader, bool>(nameof(IsShowLine),
            o => o.IsShowLine,
            (o, v) => o.IsShowLine = v);

    internal static readonly DirectProperty<TreeViewItemHeader, bool> IsShowIconProperty =
        AvaloniaProperty.RegisterDirect<TreeViewItemHeader, bool>(nameof(IsShowIcon),
            o => o.IsShowIcon,
            (o, v) => o.IsShowIcon = v);

    internal static readonly DirectProperty<TreeViewItemHeader, bool> IconEffectiveVisibleProperty =
        AvaloniaProperty.RegisterDirect<TreeViewItemHeader, bool>(nameof(IconEffectiveVisible),
            o => o.IconEffectiveVisible,
            (o, v) => o.IconEffectiveVisible = v);

    internal static readonly DirectProperty<TreeViewItemHeader, bool> IsDraggingProperty =
        AvaloniaProperty.RegisterDirect<TreeViewItemHeader, bool>(nameof(IsDragging),
            o => o.IsDragging,
            (o, v) => o.IsDragging = v);

    internal static readonly DirectProperty<TreeViewItemHeader, bool> IsDragOverProperty =
        AvaloniaProperty.RegisterDirect<TreeViewItemHeader, bool>(nameof(IsDragOver),
            o => o.IsDragOver,
            (o, v) => o.IsDragOver = v);

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<TreeViewItemHeader>();

    internal static readonly DirectProperty<TreeViewItemHeader, NodeSwitcherButtonIconMode> SwitcherModeProperty =
        AvaloniaProperty.RegisterDirect<TreeViewItemHeader, NodeSwitcherButtonIconMode>(
            nameof(SwitcherMode),
            o => o.SwitcherMode,
            (o, v) => o.SwitcherMode = v);

    internal static readonly DirectProperty<TreeViewItemHeader, bool> IsSwitcherRotationProperty =
        AvaloniaProperty.RegisterDirect<TreeViewItemHeader, bool>(
            nameof(IsSwitcherRotation),
            o => o.IsSwitcherRotation,
            (o, v) => o.IsSwitcherRotation = v);
    
    internal static readonly StyledProperty<ItemToggleType> ToggleTypeProperty =
        TreeView.ToggleTypeProperty.AddOwner<TreeViewItemHeader>();
    
    internal static readonly StyledProperty<bool> IsShowLeafIconProperty =
        AvaloniaProperty.Register<TreeViewItemHeader, bool>(nameof(IsShowLeafIcon));
    
    internal static readonly DirectProperty<TreeViewItemHeader, int> LevelProperty =
        AvaloniaProperty.RegisterDirect<TreeViewItemHeader, int>(
            nameof(Level),
            o => o.Level,
            (o, v) => o.Level = v);
    
    internal bool IsHover
    {
        get => GetValue(IsHoverProperty);
        set => SetValue(IsHoverProperty, value);
    }
    
    internal IBrush? ContentFrameBackground
    {
        get => GetValue(ContentFrameBackgroundProperty);
        set => SetValue(ContentFrameBackgroundProperty, value);
    }
    
    private TreeItemHoverMode _nodeHoverMode;

    internal TreeItemHoverMode NodeHoverMode
    {
        get => _nodeHoverMode;
        set => SetAndRaise(NodeHoverModeProperty, ref _nodeHoverMode, value);
    }

    private bool _isShowLine;

    internal bool IsShowLine
    {
        get => _isShowLine;
        set => SetAndRaise(IsShowLineProperty, ref _isShowLine, value);
    }

    private bool _isShowIcon = true;

    internal bool IsShowIcon
    {
        get => _isShowIcon;
        set => SetAndRaise(IsShowIconProperty, ref _isShowIcon, value);
    }

    private bool _iconEffectiveVisible = true;

    internal bool IconEffectiveVisible
    {
        get => _iconEffectiveVisible;
        set => SetAndRaise(IconEffectiveVisibleProperty, ref _iconEffectiveVisible, value);
    }

    private bool _isDragging;

    internal bool IsDragging
    {
        get => _isDragging;
        set => SetAndRaise(IsDraggingProperty, ref _isDragging, value);
    }

    private bool _isDragOver;

    internal bool IsDragOver
    {
        get => _isDragOver;
        set => SetAndRaise(IsDragOverProperty, ref _isDragOver, value);
    }

    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    private NodeSwitcherButtonIconMode _switcherMode;

    internal NodeSwitcherButtonIconMode SwitcherMode
    {
        get => _switcherMode;
        set => SetAndRaise(SwitcherModeProperty, ref _switcherMode, value);
    }

    private bool _isSwitcherRotation;

    internal bool IsSwitcherRotation
    {
        get => _isSwitcherRotation;
        set => SetAndRaise(IsSwitcherRotationProperty, ref _isSwitcherRotation, value);
    }
    
    internal ItemToggleType ToggleType
    {
        get => GetValue(ToggleTypeProperty);
        set => SetValue(ToggleTypeProperty, value);
    }

    internal bool IsShowLeafIcon
    {
        get => GetValue(IsShowLeafIconProperty);
        set => SetValue(IsShowLeafIconProperty, value);
    }
    
    private int _level;

    internal int Level
    {
        get => _level;
        set => SetAndRaise(LevelProperty, ref _level, value);
    }
    
    #endregion
    
    private Border? _headerContentFrame;
    private NodeSwitcherButton? _nodeSwitcherButton;
    private IconPresenter? _iconPresenter;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsShowIconProperty || change.Property == IconProperty)
        {
            IconEffectiveVisible = IsShowIcon && Icon is not null;
        }
        else if (change.Property == IsLoadingProperty ||
                 change.Property == IsLeafProperty ||
                 change.Property == IsSwitcherRotationProperty)
        {
            SetupSwitcherButtonIconMode();
        }
        else if (change.Property == ToggleTypeProperty)
        {
            HandleToggleTypeChanged(change);
        }
        
        if (IsLoaded)
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions(true);
            }
        }
    }
    
    private void SetupSwitcherButtonIconMode()
    {
        if (!IsLeaf)
        {
            if (IsLoading)
            {
                SwitcherMode = NodeSwitcherButtonIconMode.Loading;
            }
            else
            {
                if (IsSwitcherRotation)
                {
                    SwitcherMode = NodeSwitcherButtonIconMode.Rotation;
                }
                else
                {
                    SwitcherMode = NodeSwitcherButtonIconMode.Default;
                }
            }
        }
        else
        {
            SwitcherMode = NodeSwitcherButtonIconMode.Leaf;
        }
    }
    
    private void HandleToggleTypeChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var newValue = e.GetNewValue<ItemToggleType>();
        PseudoClasses.Set(TreeViewPseudoClass.NodeToggleTypeRadio, newValue == ItemToggleType.Radio);
        PseudoClasses.Set(TreeViewPseudoClass.NodeToggleTypeCheckBox, newValue == ItemToggleType.CheckBox);
    }
    
    private void ConfigureTransitions(bool force)
    {
        if (IsMotionEnabled)
        {
            if (force || Transitions == null)
            {
                Transitions =
                [
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty),
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(ContentFrameBackgroundProperty)
                ];
            }
        }
        else
        {
            Transitions = null;
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _headerContentFrame = e.NameScope.Find<Border>(TreeViewItemHeaderThemeConstants.HeaderContentFramePart);
        if (_headerContentFrame != null)
        {
            _headerContentFrame.PointerEntered += HandleHeaderPresenterEntered;
            _headerContentFrame.PointerExited  += HandleHeaderPresenterExited;
        }

        _iconPresenter = e.NameScope.Find<IconPresenter>(TreeViewItemHeaderThemeConstants.IconPresenterPart);
        _nodeSwitcherButton = e.NameScope.Find<NodeSwitcherButton>(TreeViewItemHeaderThemeConstants.NodeSwitcherButtonPart);
        SetupSwitcherButtonIconMode();
    }

    internal Rect SwitcherButtonRect(Control relativeTo)
    {
        Debug.Assert(_nodeSwitcherButton != null);
        var point = _nodeSwitcherButton.TranslatePoint(new Point(0, 0), relativeTo) ?? default;
        return new Rect(point, _nodeSwitcherButton.Bounds.Size);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        ConfigureTransitions(false);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        Transitions = null;
    }
    
    private void HandleHeaderPresenterEntered(object? sender, PointerEventArgs? args)
    {
        if (NodeHoverMode == TreeItemHoverMode.WholeLine)
        {
            return;
        }

        SetCurrentValue(IsHoverProperty, true);
    }

    private void HandleHeaderPresenterExited(object? sender, PointerEventArgs args)
    {
        if (NodeHoverMode == TreeItemHoverMode.WholeLine)
        {
            return;
        }

        SetCurrentValue(IsHoverProperty, false);
    }

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);
        if (NodeHoverMode == TreeItemHoverMode.WholeLine)
        {
            SetCurrentValue(IsHoverProperty, true);
        }
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        if (NodeHoverMode == TreeItemHoverMode.WholeLine)
        {
            SetCurrentValue(IsHoverProperty, false);
        }
    }

    internal void NotifyAnimating(bool animating)
    {
        if (_nodeSwitcherButton != null)
        {
            _nodeSwitcherButton.IsNodeAnimating = animating;
        }
    }

    public Point GetDragIndicatorOffset(Control relativeTo)
    {
        var offsetX = 0d;
        var offsetY = 0d;
        if (Level != 0)
        {
            if (_iconPresenter is not null && _iconPresenter.IsVisible)
            {
                var offset = _iconPresenter.TranslatePoint(new Point(0, 0), this) ?? default;
                offsetX = offset.X;
                offsetY = offset.Y;
            }
            else if (_headerContentFrame is not null)
            {
                var offset = _headerContentFrame.TranslatePoint(new Point(0, 0), this) ?? default;
                offsetX = offset.X;
                offsetY = offset.Y;
            }
        }
        else
        {
            if (_headerContentFrame is not null)
            {
                var offset = _headerContentFrame.TranslatePoint(new Point(0, 0), this) ?? default;
                offsetX = offset.X;
                offsetY = offset.Y;
            }
        }
        
        if (_nodeSwitcherButton is not null && _nodeSwitcherButton.IsLeafIconVisible)
        {
            offsetX -= _nodeSwitcherButton.Bounds.Width;
        }

        return this.TranslatePoint(new Point(offsetX, offsetY), relativeTo) ?? default;
    }
}