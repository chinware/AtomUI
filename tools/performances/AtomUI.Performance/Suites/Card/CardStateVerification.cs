using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunCardStateVerification()
    {
        var failures = new List<string>();
        VerifyCardUnusedSlotsAreLazy(failures);
        VerifyCardHeaderLifecycle(failures);
        VerifyCardCoverLifecycle(failures);
        VerifyCardActionLifecycle(failures);
        VerifyCardSkeletonLifecycle(failures);
        VerifyCardMotionScope(failures);
        VerifyCardGridDefinitions(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Card state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Card state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyCardUnusedSlotsAreLazy(ICollection<string> failures)
    {
        var card = CreateContentOnlyCard();
        using var _ = RealizeControl(card);

        Expect(FindVisualByName<Border>(card, "HeaderFrame") == null,
            "Content-only Card should not create HeaderFrame.",
            failures);
        Expect(FindVisualByName<ContentPresenter>(card, "HeaderExtra") == null,
            "Content-only Card should not create HeaderExtra presenter.",
            failures);
        Expect(FindVisualByName<ContentPresenter>(card, "TitlePresenter") == null,
            "Content-only Card should not create TitlePresenter.",
            failures);
        Expect(FindVisualByName<Border>(card, "CoverFrame") == null,
            "Content-only Card should not create CoverFrame.",
            failures);
        Expect(FindVisualByName<ContentPresenter>(card, "CoverContentPresenter") == null,
            "Content-only Card should not create CoverContentPresenter.",
            failures);
        Expect(FindVisualByTypeName(card, "CardActionPanel") == null,
            "Content-only Card should not create CardActionPanel.",
            failures);
        Expect(CountVisualsByTypeName(card, "Skeleton") == 0,
            "Content-only Card should not create Skeleton.",
            failures);
        Expect(FindVisualByName<ContentPresenter>(card, "PART_ContentPresenter") != null,
            "Content-only Card should create the lightweight body presenter.",
            failures);
    }

    private static void VerifyCardHeaderLifecycle(ICollection<string> failures)
    {
        var card = CreateContentOnlyCard();
        using var realized = RealizeControl(card);

        card.Header = "Card title";
        RefreshLayout(realized.Window);
        var headerFrame = FindVisualByName<Border>(card, "HeaderFrame");
        var titlePresenter = FindVisualByName<ContentPresenter>(card, "TitlePresenter");
        Expect(headerFrame != null,
            "Card should create HeaderFrame when Header is assigned.",
            failures);
        Expect(titlePresenter != null,
            "Card should create TitlePresenter when Header is assigned.",
            failures);
        Expect(headerFrame?.Padding.Left > 0,
            "Dynamically created HeaderFrame should receive Card template padding styles.",
            failures);
        Expect(FindVisualByName<ContentPresenter>(card, "HeaderExtra") == null,
            "Card without Extra should not create HeaderExtra presenter.",
            failures);

        card.SetCurrentValue(Card.ExtraProperty, CreateCardMoreLink());
        RefreshLayout(realized.Window);
        var extraPresenter = FindVisualByName<ContentPresenter>(card, "HeaderExtra");
        Expect(extraPresenter != null,
            "Card should create HeaderExtra when Extra is assigned.",
            failures);

        card.Header = null;
        card.SetCurrentValue(Card.ExtraProperty, null);
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<Border>(card, "HeaderFrame") == null,
            "Card should remove HeaderFrame when Header and Extra are cleared.",
            failures);
        Expect(headerFrame?.GetVisualParent() == null,
            "Removed HeaderFrame should not keep a visual parent.",
            failures);
        Expect(titlePresenter?.GetVisualParent() == null,
            "Removed TitlePresenter should not keep a visual parent.",
            failures);
        Expect(extraPresenter?.GetVisualParent() == null,
            "Removed HeaderExtra should not keep a visual parent.",
            failures);
        Expect(headerFrame == null || headerFrame.TemplatedParent == null,
            "Removed HeaderFrame should clear templated parent.",
            failures);
    }

    private static void VerifyCardCoverLifecycle(ICollection<string> failures)
    {
        var card = CreateContentOnlyCard();
        using var realized = RealizeControl(card);

        card.SetCurrentValue(Card.CoverProperty, CreateCardCoverImage());
        RefreshLayout(realized.Window);
        var coverFrame = FindVisualByName<Border>(card, "CoverFrame");
        var coverPresenter = FindVisualByName<ContentPresenter>(card, "CoverContentPresenter");
        Expect(coverFrame != null,
            "Card should create CoverFrame when Cover is assigned.",
            failures);
        Expect(coverPresenter != null,
            "Card should create CoverContentPresenter when Cover is assigned.",
            failures);

        card.SetCurrentValue(Card.CoverProperty, null);
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<Border>(card, "CoverFrame") == null,
            "Card should remove CoverFrame when Cover is cleared.",
            failures);
        Expect(coverFrame?.GetVisualParent() == null,
            "Removed CoverFrame should not keep a visual parent.",
            failures);
        Expect(coverPresenter?.GetVisualParent() == null,
            "Removed CoverContentPresenter should not keep a visual parent.",
            failures);
        Expect(coverPresenter == null || coverPresenter.TemplatedParent == null,
            "Removed CoverContentPresenter should clear templated parent.",
            failures);
    }

    private static void VerifyCardActionLifecycle(ICollection<string> failures)
    {
        var card = CreateContentOnlyCard();
        using var realized = RealizeControl(card);
        Expect(FindVisualByTypeName(card, "CardActionPanel") == null,
            "Card without Actions should not create CardActionPanel.",
            failures);

        var firstAction = new CardActionButton { Icon = new AtomUI.Icons.AntDesign.EditOutlined() };
        var secondAction = new CardActionButton { Icon = new AtomUI.Icons.AntDesign.SettingOutlined() };
        var thirdAction = new CardActionButton { Icon = new AtomUI.Icons.AntDesign.EllipsisOutlined() };
        card.Actions.Add(firstAction);
        card.Actions.Add(secondAction);
        card.Actions.Add(thirdAction);
        RefreshLayout(realized.Window);
        var actionPanel = FindVisualByTypeName(card, "CardActionPanel");
        Expect(actionPanel != null,
            "Card should create CardActionPanel when Actions are added.",
            failures);
        Expect(CountVisualsByTypeName(card, "CardActionButton") == 3,
            "Card should realize three CardActionButton visuals after three Actions are added.",
            failures);

        card.Actions.Clear();
        RefreshLayout(realized.Window);
        Expect(FindVisualByTypeName(card, "CardActionPanel") == null,
            "Card should remove CardActionPanel when Actions are cleared.",
            failures);
        Expect(actionPanel?.GetVisualParent() == null,
            "Removed CardActionPanel should not keep a visual parent.",
            failures);
        Expect(actionPanel == null || actionPanel.TemplatedParent == null,
            "Removed CardActionPanel should clear templated parent.",
            failures);
        Expect(firstAction.GetVisualParent() == null &&
               secondAction.GetVisualParent() == null &&
               thirdAction.GetVisualParent() == null,
            "Removed CardActionButton controls should not keep visual parents.",
            failures);

        card.Actions.Add(new CardActionButton { Icon = new AtomUI.Icons.AntDesign.EditOutlined() });
        RefreshLayout(realized.Window);
        var recreatedPanel = FindVisualByTypeName(card, "CardActionPanel");
        Expect(recreatedPanel != null,
            "Card should recreate CardActionPanel when Actions are added again.",
            failures);
        Expect(!ReferenceEquals(actionPanel, recreatedPanel),
            "Card should not reuse a removed CardActionPanel instance.",
            failures);
    }

    private static void VerifyCardSkeletonLifecycle(ICollection<string> failures)
    {
        var card = CreateLoadingCard(isLoading: false);
        using var realized = RealizeControl(card);
        var contentPresenter = FindVisualByName<ContentPresenter>(card, "PART_ContentPresenter");
        Expect(contentPresenter != null,
            "Non-loading Card should use the lightweight content presenter.",
            failures);
        Expect(CountVisualsByTypeName(card, "Skeleton") == 0,
            "Non-loading Card should not create Skeleton.",
            failures);

        card.SetCurrentValue(Card.IsLoadingProperty, true);
        RefreshLayout(realized.Window);
        var skeleton = FindVisualByTypeName(card, "Skeleton");
        Expect(skeleton != null,
            "Card should create Skeleton when IsLoading becomes true.",
            failures);
        Expect(FindVisualByName<ContentPresenter>(card, "PART_ContentPresenter") == null,
            "Loading Card should remove the lightweight content presenter.",
            failures);
        Expect(contentPresenter?.GetVisualParent() == null,
            "Removed lightweight content presenter should not keep a visual parent.",
            failures);
        Expect(contentPresenter == null || contentPresenter.TemplatedParent == null,
            "Removed lightweight content presenter should clear templated parent.",
            failures);

        card.SetCurrentValue(Card.IsLoadingProperty, false);
        RefreshLayout(realized.Window);
        Expect(CountVisualsByTypeName(card, "Skeleton") == 0,
            "Card should remove Skeleton when IsLoading becomes false.",
            failures);
        Expect(skeleton?.GetVisualParent() == null,
            "Removed Skeleton should not keep a visual parent.",
            failures);
        Expect(skeleton == null || skeleton.TemplatedParent == null,
            "Removed Skeleton should clear templated parent.",
            failures);
        Expect(FindVisualByName<ContentPresenter>(card, "PART_ContentPresenter") != null,
            "Card should restore lightweight content presenter after loading ends.",
            failures);
    }

    private static void VerifyCardMotionScope(ICollection<string> failures)
    {
        var defaultCard = CreateContentOnlyCard();
        using var _ = RealizeControl(defaultCard);
        Expect(defaultCard.Transitions?.Any(transition => transition.GetType().Name == "BoxShadowsTransition") != true,
            "Non-hoverable Card should not create BoxShadow transitions.",
            failures);

        var hoverableCard = CreateContentOnlyCard();
        hoverableCard.IsHoverable = true;
        using var __ = RealizeControl(hoverableCard);
        Expect(hoverableCard.Transitions?.Any(transition => transition.GetType().Name == "BoxShadowsTransition") == true,
            "Hoverable Card should keep BoxShadow transition.",
            failures);
    }

    private static void VerifyCardGridDefinitions(ICollection<string> failures)
    {
        var gridContent = new CardGridContent
        {
            ColumnDefinitions = CreateEqualColumns(4),
            RowDefinitions    = CreateAutoRows(2)
        };
        using var realized = RealizeControl(gridContent);
        var gridPanel = gridContent.GetSelfAndVisualDescendants().OfType<Grid>().FirstOrDefault();
        Expect(gridPanel?.ColumnDefinitions.Count == 4,
            "CardGridContent should apply initial ColumnDefinitions through the styled property.",
            failures);
        Expect(gridPanel?.RowDefinitions.Count == 2,
            "CardGridContent should apply initial RowDefinitions through the styled property.",
            failures);

        gridContent.SetValue(CardGridContent.ColumnDefinitionsProperty, CreateEqualColumns(2));
        gridContent.SetValue(CardGridContent.RowDefinitionsProperty, CreateAutoRows(3));
        RefreshLayout(realized.Window);
        Expect(gridPanel?.ColumnDefinitions.Count == 2,
            "CardGridContent should sync runtime ColumnDefinitions changes.",
            failures);
        Expect(gridPanel?.RowDefinitions.Count == 3,
            "CardGridContent should sync runtime RowDefinitions changes.",
            failures);
    }
}
