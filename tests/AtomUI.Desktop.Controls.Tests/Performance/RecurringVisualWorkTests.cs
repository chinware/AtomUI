using System.Reflection;
using AtomUI.Animations;
using AtomUI.Controls;
using AtomUI.Controls.Commons;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Threading;
using Shouldly;
using Xunit;
using AtomUICarousel = AtomUI.Desktop.Controls.Carousel;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Performance;

public class RecurringVisualWorkTests
{
    static RecurringVisualWorkTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public async Task Infinite_Avalonia_Animations_Pause_When_Effectively_Invisible()
    {
        var animation = new Animation();
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await animation.RunInfiniteAsync(new Border(), cancellationTokenSource.Token);

        animation.PlaybackBehavior.ShouldBe(PlaybackBehavior.OnlyIfVisible);
    }

    [Fact]
    public void Marquee_Animation_Pauses_When_Effectively_Invisible()
    {
        var marquee = new MarqueeLabel();
        SetPrivateField(typeof(AbstractMarqueeLabel), marquee, "_lastTextWidth", 240d);
        InvokePrivateMethod(typeof(AbstractMarqueeLabel), marquee, "ReConfigureAnimation");

        var animation = GetPrivateField<Animation>(typeof(AbstractMarqueeLabel), marquee, "_animation");

        animation.PlaybackBehavior.ShouldBe(PlaybackBehavior.OnlyIfVisible);
    }

    [Fact]
    public void Loading_Icon_Does_Not_Run_When_An_Ancestor_Is_Hidden()
    {
        var icon = new TestIcon
        {
            Width            = 24,
            Height           = 24,
            LoadingAnimation = IconAnimation.Spin
        };
        var host = new Border { Child = icon };

        ShowInWindow(host, () =>
        {
            InvokePrivateMethod<bool>(typeof(Icon), icon, "CanRunLoadingAnimation").ShouldBeTrue();

            host.IsVisible = false;
            Dispatcher.UIThread.RunJobs();

            InvokePrivateMethod<bool>(typeof(Icon), icon, "CanRunLoadingAnimation").ShouldBeFalse();

            host.IsVisible = true;
            Dispatcher.UIThread.RunJobs();

            InvokePrivateMethod<bool>(typeof(Icon), icon, "CanRunLoadingAnimation").ShouldBeTrue();
        });
    }

    [Fact]
    public void Spin_Stops_Compositor_Animations_When_An_Ancestor_Is_Hidden()
    {
        var indicator = new SpinIndicator();
        var host = new Border { Child = indicator };

        ShowInWindow(host, () =>
        {
            GetPrivateFieldValue(typeof(AbstractSpinIndicator), indicator, "_animatedIndicatorTarget")
                .ShouldNotBeNull();

            host.IsVisible = false;
            Dispatcher.UIThread.RunJobs();

            GetPrivateFieldValue(typeof(AbstractSpinIndicator), indicator, "_animatedIndicatorTarget")
                .ShouldBeNull();

            host.IsVisible = true;
            Dispatcher.UIThread.RunJobs();

            GetPrivateFieldValue(typeof(AbstractSpinIndicator), indicator, "_animatedIndicatorTarget")
                .ShouldNotBeNull();
        });
    }

    [Fact]
    public void Carousel_AutoPlay_Timer_Pauses_When_An_Ancestor_Is_Hidden()
    {
        var carousel = new AtomUICarousel
        {
            IsAutoPlay = true,
            ItemsSource = new[] { "First", "Second" }
        };
        var host = new Border { Child = carousel };

        ShowInWindow(host, () =>
        {
            var timer = GetPrivateField<DispatcherTimer>(typeof(AtomUICarousel), carousel, "_autoPlayTimer");
            timer.IsEnabled.ShouldBeTrue();

            host.IsVisible = false;
            Dispatcher.UIThread.RunJobs();

            timer.IsEnabled.ShouldBeFalse();

            host.IsVisible = true;
            Dispatcher.UIThread.RunJobs();

            timer.IsEnabled.ShouldBeTrue();
        });
    }

    [Fact]
    public void Carousel_Progress_Animation_Pauses_When_Effectively_Invisible()
    {
        var indicator = new CarouselPageIndicator
        {
            IsSelected               = true,
            IsShowTransitionProgress = true
        };
        var host = new Border { Child = indicator };

        InvokePrivateMethod(
            typeof(CarouselPageIndicator),
            indicator,
            "BuildProgressAnimation",
            true);

        var animation = GetPrivateField<Animation>(typeof(CarouselPageIndicator), indicator, "_animation");
        animation.PlaybackBehavior.ShouldBe(PlaybackBehavior.OnlyIfVisible);

        ShowInWindow(host, () =>
        {
            GetPrivateFieldValue(typeof(CarouselPageIndicator), indicator, "_cancellationTokenSource")
                .ShouldNotBeNull();

            host.IsVisible = false;
            Dispatcher.UIThread.RunJobs();

            GetPrivateFieldValue(typeof(CarouselPageIndicator), indicator, "_cancellationTokenSource")
                .ShouldBeNull();
            indicator.ProgressValue.ShouldBe(0d);

            host.IsVisible = true;
            Dispatcher.UIThread.RunJobs();

            GetPrivateFieldValue(typeof(CarouselPageIndicator), indicator, "_cancellationTokenSource")
                .ShouldNotBeNull();
        });
    }

    [Fact]
    public void TimerStatistic_Timer_Pauses_When_An_Ancestor_Is_Hidden()
    {
        var statistic = new TimerStatistic
        {
            Value           = DateTime.Now.AddMinutes(1),
            RefreshDuration = TimeSpan.FromSeconds(1)
        };
        var host = new Border { Child = statistic };

        ShowInWindow(host, () =>
        {
            var timer = GetPrivateField<DispatcherTimer>(typeof(TimerStatistic), statistic, "_timer");
            timer.IsEnabled.ShouldBeTrue();

            host.IsVisible = false;
            Dispatcher.UIThread.RunJobs();

            timer.IsEnabled.ShouldBeFalse();

            host.IsVisible = true;
            Dispatcher.UIThread.RunJobs();

            timer.IsEnabled.ShouldBeTrue();
        });
    }

    private static T GetPrivateField<T>(Type ownerType, object instance, string fieldName)
    {
        return GetPrivateFieldValue(ownerType, instance, fieldName).ShouldBeOfType<T>();
    }

    private static object? GetPrivateFieldValue(Type ownerType, object instance, string fieldName)
    {
        var field = ownerType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        field.ShouldNotBeNull();
        return field.GetValue(instance);
    }

    private static void SetPrivateField(Type ownerType, object instance, string fieldName, object value)
    {
        var field = ownerType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        field.ShouldNotBeNull();
        field.SetValue(instance, value);
    }

    private static void InvokePrivateMethod(
        Type ownerType,
        object instance,
        string methodName,
        params object?[]? parameters)
    {
        var method = ownerType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        method.ShouldNotBeNull();
        method.Invoke(instance, parameters);
    }

    private static T InvokePrivateMethod<T>(Type ownerType, object instance, string methodName)
    {
        var method = ownerType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        method.ShouldNotBeNull();
        return method.Invoke(instance, null).ShouldBeOfType<T>();
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 320,
            Height  = 220,
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
            Dispatcher.UIThread.RunJobs();
        }
    }

    private sealed class TestIcon : Icon
    {
    }
}
