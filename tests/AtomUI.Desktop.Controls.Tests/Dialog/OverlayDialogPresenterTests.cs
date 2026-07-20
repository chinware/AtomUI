using AtomUI.Controls.Primitives;
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
                double.IsNaN(fixture.Presenter.Surface.Width).ShouldBeTrue();
                double.IsNaN(fixture.Presenter.Surface.Height).ShouldBeTrue();
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
    [InlineData(OsType.Linux, true)]
    [InlineData(OsType.macOS, false)]
    public void Platform_Dialog_Owner_Bounds_Follow_The_TitleBar_Interaction_Contract(
        OsType osType,
        bool isCsdEnabled)
    {
        RunOnUIThread(() =>
        {
            var decoration = new Thickness(14, 56, 20, 24);
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
                    SetPlatformDecorationMargin(window, decoration);
                });

            try
            {
                var expectedOwnerBounds = osType == OsType.Windows
                    ? new Rect(default, fixture.Presenter.Bounds.Size)
                    : new Rect(
                        decoration.Left,
                        decoration.Top,
                        fixture.Presenter.Bounds.Width - decoration.Left - decoration.Right,
                        fixture.Presenter.Bounds.Height - decoration.Top - decoration.Bottom);
                var surface = fixture.Presenter.Surface;

                surface.MaxWidth.ShouldBe(expectedOwnerBounds.Width);
                surface.MaxHeight.ShouldBe(expectedOwnerBounds.Height);
                surface.Bounds.Size.ShouldBe(expectedOwnerBounds.Size);
                surface.Margin.Left.ShouldBe(expectedOwnerBounds.Left);
                surface.Margin.Top.ShouldBe(expectedOwnerBounds.Top);
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
    public void Platform_Maximize_Uses_The_Effective_Owner_Bounds(
        OsType osType,
        bool isCsdEnabled)
    {
        RunOnUIThread(() =>
        {
            var decoration = new Thickness(16, 60, 18, 22);
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
                    SetPlatformDecorationMargin(window, decoration);
                });

            try
            {
                var surface = fixture.Presenter.Surface;
                var maximizeButton = surface.Header.ShouldNotBeNull()
                                            .GetVisualDescendants()
                                            .OfType<DialogCaptionButton>()
                                            .Single(button => button.Name == "PART_MaximizeButton");
                var expectedOwnerBounds = osType == OsType.Windows
                    ? new Rect(default, fixture.Presenter.Bounds.Size)
                    : new Rect(
                        decoration.Left,
                        decoration.Top,
                        fixture.Presenter.Bounds.Width - decoration.Left - decoration.Right,
                        fixture.Presenter.Bounds.Height - decoration.Top - decoration.Bottom);

                maximizeButton.RaiseEvent(new RoutedEventArgs(AtomUI.Desktop.Controls.Button.ClickEvent));
                Dispatcher.UIThread.RunJobs();

                surface.Bounds.Size.ShouldBe(expectedOwnerBounds.Size);
                surface.Margin.ShouldBe(
                    new Thickness(expectedOwnerBounds.Left, expectedOwnerBounds.Top, 0, 0));
            }
            finally
            {
                fixture.Dispose();
            }
        });
    }

    [Fact]
    public void Windows_Header_Drag_Can_Move_Into_The_Drawn_TitleBar_Range()
    {
        RunOnUIThread(() =>
        {
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
                    window.SetValue(AtomUI.Desktop.Controls.Window.OsTypeProperty, OsType.Windows);
                    window.IsCsdEnabled = true;
                    SetPlatformDecorationMargin(window, new Thickness(14, 56, 20, 24));
                });

            try
            {
                var surface = fixture.Presenter.Surface;
                var header = surface.Header.ShouldNotBeNull();

                Drag(header, fixture.Window, new Point(240, 140), new Point(-1000, -1000));

                surface.Margin.Left.ShouldBe(0);
                surface.Margin.Top.ShouldBe(0);

                Drag(header, fixture.Window, new Point(240, 140), new Point(2000, 2000));

                (surface.Margin.Left + surface.Bounds.Width).ShouldBe(fixture.Presenter.Bounds.Right);
                (surface.Margin.Top + surface.Bounds.Height).ShouldBe(fixture.Presenter.Bounds.Bottom);
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
                var installedHost = InstallDrawnDialogOverlayHost(window);
                restoreDecorations = installedHost.Restore;

                WaitWithDispatcherPump(presenter.ShowAsync(CancellationToken.None).AsTask());
                var dialogLayer = presenter.Parent.ShouldBeOfType<DialogOverlayLayer>();

                dialogLayer.Parent.ShouldBeSameAs(installedHost.Host);

                WaitWithDispatcherPump(presenter.CloseAsync().AsTask());
                WaitWithDispatcherPump(presenter.DisposeAsync().AsTask());

                installedHost.Host.Children.OfType<DialogOverlayLayer>().ShouldBeEmpty();
            }
            finally
            {
                WaitWithDispatcherPump(presenter.DisposeAsync().AsTask());
                restoreDecorations?.Invoke();
                window.Close();
            }
        });
    }

    [Fact]
    public void Linux_Header_Drag_Constrains_The_Dialog_Body_Without_Shadow_Inset()
    {
        RunOnUIThread(() =>
        {
            var decoration = new Thickness(14, 58, 22, 26);
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
                    ConfigureLinuxWindow(window, isCsdEnabled: true, frameShadow: new Thickness(12));
                    SetPlatformDecorationMargin(window, decoration);
                });

            try
            {
                var surface = fixture.Presenter.Surface;
                var header = surface.Header.ShouldNotBeNull();
                var ownerBounds = new Rect(
                    decoration.Left,
                    decoration.Top,
                    fixture.Presenter.Bounds.Width - decoration.Left - decoration.Right,
                    fixture.Presenter.Bounds.Height - decoration.Top - decoration.Bottom);

                Drag(header, fixture.Window, new Point(240, 140), new Point(-1000, -1000));

                surface.Margin.Left.ShouldBe(ownerBounds.Left);
                surface.Margin.Top.ShouldBe(ownerBounds.Top);

                Drag(header, fixture.Window, new Point(240, 140), new Point(2000, 2000));

                (surface.Margin.Left + surface.Bounds.Width).ShouldBe(ownerBounds.Right);
                (surface.Margin.Top + surface.Bounds.Height).ShouldBe(ownerBounds.Bottom);
            }
            finally
            {
                fixture.Dispose();
            }
        });
    }

    [Fact]
    public void Linux_Oversized_Host_Uses_The_Full_Dialog_Body_Bounds()
    {
        RunOnUIThread(() =>
        {
            var decoration = new Thickness(14, 58, 22, 26);
            var fixture = ShowPresenter(
                new AtomUI.Desktop.Controls.Dialog
                {
                    IsMotionEnabled = false,
                    HostWidth = 2000,
                    HostHeight = 2000
                },
                window =>
                {
                    ConfigureLinuxWindow(window, isCsdEnabled: true, frameShadow: new Thickness(12));
                    SetPlatformDecorationMargin(window, decoration);
                });

            try
            {
                var expectedBodySize = new Size(
                    fixture.Presenter.Bounds.Width - decoration.Left - decoration.Right,
                    fixture.Presenter.Bounds.Height - decoration.Top - decoration.Bottom);
                var surface = fixture.Presenter.Surface;

                surface.MaxWidth.ShouldBe(expectedBodySize.Width);
                surface.MaxHeight.ShouldBe(expectedBodySize.Height);
                surface.Bounds.Size.ShouldBe(expectedBodySize);
            }
            finally
            {
                fixture.Dispose();
            }
        });
    }

    [Fact]
    public void Linux_Csd_Decoration_Changes_Keep_Full_Mask_And_Reflow_Dialog_Body()
    {
        RunOnUIThread(() =>
        {
            var initial = new Thickness(10, 48, 10, 18);
            var updated = new Thickness(22, 72, 26, 30);
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
                    ConfigureLinuxWindow(window, isCsdEnabled: true, frameShadow: new Thickness(10));
                    SetPlatformDecorationMargin(window, initial);
                });

            try
            {
                SetPlatformDecorationMargin(fixture.Window, updated);
                Dispatcher.UIThread.RunJobs();

                var maskActor = fixture.Presenter.GetVisualDescendants()
                                       .OfType<MotionActor>()
                                       .Single(actor => actor.Name == "PART_MaskMotionActor");

                maskActor.Margin.ShouldBe(default);
                maskActor.Bounds.Size.ShouldBe(fixture.Presenter.Bounds.Size);
                fixture.Presenter.Surface.Margin.Left.ShouldBe(updated.Left);
                fixture.Presenter.Surface.Margin.Top.ShouldBe(updated.Top);
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
                var start = new Point(
                    surface.Margin.Left + grabOffset.X,
                    surface.Margin.Top + grabOffset.Y);
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

                surface.Margin.Left.ShouldBe(0);
                surface.Margin.Top.ShouldBe(0);

                header.RaiseEvent(new PointerEventArgs(
                    InputElement.PointerMovedEvent,
                    header,
                    pointer,
                    fixture.Window,
                    new Point(100, 60),
                    2,
                    new PointerPointProperties(
                        RawInputModifiers.LeftMouseButton,
                        PointerUpdateKind.Other),
                    KeyModifiers.None));

                surface.Margin.Left.ShouldBe(20);
                surface.Margin.Top.ShouldBe(40);

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

                surface.Margin.Left.ShouldBe(fixture.Presenter.Bounds.Width - surface.Bounds.Width);
                surface.Margin.Top.ShouldBe(fixture.Presenter.Bounds.Height - surface.Bounds.Height);

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

                surface.Margin.Left.ShouldBeGreaterThanOrEqualTo(0);
                surface.Margin.Top.ShouldBeGreaterThanOrEqualTo(0);
                (surface.Margin.Left + surface.Bounds.Width)
                    .ShouldBeLessThanOrEqualTo(fixture.Presenter.Bounds.Width);
                (surface.Margin.Top + surface.Bounds.Height)
                    .ShouldBeLessThanOrEqualTo(fixture.Presenter.Bounds.Height);
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

    private static (Panel Host, Action Restore) InstallDrawnDialogOverlayHost(
        AtomUI.Desktop.Controls.Window window)
    {
        var topLevelHost = typeof(TopLevel)
                           .GetField("_topLevelHost", BindingFlags.Instance | BindingFlags.NonPublic)
                           .ShouldNotBeNull()
                           .GetValue(window)
                           .ShouldNotBeNull();
        var decorationsField = topLevelHost.GetType()
                                               .GetField("_decorations", BindingFlags.Instance | BindingFlags.NonPublic)
                                               .ShouldNotBeNull();
        var originalDecorations = decorationsField.GetValue(topLevelHost);
        var dialogHost = new Panel { Name = "PART_DialogOverlayLayerHost" };
        var overlay = new Panel
        {
            Children = { dialogHost }
        };
        var content = new Avalonia.Controls.Chrome.WindowDrawnDecorationsContent
        {
            Overlay = overlay
        };
        var decorations = new Avalonia.Controls.Chrome.WindowDrawnDecorations();
        typeof(Avalonia.Controls.Chrome.WindowDrawnDecorations)
            .GetProperty(
                nameof(Avalonia.Controls.Chrome.WindowDrawnDecorations.Content),
                BindingFlags.Instance | BindingFlags.Public)
            .ShouldNotBeNull()
            .GetSetMethod(nonPublic: true)
            .ShouldNotBeNull()
            .Invoke(decorations, new object?[] { content });
        decorationsField.SetValue(topLevelHost, decorations);

        return (dialogHost, () => decorationsField.SetValue(topLevelHost, originalDecorations));
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

    private static void Drag(Control source, Visual root, Point start, Point end)
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
        source.RaiseEvent(new PointerEventArgs(
            InputElement.PointerMovedEvent,
            source,
            pointer,
            root,
            end,
            1,
            new PointerPointProperties(RawInputModifiers.LeftMouseButton, PointerUpdateKind.Other),
            KeyModifiers.None));
        source.RaiseEvent(new PointerReleasedEventArgs(
            source,
            pointer,
            root,
            end,
            2,
            new PointerPointProperties(RawInputModifiers.None, PointerUpdateKind.LeftButtonReleased),
            KeyModifiers.None,
            MouseButton.Left));
        Dispatcher.UIThread.RunJobs();
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
