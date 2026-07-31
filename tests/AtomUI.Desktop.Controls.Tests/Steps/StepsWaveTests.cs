using AtomUI.Animations;
using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Icons.AntDesign;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Controls.Primitives;
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
    [InlineData(Desktop.Controls.StepsType.Default)]
    [InlineData(Desktop.Controls.StepsType.Dot)]
    [InlineData(Desktop.Controls.StepsType.Navigation)]
    [InlineData(Desktop.Controls.StepsType.Inline)]
    public void Pointer_Click_Plays_Indicator_Wave_For_Wave_Enabled_Types(Desktop.Controls.StepsType type)
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
    public void OutlineDot_Pointer_Click_Requests_Change_Without_Indicator_Wave()
    {
        var steps = CreateSteps(Desktop.Controls.StepsType.OutlineDot);
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
    public void OutlineDot_Current_Click_Does_Not_Play_Indicator_Wave()
    {
        var steps = CreateSteps(Desktop.Controls.StepsType.OutlineDot);
        var requestCount = 0;
        steps.CurrentChangeRequested += (_, _) => requestCount++;

        ShowInWindow(steps, window =>
        {
            var item = GetItem(steps, 0);
            Click(item, window);

            requestCount.ShouldBe(0);
            GetIndicator(item).IsWavePlaying.ShouldBeFalse();
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
    public void WaveSpirit_Disabled_Still_Requests_Change_But_Does_Not_Play_Wave()
    {
        var steps = CreateSteps();
        int? requested = null;
        steps.CurrentChangeRequested += (_, args) => requested = args.Current;

        ShowInWindow(steps, window =>
        {
            var item      = GetItem(steps, 1);
            var indicator = GetIndicator(item);
            indicator.IsWaveSpiritEnabled = false;

            Click(item, window);

            requested.ShouldBe(1);
            indicator.IsWavePlaying.ShouldBeFalse();
        });
    }

    [Fact]
    public void Keyboard_Activation_Plays_Indicator_Wave_When_Motion_Is_Enabled()
    {
        var steps = CreateSteps();
        int? requested = null;
        steps.CurrentChangeRequested += (_, args) => requested = args.Current;

        ShowInWindow(steps, window =>
        {
            var item = GetItem(steps, 1);
            item.Focus().ShouldBeTrue();
            window.KeyPress(Key.Enter, RawInputModifiers.None, PhysicalKey.Enter, null);
            window.KeyRelease(Key.Enter, RawInputModifiers.None, PhysicalKey.Enter, null);
            Dispatcher.UIThread.RunJobs();

            requested.ShouldBe(1);
            GetIndicator(item).IsWavePlaying.ShouldBeTrue();
        });
    }

    [Fact]
    public void Indicator_Motion_Enabled_Installs_Status_Color_Transitions()
    {
        var steps = CreateSteps();

        ShowInWindow(steps, _ =>
        {
            var transitions = GetIndicator(GetItem(steps, 1)).Transitions;

            transitions.ShouldNotBeNull();
            transitions!.OfType<SolidColorBrushTransition>()
                        .Select(transition => transition.Property)
                        .ShouldBe([
                            TemplatedControl.BackgroundProperty,
                            TemplatedControl.ForegroundProperty,
                            TemplatedControl.BorderBrushProperty
                        ], ignoreOrder: true);
        });
    }

    [Theory]
    [InlineData(Desktop.Controls.StepsStatus.Wait, StepsTokenKind.WaitIconColor)]
    [InlineData(Desktop.Controls.StepsStatus.Process, StepsTokenKind.ProcessIconBorderColor)]
    [InlineData(Desktop.Controls.StepsStatus.Finish, StepsTokenKind.FinishIconColor)]
    [InlineData(Desktop.Controls.StepsStatus.Error, StepsTokenKind.ErrorIconBorderColor)]
    public void Custom_Loading_Icon_Is_Laid_Out_And_Uses_Status_Color(
        Desktop.Controls.StepsStatus status,
        StepsTokenKind expectedColorToken)
    {
        var processIcon = new LoadingOutlined
        {
            LoadingAnimation = IconAnimation.Spin
        };
        var steps = new Desktop.Controls.Steps
        {
            Width           = 760,
            Current         = 0,
            IsItemClickable = true,
            IsMotionEnabled = true
        };
        steps.Items.Add(new Desktop.Controls.StepsItem
        {
            Header = "Pay",
            Status = status,
            Icon   = processIcon
        });

        ShowInWindow(steps, _ =>
        {
            var indicator = GetIndicator(GetItem(steps, 0));
            var presenter = indicator.GetVisualDescendants()
                                     .OfType<IconPresenter>()
                                     .Single(control => control.Name == "CustomIconPresenter");
            var icon = presenter.GetVisualDescendants()
                                .OfType<LoadingOutlined>()
                                .Single();

            presenter.Bounds.Width.ShouldBeGreaterThan(0);
            presenter.Bounds.Height.ShouldBeGreaterThan(0);
            icon.Bounds.Width.ShouldBeGreaterThan(0);
            icon.Bounds.Height.ShouldBeGreaterThan(0);
            icon.LoadingAnimation.ShouldBe(IconAnimation.Spin);
            GetColor(icon.StrokeBrush).ShouldBe(GetColor(GetThemeResource<IBrush>(expectedColorToken)));
            GetColor(icon.FillBrush).ShouldBe(GetColor(GetThemeResource<IBrush>(expectedColorToken)));
        });
    }

    [Theory]
    [InlineData(Desktop.Controls.StepsStatus.Finish, typeof(CheckOutlined), StepsTokenKind.FinishIconColor)]
    [InlineData(Desktop.Controls.StepsStatus.Error, typeof(CloseOutlined), StepsTokenKind.ErrorIconColor)]
    public void Built_In_Status_Icon_Uses_Indicator_Status_Color(
        Desktop.Controls.StepsStatus status,
        Type iconType,
        StepsTokenKind expectedColorToken)
    {
        var steps = new Desktop.Controls.Steps
        {
            Width   = 760,
            Current = 0
        };
        steps.Items.Add(new Desktop.Controls.StepsItem
        {
            Header = "Status",
            Status = status
        });

        ShowInWindow(steps, _ =>
        {
            var indicator = GetIndicator(GetItem(steps, 0));
            var icon = indicator.GetVisualDescendants()
                                .OfType<Icon>()
                                .Single(control => control.GetType() == iconType);

            icon.IsVisible.ShouldBeTrue();
            GetColor(icon.StrokeBrush).ShouldBe(GetColor(GetThemeResource<IBrush>(expectedColorToken)));
            GetColor(icon.FillBrush).ShouldBe(GetColor(GetThemeResource<IBrush>(expectedColorToken)));
        });
    }

    [Theory]
    [InlineData(Desktop.Controls.StepsStatus.Wait, StepsTokenKind.WaitDotColor)]
    [InlineData(Desktop.Controls.StepsStatus.Process, StepsTokenKind.ProcessDotColor)]
    [InlineData(Desktop.Controls.StepsStatus.Finish, StepsTokenKind.FinishDotColor)]
    [InlineData(Desktop.Controls.StepsStatus.Error, StepsTokenKind.ErrorDotColor)]
    public void OutlineDot_Uses_Transparent_Background_And_Status_Border(
        Desktop.Controls.StepsStatus status,
        StepsTokenKind expectedColorToken)
    {
        var steps = new Desktop.Controls.Steps
        {
            Width   = 760,
            Current = 0,
            Type    = Desktop.Controls.StepsType.OutlineDot
        };
        steps.Items.Add(new Desktop.Controls.StepsItem
        {
            Header = "Status",
            Status = status
        });

        ShowInWindow(steps, _ =>
        {
            var frame = GetIndicatorFrame(GetIndicator(GetItem(steps, 0)));

            GetColor(frame.Background).ShouldBe(Colors.Transparent);
            GetColor(frame.BorderBrush).ShouldBe(GetColor(GetThemeResource<IBrush>(expectedColorToken)));
            frame.BorderThickness.ShouldBe(new Thickness(2));
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

    private static PixelAlignedBorder GetIndicatorFrame(Desktop.Controls.StepsItemIndicator indicator)
    {
        return indicator.GetVisualDescendants().OfType<PixelAlignedBorder>().Single(control => control.Name == "Frame");
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

    private static T GetThemeResource<T>(object key)
    {
        var application = Application.Current.ShouldNotBeNull();
        application.TryGetResource(key, application.ActualThemeVariant, out var value).ShouldBeTrue();
        value.ShouldBeAssignableTo<T>();
        return (T)value!;
    }

    private static Color GetColor(IBrush? brush)
    {
        brush.ShouldNotBeNull();
        return ((ISolidColorBrush)brush!).Color;
    }
}
