using System.Reflection;
using AtomUI.Controls;
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
        VerifySelectDropDownEventCounts(failures);
        VerifySelectDefaultValues(failures);
        VerifySelectModeSpecificContent(failures);
        VerifySelectAccessoryPaths(failures);
        VerifySelectLoadingLifecycle(failures);
        VerifySelectCandidateKeyboardHighlight(failures);
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
        Expect(FindVisualByTypeName(select, "SelectAccessoryHost") == null,
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

    private static void VerifySelectDropDownEventCounts(ICollection<string> failures)
    {
        var select = new Select
        {
            OptionsSource = CreateSelectOptions()
        };
        var openedCount = 0;
        var closedCount = 0;
        select.DropDownOpened += (_, _) => openedCount++;
        select.DropDownClosed += (_, _) => closedCount++;

        InvokeAbstractSelectMethod(select, "OpenDropDown");
        Expect(openedCount == 0,
            $"OpenDropDown should not raise DropDownOpened before the Popup.Opened callback. Actual: {openedCount}.",
            failures);

        InvokeAbstractSelectMethod(select, "NotifyPopupOpened");
        Expect(openedCount == 1,
            $"Popup.Opened should raise DropDownOpened once. Actual: {openedCount}.",
            failures);

        InvokeAbstractSelectMethod(select, "CloseDropDown");
        Expect(closedCount == 0,
            $"CloseDropDown should not raise DropDownClosed before the Popup.Closed callback. Actual: {closedCount}.",
            failures);

        InvokeAbstractSelectMethod(select, "NotifyPopupClosed");
        Expect(closedCount == 1,
            $"Popup.Closed should raise DropDownClosed once. Actual: {closedCount}.",
            failures);
    }

    private static void VerifySelectDefaultValues(ICollection<string> failures)
    {
        var singleOptions = CreateSelectOptions();
        var singleSelect = new Select
        {
            DefaultValues = new List<object> { "orange" },
            OptionsSource = singleOptions
        };
        using (RealizeControl(singleSelect))
        {
            Expect(ReferenceEquals(singleSelect.SelectedOption, singleOptions[1]),
                "Single Select should apply the first matching default value.",
                failures);
        }

        var explicitSelected = new Select
        {
            DefaultValues  = new List<object> { "orange" },
            OptionsSource  = singleOptions,
            SelectedOption = singleOptions[0]
        };
        using (RealizeControl(explicitSelected))
        {
            Expect(ReferenceEquals(explicitSelected.SelectedOption, singleOptions[0]),
                "Single Select should not overwrite an explicit selected option with DefaultValues.",
                failures);
        }

        var multipleOptions = CreateSelectOptions();
        var multipleSelect = new Select
        {
            Mode          = SelectMode.Multiple,
            DefaultValues = new List<object> { "orange", "grape" },
            OptionsSource = multipleOptions
        };
        using (RealizeControl(multipleSelect))
        {
            Expect(multipleSelect.SelectedOptions?.Count == 2 &&
                   ReferenceEquals(multipleSelect.SelectedOptions[0], multipleOptions[1]) &&
                   ReferenceEquals(multipleSelect.SelectedOptions[1], multipleOptions[4]),
                "Multiple Select should apply all matching default values in order.",
                failures);
        }
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
        var loadingIcon = select.SuffixLoadingIcon;
        var indicator   = FindVisualByName<IconPresenter>(select, "OpenIndicator");
        Expect(indicator != null && !ReferenceEquals(indicator.Icon, loadingIcon),
            "Non-loading Select should not attach the suffix loading icon.",
            failures);

        SetSelectLoadingForTest(select, true);
        RefreshLayout(realized.Window);
        indicator = FindVisualByName<IconPresenter>(select, "OpenIndicator");
        Expect(indicator != null && ReferenceEquals(indicator.Icon, loadingIcon) && indicator.IsVisible,
            "Loading Select should show the suffix loading icon in the shared SelectHandle indicator.",
            failures);

        SetSelectLoadingForTest(select, false);
        RefreshLayout(realized.Window);
        indicator = FindVisualByName<IconPresenter>(select, "OpenIndicator");
        Expect(indicator != null && !ReferenceEquals(indicator.Icon, loadingIcon),
            "Leaving loading state should restore the non-loading SelectHandle indicator.",
            failures);
        Expect(loadingIcon?.GetVisualParent() == null,
            "Detached suffix loading icon should not keep a visual parent.",
            failures);
    }

    private static void VerifySelectCandidateKeyboardHighlight(ICollection<string> failures)
    {
        var candidateList = new SelectCandidateList
        {
            ItemsSource   = CreateSelectOptions(),
            SelectionMode = SelectionMode.Single
        };

        using var realized = RealizeControl(candidateList);
        RefreshLayout(realized.Window);

        var firstItem = candidateList.ContainerFromIndex(0) as SelectCandidateListItem;
        var secondItem = candidateList.ContainerFromIndex(1) as SelectCandidateListItem;
        if (firstItem == null || secondItem == null)
        {
            failures.Add("Select candidate keyboard highlight should realize the first two options.");
            return;
        }

        candidateList.CandidateSelectedIndex = 0;
        RefreshLayout(realized.Window);
        Expect(firstItem.IsCandidateSelected && !secondItem.IsCandidateSelected,
            "Selecting the first candidate should highlight only the first option.",
            failures);

        candidateList.CandidateSelectedIndex = 1;
        RefreshLayout(realized.Window);
        Expect(!firstItem.IsCandidateSelected && secondItem.IsCandidateSelected,
            "Moving candidate highlight should clear the old option and highlight the new option.",
            failures);

        candidateList.CandidateSelectedIndex = -1;
        RefreshLayout(realized.Window);
        Expect(!firstItem.IsCandidateSelected && !secondItem.IsCandidateSelected,
            "Clearing candidate highlight should leave no visible option highlighted.",
            failures);

        candidateList.CandidateSelectedItem = candidateList.Items[0];
        RefreshLayout(realized.Window);
        Expect(firstItem.IsCandidateSelected && !secondItem.IsCandidateSelected,
            "Selecting a candidate by item should highlight only that option.",
            failures);

        candidateList.CandidateSelectedItem = candidateList.Items[1];
        RefreshLayout(realized.Window);
        Expect(!firstItem.IsCandidateSelected && secondItem.IsCandidateSelected,
            "Changing candidate item directly should clear the old option and highlight the new option.",
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

    private static void InvokeAbstractSelectMethod(AbstractSelect select, string methodName)
    {
        typeof(AbstractSelect)
            .GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)
            ?.Invoke(select, null);
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
