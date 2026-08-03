using AtomUI.Desktop.Controls.Internal.Calendar;
using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Theme.Resources;
using Avalonia;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Threading;
using Avalonia.VisualTree;
using System.Reflection;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;
using LunarCalendarControl = AtomUI.Desktop.Controls.LunarCalendar;
using CalendarViewControl = AtomUI.Desktop.Controls.Internal.Calendar.CalendarView;

namespace AtomUI.Desktop.Controls.Tests.Calendar;

public class LunarCalendarRenderTests
{
    static LunarCalendarRenderTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Theory]
    [InlineData(true, CalendarMode.Month, 42)]
    [InlineData(false, CalendarMode.Month, 42)]
    [InlineData(true, CalendarMode.Year, 12)]
    [InlineData(false, CalendarMode.Year, 12)]
    public void AllLayoutModes_UseDedicatedLunarCells(bool fullscreen, CalendarMode mode, int expectedCount)
    {
        var calendar = new LunarCalendarControl
        {
            Value = new DateTime(2024, 2, 10),
            Fullscreen = fullscreen,
            Mode = mode
        };
        var window = Show(calendar, fullscreen ? 900 : 420, fullscreen ? 720 : 420);
        try
        {
            var cells = calendar.GetVisualDescendants().OfType<LunarCalendarViewCell>().ToList();
            cells.Count.ShouldBe(expectedCount);
            foreach (var cell in cells)
            {
                cell.LunarContext.ShouldNotBeNull();
            }
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void DefaultDateCell_RendersGregorianValueAndLunarSecondaryText()
    {
        var calendar = new LunarCalendarControl
        {
            Value = new DateTime(2024, 2, 10),
            Fullscreen = false
        };
        var window = Show(calendar);
        try
        {
            var cell = calendar.GetVisualDescendants()
                .OfType<LunarCalendarViewCell>()
                .Single(item => item.Model?.Value == calendar.Value);
            var value = cell.GetVisualDescendants()
                .OfType<AtomUI.Desktop.Controls.TextBlock>()
                .Single(text => text.Name == "PART_Value");
            var secondary = cell.GetVisualDescendants()
                .OfType<AtomUI.Desktop.Controls.TextBlock>()
                .Single(text => text.Name == "PART_SecondaryText");

            value.Text.ShouldBe("10");
            secondary.Text.ShouldBe(cell.LunarContext!.SecondaryText);
            secondary.Text.ShouldNotBeEmpty();
            secondary.IsVisible.ShouldBeTrue();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void MiniMonth_PreservesVerticalSpacingBetweenDoubleLineDateCells()
    {
        var calendar = new LunarCalendarControl
        {
            Value = new DateTime(2021, 4, 2),
            Fullscreen = false,
            Width = 450
        };
        var window = Show(calendar, 600, 600);
        try
        {
            var cells = calendar.GetVisualDescendants().OfType<LunarCalendarViewCell>().ToList();
            var selectedInner = GetCellInner(cells.Single(cell => cell.Model?.Value == new DateTime(2021, 4, 2)));
            var nextWeekInner = GetCellInner(cells.Single(cell => cell.Model?.Value == new DateTime(2021, 4, 9)));
            var selectedTop = selectedInner.TranslatePoint(default, calendar).ShouldNotBeNull().Y;
            var nextWeekTop = nextWeekInner.TranslatePoint(default, calendar).ShouldNotBeNull().Y;
            var expectedGap = GetThemeResource<double>(SharedTokenKind.UniformlyMarginXS);

            (nextWeekTop - selectedTop - selectedInner.Bounds.Height)
                .ShouldBeGreaterThanOrEqualTo(expectedGap - 0.5);

            var view = calendar.GetVisualDescendants().OfType<CalendarViewControl>().Single();
            view.Bounds.Height.ShouldBe(GetThemeResource<double>(LunarCalendarTokenKind.MiniContentHeight), 0.5);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void FullscreenDateCell_RightAlignsLunarSecondaryText()
    {
        var calendar = new LunarCalendarControl
        {
            Value = new DateTime(2024, 2, 10),
            Fullscreen = true
        };
        var window = Show(calendar, 900, 720);
        try
        {
            var cell = calendar.GetVisualDescendants()
                .OfType<LunarCalendarViewCell>()
                .Single(item => item.Model?.Value == calendar.Value);
            var presenter = cell.GetVisualDescendants()
                .OfType<Avalonia.Controls.Grid>()
                .Single(control => control.Name == "PART_SecondaryPresenter");
            var secondary = cell.GetVisualDescendants()
                .OfType<AtomUI.Desktop.Controls.TextBlock>()
                .Single(text => text.Name == "PART_SecondaryText");

            presenter.HorizontalAlignment.ShouldBe(Avalonia.Layout.HorizontalAlignment.Right);
            secondary.TextAlignment.ShouldBe(Avalonia.Media.TextAlignment.Right);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void CellTemplateReplacesSecondaryContent_AndFullCellTemplateTakesPriority()
    {
        var calendar = new LunarCalendarControl
        {
            Value = new DateTime(2024, 2, 10),
            Fullscreen = false,
            CellTemplate = MarkerTemplate("cell"),
            FullCellTemplate = MarkerTemplate("full")
        };
        var window = Show(calendar);
        try
        {
            var cell = calendar.GetVisualDescendants()
                .OfType<LunarCalendarViewCell>()
                .Single(item => item.Model?.Value == calendar.Value);

            cell.GetVisualDescendants().OfType<Control>().Any(control => Equals(control.Tag, "full")).ShouldBeTrue();
            cell.GetVisualDescendants().OfType<Control>().Any(control => Equals(control.Tag, "cell")).ShouldBeFalse();
            cell.ShowSecondaryContent.ShouldBeFalse();
            cell.GetVisualDescendants()
                .OfType<AtomUI.Desktop.Controls.TextBlock>()
                .Single(text => text.Name == "PART_Value")
                .IsVisible.ShouldBeFalse();
            cell.GetVisualDescendants()
                .OfType<AtomUI.Desktop.Controls.TextBlock>()
                .Single(text => text.Name == "PART_SecondaryText")
                .IsVisible.ShouldBeFalse();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void CellAutomationName_AppendsLunarPresentation()
    {
        var calendar = new LunarCalendarControl { Value = new DateTime(2024, 2, 10) };
        var window = Show(calendar);
        try
        {
            var cell = calendar.GetVisualDescendants()
                .OfType<LunarCalendarViewCell>()
                .Single(item => item.Model?.Value == calendar.Value);
            var peer = ControlAutomationPeer.CreatePeerForElement(cell)
                .ShouldBeOfType<CalendarViewCellAutomationPeer>();

            peer.GetName().ShouldContain("2024");
            peer.GetName().ShouldContain(cell.LunarContext!.SecondaryText);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void FullscreenMonth_UsesLunarRangeBarOffsetAfterSecondaryLine()
    {
        var calendar = new LunarCalendarControl
        {
            Value = new DateTime(2024, 2, 10),
            Fullscreen = true,
            RangeBars =
            {
                new CalendarRangeBar
                {
                    StartDate = new DateTime(2024, 2, 10),
                    EndDate = new DateTime(2024, 2, 12),
                    Label = "Range"
                }
            }
        };
        var window = Show(calendar, 900, 720);
        try
        {
            var panel = calendar.GetVisualDescendants().OfType<CalendarRangeBarPanel>().Single();
            var cell = calendar.GetVisualDescendants()
                .OfType<LunarCalendarViewCell>()
                .Single(item => item.Model?.Value == calendar.Value);
            var secondary = cell.GetVisualDescendants()
                .OfType<AtomUI.Desktop.Controls.TextBlock>()
                .Single(text => text.Name == "PART_SecondaryText");

            panel.RangeBarTopOffset.ShouldBeGreaterThan(secondary.Bounds.Bottom);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void SwitchingMode_ClearsInactiveLunarContextsInTheBoundedPool()
    {
        var calendar = new LunarCalendarControl
        {
            Value = new DateTime(2024, 2, 10),
            Mode = CalendarMode.Month
        };
        var window = Show(calendar);
        try
        {
            var view = calendar.GetVisualDescendants().OfType<CalendarViewControl>().Single();
            calendar.Mode = CalendarMode.Year;
            Dispatcher.UIThread.RunJobs();

            var pool = (IReadOnlyList<CalendarViewCell>)typeof(CalendarViewControl)
                .GetField("_cellPool", BindingFlags.Instance | BindingFlags.NonPublic)!
                .GetValue(view)!;
            pool.Count.ShouldBe(42);
            foreach (var cell in pool.Take(12))
            {
                cell.ShouldBeOfType<LunarCalendarViewCell>().LunarContext.ShouldNotBeNull();
            }
            foreach (var cell in pool.Skip(12))
            {
                cell.ShouldBeOfType<LunarCalendarViewCell>().LunarContext.ShouldBeNull();
            }
        }
        finally
        {
            window.Close();
        }
    }

    private static IDataTemplate MarkerTemplate(string marker) =>
        new FuncDataTemplate<object?>((_, _) => new Border
        {
            Tag = marker,
            Width = 8,
            Height = 8
        });

    private static Border GetCellInner(LunarCalendarViewCell cell) =>
        cell.GetVisualDescendants()
            .OfType<Border>()
            .Single(border => border.Name == "PART_CellInner");

    private static T GetThemeResource<T>(object key)
    {
        var application = Application.Current.ShouldNotBeNull();
        application!.TryGetResource(key, application.ActualThemeVariant, out var value).ShouldBeTrue();
        value.ShouldBeAssignableTo<T>();
        return (T)value!;
    }

    private static AvaloniaWindow Show(Control content, double width = 420, double height = 420)
    {
        var window = new AvaloniaWindow
        {
            Width = width,
            Height = height,
            Content = content
        };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        return window;
    }
}
