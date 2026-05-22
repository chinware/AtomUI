using System.Collections.Specialized;
using AtomUI.Controls;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Metadata;

namespace AtomUI.Desktop.Controls;

public class Breadcrumb : ItemsControl, IMotionAwareControl
{
    public const string DefaultSeparator = "/";
    
    #region 公共属性定义
    public static readonly StyledProperty<object?> SeparatorProperty =
        AvaloniaProperty.Register<Breadcrumb, object?>(
            nameof(Separator),
            defaultValue: DefaultSeparator
        );
    
    public static readonly StyledProperty<IDataTemplate?> SeparatorTemplateProperty =
        AvaloniaProperty.Register<Breadcrumb, IDataTemplate?>(nameof(SeparatorTemplate));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Breadcrumb>();

    [DependsOn(nameof(SeparatorTemplate))]
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

    public Breadcrumb()
    {
        LogicalChildren.CollectionChanged += HandleItemsCollectionChanged;
        this.RegisterTokenResourceScope(BreadcrumbToken.ScopeProvider);
    }

    private void HandleItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (LogicalChildren.Count > 0)
        {
            for (int i = 0; i < LogicalChildren.Count; i++)
            {
                var item = LogicalChildren[i];
                if (item is BreadcrumbItem breadcrumbItem)
                {
                    breadcrumbItem.IsLast = (i == LogicalChildren.Count - 1);
                }
            }
        }
    }
    
    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new BreadcrumbItem();
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
            if (item != null && item is not Visual)
            {
                if (!breadcrumbItem.IsSet(BreadcrumbItem.ContentProperty))
                {
                    breadcrumbItem.SetCurrentValue(BreadcrumbItem.ContentProperty, item);
                }
                
                if (item is IBreadcrumbItemData breadcrumbItemData)
                {
                    if (!breadcrumbItem.IsSet(BreadcrumbItem.IconProperty))
                    {
                        breadcrumbItem.SetCurrentValue(BreadcrumbItem.IconProperty, breadcrumbItemData.Icon);
                    }
                
                    if (breadcrumbItemData.Separator != null)
                    {
                        breadcrumbItem.SetValue(BreadcrumbItem.SeparatorProperty, breadcrumbItemData.Separator);
                    }
                    else
                    {
                        breadcrumbItem.ClearValue(BreadcrumbItem.SeparatorProperty);
                    }
                
                    if (breadcrumbItemData.SeparatorTemplate != null)
                    {
                        breadcrumbItem.SetValue(BreadcrumbItem.SeparatorTemplateProperty, breadcrumbItemData.SeparatorTemplate);
                    }
                    else
                    {
                        breadcrumbItem.ClearValue(BreadcrumbItem.SeparatorTemplateProperty);
                    }
                
                    if (!breadcrumbItem.IsSet(BreadcrumbItem.NavigateContextProperty) && breadcrumbItemData.NavigateContext != null)
                    {
                        breadcrumbItem.SetCurrentValue(BreadcrumbItem.NavigateContextProperty, breadcrumbItemData.NavigateContext);
                    }
                           
                    if (!breadcrumbItem.IsSet(BreadcrumbItem.NavigateUriProperty) && breadcrumbItemData.NavigateUri != null)
                    {
                        breadcrumbItem.SetCurrentValue(BreadcrumbItem.NavigateUriProperty, breadcrumbItemData.NavigateUri);
                    }
                }
            }
            
            if (ItemTemplate != null)
            {
                breadcrumbItem[!BreadcrumbItem.ContentTemplateProperty] = this[!ItemTemplateProperty];
            }
            breadcrumbItem[!IsMotionEnabledProperty] = this[!IsMotionEnabledProperty];
            PrepareBreadcrumbItem(breadcrumbItem, item, index);
            if (!Equals(Separator, DefaultSeparator) ||
                SeparatorTemplate is not null)
            {
                ConfigureItemSeparator(breadcrumbItem);
            }
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(container), "The container type is incorrect, it must be type BreadcrumbItem.");
        }
    }
    
    protected virtual void PrepareBreadcrumbItem(BreadcrumbItem breadcrumbItem, object? item, int index)
    {
    }

    private void ConfigureItemSeparator(BreadcrumbItem breadcrumbItem)
    {
        breadcrumbItem.SetValue(BreadcrumbItem.SeparatorProperty, Separator, BindingPriority.Style);
        breadcrumbItem.SetValue(BreadcrumbItem.SeparatorTemplateProperty, SeparatorTemplate, BindingPriority.Style);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SeparatorProperty ||
            change.Property == SeparatorTemplateProperty)
        {
            foreach (Control container in GetRealizedContainers())
            {
                if (container is BreadcrumbItem breadcrumbItem)
                {
                    ConfigureItemSeparator(breadcrumbItem);
                }
            }
        }
    }

    internal void NotifyNavigateRequest(BreadcrumbItem breadcrumbItem)
    {
        NavigateRequest?.Invoke(this, new BreadcrumbNavigateEventArgs(breadcrumbItem));
    }
}
