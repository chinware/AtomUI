using System.ComponentModel;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class DataGridColumnHeaderTheme : BaseControlTheme
{
    public const string FramePart = "PART_Frame";
    public const string HeaderRootLayoutPart = "PART_HeaderRootLayout";
    public const string VerticalSeparatorPart = "PART_VerticalSeparator";
    public const string ContentDecoratorPart = "PART_ContentDecorator";
    public const string ContentPresenterPart = "PART_ContentPresenter";
    public const string SortIndicatorPart = "PART_SortIndicator";
    public const string FilterIndicatorPart = "PART_FilterIndicator";
    public const string IndicatorsLayoutPart = "PART_IndicatorsLayout";
    public const string FocusVisualPart = "PART_FocusVisual";
    public const string FocusVisualPrimaryPart = "PART_FocusVisualPrimary";
    public const string FocusVisualSecondaryPart = "PART_FocusVisualSecondary";

    public DataGridColumnHeaderTheme()
        : base(typeof(DataGridColumnHeader))
    {
    }

    protected DataGridColumnHeaderTheme(Type targetType)
        : base(targetType)
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<DataGridColumnHeader>((columnHeader, scope) =>
        {
            var headerFrame = new Border
            {
                Name = FramePart
            };
            CreateTemplateParentBinding(headerFrame, Border.BackgroundProperty,
                DataGridColumnHeader.BackgroundProperty);
            CreateTemplateParentBinding(headerFrame, Border.BorderThicknessProperty, DataGridColumnHeader.BorderThicknessProperty);
            CreateTemplateParentBinding(headerFrame, Border.PaddingProperty, DataGridColumnHeader.PaddingProperty);

            var headerRootLayout = new Panel
            {
                Name = HeaderRootLayoutPart
            };

            BuildContentPresenter(headerRootLayout);

            var verticalSeparator = new Rectangle()
            {
                Name                = VerticalSeparatorPart,
                Width               = 1d,
                VerticalAlignment   = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            CreateTemplateParentBinding(verticalSeparator, Rectangle.IsVisibleProperty, DataGridColumnHeader.AreSeparatorsVisibleProperty);
            headerRootLayout.Children.Add(verticalSeparator);

            BuildFocusVisual(headerRootLayout);

            headerFrame.Child = headerRootLayout;
            return headerFrame;
        });
    }

    private void BuildContentPresenter(Panel rootLayout)
    {
        var decorator = new Decorator
        {
            Name = ContentDecoratorPart
        };
        var gridLayout = new Grid
        {
            ColumnDefinitions =
            [
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto),
            ]
        };
        CreateTemplateParentBinding(gridLayout, Grid.HorizontalAlignmentProperty,
            DataGridColumnHeader.HorizontalAlignmentProperty);
        CreateTemplateParentBinding(gridLayout, Grid.VerticalAlignmentProperty,
            DataGridColumnHeader.VerticalAlignmentProperty);
        var contentPresenter = new ContentPresenter()
        {
            Name = ContentPresenterPart,
        };
        CreateTemplateParentBinding(contentPresenter, ContentPresenter.HorizontalContentAlignmentProperty,
            DataGridColumnHeader.HorizontalContentAlignmentProperty);
        CreateTemplateParentBinding(contentPresenter, ContentPresenter.VerticalContentAlignmentProperty,
            DataGridColumnHeader.VerticalContentAlignmentProperty);
        CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty,
            DataGridColumnHeader.ContentProperty);
        CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty,
            DataGridColumnHeader.ContentTemplateProperty);
        Grid.SetColumn(contentPresenter, 0);
        gridLayout.Children.Add(contentPresenter);

        BuildIndicators(gridLayout);
        
        decorator.Child = gridLayout;
        rootLayout.Children.Add(decorator);
    }

    private void BuildIndicators(Grid contentLayout)
    {
        var indicatorsLayout = new StackPanel()
        {
            Name = IndicatorsLayoutPart,
            Orientation = Orientation.Horizontal,
        };
        var sortIndicator = new DataGridSortIndicator
        {
            Name              = SortIndicatorPart,
            VerticalAlignment = VerticalAlignment.Center
        };
        CreateTemplateParentBinding(sortIndicator, DataGridSortIndicator.IsVisibleProperty, DataGridColumnHeader.CanUserSortProperty);
        indicatorsLayout.Children.Add(sortIndicator);

        var filterIndicator = new DataGridFilterIndicator()
        {
            Name              = FilterIndicatorPart,
            VerticalAlignment = VerticalAlignment.Center
        };
        indicatorsLayout.Children.Add(filterIndicator);
 
        Grid.SetColumn(indicatorsLayout, 1);
        contentLayout.Children.Add(indicatorsLayout);
    }

    private void BuildFocusVisual(Panel rootLayout)
    {
        var focusVisualLayout = new Panel()
        {
            Name             = FocusVisualPart,
            IsVisible        = false,
            IsHitTestVisible = false,
        };

        var primaryFocusVisual = new Rectangle
        {
            Name                = FocusVisualPrimaryPart,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment   = VerticalAlignment.Stretch,
            Fill                = Brushes.Transparent,
            IsHitTestVisible    = false,
            StrokeThickness     = 2
        };

        var secondaryFocusVisual = new Rectangle
        {
            Name                = FocusVisualSecondaryPart,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment   = VerticalAlignment.Stretch,
            Fill                = Brushes.Transparent,
            IsHitTestVisible    = false,
            StrokeThickness     = 1
        };
        focusVisualLayout.Children.Add(primaryFocusVisual);
        focusVisualLayout.Children.Add(secondaryFocusVisual);

        rootLayout.Children.Add(focusVisualLayout);
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(DataGridColumnHeader.ForegroundProperty, DataGridTokenKey.TableHeaderTextColor);
        commonStyle.Add(DataGridColumnHeader.BackgroundProperty, DataGridTokenKey.TableHeaderBg);
        commonStyle.Add(DataGridColumnHeader.FontWeightProperty, SharedTokenKey.FontWeightStrong);
        commonStyle.Add(DataGridColumnHeader.HorizontalContentAlignmentProperty, HorizontalAlignment.Left);
        commonStyle.Add(DataGridColumnHeader.VerticalContentAlignmentProperty, VerticalAlignment.Center);
        
        var frameStyle  = new Style(selector => selector.Nesting().Template().Name(FramePart));
        frameStyle.Add(Border.BorderBrushProperty, DataGridTokenKey.TableBorderColor);
        commonStyle.Add(frameStyle);
        
        BuildSizeTypeStyle(commonStyle);
        BuildVerticalSeparatorStyle(commonStyle);
        BuildIndicatorStyle(commonStyle);
        BuildSortableColumnStyle(commonStyle);
        Add(commonStyle);
    }

    private void BuildSortableColumnStyle(Style commonStyle)
    {
        var canUserSortStyle = new Style(selector => selector.Nesting().PropertyEquals(DataGridColumnHeader.CanUserSortProperty, true));
        var sortActivatedStyle = new Style(selector => selector.Nesting().Not(x => x.PropertyEquals(DataGridColumnHeader.CurrentSortingStateProperty, null)));
        {
            var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
            frameStyle.Add(Border.BackgroundProperty, DataGridTokenKey.HeaderSortActiveBg);
            sortActivatedStyle.Add(frameStyle);
        }
        canUserSortStyle.Add(sortActivatedStyle);

        var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
        {
            var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
            frameStyle.Add(Border.BackgroundProperty, DataGridTokenKey.HeaderSortHoverBg);
            hoverStyle.Add(frameStyle);
        }
        canUserSortStyle.Add(hoverStyle);
        commonStyle.Add(canUserSortStyle);
    }

    private void BuildIndicatorStyle(Style commonStyle)
    {
        var indicatorsLayout = new Style(selector => selector.Nesting().Template().Name(IndicatorsLayoutPart));
        indicatorsLayout.Add(StackPanel.SpacingProperty, SharedTokenKey.MarginXXS);
        commonStyle.Add(indicatorsLayout);
        {
            var sortIndicatorStyle = new Style(selector => selector.Nesting().Template().Name(SortIndicatorPart));
            sortIndicatorStyle.Add(DataGridSortIndicator.MarginProperty, DataGridTokenKey.SortIndicatorMargin);
            commonStyle.Add(sortIndicatorStyle);
        }
        
        var ascendingStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.SortAscending));
        {
            var sortIndicatorStyle = new Style(selector => selector.Nesting().Template().Name(SortIndicatorPart));
            sortIndicatorStyle.Add(DataGridSortIndicator.CurrentSortDirectionProperty, ListSortDirection.Ascending);
            ascendingStyle.Add(sortIndicatorStyle);
        }
        commonStyle.Add(ascendingStyle);
        
        var descendingStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.SortDescending));
        {
            var sortIndicatorStyle = new Style(selector => selector.Nesting().Template().Name(SortIndicatorPart));
            sortIndicatorStyle.Add(DataGridSortIndicator.CurrentSortDirectionProperty, ListSortDirection.Descending);
            descendingStyle.Add(sortIndicatorStyle);
        }
        commonStyle.Add(descendingStyle);
        
        var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
        {
            var sortIndicatorStyle = new Style(selector => selector.Nesting().Template().Name(SortIndicatorPart));
            sortIndicatorStyle.Add(DataGridSortIndicator.IsHoverModeProperty, true);
            hoverStyle.Add(sortIndicatorStyle);
        }
        commonStyle.Add(hoverStyle);
    }

    private void BuildVerticalSeparatorStyle(Style commonStyle)
    {
        var separatorStyle = new Style(selector => selector.Nesting().Template().Name(VerticalSeparatorPart));
        separatorStyle.Add(Rectangle.FillProperty, DataGridTokenKey.HeaderSplitColor);
        separatorStyle.Add(Rectangle.HeightProperty, SharedTokenKey.FontHeight);
        commonStyle.Add(separatorStyle);
    }

    private void BuildSizeTypeStyle(Style commonStyle)
    {
        var largeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(DataGridColumnHeader.SizeTypeProperty, SizeType.Large));
        {
            var contentDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(ContentDecoratorPart));
            contentDecoratorStyle.Add(Decorator.PaddingProperty, DataGridTokenKey.TablePadding);
            largeStyle.Add(contentDecoratorStyle);
        }
        largeStyle.Add(DataGridColumnHeader.FontSizeProperty, DataGridTokenKey.TableFontSize);
        commonStyle.Add(largeStyle);

        var middleStyle =
            new Style(selector => selector.Nesting().PropertyEquals(DataGridColumnHeader.SizeTypeProperty, SizeType.Middle));
        {
            var contentDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(ContentDecoratorPart));
            contentDecoratorStyle.Add(Decorator.PaddingProperty, DataGridTokenKey.TablePaddingMiddle);
            middleStyle.Add(contentDecoratorStyle);
        }
        middleStyle.Add(DataGridColumnHeader.FontSizeProperty, DataGridTokenKey.TableFontSizeMiddle);
        commonStyle.Add(middleStyle);

        var smallStyle =
            new Style(selector => selector.Nesting().PropertyEquals(DataGridColumnHeader.SizeTypeProperty, SizeType.Small));
        {
            var contentDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(ContentDecoratorPart));
            contentDecoratorStyle.Add(Decorator.PaddingProperty, DataGridTokenKey.TablePaddingSmall);
            smallStyle.Add(contentDecoratorStyle);
        }
        smallStyle.Add(DataGridColumnHeader.FontSizeProperty, DataGridTokenKey.TableFontSizeSmall);
        commonStyle.Add(smallStyle);
    }
}