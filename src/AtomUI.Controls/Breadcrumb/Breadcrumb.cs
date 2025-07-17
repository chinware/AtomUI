using System.Collections;
using System.Windows.Input;
using AtomUI.IconPkg;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

public class BreadcrumbItem
{
    public string? Title { get; set; }
    public string? Separator { get; set; }
    public ICommand? Click { get; set; }
    public string? EffectiveSeparator { get; set; }
    public Icon? Icon { get; set; }
    public bool HasIconAndText => Icon != null && !string.IsNullOrEmpty(Title);
    public string? Value { get; set; }
}

public class Breadcrumb : TemplatedControl, IControlSharedTokenResourcesHost, IMotionAwareControl
{
    #region 公共属性定义
    
    public static readonly StyledProperty<IEnumerable?> ParamsProperty =
        AvaloniaProperty.Register<Breadcrumb, IEnumerable?>(
            nameof(Params));
    
    public static readonly StyledProperty<double> IconFontSizeProperty =
        AvaloniaProperty.Register<Breadcrumb, double>(nameof(IconFontSize), defaultValue:16);
    
    public static readonly StyledProperty<string?> SeparatorProperty =
        AvaloniaProperty.Register<Breadcrumb, string?>(
            nameof(Separator), 
            defaultValue:"/"
            );
    
    public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
        AvaloniaProperty.Register<Breadcrumb, IEnumerable?>(
            nameof(ItemsSource));
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Breadcrumb>();
    
    public IEnumerable? Params
    {
        get => GetValue(ParamsProperty);
        set => SetValue(ParamsProperty, value);
    }
    
    public double IconFontSize
    {
        get => GetValue(IconFontSizeProperty);
        set => SetValue(IconFontSizeProperty, value);
    }
    
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
        Console.WriteLine($"[Breadcrumb] IconFontSize={IconFontSize}");
    }

    /*
     * @desc: OnAttachedToVisualTree方法
     */
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        //Console.WriteLine($"IconFontSize={IconFontSize}");
        // 如果BreadcrumbItem没有设置Separator，那么使用全局Separator
        if (ItemsSource != null)
        {
            //Console.WriteLine($"Global.Separator: {Separator}");
            /*
             * 如果 ItemsSource 的类型不明确，编译器可能无法推断出具体的类型参数。
             */
            var itemsSourceList = ItemsSource.Cast<BreadcrumbItem>().ToList();
            for (int i = 0; i < itemsSourceList.Count; i++)
            {
                var itemValue = itemsSourceList[i];
                if (itemValue.Value != null)
                {
                    itemValue.Title = itemValue.Value.ToString();
                }
                if (i == (itemsSourceList.Count - 1))
                {
                    itemValue.Separator = null;
                }
                else
                {
                    itemValue.Separator = itemValue.Separator == null ? Separator : itemValue.Separator ;
                }
            }
            /*
            foreach (var _itemValue in ItemsSource)
            {
                var itemValue = (BreadcrumbItem)_itemValue;
                if (itemValue.Separator == null)
                {
                    itemValue.Separator = Separator;
                }
            }
            */
        }
    }
    
}