using AtomUI.Toolkits.GalleryBase.Controls;
using AtomUIGallery.ShowCases.TabControl;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Headless;
using Avalonia.Threading;
using Avalonia.VisualTree;
using ReactiveUI;
using Xunit;
using AtomUIScrollViewer = AtomUI.Desktop.Controls.ScrollViewer;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUIGallery.Tests.ShowCases;

public class WheelDiagnosticTests
{
    private readonly ITestOutputHelper _output;

    public WheelDiagnosticTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Diagnostic_Wheel_Chain_Under_Second_Preview()
    {
        AvaloniaTestApp.EnsureInitialized();

        var page = new TabControlShowCase
        {
            DataContext = new TabControlViewModel(new TestScreen())
        };

        var visualLayerManager = new VisualLayerManager
        {
            EnableAdornerLayer = true,
            Child = page
        };
        var window = new AvaloniaWindow
        {
            Content = visualLayerManager,
            Width = 1280,
            Height = 900
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var host = page.GetVisualDescendants().OfType<GalleryShowCaseHost>().Single();
            host.SelectedTab = GalleryShowCaseTab.SemanticParts;
            Dispatcher.UIThread.RunJobs();
            Dispatcher.UIThread.RunJobs();

            var pageScroller = page.GetVisualDescendants()
                                   .OfType<AtomUIScrollViewer>()
                                   .Single(static candidate => candidate.Name == "PART_ScrollViewer");
            pageScroller.Offset = new Vector(0, 400);
            Dispatcher.UIThread.RunJobs();
            Dispatcher.UIThread.RunJobs();

            var previews = page.GetVisualDescendants().OfType<SemanticPartPreview>().ToArray();
            foreach (var preview in previews)
            {
                var bounds = preview.TransformToVisual(window)!.Value.Transform(default);
                _output.WriteLine($"{preview.Name}: window y=[{bounds.Y:0}, {bounds.Y + preview.Bounds.Height:0}] h={preview.Bounds.Height:0}");
            }

            var second = previews.Single(static p => p.Name == "CardTabControlSemanticPreview");
            var stage = second.GetVisualDescendants().OfType<Border>()
                              .Single(static b => b.Name == "PART_PreviewStage");
            var stageTopLeft = stage.TransformToVisual(window)!.Value.Transform(default);
            _output.WriteLine($"second stage: window x={stageTopLeft.X:0} y={stageTopLeft.Y:0} w={stage.Bounds.Width:0} h={stage.Bounds.Height:0}");

            var demo = page.GetVisualDescendants()
                           .OfType<AtomUI.Desktop.Controls.CardTabControl>()
                           .Single(static c => c.Name == "CardTabControlSemanticOwner");
            var demoTopLeft = demo.TransformToVisual(window)!.Value.Transform(default);
            _output.WriteLine($"demo: window x={demoTopLeft.X:0} y={demoTopLeft.Y:0} w={demo.Bounds.Width:0} h={demo.Bounds.Height:0}");

            var relative = new Point(210, 30);
            var hit = demo.InputHitTest(relative);
            _output.WriteLine($"demo InputHitTest({relative.X:0},{relative.Y:0}) = {hit?.GetType().Name} {(hit is Control hc && !string.IsNullOrEmpty(hc.Name) ? $"#{hc.Name}" : "")}");
            _output.WriteLine($"demo IsHitTestVisible={demo.IsHitTestVisible} IsVisible={demo.IsVisible} IsEffectivelyVisible={demo.IsEffectivelyVisible}");
            foreach (var visual in demo.GetVisualDescendants().Take(14))
            {
                var info = $"{visual.GetType().Name}{(visual is Control c && !string.IsNullOrEmpty(c.Name) ? $"#{c.Name}" : "")} " +
                           $"bounds={visual.Bounds.Width:0}x{visual.Bounds.Height:0}@{visual.Bounds.X:0},{visual.Bounds.Y:0} " +
                           $"visible={visual.IsVisible}" +
                           (visual is InputElement ie ? $" hitVisible={ie.IsHitTestVisible}" : "");
                _output.WriteLine($"  demo-descendant {info}");
            }
            for (var ancestor = demo.Parent; ancestor is not null; ancestor = ancestor.Parent)
            {
                if (ancestor is not InputElement ie)
                {
                    continue;
                }

                _output.WriteLine(
                    $"ancestor {ancestor.GetType().Name}{(ancestor is Control ac && !string.IsNullOrEmpty(ac.Name) ? $"#{ac.Name}" : "")} " +
                    $"IsHitTestVisible={ie.IsHitTestVisible} IsVisible={ie.IsVisible} ClipToBounds={ancestor is Control cc && cc.ClipToBounds}");
            }

            var pane = second.GetVisualDescendants().OfType<Border>()
                             .Single(static b => b.Name == "PART_PartsPane");
            var paneTopLeft = pane.TransformToVisual(window)!.Value.Transform(default);
            _output.WriteLine($"second pane: window x={paneTopLeft.X:0} y={paneTopLeft.Y:0} w={pane.Bounds.Width:0} h={pane.Bounds.Height:0}");

            ProbeWheel(window, "demo-center", PointInWindow(window, demo), pageScroller);
            ProbeWheel(window, "stage-top-left+50,150", ClampPoint(window, stageTopLeft + new Point(50, 150)), pageScroller);
            ProbeWheel(window, "stage-center", ClampPoint(window, stageTopLeft + new Point(stage.Bounds.Width / 2, stage.Bounds.Height / 2)), pageScroller);
            ProbeWheel(window, "pane-center", PointInWindow(window, pane, new Point(pane.Bounds.Width / 2, 40)), pageScroller);
            ProbeWheel(window, "window-center", new Point(640, 450), pageScroller);
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    private void ProbeWheel(AvaloniaWindow window, string label, Point point, AtomUIScrollViewer pageScroller)
    {
        var before = pageScroller.Offset.Y;
        var chain = string.Join(
            " -> ",
            window.GetVisualsAt(point)
                  .Take(8)
                  .Select(v => $"{v.GetType().Name}{(v is Control c && !string.IsNullOrEmpty(c.Name) ? $"#{c.Name}" : "")}"));
        window.MouseWheel(new Point(point.X, point.Y), new Vector(0, -120));
        Dispatcher.UIThread.RunJobs();
        Dispatcher.UIThread.RunJobs();
        var after = pageScroller.Offset.Y;
        _output.WriteLine($"[{label}] ({point.X:0},{point.Y:0}) offset {before:0} -> {after:0} | chain: {chain}");
    }

    private static Point PointInWindow(AvaloniaWindow window, Control target, Point? relative = null)
    {
        relative ??= new Point(target.Bounds.Width / 2, target.Bounds.Height / 2);
        return ClampPoint(window, target.TransformToVisual(window)!.Value.Transform(relative.Value));
    }

    private static Point ClampPoint(AvaloniaWindow window, Point point)
    {
        var x = Math.Clamp(point.X, 8, window.Bounds.Width - 8);
        var y = Math.Clamp(point.Y, 100, window.Bounds.Height - 16);
        return new Point(x, y);
    }
}