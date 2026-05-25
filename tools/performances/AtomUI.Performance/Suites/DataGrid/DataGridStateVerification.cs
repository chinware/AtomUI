using System.Collections;
using System.Reflection;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunDataGridStateVerification()
    {
        var failures = new List<string>();
        VerifyDataGridPlainHeadersDoNotCreateFilterFlyouts(failures);
        VerifyDataGridFilterFlyoutContentIsLazy(failures);
        VerifyDataGridFilterIndicatorVisibilityTracksFilterItems(failures);
        VerifyDataGridColumnHeaderHoverUsesOverrides(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("DataGrid state verification passed.");
            return true;
        }

        Console.Error.WriteLine("DataGrid state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyDataGridPlainHeadersDoNotCreateFilterFlyouts(ICollection<string> failures)
    {
        var grid = CreateBasicDataGrid(rowCount: 4, columnCount: 3);
        using var realized = RealizeControl(grid);

        var indicators = GetDataGridFilterIndicators(grid);
        Expect(indicators.Count == 4,
            $"Plain DataGrid should create one filter indicator slot per column plus the top-left header. Actual: {indicators.Count}.",
            failures);
        Expect(indicators.All(indicator => GetDataGridFilterFlyout(indicator) is null),
            "Plain DataGrid headers without filter items should not create filter flyout shells.",
            failures);
    }

    private static void VerifyDataGridFilterFlyoutContentIsLazy(ICollection<string> failures)
    {
        var grid = CreateFilterDataGrid(DataGridFilterMode.Menu);
        var realized = RealizeControl(grid);

        var indicators = GetDataGridFilterIndicators(grid);
        Expect(indicators.Count == 4,
            $"Filter DataGrid should create one filter indicator slot per column plus the top-left header. Actual: {indicators.Count}.",
            failures);

        var indicatorsWithFlyout = indicators
            .Where(indicator => GetDataGridFilterFlyout(indicator) is not null)
            .ToList();
        Expect(indicatorsWithFlyout.Count == 2,
            $"Only columns with filter items should create filter flyout shells. Actual: {indicatorsWithFlyout.Count}.",
            failures);

        var closedFlyoutItemCount = indicatorsWithFlyout
            .Select(indicator => GetDataGridFilterFlyout(indicator))
            .Where(flyout => flyout is not null)
            .Sum(flyout => GetDataGridFlyoutItemCount(flyout!));
        Expect(closedFlyoutItemCount == 0,
            $"Closed filter flyout shells should not materialize menu items. Actual item count: {closedFlyoutItemCount}.",
            failures);

        var firstFilterIndicator = indicatorsWithFlyout[0];
        MaterializeDataGridFilterFlyoutContent(firstFilterIndicator);
        var firstFlyout = GetDataGridFilterFlyout(firstFilterIndicator);
        Expect(firstFlyout is not null && GetDataGridFlyoutItemCount(firstFlyout) > 0,
            "First filter flyout should materialize its items immediately before opening.",
            failures);

        var secondFlyout = GetDataGridFilterFlyout(indicatorsWithFlyout[1]);
        Expect(secondFlyout is not null && GetDataGridFlyoutItemCount(secondFlyout) == 0,
            "Unopened sibling filter flyout should remain unmaterialized.",
            failures);

        realized.Dispose();
        Expect(indicators.All(indicator => GetDataGridFilterFlyout(indicator) is null),
            "Detached DataGrid filter indicators should clear their flyout shells.",
            failures);
    }

    private static void VerifyDataGridFilterIndicatorVisibilityTracksFilterItems(ICollection<string> failures)
    {
        var grid = CreateDataGridShell(4);
        grid.CanUserFilterColumns = true;

        var filterColumn = new DataGridTextColumn
        {
            Header           = "Name",
            Binding          = new Binding(nameof(PerfDataGridRow.Name)),
            FilterMemberPath = nameof(PerfDataGridRow.Name)
        };
        grid.Columns.Add(filterColumn);
        grid.Columns.Add(new DataGridTextColumn
        {
            Header  = "Age",
            Binding = new Binding(nameof(PerfDataGridRow.Age))
        });

        using var realized = RealizeControl(grid);
        Expect(CountVisibleDataGridFilterIndicators(grid) == 0,
            "Columns without filter items should keep filter indicators hidden even when filtering is enabled.",
            failures);

        filterColumn.Filters.Add(new DataGridFilterItem { Text = "Joe", Value = "Joe" });
        RefreshLayout(realized.Window);
        Expect(CountVisibleDataGridFilterIndicators(grid) == 1,
            $"Adding one filter item should reveal one filter indicator. Actual: {CountVisibleDataGridFilterIndicators(grid)}.",
            failures);
        Expect(GetDataGridFilterIndicators(grid).Count(indicator => GetDataGridFilterFlyout(indicator) is not null) == 1,
            "Adding one filter item should create exactly one closed filter flyout shell.",
            failures);

        filterColumn.Filters.Clear();
        RefreshLayout(realized.Window);
        Expect(CountVisibleDataGridFilterIndicators(grid) == 0,
            $"Clearing filter items should hide the filter indicator. Actual: {CountVisibleDataGridFilterIndicators(grid)}.",
            failures);
        Expect(GetDataGridFilterIndicators(grid).All(indicator => GetDataGridFilterFlyout(indicator) is null),
            "Clearing filter items should release the closed filter flyout shell.",
            failures);
    }

    private static void VerifyDataGridColumnHeaderHoverUsesOverrides(ICollection<string> failures)
    {
        var grid = CreateBasicDataGrid(rowCount: 4, columnCount: 3);
        using var realized = RealizeControl(grid);

        var headers = GetDataGridColumnHeaders(grid);
        Expect(headers.Count >= 3,
            $"Basic 3-column grid should realize at least 3 standard column headers. Actual: {headers.Count}.",
            failures);

        foreach (var header in headers)
        {
            var localHandlerNames = GetLocalRoutedHandlerNames(header);
            Expect(!localHandlerNames.Contains("InputElement.PointerEntered"),
                "DataGrid column headers should handle PointerEntered through OnPointerEntered instead of per-instance handlers.",
                failures);
            Expect(!localHandlerNames.Contains("InputElement.PointerExited"),
                "DataGrid column headers should handle PointerExited through OnPointerExited instead of per-instance handlers.",
                failures);
        }
    }

    private static List<Control> GetDataGridFilterIndicators(Control root)
    {
        return root.GetSelfAndVisualDescendants()
                   .OfType<Control>()
                   .Where(control => control.GetType().Name == "DataGridFilterIndicator")
                   .ToList();
    }

    private static List<Control> GetDataGridColumnHeaders(Control root)
    {
        return root.GetSelfAndVisualDescendants()
                   .OfType<Control>()
                   .Where(control => control.GetType().Name == "DataGridColumnHeader")
                   .ToList();
    }

    private static int CountVisibleDataGridFilterIndicators(Control root)
    {
        return GetDataGridFilterIndicators(root).Count(indicator => indicator.IsVisible);
    }

    private static HashSet<string> GetLocalRoutedHandlerNames(Control control)
    {
        var field = typeof(Interactive).GetField("_eventHandlers", BindingFlags.Instance | BindingFlags.NonPublic);
        if (field?.GetValue(control) is not IDictionary eventHandlers)
        {
            return [];
        }

        var names = new HashSet<string>();
        foreach (DictionaryEntry entry in eventHandlers)
        {
            if (entry.Key != null)
            {
                names.Add(entry.Key.ToString() ?? string.Empty);
            }
        }
        return names;
    }

    private static PopupFlyoutBase? GetDataGridFilterFlyout(Control indicator)
    {
        var property = indicator.GetType().GetProperty(
            "Flyout",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        return property?.GetValue(indicator) as PopupFlyoutBase;
    }

    private static void MaterializeDataGridFilterFlyoutContent(Control indicator)
    {
        var method = indicator.GetType().GetMethod(
            "MaterializeFlyoutContent",
            BindingFlags.Instance | BindingFlags.NonPublic);
        method?.Invoke(indicator, null);
    }

    private static int GetDataGridFlyoutItemCount(PopupFlyoutBase flyout)
    {
        var items = flyout.GetType()
                          .GetProperty("Items", BindingFlags.Instance | BindingFlags.Public)?
                          .GetValue(flyout) as IEnumerable;
        return CountDataGridFlyoutItems(items);
    }

    private static int CountDataGridFlyoutItems(IEnumerable? items)
    {
        if (items is null)
        {
            return 0;
        }

        var count = 0;
        foreach (var item in items)
        {
            count++;
            if (item is ItemsControl itemsControl)
            {
                count += CountDataGridFlyoutItems(itemsControl.Items);
            }
        }
        return count;
    }
}
