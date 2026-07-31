using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Steps;

public class StepsStateTests
{
    static StepsStateTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Current_Is_The_Only_Current_State_Input()
    {
        typeof(Desktop.Controls.Steps).IsSubclassOf(typeof(ItemsControl)).ShouldBeTrue();
        typeof(Desktop.Controls.Steps).IsSubclassOf(typeof(SelectingItemsControl)).ShouldBeFalse();
        typeof(Desktop.Controls.Steps).GetProperty("SelectedIndex").ShouldBeNull();
        typeof(Desktop.Controls.Steps).GetProperty("SelectedItem").ShouldBeNull();
        typeof(Desktop.Controls.StepsItem).GetInterface("ISelectable").ShouldBeNull();

        var rootProperties = typeof(Desktop.Controls.Steps).GetProperties().Select(property => property.Name).ToArray();
        rootProperties.ShouldContain(nameof(Desktop.Controls.Steps.Current));
        rootProperties.ShouldContain(nameof(Desktop.Controls.Steps.Initial));
        rootProperties.ShouldContain(nameof(Desktop.Controls.Steps.Status));
        rootProperties.ShouldContain(nameof(Desktop.Controls.Steps.Percent));
        rootProperties.ShouldContain(nameof(Desktop.Controls.Steps.Type));
        rootProperties.ShouldContain(nameof(Desktop.Controls.Steps.TitlePlacement));
        rootProperties.ShouldNotContain("CurrentStep");
        rootProperties.ShouldNotContain("InitialStep");
        rootProperties.ShouldNotContain("CurrentStepStatus");
        rootProperties.ShouldNotContain("CurrentContent");
        rootProperties.ShouldNotContain("CurrentContentTemplate");

        typeof(Desktop.Controls.StepsItem).GetProperty(nameof(Desktop.Controls.StepsItem.Status))!
                                          .PropertyType.ShouldBe(typeof(Desktop.Controls.StepsStatus?));
        typeof(Desktop.Controls.StepsItem).GetProperty("Description").ShouldBeNull();
        typeof(Desktop.Controls.StepsItem).GetProperty("IsSelected").ShouldBeNull();
        typeof(Desktop.Controls.StepsItem).GetProperty("IsFinished").ShouldBeNull();
    }

    [Theory]
    [InlineData(-1, 0, Desktop.Controls.StepsStatus.Wait, Desktop.Controls.StepsStatus.Wait, Desktop.Controls.StepsStatus.Wait)]
    [InlineData(1, 0, Desktop.Controls.StepsStatus.Finish, Desktop.Controls.StepsStatus.Process, Desktop.Controls.StepsStatus.Wait)]
    [InlineData(4, 0, Desktop.Controls.StepsStatus.Finish, Desktop.Controls.StepsStatus.Finish, Desktop.Controls.StepsStatus.Finish)]
    [InlineData(11, 10, Desktop.Controls.StepsStatus.Finish, Desktop.Controls.StepsStatus.Process, Desktop.Controls.StepsStatus.Wait)]
    public void Status_Is_Derived_From_Current(
        int current,
        int initial,
        Desktop.Controls.StepsStatus first,
        Desktop.Controls.StepsStatus second,
        Desktop.Controls.StepsStatus third)
    {
        var steps = CreateSteps(current, initial, 3);

        ShowInWindow(steps, () =>
        {
            var items = GetItems(steps);
            items.Select(item => item.EffectiveStatus).ShouldBe([first, second, third]);
            items.Select(item => item.StepNumber).ShouldBe([initial, initial + 1, initial + 2]);
        });
    }

    [Fact]
    public void Root_Status_And_Reverse_Current_Recompute_All_Realized_Items()
    {
        var steps = CreateSteps(2, 0, 3);

        ShowInWindow(steps, () =>
        {
            steps.Status = Desktop.Controls.StepsStatus.Error;
            steps.Current = 0;

            var items = GetItems(steps);
            items.Select(item => item.EffectiveStatus).ShouldBe([
                Desktop.Controls.StepsStatus.Error,
                Desktop.Controls.StepsStatus.Wait,
                Desktop.Controls.StepsStatus.Wait
            ]);
            items.Select(item => item.IsCurrent).ShouldBe([true, false, false]);
        });
    }

    [Fact]
    public void Item_Status_Overrides_Automatic_Status_And_Updates_Previous_Connector()
    {
        var steps = CreateSteps(0, 0, 3);

        ShowInWindow(steps, () =>
        {
            var items = GetItems(steps);
            items[1].Status = Desktop.Controls.StepsStatus.Error;

            items[1].AutomaticStatus.ShouldBe(Desktop.Controls.StepsStatus.Wait);
            items[1].EffectiveStatus.ShouldBe(Desktop.Controls.StepsStatus.Error);
            items[0].ConnectorStatus.ShouldBe(Desktop.Controls.StepsStatus.Error);

            items[1].Status = null;

            items[1].EffectiveStatus.ShouldBe(Desktop.Controls.StepsStatus.Wait);
            items[0].ConnectorStatus.ShouldBe(Desktop.Controls.StepsStatus.Wait);
        });
    }

    [Fact]
    public void Applying_Template_Does_Not_Overwrite_Current_With_Initial()
    {
        var steps = CreateSteps(7, 5, 3);

        ShowInWindow(steps, () =>
        {
            steps.ApplyTemplate();
            steps.ApplyTemplate();

            steps.Current.ShouldBe(7);
        });
    }

    private static Desktop.Controls.Steps CreateSteps(int current, int initial, int count)
    {
        var steps = new Desktop.Controls.Steps
        {
            Width   = 640,
            Current = current,
            Initial = initial
        };

        for (var index = 0; index < count; index++)
        {
            steps.Items.Add(new Desktop.Controls.StepsItem { Header = $"Step {index + 1}" });
        }

        return steps;
    }

    private static Desktop.Controls.StepsItem[] GetItems(Desktop.Controls.Steps steps)
    {
        return steps.Items.Cast<Desktop.Controls.StepsItem>().ToArray();
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 760,
            Height  = 240,
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
