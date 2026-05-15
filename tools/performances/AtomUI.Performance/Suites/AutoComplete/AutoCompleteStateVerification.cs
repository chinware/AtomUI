using System.Reflection;
using AtomUI.Desktop.Controls;
using AtomUI.Desktop.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunAutoCompleteStateVerification()
    {
        var failures = new List<string>();
        VerifyClosedAutoCompleteCost(failures);
        VerifyAutoCompletePopupLifecycle(failures);
        VerifyAutoCompleteDropDownClosedEvent(failures);
        VerifyAutoCompleteDetachCleanup(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("AutoComplete state verification passed.");
            return true;
        }

        Console.Error.WriteLine("AutoComplete state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyClosedAutoCompleteCost(ICollection<string> failures)
    {
        VerifyClosedAutoCompleteCost(new AutoComplete
        {
            OptionsSource = CreateAutoCompleteOptions()
        }, "AutoComplete", failures);

        VerifyClosedAutoCompleteCost(new AutoCompleteSearchEdit
        {
            OptionsSource = CreateAutoCompleteOptions()
        }, "AutoCompleteSearchEdit", failures);

        VerifyClosedAutoCompleteCost(new AutoCompleteTextArea
        {
            OptionsSource = CreateAutoCompleteOptions()
        }, "AutoCompleteTextArea", failures);
    }

    private static void VerifyClosedAutoCompleteCost(AbstractAutoComplete autoComplete,
                                                     string label,
                                                     ICollection<string> failures)
    {
        using var realized = RealizeControl(autoComplete);

        Expect(GetAutoCompleteCandidateListField(autoComplete) == null,
            $"Closed {label} should keep _candidateList null.",
            failures);
        Expect(FindVisualByName<CandidateList>(autoComplete, "PART_CandidateList") == null,
            $"Closed {label} should not create CandidateList visuals.",
            failures);
        Expect(GetPopupFrame(autoComplete) == null,
            $"Closed {label} should keep Popup child empty before first open.",
            failures);
    }

    private static void VerifyAutoCompletePopupLifecycle(ICollection<string> failures)
    {
        var autoComplete = new AutoComplete
        {
            OptionsSource = CreateAutoCompleteOptions()
        };
        CandidateList? firstCandidateList;
        using (var realized = RealizeControl(autoComplete))
        {
            MaterializeLazyPopupContentForTest(autoComplete);
            RefreshLayout(realized.Window);
            firstCandidateList = GetPopupContent<CandidateList>(autoComplete);
            Expect(firstCandidateList != null,
                "Materializing AutoComplete popup should lazily create CandidateList.",
                failures);
            Expect(ReferenceEquals(firstCandidateList, GetAutoCompleteCandidateListField(autoComplete)),
                "Materialized AutoComplete CandidateList should be stored in _candidateList.",
                failures);

            MaterializeLazyPopupContentForTest(autoComplete);
            RefreshLayout(realized.Window);
            Expect(ReferenceEquals(firstCandidateList, GetPopupContent<CandidateList>(autoComplete)),
                "Second AutoComplete popup materialization should reuse CandidateList.",
                failures);
        }

        Expect(firstCandidateList?.GetVisualParent() == null,
            "Detached AutoComplete should clear lazy CandidateList visual parent.",
            failures);
        Expect(GetAutoCompleteCandidateListField(autoComplete) == null,
            "Detached AutoComplete should clear _candidateList.",
            failures);
    }

    private static void VerifyAutoCompleteDropDownClosedEvent(ICollection<string> failures)
    {
        var autoComplete = new AutoComplete
        {
            OptionsSource = CreateAutoCompleteOptions()
        };
        var closedCount = 0;
        autoComplete.DropDownClosed += (_, _) => closedCount++;

        using var realized = RealizeControl(autoComplete);
        MaterializeLazyPopupContentForTest(autoComplete);
        SetPrivateField(autoComplete, "AtomUI.Desktop.Controls.AbstractAutoComplete", "_popupHasOpened", true);
        InvokePrivateMethod(autoComplete, "AtomUI.Desktop.Controls.AbstractAutoComplete", "HandlePopupClosed", null, EventArgs.Empty);
        RefreshLayout(realized.Window);
        Expect(closedCount == 1,
            $"AutoComplete popup closed callback should raise DropDownClosed once, actual {closedCount}.",
            failures);
    }

    private static void VerifyAutoCompleteDetachCleanup(ICollection<string> failures)
    {
        var autoComplete = new AutoComplete
        {
            AsyncLoadDebounce = TimeSpan.FromMilliseconds(50),
            OptionsSource     = CreateAutoCompleteOptions()
        };

        using (var realized = RealizeControl(autoComplete))
        {
            MaterializeLazyPopupContentForTest(autoComplete);
            RefreshLayout(realized.Window);
            Expect(GetPrivateField(autoComplete, "AtomUI.Desktop.Controls.AbstractAutoComplete", "_delayTimer") != null,
                "AutoComplete with AsyncLoadDebounce should create a debounce timer while attached.",
                failures);
        }

        Expect(GetPrivateField(autoComplete, "AtomUI.Desktop.Controls.AbstractAutoComplete", "_delayTimer") == null,
            "Detached AutoComplete should stop and clear debounce timer.",
            failures);
        Expect(GetAutoCompleteCandidateListField(autoComplete) == null,
            "Detached AutoComplete should clear lazy popup content.",
            failures);
        Expect(GetPrivateField(autoComplete, "AtomUI.Desktop.Controls.AbstractAutoComplete", "_textInputBoxSubscriptions") == null,
            "Detached AutoComplete should dispose text input subscription.",
            failures);
    }

    private static void MaterializeLazyPopupContentForTest(AbstractAutoComplete autoComplete)
    {
        var method = autoComplete.GetType().GetMethod(
            "EnsurePopupContent",
            BindingFlags.Instance | BindingFlags.NonPublic);
        method?.Invoke(autoComplete, null);
    }

    private static object? GetAutoCompleteCandidateListField(AbstractAutoComplete autoComplete)
    {
        return GetPrivateField(autoComplete, "AtomUI.Desktop.Controls.AbstractAutoComplete", "_candidateList");
    }

    private static object? GetPrivateField(object target, string declaringTypeName, string fieldName)
    {
        var type = target.GetType();
        while (type is not null)
        {
            if (type.FullName == declaringTypeName)
            {
                return type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(target);
            }

            type = type.BaseType;
        }

        return null;
    }

    private static void SetPrivateField(object target, string declaringTypeName, string fieldName, object? value)
    {
        var type = target.GetType();
        while (type is not null)
        {
            if (type.FullName == declaringTypeName)
            {
                type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(target, value);
                return;
            }

            type = type.BaseType;
        }
    }

    private static void InvokePrivateMethod(object target,
                                            string declaringTypeName,
                                            string methodName,
                                            params object?[] args)
    {
        var type = target.GetType();
        while (type is not null)
        {
            if (type.FullName == declaringTypeName)
            {
                type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(target, args);
                return;
            }

            type = type.BaseType;
        }
    }
}
