using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaWindow = Avalonia.Controls.Window;
using Shouldly;
using Xunit;
using AtomUIOtpLineEdit = AtomUI.Desktop.Controls.OtpLineEdit;

namespace AtomUI.Desktop.Controls.Tests.Input;

public class OtpLineEditCellCustomizationTests
{
    static OtpLineEditCellCustomizationTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Cell_Width_And_BorderBrush_Propagate_To_Every_Cell()
    {
        var brush = new SolidColorBrush(Colors.CornflowerBlue);
        var otp = new AtomUIOtpLineEdit
        {
            Length = 6,
            Separator = "*",
            CellWidth = 32,
            CellBorderBrush = brush,
            IsMotionEnabled = false
        };

        var window = Show(otp);
        try
        {
            var cells = GetCells(otp);
            cells.Count.ShouldBe(6);
            cells.ShouldAllBe(cell => cell.Bounds.Width == 32);
            cells.ShouldAllBe(cell => ReferenceEquals(cell.BorderBrush, brush));
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Unset_Cell_Overrides_Fall_Back_To_Theme_Defaults()
    {
        var otp = new AtomUIOtpLineEdit
        {
            Length = 4,
            IsMotionEnabled = false
        };

        var window = Show(otp);
        try
        {
            var cells = GetCells(otp);
            cells.Count.ShouldBe(4);
            foreach (var cell in cells)
            {
                cell.Bounds.Width.ShouldBeGreaterThan(0);
                cell.BorderBrush.ShouldNotBeNull();
            }
        }
        finally
        {
            window.Close();
        }
    }

    private static IReadOnlyList<OtpLineEditCell> GetCells(AtomUIOtpLineEdit owner)
    {
        return owner.GetVisualDescendants().OfType<OtpLineEditCell>().ToArray();
    }

    private static AvaloniaWindow Show(Control content)
    {
        var window = new AvaloniaWindow
        {
            Width = 480,
            Height = 120,
            Content = content
        };
        window.Show();
        content.ApplyTemplate();
        window.UpdateLayout();
        Dispatcher.UIThread.RunJobs();
        return window;
    }
}
