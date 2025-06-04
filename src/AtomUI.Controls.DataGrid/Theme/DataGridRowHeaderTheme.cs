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
internal class DataGridRowHeaderTheme : BaseControlTheme
{
    public const string RootLayoutPart = "PART_RootLayout";
    public const string RowInvalidVisualElementPart = "PART_RowInvalidVisualElement";
    public const string RowBackgroundPart = "PART_RowBackground";
    public const string HorizontalSeparatorPart = "PART_HorizontalSeparator";

    public DataGridRowHeaderTheme()
        : base(typeof(DataGridRowHeader))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<DataGridRowHeader>((rowHeader, scope) =>
        {
            var rootLayout = new Grid()
            {
                Name = RootLayoutPart,
                RowDefinitions = [
                    new RowDefinition(GridLength.Star),
                    new RowDefinition(GridLength.Star),
                    new RowDefinition(GridLength.Auto)
                ],
                ColumnDefinitions = [
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(GridLength.Star),
                ]
            };
            BuildHeaderDecorator(rootLayout);

            var horizontalSeparator = new Rectangle()
            {
                Name                = HorizontalSeparatorPart,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            Grid.SetRow(horizontalSeparator, 2);
            Grid.SetColumn(horizontalSeparator, 0);
            Grid.SetColumnSpan(horizontalSeparator, 2);
            rootLayout.Children.Add(horizontalSeparator);

            var contentPresenter = new ContentPresenter();
            Grid.SetRow(contentPresenter, 0);
            Grid.SetColumn(contentPresenter, 1);
            Grid.SetRowSpan(horizontalSeparator, 2);
            rootLayout.Children.Add(contentPresenter);
            
            return rootLayout;
        });
    }
    
    private void BuildHeaderDecorator(Grid rootLayout)
    {
        var frame = new Border();
        Grid.SetRow(frame, 0);
        Grid.SetColumn(frame, 0);
        Grid.SetColumnSpan(frame, 2);

        var layout                  = new Panel();
        var rowInvalidVisualElement = new Rectangle
        {
            Name    = RowInvalidVisualElementPart,
            Opacity = 0,
            Stretch = Stretch.Fill
        };
        layout.Children.Add(rowInvalidVisualElement);

        var rowBackgroundPart = new Rectangle
        {
            Name    = RowBackgroundPart,
            Stretch = Stretch.Fill
        };
        layout.Children.Add(rowBackgroundPart);
        frame.Child = layout;
        
        rootLayout.Children.Add(frame);
    }
}