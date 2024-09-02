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

    public TimelineItemTheme() : this(typeof(TimelineItem))
    {
    }

    protected TimelineItemTheme(Type targetType) : base(targetType)
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<TimelineItem>((timelineItem, scope) =>
        {
            var columnDefinition = new ColumnDefinitions()
            {
                new ColumnDefinition(new GridLength(10)),
                new ColumnDefinition(GridLength.Star)
            };
            if (timelineItem.HasLabel)
            {
                columnDefinition = new ColumnDefinitions()
                {
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(new GridLength(10)),
                    new ColumnDefinition(GridLength.Star)
                };
            }
            var grid = new Grid()
            {
                Name = GridPart,
                ColumnDefinitions = columnDefinition,
                RowDefinitions =
                {
                    new RowDefinition(GridLength.Star),
                },
            };
            
            if (timelineItem.HasLabel)
            {
                var labelBlock = new TextBlock()
                {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Right,
                };

                CreateTemplateParentBinding(labelBlock, TextBlock.TextProperty, TimelineItem.LabelProperty);

                Grid.SetColumn(labelBlock, 0);
                grid.Children.Add(labelBlock);
            }
            
            
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
                    contentPresenterStyle.Add(ContentPresenter.BorderBrushProperty, color);
                }
                catch (Exception)
                {
                    contentPresenterStyle.Add(ContentPresenter.BorderBrushProperty, GlobalTokenResourceKey.ColorPrimary);
                }
            }
            else
            {
                // 转成switch
                switch (timelineItem.Color)
                {
                    case "blue":
                        contentPresenterStyle.Add(ContentPresenter.BorderBrushProperty, GlobalTokenResourceKey.ColorPrimary);
                        break;
                    case "green":
                        contentPresenterStyle.Add(ContentPresenter.BorderBrushProperty, GlobalTokenResourceKey.ColorSuccess);
                        break;
                    case "red":
                        contentPresenterStyle.Add(ContentPresenter.BorderBrushProperty, GlobalTokenResourceKey.ColorError);
                        break;
                    case "gray":
                        contentPresenterStyle.Add(ContentPresenter.BorderBrushProperty, GlobalTokenResourceKey.ColorTextDisabled);
                        break;
                    default:
                        contentPresenterStyle.Add(ContentPresenter.BorderBrushProperty, GlobalTokenResourceKey.ColorPrimary);
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
            contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(SplitLinePart));
            contentPresenterStyle.Add(ContentPresenter.BackgroundProperty, TimelineTokenResourceKey.TailColor);
            contentPresenterStyle.Add(ContentPresenter.WidthProperty, TimelineTokenResourceKey.TailWidth);
            Add(contentPresenterStyle);

            splitPanel.Children.Add(border);

            Grid.SetColumn(splitPanel, timelineItem.HasLabel ? 1 : 0);
            grid.Children.Add(splitPanel);


            var contentPresenter = new ContentPresenter()
            {
                Name = ItemsPresenterPart,
            };
            contentPresenter.RegisterInNameScope(scope);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty,
                TimelineItem.ContentProperty);

            contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ItemsPresenterPart));
            contentPresenterStyle.Add(ContentPresenter.MarginProperty, TimelineTokenResourceKey.ContentMargin);
            Add(contentPresenterStyle);

            Grid.SetColumn(contentPresenter, timelineItem.HasLabel ? 2 : 1);
            grid.Children.Add(contentPresenter);
            return grid;
        });
    }
}