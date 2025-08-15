using System.Collections.Specialized;
using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Metadata;

namespace AtomUI.Controls;

public class Breadcrumb : ItemsControl, IControlSharedTokenResourcesHost, IMotionAwareControl
{
    #region 公共属性定义
    public static readonly StyledProperty<object?> SeparatorProperty =
        AvaloniaProperty.Register<Breadcrumb, object?>(
            nameof(Separator),
            defaultValue: "/"
        );
    
    public static readonly StyledProperty<IDataTemplate?> SeparatorTemplateProperty =
        AvaloniaProperty.Register<Breadcrumb, IDataTemplate?>(nameof (SeparatorTemplate));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Breadcrumb>();

    [DependsOn("ContentTemplate")]
    public object? Separator
    {
        get => GetValue(SeparatorProperty);
        set => SetValue(SeparatorProperty, value);
    }
    
    public IDataTemplate? SeparatorTemplate
    {
        get => GetValue(SeparatorTemplateProperty);
        set => SetValue(SeparatorTemplateProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    #endregion

    #region 公共事件定义

    public event EventHandler<BreadcrumbNavigateEventArgs>? NavigateRequest;

    #endregion

    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => BreadcrumbToken.ID;

    public Breadcrumb()
    {
        Items.CollectionChanged += HandleItemsCollectionChanged;
        this.RegisterResources();
    }

    private void HandleItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (Items.Count > 0)
        {
            for (int i = 0; i < ItemCount; i++)
            {
                var item = Items[i];
                if (item is BreadcrumbItem breadcrumbItem)
                {
                    breadcrumbItem.IsLast = (i == ItemCount - 1);
                }
            }
        }
    }
    
    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        var breadcrumbItem = new BreadcrumbItem();
        // 避免破环 BreadcrumbItem 自行指定的分割符
        BindUtils.RelayBind(this, SeparatorProperty, breadcrumbItem, BreadcrumbItem.SeparatorProperty, BindingMode.Default, BindingPriority.Template);
        BindUtils.RelayBind(this, SeparatorTemplateProperty, breadcrumbItem, BreadcrumbItem.SeparatorTemplateProperty, BindingMode.Default, BindingPriority.Template);
        return breadcrumbItem;
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<BreadcrumbItem>(item, out recycleKey);
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is BreadcrumbItem breadcrumbItem)
        {
            breadcrumbItem[!BreadcrumbItem.IsMotionEnabledProperty]   = this[!IsMotionEnabledProperty];
        }
    }

    internal void NotifyNavigateRequest(BreadcrumbItem breadcrumbItem)
    {
        NavigateRequest?.Invoke(this, new BreadcrumbNavigateEventArgs(breadcrumbItem));
    }
}