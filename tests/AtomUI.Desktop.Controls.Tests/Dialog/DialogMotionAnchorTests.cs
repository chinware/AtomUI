using System.Reflection;
using AtomUI.Controls.Primitives;
using AtomUI.Desktop.Controls;
using Avalonia;
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
            var openingTransform = CaptureOpeningRenderTransform(dialog, window);

            openingTransform.IsIdentity.ShouldBeTrue(
                "a dialog without an explicit PlacementTarget should not scale or translate from the fallback host.");
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
            var openingTransform = CaptureOpeningRenderTransform(dialog, window);

            openingTransform.IsIdentity.ShouldBeTrue(
                "the static API fallback placement target is only a host resolver and should not become the motion anchor.");
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
            var openingTransform = CaptureOpeningRenderTransform(dialog, window);

            openingTransform.IsIdentity.ShouldBeFalse(
                "an explicit PlacementTarget should keep the existing scale/translate anchor motion.");
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

    private static TransformOperations CaptureOpeningRenderTransform(
        AtomUI.Desktop.Controls.Dialog dialog,
        AvaloniaWindow window)
    {
        TransformOperations? openingTransform = null;

        var openTask = dialog.OpenAsync();
        openTask.IsCompleted.ShouldBeTrue("non-modal Dialog.OpenAsync should finish after scheduling the opening motion.");

        Dispatcher.UIThread.Post(() =>
        {
            var overlayHost = window.GetVisualDescendants()
                                    .OfType<OverlayPopupHost>()
                                    .Single();

            openingTransform = overlayHost.RenderTransform.ShouldBeOfType<TransformOperations>();

            dialog.IsMotionEnabled = false;
            dialog.Done();
        });

        Dispatcher.UIThread.RunJobs();
        openTask.GetAwaiter().GetResult();

        openingTransform.ShouldNotBeNull();
        return openingTransform;
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
