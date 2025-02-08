using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using HorizontalAlignment = Avalonia.Layout.HorizontalAlignment;
using VerticalAlignment = Avalonia.Layout.VerticalAlignment;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TimelineItemTheme : BaseControlTheme
{
    public const string GridPart = "PART_Grid";
    public const string ItemsContentPresenterPart = "PART_ItemsContentPresenter";
    public const string SplitPanelPart = "PART_PanelHead";
    public const string SplitHeadPart = "PART_SplitHead";
    public const string SplitLineBorderPart = "PART_SplitBorderLine";
    public const string SplitLinePart = "PART_SplitLine";
    public const string LabelPart = "PART_Label";
    public const string DotPart = "PART_Dot";

    public TimelineItemTheme() : base(typeof(TimelineItem))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<TimelineItem>((timelineItem, scope) =>
        {
            var grid = new Grid()
            {
                Name = GridPart,
                ColumnDefinitions = new ColumnDefinitions()
                {
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(new GridLength(10)),
                    new ColumnDefinition(GridLength.Star)
                },
                RowDefinitions =
                {
                    new RowDefinition(GridLength.Star),
                    new RowDefinition(GridLength.Star),
                },
            };
            grid.RegisterInNameScope(scope);

            var labelBlock = new TextBlock()
            {
                Name              = LabelPart,
                VerticalAlignment = VerticalAlignment.Top,
            };
            labelBlock.RegisterInNameScope(scope);

            CreateTemplateParentBinding(labelBlock, TextBlock.TextProperty, TimelineItem.LabelProperty);
            CreateTemplateParentBinding(labelBlock, Visual.IsVisibleProperty, TimelineItem.HasLabelProperty);
            CreateTemplateParentBinding(labelBlock, Layoutable.HorizontalAlignmentProperty,
                TimelineItem.LabelTextAlignProperty);

            grid.Children.Add(labelBlock);

            var splitPanel = new DockPanel()
            {
                Name  = SplitPanelPart,
                Width = 10,
            };
            splitPanel.RegisterInNameScope(scope);
            BuildDotIcon(splitPanel, timelineItem, scope);

            var border = new Border
            {
                Name                = SplitLineBorderPart,
                VerticalAlignment   = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            var verticalDashedLine = new Rectangle
            {
                Name                = SplitLinePart,
                StrokeLineCap       = PenLineCap.Round,
                Fill                = Brushes.Transparent,
                VerticalAlignment   = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            verticalDashedLine.RegisterInNameScope(scope);

            border.Child = verticalDashedLine;

            splitPanel.Children.Add(border);
            grid.Children.Add(splitPanel);

            var contentPresenter = new ContentPresenter()
            {
                Name = ItemsContentPresenterPart,
            };
            contentPresenter.RegisterInNameScope(scope);

            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty,
                ContentControl.ContentProperty);
            CreateTemplateParentBinding(contentPresenter, Layoutable.HorizontalAlignmentProperty,
                TimelineItem.ContentTextAlignProperty);

            grid.Children.Add(contentPresenter);

            return grid;
        });
    }

    private void BuildDotIcon(DockPanel panel, TimelineItem timelineItem, INameScope scope)
    {
        if (timelineItem.DotIcon is not null)
        {
            timelineItem.DotIcon.Width             = 10;
            timelineItem.DotIcon.Height            = 10;
            timelineItem.DotIcon.Name              = DotPart;
            timelineItem.DotIcon.RegisterInNameScope(scope);
            DockPanel.SetDock(timelineItem.DotIcon, Dock.Top);
            panel.Children.Add(timelineItem.DotIcon);
        }
        else
        {
            var splitHead = new Border()
            {
                Width           = 10,
                Height          = 10,
                CornerRadius    = new CornerRadius(5),
                BorderThickness = new Thickness(3),
                Name            = SplitHeadPart,
            };
            splitHead.RegisterInNameScope(scope);
            DockPanel.SetDock(splitHead, Dock.Top);
            panel.Children.Add(splitHead);
        }
    }

    protected override void BuildStyles()
    {
        // 分割线样式
        var splitLineborderStyle = new Style(selector => selector.Nesting().Template().Name(SplitLineBorderPart));
        splitLineborderStyle.Add(Layoutable.WidthProperty, TimelineTokenKey.TailWidth);
        Add(splitLineborderStyle);
        
        var lineStyle = new Style(selector => selector.Nesting().Template().Child().OfType<Rectangle>());
        lineStyle.Add(Shape.StrokeProperty, TimelineTokenKey.TailColor);
        lineStyle.Add(Layoutable.WidthProperty, TimelineTokenKey.TailWidth);
        lineStyle.Add(Shape.StrokeThicknessProperty, TimelineTokenKey.TailWidth);

        Add(lineStyle);
        
        // 内容样式
        var contentPresenterStyle =
            new Style(selector => selector.Nesting().Template().Name(ItemsContentPresenterPart));
        contentPresenterStyle.Add(ContentPresenter.FontSizeProperty, TimelineTokenKey.FontSize);
        contentPresenterStyle.Add(ContentPresenter.PaddingProperty, TimelineTokenKey.ItemPaddingBottom);

        var contentPresenterLeftStyle =
            new Style(selector => selector.Nesting().Template().Name(ItemsContentPresenterPart));
        contentPresenterLeftStyle.Add(Layoutable.MarginProperty, TimelineTokenKey.RightMargin);

        var contentPresenterRightStyle =
            new Style(selector => selector.Nesting().Template().Name(ItemsContentPresenterPart));
        contentPresenterRightStyle.Add(Layoutable.MarginProperty, TimelineTokenKey.LeftMargin);

        var contentLeftStyle  = new Style(selector => selector.Nesting().Class(TimelineItem.ContentLeftPC));
        var contentRightStyle = new Style(selector => selector.Nesting().Not(x => x.Class(TimelineItem.ContentLeftPC)));

        contentLeftStyle.Add(contentPresenterLeftStyle);
        contentRightStyle.Add(contentPresenterRightStyle);

        Add(contentPresenterStyle);
        Add(contentLeftStyle);
        Add(contentRightStyle);

        // 标签样式
        var labelStyle = new Style(selector => selector.Nesting().Template().Name(LabelPart));
        labelStyle.Add(TextBlock.FontSizeProperty, TimelineTokenKey.FontSize);

        var labelLeftStyle = new Style(selector =>
            selector.Nesting().Class(TimelineItem.LabelLeftPC).Template().Name(LabelPart));
        var labelRightStyle = new Style(selector =>
            selector.Nesting().Not(x => x.Class(TimelineItem.LabelLeftPC)).Template().Name(LabelPart));
        labelLeftStyle.Add(TextBlock.PaddingProperty, TimelineTokenKey.RightMargin);
        labelRightStyle.Add(TextBlock.PaddingProperty, TimelineTokenKey.LeftMargin);

        Add(labelStyle);
        Add(labelLeftStyle);
        Add(labelRightStyle);
    }
}