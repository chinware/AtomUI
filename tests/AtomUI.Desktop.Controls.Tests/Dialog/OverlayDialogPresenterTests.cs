using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using AtomUI.Desktop.Controls.Tests.Window;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;

using BindingFlags = System.Reflection.BindingFlags;

namespace AtomUI.Desktop.Controls.Tests.Dialog;

public class OverlayDialogPresenterTests
{
    static OverlayDialogPresenterTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Modal_Blocks_Background_Pointer_Input_While_Modeless_Allows_It(bool isModal)
    {
        RunOnUIThread(() =>
        {
            var backgroundPressCount = 0;
            var closeRequestCount = 0;
            var background = new Border
            {
                Width = 640,
                Height = 480,
                Background = Brushes.Transparent,
                Focusable = true,
                IsHitTestVisible = true
            };
            var root = new ScopeAwareOverlayLayerPanel
            {
                Children = { background }
            };
            var window = new AtomUI.Desktop.Controls.Window
            {
                Width = 640,
                Height = 480,
                Content = root
            };
            var dialog = new AtomUI.Desktop.Controls.Dialog
            {
                IsModal = isModal,
                IsMotionEnabled = false,
                HostWidth = 320,
                HostHeight = 180
            };
            var presenter = new OverlayDialogPresenter(dialog, background);
            background.PointerPressed += (_, _) => backgroundPressCount++;
            presenter.CloseRequested += (_, _) => closeRequestCount++;

            try
            {
                window.Show();
                Dispatcher.UIThread.RunJobs();
                WaitWithDispatcherPump(presenter.ShowAsync(CancellationToken.None).AsTask());
                Dispatcher.UIThread.RunJobs();
                AvaloniaHeadlessPlatform.ForceRenderTimerTick(1);

                var point = root.TranslatePoint(
                    new Point(20, root.Bounds.Height - 20),
                    window).ShouldNotBeNull();
                window.MouseMove(point);
                window.MouseDown(point, MouseButton.Left);
                window.MouseUp(point, MouseButton.Left);
                Dispatcher.UIThread.RunJobs();

                if (isModal)
                {
                    backgroundPressCount.ShouldBe(0);
                    closeRequestCount.ShouldBe(1);
                }
                else
                {
                    backgroundPressCount.ShouldBe(1);
                    closeRequestCount.ShouldBe(0);
                }
            }
            finally
            {
                WaitWithDispatcherPump(presenter.CloseAsync().AsTask());
                WaitWithDispatcherPump(presenter.DisposeAsync().AsTask());
                window.Close();
            }
        });
    }

    [Fact]
    public void Presenter_Owns_Mask_And_Surface_In_The_TopLevel_Dialog_Layer()
    {
        RunOnUIThread(Presenter_Owns_Mask_And_Surface_In_The_TopLevel_Dialog_Layer_Core);
    }

    private static void Presenter_Owns_Mask_And_Surface_In_The_TopLevel_Dialog_Layer_Core()
    {
        var placementTarget = new Border { Width = 100, Height = 40 };
        var root = new ScopeAwareOverlayLayerPanel
        {
            Children = { placementTarget }
        };
        var window = new AtomUI.Desktop.Controls.Window
        {
            Width = 640,
            Height = 480,
            Content = root
        };
        var dialog = new AtomUI.Desktop.Controls.Dialog
        {
            Content = "Dialog content",
            IsModal = true,
            IsMotionEnabled = false,
            HostWidth = 320,
            HostHeight = 180
        };
        var presenter = new OverlayDialogPresenter(dialog, placementTarget);

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            WaitWithDispatcherPump(presenter.ShowAsync(CancellationToken.None).AsTask());
            Dispatcher.UIThread.RunJobs();

            var dialogLayer = presenter.Parent.ShouldBeOfType<DialogOverlayLayer>();
            dialogLayer.Parent.ShouldNotBeSameAs(ScopeAwareOverlayLayer.GetLayer(placementTarget));
            TopLevel.GetTopLevel(dialogLayer.Parent as Visual).ShouldBeSameAs(window);
            dialogLayer.Bounds.Size.ShouldBe(window.ClientSize);
            presenter.GetVisualDescendants().OfType<OverlayDialogMask>().Count().ShouldBe(1);
            presenter.GetVisualDescendants().OfType<DialogSurface>().Count().ShouldBe(1);

            WaitWithDispatcherPump(presenter.CloseAsync().AsTask());
            WaitWithDispatcherPump(presenter.DisposeAsync().AsTask());
            WaitWithDispatcherPump(presenter.DisposeAsync().AsTask());

            presenter.Parent.ShouldBeNull();
            dialogLayer.Parent.ShouldBeNull();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void ShowAsync_Initializes_Motion_Actors_Before_The_First_Loaded_Frame()
    {
        RunOnUIThread(() =>
        {
            var placementTarget = new Border { Width = 100, Height = 40 };
            var root = new ScopeAwareOverlayLayerPanel
            {
                Children = { placementTarget }
            };
            var window = new AtomUI.Desktop.Controls.Window
            {
                Width = 640,
                Height = 480,
                Content = root
            };
            var presenter = new OverlayDialogPresenter(
                new AtomUI.Desktop.Controls.Dialog
                {
                    IsModal = true,
                    IsMotionEnabled = true,
                    HostWidth = 320,
                    HostHeight = 180
                },
                placementTarget);

            try
            {
                window.Show();
                Dispatcher.UIThread.RunJobs();

                var showTask = presenter.ShowAsync(CancellationToken.None).AsTask();
                var actors = presenter.GetVisualDescendants().OfType<MotionActor>().ToArray();

                actors.Single(actor => actor.Name == "PART_SurfaceMotionActor").Opacity.ShouldBe(0);
                actors.Single(actor => actor.Name == "PART_MaskMotionActor").Opacity.ShouldBe(0);

                WaitWithDispatcherPump(showTask);
            }
            finally
            {
                WaitWithDispatcherPump(presenter.CloseAsync().AsTask());
                WaitWithDispatcherPump(presenter.DisposeAsync().AsTask());
                window.Close();
            }
        });
    }

    [Fact]
    public void Modeless_ShowAsync_Never_Makes_The_Mask_Visible()
    {
        RunOnUIThread(() =>
        {
            var placementTarget = new Border { Width = 100, Height = 40 };
            var root = new ScopeAwareOverlayLayerPanel
            {
                Children = { placementTarget }
            };
            var window = new AtomUI.Desktop.Controls.Window
            {
                Width = 640,
                Height = 480,
                Content = root
            };
            var presenter = new OverlayDialogPresenter(
                new AtomUI.Desktop.Controls.Dialog
                {
                    IsModal = false,
                    IsMotionEnabled = true,
                    HostWidth = 320,
                    HostHeight = 180
                },
                placementTarget);

            try
            {
                window.Show();
                Dispatcher.UIThread.RunJobs();

                var showTask = presenter.ShowAsync(CancellationToken.None).AsTask();
                var maskActor = presenter.GetVisualDescendants()
                                         .OfType<MotionActor>()
                                         .Single(actor => actor.Name == "PART_MaskMotionActor");

                maskActor.IsVisible.ShouldBeFalse();
                WaitWithDispatcherPump(showTask);
                maskActor.IsVisible.ShouldBeFalse();
            }
            finally
            {
                WaitWithDispatcherPump(presenter.CloseAsync().AsTask());
                WaitWithDispatcherPump(presenter.DisposeAsync().AsTask());
                window.Close();
            }
        });
    }

    [Fact]
    public void Overlay_Surface_Renders_The_Dialog_Shadow()
    {
        RunOnUIThread(() =>
        {
            var fixture = ShowPresenter(new AtomUI.Desktop.Controls.Dialog
            {
                IsModal = false,
                IsMotionEnabled = false,
                HostWidth = 320,
                HostHeight = 180
            });

            try
            {
                var shadowHost = fixture.Presenter.Surface.GetVisualDescendants()
                                        .OfType<ShadowsAwareContainer>()
                                        .Single();

                shadowHost.IsOverlayMode.ShouldBeTrue();
                shadowHost.BoxShadow.Count.ShouldBeGreaterThan(0);
            }
            finally
            {
                fixture.Dispose();
            }
        });
    }

    [Fact]
    public void Presenter_Uses_Attached_Dialog_Resources_And_Releases_The_Resource_Parent()
    {
        RunOnUIThread(() =>
        {
            var resourceKey = new object();
            var resourceValue = new object();
            var placementTarget = new Border { Width = 100, Height = 40 };
            var dialog = new AtomUI.Desktop.Controls.Dialog
            {
                IsMotionEnabled = false,
                HostWidth = 320,
                HostHeight = 180
            };
            dialog.Resources[resourceKey] = resourceValue;
            var root = new ScopeAwareOverlayLayerPanel
            {
                Children = { placementTarget, dialog }
            };
            var window = new AtomUI.Desktop.Controls.Window
            {
                Width = 640,
                Height = 480,
                Content = root
            };
            var presenter = new OverlayDialogPresenter(dialog, placementTarget);

            try
            {
                window.Show();
                Dispatcher.UIThread.RunJobs();
                WaitWithDispatcherPump(presenter.ShowAsync(CancellationToken.None).AsTask());

                presenter.TryFindResource(resourceKey, out var resolved).ShouldBeTrue();
                resolved.ShouldBeSameAs(resourceValue);

                WaitWithDispatcherPump(presenter.CloseAsync().AsTask());
                WaitWithDispatcherPump(presenter.DisposeAsync().AsTask());

                presenter.TryFindResource(resourceKey, out _).ShouldBeFalse();
            }
            finally
            {
                window.Close();
            }
        });
    }

    [Fact]
    public void Natural_Size_Uses_The_Surface_Measure_Without_A_Fixed_Fallback()
    {
        RunOnUIThread(() =>
        {
            var fixture = ShowPresenter(new AtomUI.Desktop.Controls.Dialog
            {
                Content = new Border
                {
                    Width = 180,
                    Height = 70
                },
                IsFooterVisible = false,
                IsMotionEnabled = false
            });

            try
            {
                fixture.Presenter.Surface.Width.ShouldBe(double.NaN);
                fixture.Presenter.Surface.Height.ShouldBe(double.NaN);
                fixture.Presenter.Surface.Bounds.Width.ShouldBeGreaterThanOrEqualTo(180);
                fixture.Presenter.Surface.Bounds.Width.ShouldBeLessThan(520);
                fixture.Presenter.Surface.Bounds.Height.ShouldBeGreaterThanOrEqualTo(70);
                fixture.Presenter.Surface.Bounds.Height.ShouldBeLessThan(240);
            }
            finally
            {
                fixture.Dispose();
            }
        });
    }

    [Fact]
    public void Open_Presenter_Tracks_Dialog_Size_Constraints()
    {
        RunOnUIThread(() =>
        {
            var fixture = ShowPresenter(new AtomUI.Desktop.Controls.Dialog
            {
                IsMotionEnabled = false,
                HostWidth = 320,
                HostHeight = 180
            });

            try
            {
                fixture.Dialog.HostWidth = 410;
                fixture.Dialog.HostHeight = 230;
                fixture.Dialog.HostMinWidth = 300;
                fixture.Dialog.HostMinHeight = 160;
                fixture.Dialog.HostMaxWidth = 450;
                fixture.Dialog.HostMaxHeight = 260;
                Dispatcher.UIThread.RunJobs();

                fixture.Presenter.Surface.Width.ShouldBe(410);
                fixture.Presenter.Surface.Height.ShouldBe(230);
                fixture.Presenter.Surface.MinWidth.ShouldBe(300);
                fixture.Presenter.Surface.MinHeight.ShouldBe(160);
                fixture.Presenter.Surface.MaxWidth.ShouldBe(450);
                fixture.Presenter.Surface.MaxHeight.ShouldBe(260);
            }
            finally
            {
                fixture.Dispose();
            }
        });
    }

    [Fact]
    public void Default_Constraints_Protect_The_Surface_Structure()
    {
        RunOnUIThread(() =>
        {
            var fixture = ShowPresenter(new AtomUI.Desktop.Controls.Dialog
            {
                IsMotionEnabled = false,
                IsResizable = true,
                StandardButtons = DialogStandardButton.Ok | DialogStandardButton.Cancel,
                HostWidth = 320,
                HostHeight = 220
            });

            try
            {
                var structuralMinimum = fixture.Presenter.Surface.MeasureStructuralMinimum();

                fixture.Presenter.Surface.MinWidth.ShouldBe(structuralMinimum.Width);
                fixture.Presenter.Surface.MinHeight.ShouldBe(structuralMinimum.Height);
            }
            finally
            {
                fixture.Dispose();
            }
        });
    }

    [Fact]
    public void Runtime_Structural_Minimum_Changes_Clamp_And_Preserve_Actual_Surface_Size()
    {
        RunOnUIThread(() =>
        {
            var fixture = ShowPresenter(new AtomUI.Desktop.Controls.Dialog
            {
                IsMotionEnabled = false,
                IsResizable = true,
                HostWidth = 320,
                HostHeight = 60
            });

            try
            {
                fixture.Dialog.StandardButtons =
                    DialogStandardButton.Ok | DialogStandardButton.Cancel;
                Dispatcher.UIThread.RunJobs();

                var structuralMinimum = fixture.Presenter.Surface.MeasureStructuralMinimum();
                fixture.Presenter.Surface.MinHeight.ShouldBe(structuralMinimum.Height);
                fixture.Presenter.Surface.Bounds.Height.ShouldBe(structuralMinimum.Height);

                fixture.Dialog.StandardButtons = DialogStandardButton.NoButton;
                Dispatcher.UIThread.RunJobs();
                fixture.Presenter.Surface.Bounds.Height.ShouldBe(structuralMinimum.Height);
            }
            finally
            {
                fixture.Dispose();
            }
        });
    }

    [Fact]
    public void Runtime_Minimum_Clamp_Commits_A_Natural_Axis_As_Actual_Size()
    {
        RunOnUIThread(() =>
        {
            var fixture = ShowPresenter(new AtomUI.Desktop.Controls.Dialog
            {
                Content = new Border { Width = 180, Height = 70 },
                IsFooterVisible = false,
                IsMotionEnabled = false,
                IsResizable = true,
                HostWidth = double.NaN,
                HostHeight = 220,
                HostMaxWidth = 600
            });

            try
            {
                var naturalWidth = fixture.Presenter.Surface.Bounds.Width;
                var clampedWidth = naturalWidth + 40;

                fixture.Dialog.HostMinWidth = clampedWidth;
                Dispatcher.UIThread.RunJobs();

                fixture.Presenter.Surface.Bounds.Width.ShouldBe(clampedWidth);
                fixture.Presenter.Surface.Width.ShouldBe(clampedWidth);

                fixture.Dialog.HostMinWidth = 0;
                Dispatcher.UIThread.RunJobs();

                fixture.Presenter.Surface.Bounds.Width.ShouldBe(clampedWidth);
            }
            finally
            {
                fixture.Dispose();
            }
        });
    }

    [Fact]
    public void Repeated_Resize_Moves_Use_One_Origin_Instead_Of_Accumulating_Deltas()
    {
        RunOnUIThread(() =>
        {
            var fixture = ShowPresenter(new AtomUI.Desktop.Controls.Dialog
            {
                IsMotionEnabled = false,
                IsResizable = true,
                HostWidth = 320,
                HostHeight = 220
            });

            try
            {
                var originalWidth = fixture.Presenter.Surface.Bounds.Width;

                ResizeSurface(
                    fixture,
                    ResizeHandleLocation.East,
                    new Vector(10, 0),
                    new Vector(20, 0));

                fixture.Presenter.Surface.Bounds.Width.ShouldBe(originalWidth + 20);
            }
            finally
            {
                fixture.Dispose();
            }
        });
    }

    [Fact]
    public void Resize_Handle_Captures_The_Pointer_And_Ends_When_Capture_Is_Lost()
    {
        RunOnUIThread(() =>
        {
            var fixture = ShowPresenter(new AtomUI.Desktop.Controls.Dialog
            {
                IsMotionEnabled = false,
                IsResizable = true,
                HostWidth = 320,
                HostHeight = 220
            });

            try
            {
                var resizer = fixture.Presenter.Surface.Resizer.ShouldNotBeNull();
                var handle = resizer.GetVisualDescendants()
                                    .OfType<Border>()
                                    .Single(border => Equals(border.Tag, ResizeHandleLocation.East));
                var start = handle.TranslatePoint(
                    new Point(handle.Bounds.Width / 2, handle.Bounds.Height / 2),
                    fixture.Window).ShouldNotBeNull();
                var resizeStartedCount = 0;
                var resizeCompletedCount = 0;
                resizer.AboutToResize += (_, _) => resizeStartedCount++;
                resizer.ResizeCompleted += (_, _) => resizeCompletedCount++;

                var pointer = BeginDrag(handle, fixture.Window, start);
                pointer.Captured.ShouldBeSameAs(handle);
                MoveDrag(handle, fixture.Window, pointer, start + new Vector(20, 0), 1);
                Dispatcher.UIThread.RunJobs();
                resizeStartedCount.ShouldBe(1);

                pointer.Capture(fixture.Presenter);
                Dispatcher.UIThread.RunJobs();
                resizeCompletedCount.ShouldBe(1);
                pointer.Captured.ShouldBeSameAs(fixture.Presenter);
                pointer.Capture(null);

                var nextPointer = BeginDrag(handle, fixture.Window, start);
                MoveDrag(handle, fixture.Window, nextPointer, start + new Vector(10, 0), 2);
                Dispatcher.UIThread.RunJobs();
                resizeStartedCount.ShouldBe(2);
                EndDrag(handle, fixture.Window, nextPointer, start + new Vector(10, 0), 3);
            }
            finally
            {
                fixture.Dispose();
            }
        });
    }

    [Theory]
    [InlineData(ResizeHandleLocation.East, 30, 0)]
    [InlineData(ResizeHandleLocation.West, -30, 0)]
    [InlineData(ResizeHandleLocation.South, 0, 30)]
    [InlineData(ResizeHandleLocation.North, 0, -30)]
    public void Edge_Resize_Preserves_The_Opposite_Surface_Edge(
        ResizeHandleLocation location,
        double deltaX,
        double deltaY)
    {
        RunOnUIThread(() =>
        {
            var fixture = ShowPresenter(new AtomUI.Desktop.Controls.Dialog
            {
                IsMotionEnabled = false,
                IsResizable = true,
                HostWidth = 320,
                HostHeight = 220
            });

            try
            {
                var originalBounds = GetSurfaceBodyBounds(fixture.Presenter.Surface, fixture.Presenter);

                ResizeSurface(fixture, location, new Vector(deltaX, deltaY));

                var resizedBounds = GetSurfaceBodyBounds(fixture.Presenter.Surface, fixture.Presenter);
                if (location == ResizeHandleLocation.East)
                {
                    resizedBounds.Left.ShouldBe(originalBounds.Left);
                }
                else if (location == ResizeHandleLocation.West)
                {
                    resizedBounds.Right.ShouldBe(originalBounds.Right);
                }
                else if (location == ResizeHandleLocation.South)
                {
                    resizedBounds.Top.ShouldBe(originalBounds.Top);
                }
                else
                {
                    resizedBounds.Bottom.ShouldBe(originalBounds.Bottom);
                }
            }
            finally
            {
                fixture.Dispose();
            }
        });
    }

    [Fact]
    public void Runtime_Constraint_And_NaN_Changes_Do_Not_Reset_Valid_Actual_Size()
    {
        RunOnUIThread(() =>
        {
            var fixture = ShowPresenter(new AtomUI.Desktop.Controls.Dialog
            {
                IsMotionEnabled = false,
                IsResizable = true,
                HostWidth = 320,
                HostHeight = 220
            });

            try
            {
                ResizeSurface(fixture, ResizeHandleLocation.SouthEast, new Vector(50, 40));
                var resizedSize = fixture.Presenter.Surface.Bounds.Size;

                fixture.Dialog.HostMinWidth = 100;
                fixture.Dialog.HostMinHeight = 100;
                fixture.Dialog.HostWidth = double.NaN;
                fixture.Dialog.HostHeight = double.NaN;
                Dispatcher.UIThread.RunJobs();

                fixture.Presenter.Surface.Bounds.Size.ShouldBe(resizedSize);
            }
            finally
            {
                fixture.Dispose();
            }
        });
    }

    [Fact]
    public void Restore_Returns_To_The_User_Resized_Surface_Geometry()
    {
        RunOnUIThread(() =>
        {
            var fixture = ShowPresenter(new AtomUI.Desktop.Controls.Dialog
            {
                IsMotionEnabled = false,
                IsResizable = true,
                IsMaximizable = true,
                HostWidth = 320,
                HostHeight = 220
            });

            try
            {
                ResizeSurface(fixture, ResizeHandleLocation.SouthEast, new Vector(45, 35));
                var resizedBounds = GetSurfaceBodyBounds(fixture.Presenter.Surface, fixture.Presenter);
                var maximizeButton = fixture.Presenter.Surface.Header.ShouldNotBeNull()
                                            .GetVisualDescendants()
                                            .OfType<DialogCaptionButton>()
                                            .Single(button => button.Name == "PART_MaximizeButton");

                maximizeButton.RaiseEvent(new RoutedEventArgs(AtomUI.Desktop.Controls.Button.ClickEvent));
                Dispatcher.UIThread.RunJobs();
                maximizeButton.RaiseEvent(new RoutedEventArgs(AtomUI.Desktop.Controls.Button.ClickEvent));
                Dispatcher.UIThread.RunJobs();

                GetSurfaceBodyBounds(fixture.Presenter.Surface, fixture.Presenter).ShouldBe(resizedBounds);
            }
            finally
            {
                fixture.Dispose();
            }
        });
    }

    [Fact]
    public void CloseAsync_Keeps_The_Presenter_Attached_Until_Motion_Completes()
    {
        RunOnUIThread(CloseAsync_Keeps_The_Presenter_Attached_Until_Motion_Completes_Core);
    }

    private static void CloseAsync_Keeps_The_Presenter_Attached_Until_Motion_Completes_Core()
    {
        var placementTarget = new Border { Width = 100, Height = 40 };
        var root = new ScopeAwareOverlayLayerPanel
        {
            Children = { placementTarget }
        };
        var window = new AtomUI.Desktop.Controls.Window
        {
            Width = 640,
            Height = 480,
            Content = root
        };
        var dialog = new AtomUI.Desktop.Controls.Dialog
        {
            Content = "Dialog content",
            IsModal = true,
            IsMotionEnabled = true,
            HostWidth = 320,
            HostHeight = 180
        };
        var presenter = new OverlayDialogPresenter(dialog, placementTarget)
        {
            MotionDuration = TimeSpan.FromMilliseconds(80)
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            WaitWithDispatcherPump(presenter.ShowAsync(CancellationToken.None).AsTask());

            var closeTask = presenter.CloseAsync().AsTask();

            closeTask.IsCompleted.ShouldBeFalse();
            presenter.Parent.ShouldBeOfType<DialogOverlayLayer>();

            WaitWithDispatcherPump(closeTask);
            presenter.Parent.ShouldBeNull();
            WaitWithDispatcherPump(presenter.DisposeAsync().AsTask());
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void CloseAsync_During_Opening_Waits_For_Opening_To_Settle_Before_Closing_Motion()
    {
        RunOnUIThread(() =>
        {
            var placementTarget = new Border { Width = 100, Height = 40 };
            var root = new ScopeAwareOverlayLayerPanel
            {
                Children = { placementTarget }
            };
            var window = new AtomUI.Desktop.Controls.Window
            {
                Width = 640,
                Height = 480,
                Content = root
            };
            var dialog = new AtomUI.Desktop.Controls.Dialog
            {
                IsMotionEnabled = true,
                HostWidth = 320,
                HostHeight = 180
            };
            var presenter = new OverlayDialogPresenter(dialog, placementTarget)
            {
                MotionDuration = TimeSpan.FromMilliseconds(80)
            };

            try
            {
                window.Show();
                Dispatcher.UIThread.RunJobs();
                var showTask = presenter.ShowAsync(CancellationToken.None).AsTask();
                var surfaceActor = presenter.GetVisualDescendants()
                                            .OfType<MotionActor>()
                                            .Single(actor => actor.Name == "PART_SurfaceMotionActor");
                var motionStartCount = 0;
                bool? showCompletedWhenClosingStarted = null;
                surfaceActor.PreStart += (_, _) =>
                {
                    motionStartCount++;
                    if (motionStartCount == 2)
                    {
                        showCompletedWhenClosingStarted = showTask.IsCompleted;
                    }
                };

                Dispatcher.UIThread.RunJobs();
                motionStartCount.ShouldBe(1);

                var closeTask = presenter.CloseAsync().AsTask();
                WaitWithDispatcherPump(closeTask);

                showCompletedWhenClosingStarted.ShouldBe(true);
                WaitWithDispatcherPump(presenter.DisposeAsync().AsTask());
            }
            finally
            {
                window.Close();
            }
        });
    }

    [Fact]
    public void Dispose_Releases_Content_When_Close_Fails()
    {
        RunOnUIThread(() =>
        {
            var expectedException = new InvalidOperationException("overlay detach failed");
            var placementTarget = new Border { Width = 100, Height = 40 };
            var root = new ScopeAwareOverlayLayerPanel
            {
                Children = { placementTarget }
            };
            var window = new AtomUI.Desktop.Controls.Window
            {
                Width = 640,
                Height = 480,
                Content = root
            };
            var presenter = new OverlayDialogPresenter(
                new AtomUI.Desktop.Controls.Dialog { IsMotionEnabled = false },
                placementTarget);
            EventHandler<Avalonia.VisualTreeAttachmentEventArgs> failingHandler =
                (_, _) => throw expectedException;

            try
            {
                window.Show();
                WaitWithDispatcherPump(presenter.ShowAsync(CancellationToken.None).AsTask());
                presenter.DetachedFromVisualTree += failingHandler;

                Should.Throw<InvalidOperationException>(() =>
                    WaitWithDispatcherPump(presenter.CloseAsync().AsTask())).ShouldBeSameAs(expectedException);
                Should.Throw<InvalidOperationException>(() =>
                    WaitWithDispatcherPump(presenter.DisposeAsync().AsTask())).ShouldBeSameAs(expectedException);

                presenter.Content.ShouldBeNull();
            }
            finally
            {
                presenter.DetachedFromVisualTree -= failingHandler;
                window.Close();
            }
        });
    }

    [Fact]
    public void Header_Maximize_And_Restore_Use_The_Dialog_Layer_Bounds()
    {
        RunOnUIThread(Header_Maximize_And_Restore_Use_The_Dialog_Layer_Bounds_Core);
    }

    private static void Header_Maximize_And_Restore_Use_The_Dialog_Layer_Bounds_Core()
    {
        var placementTarget = new Border { Width = 100, Height = 40 };
        var root = new ScopeAwareOverlayLayerPanel
        {
            Children = { placementTarget }
        };
        var window = new AtomUI.Desktop.Controls.Window
        {
            Width = 640,
            Height = 480,
            Content = root
        };
        var dialog = new AtomUI.Desktop.Controls.Dialog
        {
            Content = "Dialog content",
            IsModal = false,
            IsMotionEnabled = false,
            IsMaximizable = true,
            HostWidth = 320,
            HostHeight = 180
        };
        var presenter = new OverlayDialogPresenter(dialog, placementTarget);

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            WaitWithDispatcherPump(presenter.ShowAsync(CancellationToken.None).AsTask());
            Dispatcher.UIThread.RunJobs();

            var originalSize = presenter.Surface.Bounds.Size;
            var maximizeButton = presenter.Surface.Header.ShouldNotBeNull()
                                              .GetVisualDescendants()
                                              .OfType<DialogCaptionButton>()
                                              .Single(button => button.Name == "PART_MaximizeButton");

            maximizeButton.RaiseEvent(new RoutedEventArgs(AtomUI.Desktop.Controls.Button.ClickEvent));
            Dispatcher.UIThread.RunJobs();

            presenter.Surface.IsDialogMaximized.ShouldBeTrue();
            presenter.Surface.Width.ShouldBe(presenter.Bounds.Width);
            presenter.Surface.Height.ShouldBe(presenter.Bounds.Height);

            maximizeButton.RaiseEvent(new RoutedEventArgs(AtomUI.Desktop.Controls.Button.ClickEvent));
            Dispatcher.UIThread.RunJobs();

            presenter.Surface.IsDialogMaximized.ShouldBeFalse();
            presenter.Surface.Width.ShouldBe(originalSize.Width);
            presenter.Surface.Height.ShouldBe(originalSize.Height);

            WaitWithDispatcherPump(presenter.CloseAsync().AsTask());
            WaitWithDispatcherPump(presenter.DisposeAsync().AsTask());
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Dialog_Layer_Activates_The_Whole_Presenter_Atomically()
    {
        RunOnUIThread(Dialog_Layer_Activates_The_Whole_Presenter_Atomically_Core);
    }

    private static void Dialog_Layer_Activates_The_Whole_Presenter_Atomically_Core()
    {
        var placementTarget = new Border { Width = 100, Height = 40 };
        var root = new ScopeAwareOverlayLayerPanel
        {
            Children = { placementTarget }
        };
        var window = new AtomUI.Desktop.Controls.Window
        {
            Width = 640,
            Height = 480,
            Content = root
        };
        var first = new OverlayDialogPresenter(
            new AtomUI.Desktop.Controls.Dialog { IsMotionEnabled = false },
            placementTarget);
        var second = new OverlayDialogPresenter(
            new AtomUI.Desktop.Controls.Dialog { IsMotionEnabled = false },
            placementTarget);

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            WaitWithDispatcherPump(first.ShowAsync(CancellationToken.None).AsTask());
            WaitWithDispatcherPump(second.ShowAsync(CancellationToken.None).AsTask());

            var layer = first.Parent.ShouldBeOfType<DialogOverlayLayer>();
            layer.Children.ShouldBe(new Control[] { first, second });

            layer.Activate(first);

            layer.Children.ShouldBe(new Control[] { second, first });

            WaitWithDispatcherPump(first.CloseAsync().AsTask());
            WaitWithDispatcherPump(second.CloseAsync().AsTask());
            WaitWithDispatcherPump(first.DisposeAsync().AsTask());
            WaitWithDispatcherPump(second.DisposeAsync().AsTask());
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Linux_Csd_Mask_Covers_The_Complete_Window_Layer()
    {
        RunOnUIThread(() =>
        {
            var decoration = new Thickness(14, 58, 22, 26);
            var fixture = ShowPresenter(
                new AtomUI.Desktop.Controls.Dialog
                {
                    IsModal = true,
                    IsMotionEnabled = false,
                    HostWidth = 320,
                    HostHeight = 180
                },
                window =>
                {
                    ConfigureLinuxWindow(window, isCsdEnabled: true, frameShadow: new Thickness(12));
                    SetPlatformDecorationMargin(window, decoration);
                });

            try
            {
                var maskActor = fixture.Presenter.GetVisualDescendants()
                                       .OfType<MotionActor>()
                                       .Single(actor => actor.Name == "PART_MaskMotionActor");

                maskActor.Margin.ShouldBe(default);
                maskActor.Bounds.Size.ShouldBe(fixture.Presenter.Bounds.Size);
                maskActor.GetVisualAncestors()
                         .OfType<WindowVisualLayerClip>()
                         .ShouldHaveSingleItem();
            }
            finally
            {
                fixture.Dispose();
            }
        });
    }

    [Theory]
    [InlineData(OsType.Windows, true)]
    [InlineData(OsType.Linux, true)]
    [InlineData(OsType.macOS, false)]
    public void Platform_Mask_Covers_The_Complete_Avalonia_Window_Layer(
        OsType osType,
        bool isCsdEnabled)
    {
        RunOnUIThread(() =>
        {
            var decoration = new Thickness(10, 48, 14, 18);
            var fixture = ShowPresenter(
                new AtomUI.Desktop.Controls.Dialog
                {
                    IsModal = true,
                    IsMotionEnabled = false,
                    HostWidth = 320,
                    HostHeight = 180
                },
                window =>
                {
                    window.SetValue(AtomUI.Desktop.Controls.Window.OsTypeProperty, osType);
                    window.IsCsdEnabled = isCsdEnabled;
                    window.FrameShadowThickness = new Thickness(12);
                    SetPlatformDecorationMargin(window, decoration);
                });

            try
            {
                var maskActor = fixture.Presenter.GetVisualDescendants()
                                       .OfType<MotionActor>()
                                       .Single(actor => actor.Name == "PART_MaskMotionActor");

                maskActor.Margin.ShouldBe(default);
                maskActor.Bounds.Size.ShouldBe(fixture.Presenter.Bounds.Size);
                if (osType != OsType.macOS)
                {
                    maskActor.GetVisualAncestors()
                             .OfType<WindowVisualLayerClip>()
                             .ShouldHaveSingleItem();
                }
            }
            finally
            {
                fixture.Dispose();
            }
        });
    }

    [Theory]
    [InlineData(OsType.Windows, true)]
    [InlineData(OsType.Windows, false)]
    [InlineData(OsType.Linux, true)]
    [InlineData(OsType.Linux, false)]
    [InlineData(OsType.macOS, true)]
    [InlineData(OsType.macOS, false)]
    public void Platform_Dialog_Owner_Bounds_Use_The_Window_Visible_Frame(
        OsType osType,
        bool isCsdEnabled)
    {
        RunOnUIThread(() =>
        {
            var decoration = new Thickness(14, 56, 20, 24);
            var frameShadow = new Thickness(8, 12, 16, 20);
            var fixture = ShowPresenter(
                new AtomUI.Desktop.Controls.Dialog
                {
                    IsMotionEnabled = false,
                    HostWidth = 2000,
                    HostHeight = 2000,
                    VerticalStartupLocation = DialogVerticalAnchor.Top,
                    HorizontalStartupLocation = DialogHorizontalAnchor.Left
                },
                window =>
                {
                    window.SetValue(AtomUI.Desktop.Controls.Window.OsTypeProperty, osType);
                    window.IsCsdEnabled = isCsdEnabled;
                    window.FrameShadowThickness = frameShadow;
                    SetPlatformDecorationMargin(window, decoration);
                });

            try
            {
                var expectedOwnerBounds = GetDialogBodyOwnerBounds(fixture, frameShadow);
                var surface = fixture.Presenter.Surface;

                surface.MaxWidth.ShouldBe(expectedOwnerBounds.Width);
                surface.MaxHeight.ShouldBe(expectedOwnerBounds.Height);
                surface.Bounds.Size.ShouldBe(expectedOwnerBounds.Size);
                GetSurfacePosition(surface, fixture.Presenter).ShouldBe(expectedOwnerBounds.Position);
                GetSurfaceBodyBounds(surface, fixture.Presenter).ShouldBe(expectedOwnerBounds);
            }
            finally
            {
                fixture.Dispose();
            }
        });
    }

    [Theory]
    [InlineData(OsType.Windows, true)]
    [InlineData(OsType.Windows, false)]
    [InlineData(OsType.Linux, true)]
    [InlineData(OsType.Linux, false)]
    [InlineData(OsType.macOS, true)]
    [InlineData(OsType.macOS, false)]
    public void Platform_Maximize_Uses_The_Window_Visible_Frame(
        OsType osType,
        bool isCsdEnabled)
    {
        RunOnUIThread(() =>
        {
            var decoration = new Thickness(16, 60, 18, 22);
            var frameShadow = new Thickness(9, 13, 17, 21);
            var fixture = ShowPresenter(
                new AtomUI.Desktop.Controls.Dialog
                {
                    IsMotionEnabled = false,
                    IsMaximizable = true,
                    HostWidth = 320,
                    HostHeight = 180
                },
                window =>
                {
                    window.SetValue(AtomUI.Desktop.Controls.Window.OsTypeProperty, osType);
                    window.IsCsdEnabled = isCsdEnabled;
                    window.FrameShadowThickness = frameShadow;
                    SetPlatformDecorationMargin(window, decoration);
                });

            try
            {
                var surface = fixture.Presenter.Surface;
                var maximizeButton = surface.Header.ShouldNotBeNull()
                                            .GetVisualDescendants()
                                            .OfType<DialogCaptionButton>()
                                            .Single(button => button.Name == "PART_MaximizeButton");
                var expectedOwnerBounds = GetDialogBodyOwnerBounds(fixture, frameShadow);

                maximizeButton.RaiseEvent(new RoutedEventArgs(AtomUI.Desktop.Controls.Button.ClickEvent));
                Dispatcher.UIThread.RunJobs();

                surface.Bounds.Size.ShouldBe(expectedOwnerBounds.Size);
                surface.Margin.ShouldBe(default);
                GetSurfacePosition(surface, fixture.Presenter).ShouldBe(expectedOwnerBounds.Position);
            }
            finally
            {
                fixture.Dispose();
            }
        });
    }

    [Theory]
    [InlineData(OsType.Windows, true)]
    [InlineData(OsType.Linux, true)]
    [InlineData(OsType.Linux, false)]
    [InlineData(OsType.macOS, false)]
    public void Platform_Header_Drag_Can_Use_The_TitleBar_Within_The_Visible_Frame(
        OsType osType,
        bool isCsdEnabled)
    {
        RunOnUIThread(() =>
        {
            var frameShadow = new Thickness(8, 12, 16, 20);
            var fixture = ShowPresenter(
                new AtomUI.Desktop.Controls.Dialog
                {
                    IsMotionEnabled = false,
                    IsDragMovable = true,
                    HostWidth = 320,
                    HostHeight = 180
                },
                window =>
                {
                    window.SetValue(AtomUI.Desktop.Controls.Window.OsTypeProperty, osType);
                    window.IsCsdEnabled = isCsdEnabled;
                    window.FrameShadowThickness = frameShadow;
                    SetPlatformDecorationMargin(window, new Thickness(14, 56, 20, 24));
                });

            try
            {
                var surface = fixture.Presenter.Surface;
                var header = surface.Header.ShouldNotBeNull();
                var ownerBounds = GetDialogBodyOwnerBounds(fixture, frameShadow);

                Drag(header, fixture.Window, new Point(240, 140), new Point(-1000, -1000));

                GetSurfacePosition(surface, fixture.Presenter).ShouldBe(ownerBounds.Position);

                Drag(header, fixture.Window, new Point(240, 140), new Point(2000, 2000));

                var bottomRightPosition = GetSurfacePosition(surface, fixture.Presenter);
                (bottomRightPosition.X + surface.Bounds.Width).ShouldBe(ownerBounds.Right);
                (bottomRightPosition.Y + surface.Bounds.Height).ShouldBe(ownerBounds.Bottom);
                ownerBounds.Contains(GetSurfaceBodyBounds(surface, fixture.Presenter)).ShouldBeTrue();
            }
            finally
            {
                fixture.Dispose();
            }
        });
    }

    [Fact]
    public void Linux_NonCsd_Mask_Covers_The_Complete_Window_Layer()
    {
        RunOnUIThread(() =>
        {
            var frameShadow = new Thickness(12, 18, 24, 30);
            const double titleBarHeight = 42;
            var fixture = ShowPresenter(
                new AtomUI.Desktop.Controls.Dialog
                {
                    IsModal = true,
                    IsMotionEnabled = false,
                    HostWidth = 320,
                    HostHeight = 180
                },
                window => ConfigureLinuxWindow(
                    window,
                    isCsdEnabled: false,
                    frameShadow,
                    titleBarHeight));

            try
            {
                var maskActor = fixture.Presenter.GetVisualDescendants()
                                       .OfType<MotionActor>()
                                       .Single(actor => actor.Name == "PART_MaskMotionActor");

                maskActor.Margin.ShouldBe(default);
                maskActor.Bounds.Size.ShouldBe(fixture.Presenter.Bounds.Size);
                maskActor.GetVisualAncestors()
                         .OfType<WindowVisualLayerClip>()
                         .ShouldHaveSingleItem();
            }
            finally
            {
                fixture.Dispose();
            }
        });
    }

    [Theory]
    [InlineData(OsType.Windows, true)]
    [InlineData(OsType.Linux, true)]
    [InlineData(OsType.macOS, false)]
    public void Available_Drawn_Decorations_Dialog_Host_Is_Used_On_Every_Platform(
        OsType osType,
        bool isCsdEnabled)
    {
        RunOnUIThread(() =>
        {
            var placementTarget = new Border { Width = 100, Height = 40 };
            var root = new ScopeAwareOverlayLayerPanel
            {
                Children = { placementTarget }
            };
            var window = new AtomUI.Desktop.Controls.Window
            {
                Width = 640,
                Height = 480,
                Content = root
            };
            var presenter = new OverlayDialogPresenter(
                new AtomUI.Desktop.Controls.Dialog
                {
                    IsModal = true,
                    IsMotionEnabled = false,
                    HostWidth = 320,
                    HostHeight = 180
                },
                placementTarget);
            Action? restoreDecorations = null;

            try
            {
                window.Show();
                Dispatcher.UIThread.RunJobs();
                window.SetValue(AtomUI.Desktop.Controls.Window.OsTypeProperty, osType);
                window.IsCsdEnabled = isCsdEnabled;
                var dialogHost = new Panel { Name = "PART_DialogOverlayLayerHost" };
                restoreDecorations = DrawnDecorationsTestHost.Install(window, dialogHost);

                WaitWithDispatcherPump(presenter.ShowAsync(CancellationToken.None).AsTask());
                var dialogLayer = presenter.Parent.ShouldBeOfType<DialogOverlayLayer>();

                dialogLayer.Parent.ShouldBeSameAs(dialogHost);

                WaitWithDispatcherPump(presenter.CloseAsync().AsTask());
                WaitWithDispatcherPump(presenter.DisposeAsync().AsTask());

                dialogHost.Children.OfType<DialogOverlayLayer>().ShouldBeEmpty();
            }
            finally
            {
                WaitWithDispatcherPump(presenter.DisposeAsync().AsTask());
                restoreDecorations?.Invoke();
                window.Close();
            }
        });
    }

    [Theory]
    [InlineData(OsType.Windows, true)]
    [InlineData(OsType.Linux, true)]
    [InlineData(OsType.Linux, false)]
    [InlineData(OsType.macOS, false)]
    public void Platform_Oversized_Host_Uses_The_Window_Visible_Frame(
        OsType osType,
        bool isCsdEnabled)
    {
        RunOnUIThread(() =>
        {
            var decoration = new Thickness(14, 58, 22, 26);
            var frameShadow = new Thickness(8, 12, 16, 20);
            var fixture = ShowPresenter(
                new AtomUI.Desktop.Controls.Dialog
                {
                    IsMotionEnabled = false,
                    HostWidth = 2000,
                    HostHeight = 2000
                },
                window =>
                {
                    window.SetValue(AtomUI.Desktop.Controls.Window.OsTypeProperty, osType);
                    window.IsCsdEnabled = isCsdEnabled;
                    window.FrameShadowThickness = frameShadow;
                    SetPlatformDecorationMargin(window, decoration);
                });

            try
            {
                var expectedOwnerBounds = GetDialogBodyOwnerBounds(fixture, frameShadow);
                var surface = fixture.Presenter.Surface;

                surface.MaxWidth.ShouldBe(expectedOwnerBounds.Width);
                surface.MaxHeight.ShouldBe(expectedOwnerBounds.Height);
                surface.Bounds.Size.ShouldBe(expectedOwnerBounds.Size);
            }
            finally
            {
                fixture.Dispose();
            }
        });
    }

    [Fact]
    public void Frame_Shadow_Changes_Keep_Full_Mask_And_Reflow_The_Window_Visible_Frame()
    {
        RunOnUIThread(() =>
        {
            var decoration = new Thickness(10, 48, 10, 18);
            var initialShadow = new Thickness(6, 8, 10, 12);
            var updatedShadow = new Thickness(14, 18, 22, 26);
            var fixture = ShowPresenter(
                new AtomUI.Desktop.Controls.Dialog
                {
                    IsModal = true,
                    IsMotionEnabled = false,
                    HostWidth = 320,
                    HostHeight = 180,
                    VerticalStartupLocation = DialogVerticalAnchor.Top,
                    HorizontalStartupLocation = DialogHorizontalAnchor.Left
                },
                window =>
                {
                    ConfigureLinuxWindow(window, isCsdEnabled: true, frameShadow: initialShadow);
                    SetPlatformDecorationMargin(window, decoration);
                });

            try
            {
                fixture.Window.FrameShadowThickness = updatedShadow;
                Dispatcher.UIThread.RunJobs();

                var maskActor = fixture.Presenter.GetVisualDescendants()
                                       .OfType<MotionActor>()
                                       .Single(actor => actor.Name == "PART_MaskMotionActor");

                maskActor.Margin.ShouldBe(default);
                maskActor.Bounds.Size.ShouldBe(fixture.Presenter.Bounds.Size);
                var surface = fixture.Presenter.Surface;
                var ownerBounds = GetDialogBodyOwnerBounds(fixture, updatedShadow);
                GetSurfacePosition(surface, fixture.Presenter).ShouldBe(ownerBounds.Position);
            }
            finally
            {
                fixture.Dispose();
            }
        });
    }

    [Fact]
    public void Header_Drag_Updates_Dialog_Offsets()
    {
        RunOnUIThread(() =>
        {
            var fixture = ShowPresenter(new AtomUI.Desktop.Controls.Dialog
            {
                IsMotionEnabled = false,
                IsDragMovable = true,
                HostWidth = 320,
                HostHeight = 180
            });

            try
            {
                var header = fixture.Presenter.Surface.Header.ShouldNotBeNull();

                Drag(header, fixture.Window, new Point(240, 140), new Point(285, 170));

                fixture.Dialog.OffsetX.ShouldBe(45);
                fixture.Dialog.OffsetY.ShouldBe(30);
            }
            finally
            {
                fixture.Dispose();
            }
        });
    }

    [Fact]
    public void Header_Drag_Does_Not_Run_Layout_For_Each_Pointer_Move()
    {
        RunOnUIThread(() =>
        {
            var fixture = ShowPresenter(new AtomUI.Desktop.Controls.Dialog
            {
                IsMotionEnabled = false,
                IsDragMovable = true,
                HostWidth = 320,
                HostHeight = 180
            });

            try
            {
                var header = fixture.Presenter.Surface.Header.ShouldNotBeNull();
                var pointer = BeginDrag(header, fixture.Window, new Point(240, 140));
                Dispatcher.UIThread.RunJobs();

                var layoutPassCount = 0;
                fixture.Window.LayoutUpdated += HandleLayoutUpdated;
                try
                {
                    for (var index = 1; index <= 60; index++)
                    {
                        MoveDrag(
                            header,
                            fixture.Window,
                            pointer,
                            new Point(240 + index, 140 + index / 2d),
                            (ulong)index);
                        Dispatcher.UIThread.RunJobs();
                    }
                }
                finally
                {
                    fixture.Window.LayoutUpdated -= HandleLayoutUpdated;
                    EndDrag(header, fixture.Window, pointer, new Point(300, 170), 61);
                    Dispatcher.UIThread.RunJobs();
                }

                layoutPassCount.ShouldBe(
                    0,
                    "pointer-move positioning should stay off the measure/arrange path.");

                void HandleLayoutUpdated(object? sender, EventArgs e)
                {
                    layoutPassCount++;
                }
            }
            finally
            {
                fixture.Dispose();
            }
        });
    }

    [Theory]
    [InlineData(1.0, 1.0)]
    [InlineData(1.25, 1.0)]
    [InlineData(1.5, 2.0)]
    [InlineData(2.0, 2.0)]
    public void Windows_Header_Drag_Uses_The_Dpi_Rounded_Frame_As_The_Top_Boundary(
        double renderScaling,
        double physicalBorderPixels)
    {
        RunOnUIThread(() =>
        {
            var effectiveFrame = new Thickness(physicalBorderPixels / renderScaling);
            var fixture = ShowPresenter(
                new AtomUI.Desktop.Controls.Dialog
                {
                    IsMotionEnabled = false,
                    IsDragMovable = true,
                    HostWidth = 600,
                    HostHeight = 220
                },
                window =>
                {
                    window.SetValue(AtomUI.Desktop.Controls.Window.OsTypeProperty, OsType.Windows);
                    window.IsCsdEnabled = true;
                    window.FrameShadowThickness = default;
                    window.SetRenderScaling(renderScaling);
                });

            try
            {
                // The platform manager refreshes the native border after a scaling
                // change. Set the deterministic test geometry after that refresh.
                fixture.Window.VisibleFrameBorderThickness = effectiveFrame;
                Dispatcher.UIThread.RunJobs();
                var surface = fixture.Presenter.Surface;
                var header = surface.Header.ShouldNotBeNull();
                var frameThickness = fixture.Window.VisibleFrameBorderThickness;
                frameThickness.ShouldBe(effectiveFrame);
                (frameThickness.Top * renderScaling).ShouldBe(physicalBorderPixels, 0.001);

                Drag(header, fixture.Window, new Point(320, 140), new Point(320, -1000));

                var surfaceTopLeft = GetSurfacePosition(surface, fixture.Presenter);
                surfaceTopLeft.Y.ShouldBe(effectiveFrame.Top, 0.001);
                (surfaceTopLeft.Y * renderScaling).ShouldBe(physicalBorderPixels, 0.001);
            }
            finally
            {
                fixture.Dispose();
            }
        });
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, true)]
    public void Header_Drag_Is_Ignored_When_Disabled_Or_Maximized(bool isDragMovable, bool maximize)
    {
        RunOnUIThread(() =>
        {
            var fixture = ShowPresenter(new AtomUI.Desktop.Controls.Dialog
            {
                IsMotionEnabled = false,
                IsDragMovable = isDragMovable,
                IsMaximizable = true,
                HostWidth = 320,
                HostHeight = 180
            });

            try
            {
                var header = fixture.Presenter.Surface.Header.ShouldNotBeNull();
                if (maximize)
                {
                    var maximizeButton = header.GetVisualDescendants()
                                               .OfType<DialogCaptionButton>()
                                               .Single(button => button.Name == "PART_MaximizeButton");
                    maximizeButton.RaiseEvent(new RoutedEventArgs(AtomUI.Desktop.Controls.Button.ClickEvent));
                    Dispatcher.UIThread.RunJobs();
                }

                Drag(header, fixture.Window, new Point(240, 140), new Point(285, 170));

                fixture.Dialog.OffsetX.ShouldBe(0);
                fixture.Dialog.OffsetY.ShouldBe(0);
            }
            finally
            {
                fixture.Dispose();
            }
        });
    }

    [Fact]
    public void Header_Drag_Clamps_The_Whole_Surface_And_Preserves_The_Grab_Point()
    {
        RunOnUIThread(() =>
        {
            var fixture = ShowPresenter(new AtomUI.Desktop.Controls.Dialog
            {
                IsMotionEnabled = false,
                IsDragMovable = true,
                HostWidth = 320,
                HostHeight = 180
            });

            try
            {
                var surface = fixture.Presenter.Surface;
                var header = fixture.Presenter.Surface.Header.ShouldNotBeNull();
                var grabOffset = new Vector(80, 20);
                var visibleFrameBounds = WindowVisualLayerClip.CalculateClipBounds(
                    fixture.Presenter.Bounds.Size,
                    fixture.Window.FrameShadowThickness);
                var ownerBounds = visibleFrameBounds.Deflate(
                    fixture.Window.VisibleFrameBorderThickness);
                var initialPosition = GetSurfacePosition(surface, fixture.Presenter);
                var start = new Point(
                    initialPosition.X + grabOffset.X,
                    initialPosition.Y + grabOffset.Y);
                var pointer = new Pointer(Pointer.GetNextFreeId(), PointerType.Mouse, true);

                header.RaiseEvent(new PointerPressedEventArgs(
                    header,
                    pointer,
                    fixture.Window,
                    start,
                    0,
                    new PointerPointProperties(
                        RawInputModifiers.LeftMouseButton,
                        PointerUpdateKind.LeftButtonPressed),
                    KeyModifiers.None));

                header.RaiseEvent(new PointerEventArgs(
                    InputElement.PointerMovedEvent,
                    header,
                    pointer,
                    fixture.Window,
                    new Point(-1000, -1000),
                    1,
                    new PointerPointProperties(
                        RawInputModifiers.LeftMouseButton,
                        PointerUpdateKind.Other),
                    KeyModifiers.None));

                GetSurfacePosition(surface, fixture.Presenter).ShouldBe(ownerBounds.Position);

                var interiorPosition = new Point(ownerBounds.Left + 20, ownerBounds.Top + 40);

                header.RaiseEvent(new PointerEventArgs(
                    InputElement.PointerMovedEvent,
                    header,
                    pointer,
                    fixture.Window,
                    interiorPosition + grabOffset,
                    2,
                    new PointerPointProperties(
                        RawInputModifiers.LeftMouseButton,
                        PointerUpdateKind.Other),
                    KeyModifiers.None));

                GetSurfacePosition(surface, fixture.Presenter).ShouldBe(interiorPosition);

                header.RaiseEvent(new PointerEventArgs(
                    InputElement.PointerMovedEvent,
                    header,
                    pointer,
                    fixture.Window,
                    new Point(2000, 2000),
                    3,
                    new PointerPointProperties(
                        RawInputModifiers.LeftMouseButton,
                        PointerUpdateKind.Other),
                    KeyModifiers.None));

                var bottomRightPosition = GetSurfacePosition(surface, fixture.Presenter);
                bottomRightPosition.X.ShouldBe(ownerBounds.Right - surface.Bounds.Width);
                bottomRightPosition.Y.ShouldBe(ownerBounds.Bottom - surface.Bounds.Height);

                header.RaiseEvent(new PointerReleasedEventArgs(
                    header,
                    pointer,
                    fixture.Window,
                    new Point(2000, 2000),
                    4,
                    new PointerPointProperties(
                        RawInputModifiers.None,
                        PointerUpdateKind.LeftButtonReleased),
                    KeyModifiers.None,
                    MouseButton.Left));

                ownerBounds.Contains(GetSurfaceBodyBounds(surface, fixture.Presenter)).ShouldBeTrue();
            }
            finally
            {
                fixture.Dispose();
            }
        });
    }

    [Fact]
    public void Mask_Click_Only_Requests_Close_For_The_Topmost_Presenter()
    {
        RunOnUIThread(() =>
        {
            var placementTarget = new Border { Width = 100, Height = 40 };
            var root = new ScopeAwareOverlayLayerPanel
            {
                Children = { placementTarget }
            };
            var window = new AtomUI.Desktop.Controls.Window
            {
                Width = 640,
                Height = 480,
                Content = root
            };
            var first = new OverlayDialogPresenter(
                new AtomUI.Desktop.Controls.Dialog { IsModal = true, IsMotionEnabled = false },
                placementTarget);
            var second = new OverlayDialogPresenter(
                new AtomUI.Desktop.Controls.Dialog { IsModal = true, IsMotionEnabled = false },
                placementTarget);
            var firstCloseCount = 0;
            var secondCloseCount = 0;
            first.CloseRequested += (_, _) => firstCloseCount++;
            second.CloseRequested += (_, _) => secondCloseCount++;

            try
            {
                window.Show();
                Dispatcher.UIThread.RunJobs();
                WaitWithDispatcherPump(first.ShowAsync(CancellationToken.None).AsTask());
                WaitWithDispatcherPump(second.ShowAsync(CancellationToken.None).AsTask());

                RaisePointerPressed(first.GetVisualDescendants().OfType<OverlayDialogMask>().Single());
                RaisePointerPressed(second.GetVisualDescendants().OfType<OverlayDialogMask>().Single());

                firstCloseCount.ShouldBe(0);
                secondCloseCount.ShouldBe(1);
            }
            finally
            {
                WaitWithDispatcherPump(first.CloseAsync().AsTask());
                WaitWithDispatcherPump(second.CloseAsync().AsTask());
                WaitWithDispatcherPump(first.DisposeAsync().AsTask());
                WaitWithDispatcherPump(second.DisposeAsync().AsTask());
                window.Close();
            }
        });
    }

    [Fact]
    public void Escape_From_A_Lower_Presenter_Is_Handled_By_The_Topmost_Presenter()
    {
        RunOnUIThread(() =>
        {
            var placementTarget = new Border { Width = 100, Height = 40 };
            var root = new ScopeAwareOverlayLayerPanel
            {
                Children = { placementTarget }
            };
            var window = new AtomUI.Desktop.Controls.Window
            {
                Width = 640,
                Height = 480,
                Content = root
            };
            var first = new OverlayDialogPresenter(
                new AtomUI.Desktop.Controls.Dialog
                {
                    IsMotionEnabled = false,
                    StandardButtons = DialogStandardButton.Cancel
                },
                placementTarget);
            var second = new OverlayDialogPresenter(
                new AtomUI.Desktop.Controls.Dialog
                {
                    IsMotionEnabled = false,
                    StandardButtons = DialogStandardButton.Cancel
                },
                placementTarget);
            var firstCloseCount = 0;
            var secondCloseCount = 0;
            first.CloseRequested += (_, _) => firstCloseCount++;
            second.CloseRequested += (_, _) => secondCloseCount++;

            try
            {
                window.Show();
                Dispatcher.UIThread.RunJobs();
                WaitWithDispatcherPump(first.ShowAsync(CancellationToken.None).AsTask());
                WaitWithDispatcherPump(second.ShowAsync(CancellationToken.None).AsTask());
                Dispatcher.UIThread.RunJobs();

                var args = new KeyEventArgs
                {
                    RoutedEvent = InputElement.KeyDownEvent,
                    Source = first.Surface,
                    Key = Key.Escape
                };
                first.Surface.RaiseEvent(args);

                args.Handled.ShouldBeTrue();
                firstCloseCount.ShouldBe(0);
                secondCloseCount.ShouldBe(1);
            }
            finally
            {
                WaitWithDispatcherPump(first.CloseAsync().AsTask());
                WaitWithDispatcherPump(second.CloseAsync().AsTask());
                WaitWithDispatcherPump(first.DisposeAsync().AsTask());
                WaitWithDispatcherPump(second.DisposeAsync().AsTask());
                window.Close();
            }
        });
    }

    private static PresenterFixture ShowPresenter(
        AtomUI.Desktop.Controls.Dialog dialog,
        Action<AtomUI.Desktop.Controls.Window>? configureWindow = null)
    {
        var placementTarget = new Border { Width = 100, Height = 40 };
        var root = new ScopeAwareOverlayLayerPanel
        {
            Children = { placementTarget }
        };
        var window = new AtomUI.Desktop.Controls.Window
        {
            Width = 640,
            Height = 480,
            Content = root
        };
        var presenter = new OverlayDialogPresenter(dialog, placementTarget);

        window.Show();
        Dispatcher.UIThread.RunJobs();
        configureWindow?.Invoke(window);
        Dispatcher.UIThread.RunJobs();
        WaitWithDispatcherPump(presenter.ShowAsync(CancellationToken.None).AsTask());
        Dispatcher.UIThread.RunJobs();

        return new PresenterFixture(window, dialog, presenter);
    }

    private static void ConfigureLinuxWindow(
        AtomUI.Desktop.Controls.Window window,
        bool isCsdEnabled,
        Thickness frameShadow,
        double titleBarHeight = 40)
    {
        window.SetValue(AtomUI.Desktop.Controls.Window.OsTypeProperty, OsType.Linux);
        window.IsCsdEnabled = isCsdEnabled;
        window.FrameShadowThickness = frameShadow;
        window.TitleBarHeight = titleBarHeight;
        window.IsTitleBarVisible = true;
    }

    private static void SetPlatformDecorationMargin(
        AtomUI.Desktop.Controls.Window window,
        Thickness margin)
    {
        var setter = typeof(Avalonia.Controls.Window)
                     .GetProperty(
                         nameof(Avalonia.Controls.Window.WindowDecorationMargin),
                         BindingFlags.Instance | BindingFlags.Public)
                     .ShouldNotBeNull()
                     .GetSetMethod(nonPublic: true)
                     .ShouldNotBeNull();
        setter.Invoke(window, new object?[] { margin });
    }

    private static Point GetSurfacePosition(DialogSurface surface, Visual relativeTo)
    {
        return surface.TranslatePoint(default, relativeTo).ShouldNotBeNull();
    }

    private static Rect GetSurfaceBodyBounds(DialogSurface surface, Visual relativeTo)
    {
        var position = GetSurfacePosition(surface, relativeTo);
        return new Rect(position, surface.Bounds.Size);
    }

    private static Rect GetDialogBodyOwnerBounds(PresenterFixture fixture, Thickness frameShadow)
    {
        var visibleFrameBounds = WindowVisualLayerClip.CalculateClipBounds(
            fixture.Presenter.Bounds.Size,
            frameShadow);
        return visibleFrameBounds.Deflate(fixture.Window.VisibleFrameBorderThickness);
    }

    private static void Drag(Control source, Visual root, Point start, Point end)
    {
        var pointer = BeginDrag(source, root, start);
        MoveDrag(source, root, pointer, end, 1);
        EndDrag(source, root, pointer, end, 2);
        Dispatcher.UIThread.RunJobs();
    }

    private static void ResizeSurface(
        PresenterFixture fixture,
        ResizeHandleLocation location,
        params Vector[] cumulativeDeltas)
    {
        var handle = fixture.Presenter.Surface.Resizer.ShouldNotBeNull()
                            .GetVisualDescendants()
                            .OfType<Border>()
                            .Single(border => Equals(border.Tag, location));
        var start = handle.TranslatePoint(
            new Point(handle.Bounds.Width / 2, handle.Bounds.Height / 2),
            fixture.Window).ShouldNotBeNull();
        var pointer = BeginDrag(handle, fixture.Window, start);
        for (var index = 0; index < cumulativeDeltas.Length; index++)
        {
            MoveDrag(
                handle,
                fixture.Window,
                pointer,
                start + cumulativeDeltas[index],
                (ulong)(index + 1));
            Dispatcher.UIThread.RunJobs();
        }

        var finalPosition = cumulativeDeltas.Length == 0
            ? start
            : start + cumulativeDeltas[^1];
        EndDrag(handle, fixture.Window, pointer, finalPosition, (ulong)(cumulativeDeltas.Length + 1));
        Dispatcher.UIThread.RunJobs();
    }

    private static Pointer BeginDrag(Control source, Visual root, Point start)
    {
        var pointer = new Pointer(Pointer.GetNextFreeId(), PointerType.Mouse, true);
        source.RaiseEvent(new PointerPressedEventArgs(
            source,
            pointer,
            root,
            start,
            0,
            new PointerPointProperties(RawInputModifiers.LeftMouseButton, PointerUpdateKind.LeftButtonPressed),
            KeyModifiers.None));
        return pointer;
    }

    private static void MoveDrag(Control source, Visual root, Pointer pointer, Point position, ulong timestamp)
    {
        source.RaiseEvent(new PointerEventArgs(
            InputElement.PointerMovedEvent,
            source,
            pointer,
            root,
            position,
            timestamp,
            new PointerPointProperties(RawInputModifiers.LeftMouseButton, PointerUpdateKind.Other),
            KeyModifiers.None));
    }

    private static void EndDrag(Control source, Visual root, Pointer pointer, Point position, ulong timestamp)
    {
        source.RaiseEvent(new PointerReleasedEventArgs(
            source,
            pointer,
            root,
            position,
            timestamp,
            new PointerPointProperties(RawInputModifiers.None, PointerUpdateKind.LeftButtonReleased),
            KeyModifiers.None,
            MouseButton.Left));
    }

    private static void RaisePointerPressed(Control source)
    {
        source.RaiseEvent(new PointerPressedEventArgs(
            source,
            new Pointer(Pointer.GetNextFreeId(), PointerType.Mouse, true),
            source,
            default,
            0,
            new PointerPointProperties(RawInputModifiers.LeftMouseButton, PointerUpdateKind.LeftButtonPressed),
            KeyModifiers.None));
    }

    private sealed record PresenterFixture(
        AtomUI.Desktop.Controls.Window Window,
        AtomUI.Desktop.Controls.Dialog Dialog,
        OverlayDialogPresenter Presenter) : IDisposable
    {
        public void Dispose()
        {
            WaitWithDispatcherPump(Presenter.CloseAsync().AsTask());
            WaitWithDispatcherPump(Presenter.DisposeAsync().AsTask());
            Window.Close();
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

    private static void RunOnUIThread(Action action)
    {
        Dispatcher.UIThread.Invoke(action);
    }
}
