using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Steps;

public class StepsNavigationLayoutTests
{
    static StepsNavigationLayoutTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Theory]
    [InlineData(StepsItemIndicatorType.Default, Orientation.Horizontal)]
    [InlineData(StepsItemIndicatorType.Default, Orientation.Vertical)]
    [InlineData(StepsItemIndicatorType.Dot, Orientation.Horizontal)]
    public void Horizontal_Navigation_Arrow_Aligns_With_Indicator_Center(
        StepsItemIndicatorType indicatorType,
        Orientation labelPlacement)
    {
        var steps = new Desktop.Controls.Steps
        {
            Width             = 760,
            CurrentStep       = 1,
            Style             = StepsStyle.Navigation,
            IsItemClickable   = true,
            ItemIndicatorType = indicatorType,
            LabelPlacement    = labelPlacement
        };
        steps.Items.Add(new StepsItem
        {
            Header      = "Step 1",
            SubHeader   = "00:00:05",
            Description = "This is a description."
        });
        steps.Items.Add(new StepsItem
        {
            Header      = "Step 2",
            SubHeader   = "00:01:02",
            Description = "This is a description."
        });
        steps.Items.Add(new StepsItem
        {
            Header      = "Step 3",
            SubHeader   = "waiting for longlong time",
            Description = "This is a description."
        });

        ShowInWindow(steps, () =>
        {
            var firstItem = steps.GetVisualDescendants()
                                 .OfType<StepsItem>()
                                 .First();
            var indicator = FindTemplatePart(firstItem, "PART_Indicator");
            var navArrow  = FindTemplatePart(firstItem, "NavArrow");

            Math.Abs(CenterY(indicator, firstItem) - CenterY(navArrow, firstItem))
                .ShouldBeLessThanOrEqualTo(1.0);
        });
    }

    [Fact]
    public void Horizontal_Dot_Navigation_Arrow_Fits_Inside_Layout()
    {
        var steps = new Desktop.Controls.Steps
        {
            Width             = 760,
            CurrentStep       = 1,
            Style             = StepsStyle.Navigation,
            IsItemClickable   = true,
            ItemIndicatorType = StepsItemIndicatorType.Dot,
            LabelPlacement    = Orientation.Horizontal
        };
        steps.Items.Add(new StepsItem
        {
            Header      = "Step 1",
            SubHeader   = "00:00:05",
            Description = "This is a description."
        });
        steps.Items.Add(new StepsItem
        {
            Header      = "Step 2",
            SubHeader   = "00:01:02",
            Description = "This is a description."
        });
        steps.Items.Add(new StepsItem
        {
            Header      = "Step 3",
            SubHeader   = "03:01:02",
            Description = "This is a description."
        });

        ShowInWindow(steps, () =>
        {
            var firstItem = steps.GetVisualDescendants()
                                 .OfType<StepsItem>()
                                 .First();
            var navArrow       = FindTemplatePart(firstItem, "NavArrow");
            var navArrowLayout = navArrow.GetVisualParent()!.ShouldBeAssignableTo<Control>();

            navArrowLayout.Bounds.Height.ShouldBeGreaterThanOrEqualTo(navArrow.Bounds.Height);
            navArrowLayout.Bounds.Width.ShouldBeGreaterThanOrEqualTo(navArrow.Bounds.Width);
        });
    }

    [Fact]
    public void Navigation_Arrow_Theme_Uses_Analyzer_Friendly_Size_And_Description_Color()
    {
        var themeSource = ReadRepoFile("src/AtomUI.Desktop.Controls/Steps/Themes/StepsItemTheme.axaml");
        var tokenSource = ReadRepoFile("src/AtomUI.Desktop.Controls/Steps/StepsToken.cs");
        var navArrowStyle = ExtractStyleContaining(themeSource, "Value=\"{atom:StepsTokenResource NavArrowColor}\"");

        themeSource.ShouldNotContain("#PART_Indicator.Bounds.Height");
        navArrowStyle.ShouldContain("Selector=\"^ /template/ atom|Icon#NavArrow\"");
        navArrowStyle.ShouldContain("Value=\"{atom:StepsTokenResource NavArrowColor}\"");
        navArrowStyle.ShouldContain("Property=\"FillBrush\"");
        navArrowStyle.ShouldContain("Property=\"StrokeBrush\"");
        navArrowStyle.ShouldContain("Property=\"FallbackBrush\"");
        tokenSource.ShouldContain("NavArrowColor       = SharedToken.ColorTextDescription;");
    }

    private static Control FindTemplatePart(Control owner, string name)
    {
        return owner.GetVisualDescendants()
                    .OfType<Control>()
                    .Single(control => control.Name == name);
    }

    private static double CenterY(Control target, Visual relativeTo)
    {
        var offset = target.TranslatePoint(default, relativeTo);
        offset.ShouldNotBeNull();
        return offset.Value.Y + target.Bounds.Height / 2;
    }

    private static string ReadRepoFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(candidate))
            {
                return File.ReadAllText(candidate);
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"Could not find repository file: {relativePath}");
    }

    private static string ExtractStyleContaining(string source, string markerText)
    {
        var markerIndex = source.IndexOf(markerText, StringComparison.Ordinal);
        markerIndex.ShouldNotBe(-1);

        var styleStart = source.LastIndexOf("<Style", markerIndex, StringComparison.Ordinal);
        styleStart.ShouldNotBe(-1);

        var styleEnd = source.IndexOf("</Style>", styleStart, StringComparison.Ordinal);
        styleEnd.ShouldNotBe(-1);
        return source[styleStart..styleEnd];
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 900,
            Height  = 260,
            Content = content
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            assertion();
        }
        finally
        {
            window.Close();
        }
    }
}
