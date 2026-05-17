using AtomUI.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using AtomGroupBox = AtomUI.Desktop.Controls.GroupBox;
using AtomGroupBoxTitlePosition = AtomUI.Desktop.Controls.GroupBoxTitlePosition;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunGroupBoxStateVerification()
    {
        var failures = new List<string>();
        VerifyGroupBoxHeaderIconPresenterLifecycle(failures);
        VerifyGroupBoxHeaderStyleRuntimeSync(failures);
        VerifyGroupBoxHeaderPositionRuntimeSync(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("GroupBox state verification passed.");
            return true;
        }

        Console.Error.WriteLine("GroupBox state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyGroupBoxHeaderIconPresenterLifecycle(ICollection<string> failures)
    {
        var groupBox = CreateVerificationGroupBox();
        using var realized = RealizeControl(groupBox);

        Expect(FindVisualByName<IconPresenter>(groupBox, "PART_HeaderIconPresenter") == null,
            "GroupBox without HeaderIcon should not create PART_HeaderIconPresenter.",
            failures);

        var firstIcon = new GithubOutlined();
        groupBox.HeaderIcon = firstIcon;
        RefreshLayout(realized.Window);
        var firstPresenter = FindVisualByName<IconPresenter>(groupBox, "PART_HeaderIconPresenter");
        Expect(firstPresenter != null,
            "GroupBox should create PART_HeaderIconPresenter when HeaderIcon is assigned.",
            failures);
        Expect(ReferenceEquals(firstPresenter?.Icon, firstIcon),
            "GroupBox header icon presenter should receive HeaderIcon.",
            failures);
        Expect(firstPresenter is { VerticalAlignment: VerticalAlignment.Center },
            $"GroupBox header icon presenter should keep template VerticalAlignment=Center, actual {firstPresenter?.VerticalAlignment}.",
            failures);
        Expect(firstPresenter is { Width: > 0, Height: > 0 },
            $"GroupBox header icon presenter should keep template token size, actual {firstPresenter?.Width}x{firstPresenter?.Height}.",
            failures);
        Expect(firstPresenter?.Margin.Right > 0,
            $"GroupBox header icon presenter should keep template token margin, actual {firstPresenter?.Margin}.",
            failures);
        Expect(firstPresenter?.IconBrush is not null,
            "GroupBox header icon presenter should keep template token IconBrush.",
            failures);

        var secondIcon = new SettingOutlined();
        groupBox.HeaderIcon = secondIcon;
        RefreshLayout(realized.Window);
        var secondPresenter = FindVisualByName<IconPresenter>(groupBox, "PART_HeaderIconPresenter");
        Expect(ReferenceEquals(firstPresenter, secondPresenter),
            "GroupBox should reuse the header icon presenter when HeaderIcon changes.",
            failures);
        Expect(ReferenceEquals(secondPresenter?.Icon, secondIcon),
            "GroupBox header icon presenter should update to the new HeaderIcon.",
            failures);
        Expect(firstIcon.GetVisualParent() == null,
            "Replacing GroupBox HeaderIcon should detach the old icon visual parent.",
            failures);

        groupBox.HeaderIcon = null;
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<IconPresenter>(groupBox, "PART_HeaderIconPresenter") == null,
            "GroupBox should remove PART_HeaderIconPresenter when HeaderIcon is cleared.",
            failures);
        Expect(firstPresenter?.GetVisualParent() == null,
            "Removed GroupBox header icon presenter should not keep a visual parent.",
            failures);
        Expect(firstPresenter == null || firstPresenter.TemplatedParent == null,
            "Removed GroupBox header icon presenter should clear templated parent.",
            failures);
        Expect(secondIcon.GetVisualParent() == null,
            "Clearing GroupBox HeaderIcon should detach the active icon visual parent.",
            failures);

        groupBox.HeaderIcon = new GithubOutlined();
        RefreshLayout(realized.Window);
        var recreatedPresenter = FindVisualByName<IconPresenter>(groupBox, "PART_HeaderIconPresenter");
        Expect(recreatedPresenter != null && !ReferenceEquals(firstPresenter, recreatedPresenter),
            "GroupBox should recreate the header icon presenter cleanly after it was removed.",
            failures);
    }

    private static void VerifyGroupBoxHeaderStyleRuntimeSync(ICollection<string> failures)
    {
        var groupBox = CreateVerificationGroupBox();
        using var realized = RealizeControl(groupBox);
        var header = FindVisualByName<TextBlock>(groupBox, "PART_HeaderPresenter");

        groupBox.HeaderTitle = "Styled title";
        groupBox.HeaderTitleColor = Brushes.Coral;
        groupBox.HeaderFontSize = 18;
        groupBox.HeaderFontStyle = FontStyle.Italic;
        groupBox.HeaderFontWeight = FontWeight.Bold;
        RefreshLayout(realized.Window);

        Expect(header?.Text == "Styled title",
            $"GroupBox header text should follow HeaderTitle, actual {header?.Text ?? "<null>"}.",
            failures);
        Expect(BrushEquals(header?.Foreground, Brushes.Coral),
            $"GroupBox header foreground should follow HeaderTitleColor, actual {DescribeBrush(header?.Foreground)}.",
            failures);
        Expect(Math.Abs((header?.FontSize ?? 0) - 18) < 0.001,
            $"GroupBox header font size should follow HeaderFontSize, actual {header?.FontSize}.",
            failures);
        Expect(header?.FontStyle == FontStyle.Italic,
            $"GroupBox header font style should follow HeaderFontStyle, actual {header?.FontStyle}.",
            failures);
        Expect(header?.FontWeight == FontWeight.Bold,
            $"GroupBox header font weight should follow HeaderFontWeight, actual {header?.FontWeight}.",
            failures);
    }

    private static void VerifyGroupBoxHeaderPositionRuntimeSync(ICollection<string> failures)
    {
        var groupBox = CreateVerificationGroupBox();
        using var realized = RealizeControl(groupBox);
        var headerContent = FindVisualByName<Decorator>(groupBox, "PART_HeaderContent");

        groupBox.HeaderTitlePosition = AtomGroupBoxTitlePosition.Center;
        RefreshLayout(realized.Window);
        Expect(headerContent?.HorizontalAlignment == HorizontalAlignment.Center,
            $"GroupBox HeaderTitlePosition=Center should center header content, actual {headerContent?.HorizontalAlignment}.",
            failures);

        groupBox.HeaderTitlePosition = AtomGroupBoxTitlePosition.Right;
        RefreshLayout(realized.Window);
        Expect(headerContent?.HorizontalAlignment == HorizontalAlignment.Right,
            $"GroupBox HeaderTitlePosition=Right should right-align header content, actual {headerContent?.HorizontalAlignment}.",
            failures);

        groupBox.HeaderTitlePosition = AtomGroupBoxTitlePosition.Left;
        RefreshLayout(realized.Window);
        Expect(headerContent?.HorizontalAlignment == HorizontalAlignment.Left,
            $"GroupBox HeaderTitlePosition=Left should left-align header content, actual {headerContent?.HorizontalAlignment}.",
            failures);
    }

    private static AtomGroupBox CreateVerificationGroupBox()
    {
        return new AtomGroupBox
        {
            HeaderTitle = "Title Info",
            Width       = 420,
            Content     = new Panel
            {
                Height = 40
            }
        };
    }
}
