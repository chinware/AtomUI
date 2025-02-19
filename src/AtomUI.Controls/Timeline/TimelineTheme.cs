using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TimelineTheme : BaseControlTheme
{
    public const string FrameDecoratorPart = "PART_FrameDecorator";
    public const string ScrollViewerPart = "PART_ScrollViewer";
    public const string ItemsPresenterPart = "PART_ItemsPresenter";

    public TimelineTheme() : base(typeof(Timeline))
    {
    }
    
    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<Timeline>((timeline, scope) =>
        {
            var frameDecorator = new Border()
            {
                Name = FrameDecoratorPart
            };
         
            var itemsPresenter = BuildItemsPresenter(timeline, scope);
            var scrollViewer   = BuildScrollViewer(timeline, scope);
            
            CreateTemplateParentBinding(frameDecorator, Border.CornerRadiusProperty, TemplatedControl.CornerRadiusProperty);
            CreateTemplateParentBinding(frameDecorator, Border.BorderBrushProperty, TemplatedControl.BorderBrushProperty);
            CreateTemplateParentBinding(frameDecorator, Border.BackgroundProperty, TemplatedControl.BackgroundProperty);
            CreateTemplateParentBinding(frameDecorator, Decorator.PaddingProperty, TemplatedControl.PaddingProperty);

            scrollViewer.Content = itemsPresenter;
            frameDecorator.Child = scrollViewer;

            return frameDecorator;
        });
    }
    
    private ScrollViewer BuildScrollViewer(Timeline timeline, INameScope scope)
    {
        var scrollViewer = new ScrollViewer
        {
            Name = ScrollViewerPart,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            IsScrollChainingEnabled = true,
        };

        return scrollViewer;
    }

    private ItemsPresenter BuildItemsPresenter(Timeline timeline, INameScope scope)
    {
        // TODO 需要观察是否会有内存泄漏
        var itemsPresenter = new ItemsPresenter
        {
            Name = ItemsPresenterPart,
            ItemsPanel = new FuncTemplate<Panel?>(() =>
            {
                var itemsPanel = new TimelineStackPanel();
                BindUtils.RelayBind(timeline, Timeline.IsReverseProperty, itemsPanel, TimelineStackPanel.IsReverseProperty);
                return itemsPanel;
            })
        };
        itemsPresenter.RegisterInNameScope(scope);
        CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.ItemsPanelProperty,
            ItemsControl.ItemsPanelProperty);
        return itemsPresenter;
    }
    
    protected override void BuildStyles()
    {
        BuildCommonStyle();
    }

    private void BuildCommonStyle()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(Timeline.BorderBrushProperty, SharedTokenKey.ColorBorder);
        commonStyle.Add(Timeline.BackgroundProperty, SharedTokenKey.ColorBgContainer);
        Add(commonStyle);
    }
}