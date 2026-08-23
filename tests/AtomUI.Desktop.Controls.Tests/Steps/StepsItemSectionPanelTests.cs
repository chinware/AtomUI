using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Steps;

public class StepsItemSectionPanelTests
{
    static StepsItemSectionPanelTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Horizontal_Body_Places_Header_Subheader_And_Content_Without_Overlap()
    {
        var section = CreateSection(
            Desktop.Controls.StepsType.Default, Orientation.Horizontal, Orientation.Horizontal);
        var header    = AddChild(section, Desktop.Controls.StepsItemLayoutRole.Header, 60, 20);
        var subHeader = AddChild(section, Desktop.Controls.StepsItemLayoutRole.SubHeader, 40, 12);
        var content   = AddChild(section, Desktop.Controls.StepsItemLayoutRole.Content, 80, 24);

        section.Measure(new Size(300, 100));
        section.DesiredSize.ShouldBe(new Size(100, 44));
        section.HeadingHeight.ShouldBe(20);
        section.HasVisibleBody.ShouldBeTrue();

        section.Arrange(new Rect(0, 0, 100, 44));

        header.Bounds.ShouldBe(new Rect(0, 0, 60, 20));
        subHeader.Bounds.ShouldBe(new Rect(60, 4, 40, 12));
        content.Bounds.ShouldBe(new Rect(0, 20, 80, 24));
        section.HeadingLineRight.ShouldBe(100);
    }

    [Fact]
    public void Horizontal_Body_Wraps_The_Subheader_When_The_Heading_Does_Not_Fit()
    {
        var section = CreateSection(
            Desktop.Controls.StepsType.Default, Orientation.Horizontal, Orientation.Horizontal);
        var header    = AddChild(section, Desktop.Controls.StepsItemLayoutRole.Header, 60, 20);
        var subHeader = AddChild(section, Desktop.Controls.StepsItemLayoutRole.SubHeader, 40, 12);
        var content   = AddChild(section, Desktop.Controls.StepsItemLayoutRole.Content, 80, 24);

        section.Measure(new Size(80, 100));
        section.DesiredSize.ShouldBe(new Size(80, 56));
        section.HeadingHeight.ShouldBe(32);
        section.HasVisibleBody.ShouldBeTrue();

        section.Arrange(new Rect(0, 0, 80, 56));

        header.Bounds.ShouldBe(new Rect(0, 0, 60, 20));
        subHeader.Bounds.ShouldBe(new Rect(0, 20, 40, 12));
        content.Bounds.ShouldBe(new Rect(0, 32, 80, 24));
        section.HeadingLineRight.ShouldBe(60);
    }

    [Fact]
    public void Vertical_Centered_Body_Stacks_And_Centers_Each_Line()
    {
        var section = CreateSection(
            Desktop.Controls.StepsType.Dot, Orientation.Horizontal, Orientation.Horizontal);
        var header    = AddChild(section, Desktop.Controls.StepsItemLayoutRole.Header, 60, 20);
        var subHeader = AddChild(section, Desktop.Controls.StepsItemLayoutRole.SubHeader, 40, 12);
        var content   = AddChild(section, Desktop.Controls.StepsItemLayoutRole.Content, 80, 24);

        section.Measure(new Size(300, 100));
        section.DesiredSize.ShouldBe(new Size(80, 56));
        section.HasVisibleBody.ShouldBeTrue();

        section.Arrange(new Rect(0, 0, 80, 56));

        header.Bounds.ShouldBe(new Rect(10, 0, 60, 20));
        subHeader.Bounds.ShouldBe(new Rect(20, 20, 40, 12));
        content.Bounds.ShouldBe(new Rect(0, 32, 80, 24));
    }

    [Fact]
    public void Hidden_Body_Children_Are_Excluded_From_Measure_And_Arrange()
    {
        var section = CreateSection(
            Desktop.Controls.StepsType.Default, Orientation.Horizontal, Orientation.Horizontal);
        AddChild(section, Desktop.Controls.StepsItemLayoutRole.Header, 60, 20);
        var subHeader = AddChild(section, Desktop.Controls.StepsItemLayoutRole.SubHeader, 40, 12);
        var content   = AddChild(section, Desktop.Controls.StepsItemLayoutRole.Content, 80, 24);
        subHeader.IsVisible = false;

        section.Measure(new Size(300, 100));
        section.DesiredSize.ShouldBe(new Size(80, 44));
        section.HeadingHeight.ShouldBe(20);
        section.HasVisibleBody.ShouldBeTrue();

        section.Arrange(new Rect(0, 0, 80, 44));

        subHeader.Bounds.ShouldBe(default);
        content.Bounds.ShouldBe(new Rect(0, 20, 80, 24));
        section.HeadingLineRight.ShouldBe(60);
    }

    private static Desktop.Controls.StepsItemSectionPanel CreateSection(
        Desktop.Controls.StepsType type,
        Orientation orientation,
        Orientation titlePlacement)
    {
        return new Desktop.Controls.StepsItemSectionPanel
        {
            Type           = type,
            Orientation    = orientation,
            TitlePlacement = titlePlacement
        };
    }

    private static FixedSizeControl AddChild(
        Desktop.Controls.StepsItemSectionPanel section,
        Desktop.Controls.StepsItemLayoutRole role,
        double width,
        double height)
    {
        var child = new FixedSizeControl(width, height);
        Desktop.Controls.StepsItemLayoutPanel.SetRole(child, role);
        section.Children.Add(child);
        return child;
    }

    private sealed class FixedSizeControl(double width, double height) : Control
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(width, height);
        }
    }
}
