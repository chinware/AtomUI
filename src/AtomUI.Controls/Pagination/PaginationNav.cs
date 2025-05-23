using System.Reactive.Disposables;
using AtomUI.Data;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

internal class PaginationNav : SelectingItemsControl,
                               ISizeTypeAware,
                               ITokenResourceConsumer
{
    #region 公共属性
    
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<PaginationNav>();

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<PaginationNav>();

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public event EventHandler<PageNavRequestArgs>? PageNavigateRequest;
    
    #endregion

    #region 内部属性

    CompositeDisposable? ITokenResourceConsumer.TokenBindingsDisposable => _tokenBindingsDisposable;

    #endregion
     
    private CompositeDisposable? _tokenBindingsDisposable;

    static PaginationNav()
    {
        AffectsMeasure<PaginationNav>(SizeTypeProperty);
    }

    public PaginationNav()
    {
        for (var i = 0; i < Pagination.MaxNavItemCount; i++)
        {
            Items.Add(new PaginationNavItem());
        }

        SelectionMode = SelectionMode.Single;
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new PaginationNavItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<PaginationNavItem>(item, out recycleKey);
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is PaginationNavItem navItem)
        {
            BindUtils.RelayBind(this, SizeTypeProperty, navItem, PaginationNavItem.SizeTypeProperty);
            BindUtils.RelayBind(this, IsMotionEnabledProperty, navItem, PaginationNavItem.IsMotionEnabledProperty);
            navItem.Click += (sender, args) =>
            {
                if (sender is PaginationNavItem navItemSender)
                {
                    PageNavigateRequest?.Invoke(this, new PageNavRequestArgs(navItemSender, IndexFromContainer(navItemSender), navItemSender.PageNumber));
                }
            };
        }
    }
    
    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        _tokenBindingsDisposable = new CompositeDisposable();
        base.OnAttachedToLogicalTree(e);
    }
    
    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
    }
}