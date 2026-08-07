using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunStepsStateVerification()
    {
        var failures = new List<string>();
        VerifyStepsPanelDefinitions(failures);
        VerifyStepsIconProgressState(failures);
        VerifyStepsStatusMatrix(failures);
        VerifyStepsIndicatorTemplateShape(failures);
        VerifyStepsCurrentContentLifecycle(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Steps state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Steps state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyStepsPanelDefinitions(ICollection<string> failures)
    {
        var steps = CreateSteps(orientation: Orientation.Vertical);

        using var realized = RealizeControl(steps);
        var grid = GetStepsItemsGrid(steps);
        Expect(grid?.ColumnDefinitions.Count == 1,
            $"Vertical Steps should start with one column definition, actual {grid?.ColumnDefinitions.Count}.",
            failures);

        steps.Orientation = Orientation.Horizontal;
        RefreshLayout(realized.Window);
        Expect(grid?.ColumnDefinitions.Count == 3,
            $"Horizontal Steps should create one column per item, actual {grid?.ColumnDefinitions.Count}.",
            failures);

        steps.Orientation = Orientation.Vertical;
        RefreshLayout(realized.Window);
        Expect(grid?.ColumnDefinitions.Count == 1,
            $"Vertical Steps should clear old horizontal columns before adding its single column, actual {grid?.ColumnDefinitions.Count}.",
            failures);
    }

    private static void VerifyStepsIconProgressState(ICollection<string> failures)
    {
        var steps = CreateSteps(currentStep: 1, isShowProgress: true, progressValue: 50);

        using var realized = RealizeControl(steps);
        var items = GetRealizedStepsItems(steps);
        Expect(items.Count == 3,
            $"Steps progress verification should realize three items, actual {items.Count}.",
            failures);
        if (items.Count == 0)
        {
            return;
        }

        var first = items[0];
        Expect(GetNonPublicProperty<bool>(first, "IsEffectiveShowProgress"),
            "StepsItem should enable effective progress when progress is shown and no custom icon is set.",
            failures);

        first.Icon = new UserOutlined();
        RefreshLayout(realized.Window);
        Expect(!GetNonPublicProperty<bool>(first, "IsEffectiveShowProgress"),
            "StepsItem should disable effective progress when a custom icon is added.",
            failures);

        first.Icon = null;
        RefreshLayout(realized.Window);
        Expect(GetNonPublicProperty<bool>(first, "IsEffectiveShowProgress"),
            "StepsItem should re-enable effective progress when the custom icon is cleared.",
            failures);
    }

    private static void VerifyStepsStatusMatrix(ICollection<string> failures)
    {
        var steps = CreateSteps(currentStep: 1);

        using var realized = RealizeControl(steps);
        var items = GetRealizedStepsItems(steps);
        Expect(items.Count == 3,
            $"Steps status verification should realize three items, actual {items.Count}.",
            failures);
        if (items.Count != 3)
        {
            return;
        }

        Expect(items[0].Status == StepsItemStatus.Finish && GetNonPublicProperty<bool>(items[0], "IsFinished"),
            $"Step before current should be Finish/finished, actual {items[0].Status}/{GetNonPublicProperty<bool>(items[0], "IsFinished")}.",
            failures);
        Expect(items[1].Status == StepsItemStatus.Process && !GetNonPublicProperty<bool>(items[1], "IsFinished"),
            $"Current step should be Process/not finished, actual {items[1].Status}/{GetNonPublicProperty<bool>(items[1], "IsFinished")}.",
            failures);
        Expect(items[2].Status == StepsItemStatus.Wait && !GetNonPublicProperty<bool>(items[2], "IsFinished"),
            $"Step after current should be Wait/not finished, actual {items[2].Status}/{GetNonPublicProperty<bool>(items[2], "IsFinished")}.",
            failures);

        steps.CurrentStepStatus = StepsItemStatus.Error;
        RefreshLayout(realized.Window);
        Expect(items[1].Status == StepsItemStatus.Error,
            $"CurrentStepStatus change should update the selected item, actual {items[1].Status}.",
            failures);

        steps.CurrentStep = 2;
        RefreshLayout(realized.Window);
        Expect(items[1].Status == StepsItemStatus.Finish && GetNonPublicProperty<bool>(items[1], "IsFinished"),
            $"Previously current step should become Finish/finished, actual {items[1].Status}/{GetNonPublicProperty<bool>(items[1], "IsFinished")}.",
            failures);
        Expect(items[2].Status == StepsItemStatus.Error && !GetNonPublicProperty<bool>(items[2], "IsFinished"),
            $"New current step should inherit CurrentStepStatus Error/not finished, actual {items[2].Status}/{GetNonPublicProperty<bool>(items[2], "IsFinished")}.",
            failures);
    }

    private static void VerifyStepsCurrentContentLifecycle(ICollection<string> failures)
    {
        var steps = new Steps
        {
            CurrentStep = 0
        };
        steps.Items.Add(new StepsItem { Header = "First", Content = "First content" });
        steps.Items.Add(new StepsItem { Header = "Second", Content = "Second content" });

        using (var realized = RealizeControl(steps))
        {
            Expect(Equals(steps.CurrentContent, "First content"),
                $"Steps.CurrentContent should track the selected item content, actual {steps.CurrentContent ?? "<null>"}.",
                failures);

            steps.CurrentStep = 1;
            RefreshLayout(realized.Window);
            Expect(Equals(steps.CurrentContent, "Second content"),
                $"Steps.CurrentContent should update after CurrentStep changes, actual {steps.CurrentContent ?? "<null>"}.",
                failures);
        }

        Expect(GetPrivateField(steps, "AtomUI.Desktop.Controls.Steps", "_currentItemSubscriptions") is null,
            "Steps should dispose current-item content subscriptions when detached from the visual tree.",
            failures);
    }

    private static void VerifyStepsIndicatorTemplateShape(ICollection<string> failures)
    {
        var steps = CreateSteps(currentStep: 1);

        using var realized = RealizeControl(steps);
        Expect(CountNamedVisuals(steps, "FinishedMark") == 1,
            $"Steps default indicator should materialize one FinishedMark for the finished item, actual {CountNamedVisuals(steps, "FinishedMark")}.",
            failures);
        Expect(CountNamedVisuals(steps, "ErrorMark") == 0,
            $"Steps default indicator should not materialize ErrorMark when no item is in error, actual {CountNamedVisuals(steps, "ErrorMark")}.",
            failures);
        Expect(CountNamedVisuals(steps, "PositionText") == 2,
            $"Steps default indicator should keep PositionText only for wait/process items, actual {CountNamedVisuals(steps, "PositionText")}.",
            failures);
        Expect(CountNamedVisuals(steps, "CustomIconPresenter") == 0,
            $"Steps without custom icons should not materialize CustomIconPresenter, actual {CountNamedVisuals(steps, "CustomIconPresenter")}.",
            failures);

        steps.CurrentStepStatus = StepsItemStatus.Error;
        RefreshLayout(realized.Window);
        Expect(CountNamedVisuals(steps, "ErrorMark") == 1,
            $"Steps error status should materialize one ErrorMark, actual {CountNamedVisuals(steps, "ErrorMark")}.",
            failures);

        var iconSteps = CreateIconSteps();
        using var _ = RealizeControl(iconSteps);
        Expect(CountNamedVisuals(iconSteps, "CustomIconPresenter") == 4,
            $"Icon Steps should materialize one CustomIconPresenter per icon item, actual {CountNamedVisuals(iconSteps, "CustomIconPresenter")}.",
            failures);
    }

    private static Grid? GetStepsItemsGrid(Steps steps)
    {
        return GetPrivateField(steps, "AtomUI.Desktop.Controls.Steps", "_grid") as Grid;
    }

    private static IReadOnlyList<StepsItem> GetRealizedStepsItems(Steps steps)
    {
        return steps.GetSelfAndVisualDescendants()
                    .OfType<StepsItem>()
                    .OrderBy(item => GetNonPublicProperty<int>(item, "Position"))
                    .ToList();
    }

    private static int CountNamedVisuals(Control root, string name)
    {
        return root.GetSelfAndVisualDescendants()
                   .OfType<Control>()
                   .Count(control => control.Name == name);
    }
}
