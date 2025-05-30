using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;

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
        var contentPresenterLayout = new Panel();
        var gridLayout = new Grid()
        {
            ColumnDefinitions =
            [
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto),
            ]
        };

        var contentPresenter = new ContentPresenter()
        {
            Name = ContentPresenterPart,
        };
        CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty, DataGridColumnHeader.ContentProperty);
        CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty, DataGridColumnHeader.ContentTemplateProperty);
        Grid.SetColumn(contentPresenter, 0);
        gridLayout.Children.Add(contentPresenter);

        var iconPresenter = new ContentPresenter()
        {
            Name                = SortIconPresenterPart,
            IsVisible           = false,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment   = VerticalAlignment.Center,
        };
        Grid.SetColumn(iconPresenter, 1);
        gridLayout.Children.Add(iconPresenter);

        contentPresenterLayout.Children.Add(gridLayout);
        Grid.SetColumn(rootLayout, 0);
        rootLayout.Children.Add(contentPresenterLayout);
    }

    private void BuildFocusVisual(Grid rootLayout)
    {
        var focusVisualLayout = new Grid()
        {
            IsVisible        = false,
            Name             = FocusVisualPart,
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
}