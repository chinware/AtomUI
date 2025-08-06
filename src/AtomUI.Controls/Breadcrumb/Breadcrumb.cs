using System.Collections;
using System.Collections.Specialized;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

/// <summary>
/// 继承ItemsControl
/// </summary>
public class Breadcrumb : ItemsControl, IControlSharedTokenResourcesHost, IMotionAwareControl
{
    #region 公共属性定义

    public static readonly StyledProperty<IEnumerable?> ParamsProperty =
        AvaloniaProperty.Register<Breadcrumb, IEnumerable?>(
            nameof(Params));

    public static readonly StyledProperty<double> IconSizeProperty =
        AvaloniaProperty.Register<Breadcrumb, double>(nameof(IconSize), defaultValue: 16);

    public static readonly StyledProperty<string?> SeparatorProperty =
        AvaloniaProperty.Register<Breadcrumb, string?>(
            nameof(Separator),
            defaultValue: "/"
        );

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Breadcrumb>();

    public IEnumerable? Params
    {
        get => GetValue(ParamsProperty);
        set => SetValue(ParamsProperty, value);
    }

    public double IconSize
    {
        get => GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    public string? Separator
    {
        get => GetValue(SeparatorProperty);
        set => SetValue(SeparatorProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    #endregion

    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => BreadcrumbToken.ID;

    public Breadcrumb()
    {
        Items.CollectionChanged += OnItemsCollectionChanged;
        this.RegisterResources();
        this.BindMotionProperties();
    }

    private void OnItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // parse a property from axaml is async in Avalonia. So use Dispatcher.UIThread to make sure object property has a value 
        Dispatcher.UIThread.Post(() =>
        {
            this._processChildItems();
        }, DispatcherPriority.Background);
    }

    private void _processChildItems()
    {
        if (Items != null && Items.Count > 0)
        {
            var lastChildItem = Items[Items.Count - 1];
            foreach (var childItem in Items)
            {
                if (childItem is BreadcrumbItem breadcrumbItem)
                {
                    breadcrumbItem.IsLast             = false;
                    breadcrumbItem.EffectiveSeparator = ((breadcrumbItem.Separator ?? this.Separator)) ?? "/";
                    breadcrumbItem.Content = breadcrumbItem.Value != null ? breadcrumbItem.Value.ToString() : breadcrumbItem.Content ;
                }
            }
            if (lastChildItem is BreadcrumbItem lastBreadcrumbItem)
            {
                lastBreadcrumbItem.IsLast = true;
                lastBreadcrumbItem.EffectiveSeparator = lastBreadcrumbItem.IsLast
                    ? string.Empty
                    : ((lastBreadcrumbItem.Separator ?? this.Separator)) ?? string.Empty;
            }
        }
    }
}