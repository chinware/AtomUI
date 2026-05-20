using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Layout;
using Avalonia.VisualTree;
using AtomButton = AtomUI.Desktop.Controls.Button;
using AvaloniaTextBlock = Avalonia.Controls.TextBlock;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunDescriptionsStateVerification()
    {
        var failures = new List<string>();
        VerifyDescriptionsHeaderLifecycle(failures);
        VerifyDescriptionsCollectionLifecycle(failures);
        VerifyDescriptionsLayoutAndBorderedSwitch(failures);
        VerifyDescriptionsColonBinding(failures);
        VerifyDescriptionsWindowSubscriptionLifecycle(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Descriptions state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Descriptions state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyDescriptionsHeaderLifecycle(ICollection<string> failures)
    {
        var descriptions = CreateVerificationDescriptions(itemCount: 1);
        using var realized = RealizeControl(descriptions);

        Expect(FindVisualByName<DockPanel>(descriptions, "HeaderLayout") == null,
            "Descriptions without Header/Extra should not create HeaderLayout.",
            failures);

        descriptions.Header = "User Info";
        RefreshLayout(realized.Window);
        var firstHeaderLayout = FindVisualByName<DockPanel>(descriptions, "HeaderLayout");
        Expect(firstHeaderLayout != null,
            "Descriptions should create HeaderLayout when Header is assigned.",
            failures);
        Expect(FindVisualByName<ContentPresenter>(descriptions, "HeaderPresenter") != null,
            "Descriptions should create HeaderPresenter when Header is assigned.",
            failures);

        descriptions.Extra = new AtomButton { Content = "Edit" };
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<ContentPresenter>(descriptions, "ExtraPresenter")?.Content is AtomButton,
            "Descriptions should bind Extra into ExtraPresenter.",
            failures);

        descriptions.Header = null;
        descriptions.Extra  = null;
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<DockPanel>(descriptions, "HeaderLayout") == null,
            "Clearing Header/Extra should remove HeaderLayout.",
            failures);
        Expect(firstHeaderLayout == null || !firstHeaderLayout.IsAttachedToVisualTree(),
            "Removed HeaderLayout should be detached from the active visual tree.",
            failures);
    }

    private static void VerifyDescriptionsCollectionLifecycle(ICollection<string> failures)
    {
        var descriptions = CreateVerificationDescriptions(itemCount: 3);
        using var realized = RealizeControl(descriptions);

        Expect(CountDescriptionsVisualsByTypeName(descriptions, "DescriptionDefaultItem") == 3,
            "Descriptions should create one default item visual per non-bordered item.",
            failures);

        descriptions.Items.Insert(1, CreateVerificationDescriptionItem("Inserted"));
        RefreshLayout(realized.Window);
        Expect(CountDescriptionsVisualsByTypeName(descriptions, "DescriptionDefaultItem") == 4,
            "Descriptions should insert new item visuals without rebuilding duplicate items.",
            failures);

        descriptions.Items.RemoveAt(0);
        RefreshLayout(realized.Window);
        Expect(CountDescriptionsVisualsByTypeName(descriptions, "DescriptionDefaultItem") == 3,
            "Descriptions should remove item visuals by the original collection index.",
            failures);

        descriptions.Items.Clear();
        RefreshLayout(realized.Window);
        Expect(CountDescriptionsVisualsByTypeName(descriptions, "DescriptionDefaultItem") == 0,
            "Descriptions reset should clear generated item visuals.",
            failures);

        var oldItems = descriptions.Items;
        var newItems = new DescriptionItems
        {
            CreateVerificationDescriptionItem("New 1"),
            CreateVerificationDescriptionItem("New 2")
        };
        descriptions.Items = newItems;
        RefreshLayout(realized.Window);
        Expect(CountDescriptionsVisualsByTypeName(descriptions, "DescriptionDefaultItem") == 2,
            "Replacing Items should rebuild generated item visuals from the new collection.",
            failures);

        oldItems.Add(CreateVerificationDescriptionItem("Old mutation"));
        RefreshLayout(realized.Window);
        Expect(CountDescriptionsVisualsByTypeName(descriptions, "DescriptionDefaultItem") == 2,
            "Mutating a replaced Items collection should not update Descriptions.",
            failures);
    }

    private static void VerifyDescriptionsLayoutAndBorderedSwitch(ICollection<string> failures)
    {
        var descriptions = CreateVerificationDescriptions(itemCount: 2);
        using var realized = RealizeControl(descriptions);

        Expect(CountDescriptionsVisualsByTypeName(descriptions, "DescriptionDefaultItem") == 2,
            "Default horizontal Descriptions should create default item visuals.",
            failures);

        descriptions.IsBordered = true;
        RefreshLayout(realized.Window);
        Expect(CountDescriptionsVisualsByTypeName(descriptions, "DescriptionDefaultItem") == 0,
            "Horizontal bordered Descriptions should replace default item visuals.",
            failures);
        Expect(CountDescriptionsVisualsByTypeName(descriptions, "DescriptionBorderedItemLabel") == 2 &&
               CountDescriptionsVisualsByTypeName(descriptions, "DescriptionBorderedItemContent") == 2,
            "Horizontal bordered Descriptions should create one label and one content cell per item.",
            failures);

        descriptions.IsBordered = false;
        RefreshLayout(realized.Window);
        Expect(CountDescriptionsVisualsByTypeName(descriptions, "DescriptionDefaultItem") == 2,
            "Clearing IsBordered should restore default item visuals.",
            failures);
        Expect(CountDescriptionsVisualsByTypeName(descriptions, "DescriptionBorderedItemLabel") == 0 &&
               CountDescriptionsVisualsByTypeName(descriptions, "DescriptionBorderedItemContent") == 0,
            "Clearing IsBordered should remove bordered cell visuals.",
            failures);

        descriptions.Layout = Orientation.Vertical;
        descriptions.IsBordered = true;
        RefreshLayout(realized.Window);
        Expect(CountDescriptionsVisualsByTypeName(descriptions, "DescriptionDefaultItem") == 2,
            "Vertical bordered Descriptions should use default item visuals.",
            failures);
        Expect(CountDescriptionsVisualsByTypeName(descriptions, "DescriptionBorderedItemLabel") == 0 &&
               CountDescriptionsVisualsByTypeName(descriptions, "DescriptionBorderedItemContent") == 0,
            "Vertical bordered Descriptions should not keep horizontal bordered cell visuals.",
            failures);
    }

    private static void VerifyDescriptionsColonBinding(ICollection<string> failures)
    {
        var descriptions = CreateVerificationDescriptions(itemCount: 1);
        descriptions.IsShowColon = false;
        using var realized = RealizeControl(descriptions);

        var colon = FindVisualByName<AvaloniaTextBlock>(descriptions, "Colon");
        Expect(colon != null && !colon.IsVisible,
            "Descriptions IsShowColon=false should hide the generated colon.",
            failures);

        descriptions.IsShowColon = true;
        RefreshLayout(realized.Window);
        Expect(colon != null && colon.IsVisible,
            "Descriptions IsShowColon=true should update generated colon visibility.",
            failures);

        descriptions.IsShowColon = false;
        RefreshLayout(realized.Window);
        Expect(colon != null && !colon.IsVisible,
            "Descriptions IsShowColon should update repeatedly without recreating leaked bindings.",
            failures);
    }

    private static void VerifyDescriptionsWindowSubscriptionLifecycle(ICollection<string> failures)
    {
        var descriptions = CreateVerificationDescriptions(itemCount: 1);
        object? attachedWindow;
        using (RealizeControl(descriptions))
        {
            attachedWindow = GetPrivateField(descriptions, "AtomUI.Desktop.Controls.Descriptions", "_attachedWindow");
        }

        Expect(attachedWindow == null ||
               GetPrivateField(descriptions, "AtomUI.Desktop.Controls.Descriptions", "_attachedWindow") == null,
            "Detached Descriptions should clear Window.MediaBreakPointChanged subscription state.",
            failures);
    }

    private static Descriptions CreateVerificationDescriptions(int itemCount)
    {
        var descriptions = new Descriptions();
        for (var i = 0; i < itemCount; i++)
        {
            descriptions.Items.Add(CreateVerificationDescriptionItem($"Label {i + 1}"));
        }
        return descriptions;
    }

    private static DescriptionItem CreateVerificationDescriptionItem(string label)
    {
        return new DescriptionItem
        {
            Label   = label,
            Content = $"{label} content"
        };
    }

    private static int CountDescriptionsVisualsByTypeName(Control root, string typeName)
    {
        return root.GetSelfAndVisualDescendants()
                   .OfType<Control>()
                   .Count(control => control.GetType().Name == typeName);
    }
}
