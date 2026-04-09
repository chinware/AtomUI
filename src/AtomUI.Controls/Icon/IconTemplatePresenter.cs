using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Metadata;

namespace AtomUI.Controls;

public class IconTemplatePresenter : Control, IMotionAwareControl
{
    #region 公共属性定义
    public static readonly StyledProperty<IconTemplate?> IconTemplateProperty =
        AvaloniaProperty.Register<IconTemplatePresenter, IconTemplate?>(nameof(IconTemplate));
    
    public static readonly StyledProperty<IBrush?> IconBrushProperty =
        AvaloniaProperty.Register<IconTemplatePresenter, IBrush?>(nameof(IconBrush));
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<IconTemplatePresenter>();
    
    [Content]
    public IconTemplate? IconTemplate
    {
        get => GetValue(IconTemplateProperty);
        set => SetValue(IconTemplateProperty, value);
    }
    
    public IBrush? IconBrush
    {
        get => GetValue(IconBrushProperty);
        set => SetValue(IconBrushProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    #endregion
    
    static IconTemplatePresenter()
    {
        AffectsMeasure<IconTemplatePresenter>( IconTemplateProperty);
        AffectsRender<IconTemplatePresenter>(IconBrushProperty);
        IconTemplateProperty.Changed.AddClassHandler<IconTemplatePresenter>((x, e) => x.HandleIconTemplateChanged(e));
    }

    private void HandleIconTemplateChanged(AvaloniaPropertyChangedEventArgs change)
    {
        var oldIconTemplate = (IconTemplate?)change.OldValue;
        var newIconTemplate = (IconTemplate?)change.NewValue;
        if (oldIconTemplate != null)
        {
            LogicalChildren.Clear();
            VisualChildren.Clear();
        }

        if (newIconTemplate != null)
        {
            var pathIcon = newIconTemplate.Build();
            if (pathIcon != null)
            {
                ConfigureIcon(pathIcon);
            }
        }
    }

    private void ConfigureIcon(PathIcon pathIcon)
    {
        pathIcon[!WidthProperty]            = this[!WidthProperty];
        pathIcon[!HeightProperty]           = this[!HeightProperty];
        pathIcon[!IsMotionEnabledProperty]  = this[!IsMotionEnabledProperty];
        pathIcon[!Icon.StrokeBrushProperty] = this[!IconBrushProperty];
        pathIcon[!Icon.FillBrushProperty]   = this[!IconBrushProperty];
        VisualChildren.Add(pathIcon);
        LogicalChildren.Add(pathIcon);
    }
}