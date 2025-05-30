using AtomUI.IconPkg;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class DataGridColumnHeaderTheme : BaseControlTheme
{
    public const string HeaderFramePart = "PART_HeaderFrame";
    public const string HeaderRootLayoutPart = "PART_HeaderRootLayout";
    public const string VerticalSeparatorPart = "PART_VerticalSeparator";
    public const string ContentPresenterPart = "PART_ContentPresenter";
    public const string SortIconPresenterPart = "PART_SortIconPresenter";
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
            var headerFrame = new Border()
            {
                Name = HeaderFramePart
            };
            CreateTemplateParentBinding(headerFrame, Border.BackgroundProperty, DataGridColumnHeader.BackgroundProperty);

            var headerRootLayout = new Grid
            {
                Name = HeaderRootLayoutPart,
                ColumnDefinitions =
                [
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto),
                ]
            };

            BuildContentPresenter(headerRootLayout);

            var verticalSeparator = new Rectangle()
            {
                Name              = VerticalSeparatorPart,
                Width             = 1,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            Grid.SetColumn(verticalSeparator, 1);
            headerRootLayout.Children.Add(verticalSeparator);

            BuildFocusVisual(headerRootLayout);

            headerFrame.Child = headerRootLayout;
            return headerFrame;
        });
    }

    private void BuildContentPresenter(Grid rootLayout)
    {
        var gridLayout = new Grid()
        {
            ColumnDefinitions =
            [
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto),
            ]
        };
        CreateTemplateParentBinding(gridLayout, Grid.HorizontalAlignmentProperty, DataGridColumnHeader.HorizontalAlignmentProperty);
        CreateTemplateParentBinding(gridLayout, Grid.VerticalAlignmentProperty, DataGridColumnHeader.VerticalAlignmentProperty);
        var contentPresenter = new ContentPresenter()
        {
            Name = ContentPresenterPart,
        };
        CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty, DataGridColumnHeader.ContentProperty);
        CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty, DataGridColumnHeader.ContentTemplateProperty);
        Grid.SetColumn(contentPresenter, 0);
        gridLayout.Children.Add(contentPresenter);

        var iconPresenter = new IconPresenter()
        {
            Name                = SortIconPresenterPart,
            IsVisible           = false
        };
        Grid.SetColumn(iconPresenter, 1);
        gridLayout.Children.Add(iconPresenter);
        
        Grid.SetColumn(gridLayout, 0);
        rootLayout.Children.Add(gridLayout);
    }

    private void BuildFocusVisual(Grid rootLayout)
    {
        var focusVisualLayout = new Panel()
        {
            Name      = FocusVisualPart,
            IsVisible = false,
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

        Grid.SetColumn(focusVisualLayout, 0);
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
        Add(commonStyle);
    }
}