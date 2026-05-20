using System.Reflection;
using AtomUI.Desktop.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunCascaderStateVerification()
    {
        var failures = new List<string>();
        VerifyCascaderPopupLifecycle(failures);
        VerifyCascaderModeSpecificContent(failures);
        VerifyCascaderViewDirectStates(failures);
        VerifyCascaderViewItemLazySlots(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Cascader state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Cascader state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
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

    private static void VerifyCascaderModeSpecificContent(ICollection<string> failures)
    {
        var defaultCascader = new Cascader
        {
            OptionsSource = CreateCascaderOptions()
        };
        using var defaultRealized = RealizeControl(defaultCascader);
        Expect(FindVisualByName<SelectFilterTextBox>(defaultCascader, "PART_SingleFilterInput") == null,
            "Closed default Cascader should not create SelectFilterTextBox.",
            failures);
        Expect(FindVisualByName<SelectTagAwareTextBox>(defaultCascader, "SelectedOptionsBox") == null,
            "Closed default single Cascader should not create SelectTagAwareTextBox.",
            failures);

        var filterCascader = new Cascader
        {
            IsFilterEnabled = true,
            OptionsSource   = CreateCascaderOptions()
        };
        using var filterRealized = RealizeControl(filterCascader);
        Expect(FindVisualByName<SelectFilterTextBox>(filterCascader, "PART_SingleFilterInput") == null,
            "Closed filter Cascader should defer SelectFilterTextBox until opened.",
            failures);
        InvokeCascaderMethod(filterCascader, "EnsureSingleFilterInput");
        RefreshLayout(filterRealized.Window);
        var openedFilterInput = FindVisualByName<SelectFilterTextBox>(filterCascader, "PART_SingleFilterInput");
        Expect(openedFilterInput != null,
            "Opening filter Cascader should create SelectFilterTextBox on demand.",
            failures);
        InvokeCascaderMethod(filterCascader, "ClearSingleFilterInput");
        RefreshLayout(filterRealized.Window);
        Expect(FindVisualByName<SelectFilterTextBox>(filterCascader, "PART_SingleFilterInput") == null,
            "Closing empty filter Cascader should detach SelectFilterTextBox.",
            failures);
        Expect(openedFilterInput?.GetVisualParent() == null,
            "Detached filter Cascader SelectFilterTextBox should not keep a visual parent.",
            failures);
        Expect(GetPopupContent<CascaderView>(filterCascader) == null,
            "Closed filter Cascader should still defer CascaderView.",
            failures);

        var multiCascader = new Cascader
        {
            IsMultiple    = true,
            OptionsSource = CreateCascaderOptions()
        };
        using var multiRealized = RealizeControl(multiCascader);
        Expect(FindVisualByName<SelectTagAwareTextBox>(multiCascader, "SelectedOptionsBox") == null,
            "Closed empty multiple Cascader should defer SelectTagAwareTextBox.",
            failures);
        InvokeCascaderMethod(multiCascader, "EnsureSelectedOptionsBox");
        RefreshLayout(multiRealized.Window);
        var openedOptionsBox = FindVisualByName<SelectTagAwareTextBox>(multiCascader, "SelectedOptionsBox");
        Expect(openedOptionsBox != null,
            "Opening multiple Cascader should create SelectTagAwareTextBox on demand.",
            failures);
        InvokeCascaderMethod(multiCascader, "ClearSelectedOptionsBox");
        RefreshLayout(multiRealized.Window);
        Expect(FindVisualByName<SelectTagAwareTextBox>(multiCascader, "SelectedOptionsBox") == null,
            "Closing empty multiple Cascader should detach SelectTagAwareTextBox.",
            failures);
        Expect(openedOptionsBox?.GetVisualParent() == null,
            "Detached multiple Cascader SelectTagAwareTextBox should not keep a visual parent.",
            failures);
        Expect(FindVisualByName<SelectFilterTextBox>(multiCascader, "PART_SingleFilterInput") == null,
            "Multiple Cascader should not create the single filter text box.",
            failures);
    }

    private static void VerifyCascaderViewDirectStates(ICollection<string> failures)
    {
        var cascaderView = new CascaderView
        {
            OptionsSource = CreateCascaderOptions()
        };
        using var realized = RealizeControl(cascaderView);
        Expect(FindVisualByTypeName(cascaderView, "CascaderViewLevelList") != null,
            "Direct CascaderView should create the root level list.",
            failures);
        Expect(FindVisualByTypeName(cascaderView, "CascaderViewFilterList") == null,
            "Direct CascaderView should defer the filter list until filtering.",
            failures);
        Expect(FindVisualByName<ContentPresenter>(cascaderView, "EmptyIndicator") == null,
            "Non-empty CascaderView should defer EmptyIndicator presenter.",
            failures);

        cascaderView.SetCurrentValue(CascaderView.FilterValueProperty, "lake");
        RefreshLayout(realized.Window);
        var filterList = FindVisualByTypeName(cascaderView, "CascaderViewFilterList");
        Expect(filterList != null,
            "Filtering CascaderView should create CascaderViewFilterList on demand.",
            failures);

        cascaderView.ClearFilter();
        RefreshLayout(realized.Window);
        Expect(FindVisualByTypeName(cascaderView, "CascaderViewFilterList") == null,
            "Clearing CascaderView filter should detach CascaderViewFilterList.",
            failures);
        Expect(filterList?.GetVisualParent() == null,
            "Detached CascaderViewFilterList should not keep a visual parent.",
            failures);

        var emptyView = new CascaderView();
        using var emptyRealized = RealizeControl(emptyView);
        var emptyIndicator = FindVisualByName<ContentPresenter>(emptyView, "EmptyIndicator");
        Expect(emptyIndicator != null,
            "Empty CascaderView should create EmptyIndicator presenter on demand.",
            failures);
        emptyView.SetCurrentValue(CascaderView.OptionsSourceProperty, CreateCascaderOptions());
        RefreshLayout(emptyRealized.Window);
        Expect(FindVisualByName<ContentPresenter>(emptyView, "EmptyIndicator") == null,
            "Non-empty CascaderView should detach EmptyIndicator presenter.",
            failures);
        Expect(emptyIndicator?.GetVisualParent() == null,
            "Detached CascaderView EmptyIndicator presenter should not keep a visual parent.",
            failures);
    }

    private static void VerifyCascaderViewItemLazySlots(ICollection<string> failures)
    {
        var defaultView = new CascaderView
        {
            OptionsSource = CreateCascaderOptions()
        };
        using var defaultRealized = RealizeControl(defaultView);
        Expect(FindVisualByName<ToggleButton>(defaultView, "ToggleCheckbox") == null,
            "Non-checkable CascaderViewItem should not create ToggleCheckbox.",
            failures);
        Expect(FindVisualByTypeName(defaultView, "IconTemplatePresenter", "LoadingIconPresenter") == null,
            "Non-loading CascaderViewItem should not create LoadingIconPresenter.",
            failures);
        var expandIconPresenter = FindVisualByTypeName(defaultView, "IconTemplatePresenter", "ExpandIconPresenter");
        Expect(expandIconPresenter != null,
            "Non-leaf CascaderViewItem should create ExpandIconPresenter.",
            failures);

        var firstItem = FindVisualByTypeName(defaultView, "CascaderViewItem") as CascaderViewItem;
        Expect(firstItem != null,
            "Direct CascaderView should create CascaderViewItem containers.",
            failures);
        if (firstItem != null)
        {
            firstItem.SetCurrentValue(CascaderViewItem.IsLoadingProperty, true);
            RefreshLayout(defaultRealized.Window);
            var loadingIcon = FindVisualByTypeName(defaultView, "IconTemplatePresenter", "LoadingIconPresenter");
            Expect(loadingIcon != null,
                "Loading CascaderViewItem should create LoadingIconPresenter on demand.",
                failures);
            Expect(expandIconPresenter?.IsVisible == false,
                "Loading CascaderViewItem should keep ExpandIconPresenter hidden while loading.",
                failures);

            firstItem.SetCurrentValue(CascaderViewItem.IsLoadingProperty, false);
            RefreshLayout(defaultRealized.Window);
            Expect(FindVisualByTypeName(defaultView, "IconTemplatePresenter", "LoadingIconPresenter") == null,
                "Leaving loading state should detach LoadingIconPresenter.",
                failures);
            Expect(loadingIcon?.GetVisualParent() == null,
                "Detached LoadingIconPresenter should not keep a visual parent.",
                failures);
            Expect(expandIconPresenter?.IsVisible == true,
                "Leaving loading state should restore ExpandIconPresenter visibility for non-leaf items.",
                failures);
        }

        var leafView = new CascaderView
        {
            OptionsSource =
            [
                new CascaderOption { Header = "Leaf", Value = "leaf" }
            ]
        };
        using var leafRealized = RealizeControl(leafView);
        var leafExpandIcon = FindVisualByTypeName(leafView, "IconTemplatePresenter", "ExpandIconPresenter");
        Expect(leafExpandIcon?.IsVisible == false,
            "Leaf CascaderViewItem should keep ExpandIconPresenter hidden.",
            failures);

        var checkableView = new CascaderView
        {
            IsCheckable   = true,
            OptionsSource = CreateCascaderOptions()
        };
        using var checkableRealized = RealizeControl(checkableView);
        var toggleCheckbox = FindVisualByName<ToggleButton>(checkableView, "ToggleCheckbox");
        Expect(toggleCheckbox != null,
            "Checkable CascaderViewItem should create ToggleCheckbox.",
            failures);
        checkableView.SetCurrentValue(CascaderView.IsCheckableProperty, false);
        RefreshLayout(checkableRealized.Window);
        Expect(FindVisualByName<ToggleButton>(checkableView, "ToggleCheckbox") == null,
            "Turning checkable off should detach CascaderViewItem ToggleCheckbox.",
            failures);
        Expect(toggleCheckbox?.GetVisualParent() == null,
            "Detached CascaderViewItem ToggleCheckbox should not keep a visual parent.",
            failures);
    }

    private static void InvokeCascaderMethod(Cascader cascader, string methodName)
    {
        typeof(Cascader)
            .GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)
            ?.Invoke(cascader, null);
    }

}
