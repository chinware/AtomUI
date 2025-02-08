using AtomUI.IconPkg;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class ProgressBarTheme : AbstractLineProgressTheme
{
    public ProgressBarTheme() : base(typeof(ProgressBar))
    {
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(ProgressBar.ColorTextLabelProperty, SharedTokenKey.ColorTextLabel);
        commonStyle.Add(ProgressBar.ColorTextLightSolidProperty, SharedTokenKey.ColorTextLightSolid);
        Add(commonStyle);
        BuildPercentPositionStyle();
        BuildCompletedIconStyle();
        BuildLabelRotationStyle();
    }

    private void BuildPercentPositionStyle()
    {
        var showProgressInfoStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(AbstractProgressBar.ShowProgressInfoProperty, true));

        // 水平方向
        var horizontalStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(AbstractLineProgress.OrientationProperty, Orientation.Horizontal));
        {
            var notInnerStartStyle = new Style(selector => selector.Nesting().PropertyEquals(
                ProgressBar.PercentPositionProperty, new PercentPosition
                {
                    IsInner   = false,
                    Alignment = LinePercentAlignment.Start
                }));
            {
                var icons = new Style(selector => selector.Nesting().Template().OfType<Icon>());
                icons.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Right);
                notInnerStartStyle.Add(icons);
            }

            horizontalStyle.Add(notInnerStartStyle);

            var notInnerEndStyle = new Style(selector => selector.Nesting().PropertyEquals(
                ProgressBar.PercentPositionProperty, new PercentPosition
                {
                    IsInner   = false,
                    Alignment = LinePercentAlignment.End
                }));
            {
                var icons = new Style(selector => selector.Nesting().Template().OfType<Icon>());
                icons.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Left);
                notInnerEndStyle.Add(icons);
            }
            horizontalStyle.Add(notInnerEndStyle);

            var notInnerCenterStyle = new Style(selector => selector.Nesting().PropertyEquals(
                ProgressBar.PercentPositionProperty, new PercentPosition
                {
                    IsInner   = false,
                    Alignment = LinePercentAlignment.Center
                }));
            {
                var icons = new Style(selector => selector.Nesting().Template().OfType<Icon>());
                icons.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                notInnerCenterStyle.Add(icons);
            }
            horizontalStyle.Add(notInnerCenterStyle);
        }

        showProgressInfoStyle.Add(horizontalStyle);

        // 垂直方向
        var verticalStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(AbstractLineProgress.OrientationProperty, Orientation.Vertical));
        {
            var notInnerStartStyle = new Style(selector => selector.Nesting().PropertyEquals(
                ProgressBar.PercentPositionProperty, new PercentPosition
                {
                    IsInner   = false,
                    Alignment = LinePercentAlignment.Start
                }));
            {
                var icons = new Style(selector => selector.Nesting().Template().OfType<Icon>());
                icons.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Bottom);
                notInnerStartStyle.Add(icons);
            }

            verticalStyle.Add(notInnerStartStyle);

            var notInnerEndStyle = new Style(selector => selector.Nesting().PropertyEquals(
                ProgressBar.PercentPositionProperty, new PercentPosition
                {
                    IsInner   = false,
                    Alignment = LinePercentAlignment.End
                }));
            {
                var icons = new Style(selector => selector.Nesting().Template().OfType<Icon>());
                icons.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Top);
                notInnerEndStyle.Add(icons);
            }
            verticalStyle.Add(notInnerEndStyle);

            var notInnerCenterStyle = new Style(selector => selector.Nesting().PropertyEquals(
                ProgressBar.PercentPositionProperty, new PercentPosition
                {
                    IsInner   = false,
                    Alignment = LinePercentAlignment.Center
                }));
            {
                var icons = new Style(selector => selector.Nesting().Template().OfType<Icon>());
                icons.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center);
                notInnerCenterStyle.Add(icons);
            }
            verticalStyle.Add(notInnerCenterStyle);
        }
        showProgressInfoStyle.Add(verticalStyle);
        Add(showProgressInfoStyle);
    }

    // 如果是 Inner 模式，成功状态下不显示成功 icon
    private void BuildCompletedIconStyle()
    {
        var labelInnerStyle = new Style(selector => selector.Nesting().Class(ProgressBar.PercentLabelInnerPC));
        var icons           = new Style(selector => selector.Nesting().Template().OfType<Icon>());
        icons.Add(Visual.IsVisibleProperty, false);
        labelInnerStyle.Add(icons);
        var labelStyle = new Style(selector => selector.Nesting().Template().OfType<LayoutTransformControl>());
        labelStyle.Add(Visual.IsVisibleProperty, true);
        labelInnerStyle.Add(labelStyle);
        Add(labelInnerStyle);
    }

    private void BuildLabelRotationStyle()
    {
        var rotationStyle = new Style(selector =>
            selector.Nesting().Class(AbstractLineProgress.VerticalPC).Class(ProgressBar.PercentLabelInnerPC));
        var layoutControl = new Style(selector => selector.Nesting().Template().OfType<LayoutTransformControl>());
        layoutControl.Add(LayoutTransformControl.LayoutTransformProperty, new RotateTransform(90));
        rotationStyle.Add(layoutControl);
        Add(rotationStyle);
    }
}