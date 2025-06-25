using System.Reactive.Disposables;
using AtomUI.Animations;
using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.MotionScene;
using AtomUI.Reflection;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

using AvaloniaTreeItem = Avalonia.Controls.TreeViewItem;

public class TreeViewItem : AvaloniaTreeItem,
                            IResourceBindingManager
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

    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<TreeViewItem, bool>(nameof(IsLoading), false);

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

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty
        = MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<TreeViewItem>();

    internal static readonly DirectProperty<TreeViewItem, NodeSwitcherButtonIconMode> SwitcherModeProperty =
        AvaloniaProperty.RegisterDirect<TreeViewItem, NodeSwitcherButtonIconMode>(
            nameof(SwitcherMode),
            o => o.SwitcherMode,
            (o, v) => o.SwitcherMode = v);

    internal static readonly DirectProperty<TreeViewItem, bool> IsSwitcherRotationProperty =
        AvaloniaProperty.RegisterDirect<TreeViewItem, bool>(
            nameof(IsSwitcherRotation),
            o => o.IsSwitcherRotation,
            (o, v) => o.IsSwitcherRotation = v);

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

    internal TreeView? OwnerTreeView { get; set; }

    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable => _resourceBindingsDisposable;
    private bool _tempAnimationDisabled = false;

    #endregion

    private CompositeDisposable? _resourceBindingsDisposable;
    private bool _animating;
    private ContentPresenter? _headerPresenter;
    private MotionActorControl? _itemsPresenterMotionActor;
    private Border? _frame;
    private IconPresenter? _iconPresenter;
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
        _borderRenderHelper         = new BorderRenderHelper();
        _resourceBindingsDisposable = new CompositeDisposable();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, DragFrameBorderThicknessProperty,
            SharedTokenKey.BorderThickness,
            BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this)));
        OwnerTreeView = this.GetLogicalAncestors().OfType<TreeView>().FirstOrDefault<TreeView>();
        if (IsChecked.HasValue && IsChecked.Value)
        {
            // 注册到 TreeView
            OwnerTreeView?.DefaultCheckedItems.Add(this);
        }

        SetupSwitcherButtonIconMode();
        SetupCheckBoxEnabled();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        this.DisposeTokenBindings();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

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
                oldIcon.SetTemplatedParent(null);
            }

            if (change.NewValue is Icon newIcon)
            {
                newIcon.SetTemplatedParent(this);
            }
        }
        else if (change.Property == IsLoadingProperty ||
                 change.Property == IsLeafProperty ||
                 change.Property == IsSwitcherRotationProperty)
        {
            SetupSwitcherButtonIconMode();
        }

        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions();
            }
            else if (change.Property == NodeHoverModeProperty)
            {
                CalculateEffectiveBgRect();
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

                if (IsChecked != true)
                {
                    OwnerTreeView?.DefaultCheckedItems.Remove(this);
                }
            }
            else if (change.Property == IsExpandedProperty)
            {
                HandleExpandedChanged();
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

    private void HandleExpandedChanged()
    {
        if (IsExpanded)
        {
            ExpandChildren();
        }
        else
        {
            CollapseChildren();
        }
    }

    private void ExpandChildren()
    {
        if (_itemsPresenterMotionActor is null ||
            _animating ||
            OwnerTreeView is null)
        {
            return;
        }

        if (!IsMotionEnabled || OwnerTreeView.IsExpandAllProcess || _tempAnimationDisabled)
        {
            _itemsPresenterMotionActor.Opacity   = 1.0;
            _itemsPresenterMotionActor.IsVisible = true;
            return;
        }

        _animating = true;
        if (_switcherButton != null)
        {
            _switcherButton.IsNodeAnimating = true;
        }

        var motion = OwnerTreeView.OpenMotion ?? new ExpandMotion(Direction.Top, null, new CubicEaseOut());
        motion.Duration = OwnerTreeView.MotionDuration;

        MotionInvoker.Invoke(_itemsPresenterMotionActor, motion, () => { _itemsPresenterMotionActor.IsVisible = true; },
            () =>
            {
                _animating = false;
                if (_switcherButton != null)
                {
                    _switcherButton.IsNodeAnimating = false;
                }
            });
    }

    private void CollapseChildren()
    {
        if (_itemsPresenterMotionActor is null ||
            _animating ||
            OwnerTreeView is null)
        {
            return;
        }

        if (!IsMotionEnabled || OwnerTreeView.IsExpandAllProcess || _tempAnimationDisabled)
        {
            _itemsPresenterMotionActor.Opacity   = 0.0;
            _itemsPresenterMotionActor.IsVisible = false;
            return;
        }

        _animating = true;
        if (_switcherButton != null)
        {
            _switcherButton.IsNodeAnimating = true;
        }

        var motion = OwnerTreeView.OpenMotion ?? new ExpandMotion(Direction.Top, null, new CubicEaseIn());
        motion.Duration = OwnerTreeView.MotionDuration;

        MotionInvoker.Invoke(_itemsPresenterMotionActor, motion, null, () =>
        {
            _itemsPresenterMotionActor.IsVisible = false;
            _animating                           = false;
            if (_switcherButton != null)
            {
                _switcherButton.IsNodeAnimating = false;
            }
        });
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var size = base.ArrangeOverride(finalSize);
        CalculateEffectiveBgRect();
        return size;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _headerPresenter = e.NameScope.Find<ContentPresenter>(TreeViewItemThemeConstants.HeaderPresenterPart);
        _iconPresenter   = e.NameScope.Find<IconPresenter>(TreeViewItemThemeConstants.IconPresenterPart);
        _frame           = e.NameScope.Find<Border>(TreeViewItemThemeConstants.FramePart);
        _switcherButton  = e.NameScope.Find<NodeSwitcherButton>(TreeViewItemThemeConstants.NodeSwitcherButtonPart);
        _itemsPresenterMotionActor =
            e.NameScope.Find<MotionActorControl>(TreeViewItemThemeConstants.ItemsPresenterMotionActorPart);
        ConfigureTransitions();
        if (_frame is not null)
        {
            _frame.PointerEntered += HandleFrameEntered;
            _frame.PointerExited  += HandleFrameExited;
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

        IsLeaf                 = ItemCount == 0;
        _tempAnimationDisabled = true;
        HandleExpandedChanged();
        _tempAnimationDisabled = false;
    }

    private void ConfigureTransitions()
    {
        if (IsMotionEnabled)
        {
            Transitions = new Transitions
            {
                TransitionUtils.CreateTransition<SolidColorBrushTransition>(EffectiveNodeBgProperty)
            };
        }
        else
        {
            Transitions = null;
        }
    }

    private void CalculateEffectiveBgRect()
    {
        if (_frame is null)
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
                targetWidth  = _iconPresenter.DesiredSize.Width + _headerPresenter?.DesiredSize.Width ?? 0d;
                targetHeight = _frame.Bounds.Height;
            }
            else if (_headerPresenter is not null)
            {
                offset       = _headerPresenter.TranslatePoint(new Point(0, 0), this) ?? default;
                targetWidth  = _headerPresenter.DesiredSize.Width;
                targetHeight = _frame.Bounds.Height;
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

    private void HandleFrameEntered(object? sender, PointerEventArgs? args)
    {
        if (NodeHoverMode != TreeItemHoverMode.WholeLine)
        {
            return;
        }

        PseudoClasses.Set(TreeNodeHoverPC, true);
    }

    private void HandleFrameExited(object? sender, PointerEventArgs args)
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

    internal Thickness FrameMargin()
    {
        return _frame?.Margin ?? default;
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
        return new DragPreviewAdorner(_frame!);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        var bounds = new Rect(new Point(0, 0), _headerPresenter?.DesiredSize ?? default);
        var point  = e.GetPosition(_headerPresenter);
        if (bounds.Contains(point))
        {
            NotifyHeaderClick();
        }
    }

    protected virtual void NotifyHeaderClick()
    {
    }
}