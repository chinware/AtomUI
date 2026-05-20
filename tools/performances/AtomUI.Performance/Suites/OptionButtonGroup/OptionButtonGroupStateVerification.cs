using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunOptionButtonGroupStateVerification()
    {
        var failures = new List<string>();
        VerifyOptionButtonRuntimeSlots(failures);
        VerifyOptionButtonSelectionAndPosition(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("OptionButtonGroup state verification passed.");
            return true;
        }

        Console.Error.WriteLine("OptionButtonGroup state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyOptionButtonRuntimeSlots(ICollection<string> failures)
    {
        var group = new OptionButtonGroup
        {
            ButtonStyle = OptionButtonStyle.Outline
        };
        var first = new OptionButton
        {
            Content   = "Apple",
            IsChecked = true
        };
        group.Items.Add(first);
        group.Items.Add(new OptionButton { Content = "Pear" });
        group.Items.Add(new OptionButton { Content = "Orange" });

        using var realized = RealizeControl(group);
        Expect(FindVisualByName<IconPresenter>(first, "IconPresenter") == null,
            "Text-only OptionButton should not create IconPresenter.",
            failures);
        Expect(CountVisualByTypeName(first, "WaveSpiritDecorator") == 0,
            "Idle OptionButton should not create WaveSpiritDecorator.",
            failures);

        first.SetCurrentValue(OptionButton.IconProperty, new AppleOutlined());
        RefreshLayout(realized.Window);
        var iconPresenter = FindVisualByName<IconPresenter>(first, "IconPresenter");
        Expect(iconPresenter != null,
            "OptionButton should create IconPresenter when Icon is assigned.",
            failures);
        Expect((iconPresenter?.GetVisualParent() as Control)?.Name == "ContentLayout",
            "OptionButton IconPresenter should be attached to ContentLayout.",
            failures);
        Expect(iconPresenter is null || iconPresenter.Width > 0,
            "OptionButton IconPresenter should keep size selector behavior.",
            failures);

        first.SetCurrentValue(OptionButton.IconProperty, null);
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<IconPresenter>(first, "IconPresenter") == null,
            "OptionButton should remove IconPresenter when Icon is cleared.",
            failures);
        Expect(iconPresenter?.GetVisualParent() == null,
            "Removed OptionButton IconPresenter should not keep a visual parent.",
            failures);

        var point = GetControlCenterPoint(realized.Window, first);
        if (point.HasValue)
        {
            realized.Window.MouseMove(point.Value);
            realized.Window.MouseDown(point.Value, MouseButton.Left);
            realized.Window.MouseUp(point.Value, MouseButton.Left);
            RefreshLayout(realized.Window);
            var wave = FindVisualByName<Control>(first, "PART_WaveSpirit");
            Expect(wave != null,
                "OptionButton should create WaveSpiritDecorator on first press/release.",
                failures);

            group.SetCurrentValue(OptionButtonGroup.IsWaveSpiritEnabledProperty, false);
            RefreshLayout(realized.Window);
            Expect(FindVisualByName<Control>(first, "PART_WaveSpirit") == null,
                "OptionButton should remove WaveSpiritDecorator when wave is disabled.",
                failures);
            Expect(wave?.GetVisualParent() == null,
                "Removed OptionButton WaveSpiritDecorator should not keep a visual parent.",
                failures);
        }
        else
        {
            failures.Add("OptionButton center point should be available for pointer verification.");
        }
    }

    private static void VerifyOptionButtonSelectionAndPosition(ICollection<string> failures)
    {
        OptionButtonData[] options =
        [
            new OptionButtonData { Header = "Apple" },
            new OptionButtonData { Header = "Pear" },
            new OptionButtonData { Header = "Orange" }
        ];
        var group = new OptionButtonGroup
        {
            ButtonStyle  = OptionButtonStyle.Solid,
            ItemsSource  = options,
            SelectedItem = options[1]
        };

        using var realized = RealizeControl(group);
        RefreshLayout(realized.Window);

        var containers = group.GetSelfAndVisualDescendants()
                              .OfType<OptionButton>()
                              .ToArray();
        Expect(containers.Length == 3,
            $"OptionButtonGroup should realize 3 option containers. Actual: {containers.Length}.",
            failures);
        Expect(group.SelectedIndex == 1,
            $"OptionButtonGroup should keep selected data item index. Actual: {group.SelectedIndex}.",
            failures);
    }
}
