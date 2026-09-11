using AtomUI.Controls.Tests.ImageLoading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;

namespace AtomUI.Controls.Tests.Avatar;

public class AvatarTextLayoutTests
{
    static AvatarTextLayoutTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Scaled_Text_Remains_Horizontally_Centered_When_Its_Natural_Width_Is_Less_Than_The_Avatar()
    {
        var avatar = new global::AtomUI.Controls.Avatar
        {
            Size = 40,
            FontSize = 9,
            Text = "USER"
        };
        using var host = new ImageControlTestHost(avatar);
        var textPresenter = GetTextPresenter(avatar);
        var availableWidth = avatar.Bounds.Width - avatar.Gap * 2;

        textPresenter.Bounds.Width.ShouldBeGreaterThan(availableWidth);
        textPresenter.Bounds.Width.ShouldBeLessThan(avatar.Bounds.Width);

        var (left, right) = GetRenderedHorizontalBounds(textPresenter, avatar);

        ((left + right) / 2).ShouldBe(avatar.Bounds.Width / 2, 0.01);
        (right - left).ShouldBe(availableWidth, 0.01);
    }

    [Fact]
    public void Short_Text_Is_Centered_Without_Being_Upscaled()
    {
        var avatar = new global::AtomUI.Controls.Avatar
        {
            Size = 40,
            FontSize = 9,
            Text = "U"
        };
        using var host = new ImageControlTestHost(avatar);
        var textPresenter = GetTextPresenter(avatar);
        var (left, right) = GetRenderedHorizontalBounds(textPresenter, avatar);

        ((left + right) / 2).ShouldBe(avatar.Bounds.Width / 2, 0.51);
        (right - left).ShouldBe(textPresenter.Bounds.Width, 0.01);
    }

    [Fact]
    public void Long_Text_Is_Centered_And_Scaled_Down_To_The_Available_Width()
    {
        var avatar = new global::AtomUI.Controls.Avatar
        {
            Size = 40,
            FontSize = 9,
            Text = "ABCDEFGHIJK"
        };
        using var host = new ImageControlTestHost(avatar);
        var textPresenter = GetTextPresenter(avatar);
        var availableWidth = avatar.Bounds.Width - avatar.Gap * 2;
        var (left, right) = GetRenderedHorizontalBounds(textPresenter, avatar);

        textPresenter.Bounds.Width.ShouldBeGreaterThan(avatar.Bounds.Width);
        ((left + right) / 2).ShouldBe(avatar.Bounds.Width / 2, 0.01);
        (right - left).ShouldBe(availableWidth, 0.01);
    }

    [Fact]
    public void Changing_Gap_Updates_The_Scaled_Width_Without_Moving_The_Text_Center()
    {
        var avatar = new global::AtomUI.Controls.Avatar
        {
            Size = 40,
            FontSize = 9,
            Text = "ABCDEFGHIJK"
        };
        using var host = new ImageControlTestHost(avatar);
        var textPresenter = GetTextPresenter(avatar);

        avatar.Gap = 8;
        Dispatcher.UIThread.RunJobs();
        var (left, right) = GetRenderedHorizontalBounds(textPresenter, avatar);

        ((left + right) / 2).ShouldBe(avatar.Bounds.Width / 2, 0.01);
        (right - left).ShouldBe(avatar.Bounds.Width - avatar.Gap * 2, 0.01);
    }

    private static TextBlock GetTextPresenter(global::AtomUI.Controls.Avatar avatar)
    {
        return avatar.GetVisualDescendants()
                     .OfType<TextBlock>()
                     .Single(control => control.Name == "PART_TextPresenter");
    }

    private static (double Left, double Right) GetRenderedHorizontalBounds(
        TextBlock textPresenter,
        global::AtomUI.Controls.Avatar avatar)
    {
        var transform = textPresenter.TransformToVisual(avatar);
        transform.ShouldNotBeNull();
        var left  = transform.Value.Transform(default).X;
        var right = transform.Value.Transform(new Point(textPresenter.Bounds.Width, 0)).X;
        return (left, right);
    }
}
