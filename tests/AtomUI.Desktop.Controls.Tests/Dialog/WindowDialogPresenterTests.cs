using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AtomUI.Icons.AntDesign;
using AtomUI.Theme.Resources;
using AtomUI.Theme.Styling;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Dialog;

public class WindowDialogPresenterTests
{
    static WindowDialogPresenterTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void ShowAsync_Completes_When_The_Native_Window_Opens()
    {
        RunOnUIThread(ShowAsync_Completes_When_The_Native_Window_Opens_Core);
    }

    private static void ShowAsync_Completes_When_The_Native_Window_Opens_Core()
    {
        var owner = new AtomUI.Desktop.Controls.Window
        {
            Width = 800,
            Height = 600
        };
        var dialog = CreateAnimatedDialog();
        var presenter = new WindowDialogPresenter(dialog, owner);

        try
        {
            owner.Show();
            var showTask = presenter.ShowAsync(CancellationToken.None).AsTask();
            PumpUntil(() => presenter.HostWindow.IsVisible);

            WaitWithDispatcherPump(showTask);
            presenter.HostWindow.GetVisualDescendants()
                     .OfType<AtomUI.Controls.Primitives.MotionActor>()
                     .ShouldBeEmpty();
        }
        finally
        {
            WaitWithDispatcherPump(presenter.CloseAsync().AsTask());
            WaitWithDispatcherPump(presenter.DisposeAsync().AsTask());
            owner.Close();
        }
    }

    [Fact]
    public void CloseAsync_Closes_The_Native_Window_Without_Content_Motion()
    {
        RunOnUIThread(CloseAsync_Closes_The_Native_Window_Without_Content_Motion_Core);
    }

    private static void CloseAsync_Closes_The_Native_Window_Without_Content_Motion_Core()
    {
        var owner = new AtomUI.Desktop.Controls.Window
        {
            Width = 800,
            Height = 600
        };
        var dialog = CreateAnimatedDialog();
        var presenter = new WindowDialogPresenter(dialog, owner);

        try
        {
            owner.Show();
            WaitWithDispatcherPump(presenter.ShowAsync(CancellationToken.None).AsTask());

            var closeTask = presenter.CloseAsync().AsTask();
            Dispatcher.UIThread.RunJobs();

            WaitWithDispatcherPump(closeTask);
            presenter.HostWindow.IsVisible.ShouldBeFalse();
        }
        finally
        {
            WaitWithDispatcherPump(presenter.DisposeAsync().AsTask());
            owner.Close();
        }
    }

    [Fact]
    public void CloseAsync_After_ShowAsync_Completes_Teardown()
    {
        RunOnUIThread(CloseAsync_After_ShowAsync_Completes_Teardown_Core);
    }

    private static void CloseAsync_After_ShowAsync_Completes_Teardown_Core()
    {
        var owner = new AtomUI.Desktop.Controls.Window
        {
            Width = 800,
            Height = 600
        };
        var dialog = CreateAnimatedDialog();
        var presenter = new WindowDialogPresenter(dialog, owner);

        try
        {
            owner.Show();
            var showTask = presenter.ShowAsync(CancellationToken.None).AsTask();
            WaitWithDispatcherPump(presenter.CloseAsync().AsTask());

            WaitWithDispatcherPump(showTask);
            presenter.HostWindow.IsVisible.ShouldBeFalse();
        }
        finally
        {
            WaitWithDispatcherPump(presenter.DisposeAsync().AsTask());
            owner.Close();
        }
    }

    [Theory]
    [InlineData(double.NaN, double.NaN)]
    [InlineData(double.NaN, 220)]
    [InlineData(360, double.NaN)]
    [InlineData(360, 220)]
    public void Sizing_Measures_Natural_Dimensions_Before_Show(double width, double height)
    {
        var owner = new AtomUI.Desktop.Controls.Window
        {
            Width = 800,
            Height = 600
        };
        var dialog = new AtomUI.Desktop.Controls.Dialog
        {
            Content = new Border { Width = 180, Height = 70 },
            IsModal = false,
            IsMotionEnabled = false,
            HostWidth = width,
            HostHeight = height
        };
        var presenter = new WindowDialogPresenter(dialog, owner);

        try
        {
            owner.Show();
            WaitWithDispatcherPump(presenter.ShowAsync(CancellationToken.None).AsTask());

            if (double.IsNaN(width))
            {
                presenter.HostWindow.ClientSize.Width.ShouldBeGreaterThanOrEqualTo(180);
                presenter.HostWindow.ClientSize.Width.ShouldBeLessThan(owner.ClientSize.Width);
            }
            else
            {
                presenter.HostWindow.ClientSize.Width.ShouldBe(width);
            }

            if (double.IsNaN(height))
            {
                presenter.HostWindow.ClientSize.Height.ShouldBeGreaterThanOrEqualTo(70);
                presenter.HostWindow.ClientSize.Height.ShouldBeLessThan(owner.ClientSize.Height);
            }
            else
            {
                presenter.HostWindow.ClientSize.Height.ShouldBe(height);
            }

            presenter.HostWindow.SizeToContent.ShouldBe(SizeToContent.Manual);
        }
        finally
        {
            WaitWithDispatcherPump(presenter.CloseAsync().AsTask());
            WaitWithDispatcherPump(presenter.DisposeAsync().AsTask());
            owner.Close();
        }
    }

    [Fact]
    public void Open_Window_Tracks_Dialog_Size_Constraints()
    {
        var owner = new AtomUI.Desktop.Controls.Window
        {
            Width = 800,
            Height = 600
        };
        var dialog = new AtomUI.Desktop.Controls.Dialog
        {
            IsModal = false,
            IsMotionEnabled = false,
            HostWidth = 360,
            HostHeight = 220
        };
        var presenter = new WindowDialogPresenter(dialog, owner);

        try
        {
            owner.Show();
            WaitWithDispatcherPump(presenter.ShowAsync(CancellationToken.None).AsTask());

            dialog.HostWidth = double.NaN;
            dialog.HostHeight = 260;
            dialog.HostMinWidth = 240;
            dialog.HostMinHeight = 180;
            dialog.HostMaxWidth = 480;
            dialog.HostMaxHeight = 320;
            Dispatcher.UIThread.RunJobs();

            presenter.HostWindow.ClientSize.Height.ShouldBe(260);
            presenter.HostWindow.MinWidth.ShouldBe(240);
            presenter.HostWindow.MinHeight.ShouldBe(180);
            presenter.HostWindow.MaxWidth.ShouldBe(480);
            presenter.HostWindow.MaxHeight.ShouldBe(320);
            presenter.HostWindow.SizeToContent.ShouldBe(SizeToContent.Manual);

            dialog.HostWidth = 420;
            Dispatcher.UIThread.RunJobs();

            presenter.HostWindow.ClientSize.Width.ShouldBe(420);
            presenter.HostWindow.SizeToContent.ShouldBe(SizeToContent.Manual);
        }
        finally
        {
            WaitWithDispatcherPump(presenter.CloseAsync().AsTask());
            WaitWithDispatcherPump(presenter.DisposeAsync().AsTask());
            owner.Close();
        }
    }

    [Fact]
    public void Presenter_Applies_Dialog_Startup_Placement_After_Initial_Measure()
    {
        var owner = new AtomUI.Desktop.Controls.Window
        {
            Width = 800,
            Height = 600,
            Position = new PixelPoint(100, 80)
        };
        var dialog = new AtomUI.Desktop.Controls.Dialog
        {
            IsModal = false,
            IsMotionEnabled = false,
            HostWidth = 240,
            HostHeight = 160,
            HorizontalStartupLocation = DialogHorizontalAnchor.Right,
            VerticalStartupLocation = DialogVerticalAnchor.Bottom
        };
        var presenter = new WindowDialogPresenter(dialog, owner);

        try
        {
            owner.Show();
            WaitWithDispatcherPump(presenter.ShowAsync(CancellationToken.None).AsTask());

            var ownerOrigin = new Point(
                owner.Position.X / owner.DesktopScaling,
                owner.Position.Y / owner.DesktopScaling);
            var offset = dialog.CalculatePlacementOffset(
                presenter.HostWindow.ClientSize,
                owner.ClientSize);
            var expected = new PixelPoint(
                (int)Math.Round((ownerOrigin.X + offset.X) * presenter.HostWindow.DesktopScaling),
                (int)Math.Round((ownerOrigin.Y + offset.Y) * presenter.HostWindow.DesktopScaling));

            presenter.HostWindow.Position.ShouldBe(expected);
        }
        finally
        {
            WaitWithDispatcherPump(presenter.CloseAsync().AsTask());
            WaitWithDispatcherPump(presenter.DisposeAsync().AsTask());
            owner.Close();
        }
    }

    [Fact]
    public void Presenter_Uses_Attached_Dialog_Resources_And_Releases_The_Resource_Parent()
    {
        var resourceKey = new object();
        var resourceValue = new object();
        var dialog = new AtomUI.Desktop.Controls.Dialog
        {
            IsModal = false,
            IsMotionEnabled = false,
            HostWidth = 320,
            HostHeight = 180
        };
        dialog.Resources[resourceKey] = resourceValue;
        var owner = new AtomUI.Desktop.Controls.Window
        {
            Width = 800,
            Height = 600,
            Content = dialog
        };
        var presenter = new WindowDialogPresenter(dialog, owner);

        try
        {
            owner.Show();
            WaitWithDispatcherPump(presenter.ShowAsync(CancellationToken.None).AsTask());

            var surface = presenter.HostWindow.GetVisualDescendants()
                                   .OfType<DialogSurface>()
                                   .Single();
            ((ILogical)presenter.HostWindow).LogicalParent.ShouldBeSameAs(dialog);
            ((IStyleHost)surface).StylingParent.ShouldBeSameAs(dialog);
            surface.TryFindResource(resourceKey, out var resolved).ShouldBeTrue();
            resolved.ShouldBeSameAs(resourceValue);

            WaitWithDispatcherPump(presenter.CloseAsync().AsTask());
            WaitWithDispatcherPump(presenter.DisposeAsync().AsTask());

            surface.TryFindResource(resourceKey, out _).ShouldBeFalse();
        }
        finally
        {
            owner.Close();
        }
    }

    [Fact]
    public void Owner_Close_Forces_A_Modal_Window_Dialog_To_Complete()
    {
        var placementTarget = new Border { Width = 100, Height = 40 };
        var dialog = new AtomUI.Desktop.Controls.Dialog
        {
            PlacementTarget = placementTarget,
            DialogHostType = DialogHostType.Window,
            IsModal = true,
            IsMotionEnabled = false,
            HostWidth = 320,
            HostHeight = 180
        };
        var root = new StackPanel
        {
            Children = { placementTarget, dialog }
        };
        var owner = new AtomUI.Desktop.Controls.Window
        {
            Width = 800,
            Height = 600,
            Content = root
        };
        var opened = false;
        dialog.Opened += (_, _) => opened = true;

        try
        {
            owner.Show();
            var sessionTask = dialog.OpenAsync(TestContext.Current.CancellationToken);
            PumpUntil(() => opened);

            owner.Close();

            WaitWithDispatcherPump(sessionTask);
            dialog.IsOpen.ShouldBeFalse();
            owner.IsVisible.ShouldBeFalse();
        }
        finally
        {
            if (owner.IsVisible)
            {
                owner.Close();
            }
        }
    }

    [Fact]
    public void Presenter_Uses_The_Shared_Surface_Inside_One_Native_Window()
    {
        var owner = new AtomUI.Desktop.Controls.Window
        {
            Width = 800,
            Height = 600
        };
        var dialog = new AtomUI.Desktop.Controls.Dialog
        {
            Title = "Window dialog",
            Content = "Dialog content",
            IsModal = false,
            IsMotionEnabled = false,
            HostWidth = 360,
            HostHeight = 220
        };
        var presenter = new WindowDialogPresenter(dialog, owner);

        try
        {
            owner.Show();
            WaitWithDispatcherPump(presenter.ShowAsync(CancellationToken.None).AsTask());

            presenter.HostWindow.IsVisible.ShouldBeTrue();
            var surface = presenter.HostWindow.GetVisualDescendants()
                                   .OfType<DialogSurface>()
                                   .Single();
            surface.Content.ShouldBe("Dialog content");
            surface.IsHeaderVisible.ShouldBeFalse();

            WaitWithDispatcherPump(presenter.CloseAsync().AsTask());
            WaitWithDispatcherPump(presenter.DisposeAsync().AsTask());
            WaitWithDispatcherPump(presenter.DisposeAsync().AsTask());

            presenter.HostWindow.IsVisible.ShouldBeFalse();
        }
        finally
        {
            owner.Close();
        }
    }

    [Fact]
    public void Native_Window_Uses_Native_Resize_Without_The_Overlay_Resizer()
    {
        var owner = new AtomUI.Desktop.Controls.Window
        {
            Width = 800,
            Height = 600
        };
        var dialog = new AtomUI.Desktop.Controls.Dialog
        {
            IsModal = false,
            IsMotionEnabled = false,
            IsResizable = true
        };
        var presenter = new WindowDialogPresenter(dialog, owner);

        try
        {
            owner.Show();
            WaitWithDispatcherPump(presenter.ShowAsync(CancellationToken.None).AsTask());

            presenter.HostWindow.CanResize.ShouldBeTrue();
            var surface = presenter.HostWindow.GetVisualDescendants()
                                   .OfType<DialogSurface>()
                                   .Single();
            surface.IsResizable.ShouldBeFalse();
            surface.Resizer.ShouldNotBeNull();
            surface.Resizer.IsVisible.ShouldBeFalse();

            WaitWithDispatcherPump(presenter.CloseAsync().AsTask());
            WaitWithDispatcherPump(presenter.DisposeAsync().AsTask());
        }
        finally
        {
            owner.Close();
        }
    }

    [Fact]
    public void Window_Surface_Does_Not_Render_The_Overlay_Shadow()
    {
        var owner = new AtomUI.Desktop.Controls.Window
        {
            Width = 800,
            Height = 600
        };
        var dialog = new AtomUI.Desktop.Controls.Dialog
        {
            IsModal = false,
            IsMotionEnabled = false,
            HostWidth = 320,
            HostHeight = 180
        };
        var presenter = new WindowDialogPresenter(dialog, owner);

        try
        {
            owner.Show();
            WaitWithDispatcherPump(presenter.ShowAsync(CancellationToken.None).AsTask());

            var shadowHost = presenter.HostWindow.GetVisualDescendants()
                                      .OfType<ShadowsAwareContainer>()
                                      .Single();

            shadowHost.BoxShadow.Count.ShouldBe(0);
        }
        finally
        {
            WaitWithDispatcherPump(presenter.CloseAsync().AsTask());
            WaitWithDispatcherPump(presenter.DisposeAsync().AsTask());
            owner.Close();
        }
    }

    [Fact]
    public void Native_Window_Is_The_Only_Owner_Of_The_Title_Icon()
    {
        var owner = new AtomUI.Desktop.Controls.Window
        {
            Width = 800,
            Height = 600
        };
        var titleIcon = new InfoCircleOutlined();
        var dialog = new AtomUI.Desktop.Controls.Dialog
        {
            IsModal = false,
            IsMotionEnabled = false,
            TitleIcon = titleIcon
        };
        var presenter = new WindowDialogPresenter(dialog, owner);

        try
        {
            owner.Show();
            WaitWithDispatcherPump(presenter.ShowAsync(CancellationToken.None).AsTask());

            presenter.HostWindow.Logo.ShouldBeSameAs(titleIcon);
            var surface = presenter.HostWindow.GetVisualDescendants()
                                   .OfType<DialogSurface>()
                                   .Single();
            surface.TitleIcon.ShouldBeNull();
            surface.Header.ShouldNotBeNull().Logo.ShouldBeNull();

            WaitWithDispatcherPump(presenter.CloseAsync().AsTask());
            WaitWithDispatcherPump(presenter.DisposeAsync().AsTask());
        }
        finally
        {
            owner.Close();
        }
    }

    [Fact]
    public void Dispose_Releases_Content_When_Close_Fails()
    {
        var expectedException = new InvalidOperationException("window close failed");
        var owner = new AtomUI.Desktop.Controls.Window
        {
            Width = 800,
            Height = 600
        };
        var presenter = new WindowDialogPresenter(
            new AtomUI.Desktop.Controls.Dialog
            {
                IsModal = false,
                IsMotionEnabled = false
            },
            owner);
        EventHandler<Avalonia.Controls.WindowClosingEventArgs> failingHandler =
            (_, _) => throw expectedException;

        try
        {
            owner.Show();
            WaitWithDispatcherPump(presenter.ShowAsync(CancellationToken.None).AsTask());
            presenter.HostWindow.Closing += failingHandler;

            Should.Throw<InvalidOperationException>(() =>
                WaitWithDispatcherPump(presenter.CloseAsync().AsTask())).ShouldBeSameAs(expectedException);
            Should.Throw<InvalidOperationException>(() =>
                WaitWithDispatcherPump(presenter.DisposeAsync().AsTask())).ShouldBeSameAs(expectedException);

            presenter.HostWindow.Content.ShouldBeNull();
        }
        finally
        {
            presenter.HostWindow.Closing -= failingHandler;
            if (presenter.HostWindow.IsVisible)
            {
                presenter.HostWindow.CloseFromPresenter();
            }
            owner.Close();
        }
    }

    private static void WaitWithDispatcherPump(Task task)
    {
        var timeoutAt = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(5);
        while (!task.IsCompleted && DateTimeOffset.UtcNow < timeoutAt)
        {
            Dispatcher.UIThread.RunJobs();
            Thread.Sleep(1);
        }

        task.IsCompleted.ShouldBeTrue();
        task.GetAwaiter().GetResult();
    }

    private static void PumpUntil(Func<bool> condition)
    {
        var timeoutAt = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(5);
        while (!condition() && DateTimeOffset.UtcNow < timeoutAt)
        {
            Dispatcher.UIThread.RunJobs();
            Thread.Sleep(1);
        }

        condition().ShouldBeTrue();
    }

    private static void RunOnUIThread(Action action)
    {
        Dispatcher.UIThread.Invoke(action);
    }

    private static AtomUI.Desktop.Controls.Dialog CreateAnimatedDialog()
    {
        var dialog = new AtomUI.Desktop.Controls.Dialog
        {
            Content = new Border { Width = 180, Height = 70 },
            IsModal = false,
            IsMotionEnabled = true,
            HostWidth = 360,
            HostHeight = 220
        };
        dialog.Resources[new ControlSharedTokenResourceKey(
            "AtomUI",
            "Dialog",
            SharedTokenKind.MotionDurationMid)] = TimeSpan.FromMilliseconds(120);
        return dialog;
    }
}
