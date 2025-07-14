using System.Collections;
using System.Diagnostics;
using System.Windows.Input;
using AtomUI.Controls.Themes;
using AtomUI.IconPkg;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Avalonia.Metadata;

namespace AtomUI.Controls;

public class BreadcrumbItem
{
    public string? Title { get; set; }
    public string? Separator { get; set; }
    public ICommand? Click { get; set; }
}

public class Breadcrumb : TemplatedControl, IControlSharedTokenResourcesHost, IMotionAwareControl
{
    #region 公共属性定义
    
    public static readonly StyledProperty<string?> SeparatorProperty =
        AvaloniaProperty.Register<Breadcrumb, string?>(nameof(Separator), defaultValue:"=");
    
    public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
        ItemsControl.ItemsSourceProperty.AddOwner<Breadcrumb>();
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Breadcrumb>();
    
    public IEnumerable? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
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
    
    static Breadcrumb()
    {
        AffectsMeasure<Breadcrumb>();
        AffectsRender<Breadcrumb>();
    }
    
    public Breadcrumb()
    {
        this.RegisterResources();
        this.BindMotionProperties();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (ItemsSource != null)
        {
            foreach (var _itemValue in ItemsSource)
            {
                var itemValue = (BreadcrumbItem)_itemValue;
                Console.WriteLine($"itemValue={itemValue.Title} | {itemValue.Separator}");
            }
        }
    }
    
}