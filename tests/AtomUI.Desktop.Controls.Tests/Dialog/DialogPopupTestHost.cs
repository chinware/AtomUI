using AtomUI.Controls.Primitives;
using AtomUI.Desktop.Controls.Tests.Window;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;

namespace AtomUI.Desktop.Controls.Tests.Dialog;

internal sealed class DialogPopupTestHost : IDisposable
{
    private readonly Action? _restoreDecorations;
    private bool _disposed;

    private DialogPopupTestHost(
        AtomUI.Desktop.Controls.Window window,
        Border placementTarget,
        Control content,
        OverlayDialogPresenter presenter,
        Action? restoreDecorations)
    {
        Window = window;
        PlacementTarget = placementTarget;
        Content = content;
        Presenter = presenter;
        _restoreDecorations = restoreDecorations;
    }

    public AtomUI.Desktop.Controls.Window Window { get; }

    public Border PlacementTarget { get; }

    public Control Content { get; }

    public OverlayDialogPresenter Presenter { get; }

    public DialogOverlayLayer DialogLayer => Presenter.Parent.ShouldBeOfType<DialogOverlayLayer>();

    public static DialogPopupTestHost Open(Control content, bool installSiblingDecorations = true)
    {
        var placementTarget = new Border { Width = 100, Height = 40 };
        var window = new AtomUI.Desktop.Controls.Window
        {
            Width = 640,
            Height = 480,
            Content = new ScopeAwareOverlayLayerPanel
            {
                Children = { placementTarget }
            }
        };
        var dialog = new AtomUI.Desktop.Controls.Dialog
        {
            Content = content,
            IsModal = true,
            IsMaskClosable = false,
            IsMotionEnabled = false,
            HostWidth = 420,
            HostHeight = 300
        };
        var presenter = new OverlayDialogPresenter(dialog, placementTarget);

        window.Show();
        Pump();

        Action? restoreDecorations = null;
        if (installSiblingDecorations)
        {
            var chrome = new Border { Name = "DrawnChromeTestSibling" };
            restoreDecorations = DrawnDecorationsTestHost.InstallOnAttachedDecorations(window, chrome);
            TopLevel.GetTopLevel(chrome).ShouldBeNull();
        }

        Wait(presenter.ShowAsync(CancellationToken.None).AsTask());
        Pump();

        content.IsAttachedToVisualTree().ShouldBeTrue();
        TopLevel.GetTopLevel(content).ShouldBeSameAs(window);
        presenter.Parent.ShouldBeOfType<DialogOverlayLayer>()
                 .GetVisualParent()
                 .ShouldBeOfType<OverlayLayer>();

        return new DialogPopupTestHost(window, placementTarget, content, presenter, restoreDecorations);
    }

    public OverlayPopupHost FindPopupHost(Control? logicalOwner = null)
    {
        for (var attempt = 0; attempt < 20; attempt++)
        {
            Pump();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick(1);
            Pump();

            var hosts = Window.GetVisualDescendants().OfType<OverlayPopupHost>();
            if (logicalOwner is not null)
            {
                hosts = hosts.Where(host => host.GetLogicalAncestors().Contains(logicalOwner));
            }

            if (hosts.LastOrDefault() is { } popupHost)
            {
                AssertPopupLayer(popupHost);
                return popupHost;
            }
        }

        OverlayPopupHost? missingHost = null;
        return missingHost.ShouldNotBeNull();
    }

    public void AssertNoPopupHosts()
    {
        for (var attempt = 0; attempt < 20; attempt++)
        {
            Pump();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick(1);
            Pump();
            if (!Window.GetVisualDescendants().OfType<OverlayPopupHost>().Any())
            {
                return;
            }
        }

        Window.GetVisualDescendants().OfType<OverlayPopupHost>().ShouldBeEmpty();
    }

    private void AssertPopupLayer(OverlayPopupHost popupHost)
    {
        TopLevel.GetTopLevel(popupHost).ShouldBeSameAs(Window);
        var overlayLayer = DialogLayer.GetVisualParent().ShouldBeOfType<OverlayLayer>();
        var manager = overlayLayer.GetVisualAncestors().OfType<VisualLayerManager>().Single();
        popupHost.GetVisualAncestors().OfType<VisualLayerManager>().Single().ShouldBeSameAs(manager);
        AssertRendersAbove(popupHost, overlayLayer);
    }

    private static void AssertRendersAbove(Visual upper, Visual lower)
    {
        var upperPath = upper.GetSelfAndVisualAncestors().Reverse().ToList();
        var lowerPath = lower.GetSelfAndVisualAncestors().Reverse().ToList();
        var sharedCount = 0;
        while (sharedCount < upperPath.Count &&
               sharedCount < lowerPath.Count &&
               ReferenceEquals(upperPath[sharedCount], lowerPath[sharedCount]))
        {
            sharedCount++;
        }

        sharedCount.ShouldBeGreaterThan(0);
        sharedCount.ShouldBeLessThan(upperPath.Count);
        sharedCount.ShouldBeLessThan(lowerPath.Count);
        var commonParent = upperPath[sharedCount - 1];
        var upperBranch = upperPath[sharedCount];
        var lowerBranch = lowerPath[sharedCount];
        var siblings = commonParent.GetVisualChildren().ToList();

        if (upperBranch.ZIndex != lowerBranch.ZIndex)
        {
            upperBranch.ZIndex.ShouldBeGreaterThan(
                lowerBranch.ZIndex,
                "the owning Window popup branch must render above the Dialog overlay branch.");
            return;
        }

        siblings.IndexOf(upperBranch).ShouldBeGreaterThan(
            siblings.IndexOf(lowerBranch),
            "the owning Window popup branch must follow the Dialog overlay branch in render order.");
    }

    public void Click(Control control)
    {
        Pump();
        var clickPoint = control.TranslatePoint(
            new Point(control.Bounds.Width / 2, control.Bounds.Height / 2),
            Window);
        clickPoint.ShouldNotBeNull();

        Window.MouseMove(clickPoint.Value);
        Window.MouseDown(clickPoint.Value, MouseButton.Left);
        Window.MouseUp(clickPoint.Value, MouseButton.Left);
        Pump();
    }

    public void PointerClick(Control control)
    {
        Pump();
        var position = control.TranslatePoint(
            new Point(control.Bounds.Width / 2, control.Bounds.Height / 2),
            Window).ShouldNotBeNull();
        var pointer = new Pointer(Pointer.GetNextFreeId(), PointerType.Mouse, true);
        control.RaiseEvent(new PointerPressedEventArgs(
            control,
            pointer,
            Window,
            position,
            0,
            new PointerPointProperties(RawInputModifiers.LeftMouseButton, PointerUpdateKind.LeftButtonPressed),
            KeyModifiers.None));
        control.RaiseEvent(new PointerReleasedEventArgs(
            control,
            pointer,
            Window,
            position,
            1,
            new PointerPointProperties(RawInputModifiers.None, PointerUpdateKind.LeftButtonReleased),
            KeyModifiers.None,
            MouseButton.Left));
        Pump();
    }

    public void LightDismiss()
    {
        Window.MouseMove(new Point(8, 8));
        Window.MouseDown(new Point(8, 8), MouseButton.Left);
        Window.MouseUp(new Point(8, 8), MouseButton.Left);
        Pump();
    }

    public void WaitForClosed(Func<bool> isOpen)
    {
        for (var attempt = 0; attempt < 20 && isOpen(); attempt++)
        {
            Pump();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick(1);
            Pump();
        }

        isOpen().ShouldBeFalse();
    }

    public static void Pump()
    {
        Dispatcher.UIThread.RunJobs();
    }

    public static void Wait(Task task)
    {
        var timeoutAt = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(5);
        while (!task.IsCompleted && DateTimeOffset.UtcNow < timeoutAt)
        {
            Pump();
            Thread.Sleep(1);
        }

        task.IsCompleted.ShouldBeTrue();
        task.GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        Wait(Presenter.CloseAsync().AsTask());
        Wait(Presenter.DisposeAsync().AsTask());
        _restoreDecorations?.Invoke();
        Window.Close();
    }
}
