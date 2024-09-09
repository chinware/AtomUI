using AtomUI.Theme.Styling;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class StepsProgressBarTheme : AbstractLineProgressTheme
{
    public StepsProgressBarTheme() : base(typeof(StepsProgressBar))
    {
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();
        BuildCommonStyle();
        BuildPercentPositionStyle();
    }

    private void BuildCommonStyle()
    {
        var commonStyle     = new Style(selector => selector.Nesting());
        var horizontalStyle = new Style(selector => selector.Nesting().Class(AbstractLineProgress.HorizontalPC));
        horizontalStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Left);
        var verticalStyle = new Style(selector => selector.Nesting().Class(AbstractLineProgress.VerticalPC));
        verticalStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Top);
        commonStyle.Add(horizontalStyle);
        commonStyle.Add(verticalStyle);
        Add(commonStyle);
    }

    private void BuildPercentPositionStyle()
    {
        var showProgressInfoStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(AbstractProgressBar.ShowProgressInfoProperty, true));

        // 水平方向
        var horizontalStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(AbstractLineProgress.OrientationProperty, Orientation.Horizontal));
        {
            var startStyle = new Style(selector =>
                selector.Nesting()
                    .PropertyEquals(StepsProgressBar.PercentPositionProperty, LinePercentAlignment.Start));
            {
                var icons = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
                icons.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Right);
                startStyle.Add(icons);
            }

            horizontalStyle.Add(startStyle);

            var endStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(StepsProgressBar.PercentPositionProperty, LinePercentAlignment.End));
            {
                var icons = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
                icons.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Left);
                endStyle.Add(icons);
            }
            horizontalStyle.Add(endStyle);

            var centerStyle = new Style(selector =>
                selector.Nesting()
                    .PropertyEquals(StepsProgressBar.PercentPositionProperty, LinePercentAlignment.Center));
            {
                var icons = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
                icons.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                centerStyle.Add(icons);
            }
            horizontalStyle.Add(centerStyle);
        }

        showProgressInfoStyle.Add(horizontalStyle);

        // 垂直方向
        var verticalStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(AbstractLineProgress.OrientationProperty, Orientation.Vertical));
        {
            var startStyle = new Style(selector =>
                selector.Nesting()
                    .PropertyEquals(StepsProgressBar.PercentPositionProperty, LinePercentAlignment.Start));
            {
                var icons = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
                icons.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Bottom);
                startStyle.Add(icons);
            }

            verticalStyle.Add(startStyle);

            var endStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(StepsProgressBar.PercentPositionProperty, LinePercentAlignment.End));
            {
                var icons = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
                icons.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Top);
                endStyle.Add(icons);
            }
            verticalStyle.Add(endStyle);

            var centerStyle = new Style(selector =>
                selector.Nesting()
                    .PropertyEquals(StepsProgressBar.PercentPositionProperty, LinePercentAlignment.Center));
            {
                var icons = new Style(selector => selector.Nesting().Template().OfType<PathIcon>());
                icons.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center);
                centerStyle.Add(icons);
            }
            verticalStyle.Add(centerStyle);
        }
        showProgressInfoStyle.Add(verticalStyle);

        Add(showProgressInfoStyle);
    }
}