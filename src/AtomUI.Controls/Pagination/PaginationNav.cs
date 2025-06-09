using System.Reactive.Disposables;
using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;

namespace AtomUI.Controls;

internal class PaginationNav : SelectingItemsControl,
                               ISizeTypeAware,
                               IResourceBindingManager
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

    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable => _resourceBindingsDisposable;

    #endregion
     
    private CompositeDisposable? _resourceBindingsDisposable;
    private NavItemsPresenter? _itemsPresenter;

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

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _resourceBindingsDisposable = new CompositeDisposable();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        this.DisposeTokenBindings();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _itemsPresenter = e.NameScope.Find<NavItemsPresenter>(PaginationNavThemeConstants.ItemsPresenterPart);
        SetupItemPresenterSpacing();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == PaginationNavItem.SizeTypeProperty)
        {
            SetupItemPresenterSpacing();
        }
    }

    private void SetupItemPresenterSpacing()
    {
        if (_itemsPresenter != null)
        {
            if (SizeType == SizeType.Large || SizeType == SizeType.Middle)
            {
                _itemsPresenter.IsEnabledSpacing = true;
            }
            else
            {
                _itemsPresenter.IsEnabledSpacing = false;
            }
        }
    }
}

internal class NavItemsPresenter : ItemsPresenter,
                                   IResourceBindingManager
{
    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable => _resourceBindingsDisposable;
    private CompositeDisposable? _resourceBindingsDisposable;
    
    internal static readonly DirectProperty<NavItemsPresenter, bool> IsEnabledSpacingProperty =
        AvaloniaProperty.RegisterDirect<NavItemsPresenter, bool>(nameof(IsEnabledSpacing),
            o => o.IsEnabledSpacing,
            (o, v) => o.IsEnabledSpacing = v);
    
    private bool _isEnabledSpacing = true;
    public bool IsEnabledSpacing
    {
        get => _isEnabledSpacing;
        set => SetAndRaise(IsEnabledSpacingProperty, ref _isEnabledSpacing, value);
    }
    
    public NavItemsPresenter()
    {
        ItemsPanel = new FuncTemplate<Panel?>(() =>
        {
            var panel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            if (IsEnabledSpacing)
            {
                this.AddResourceBindingDisposable(
                    TokenResourceBinder.CreateTokenBinding(panel, StackPanel.SpacingProperty, SharedTokenKey.MarginXS));
            }
            return panel;
        });
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsEnabledSpacingProperty)
        {
            if (Panel is StackPanel stackPanel)
            {
                if (IsEnabledSpacing)
                {
                    this.AddResourceBindingDisposable(
                        TokenResourceBinder.CreateTokenBinding(stackPanel, StackPanel.SpacingProperty, SharedTokenKey.MarginXS));
                }
                else
                {
                    stackPanel.SetValue(StackPanel.SpacingProperty, 0, BindingPriority.Template);
                }
            }
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _resourceBindingsDisposable = new CompositeDisposable();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        this.DisposeTokenBindings();
    }
}