using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
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
    [InlineData(StepsStyle.Default, StepsItemIndicatorType.Default)]
    [InlineData(StepsStyle.Default, StepsItemIndicatorType.Dot)]
    [InlineData(StepsStyle.Navigation, StepsItemIndicatorType.Default)]
    [InlineData(StepsStyle.Inline, StepsItemIndicatorType.Default)]
    public void Clickable_Item_Selection_Plays_Indicator_Wave(
        StepsStyle style,
        StepsItemIndicatorType indicatorType)
    {
        var steps = CreateSteps(
            isItemClickable: true,
            isMotionEnabled: true,
            style: style,
            indicatorType: indicatorType);

        ShowInWindow(steps, window =>
        {
            var items = steps.GetVisualDescendants()
                             .OfType<StepsItem>()
                             .ToList();

            ClickIndicator(items[1], window);
            Dispatcher.UIThread.RunJobs();

            steps.SelectedIndex.ShouldBe(1);
            IsIndicatorWavePlaying(items[1]).ShouldBeTrue();
        });
    }

    [Fact]
    public void Clicking_Current_Clickable_Item_Does_Not_Play_Indicator_Wave()
    {
        var steps = CreateSteps(isItemClickable: true, isMotionEnabled: true);

        ShowInWindow(steps, window =>
        {
            var currentItem = steps.GetVisualDescendants()
                                   .OfType<StepsItem>()
                                   .First();

            ClickIndicator(currentItem, window);
            Dispatcher.UIThread.RunJobs();

            steps.SelectedIndex.ShouldBe(0);
            IsIndicatorWavePlaying(currentItem).ShouldBeFalse();
        });
    }

    [Fact]
    public void Clicking_Item_Content_Plays_Indicator_Wave()
    {
        var steps = CreateSteps(isItemClickable: true, isMotionEnabled: true);

        ShowInWindow(steps, window =>
        {
            var items = steps.GetVisualDescendants()
                             .OfType<StepsItem>()
                             .ToList();

            ClickItemContent(items[1], window);
            Dispatcher.UIThread.RunJobs();

            steps.SelectedIndex.ShouldBe(1);
            IsIndicatorWavePlaying(items[1]).ShouldBeTrue();
        });
    }

    [Fact]
    public void CurrentStep_Change_Plays_Indicator_Wave_When_Items_Are_Clickable()
    {
        var steps = CreateSteps(isItemClickable: true, isMotionEnabled: true);

        ShowInWindow(steps, _ =>
        {
            var items = steps.GetVisualDescendants()
                             .OfType<StepsItem>()
                             .ToList();

            steps.CurrentStep = 1;
            Dispatcher.UIThread.RunJobs();

            steps.SelectedIndex.ShouldBe(1);
            IsIndicatorWavePlaying(items[1]).ShouldBeTrue();
        });
    }

    [Fact]
    public void Clickable_Item_Selection_Wave_Replays_After_Selecting_Another_Item()
    {
        var steps = CreateSteps(isItemClickable: true, isMotionEnabled: true);

        ShowInWindow(steps, window =>
        {
            var items = steps.GetVisualDescendants()
                             .OfType<StepsItem>()
                             .ToList();

            ClickIndicator(items[1], window);
            Dispatcher.UIThread.RunJobs();
            IsIndicatorWavePlaying(items[1]).ShouldBeTrue();

            WaitForIndicatorWaveToFinish(items[1]);
            ClickIndicator(items[2], window);
            Dispatcher.UIThread.RunJobs();
            ClickIndicator(items[1], window);
            Dispatcher.UIThread.RunJobs();

            steps.SelectedIndex.ShouldBe(1);
            IsIndicatorWavePlaying(items[1]).ShouldBeTrue();
        });
    }

    [Fact]
    public void Clickable_Item_Wave_Has_NonClipping_Visual_Path()
    {
        var steps = CreateSteps(isItemClickable: true, isMotionEnabled: true);

        ShowInWindow(steps, _ =>
        {
            var item = steps.GetVisualDescendants()
                            .OfType<StepsItem>()
                            .Skip(1)
                            .First();
            var indicator = GetIndicator(item);
            var wave = GetWave(indicator);
            var clippingAncestors = wave.GetVisualAncestors()
                                        .TakeWhile(visual => visual is not AvaloniaWindow)
                                        .Where(visual => visual.ClipToBounds)
                                        .Select(visual => $"{visual.GetType().Name}#{visual.Name}")
                                        .ToArray();

            wave.Bounds.Size.ShouldBe(indicator.Bounds.Size);
            wave.ClipToBounds.ShouldBeFalse();
            clippingAncestors.ShouldBeEmpty();
        });
    }

    [Fact]
    public void Clickable_Item_Wave_Keeps_Stable_Opaque_Brush_During_Indicator_Background_Transition()
    {
        var steps = CreateSteps(isItemClickable: true, isMotionEnabled: true);

        ShowInWindow(steps, window =>
        {
            var item = steps.GetVisualDescendants()
                            .OfType<StepsItem>()
                            .Skip(1)
                            .First();
            var indicator = GetIndicator(item);
            var wave = GetWave(indicator);
            var initialWaveColor = GetWaveBrushColor(wave);

            ClickIndicator(item, window);
            Dispatcher.UIThread.RunJobs();

            initialWaveColor.A.ShouldBe(byte.MaxValue);
            GetWaveBrushColor(wave).ShouldBe(initialWaveColor);
        });
    }

    [Theory]
    [InlineData(false, true, 0)]
    [InlineData(true, false, 1)]
    public void Indicator_Wave_Requires_Clickable_And_Motion(
        bool isItemClickable,
        bool isMotionEnabled,
        int expectedSelectedIndex)
    {
        var steps = CreateSteps(isItemClickable, isMotionEnabled);

        ShowInWindow(steps, window =>
        {
            var items = steps.GetVisualDescendants()
                             .OfType<StepsItem>()
                             .ToList();

            ClickIndicator(items[1], window);
            Dispatcher.UIThread.RunJobs();

            steps.SelectedIndex.ShouldBe(expectedSelectedIndex);
            IsIndicatorWavePlaying(items[1]).ShouldBeFalse();
        });
    }

    private static Desktop.Controls.Steps CreateSteps(
        bool isItemClickable,
        bool isMotionEnabled,
        StepsStyle style = StepsStyle.Default,
        StepsItemIndicatorType indicatorType = StepsItemIndicatorType.Default)
    {
        var steps = new Desktop.Controls.Steps
        {
            Width             = 760,
            CurrentStep       = 0,
            IsItemClickable   = isItemClickable,
            IsMotionEnabled   = isMotionEnabled,
            Style             = style,
            ItemIndicatorType = indicatorType
        };
        steps.Items.Add(new StepsItem { Header = "Step 1" });
        steps.Items.Add(new StepsItem { Header = "Step 2" });
        steps.Items.Add(new StepsItem { Header = "Step 3" });
        return steps;
    }

    private static bool IsIndicatorWavePlaying(StepsItem item)
    {
        var indicator = GetIndicator(item);
        var wave = GetWave(indicator);
        var field = wave.GetType().GetField("_isPlaying", BindingFlags.Instance | BindingFlags.NonPublic);

        field.ShouldNotBeNull();
        return (bool)field.GetValue(wave)!;
    }

    private static void WaitForIndicatorWaveToFinish(StepsItem item)
    {
        for (var i = 0; i < 64 && IsIndicatorWavePlaying(item); i++)
        {
            Thread.Sleep(10);
            AvaloniaHeadlessPlatform.ForceRenderTimerTick(1);
            Dispatcher.UIThread.RunJobs();
        }

        IsIndicatorWavePlaying(item).ShouldBeFalse();
    }

    private static Visual GetIndicator(StepsItem item)
    {
        return item.GetVisualDescendants()
                   .Single(control => control.GetType().Name == "StepsItemIndicator");
    }

    private static Visual GetWave(Visual indicator)
    {
        return indicator.GetVisualDescendants()
                        .Single(control => control.GetType().Name == "WaveSpiritDecorator");
    }

    private static Color GetWaveBrushColor(Visual wave)
    {
        var waveBrushProperty = wave.GetType().GetProperty("WaveBrush");

        waveBrushProperty.ShouldNotBeNull();
        var waveBrush = waveBrushProperty.GetValue(wave) as ISolidColorBrush;
        waveBrush.ShouldNotBeNull();
        return waveBrush.Color;
    }

    private static void ClickIndicator(StepsItem item, AvaloniaWindow window)
    {
        var indicator = item.GetVisualDescendants()
                            .OfType<Control>()
                            .Single(control => control.GetType().Name == "StepsItemIndicator");
        var point = indicator.TranslatePoint(
            new Point(indicator.Bounds.Width / 2, indicator.Bounds.Height / 2),
            window);

        point.ShouldNotBeNull();
        window.MouseDown(point.Value, MouseButton.Left);
        window.MouseUp(point.Value, MouseButton.Left);
    }

    private static void ClickItemContent(StepsItem item, AvaloniaWindow window)
    {
        var header = item.GetVisualDescendants()
                         .OfType<Control>()
                         .Single(control => control.Name == "HeaderPresenter");
        var point = header.TranslatePoint(
            new Point(header.Bounds.Width / 2, header.Bounds.Height / 2),
            window);

        point.ShouldNotBeNull();
        window.MouseDown(point.Value, MouseButton.Left);
        window.MouseUp(point.Value, MouseButton.Left);
    }

    private static void ShowInWindow(Control content, Action<AvaloniaWindow> assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 900,
            Height  = 220,
            Content = content
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            assertion(window);
        }
        finally
        {
            window.Close();
        }
    }
}
