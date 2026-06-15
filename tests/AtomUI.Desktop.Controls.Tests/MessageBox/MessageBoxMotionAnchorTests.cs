using System.Reflection;
using AtomUI.Controls.Primitives;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media.Transformation;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.MessageBox;

public class MessageBoxMotionAnchorTests
{
    static MessageBoxMotionAnchorTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Static_MessageBox_Without_PlacementTarget_Uses_Opacity_Only_Opening_Motion()
    {
        var trigger = new AtomUI.Desktop.Controls.Button
        {
            Width   = 80,
            Height  = 32,
            Content = "Open"
        };
        var window     = CreateWindow(trigger, out var overlayPanel);
        var messageBox = CreateStaticMessageBox(overlayPanel, options: null);
        messageBox.IsModal         = false;
        messageBox.IsMotionEnabled = true;

        overlayPanel.Children.Add(messageBox);

        try
        {
            var openingTransform = CaptureOpeningRenderTransform(messageBox, window);

            openingTransform.IsIdentity.ShouldBeTrue(
                "the MessageBox static API fallback placement target is only a host resolver and should not become the motion anchor.");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Static_MessageBox_With_Explicit_PlacementTarget_Keeps_Anchor_Opening_Motion()
    {
        var anchor = new Border
        {
            Width  = 40,
            Height = 40
        };
        var window = CreateWindow(anchor, out var overlayPanel);
        var messageBox = CreateStaticMessageBox(
            overlayPanel,
            new MessageBoxOptions
            {
                PlacementTarget = anchor
            });
        messageBox.IsModal         = false;
        messageBox.IsMotionEnabled = true;

        overlayPanel.Children.Add(messageBox);

        try
        {
            var openingTransform = CaptureOpeningRenderTransform(messageBox, window);

            openingTransform.IsIdentity.ShouldBeFalse(
                "an explicit MessageBox PlacementTarget should keep the existing scale/translate anchor motion.");
        }
        finally
        {
            window.Close();
        }
    }

    private static AtomUI.Desktop.Controls.MessageBox CreateStaticMessageBox(
        Control placementTarget,
        MessageBoxOptions? options)
    {
        var createMessageBox = typeof(AtomUI.Desktop.Controls.MessageBox).GetMethod(
            "CreateMessageBox",
            BindingFlags.Static | BindingFlags.NonPublic);

        createMessageBox.ShouldNotBeNull();
        return (AtomUI.Desktop.Controls.MessageBox)createMessageBox.Invoke(
            null,
            [new AtomUI.Desktop.Controls.TextBlock { Text = "Message" }, null, options, placementTarget])!;
    }

    private static TransformOperations CaptureOpeningRenderTransform(
        AtomUI.Desktop.Controls.MessageBox messageBox,
        AvaloniaWindow window)
    {
        TransformOperations? openingTransform = null;

        var openTask = messageBox.OpenAsync();
        openTask.IsCompleted.ShouldBeTrue("non-modal MessageBox.OpenAsync should finish after scheduling the opening motion.");

        Dispatcher.UIThread.Post(() =>
        {
            var overlayHost = window.GetVisualDescendants()
                                    .OfType<OverlayPopupHost>()
                                    .Single();

            openingTransform = overlayHost.RenderTransform.ShouldBeOfType<TransformOperations>();

            messageBox.IsMotionEnabled = false;
            messageBox.Cancel();
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
