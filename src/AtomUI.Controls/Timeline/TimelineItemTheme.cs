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

    protected TimelineItemTheme(Type targetType) : base(targetType)
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<TimelineItem>((timelineItem, scope) =>
        {
            var columnDefinition = CalculateGridColumn(timelineItem);
            var grid = new Grid()
            {
                Name              = GridPart,
                ColumnDefinitions = columnDefinition,
                RowDefinitions =
                {
                    new RowDefinition(GridLength.Star),
                    new RowDefinition(GridLength.Star),
                },
            };

            var labelBlock = new TextBlock()
            {
                Name                = LabelPart,
                VerticalAlignment   = VerticalAlignment.Top,
                HorizontalAlignment = timelineItem.LabelTextAlign,
            };

            CreateTemplateParentBinding(labelBlock, TextBlock.TextProperty, TimelineItem.LabelProperty);
            CreateTemplateParentBinding(labelBlock, TextBlock.IsVisibleProperty, TimelineItem.HasLabelProperty);

            Grid.SetColumn(labelBlock, timelineItem.LabelIndex);
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
            Grid.SetColumn(splitPanel, timelineItem.SplitIndex);
            grid.Children.Add(splitPanel);

            var contentPresenter = new ContentPresenter()
            {
                Name                = ItemsContentPresenterPart,
                HorizontalAlignment = timelineItem.ContentTextAlign,
            };
            contentPresenter.RegisterInNameScope(scope);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty,
                ContentControl.ContentProperty);

            Grid.SetColumn(contentPresenter, timelineItem.ContentIndex);
            grid.Children.Add(contentPresenter);

            return grid;
        });
    }

    private void BuildDotIcon(DockPanel panel, TimelineItem timelineItem, INameScope scope)
    {
        if (timelineItem.DotIcon is not null)
        {
            timelineItem.DotIcon.Width  = 10;
            timelineItem.DotIcon.Height = 10;
            timelineItem.DotIcon.Name   = DotPart;
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
            DockPanel.SetDock(splitHead, Dock.Top);
            panel.Children.Add(splitHead);
        }

        var dotBorderStyle = new Style(selector => selector.Nesting().Template().Name(SplitHeadPart));
        var dotIconStyle = new Style(selector =>
            selector.Nesting().Not(x => x.PropertyEquals(TimelineItem.DotIconProperty, null)).Template().Name(DotPart));

        if (timelineItem.Color.StartsWith("#"))
        {
            try
            {
                var color = Color.Parse(timelineItem.Color);
                var brush = new SolidColorBrush(color);
                dotBorderStyle.Add(ContentPresenter.BorderBrushProperty, brush);
                dotIconStyle.Add(PathIcon.NormalFilledBrushProperty, brush);
            }
            catch (Exception)
            {
                dotBorderStyle.Add(ContentPresenter.BorderBrushProperty,
                    GlobalTokenResourceKey.ColorPrimary);
            }
        }
        else
        {
            switch (timelineItem.Color)
            {
                case "blue":
                    dotBorderStyle.Add(ContentPresenter.BorderBrushProperty,
                        GlobalTokenResourceKey.ColorPrimary);
                    dotIconStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalTokenResourceKey.ColorPrimary);
                    break;
                case "green":
                    dotBorderStyle.Add(ContentPresenter.BorderBrushProperty,
                        GlobalTokenResourceKey.ColorSuccess);
                    dotIconStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalTokenResourceKey.ColorSuccess);
                    break;
                case "red":
                    dotBorderStyle.Add(ContentPresenter.BorderBrushProperty,
                        GlobalTokenResourceKey.ColorError);
                    dotIconStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalTokenResourceKey.ColorError);
                    break;
                case "gray":
                    dotBorderStyle.Add(ContentPresenter.BorderBrushProperty,
                        GlobalTokenResourceKey.ColorTextDisabled);
                    dotIconStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalTokenResourceKey.ColorTextDisabled);
                    break;
                default:
                    dotBorderStyle.Add(ContentPresenter.BorderBrushProperty,
                        GlobalTokenResourceKey.ColorPrimary);
                    dotIconStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalTokenResourceKey.ColorPrimary);
                    break;
            }
        }

        dotBorderStyle.Add(ContentPresenter.BackgroundProperty, TimelineTokenResourceKey.DotBg);
        Add(dotBorderStyle);
        if (timelineItem.DotIcon?.NormalFilledBrush is null)
        {
            Add(dotIconStyle);
        }
    }

    private ColumnDefinitions CalculateGridColumn(TimelineItem timelineItem)
    {
        if (timelineItem.HasLabel || timelineItem.Mode == "alternate")
        {
            return new ColumnDefinitions()
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(new GridLength(10)),
                new ColumnDefinition(GridLength.Star)
            };
        }

        if (timelineItem.LabelIndex == 0)
        {
            return new ColumnDefinitions()
            {
                new ColumnDefinition(new GridLength(10)),
                new ColumnDefinition(GridLength.Star)
            };
        }
        else
        {
            return new ColumnDefinitions()
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(new GridLength(10))
            };
        }
    }

    protected override void BuildStyles()
    {
        // 分割线样式
        var splitLineborderStyle = new Style(selector => selector.Nesting().Template().Name(SplitLineBorderPart));
        splitLineborderStyle.Add(Layoutable.WidthProperty, TimelineTokenResourceKey.TailWidth);
        Add(splitLineborderStyle);

        var lineStyle = new Style(selector => selector.Nesting().Template().Child().OfType<Rectangle>());
        lineStyle.Add(Shape.StrokeProperty, TimelineTokenResourceKey.TailColor);
        lineStyle.Add(Layoutable.WidthProperty, TimelineTokenResourceKey.TailWidth);
        lineStyle.Add(Shape.StrokeThicknessProperty, TimelineTokenResourceKey.TailWidth);

        Add(lineStyle);

        // 内容样式
        var contentPresenterStyle =
            new Style(selector => selector.Nesting().Template().Name(ItemsContentPresenterPart));
        contentPresenterStyle.Add(ContentPresenter.FontSizeProperty, TimelineTokenResourceKey.FontSize);
        contentPresenterStyle.Add(ContentPresenter.PaddingProperty, TimelineTokenResourceKey.ItemPaddingBottom);

        var contentPresenterLeftStyle =
            new Style(selector => selector.Nesting().Template().Name(ItemsContentPresenterPart));
        contentPresenterLeftStyle.Add(Layoutable.MarginProperty, TimelineTokenResourceKey.RightMargin);

        var contentPresenterRightStyle =
            new Style(selector => selector.Nesting().Template().Name(ItemsContentPresenterPart));
        contentPresenterRightStyle.Add(Layoutable.MarginProperty, TimelineTokenResourceKey.LeftMargin);

        var contentLeftStyle  = new Style(selector => selector.Nesting().Class(TimelineItem.ContentLeft));
        var contentRightStyle = new Style(selector => selector.Nesting().Not(x => x.Class(TimelineItem.ContentLeft)));

        contentLeftStyle.Add(contentPresenterLeftStyle);
        contentRightStyle.Add(contentPresenterRightStyle);

        Add(contentPresenterStyle);
        Add(contentLeftStyle);
        Add(contentRightStyle);

        // 标签样式
        var labelStyle = new Style(selector => selector.Nesting().Template().Name(LabelPart));
        labelStyle.Add(TextBlock.FontSizeProperty, TimelineTokenResourceKey.FontSize);

        var labelLeftStyle = new Style(selector =>
            selector.Nesting().Class(TimelineItem.LabelLeft).Template().Name(LabelPart));
        var labelRightStyle = new Style(selector =>
            selector.Nesting().Not(x => x.Class(TimelineItem.LabelLeft)).Template().Name(LabelPart));
        labelLeftStyle.Add(TextBlock.PaddingProperty, TimelineTokenResourceKey.RightMargin);
        labelRightStyle.Add(TextBlock.PaddingProperty, TimelineTokenResourceKey.LeftMargin);

        Add(labelStyle);
        Add(labelLeftStyle);
        Add(labelRightStyle);
    }
}