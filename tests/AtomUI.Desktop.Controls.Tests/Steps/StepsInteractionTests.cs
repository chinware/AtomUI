using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Steps;

public class StepsInteractionTests
{
    static StepsInteractionTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Clicking_NonCurrent_Item_Requests_Change_Without_Mutating_Current()
    {
        var steps = CreateSteps(current: 10, initial: 10);
        int? requested = null;
        steps.CurrentChangeRequested += (_, args) => requested = args.Current;

        ShowInWindow(steps, window => Click(GetPresenter(GetItem(steps, 1), "HeaderPresenter"), window));

        requested.ShouldBe(11);
        steps.Current.ShouldBe(10);
    }

    [Theory]
    [InlineData("HeaderPresenter")]
    [InlineData("SubHeaderPresenter")]
    [InlineData("ContentPresenter")]
    public void Entire_Item_Content_Area_Uses_The_Same_Pointer_Activation_Path(string presenterName)
    {
        var steps = CreateSteps();
        int? requested = null;
        steps.CurrentChangeRequested += (_, args) => requested = args.Current;

        ShowInWindow(steps, window => Click(GetPresenter(GetItem(steps, 1), presenterName), window));

        requested.ShouldBe(1);
        steps.Current.ShouldBe(0);
    }

    [Fact]
    public void Clicking_Current_Item_Plays_Wave_Without_Request()
    {
        var steps    = CreateSteps();
        var requests = 0;
        steps.CurrentChangeRequested += (_, _) => requests++;

        ShowInWindow(steps, window =>
        {
            var item = GetItem(steps, 0);
            Click(GetPresenter(item, "HeaderPresenter"), window);

            requests.ShouldBe(0);
            GetIndicator(item).IsWavePlaying.ShouldBeTrue();
        });
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void NonClickable_Or_Disabled_Item_Does_Not_Activate(bool isItemClickable, bool isItemEnabled)
    {
        var steps = CreateSteps();
        steps.IsItemClickable = isItemClickable;
        GetItem(steps, 1).IsEnabled = isItemEnabled;
        var requests = 0;
        steps.CurrentChangeRequested += (_, _) => requests++;

        ShowInWindow(steps, window =>
        {
            var item = GetItem(steps, 1);
            Click(GetPresenter(item, "HeaderPresenter"), window);

            requests.ShouldBe(0);
            GetIndicator(item).IsWavePlaying.ShouldBeFalse();
        });
    }

    [Fact]
    public void Releasing_Outside_The_Pressed_Item_Cancels_Activation()
    {
        var steps    = CreateSteps();
        var requests = 0;
        steps.CurrentChangeRequested += (_, _) => requests++;

        ShowInWindow(steps, window =>
        {
            var item   = GetItem(steps, 1);
            var target = GetPresenter(item, "HeaderPresenter");
            var point  = GetCenter(target, window);
            window.MouseMove(point);
            window.MouseDown(point, MouseButton.Left);
            window.MouseMove(new Point(window.Bounds.Width - 2, window.Bounds.Height - 2));
            window.MouseUp(new Point(window.Bounds.Width - 2, window.Bounds.Height - 2), MouseButton.Left);
            Dispatcher.UIThread.RunJobs();

            requests.ShouldBe(0);
            GetIndicator(item).IsWavePlaying.ShouldBeFalse();
        });
    }

    [Fact]
    public void Empty_Area_Within_Item_Uses_The_Item_Activation_Path()
    {
        var steps = CreateSteps();
        int? requested = null;
        steps.CurrentChangeRequested += (_, args) => requested = args.Current;

        ShowInWindow(steps, window =>
        {
            var item = GetItem(steps, 1);
            var point = item.TranslatePoint(
                new Point(item.Bounds.Width - 4, item.Bounds.Height - 4),
                window).ShouldNotBeNull();

            window.MouseMove(point);
            window.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();

            requested.ShouldBe(1);
        });
    }

    [Theory]
    [InlineData(Key.Enter, PhysicalKey.Enter)]
    [InlineData(Key.Space, PhysicalKey.Space)]
    public void Keyboard_Activation_Requests_Change_And_Plays_Wave(Key key, PhysicalKey physicalKey)
    {
        var steps = CreateSteps();
        int? requested = null;
        steps.CurrentChangeRequested += (_, args) => requested = args.Current;

        ShowInWindow(steps, window =>
        {
            var item = GetItem(steps, 1);
            item.Focus().ShouldBeTrue();
            window.KeyPress(key, RawInputModifiers.None, physicalKey, null);
            window.KeyRelease(key, RawInputModifiers.None, physicalKey, null);
            Dispatcher.UIThread.RunJobs();

            requested.ShouldBe(1);
            steps.Current.ShouldBe(0);
            GetIndicator(item).IsWavePlaying.ShouldBeTrue();
        });
    }

    [Theory]
    [InlineData(0, true, true, true)]
    [InlineData(1, false, true, false)]
    [InlineData(1, true, false, false)]
    public void Keyboard_Does_Not_Request_Current_Disabled_Or_NonClickable_Item_And_Waves_Only_When_Invokable(
        int index,
        bool isItemClickable,
        bool isItemEnabled,
        bool expectedWave)
    {
        var steps = CreateSteps();
        steps.IsItemClickable = isItemClickable;
        GetItem(steps, index).IsEnabled = isItemEnabled;
        var requests = 0;
        steps.CurrentChangeRequested += (_, _) => requests++;

        ShowInWindow(steps, window =>
        {
            var item = GetItem(steps, index);
            item.Focus();
            window.KeyPress(Key.Enter, RawInputModifiers.None, PhysicalKey.Enter, null);
            window.KeyRelease(Key.Enter, RawInputModifiers.None, PhysicalKey.Enter, null);
            Dispatcher.UIThread.RunJobs();

            requests.ShouldBe(0);
            GetIndicator(item).IsWavePlaying.ShouldBe(expectedWave);
        });
    }

    private static Desktop.Controls.Steps CreateSteps(int current = 0, int initial = 0)
    {
        var steps = new Desktop.Controls.Steps
        {
            Width             = 760,
            Current           = current,
            Initial           = initial,
            IsItemClickable   = true,
            IsMotionEnabled   = true
        };
        steps.Items.Add(new Desktop.Controls.StepsItem { Header = "Step 1", SubHeader = "Sub 1", Content = "Content 1" });
        steps.Items.Add(new Desktop.Controls.StepsItem { Header = "Step 2", SubHeader = "Sub 2", Content = "Content 2" });
        steps.Items.Add(new Desktop.Controls.StepsItem { Header = "Step 3", SubHeader = "Sub 3", Content = "Content 3" });
        return steps;
    }

    private static Desktop.Controls.StepsItem GetItem(Desktop.Controls.Steps steps, int index)
    {
        return steps.Items[index].ShouldBeOfType<Desktop.Controls.StepsItem>();
    }

    private static Control GetPresenter(Desktop.Controls.StepsItem item, string name)
    {
        return item.GetVisualDescendants().OfType<Control>().Single(control => control.Name == name);
    }

    private static Desktop.Controls.StepsItemIndicator GetIndicator(Desktop.Controls.StepsItem item)
    {
        return item.GetVisualDescendants().OfType<Desktop.Controls.StepsItemIndicator>().Single();
    }

    private static void Click(Control target, AvaloniaWindow window)
    {
        var point = GetCenter(target, window);
        window.MouseMove(point);
        window.MouseDown(point, MouseButton.Left);
        window.MouseUp(point, MouseButton.Left);
        Dispatcher.UIThread.RunJobs();
    }

    private static Point GetCenter(Control target, AvaloniaWindow window)
    {
        var point = target.TranslatePoint(
            new Point(target.Bounds.Width / 2, target.Bounds.Height / 2),
            window);
        return point.ShouldNotBeNull();
    }

    private static void ShowInWindow(Control content, Action<AvaloniaWindow> assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 900,
            Height  = 240,
            Content = content
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick(1);
            Dispatcher.UIThread.RunJobs();
            assertion(window);
        }
        finally
        {
            window.Close();
        }
    }
}
