using System.Reflection;
using AtomUI.Controls.Primitives;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media.Transformation;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Dialog;

public class DialogMotionAnchorTests
{
    static DialogMotionAnchorTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Dialog_Without_PlacementTarget_Uses_Opacity_Only_Opening_Motion()
    {
        var trigger = new AtomUI.Desktop.Controls.Button
        {
            Width   = 80,
            Height  = 32,
            Content = "Open"
        };
        var window = CreateWindow(trigger, out var overlayPanel);
        var dialog = new AtomUI.Desktop.Controls.Dialog
        {
            Content         = new AtomUI.Desktop.Controls.TextBlock { Text = "Dialog" },
            IsModal         = false,
            IsMotionEnabled = true,
            HostWidth       = 160,
            HostHeight      = 100
        };

        overlayPanel.Children.Add(dialog);

        try
        {
            var openingState = CaptureOpeningMotionState(dialog, window);

            openingState.RenderTransform.IsIdentity.ShouldBeTrue(
                "a dialog without an explicit PlacementTarget should not scale or translate from the fallback host.");
            openingState.HostTransitions
                        .OfType<TransformOperationsTransition>()
                        .ShouldBeEmpty(
                            "opacity-only opening should not create a no-op transform transition.");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Static_Dialog_Without_PlacementTarget_Uses_Opacity_Only_Opening_Motion()
    {
        var trigger = new AtomUI.Desktop.Controls.Button
        {
            Width   = 80,
            Height  = 32,
            Content = "Open"
        };
        var window = CreateWindow(trigger, out var overlayPanel);
        var dialog = CreateStaticDialog(overlayPanel, options: null);
        dialog.IsModal         = false;
        dialog.IsMotionEnabled = true;

        overlayPanel.Children.Add(dialog);

        try
        {
            var openingState = CaptureOpeningMotionState(dialog, window);

            openingState.RenderTransform.IsIdentity.ShouldBeTrue(
                "the static API fallback placement target is only a host resolver and should not become the motion anchor.");
            openingState.HostTransitions
                        .OfType<TransformOperationsTransition>()
                        .ShouldBeEmpty(
                            "static fallback placement should not create a no-op transform transition.");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Static_Dialog_With_Explicit_PlacementTarget_Keeps_Anchor_Opening_Motion()
    {
        var anchor = new Border
        {
            Width  = 40,
            Height = 40
        };
        var window = CreateWindow(anchor, out var overlayPanel);
        var dialog = CreateStaticDialog(
            overlayPanel,
            new DialogOptions
            {
                PlacementTarget = anchor
            });
        dialog.IsModal         = false;
        dialog.IsMotionEnabled = true;

        overlayPanel.Children.Add(dialog);

        try
        {
            var openingState = CaptureOpeningMotionState(dialog, window);

            openingState.RenderTransform.IsIdentity.ShouldBeFalse(
                "an explicit PlacementTarget should keep the existing scale/translate anchor motion.");
            openingState.HostTransitions
                        .OfType<TransformOperationsTransition>()
                        .Single()
                        .Property.ShouldBe(Visual.RenderTransformProperty);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Modal_Dialog_Without_PlacementTarget_Uses_Gentle_Close_Fade()
    {
        var trigger = new AtomUI.Desktop.Controls.Button
        {
            Width   = 80,
            Height  = 32,
            Content = "Open"
        };
        var window = CreateWindow(trigger, out var overlayPanel);
        var dialog = new AtomUI.Desktop.Controls.Dialog
        {
            Content         = new AtomUI.Desktop.Controls.TextBlock { Text = "Dialog" },
            IsModal         = true,
            IsMotionEnabled = true,
            HostWidth       = 160,
            HostHeight      = 100
        };

        overlayPanel.Children.Add(dialog);

        try
        {
            var closingState = CaptureModalClosingState(dialog, window);

            closingState.HostTransform.IsIdentity.ShouldBeTrue(
                "a modal dialog without an explicit PlacementTarget should fade out as one layer instead of collapsing toward a fallback host.");
            closingState.HostTransformTransitionCount.ShouldBe(0,
                "opacity-only close should not create a no-op transform transition.");
            closingState.HostOpacityTransition.Easing.ShouldBeOfType<CubicEaseIn>(
                "close opacity should start gently so the dialog frame does not disappear before the content.");
            closingState.MaskOpacityTransition.Easing.ShouldBeOfType<CubicEaseIn>(
                "the modal mask should use the same close curve as the host to avoid an early background drop.");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Dialog_Close_Keeps_Footer_Buttons_Until_Overlay_Fade_Finishes()
    {
        var trigger = new AtomUI.Desktop.Controls.Button
        {
            Width   = 80,
            Height  = 32,
            Content = "Open"
        };
        var window = CreateWindow(trigger, out var overlayPanel);
        var dialog = new AtomUI.Desktop.Controls.Dialog
        {
            Content         = new AtomUI.Desktop.Controls.TextBlock { Text = "Dialog" },
            IsModal         = false,
            IsMotionEnabled = true,
            StandardButtons = DialogStandardButton.Ok,
            HostWidth       = 160,
            HostHeight      = 100
        };

        overlayPanel.Children.Add(dialog);

        try
        {
            var openTask = dialog.OpenAsync(TestContext.Current.CancellationToken);
            openTask.IsCompleted.ShouldBeTrue("non-modal Dialog.OpenAsync should finish after scheduling the opening motion.");
            Dispatcher.UIThread.RunJobs();

            var overlayHost = window.GetVisualDescendants()
                                    .OfType<OverlayPopupHost>()
                                    .Single();
            var buttonBox = overlayHost.GetVisualDescendants()
                                       .OfType<DialogButtonBox>()
                                       .Single();
            var buttonCount  = CountDialogButtons(buttonBox);
            var hostHeight   = overlayHost.Bounds.Height;
            var footerHeight = buttonBox.Bounds.Height;

            buttonCount.ShouldBe(1);
            hostHeight.ShouldBeGreaterThan(0);
            footerHeight.ShouldBeGreaterThan(0);

            SetOverlayDialogHostAnimationDuration(window, TimeSpan.Zero);
            dialog.Done();

            CountDialogButtons(buttonBox).ShouldBe(
                buttonCount,
                "closing should keep footer buttons in the visual tree until the popup is actually closed.");
            overlayHost.Bounds.Height.ShouldBe(
                hostHeight,
                "closing should not remeasure the dialog to a shorter height before the fade completes.");
            buttonBox.Bounds.Height.ShouldBe(
                footerHeight,
                "closing should not collapse the footer before the fade completes.");

            Dispatcher.UIThread.RunJobs();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Modal_Dialog_With_Explicit_Anchor_Closes_When_Page_Is_Detached()
    {
        var anchor = new Border
        {
            Width  = 40,
            Height = 40
        };
        var page = new Panel();
        page.Children.Add(anchor);

        var window = CreateWindow(page, out var overlayPanel);
        var dialog = new AtomUI.Desktop.Controls.Dialog
        {
            Content         = new AtomUI.Desktop.Controls.TextBlock { Text = "Dialog" },
            PlacementTarget = anchor,
            IsModal         = true,
            IsMotionEnabled = true,
            HostWidth       = 160,
            HostHeight      = 100
        };
        page.Children.Add(dialog);

        try
        {
            var openTask = dialog.OpenAsync(TestContext.Current.CancellationToken);
            openTask.IsCompleted.ShouldBeFalse();
            Dispatcher.UIThread.RunJobs();

            overlayPanel.Children.Remove(page);
            Dispatcher.UIThread.RunJobs();

            dialog.IsOpen.ShouldBeFalse();
        }
        finally
        {
            window.Close();
        }
    }

    private static AtomUI.Desktop.Controls.Dialog CreateStaticDialog(
        Control placementTarget,
        DialogOptions? options)
    {
        var createDialog = typeof(AtomUI.Desktop.Controls.Dialog).GetMethod(
            "CreateDialog",
            BindingFlags.Static | BindingFlags.NonPublic);

        createDialog.ShouldNotBeNull();
        return (AtomUI.Desktop.Controls.Dialog)createDialog.Invoke(
            null,
            [new AtomUI.Desktop.Controls.TextBlock { Text = "Dialog" }, null, options, placementTarget])!;
    }

    private static (TransformOperations RenderTransform, Transitions HostTransitions) CaptureOpeningMotionState(
        AtomUI.Desktop.Controls.Dialog dialog,
        AvaloniaWindow window)
    {
        TransformOperations? openingTransform = null;
        Transitions? hostTransitions = null;

        var openTask = dialog.OpenAsync();
        openTask.IsCompleted.ShouldBeTrue("non-modal Dialog.OpenAsync should finish after scheduling the opening motion.");

        Dispatcher.UIThread.Post(() =>
        {
            var overlayHost = window.GetVisualDescendants()
                                    .OfType<OverlayPopupHost>()
                                    .Single();

            openingTransform = overlayHost.RenderTransform.ShouldBeOfType<TransformOperations>();
            hostTransitions  = GetTransitions(overlayHost);

            dialog.IsMotionEnabled = false;
            dialog.Done();
        }, DispatcherPriority.Loaded);

        Dispatcher.UIThread.RunJobs();
        openTask.GetAwaiter().GetResult();

        openingTransform.ShouldNotBeNull();
        hostTransitions.ShouldNotBeNull();
        return (openingTransform, hostTransitions);
    }

    private static (
        TransformOperations HostTransform,
        int HostTransformTransitionCount,
        DoubleTransition HostOpacityTransition,
        DoubleTransition MaskOpacityTransition) CaptureModalClosingState(
            AtomUI.Desktop.Controls.Dialog dialog,
            AvaloniaWindow window)
    {
        var openTask = dialog.OpenAsync();
        openTask.IsCompleted.ShouldBeFalse("modal Dialog.OpenAsync should wait for the close animation.");

        Dispatcher.UIThread.RunJobs();

        var overlayHost = window.GetVisualDescendants()
                                .OfType<OverlayPopupHost>()
                                .Single();
        var mask = window.GetVisualDescendants()
                         .Single(x => x.GetType().Name == "OverlayDialogMask");

        SetOverlayDialogHostAnimationDuration(window, TimeSpan.Zero);
        dialog.Done();

        var hostTransform          = overlayHost.RenderTransform.ShouldBeOfType<TransformOperations>();
        var hostOpacityTransition  = GetSingleOpacityTransition(overlayHost);
        var hostTransformCount     = GetTransitions(overlayHost).OfType<TransformOperationsTransition>().Count();
        var maskOpacityTransition  = GetSingleOpacityTransition(mask);

        Dispatcher.UIThread.RunJobs();
        openTask.GetAwaiter().GetResult();

        return (hostTransform, hostTransformCount, hostOpacityTransition, maskOpacityTransition);
    }

    private static int CountDialogButtons(Visual visual)
    {
        return visual.GetVisualDescendants()
                     .OfType<DialogButton>()
                     .Count();
    }

    private static DoubleTransition GetSingleOpacityTransition(object control)
    {
        return GetTransitions(control).OfType<DoubleTransition>().Single();
    }

    private static Transitions GetTransitions(object control)
    {
        var transitionsProperty = control.GetType().GetProperty(
            "Transitions",
            BindingFlags.Instance | BindingFlags.Public);

        transitionsProperty.ShouldNotBeNull();
        var transitions = (Transitions?)transitionsProperty.GetValue(control);
        transitions.ShouldNotBeNull();
        return transitions;
    }

    private static void SetOverlayDialogHostAnimationDuration(Visual searchRoot, TimeSpan duration)
    {
        var overlayDialogHost = searchRoot.GetVisualDescendants()
                                          .Single(x => x.GetType().Name == "OverlayDialogHost");
        var property = overlayDialogHost.GetType().GetProperty(
            "AnimationDuration",
            BindingFlags.Instance | BindingFlags.NonPublic);

        property.ShouldNotBeNull();
        property.SetValue(overlayDialogHost, duration);
    }

    private static AvaloniaWindow CreateWindow(Control content, out ScopeAwareOverlayLayerPanel overlayPanel)
    {
        overlayPanel = new ScopeAwareOverlayLayerPanel
        {
            Width  = 320,
            Height = 240
        };
        overlayPanel.Children.Add(content);

        var visualLayerManager = new VisualLayerManager
        {
            Child = overlayPanel
        };
        EnablePopupOverlayLayer(visualLayerManager);

        var window = new AvaloniaWindow
        {
            Width   = 320,
            Height  = 240,
            Content = visualLayerManager
        };

        window.Show();
        Dispatcher.UIThread.RunJobs();
        return window;
    }

    private static void EnablePopupOverlayLayer(VisualLayerManager visualLayerManager)
    {
        var property = typeof(VisualLayerManager).GetProperty(
            "EnablePopupOverlayLayer",
            BindingFlags.Instance | BindingFlags.NonPublic);

        property.ShouldNotBeNull();
        property.SetValue(visualLayerManager, true);
    }
}
