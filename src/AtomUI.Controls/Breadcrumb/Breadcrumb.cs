using System.Collections;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

/// <summary>
/// 继承SelectingItemsControl，配合TemplatePart注解，表示这个组件在使用的时候，可以包裹其他内容
/// </summary>
[TemplatePart("PART_ItemsPresenter", typeof(ItemsPresenter))]
public class Breadcrumb : SelectingItemsControl, IControlSharedTokenResourcesHost, IMotionAwareControl
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
        this.LayoutUpdated += OnLayoutUpdatedOnce;
    }

    private void OnLayoutUpdatedOnce(object? sender, EventArgs e)
    {
        this.LayoutUpdated -= OnLayoutUpdatedOnce;
        UpdateSeparators();
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdateSeparators();
    }
    
    private void UpdateSeparators()
    {
        var items = this.GetVisualDescendants().OfType<BreadcrumbItem>().ToList();
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            if (item.Value != null)
            {
                item.Content = item.Value.ToString();
            }
            bool isLast = (i == items.Count - 1);
            item.EffectiveSeparator = isLast
                ? string.Empty
                : ((item.Separator ?? this.Separator)) ?? string.Empty;
            item.IsLast = isLast;
            item.SetPseudoClass(BreadcrumbPseudoClass.IsLast, isLast);
        }
    }
    
}