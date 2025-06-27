using AtomUI.Controls.Themes;
using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AtomUI.Controls;

public class TreeViewFlyoutPresenter : FloatableTreeView,
                                       IShadowMaskInfoProvider,
                                       IMotionAwareControl,
                                       IControlSharedTokenResourcesHost
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsShowArrowProperty =
        ArrowDecoratedBox.IsShowArrowProperty.AddOwner<TreeViewFlyoutPresenter>();

    public static readonly StyledProperty<ArrowPosition> ArrowPositionProperty =
        ArrowDecoratedBox.ArrowPositionProperty.AddOwner<TreeViewFlyoutPresenter>();

    /// <summary>
    /// 是否显示指示箭头
    /// </summary>
    public bool IsShowArrow
    {
        get => GetValue(IsShowArrowProperty);
        set => SetValue(IsShowArrowProperty, value);
    }

    /// <summary>
    /// 箭头渲染的位置
    /// </summary>
    public ArrowPosition ArrowPosition
    {
        get => GetValue(ArrowPositionProperty);
        set => SetValue(ArrowPositionProperty, value);
    }
    
    #endregion

    #region 公共事件定义

    public static readonly RoutedEvent<FlyoutTreeViewItemClickedEventArgs> TreeViewItemClickedEvent =
        RoutedEvent.Register<TreeViewFlyoutPresenter, FlyoutTreeViewItemClickedEventArgs>(
            nameof(TreeViewItemClicked),
            RoutingStrategies.Bubble);
    
    public event EventHandler<FlyoutTreeViewItemClickedEventArgs>? TreeViewItemClicked
    {
        add => AddHandler(TreeViewItemClickedEvent, value);
        remove => RemoveHandler(TreeViewItemClickedEvent, value);
    }

    #endregion
        
    #region 内部属性定义

    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => MenuToken.ID;
    
    private ArrowDecoratedBox? _arrowDecoratedBox;

    #endregion
    
    public TreeViewFlyoutPresenter()
        : base(new DefaultTreeViewInteractionHandler(true))
    {
        this.RegisterResources();
        this.BindMotionProperties();
    }

    public TreeViewFlyoutPresenter(ITreeViewInteractionHandler menuInteractionHandler)
        : base(menuInteractionHandler)
    {
        this.RegisterResources();
        this.BindMotionProperties();
    }
    
    protected override void ContainerForItemPreparedOverride(Control container, object? item, int index)
    {
        base.ContainerForItemPreparedOverride(container, item, index);
        if (container is MenuItem menuItem)
        {
            BindTreeViewItemClickedRecursive(menuItem);
        }
    }
    
    protected override void ClearContainerForItemOverride(Control container)
    {
        base.ClearContainerForItemOverride(container);
        if (container is MenuItem menuItem)
        {
            ClearMenuItemClickedRecursive(menuItem);
        }
    }
    
    private void BindTreeViewItemClickedRecursive(MenuItem menuItem)
    {
        foreach (var childItem in menuItem.Items)
        {
            if (childItem is MenuItem childMenuItem)
            {
                BindTreeViewItemClickedRecursive(childMenuItem);
            }
        }

        // 绑定自己
        menuItem.Click += HandleTreeViewItemClicked;
    }

    private void ClearMenuItemClickedRecursive(MenuItem menuItem)
    {
        foreach (var childItem in menuItem.Items)
        {
            if (childItem is MenuItem childMenuItem)
            {
                ClearMenuItemClickedRecursive(childMenuItem);
            }
        }

        // 绑定自己
        menuItem.Click -= HandleTreeViewItemClicked;
    }

    private void HandleTreeViewItemClicked(object? sender, RoutedEventArgs args)
    {
        if (sender is TreeViewItem treeViewItem)
        {
            var ev = new FlyoutTreeViewItemClickedEventArgs(TreeViewItemClickedEvent, treeViewItem);
            RaiseEvent(ev);
        }
    }
    
    public CornerRadius GetMaskCornerRadius()
    {
        if (_arrowDecoratedBox is not null)
        {
            return _arrowDecoratedBox.CornerRadius;
        }

        return new CornerRadius(0);
    }
    
    public Rect GetMaskBounds()
    {
        if (_arrowDecoratedBox is not null)
        {
            var contentRect = _arrowDecoratedBox.GetMaskBounds();
            var adjustedPos = _arrowDecoratedBox.TranslatePoint(contentRect.Position, this) ?? default;
            return new Rect(adjustedPos, contentRect.Size);
        }

        return Bounds;
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _arrowDecoratedBox = e.NameScope.Find<ArrowDecoratedBox>(TreeViewFlyoutThemeConstants.ArrowDecoratorPart);
    } 
    
    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        if (container is MenuItem menuItem)
        {
            BindUtils.RelayBind(this, IsMotionEnabledProperty, menuItem, MenuItem.IsMotionEnabledProperty);
        }

        base.PrepareContainerForItemOverride(container, item, index);
    }
}

public class FlyoutTreeViewItemClickedEventArgs : RoutedEventArgs
{
    /// <summary>
    /// 当前鼠标点击的树节点
    /// </summary>
    public TreeViewItem Item { get; }

    public FlyoutTreeViewItemClickedEventArgs(RoutedEvent routedEvent, TreeViewItem treeViewItem)
        : base(routedEvent)
    {
        Item = treeViewItem;
    }
}
