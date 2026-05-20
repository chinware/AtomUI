using System.Reflection;
using AtomUI.Desktop.Controls;
using AtomUI.Desktop.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunMentionsStateVerification()
    {
        var failures = new List<string>();
        VerifyClosedMentionsCost(failures);
        VerifyMentionsPopupLifecycle(failures);
        VerifyMentionsDropDownEvents(failures);
        VerifyMentionsLoadingIndicator(failures);
        VerifyMentionsDetachCleanup(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Mentions state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Mentions state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyClosedMentionsCost(ICollection<string> failures)
    {
        var mentions = new Mentions
        {
            OptionsSource = CreateMentionOptions()
        };

        using var realized = RealizeControl(mentions);
        Expect(GetMentionsCandidateListField(mentions) == null,
            "Closed Mentions should keep _candidateList null.",
            failures);
        Expect(GetPopupFrame(mentions) == null,
            "Closed Mentions should keep Popup child empty before first open.",
            failures);
        Expect(GetPrivateField(mentions, "AtomUI.Desktop.Controls.Mentions", "_items") == null,
            "Closed Mentions should not copy OptionsSource into _items before first populate.",
            failures);
    }

    private static void VerifyMentionsPopupLifecycle(ICollection<string> failures)
    {
        var mentions = new Mentions
        {
            OptionsSource = CreateMentionOptions()
        };
        CandidateList? firstCandidateList;

        using (var realized = RealizeControl(mentions))
        {
            MaterializeMentionsPopupContentForTest(mentions);
            RefreshLayout(realized.Window);

            firstCandidateList = GetMentionsCandidateListField(mentions);
            Expect(firstCandidateList != null,
                "Materializing Mentions popup should lazily create CandidateList.",
                failures);
            Expect(GetMentionsPopupContentPanel(mentions)?.Children.Contains(firstCandidateList!) == true,
                "Materialized Mentions CandidateList should be attached under PopupFrame content panel.",
                failures);

            MaterializeMentionsPopupContentForTest(mentions);
            RefreshLayout(realized.Window);
            Expect(ReferenceEquals(firstCandidateList, GetMentionsCandidateListField(mentions)),
                "Second Mentions popup materialization should reuse CandidateList.",
                failures);
        }

        Expect(firstCandidateList?.GetVisualParent() == null,
            "Detached Mentions should clear lazy CandidateList visual parent.",
            failures);
        Expect(GetMentionsCandidateListField(mentions) == null,
            "Detached Mentions should clear _candidateList.",
            failures);
        Expect(GetPrivateField(mentions, "AtomUI.Desktop.Controls.Mentions", "_popupFrame") == null,
            "Detached Mentions should clear _popupFrame.",
            failures);
    }

    private static void VerifyMentionsDropDownEvents(ICollection<string> failures)
    {
        var mentions = new Mentions
        {
            OptionsSource = CreateMentionOptions()
        };
        var openedCount = 0;
        var closedCount = 0;
        mentions.DropDownOpened += (_, _) => openedCount++;
        mentions.DropDownClosed += (_, _) => closedCount++;

        using var realized = RealizeControl(mentions);
        MaterializeMentionsPopupContentForTest(mentions);
        InvokePrivateMethod(mentions, "AtomUI.Desktop.Controls.Mentions", "HandlePopupOpened", null, EventArgs.Empty);
        RefreshLayout(realized.Window);
        SetPrivateField(mentions, "AtomUI.Desktop.Controls.Mentions", "_popupHasOpened", true);
        InvokePrivateMethod(mentions, "AtomUI.Desktop.Controls.Mentions", "HandlePopupClosed", null, EventArgs.Empty);
        RefreshLayout(realized.Window);

        Expect(openedCount == 1,
            $"Mentions should raise DropDownOpened once per open, actual {openedCount}.",
            failures);
        Expect(closedCount == 1,
            $"Mentions should raise DropDownClosed once per close, actual {closedCount}.",
            failures);
    }

    private static void VerifyMentionsLoadingIndicator(ICollection<string> failures)
    {
        var mentions = new Mentions
        {
            OptionsSource = CreateMentionOptions()
        };

        using var realized = RealizeControl(mentions);
        MaterializeMentionsPopupContentForTest(mentions);
        RefreshLayout(realized.Window);
        Expect(GetPrivateField(mentions, "AtomUI.Desktop.Controls.Mentions", "_loadingIndicator") == null,
            "Mentions should not create LoadingIndicator while IsLoading is false.",
            failures);

        SetMentionsLoadingForTest(mentions, true);
        RefreshLayout(realized.Window);
        Expect(GetPrivateField(mentions, "AtomUI.Desktop.Controls.Mentions", "_loadingIndicator") != null,
            "Mentions should create LoadingIndicator while IsLoading is true.",
            failures);
        Expect(GetMentionsCandidateListField(mentions) is { IsVisible: false },
            "Mentions CandidateList should be hidden while loading.",
            failures);

        SetMentionsLoadingForTest(mentions, false);
        RefreshLayout(realized.Window);
        Expect(GetPrivateField(mentions, "AtomUI.Desktop.Controls.Mentions", "_loadingIndicator") == null,
            "Mentions should remove LoadingIndicator after loading ends.",
            failures);
        Expect(GetMentionsCandidateListField(mentions) is { IsVisible: true },
            "Mentions CandidateList should be visible after loading ends.",
            failures);
    }

    private static void VerifyMentionsDetachCleanup(ICollection<string> failures)
    {
        var mentions = new Mentions
        {
            AsyncLoadDebounce = TimeSpan.FromMilliseconds(50),
            OptionsSource     = CreateMentionOptions()
        };

        using (var realized = RealizeControl(mentions))
        {
            MaterializeMentionsPopupContentForTest(mentions);
            InvokePrivateMethod(mentions, "AtomUI.Desktop.Controls.Mentions", "HandlePopupOpened", null, EventArgs.Empty);
            RefreshLayout(realized.Window);
            Expect(GetPrivateField(mentions, "AtomUI.Desktop.Controls.Mentions", "_delayTimer") != null,
                "Mentions with AsyncLoadDebounce should create a debounce timer while attached.",
                failures);
            Expect(GetPrivateField(mentions, "AtomUI.Desktop.Controls.Mentions", "_subscriptionsOnOpen") != null,
                "Open Mentions should create open-state subscriptions.",
                failures);
        }

        Expect(GetPrivateField(mentions, "AtomUI.Desktop.Controls.Mentions", "_delayTimer") == null,
            "Detached Mentions should stop and clear debounce timer.",
            failures);
        Expect(GetPrivateField(mentions, "AtomUI.Desktop.Controls.Mentions", "_subscriptionsOnOpen") == null,
            "Detached Mentions should dispose open-state subscriptions.",
            failures);
        Expect(GetPrivateField(mentions, "AtomUI.Desktop.Controls.Mentions", "_attachedWindow") == null,
            "Detached Mentions should unsubscribe Window.Deactivated.",
            failures);
        Expect(GetMentionsCandidateListField(mentions) == null,
            "Detached Mentions should clear lazy popup content.",
            failures);
    }

    private static CandidateList? GetMentionsCandidateListField(Mentions mentions)
    {
        return GetPrivateField(mentions, "AtomUI.Desktop.Controls.Mentions", "_candidateList") as CandidateList;
    }

    private static Panel? GetMentionsPopupContentPanel(Mentions mentions)
    {
        return GetPopupFrame(mentions)?.Child as Panel;
    }

    private static void SetMentionsLoadingForTest(Mentions mentions, bool value)
    {
        typeof(Mentions)
            .GetProperty(nameof(Mentions.IsLoading), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            ?.SetValue(mentions, value);
    }
}
