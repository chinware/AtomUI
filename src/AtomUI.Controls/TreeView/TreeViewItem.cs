using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Media;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace AtomUI.Controls;

using AvaloniaTreeItem = Avalonia.Controls.TreeViewItem;

public class TreeViewItem : AvaloniaTreeItem
{
    public const string TreeNodeHoverPC = ":treenode-hover";

    #region 公共属性定义

    public static readonly StyledProperty<bool> IsCheckableProperty =
        AvaloniaProperty.Register<TreeViewItem, bool>(nameof(IsCheckable), true);

    public static readonly StyledProperty<Icon?> IconProperty
        = AvaloniaProperty.Register<TreeViewItem, Icon?>(nameof(Icon));

    public static readonly StyledProperty<bool?> IsCheckedProperty
        = AvaloniaProperty.Register<TreeViewItem, bool?>(nameof(IsChecked), false);

    public static readonly StyledProperty<Icon?> SwitcherExpandIconProperty
        = AvaloniaProperty.Register<TreeViewItem, Icon?>(nameof(SwitcherExpandIcon));

    public static readonly StyledProperty<Icon?> SwitcherCollapseIconProperty
        = AvaloniaProperty.Register<TreeViewItem, Icon?>(nameof(SwitcherCollapseIcon));
    
    public static readonly StyledProperty<Icon?> SwitcherRotationIconProperty
        = AvaloniaProperty.Register<TreeViewItem, Icon?>(nameof(SwitcherRotationIcon));
    
    public static readonly StyledProperty<Icon?> SwitcherLoadingIconProperty
        = AvaloniaProperty.Register<TreeViewItem, Icon?>(nameof(SwitcherRotationIcon));
    
    public static readonly StyledProperty<Icon?> SwitcherLeafIconProperty
        = AvaloniaProperty.Register<TreeViewItem, Icon?>(nameof(SwitcherLeafIcon));
    
    public static readonly DirectProperty<TreeViewItem, bool> IsLeafProperty
        = AvaloniaProperty.RegisterDirect<TreeViewItem, bool>(nameof(IsLeaf),
            o => o.IsLeaf,
            (o, v) => o.IsLeaf = v);
    
    public static readonly StyledProperty<bool> IsShowLeafIconProperty =
        AvaloniaProperty.Register<TreeViewItem, bool>(nameof(IsShowLeafIcon));
    
    public static readonly StyledProperty<bool> IsLoadingProperty
        = AvaloniaProperty.Register<TreeViewItem, bool>(nameof(IsLoading), false);

    public bool IsCheckable
    {
        get => GetValue(IsCheckableProperty);
        set => SetValue(IsCheckableProperty, value);
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
    
    internal bool IsShowLeafIcon
    {
        get => GetValue(IsShowLeafIconProperty);
        set => SetValue(IsShowLeafIconProperty, value);
    }

    public bool? IsChecked
    {
        get => GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
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

    public TreeNodeKey? Key { get; set; }

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<TreeViewItem, double> TitleHeightProperty
        = AvaloniaProperty.RegisterDirect<TreeViewItem, double>(nameof(TitleHeight),
            o => o.TitleHeight,
            (o, v) => o.TitleHeight = v);

    internal static readonly DirectProperty<TreeViewItem, TreeItemHoverMode> NodeHoverModeProperty =
        AvaloniaProperty.RegisterDirect<TreeViewItem, TreeItemHoverMode>(nameof(NodeHoverMode),
            o => o.NodeHoverMode,
            (o, v) => o.NodeHoverMode = v);

    internal static readonly DirectProperty<TreeViewItem, bool> IsShowLineProperty =
        AvaloniaProperty.RegisterDirect<TreeViewItem, bool>(nameof(IsShowLine),
            o => o.IsShowLine,
            (o, v) => o.IsShowLine = v);

    internal static readonly DirectProperty<TreeViewItem, bool> IsShowIconProperty =
        AvaloniaProperty.RegisterDirect<TreeViewItem, bool>(nameof(IsShowIcon),
            o => o.IsShowIcon,
            (o, v) => o.IsShowIcon = v);

    internal static readonly DirectProperty<TreeViewItem, bool> IconEffectiveVisibleProperty =
        AvaloniaProperty.RegisterDirect<TreeViewItem, bool>(nameof(IconEffectiveVisible),
            o => o.IconEffectiveVisible,
            (o, v) => o.IconEffectiveVisible = v);

    internal static readonly DirectProperty<TreeViewItem, bool> IsCheckboxVisibleProperty =
        AvaloniaProperty.RegisterDirect<TreeViewItem, bool>(nameof(IsCheckboxVisible),
            o => o.IsCheckboxVisible,
            (o, v) => o.IsCheckboxVisible = v);

    internal static readonly DirectProperty<TreeViewItem, bool> IsCheckboxEnableProperty =
        AvaloniaProperty.RegisterDirect<TreeViewItem, bool>(nameof(IsCheckboxEnable),
            o => o.IsCheckboxEnable,
            (o, v) => o.IsCheckboxEnable = v);

    internal static readonly DirectProperty<TreeViewItem, bool> IsDraggingProperty =
        AvaloniaProperty.RegisterDirect<TreeViewItem, bool>(nameof(IsDragging),
            o => o.IsDragging,
            (o, v) => o.IsDragging = v);

    internal static readonly DirectProperty<TreeViewItem, bool> IsDragOverProperty =
        AvaloniaProperty.RegisterDirect<TreeViewItem, bool>(nameof(IsDragOver),
            o => o.IsDragOver,
            (o, v) => o.IsDragOver = v);

    internal static readonly DirectProperty<TreeViewItem, Thickness> DragFrameBorderThicknessProperty =
        AvaloniaProperty.RegisterDirect<TreeViewItem, Thickness>(nameof(DragFrameBorderThickness),
            o => o.DragFrameBorderThickness,
            (o, v) => o.DragFrameBorderThickness = v);

    internal static readonly StyledProperty<IBrush?> EffectiveNodeBgProperty
        = AvaloniaProperty.Register<TreeViewItem, IBrush?>(nameof(EffectiveNodeBg));

    internal static readonly StyledProperty<CornerRadius> EffectiveNodeCornerRadiusProperty
        = AvaloniaProperty.Register<TreeViewItem, CornerRadius>(nameof(EffectiveNodeCornerRadius));
    
    internal static readonly DirectProperty<TreeViewItem, bool> IsMotionEnabledProperty
        = AvaloniaProperty.RegisterDirect<TreeViewItem, bool>(nameof(IsMotionEnabled),
            o => o.IsMotionEnabled,
            (o, v) => o.IsMotionEnabled = v);

    private double _titleHeight;

    internal double TitleHeight
    {
        get => _titleHeight;
        set => SetAndRaise(TitleHeightProperty, ref _titleHeight, value);
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

    private bool _isCheckboxVisible;

    internal bool IsCheckboxVisible
    {
        get => _isCheckboxVisible;
        set => SetAndRaise(IsCheckboxVisibleProperty, ref _isCheckboxVisible, value);
    }

    private bool _isCheckboxEnable;

    internal bool IsCheckboxEnable
    {
        get => _isCheckboxEnable;
        set => SetAndRaise(IsCheckboxEnableProperty, ref _isCheckboxEnable, value);
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

    private Thickness _dragFrameBorderThickness;

    internal Thickness DragFrameBorderThickness
    {
        get => _dragFrameBorderThickness;
        set => SetAndRaise(DragFrameBorderThicknessProperty, ref _dragFrameBorderThickness, value);
    }

    internal IBrush? EffectiveNodeBg
    {
        get => GetValue(EffectiveNodeBgProperty);
        set => SetValue(EffectiveNodeBgProperty, value);
    }

    internal CornerRadius EffectiveNodeCornerRadius
    {
        get => GetValue(EffectiveNodeCornerRadiusProperty);
        set => SetValue(EffectiveNodeCornerRadiusProperty, value);
    }
    
    private bool _isMotionEnabled = true;

    internal bool IsMotionEnabled
    {
        get => _isMotionEnabled;
        set => SetAndRaise(IsMotionEnabledProperty, ref _isMotionEnabled, value);
    }

    internal TreeView? OwnerTreeView { get; set; }

    #endregion

    private bool _initialized;
    private ContentPresenter? _headerPresenter;
    private Border? _frameDecorator;
    private ContentPresenter? _iconPresenter;
    private NodeSwitcherButton? _switcherButton;
    private Rect _effectiveBgRect;
    private readonly BorderRenderHelper _borderRenderHelper;

    static TreeViewItem()
    {
        AffectsRender<TreeViewItem>(EffectiveNodeCornerRadiusProperty,
            EffectiveNodeBgProperty,
            IsShowLineProperty,
            IsShowLeafIconProperty,
            IsDraggingProperty,
            IsDragOverProperty);
    }

    public TreeViewItem()
    {
        _borderRenderHelper = new BorderRenderHelper();
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        if (!_initialized)
        {
            TokenResourceBinder.CreateTokenBinding(this, TitleHeightProperty, TreeViewTokenKey.TitleHeight);
            if (IsChecked.HasValue && IsChecked.Value)
            {
                OwnerTreeView = this.FindLogicalAncestorOfType<TreeView>();
                // 注册到 TreeView
                OwnerTreeView?.DefaultCheckedItems.Add(this);
            }
            
            CreateNodeSwitcherDefaultIcons();
            SetNodeSwitcherIcons();
            SetupSwitcherButtonIconMode();
            _initialized = true;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (VisualRoot is not null)
        {
            if (change.Property == NodeHoverModeProperty)
            {
                CalculateEffectiveBgRect();
            }
            else if (change.Property == SwitcherExpandIconProperty ||
                     change.Property == SwitcherCollapseIconProperty ||
                     change.Property == SwitcherRotationIconProperty)
            {
                SetNodeSwitcherIcons();
                SetupSwitcherButtonIconMode();
            }
            else if (change.Property == IsEnabledProperty ||
                     change.Property == IsCheckableProperty)
            {
                SetupCheckBoxEnabled();
            }
            else if (change.Property == IsCheckedProperty)
            {
                // 我们处理某个节点的点击只有 true 或者 false
                if (IsChecked.HasValue)
                {
                    if (IsChecked.Value)
                    {
                        OwnerTreeView?.CheckedSubTree(this);
                    }
                    else
                    {
                        OwnerTreeView?.UnCheckedSubTree(this);
                    }
                }
            }      
            else if (change.Property == IsShowLineProperty)
            {
                CreateNodeSwitcherDefaultIcons();
                SetNodeSwitcherIcons();
            }
        }

        if (change.Property == IsShowIconProperty || change.Property == IconProperty)
        {
            IconEffectiveVisible = IsShowIcon && Icon is not null;
        }

        if (change.Property == ItemCountProperty)
        {
            IsLeaf = ItemCount == 0;
        }
        else if (change.Property == IconProperty)
        {
            if (change.OldValue is Icon oldIcon)
            {
                UIStructureUtils.SetTemplateParent(oldIcon, null);
            }

            if (change.NewValue is Icon newIcon)
            {
                UIStructureUtils.SetTemplateParent(newIcon, this);
            }
        } 
        else if (change.Property == IsLoadingProperty || change.Property == IsLeafProperty)
        {
            SetupSwitcherButtonIconMode();
        }
        else if (change.Property == IsMotionEnabledProperty)
        {
            SetupTransitions();
        }
    }

    private void SetupSwitcherButtonIconMode()
    {
        if (_switcherButton is not null)
        {
            if (!IsLeaf)
            {
                if (IsLoading)
                {
                    _switcherButton.IconMode = NodeSwitcherButtonIconMode.Loading;
                }
                else
                {
                    if (SwitcherExpandIcon != null && SwitcherCollapseIcon != null)
                    {
                        _switcherButton.IconMode = NodeSwitcherButtonIconMode.Default;
                    } 
                    else if (SwitcherRotationIcon != null)
                    {
                        _switcherButton.IconMode = NodeSwitcherButtonIconMode.Rotation;
                    }
                }
            }
            else
            {
                _switcherButton.IconMode = NodeSwitcherButtonIconMode.Leaf;
            }
        }
    }

    private void CreateNodeSwitcherDefaultIcons()
    {
        if (IsShowLine)
        {
            if (SwitcherExpandIcon == null)
            {
                SetValue(SwitcherExpandIconProperty, AntDesignIconPackage.PlusSquareOutlined(), BindingPriority.Template);
            }

            if (SwitcherCollapseIcon == null)
            {
                SetValue(SwitcherCollapseIconProperty, AntDesignIconPackage.MinusSquareOutlined(), BindingPriority.Template);
            }
        }
        else
        {
            if (SwitcherRotationIcon == null)
            {
                SetValue(SwitcherRotationIconProperty, AntDesignIconPackage.CaretRightOutlined(), BindingPriority.Template);
            }
        }
        
        if (SwitcherLoadingIcon == null)
        {
            var loadingIcon = AntDesignIconPackage.LoadingOutlined();
            loadingIcon.LoadingAnimation = IconAnimation.Spin;
            SetValue(SwitcherLoadingIconProperty, loadingIcon, BindingPriority.Template);
        }
        
        if (SwitcherLeafIcon == null)
        {
            SetValue(SwitcherLeafIconProperty, AntDesignIconPackage.FileOutlined(), BindingPriority.Template);
        }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var size = base.ArrangeOverride(finalSize);
        CalculateEffectiveBgRect();
        return size;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        TokenResourceBinder.CreateTokenBinding(this, DragFrameBorderThicknessProperty,
            SharedTokenKey.BorderThickness,
            BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this));
        
        base.OnApplyTemplate(e);
        SetNodeSwitcherIcons();
        _headerPresenter = e.NameScope.Find<ContentPresenter>(TreeViewItemTheme.HeaderPresenterPart);
        _iconPresenter   = e.NameScope.Find<ContentPresenter>(TreeViewItemTheme.IconPresenterPart);
        _frameDecorator  = e.NameScope.Find<Border>(TreeViewItemTheme.FrameDecoratorPart);
        _switcherButton  = e.NameScope.Find<NodeSwitcherButton>(TreeViewItemTheme.NodeSwitcherButtonPart);
        
        SetupSwitcherButtonIconMode();

        if (_frameDecorator is not null)
        {
            _frameDecorator.PointerEntered += HandleFrameDecoratorEntered;
            _frameDecorator.PointerExited  += HandleFrameDecoratorExited;
        }

        if (_headerPresenter is not null)
        {
            _headerPresenter.PointerEntered += HandleHeaderPresenterEntered;
            _headerPresenter.PointerExited  += HandleHeaderPresenterExited;
        }

        if (_iconPresenter is not null)
        {
            _iconPresenter.PointerEntered += HandleHeaderPresenterEntered;
            _iconPresenter.PointerExited  += HandleHeaderPresenterExited;
        }

        IsLeaf = ItemCount == 0;
        SetupCheckBoxEnabled();
        SetupTransitions();
    }

    private void SetupTransitions()
    {
        if (IsMotionEnabled)
        {
            Transitions ??= new Transitions
            {
                AnimationUtils.CreateTransition<SolidColorBrushTransition>(EffectiveNodeBgProperty)
            };
        }
        else
        {
            Transitions = null;
        }
    }

    private void CalculateEffectiveBgRect()
    {
        if (_frameDecorator is null)
        {
            return;
        }

        Point offset       = default;
        var   targetWidth  = 0d;
        var   targetHeight = 0d;
        if (NodeHoverMode == TreeItemHoverMode.Default || NodeHoverMode == TreeItemHoverMode.Block)
        {
            if (_iconPresenter is not null && _iconPresenter.IsVisible)
            {
                offset       = _iconPresenter.TranslatePoint(new Point(0, 0), this) ?? default;
                targetWidth  = _iconPresenter.Bounds.Width + _headerPresenter?.Bounds.Width ?? 0d;
                targetHeight = _frameDecorator.Bounds.Height;
            }
            else if (_headerPresenter is not null)
            {
                offset       = _headerPresenter.TranslatePoint(new Point(0, 0), this) ?? default;
                targetWidth  = _headerPresenter.Bounds.Width;
                targetHeight = _frameDecorator.Bounds.Height;
            }
        }

        _effectiveBgRect = new Rect(offset, new Size(targetWidth, targetHeight));
    }

    public override void Render(DrawingContext context)
    {
        {
            using var state = context.PushTransform(Matrix.CreateTranslation(_effectiveBgRect.X, 0));
            _borderRenderHelper.Render(context,
                _effectiveBgRect.Size,
                new Thickness(),
                EffectiveNodeCornerRadius,
                BackgroundSizing.InnerBorderEdge,
                EffectiveNodeBg,
                null,
                default);
        }
        if (IsShowLine && (IsExpanded || IsLeaf))
        {
            RenderTreeNodeLine(context);
        }
    }

    private void RenderTreeNodeLine(DrawingContext context)
    {
        if (_switcherButton is null)
        {
            return;
        }

        var penWidth = BorderUtils.BuildRenderScaleAwareThickness(BorderThickness, VisualRoot?.RenderScaling ?? 1.0)
                                  .Top;
        using var state = context.PushRenderOptions(new RenderOptions
        {
            EdgeMode = EdgeMode.Aliased
        });
        
        var isLastChild = false;
        if (Parent is ItemsControl parentTreeItem)
        {
            if (parentTreeItem.ContainerFromIndex(parentTreeItem.ItemCount - 1) == this)
            {
                isLastChild = true;
            }
        }

        if (!IsLeaf && !isLastChild)
        {
            var switcherMiddleBottom =
                _switcherButton.TranslatePoint(
                    new Point(_switcherButton.DesiredSize.Width / 2, _switcherButton.DesiredSize.Height), this) ??
                default;
            var blockStartPoint = new Point(switcherMiddleBottom.X, switcherMiddleBottom.Y);
            var blockEndPoint   = new Point(blockStartPoint.X, DesiredSize.Height);

            context.DrawLine(new Pen(BorderBrush, penWidth), blockStartPoint, blockEndPoint);
        }

        // 画孩子线条
        if (!IsShowLeafIcon && IsLeaf)
        {
            {
                // 纵向
                var childStartPoint =
                    _switcherButton.TranslatePoint(new Point(_switcherButton.DesiredSize.Width / 2, 0), this) ??
                    default;
                var childEndPoint =
                    _switcherButton.TranslatePoint(
                        new Point(_switcherButton.DesiredSize.Width / 2,
                            isLastChild ? _switcherButton.DesiredSize.Height : DesiredSize.Height), this) ?? default;

                if (isLastChild)
                {
                    childEndPoint = childEndPoint.WithY(childEndPoint.Y / 2);
                }

                context.DrawLine(new Pen(BorderBrush, penWidth), childStartPoint, childEndPoint);
            }

            {
                // 横向
                var childStartPoint =
                    _switcherButton.TranslatePoint(
                        new Point(_switcherButton.DesiredSize.Width / 2, _switcherButton.DesiredSize.Height / 2),
                        this) ??
                    default;
                var childEndPoint =
                    _switcherButton.TranslatePoint(
                        new Point(_switcherButton.DesiredSize.Width, _switcherButton.DesiredSize.Height / 2),
                        this) ?? default;

                context.DrawLine(new Pen(BorderBrush, penWidth), childStartPoint, childEndPoint);
            }
        }
    }

    private void SetNodeSwitcherIcons()
    {
        if (SwitcherExpandIcon is not null)
        {
            TokenResourceBinder.CreateTokenBinding(SwitcherExpandIcon, WidthProperty,
                SharedTokenKey.IconSize);
            TokenResourceBinder.CreateTokenBinding(SwitcherExpandIcon, HeightProperty,
                SharedTokenKey.IconSize);
        }

        if (SwitcherCollapseIcon is not null)
        {
            TokenResourceBinder.CreateTokenBinding(SwitcherCollapseIcon, WidthProperty,
                SharedTokenKey.IconSize);
            TokenResourceBinder.CreateTokenBinding(SwitcherCollapseIcon, HeightProperty,
                SharedTokenKey.IconSize);
        }

        if (SwitcherRotationIcon is not null)
        {
            TokenResourceBinder.CreateTokenBinding(SwitcherRotationIcon, WidthProperty,
                SharedTokenKey.IconSizeXS);
            TokenResourceBinder.CreateTokenBinding(SwitcherRotationIcon, HeightProperty,
                SharedTokenKey.IconSizeXS);
        }

        if (SwitcherLoadingIcon is not null)
        {
            TokenResourceBinder.CreateTokenBinding(SwitcherLoadingIcon, WidthProperty,
                SharedTokenKey.IconSize);
            TokenResourceBinder.CreateTokenBinding(SwitcherLoadingIcon, HeightProperty,
                SharedTokenKey.IconSize);
        }

        if (SwitcherLeafIcon is not null)
        {
            TokenResourceBinder.CreateTokenBinding(SwitcherLeafIcon, WidthProperty,
                SharedTokenKey.IconSize);
            TokenResourceBinder.CreateTokenBinding(SwitcherLeafIcon, HeightProperty,
                SharedTokenKey.IconSize);
        }
    }

    private void SetupCheckBoxEnabled()
    {
        if (!IsEnabled)
        {
            IsCheckboxEnable = false;
        }
        else
        {
            IsCheckboxEnable = IsCheckable;
        }
    }

    private void HandleFrameDecoratorEntered(object? sender, PointerEventArgs? args)
    {
        if (NodeHoverMode != TreeItemHoverMode.WholeLine)
        {
            return;
        }

        PseudoClasses.Set(TreeNodeHoverPC, true);
    }

    private void HandleFrameDecoratorExited(object? sender, PointerEventArgs args)
    {
        if (NodeHoverMode != TreeItemHoverMode.WholeLine)
        {
            return;
        }

        PseudoClasses.Set(TreeNodeHoverPC, false);
    }

    private void HandleHeaderPresenterEntered(object? sender, PointerEventArgs? args)
    {
        if (NodeHoverMode == TreeItemHoverMode.WholeLine)
        {
            return;
        }

        PseudoClasses.Set(TreeNodeHoverPC, true);
    }

    private void HandleHeaderPresenterExited(object? sender, PointerEventArgs args)
    {
        if (NodeHoverMode == TreeItemHoverMode.WholeLine)
        {
            return;
        }

        PseudoClasses.Set(TreeNodeHoverPC, false);
    }

    internal Rect GetDragBounds(bool includeChildren = false)
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
            else if (_headerPresenter is not null)
            {
                var offset = _headerPresenter.TranslatePoint(new Point(0, 0), this) ?? default;
                offsetX = offset.X;
                offsetY = offset.Y;
            }
        }
        else
        {
            if (_headerPresenter is not null)
            {
                var offset = _headerPresenter.TranslatePoint(new Point(0, 0), this) ?? default;
                offsetX = offset.X;
                offsetY = offset.Y;
            }
        }

        if (_switcherButton is not null && _switcherButton.IsLeafIconVisible)
        {
            offsetX -= _switcherButton.Bounds.Width;
        }

        return new Rect(new Point(offsetX, offsetY),
            new Size(Bounds.Width - offsetX,
                includeChildren ? Bounds.Height : _headerPresenter?.Bounds.Height ?? default));
    }

    internal Thickness FrameDecoratorMargin()
    {
        return _frameDecorator?.Margin ?? default;
    }

    internal bool IsInDragBounds(Point point)
    {
        return GetDragBounds(true).Contains(point);
    }

    internal bool IsInDragHeaderBounds(Point point)
    {
        return GetDragBounds().Contains(point);
    }

    internal bool IsDragOverForOffsetY(Point point)
    {
        if (OwnerTreeView is null)
        {
            return false;
        }

        return new Rect(Bounds.Size).Contains(point);
    }

    internal DragPreviewAdorner BuildPreviewAdorner()
    {
        return new DragPreviewAdorner(_frameDecorator!);
    }
}