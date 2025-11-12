using AtomUI.Controls.Themes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

internal class TextAreaDecoratedBox : AddOnDecoratedBox
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsAllowAutoHideProperty =
        AvaloniaProperty.Register<TextAreaDecoratedBox, bool>(
            nameof(IsAllowAutoHide));
    
    public static readonly StyledProperty<ScrollBarVisibility> HorizontalScrollBarVisibilityProperty =
        AvaloniaProperty.Register<TextAreaDecoratedBox, ScrollBarVisibility>(
            nameof(HorizontalScrollBarVisibility), ScrollBarVisibility.Disabled);
    
    public static readonly StyledProperty<ScrollBarVisibility> VerticalScrollBarVisibilityProperty =
        AvaloniaProperty.Register<TextAreaDecoratedBox, ScrollBarVisibility>(
            nameof(VerticalScrollBarVisibility), ScrollBarVisibility.Auto);
    
    public static readonly StyledProperty<bool> IsScrollChainingEnabledProperty =
        AvaloniaProperty.Register<TextAreaDecoratedBox, bool>(
            nameof(IsScrollChainingEnabled), true);
    
    public bool IsAllowAutoHide
    {
        get => GetValue(IsAllowAutoHideProperty);
        set => SetValue(IsAllowAutoHideProperty, value);
    }
    
    public ScrollBarVisibility HorizontalScrollBarVisibility
    {
        get => GetValue(HorizontalScrollBarVisibilityProperty);
        set => SetValue(HorizontalScrollBarVisibilityProperty, value);
    }
    
    public ScrollBarVisibility VerticalScrollBarVisibility
    {
        get => GetValue(VerticalScrollBarVisibilityProperty);
        set => SetValue(VerticalScrollBarVisibilityProperty, value);
    }
    
    public bool IsScrollChainingEnabled
    {
        get => GetValue(IsScrollChainingEnabledProperty);
        set => SetValue(IsScrollChainingEnabledProperty, value);
    }
    #endregion
    
    internal ScrollViewer? ScrollViewer { get; set; }
    internal TextArea? Owner { get; set; }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        ScrollViewer = e.NameScope.Find<ScrollViewer>(TextAreaThemeConstants.ScrollViewerPart);
        if (ScrollViewer != null)
        {
            Owner?.NotifyScrollViewerCreated(ScrollViewer);
        }
    }
}