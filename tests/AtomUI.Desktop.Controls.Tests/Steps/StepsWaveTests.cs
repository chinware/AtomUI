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

public class StepsWaveTests
{
    static StepsWaveTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Theory]
    [InlineData(Desktop.Controls.StepsType.Default)]
    [InlineData(Desktop.Controls.StepsType.Dot)]
    [InlineData(Desktop.Controls.StepsType.Navigation)]
    [InlineData(Desktop.Controls.StepsType.Inline)]
    public void Pointer_Click_Plays_Indicator_Wave_For_Every_Type(Desktop.Controls.StepsType type)
    {
        var steps = CreateSteps(type);

        ShowInWindow(steps, window =>
        {
            var item = GetItem(steps, 1);
            Click(item, window);

            GetIndicator(item).IsWavePlaying.ShouldBeTrue();
        });
    }

    [Fact]
    public void Programmatic_Current_Change_Does_Not_Play_Wave()
    {
        var steps = CreateSteps();

        ShowInWindow(steps, _ =>
        {
            var item = GetItem(steps, 1);
            steps.Current = 1;
            Dispatcher.UIThread.RunJobs();

            GetIndicator(item).IsWavePlaying.ShouldBeFalse();
        });
    }

    [Fact]
    public void Motion_Disabled_Still_Requests_Change_But_Does_Not_Play_Wave()
    {
        var steps = CreateSteps();
        steps.IsMotionEnabled = false;
        int? requested = null;
        steps.CurrentChangeRequested += (_, args) => requested = args.Current;

        ShowInWindow(steps, window =>
        {
            var item = GetItem(steps, 1);
            Click(item, window);

            requested.ShouldBe(1);
            GetIndicator(item).IsWavePlaying.ShouldBeFalse();
        });
    }

    [Fact]
    public void Indicator_Wave_Has_A_NonClipping_Visual_Path()
    {
        var steps = CreateSteps();

        ShowInWindow(steps, _ =>
        {
            var indicator = GetIndicator(GetItem(steps, 1));
            var wave = indicator.GetVisualDescendants()
                                .Single(visual => visual.GetType().Name == "WaveSpiritDecorator");
            var clippingAncestors = wave.GetVisualAncestors()
                                        .TakeWhile(visual => visual is not AvaloniaWindow)
                                        .Where(visual => visual.ClipToBounds)
                                        .ToArray();

            wave.Bounds.Size.ShouldBe(indicator.Bounds.Size);
            wave.ClipToBounds.ShouldBeFalse();
            clippingAncestors.ShouldBeEmpty();
        });
    }

    private static Desktop.Controls.Steps CreateSteps(
        Desktop.Controls.StepsType type = Desktop.Controls.StepsType.Default)
    {
        var steps = new Desktop.Controls.Steps
        {
            Width           = 760,
            Current         = 0,
            Type            = type,
            IsItemClickable = true,
            IsMotionEnabled = true
        };
        steps.Items.Add(new Desktop.Controls.StepsItem { Header = "Step 1" });
        steps.Items.Add(new Desktop.Controls.StepsItem { Header = "Step 2" });
        steps.Items.Add(new Desktop.Controls.StepsItem { Header = "Step 3" });
        return steps;
    }

    private static Desktop.Controls.StepsItem GetItem(Desktop.Controls.Steps steps, int index)
    {
        return steps.Items[index].ShouldBeOfType<Desktop.Controls.StepsItem>();
    }

    private static Desktop.Controls.StepsItemIndicator GetIndicator(Desktop.Controls.StepsItem item)
    {
        return item.GetVisualDescendants().OfType<Desktop.Controls.StepsItemIndicator>().Single();
    }

    private static void Click(Desktop.Controls.StepsItem item, AvaloniaWindow window)
    {
        var target = item.GetVisualDescendants()
                         .OfType<Control>()
                         .Single(control => control.Name == "HeaderPresenter");
        var point = target.TranslatePoint(
            new Point(target.Bounds.Width / 2, target.Bounds.Height / 2),
            window);
        point.ShouldNotBeNull();

        window.MouseMove(point.Value);
        window.MouseDown(point.Value, MouseButton.Left);
        window.MouseUp(point.Value, MouseButton.Left);
        Dispatcher.UIThread.RunJobs();
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
