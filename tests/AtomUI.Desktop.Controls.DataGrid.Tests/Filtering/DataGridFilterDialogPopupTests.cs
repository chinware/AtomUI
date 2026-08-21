using AtomUI.Controls.Data;
using AtomUI.Controls.Primitives;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.DataGrid.Filtering;

public class DataGridFilterDialogPopupTests
{
    static DataGridFilterDialogPopupTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void DataGrid_Filter_In_Dialog_Uses_The_Owning_Window_Popup_Layer_And_Closes()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var column = new DataGridTextColumn
            {
                Header = "Name",
                Binding = new Binding(nameof(DialogFilterRow.Name)),
                FilterMemberPath = nameof(DialogFilterRow.Name),
                Filters = new[]
                {
                    new DataGridFilterItem { Text = "Alpha", Value = "Alpha" },
                    new DataGridFilterItem { Text = "Beta", Value = "Beta" }
                }
            };
            var grid = new global::AtomUI.Desktop.Controls.DataGrid
            {
                AutoGenerateColumns = false,
                CanUserFilterColumns = true,
                IsMotionEnabled = false,
                Width = 360,
                Height = 220,
                ItemsSource = new[]
                {
                    new DialogFilterRow("Alpha"),
                    new DialogFilterRow("Beta")
                }
            };
            grid.Columns.Add(column);
            var dialog = new Dialog
            {
                Content = grid,
                DialogHostType = DialogHostType.Overlay,
                IsModal = true,
                IsMotionEnabled = false,
                HostWidth = 420,
                HostHeight = 300
            };
            var window = new AtomUI.Desktop.Controls.Window
            {
                Width = 640,
                Height = 480,
                Content = new Avalonia.Controls.Grid { Children = { dialog } }
            };

            try
            {
                window.Show();
                Pump();
                dialog.IsOpen = true;
                PumpAndRender();

                TopLevel.GetTopLevel(grid).ShouldBeSameAs(window);
                var indicator = grid.GetVisualDescendants()
                                    .OfType<DataGridFilterIndicator>()
                                    .First(control => control.IsVisible && control.Flyout is not null);

                PointerClick(window, indicator);
                var popupHost = FindPopupHost(window);
                TopLevel.GetTopLevel(popupHost).ShouldBeSameAs(window);
                AssertPopupLayer(grid, popupHost);
                indicator.Flyout.ShouldNotBeNull().IsOpen.ShouldBeTrue();

                Click(window, popupHost.GetVisualDescendants()
                                       .OfType<DataGridFilterMenuItem>()
                                       .First(item => item.Header?.ToString() == "Beta"));
                window.MouseMove(new Point(8, 8));
                window.MouseDown(new Point(8, 8), MouseButton.Left);
                window.MouseUp(new Point(8, 8), MouseButton.Left);
                PumpAndRender();

                indicator.Flyout.ShouldNotBeNull().IsOpen.ShouldBeFalse();
                AssertNoPopupHosts(window);
                dialog.IsOpen.ShouldBeTrue();
            }
            finally
            {
                dialog.IsOpen = false;
                Pump();
                window.Close();
            }
        });
    }

    private static OverlayPopupHost FindPopupHost(AtomUI.Desktop.Controls.Window window)
    {
        for (var attempt = 0; attempt < 20; attempt++)
        {
            PumpAndRender();
            if (window.GetVisualDescendants().OfType<OverlayPopupHost>().LastOrDefault() is { } host)
            {
                return host;
            }
        }

        OverlayPopupHost? missingHost = null;
        return missingHost.ShouldNotBeNull();
    }

    private static void AssertPopupLayer(Control dialogContent, OverlayPopupHost popupHost)
    {
        var overlayLayer = OverlayLayer.GetOverlayLayer(dialogContent).ShouldNotBeNull();
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
        if (upperBranch.ZIndex != lowerBranch.ZIndex)
        {
            upperBranch.ZIndex.ShouldBeGreaterThan(lowerBranch.ZIndex);
            return;
        }

        commonParent.GetVisualChildren().ToList().IndexOf(upperBranch).ShouldBeGreaterThan(
            commonParent.GetVisualChildren().ToList().IndexOf(lowerBranch));
    }

    private static void AssertNoPopupHosts(AtomUI.Desktop.Controls.Window window)
    {
        for (var attempt = 0; attempt < 20; attempt++)
        {
            PumpAndRender();
            if (!window.GetVisualDescendants().OfType<OverlayPopupHost>().Any())
            {
                return;
            }
        }

        window.GetVisualDescendants().OfType<OverlayPopupHost>().ShouldBeEmpty();
    }

    private static void Click(AtomUI.Desktop.Controls.Window window, Control control)
    {
        PumpAndRender();
        var point = control.TranslatePoint(
            new Point(control.Bounds.Width / 2, control.Bounds.Height / 2),
            window).ShouldNotBeNull();
        window.MouseMove(point);
        window.MouseDown(point, MouseButton.Left);
        window.MouseUp(point, MouseButton.Left);
        PumpAndRender();
    }

    private static void PointerClick(AtomUI.Desktop.Controls.Window window, Control control)
    {
        PumpAndRender();
        var point = control.TranslatePoint(
            new Point(control.Bounds.Width / 2, control.Bounds.Height / 2),
            window).ShouldNotBeNull();
        var pointer = new Pointer(Pointer.GetNextFreeId(), PointerType.Mouse, true);
        control.RaiseEvent(new PointerPressedEventArgs(
            control,
            pointer,
            window,
            point,
            0,
            new PointerPointProperties(RawInputModifiers.LeftMouseButton, PointerUpdateKind.LeftButtonPressed),
            KeyModifiers.None));
        control.RaiseEvent(new PointerReleasedEventArgs(
            control,
            pointer,
            window,
            point,
            1,
            new PointerPointProperties(RawInputModifiers.None, PointerUpdateKind.LeftButtonReleased),
            KeyModifiers.None,
            MouseButton.Left));
        PumpAndRender();
    }

    private static void PumpAndRender()
    {
        Pump();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick(1);
        Pump();
    }

    private static void Pump()
    {
        Dispatcher.UIThread.RunJobs();
    }
}

[GenerateDataMemberAccessors]
internal sealed partial record DialogFilterRow(string Name);
