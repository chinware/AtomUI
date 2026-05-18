using System.Reflection;
using AtomUI.Controls.Utils;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
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
        VerifySelectAccessoryPaths(failures);
        VerifySelectLoadingLifecycle(failures);
        VerifyFilteredSelectClickUsesVisibleOption(failures);
        VerifyFilteredMultiSelectClickUsesVisibleOption(failures);
        VerifySelectMultiCandidateClicksAccumulate(failures);
        VerifyTreeSelectPopupLifecycle(failures);

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

    private static void VerifyFilteredSelectClickUsesVisibleOption(ICollection<string> failures)
    {
        var options = CreatePersonSelectOptions();
        var candidateList = new SelectCandidateList
        {
            Filter              = ValueFilterFactory.BuildFilter(ValueFilterMode.Contains),
            FilterValue         = "Tom",
            FilterValueSelector = Select.HeaderFilterPropertySelector,
            ItemsSource         = options,
            SelectionMode       = SelectionMode.Single
        };

        using var realized = RealizeControl(candidateList);
        RefreshLayout(realized.Window);

        var visibleItem = candidateList.ContainerFromIndex(0) as SelectCandidateListItem;
        if (visibleItem == null)
        {
            failures.Add("Filtered Select should realize the first visible candidate item.");
            return;
        }

        RaisePrimaryPointerPressed(visibleItem, realized.Window);
        visibleItem.RaiseEvent(new RoutedEventArgs(ListViewItem.ClickedEvent, visibleItem));

        Expect(ReferenceEquals(candidateList.SelectedItem, options[2]),
            $"Filtered Select candidate click should select visible option Tom. Actual: {(candidateList.SelectedItem as ISelectOption)?.Header}.",
            failures);
    }

    private static void VerifyFilteredMultiSelectClickUsesVisibleOption(ICollection<string> failures)
    {
        var options = CreatePersonSelectOptions();
        var candidateList = new SelectCandidateList
        {
            Filter              = ValueFilterFactory.BuildFilter(ValueFilterMode.Contains),
            FilterValue         = "Tom",
            FilterValueSelector = Select.HeaderFilterPropertySelector,
            ItemsSource         = options,
            SelectionMode       = SelectionMode.Multiple
        };

        using var realized = RealizeControl(candidateList);
        RefreshLayout(realized.Window);

        var visibleItem = candidateList.ContainerFromIndex(0) as SelectCandidateListItem;
        if (visibleItem == null)
        {
            failures.Add("Filtered multi Select should realize the first visible candidate item.");
            return;
        }

        RaisePrimaryPointerPressed(visibleItem, realized.Window);

        Expect(candidateList.SelectedItems?.Contains(options[2]) == true &&
               candidateList.SelectedItems?.Contains(options[0]) != true,
            "Filtered multi Select click should toggle visible option Tom, not source option Jack.",
            failures);

        candidateList.FilterValue = null;
        RefreshLayout(realized.Window);

        var secondVisibleItem = candidateList.ContainerFromIndex(1) as SelectCandidateListItem;
        if (secondVisibleItem == null)
        {
            failures.Add("Multi Select should realize the second visible candidate item.");
            return;
        }

        RaisePrimaryPointerPressed(secondVisibleItem, realized.Window);

        Expect(candidateList.SelectedItems?.Contains(options[1]) == true &&
               candidateList.SelectedItems?.Contains(options[2]) == true,
            "Multi Select should preserve existing selected option when clicking another visible option.",
            failures);
    }

    private static void VerifySelectMultiCandidateClicksAccumulate(ICollection<string> failures)
    {
        var options = CreatePersonSelectOptions();
        var select = new Select
        {
            Mode            = SelectMode.Multiple,
            IsFilterEnabled = true,
            OptionsSource   = options
        };

        using var selectRealized = RealizeControl(select);
        MaterializeLazyPopupContentForTest(select);
        var candidateList = GetPopupContent<SelectCandidateList>(select);
        if (candidateList == null)
        {
            failures.Add("Multiple Select should materialize SelectCandidateList for accumulation verification.");
            return;
        }

        var popupFrame = GetPopupFrame(select);
        if (popupFrame != null)
        {
            popupFrame.Child = null;
        }

        SetCandidateListActivatedForTest(select, true);
        using var candidateRealized = RealizeControl(candidateList);
        RefreshLayout(candidateRealized.Window);

        var firstItem = candidateList.ContainerFromIndex(0) as SelectCandidateListItem;
        var secondItem = candidateList.ContainerFromIndex(1) as SelectCandidateListItem;
        if (firstItem == null || secondItem == null)
        {
            failures.Add("Multiple Select should realize first two candidate items.");
            return;
        }

        RaisePrimaryPointerPressed(firstItem, candidateRealized.Window);
        RaisePrimaryPointerPressed(secondItem, candidateRealized.Window);

        Expect(select.SelectedOptions?.Contains(options[0]) == true &&
               select.SelectedOptions?.Contains(options[1]) == true,
            $"Multiple Select should accumulate clicked options. Mode: {candidateList.SelectionMode}, activated: {GetCandidateListActivatedForTest(select)}, select count: {select.SelectedOptions?.Count ?? 0}, candidate count: {candidateList.SelectedItems?.Count ?? 0}.",
            failures);
    }

    private static List<SelectOption> CreatePersonSelectOptions()
    {
        return
        [
            new SelectOption { Header = "Jack", Content = "jack" },
            new SelectOption { Header = "Lucy", Content = "lucy" },
            new SelectOption { Header = "Tom", Content = "tom" }
        ];
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

    private static void SetCandidateListActivatedForTest(Select select, bool value)
    {
        typeof(Select)
            .GetField("_candidateListActivated", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.SetValue(select, value);
    }

    private static bool GetCandidateListActivatedForTest(Select select)
    {
        return (bool)(typeof(Select)
                   .GetField("_candidateListActivated", BindingFlags.Instance | BindingFlags.NonPublic)
                   ?.GetValue(select) ?? false);
    }

    private static void RaisePrimaryPointerPressed(Control target, Visual root)
    {
        var pointer = new Avalonia.Input.Pointer(
            Avalonia.Input.Pointer.GetNextFreeId(),
            PointerType.Mouse,
            true);
        var properties = new PointerPointProperties(
            RawInputModifiers.LeftMouseButton,
            PointerUpdateKind.LeftButtonPressed);

        target.RaiseEvent(new PointerPressedEventArgs(
            target,
            pointer,
            root,
            default,
            1,
            properties,
            KeyModifiers.None));
    }
}
