using AtomUI.Controls.Themes;
using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.MotionScene;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

using AvaloniaTreeItem = Avalonia.Controls.TreeViewItem;

[PseudoClasses(TreeViewPseudoClass.NodeToggleTypeCheckBox, TreeViewPseudoClass.NodeToggleTypeRadio)]
public class TreeViewItem : AvaloniaTreeItem, IRadioButton, ITreeViewItemData
{
    #region 公共属性定义
    public static readonly StyledProperty<Icon?> IconProperty =
        AvaloniaProperty.Register<TreeViewItem, Icon?>(nameof(Icon));

    public static readonly StyledProperty<bool?> IsCheckedProperty =
        AvaloniaProperty.Register<TreeViewItem, bool?>(nameof(IsChecked), false);

    public static readonly StyledProperty<Icon?> SwitcherExpandIconProperty =
        AvaloniaProperty.Register<TreeViewItem, Icon?>(nameof(SwitcherExpandIcon));

    public static readonly StyledProperty<Icon?> SwitcherCollapseIconProperty =
        AvaloniaProperty.Register<TreeViewItem, Icon?>(nameof(SwitcherCollapseIcon));

    public static readonly StyledProperty<Icon?> SwitcherRotationIconProperty =
        AvaloniaProperty.Register<TreeViewItem, Icon?>(nameof(SwitcherRotationIcon));

    public static readonly StyledProperty<Icon?> SwitcherLoadingIconProperty =
        AvaloniaProperty.Register<TreeViewItem, Icon?>(nameof(SwitcherRotationIcon));

    public static readonly StyledProperty<Icon?> SwitcherLeafIconProperty =
        AvaloniaProperty.Register<TreeViewItem, Icon?>(nameof(SwitcherLeafIcon));

    public static readonly DirectProperty<TreeViewItem, bool> IsLeafProperty =
        AvaloniaProperty.RegisterDirect<TreeViewItem, bool>(nameof(IsLeaf),
            o => o.IsLeaf);

    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<TreeViewItem, bool>(nameof(IsLoading), false);
    
    public static readonly StyledProperty<string?> GroupNameProperty =
        RadioButton.GroupNameProperty.AddOwner<TreeViewItem>();

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
    
    IList<ITreeViewItemData> ITreeNode<ITreeViewItemData>.Children => Items.OfType<ITreeViewItemData>().ToList();
    public TreeNodeKey? ItemKey { get; set; }

    #endregion

    #region 公共事件定义

    public static readonly RoutedEvent<RoutedEventArgs> ClickEvent =
        RoutedEvent.Register<MenuItem, RoutedEventArgs>(
            nameof(Click),
            RoutingStrategies.Bubble);
    
    public event EventHandler<RoutedEventArgs>? Click
    {
        add => AddHandler(ClickEvent, value);
        remove => RemoveHandler(ClickEvent, value);
    }
    #endregion

    #region 内部属性定义

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

    internal static readonly DirectProperty<TreeViewItem, bool> IsDraggingProperty =
        AvaloniaProperty.RegisterDirect<TreeViewItem, bool>(nameof(IsDragging),
            o => o.IsDragging,
            (o, v) => o.IsDragging = v);

    internal static readonly DirectProperty<TreeViewItem, bool> IsDragOverProperty =
        AvaloniaProperty.RegisterDirect<TreeViewItem, bool>(nameof(IsDragOver),
            o => o.IsDragOver,
            (o, v) => o.IsDragOver = v);

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<TreeViewItem>();

    internal static readonly DirectProperty<TreeViewItem, bool> IsSwitcherRotationProperty =
        AvaloniaProperty.RegisterDirect<TreeViewItem, bool>(
            nameof(IsSwitcherRotation),
            o => o.IsSwitcherRotation,
            (o, v) => o.IsSwitcherRotation = v);
    
    internal static readonly StyledProperty<ItemToggleType> ToggleTypeProperty =
        TreeView.ToggleTypeProperty.AddOwner<TreeViewItem>();
    
    internal static readonly StyledProperty<bool> IsShowLeafIconProperty =
        AvaloniaProperty.Register<TreeViewItem, bool>(nameof(IsShowLeafIcon));
    
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

    private bool _isSwitcherRotation;

    internal bool IsSwitcherRotation
    {
        get => _isSwitcherRotation;
        set => SetAndRaise(IsSwitcherRotationProperty, ref _isSwitcherRotation, value);
    }
    
    public ItemToggleType ToggleType
    {
        get => GetValue(ToggleTypeProperty);
        set => SetValue(ToggleTypeProperty, value);
    }

    internal bool IsShowLeafIcon
    {
        get => GetValue(IsShowLeafIconProperty);
        set => SetValue(IsShowLeafIconProperty, value);
    }
    
    internal TreeView? OwnerTreeView { get; set; }

    private ITreeViewInteractionHandler? TreeViewInteractionHandler => this.FindLogicalAncestorOfType<TreeView>()?.InteractionHandler;
    
    #endregion
    
    private bool _animating;
    private BaseMotionActor? _itemsPresenterMotionActor;
    private readonly BorderRenderHelper _borderRenderHelper;
    private TreeViewItemHeader? _header;
    private IDisposable? _borderThicknessDisposable;

    static TreeViewItem()
    {
        AffectsRender<TreeViewItem>(
            IsShowLineProperty,
            IsShowLeafIconProperty,
            IsDraggingProperty,
            IsDragOverProperty,
            BorderBrushProperty,
            BorderThicknessProperty,
            NodeHoverModeProperty,
            BackgroundProperty);
    }

    public TreeViewItem()
    {
        _borderRenderHelper = new BorderRenderHelper();
    }
    
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _borderThicknessDisposable = TokenResourceBinder.CreateTokenBinding(this, BorderThicknessProperty,
            SharedTokenKey.BorderThickness,
            BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this));
        OwnerTreeView = this.GetLogicalAncestors().OfType<TreeView>().FirstOrDefault();
    }
    
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _borderThicknessDisposable?.Dispose();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        
        if (change.Property == ItemCountProperty)
        {
            IsLeaf = ItemCount == 0;
        }
 
        else if (change.Property == GroupNameProperty)
        {
            HandleGroupNameChanged(change);
        }
        else if (change.Property == IsCheckedProperty)
        {
            HandleIsCheckedChanged(change);
        }
        else if (change.Property == ToggleTypeProperty)
        {
            HandleToggleTypeChanged(change);
        }

        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == IsExpandedProperty)
            {
                HandleExpandedChanged();
            }
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

    private void HandleGroupNameChanged(AvaloniaPropertyChangedEventArgs e)
    {
        (TreeViewInteractionHandler as DefaultTreeViewInteractionHandler)?.OnGroupOrTypeChanged(this, e.GetOldValue<string>());
    }
    
    private void HandleToggleTypeChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var newValue = e.GetNewValue<ItemToggleType>();
        PseudoClasses.Set(TreeViewPseudoClass.NodeToggleTypeRadio, newValue == ItemToggleType.Radio);
        PseudoClasses.Set(TreeViewPseudoClass.NodeToggleTypeCheckBox, newValue == ItemToggleType.CheckBox);

        (TreeViewInteractionHandler as DefaultTreeViewInteractionHandler)?.OnGroupOrTypeChanged(this, GroupName);
    }
    
    private void HandleIsCheckedChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var newValue = e.GetNewValue<bool?>();
        PseudoClasses.Set(StdPseudoClass.Checked, newValue == true);

        (TreeViewInteractionHandler as DefaultTreeViewInteractionHandler)?.OnCheckedChanged(this);
    }

    private void ExpandChildren()
    {
        if (_itemsPresenterMotionActor is null ||
            _animating ||
            OwnerTreeView is null)
        {
            return;
        }

        if (!IsMotionEnabled || OwnerTreeView.IsExpandAllProcess)
        {
            _itemsPresenterMotionActor.Opacity   = 1.0;
            _itemsPresenterMotionActor.IsVisible = true;
            return;
        }

        _animating = true;
        _header?.NotifyAnimating(true);

        var motion = OwnerTreeView.OpenMotion ?? new ExpandMotion(Direction.Top, null, new CubicEaseOut());
        motion.Duration = OwnerTreeView.MotionDuration;

        motion.Run(_itemsPresenterMotionActor, () => { _itemsPresenterMotionActor.IsVisible = true; },
            () =>
            {
                _animating = false;
                _header?.NotifyAnimating(false);
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

        if (!IsMotionEnabled || OwnerTreeView.IsExpandAllProcess)
        {
            _itemsPresenterMotionActor.Opacity   = 0.0;
            _itemsPresenterMotionActor.IsVisible = false;
            return;
        }

        _animating = true;
        _header?.NotifyAnimating(true);

        var motion = OwnerTreeView.CloseMotion ?? new CollapseMotion(Direction.Top, null, new CubicEaseIn());
        motion.Duration = OwnerTreeView.MotionDuration;

        motion.Run(_itemsPresenterMotionActor, null, () =>
        {
            _itemsPresenterMotionActor.IsVisible = false;
            _animating                           = false;
            _header?.NotifyAnimating(false);
        });
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _header             = e.NameScope.Find<TreeViewItemHeader>(TreeViewItemThemeConstants.HeaderPart);
        _itemsPresenterMotionActor =
            e.NameScope.Find<BaseMotionActor>(TreeViewItemThemeConstants.ItemsPresenterMotionActorPart);

        IsLeaf = ItemCount == 0;
        
        var originIsMotionEnabled = IsMotionEnabled;
        try
        {
            SetCurrentValue(IsMotionEnabledProperty, false);
            HandleExpandedChanged();
        }
        finally
        {
            SetCurrentValue(IsMotionEnabledProperty, originIsMotionEnabled);
        }
        
    }
    
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (OwnerTreeView is not null)
        {
            if (IsChecked != false)
            {
                if (IsEnabled)
                {
                    SetCurrentValue(IsCheckedProperty, true);
                }
                else
                {
                    OwnerTreeView.CheckedSubTree(this);
                }
            }
            else
            {
                var allChecked = false;
                var hasAnyChecked = false;
             
                if (Items.Count > 0)
                {
                    allChecked = Items.All(item => OwnerTreeView.CheckedItems.Contains(item));
                    hasAnyChecked = Items.Any(item => OwnerTreeView.CheckedItems.Contains(item));
                }
                if (allChecked)
                {
                    SetCurrentValue(IsCheckedProperty, true);
                }
                else if (hasAnyChecked)
                {
                    SetCurrentValue(IsCheckedProperty, null);
                }
            }
        }
    }

    public override void Render(DrawingContext context)
    {
        if (NodeHoverMode == TreeItemHoverMode.WholeLine && _header != null)
        {
            using var state = context.PushTransform(Matrix.CreateTranslation(_header.Bounds.X, 0));
            _borderRenderHelper.Render(context,
                _header.Bounds.Size,
                new Thickness(),
                CornerRadius,
                BackgroundSizing.InnerBorderEdge,
                Background,
                null);
        }
   
        if (IsShowLine)
        {
            RenderTreeNodeLine(context);
        }
    }

    private void RenderTreeNodeLine(DrawingContext context)
    {
        if (_header is null)
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

        var switcherButtonRect = _header.SwitcherButtonRect(this);
        
        if (!IsLeaf && !isLastChild && _itemsPresenterMotionActor?.IsVisible == true)
        {
            var switcherMiddleBottom = new Point(switcherButtonRect.X + switcherButtonRect.Width / 2, switcherButtonRect.Bottom);
                                   var blockStartPoint = new Point(switcherMiddleBottom.X, switcherMiddleBottom.Y);
            var blockEndPoint   = new Point(blockStartPoint.X, DesiredSize.Height);
            context.DrawLine(new Pen(BorderBrush, penWidth), blockStartPoint, blockEndPoint);
        }

        // 画孩子线条
        if (!IsShowLeafIcon && IsLeaf)
        {
            {
                // 纵向
                var childStartPoint = new Point(switcherButtonRect.X + switcherButtonRect.Width / 2, switcherButtonRect.Top);
                var childEndPoint = new Point(switcherButtonRect.X + switcherButtonRect.Width / 2, isLastChild ? switcherButtonRect.Bottom : switcherButtonRect.Top + DesiredSize.Height);
                if (isLastChild)
                {
                    childEndPoint = childEndPoint.WithY(childEndPoint.Y / 2);
                }

                context.DrawLine(new Pen(BorderBrush, penWidth), childStartPoint.WithY(0), childEndPoint);
            }

            {
                // 横向
                var childStartPoint =  new Point(switcherButtonRect.X + switcherButtonRect.Width / 2 - penWidth / 2, switcherButtonRect.Top +  switcherButtonRect.Height / 2 - penWidth / 2);
                var childEndPoint = new Point(switcherButtonRect.Right, switcherButtonRect.Top +  switcherButtonRect.Height / 2 - penWidth / 2);

                context.DrawLine(new Pen(BorderBrush, penWidth), childStartPoint, childEndPoint);
            }
        }
    }

    internal Rect GetDragBounds(bool includeChildren = false)
    {
        var dragOffset = _header?.GetDragIndicatorOffset(this) ?? default;
        var offsetX    = dragOffset.X;
        return new Rect(new Point(offsetX, 0),
            new Size(Bounds.Width - offsetX,
                includeChildren ? Bounds.Height : _header?.Bounds.Height ?? default));
    }

    internal Thickness FrameMargin()
    {
        return _header?.Margin ?? default;
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
        return new DragPreviewAdorner(_header!);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (PointInHeaderBounds(e))
        {
            NotifyHeaderClick();
        }
    }
    
    internal bool PointInHeaderBounds(PointerReleasedEventArgs e)
    {
        var bounds = new Rect(new Point(0, 0), _header?.DesiredSize ?? default);
        var point  = e.GetPosition(_header);
        return bounds.Contains(point);
    }
    
    protected virtual void NotifyHeaderClick()
    {
    }
    
    internal void RaiseClick() => RaiseEvent(new RoutedEventArgs(ClickEvent));
}