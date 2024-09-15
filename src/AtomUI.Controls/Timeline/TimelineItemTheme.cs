using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Avalonia.Styling;
using HorizontalAlignment = Avalonia.Layout.HorizontalAlignment;
using VerticalAlignment = Avalonia.Layout.VerticalAlignment;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TimelineItemTheme : BaseControlTheme
{
    public const string GridPart = "PART_Grid";
    public const string ItemsPresenterPart = "PART_ItemsPresenter";
    public const string SplitHeadPart = "PART_SplitHead";
    public const string SplitLinePart = "PART_SplitLine";
    public const string LabelPart = "PART_Label";

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
            var labelIndex = 0;
            var splitIndex = 1;
            var contentIndex = 2;
            var labelTextAlign = HorizontalAlignment.Right;
            var contentTextAlign = HorizontalAlignment.Right;

            CalculateGridIndex(timelineItem, out labelIndex, out splitIndex, out contentIndex, out labelTextAlign,
                out contentTextAlign);
            System.Console.WriteLine(
                "TimelineItem Mode: {0},hasLabel: {1}, index: {2} labelIndex: {3} splitIndex:{4} contentIndex：{5}",
                timelineItem.Mode, timelineItem.HasLabel, timelineItem.Index, labelIndex, splitIndex, contentIndex);

            var columnDefinition = CalculateGridColumn(timelineItem, labelIndex);

            var grid = new Grid()
            {
                Name = GridPart,
                ColumnDefinitions = columnDefinition,
                RowDefinitions =
                {
                    new RowDefinition(GridLength.Star),
                },
            };

            var labelBlock = new TextBlock()
            {
                Name = LabelPart,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = labelTextAlign,
            };

            CreateTemplateParentBinding(labelBlock, TextBlock.TextProperty, TimelineItem.LabelProperty);
            CreateTemplateParentBinding(labelBlock, TextBlock.IsVisibleProperty, TimelineItem.HasLabelProperty);
            CreateTemplateParentBinding(labelBlock, TextBlock.IsVisibleProperty, TimelineItem.HasLabelProperty);

            var labelStyle = new Style(selector => selector.Nesting().Template().Name(LabelPart));
            labelStyle.Add(TextBlock.FontSizeProperty, TimelineTokenResourceKey.FontSize);
            // todo 未生效
            if (labelIndex == 0)
            {
                labelStyle.Add(TextBlock.PaddingProperty, TimelineTokenResourceKey.RightMargin);
            }
            else
            {
                labelStyle.Add(TextBlock.PaddingProperty, TimelineTokenResourceKey.LeftMargin);
            }
            
            Add(labelStyle);

            Grid.SetColumn(labelBlock, labelIndex);
            grid.Children.Add(labelBlock);


            var splitPanel = new DockPanel()
            {
                Width = 10,
            };

            var splitHead = new Border()
            {
                Width = 10,
                Height = 10,
                CornerRadius = new CornerRadius(5),
                BorderThickness = new Thickness(3),
                Name = SplitHeadPart,
            };
            var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(SplitHeadPart));

            if (timelineItem.Color.StartsWith("#"))
            {
                try
                {
                    var color = Color.Parse(timelineItem.Color);
                    var brush = new SolidColorBrush(color);
                    contentPresenterStyle.Add(ContentPresenter.BorderBrushProperty, brush);
                }
                catch (Exception)
                {
                    contentPresenterStyle.Add(ContentPresenter.BorderBrushProperty,
                        GlobalTokenResourceKey.ColorPrimary);
                }
            }
            else
            {
                // 转成switch
                switch (timelineItem.Color)
                {
                    case "blue":
                        contentPresenterStyle.Add(ContentPresenter.BorderBrushProperty,
                            GlobalTokenResourceKey.ColorPrimary);
                        break;
                    case "green":
                        contentPresenterStyle.Add(ContentPresenter.BorderBrushProperty,
                            GlobalTokenResourceKey.ColorSuccess);
                        break;
                    case "red":
                        contentPresenterStyle.Add(ContentPresenter.BorderBrushProperty,
                            GlobalTokenResourceKey.ColorError);
                        break;
                    case "gray":
                        contentPresenterStyle.Add(ContentPresenter.BorderBrushProperty,
                            GlobalTokenResourceKey.ColorTextDisabled);
                        break;
                    default:
                        contentPresenterStyle.Add(ContentPresenter.BorderBrushProperty,
                            GlobalTokenResourceKey.ColorPrimary);
                        break;
                }
            }

            contentPresenterStyle.Add(ContentPresenter.BackgroundProperty, TimelineTokenResourceKey.DotBg);
            Add(contentPresenterStyle);

            DockPanel.SetDock(splitHead, Dock.Top);
            splitPanel.Children.Add(splitHead);

            var border = new Border()
            {
                Name = SplitLinePart,
            };
            var borderStyle = new Style(selector => selector.Nesting().Template().Name(SplitLinePart));
            borderStyle.Add(ContentPresenter.BackgroundProperty, TimelineTokenResourceKey.TailColor);
            borderStyle.Add(ContentPresenter.WidthProperty, TimelineTokenResourceKey.TailWidth);
            borderStyle.Add(ContentPresenter.PaddingProperty, TimelineTokenResourceKey.ItemPaddingBottom);
            if (timelineItem.IsLast)
            {
                borderStyle.Add(ContentPresenter.MinHeightProperty, TimelineTokenResourceKey.LastItemContentMinHeight);
            }

            Add(borderStyle);

            splitPanel.Children.Add(border);

            Grid.SetColumn(splitPanel, splitIndex);
            grid.Children.Add(splitPanel);


            var contentPresenter = new ContentPresenter()
            {
                Name = ItemsPresenterPart,
                HorizontalAlignment = contentTextAlign,
            };
            contentPresenter.RegisterInNameScope(scope);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty,
                TimelineItem.ContentProperty);

            contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ItemsPresenterPart));
            contentPresenterStyle.Add(ContentPresenter.FontSizeProperty, TimelineTokenResourceKey.FontSize);
            if (contentIndex == 0)
            {
                contentPresenterStyle.Add(ContentPresenter.MarginProperty, TimelineTokenResourceKey.RightMargin);
            }
            else
            {
                contentPresenterStyle.Add(ContentPresenter.MarginProperty, TimelineTokenResourceKey.LeftMargin);
            }

            Add(contentPresenterStyle);

            Grid.SetColumn(contentPresenter, contentIndex);
            grid.Children.Add(contentPresenter);
            return grid;
        });
    }

    private void CalculateGridIndex(TimelineItem timelineItem, out int labelIndex, out int splitIndex,
        out int contentIndex, out HorizontalAlignment labelTextAlign, out HorizontalAlignment contentTextAlign)
    {
        labelIndex = 0;
        splitIndex = 1;
        contentIndex = 2;
        labelTextAlign = HorizontalAlignment.Right;
        contentTextAlign = HorizontalAlignment.Left;

        if (timelineItem.Mode == "right" || (timelineItem.Mode == "alternate" && timelineItem.Index % 2 == 1))
        {
            labelIndex = 2;
            contentIndex = 0;
            labelTextAlign = HorizontalAlignment.Left;
            contentTextAlign = HorizontalAlignment.Right;
        }

        if (!timelineItem.HasLabel && timelineItem.Mode == "left")
        {
            splitIndex = 0;
            contentIndex = 1;
            contentTextAlign = HorizontalAlignment.Left;
        }

        if (!timelineItem.HasLabel && timelineItem.Mode == "right")
        {
            splitIndex = 1;
            contentIndex = 0;
            contentTextAlign = HorizontalAlignment.Right;
        }
    }

    private ColumnDefinitions CalculateGridColumn(TimelineItem timelineItem, int labelIndex)
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

        if (labelIndex == 0)
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
}