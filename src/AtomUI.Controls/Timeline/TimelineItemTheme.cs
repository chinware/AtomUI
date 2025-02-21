using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TimelineItemTheme : BaseControlTheme
{
    public const string RootLayoutPart = "PART_RootLayout";
    public const string LabelPart = "PART_Label";
    public const string IndicatorPart = "PART_Indicator";
    public const string ContentPresenterPart = "PART_ContentPresenter";
    
    public TimelineItemTheme() : base(typeof(TimelineItem))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<TimelineItem>((timelineItem, scope) =>
        {
            var rootLayout = new TimelineItemPanel
            {
                Name = RootLayoutPart
            };
            CreateTemplateParentBinding(rootLayout, TimelineItemPanel.IsOddProperty, TimelineItem.IsOddProperty);
            CreateTemplateParentBinding(rootLayout, TimelineItemPanel.ModeProperty, TimelineItem.ModeProperty);
            CreateTemplateParentBinding(rootLayout, TimelineItemPanel.IsLabelLayoutProperty, TimelineItem.IsLabelLayoutProperty);
            var labelText = new TextBlock
            {
                Name              = LabelPart,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping      = TextWrapping.Wrap,
            };
            
            CreateTemplateParentBinding(labelText, TextBlock.TextProperty, TimelineItem.LabelProperty);
            rootLayout.Children.Add(labelText);

            var indicator = new TimelineIndicator
            {
                Name = IndicatorPart,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            CreateTemplateParentBinding(indicator, TimelineIndicator.IsFirstProperty, TimelineItem.IsFirstProperty);
            CreateTemplateParentBinding(indicator, TimelineIndicator.IsLastProperty, TimelineItem.IsLastProperty);
            CreateTemplateParentBinding(indicator, TimelineIndicator.IndicatorColorProperty, TimelineItem.IndicatorColorProperty);
            CreateTemplateParentBinding(indicator, TimelineIndicator.IndicatorIconProperty, TimelineItem.IndicatorIconProperty);
            CreateTemplateParentBinding(indicator, TimelineIndicator.NextIsPendingProperty, TimelineItem.NextIsPendingProperty);
            rootLayout.Children.Add(indicator);

            var contentPresenter = new ContentPresenter
            {
                Name = ContentPresenterPart,
            };
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty, TimelineItem.ContentProperty,
                BindingMode.Default, 
                new FuncValueConverter<object?, object?>(o =>
                {
                    if (o is string text)
                    {
                        return new TextBlock
                        {
                            Text = text,
                            VerticalAlignment = VerticalAlignment.Center,
                            TextWrapping = TextWrapping.Wrap
                        };
                    }
                    return o;
                }));
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty, TimelineItem.ContentTemplateProperty);
            rootLayout.Children.Add(contentPresenter);
            
            return rootLayout;
        });
    }

    protected override void BuildStyles()
    {
        var commonStyle    = new Style(selector => selector.Nesting());

        {
            {
                var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(IndicatorPart));
                indicatorStyle.Add(TimelineIndicator.WidthProperty, TimelineTokenKey.IndicatorWidth);
                commonStyle.Add(indicatorStyle);
            }
        
            var labelStyle = new Style(selector => selector.Nesting().Template().Name(LabelPart));
            labelStyle.Add(TextBlock.PaddingProperty, TimelineTokenKey.ItemPaddingBottom);
            commonStyle.Add(labelStyle);
        
            var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
            contentPresenterStyle.Add(TextBlock.PaddingProperty, TimelineTokenKey.ItemPaddingBottom);
            commonStyle.Add(contentPresenterStyle);
        }
        {
            // 非 Reverse 状态下，下一个是 pending item 设置大内间距
            var nextPendingStyle = new Style(selector => selector.Nesting().PropertyEquals(TimelineItem.NextIsPendingProperty, true)
                                                                 .PropertyEquals(TimelineItem.IsReverseProperty, false));
            
            var labelStyle = new Style(selector => selector.Nesting().Template().Name(LabelPart));
            labelStyle.Add(TextBlock.PaddingProperty, TimelineTokenKey.ItemPaddingBottomLG);
            nextPendingStyle.Add(labelStyle);
        
            var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
            contentPresenterStyle.Add(TextBlock.PaddingProperty, TimelineTokenKey.ItemPaddingBottomLG);
            nextPendingStyle.Add(contentPresenterStyle);
            
            commonStyle.Add(nextPendingStyle);
        }
        {
            // 在 Reverse 下，pending item 自己设置大内间距
            var nextPendingStyle = new Style(selector => selector.Nesting().PropertyEquals(TimelineItem.IsPendingProperty, true)
                                                                 .PropertyEquals(TimelineItem.IsReverseProperty, true));
            var labelStyle = new Style(selector => selector.Nesting().Template().Name(LabelPart));
            labelStyle.Add(TextBlock.PaddingProperty, TimelineTokenKey.ItemPaddingBottomLG);
            nextPendingStyle.Add(labelStyle);
        
            var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
            contentPresenterStyle.Add(TextBlock.PaddingProperty, TimelineTokenKey.ItemPaddingBottomLG);
            nextPendingStyle.Add(contentPresenterStyle);
            
            commonStyle.Add(nextPendingStyle);
        }
        
        var indicatorAtLeftStyle = new Style(selector => selector.Nesting().PropertyEquals(TimelineItem.ModeProperty, TimeLineMode.Left));
        {
            var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(IndicatorPart));
            indicatorStyle.Add(TimelineIndicator.MarginProperty, TimelineTokenKey.IndicatorLeftModeMargin);
            indicatorAtLeftStyle.Add(indicatorStyle);
        }
        commonStyle.Add(indicatorAtLeftStyle);
        
        var indicatorAtRightStyle = new Style(selector => selector.Nesting().PropertyEquals(TimelineItem.ModeProperty, TimeLineMode.Right));
        {
            var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(IndicatorPart));
            indicatorStyle.Add(TimelineIndicator.MarginProperty, TimelineTokenKey.IndicatorRightModeMargin);
            indicatorAtRightStyle.Add(indicatorStyle);
        }
        commonStyle.Add(indicatorAtRightStyle);
        
        var indicatorAtMiddleStyle = new Style(selector => Selectors.Or(selector.Nesting().PropertyEquals(TimelineItem.ModeProperty, TimeLineMode.Alternate),
            selector.Nesting().PropertyEquals(TimelineItem.IsLabelLayoutProperty, true)));
        {
            var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(IndicatorPart));
            indicatorStyle.Add(TimelineIndicator.MarginProperty, TimelineTokenKey.IndicatorMiddleModeMargin);
            indicatorAtMiddleStyle.Add(indicatorStyle);
        }
        commonStyle.Add(indicatorAtMiddleStyle);
        Add(commonStyle);
    }
}