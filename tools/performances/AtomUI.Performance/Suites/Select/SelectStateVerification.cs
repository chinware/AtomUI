using System.Reflection;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunSelectStateVerification()
    {
        var failures = new List<string>();
        VerifyClosedSelectCost(failures);
        VerifySelectPopupLifecycle(failures);
        VerifySelectModeSpecificContent(failures);
        VerifySelectFilteringSelection(failures);
        VerifySelectAccessoryPaths(failures);
        VerifySelectLoadingLifecycle(failures);
        VerifyTreeSelectPopupLifecycle(failures);
        VerifyCascaderPopupLifecycle(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Select state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Select state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyClosedSelectCost(ICollection<string> failures)
    {
        var select = new Select
        {
            OptionsSource = CreateSelectOptions()
        };
        using var realized = RealizeControl(select);

        Expect(FindVisualByName<SelectCandidateList>(select, "PART_CandidateList") == null,
            "Closed default Select should not create SelectCandidateList.",
            failures);
        Expect(FindVisualByName<SelectFilterTextBox>(select, "PART_SingleFilterInput") == null,
            "Closed default Select without filter should not create SelectFilterTextBox.",
            failures);
        Expect(FindVisualByName<SelectResultOptionsBox>(select, "SelectedOptionsBox") == null,
            "Closed default single Select should not create SelectResultOptionsBox.",
            failures);
        Expect(FindVisualByName<SelectAccessoryHost>(select, "PART_SelectAccessoryHost") == null &&
               FindVisualByTypeName(select, "SelectAccessoryHost") == null,
            "Closed default Select should use lightweight SelectHandle instead of SelectAccessoryHost.",
            failures);
        Expect(FindVisualByTypeName(select, "SelectHandle") != null,
            "Closed default Select should still show a lightweight SelectHandle.",
            failures);
        Expect(GetPopupFrame(select) == null,
            "Closed default Select should keep Popup child empty before first open.",
            failures);
    }

    private static void VerifySelectPopupLifecycle(ICollection<string> failures)
    {
        var select = new Select
        {
            OptionsSource = CreateSelectOptions()
        };
        SelectCandidateList? firstCandidateList;
        using (var realized = RealizeControl(select))
        {
            MaterializeLazyPopupContentForTest(select);
            RefreshLayout(realized.Window);
            firstCandidateList = GetPopupContent<SelectCandidateList>(select);
            Expect(firstCandidateList != null,
                "Materializing Select popup should lazily create SelectCandidateList.",
                failures);

            MaterializeLazyPopupContentForTest(select);
            RefreshLayout(realized.Window);
            Expect(ReferenceEquals(firstCandidateList, GetPopupContent<SelectCandidateList>(select)),
                "Second Select popup materialization should reuse the first SelectCandidateList.",
                failures);
        }

        Expect(firstCandidateList?.GetVisualParent() == null,
            "Detached Select should clear lazy SelectCandidateList visual parent.",
            failures);
    }

    private static void VerifySelectModeSpecificContent(ICollection<string> failures)
    {
        var filterSelect = new Select
        {
            IsFilterEnabled = true,
            OptionsSource   = CreateSelectOptions()
        };
        using var filterRealized = RealizeControl(filterSelect);
        Expect(FindVisualByName<SelectFilterTextBox>(filterSelect, "PART_SingleFilterInput") != null,
            "Single filter Select should create SelectFilterTextBox.",
            failures);
        Expect(GetPopupContent<SelectCandidateList>(filterSelect) == null,
            "Closed single filter Select should still defer SelectCandidateList.",
            failures);

        var multiSelect = new Select
        {
            Mode          = SelectMode.Multiple,
            OptionsSource = CreateSelectOptions()
        };
        using var multiRealized = RealizeControl(multiSelect);
        Expect(FindVisualByName<SelectResultOptionsBox>(multiSelect, "SelectedOptionsBox") != null,
            "Multiple Select should create SelectResultOptionsBox.",
            failures);
        Expect(FindVisualByName<SelectFilterTextBox>(multiSelect, "PART_SingleFilterInput") == null,
            "Multiple Select should not create the single-mode filter text box.",
            failures);

        multiSelect.SetCurrentValue(Select.ModeProperty, SelectMode.Single);
        RefreshLayout(multiRealized.Window);
        Expect(FindVisualByName<SelectResultOptionsBox>(multiSelect, "SelectedOptionsBox") == null,
            "Switching Multiple Select to Single should detach SelectResultOptionsBox.",
            failures);
    }

    private static void VerifySelectFilteringSelection(ICollection<string> failures)
    {
        var options = CreateSelectOptions();
        var select = new Select
        {
            IsFilterEnabled = true,
            OptionsSource   = options
        };

        using var realized = RealizeControl(select);
        MaterializeLazyPopupContentForTest(select);
        RefreshLayout(realized.Window);

        var candidateList = GetPopupContent<SelectCandidateList>(select);
        Expect(select.Filter != null,
            "Filter-enabled Select should create a default filter.",
            failures);
        Expect(candidateList != null,
            "Materializing filter-enabled Select popup should create SelectCandidateList.",
            failures);

        if (candidateList == null)
        {
            return;
        }

        _ = candidateList.Selection;
        select.SetCurrentValue(AbstractSelect.FilterValueProperty, "Grape");
        RefreshLayout(realized.Window);

        Expect(candidateList.TotalItemCount == 1,
            "Filter-enabled Select should reduce candidates with the default Contains filter.",
            failures);
        var firstCandidate = candidateList.Items.Count > 0 ? candidateList.Items[0] : null;
        Expect(ReferenceEquals(firstCandidate, options[4]),
            "Filtered Select candidate should be the matching option from the original source.",
            failures);

        var mappedIndex = typeof(ListView)
            .GetMethod("GetSelectionIndexFromViewIndex", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.Invoke(candidateList, [0]) as int?;
        Expect(mappedIndex == 4,
            "Filtered Select visible candidate index should map back to the original source item index.",
            failures);
    }

    private static void VerifySelectAccessoryPaths(ICollection<string> failures)
    {
        var defaultSelect = new Select
        {
            OptionsSource = CreateSelectOptions()
        };
        using var defaultRealized = RealizeControl(defaultSelect);
        Expect(FindVisualByTypeName(defaultSelect, "SelectAccessoryHost") == null,
            "Default Select should not create SelectAccessoryHost.",
            failures);

        var maxCountSelect = new Select
        {
            Mode                    = SelectMode.Multiple,
            IsShowMaxCountIndicator = true,
            OptionsSource           = CreateSelectOptions()
        };
        using var maxCountRealized = RealizeControl(maxCountSelect);
        Expect(FindVisualByTypeName(maxCountSelect, "SelectAccessoryHost") != null,
            "Select with max count indicator should create SelectAccessoryHost.",
            failures);

        maxCountSelect.SetCurrentValue(AbstractSelect.IsShowMaxCountIndicatorProperty, false);
        RefreshLayout(maxCountRealized.Window);
        Expect(FindVisualByTypeName(maxCountSelect, "SelectAccessoryHost") == null,
            "Turning max count indicator off should detach SelectAccessoryHost.",
            failures);
        Expect(FindVisualByTypeName(maxCountSelect, "SelectHandle") != null,
            "Turning max count indicator off should fall back to lightweight SelectHandle.",
            failures);
    }

    private static void VerifySelectLoadingLifecycle(ICollection<string> failures)
    {
        var select = new Select
        {
            OptionsSource = CreateSelectOptions()
        };
        using var realized = RealizeControl(select);
        Expect(FindVisualByName<LoadingOutlined>(select, "LoadingIndicator") == null,
            "Non-loading Select should not create default LoadingOutlined.",
            failures);

        SetSelectLoadingForTest(select, true);
        RefreshLayout(realized.Window);
        var loadingIcon = FindVisualByTypeName(select, "IconPresenter", "LoadingIndicator");
        Expect(loadingIcon != null,
            "Loading Select should create a loading indicator on demand.",
            failures);

        SetSelectLoadingForTest(select, false);
        RefreshLayout(realized.Window);
        Expect(FindVisualByTypeName(select, "IconPresenter", "LoadingIndicator") == null,
            "Leaving loading state should detach the loading indicator.",
            failures);
        Expect(loadingIcon?.GetVisualParent() == null,
            "Detached loading indicator should not keep a visual parent.",
            failures);
    }

    private static void VerifyTreeSelectPopupLifecycle(ICollection<string> failures)
    {
        var treeSelect = new TreeSelect
        {
            ItemsSource = CreateTreeNodes()
        };
        AtomUI.Desktop.Controls.TreeView? firstTreeView;
        using (var realized = RealizeControl(treeSelect))
        {
            Expect(GetPopupContent<AtomUI.Desktop.Controls.TreeView>(treeSelect) == null,
                "Closed TreeSelect should not create TreeSelectTreeView.",
                failures);

            MaterializeLazyPopupContentForTest(treeSelect);
            RefreshLayout(realized.Window);
            firstTreeView = GetPopupContent<AtomUI.Desktop.Controls.TreeView>(treeSelect);
            Expect(firstTreeView?.GetType().Name == "TreeSelectTreeView",
                "Materializing TreeSelect popup should lazily create TreeSelectTreeView.",
                failures);

            MaterializeLazyPopupContentForTest(treeSelect);
            RefreshLayout(realized.Window);
            Expect(ReferenceEquals(firstTreeView, GetPopupContent<AtomUI.Desktop.Controls.TreeView>(treeSelect)),
                "TreeSelect should reuse lazy TreeSelectTreeView on repeated materialization.",
                failures);
        }

        Expect(firstTreeView?.GetVisualParent() == null,
            "Detached TreeSelect should clear lazy TreeSelectTreeView visual parent.",
            failures);
    }

    private static void VerifyCascaderPopupLifecycle(ICollection<string> failures)
    {
        var cascader = new Cascader
        {
            OptionsSource = CreateCascaderOptions()
        };
        CascaderView? firstCascaderView;
        using (var realized = RealizeControl(cascader))
        {
            Expect(GetPopupContent<CascaderView>(cascader) == null,
                "Closed Cascader should not create CascaderView.",
                failures);

            MaterializeLazyPopupContentForTest(cascader);
            RefreshLayout(realized.Window);
            firstCascaderView = GetPopupContent<CascaderView>(cascader);
            Expect(firstCascaderView != null,
                "Materializing Cascader popup should lazily create CascaderView.",
                failures);

            MaterializeLazyPopupContentForTest(cascader);
            RefreshLayout(realized.Window);
            Expect(ReferenceEquals(firstCascaderView, GetPopupContent<CascaderView>(cascader)),
                "Cascader should reuse lazy CascaderView on repeated materialization.",
                failures);
        }

        Expect(firstCascaderView?.GetVisualParent() == null,
            "Detached Cascader should clear lazy CascaderView visual parent.",
            failures);
    }

    private static Border? GetPopupFrame(Control control)
    {
        var popup = FindVisualByName<Avalonia.Controls.Primitives.Popup>(control, "PART_Popup");
        return popup?.Child as Border;
    }

    private static T? GetPopupContent<T>(Control control)
        where T : Control
    {
        return GetPopupFrame(control)?.Child as T;
    }

    private static void MaterializeLazyPopupContentForTest(AbstractSelect select)
    {
        var method = select.GetType().GetMethod(
            "EnsurePopupContent",
            BindingFlags.Instance | BindingFlags.NonPublic);
        method?.Invoke(select, null);
    }

    private static void SetSelectLoadingForTest(AbstractSelect select, bool value)
    {
        typeof(AbstractSelect)
            .GetProperty(nameof(AbstractSelect.IsLoading), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            ?.SetValue(select, value);
    }
}
