using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunCheckBoxStateVerification()
    {
        var failures = new List<string>();
        VerifyCheckBoxIndicatorSlots(failures);
        VerifyCheckBoxContentPresenterLifecycle(failures);
        VerifyCheckBoxGroupSelectionSync(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("CheckBox state verification passed.");
            return true;
        }

        Console.Error.WriteLine("CheckBox state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyCheckBoxIndicatorSlots(ICollection<string> failures)
    {
        var uncheckedBox = new AtomUI.Desktop.Controls.CheckBox
        {
            Content = "Unchecked"
        };
        using var uncheckedRealized = RealizeControl(uncheckedBox);
        Expect(FindVisualByTypeName(uncheckedBox, "CheckBoxIndicator", "Indicator") != null,
            "CheckBox should create CheckBoxIndicator.",
            failures);
        Expect(FindVisualByTypeName(uncheckedBox, "Path", "CheckedMark") == null &&
               FindVisualByTypeName(uncheckedBox, "CheckBoldOutlined", "CheckedMark") == null,
            "Unchecked CheckBox should not create checked mark.",
            failures);
        Expect(FindVisualByTypeName(uncheckedBox, "Rectangle", "TristateMark") == null,
            "Unchecked CheckBox should not create tristate mark.",
            failures);
        Expect(FindVisualByTypeName(uncheckedBox, "WaveSpiritDecorator", "PART_WaveSpirit") == null,
            "Unchecked CheckBox should not create wave decorator before interaction.",
            failures);

        uncheckedBox.SetCurrentValue(Avalonia.Controls.Primitives.ToggleButton.IsCheckedProperty, true);
        RefreshLayout(uncheckedRealized.Window);
        var runtimeCheckedMark = FindVisualByTypeName(uncheckedBox, "Path", "CheckedMark");
        Expect(runtimeCheckedMark != null,
            "CheckBox should create a lightweight checked mark when checked at runtime.",
            failures);
        var runtimeWave = FindVisualByTypeName(uncheckedBox, "WaveSpiritDecorator", "PART_WaveSpirit");
        Expect(runtimeWave != null,
            "CheckBox should create wave decorator only when a checked interaction needs it.",
            failures);

        uncheckedBox.SetCurrentValue(AtomUI.Desktop.Controls.CheckBox.IsWaveSpiritEnabledProperty, false);
        RefreshLayout(uncheckedRealized.Window);
        Expect(FindVisualByTypeName(uncheckedBox, "WaveSpiritDecorator", "PART_WaveSpirit") == null,
            "CheckBox should remove wave decorator when wave is disabled.",
            failures);
        Expect(runtimeWave?.GetVisualParent() == null,
            "Removed CheckBox wave decorator should not keep a visual parent.",
            failures);

        var checkedBox = new AtomUI.Desktop.Controls.CheckBox
        {
            Content   = "Checked",
            IsChecked = true
        };
        using var checkedRealized = RealizeControl(checkedBox);
        var initialCheckedMark = FindVisualByTypeName(checkedBox, "Path", "CheckedMark");
        Expect(initialCheckedMark != null,
            "Initially checked CheckBox should create checked mark.",
            failures);
        VerifyCheckedMarkLayout(checkedBox, initialCheckedMark, failures);
        Expect(FindVisualByTypeName(checkedBox, "CheckBoldOutlined", "CheckedMark") == null,
            "CheckBox checked mark should not use full AntDesign Icon control.",
            failures);
        Expect(FindVisualByTypeName(checkedBox, "WaveSpiritDecorator", "PART_WaveSpirit") == null,
            "Initially checked CheckBox should not create wave decorator without interaction.",
            failures);

        checkedBox.SetCurrentValue(Avalonia.Controls.Primitives.ToggleButton.IsCheckedProperty, false);
        RefreshLayout(checkedRealized.Window);
        Expect(FindVisualByTypeName(checkedBox, "Path", "CheckedMark") == null,
            "CheckBox should remove checked mark when unchecked.",
            failures);
        Expect(initialCheckedMark?.GetVisualParent() == null,
            "Removed CheckBox checked mark should not keep a visual parent.",
            failures);

        var indeterminateBox = new AtomUI.Desktop.Controls.CheckBox
        {
            Content   = "Indeterminate",
            IsChecked = null
        };
        using var indeterminateRealized = RealizeControl(indeterminateBox);
        Expect(FindVisualByTypeName(indeterminateBox, "Rectangle", "TristateMark") != null,
            "Indeterminate CheckBox should create tristate mark.",
            failures);
        Expect(FindVisualByTypeName(indeterminateBox, "Path", "CheckedMark") == null,
            "Indeterminate CheckBox should not create checked mark.",
            failures);
    }

    private static void VerifyCheckBoxContentPresenterLifecycle(ICollection<string> failures)
    {
        var contentless = new AtomUI.Desktop.Controls.CheckBox();
        using var realized = RealizeControl(contentless);
        Expect(FindVisualByName<ContentPresenter>(contentless, "ContentPresenter") == null,
            "Contentless CheckBox should not create ContentPresenter.",
            failures);

        contentless.SetCurrentValue(ContentControl.ContentProperty, "Runtime");
        RefreshLayout(realized.Window);
        var presenter = FindVisualByName<ContentPresenter>(contentless, "ContentPresenter");
        Expect(presenter != null,
            "CheckBox should create ContentPresenter when content is assigned at runtime.",
            failures);

        contentless.SetCurrentValue(ContentControl.ContentProperty, null);
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<ContentPresenter>(contentless, "ContentPresenter") == null,
            "CheckBox should remove ContentPresenter when content is cleared.",
            failures);
        Expect(presenter?.GetVisualParent() == null,
            "Removed CheckBox ContentPresenter should not keep a visual parent.",
            failures);
    }

    private static void VerifyCheckBoxGroupSelectionSync(ICollection<string> failures)
    {
        var apple = new CheckBoxOption { Content = "Apple" };
        var pear  = new CheckBoxOption { Content = "Pear", IsChecked = true };
        var group = new AtomUI.Desktop.Controls.CheckBoxGroup
        {
            ItemsSource  = new[] { apple, pear },
            CheckedItems = new List<CheckBoxOption> { pear }
        };

        using var realized = RealizeControl(group);
        Expect(GetOptionCheckBox(group, apple)?.IsChecked == false,
            "CheckBoxGroup should initialize unchecked option as false.",
            failures);
        Expect(GetOptionCheckBox(group, pear)?.IsChecked == true,
            "CheckBoxGroup should initialize checked option as true.",
            failures);

        group.CheckedItems = new List<CheckBoxOption> { apple };
        RefreshLayout(realized.Window);
        Expect(GetOptionCheckBox(group, apple)?.IsChecked == true,
            "CheckBoxGroup should check newly selected item when CheckedItems is replaced.",
            failures);
        Expect(GetOptionCheckBox(group, pear)?.IsChecked == false,
            "CheckBoxGroup should clear stale checked item when CheckedItems is replaced.",
            failures);

        group.CheckedItems = null;
        RefreshLayout(realized.Window);
        Expect(GetOptionCheckBox(group, apple)?.IsChecked == false &&
               GetOptionCheckBox(group, pear)?.IsChecked == false,
            "CheckBoxGroup should clear all item states when CheckedItems becomes null.",
            failures);
    }

    private static AtomUI.Desktop.Controls.CheckBox? GetOptionCheckBox(Control root, CheckBoxOption option)
    {
        return root.GetSelfAndVisualDescendants()
                   .OfType<AtomUI.Desktop.Controls.CheckBox>()
                   .FirstOrDefault(checkBox => ReferenceEquals(checkBox.Content, option) ||
                                               ReferenceEquals(checkBox.Content, option.Content) ||
                                               Equals(checkBox.Content, option.Content));
    }

    private static void VerifyCheckedMarkLayout(
        Control checkBox,
        Control? checkedMark,
        ICollection<string> failures)
    {
        var indicator = FindVisualByTypeName(checkBox, "CheckBoxIndicator", "Indicator");
        if (indicator == null || checkedMark == null)
        {
            return;
        }

        Expect(checkedMark.Bounds.Height < checkedMark.Bounds.Width * 0.8,
            "CheckBox checked mark layout should preserve the source icon viewBox vertical padding.",
            failures);

        var markCenter = checkedMark.TranslatePoint(
            new Avalonia.Point(checkedMark.Bounds.Width / 2, checkedMark.Bounds.Height / 2),
            indicator);
        if (!markCenter.HasValue)
        {
            failures.Add("CheckBox checked mark should be transformable to its indicator.");
            return;
        }

        var indicatorCenter = new Avalonia.Point(
            indicator.Bounds.Width / 2,
            indicator.Bounds.Height / 2);
        Expect(Math.Abs(markCenter.Value.X - indicatorCenter.X) < 0.75 &&
               Math.Abs(markCenter.Value.Y - indicatorCenter.Y) < 0.75,
            $"CheckBox checked mark should be centered in indicator, actual ({markCenter.Value.X:0.###}, {markCenter.Value.Y:0.###}), expected ({indicatorCenter.X:0.###}, {indicatorCenter.Y:0.###}).",
            failures);
    }
}
