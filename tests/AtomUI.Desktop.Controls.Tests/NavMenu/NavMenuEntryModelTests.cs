using System.Collections.Specialized;
using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection;
using AtomUI.Controls;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.NavMenu;

public class NavMenuEntryModelTests
{
    [Fact]
    public void Children_Compatibility_View_Is_Created_Only_When_First_Accessed()
    {
        var node = new NavMenuNode();
        var field = typeof(NavMenuNode).GetField("_children", BindingFlags.Instance | BindingFlags.NonPublic);
        field.ShouldNotBeNull();

        field.GetValue(node).ShouldBeNull();
        _ = node.Entries;
        field.GetValue(node).ShouldBeNull();

        _ = node.Children;
        field.GetValue(node).ShouldNotBeNull();
    }

    [Fact]
    public void NavMenuNode_Entries_Are_Canonical_And_Children_View_Is_Group_Transparent()
    {
        var directChild = new NavMenuNode { Header = "Direct" };
        var groupedChild = new NavMenuNode { Header = "Grouped" };
        var nestedGroup = new NavMenuGroup();
        nestedGroup.Entries.Add(groupedChild);
        var group = new NavMenuGroup();
        group.Entries.Add(new NavMenuDivider());
        group.Entries.Add(nestedGroup);

        var owner = new NavMenuNode { Header = "Owner" };
        owner.Entries.Add(directChild);
        owner.Entries.Add(new NavMenuDivider());
        owner.Entries.Add(group);

        owner.Entries.ShouldBe([directChild, owner.Entries[1], group]);
        owner.Children.ShouldBe([directChild, groupedChild]);
        directChild.ParentNode.ShouldBeSameAs(owner);
        groupedChild.ParentNode.ShouldBeSameAs(owner);
        ((INavMenuNode)owner).Entries.ShouldBeSameAs(owner.Entries);
    }

    [Fact]
    public void Children_View_Writes_Through_To_The_Targets_Actual_Entry_Owner()
    {
        var groupedChild = new NavMenuNode { Header = "Grouped" };
        var group = new NavMenuGroup();
        group.Entries.Add(groupedChild);
        var owner = new NavMenuNode();
        owner.Entries.Add(new NavMenuDivider());
        owner.Entries.Add(group);

        var inserted = new NavMenuNode { Header = "Inserted" };
        owner.Children.Insert(0, inserted);

        group.Entries.ShouldBe([inserted, groupedChild]);
        inserted.ParentNode.ShouldBeSameAs(owner);

        var replacement = new NavMenuNode { Header = "Replacement" };
        owner.Children[1] = replacement;

        group.Entries.ShouldBe([inserted, replacement]);
        groupedChild.ParentNode.ShouldBeNull();
        replacement.ParentNode.ShouldBeSameAs(owner);

        var appended = new NavMenuNode { Header = "Appended" };
        owner.Children.Add(appended);
        owner.Entries[^1].ShouldBeSameAs(appended);

        owner.Children.Remove(inserted).ShouldBeTrue();
        group.Entries.ShouldBe([replacement]);

        owner.Children.Clear();
        owner.Entries.ShouldBeEmpty();
        replacement.ParentNode.ShouldBeNull();
        appended.ParentNode.ShouldBeNull();
    }

    [Fact]
    public void Children_View_Forwards_Pure_Node_Changes_And_Resets_For_Structural_Changes()
    {
        var owner = new NavMenuNode();
        var changes = new List<NotifyCollectionChangedEventArgs>();
        var notifier = owner.Children.ShouldBeAssignableTo<INotifyCollectionChanged>();
        notifier.CollectionChanged += (_, args) => changes.Add(args);

        var direct = new NavMenuNode();
        owner.Entries.Add(direct);

        changes.Count.ShouldBe(1);
        changes[0].Action.ShouldBe(NotifyCollectionChangedAction.Add);
        changes[0].NewItems!.Cast<INavMenuNode>().ShouldBe([direct]);
        changes.Clear();

        var group = new NavMenuGroup();
        owner.Entries.Add(group);

        changes.Single().Action.ShouldBe(NotifyCollectionChangedAction.Reset);
        changes.Clear();

        var grouped = new NavMenuNode();
        group.Entries.Add(grouped);

        changes.Single().Action.ShouldBe(NotifyCollectionChangedAction.Reset);
        owner.Children.ShouldBe([direct, grouped]);
    }

    [Fact]
    public void Entry_Collections_Reject_Indirect_Cycles_Before_Mutating_The_Graph()
    {
        var outer = new NavMenuGroup { Header = "Outer" };
        var inner = new NavMenuGroup { Header = "Inner" };
        outer.Entries.Add(inner);

        var exception = Should.Throw<InvalidOperationException>(() => inner.Entries.Add(outer));

        exception.Message.ShouldContain(nameof(NavMenuGroup));
        inner.Entries.ShouldBeEmpty();
        outer.Entries.ShouldBe([inner]);
    }

    [Fact]
    public void Entry_Collections_Reject_A_NavMenuNode_Instance_That_Is_Already_Attached()
    {
        var child = new NavMenuNode();
        var firstOwner = new NavMenuNode();
        var secondOwner = new NavMenuGroup();
        firstOwner.Entries.Add(child);

        var sameOwnerException = Should.Throw<InvalidOperationException>(() => firstOwner.Entries.Add(child));
        var otherOwnerException = Should.Throw<InvalidOperationException>(() => secondOwner.Entries.Add(child));

        sameOwnerException.Message.ShouldContain(nameof(NavMenuNode));
        otherOwnerException.Message.ShouldContain(nameof(NavMenuNode));
        firstOwner.Entries.ShouldBe([child]);
        secondOwner.Entries.ShouldBeEmpty();
    }

    [Fact]
    public void Entry_Collections_Reject_A_NavMenuGroup_Instance_That_Is_Already_Attached()
    {
        var group = new NavMenuGroup();
        var firstOwner = new NavMenuNode();
        var secondOwner = new NavMenuNode();
        firstOwner.Entries.Add(group);

        var exception = Should.Throw<InvalidOperationException>(() => secondOwner.Entries.Add(group));

        exception.Message.ShouldContain(nameof(NavMenuGroup));
        firstOwner.Entries.ShouldBe([group]);
        secondOwner.Entries.ShouldBeEmpty();
    }

    [Fact]
    public void Stateful_Entries_Can_Be_Attached_To_A_New_Owner_After_Removal()
    {
        var child = new NavMenuNode();
        var firstOwner = new NavMenuNode();
        var secondOwner = new NavMenuGroup();
        firstOwner.Entries.Add(child);

        firstOwner.Entries.Remove(child).ShouldBeTrue();
        secondOwner.Entries.Add(child);

        firstOwner.Entries.ShouldBeEmpty();
        secondOwner.Entries.ShouldBe([child]);
    }

    [Fact]
    public void NavMenuDivider_Instance_Can_Be_Reused_Because_It_Has_No_Owned_State()
    {
        var divider = new NavMenuDivider();
        var firstOwner = new NavMenuNode();
        var secondOwner = new NavMenuGroup();

        firstOwner.Entries.Add(divider);
        firstOwner.Entries.Add(divider);
        secondOwner.Entries.Add(divider);

        firstOwner.Entries.ShouldBe([divider, divider]);
        secondOwner.Entries.ShouldBe([divider]);
    }

    [Fact]
    public void AddRange_Rejects_An_Owned_Entry_Without_Partially_Mutating_The_Target()
    {
        var alreadyOwned = new NavMenuNode();
        var existingOwner = new NavMenuNode();
        existingOwner.Entries.Add(alreadyOwned);
        var fresh = new NavMenuNode();
        var target = new NavMenuNode();
        var targetEntries = target.Entries.ShouldBeOfType<NavMenuEntryCollection>();

        Should.Throw<InvalidOperationException>(() => targetEntries.AddRange([fresh, alreadyOwned]));

        target.Entries.ShouldBeEmpty();
        var newOwner = new NavMenuGroup();
        newOwner.Entries.Add(fresh);
        newOwner.Entries.ShouldBe([fresh]);
    }

    [Fact]
    public void AddRange_Commits_The_Whole_Batch_Before_Notifying_Observers()
    {
        var first = new NavMenuNode();
        var second = new NavMenuNode();
        var target = new NavMenuNode();
        var targetEntries = target.Entries.ShouldBeOfType<NavMenuEntryCollection>();
        var competingOwner = new NavMenuGroup();
        InvalidOperationException? competingAttachException = null;
        targetEntries.CollectionChanged += (_, _) =>
        {
            if (competingAttachException is null)
            {
                competingAttachException = Should.Throw<InvalidOperationException>(() =>
                    competingOwner.Entries.Add(second));
            }
        };

        targetEntries.AddRange([first, second]);

        target.Entries.ShouldBe([first, second]);
        competingOwner.Entries.ShouldBeEmpty();
        competingAttachException.ShouldNotBeNull();
    }

    [Fact]
    public void AddRange_Commits_Ownership_Before_Custom_Parent_Callbacks()
    {
        var second = new NavMenuNode();
        var competingOwner = new NavMenuGroup();
        InvalidOperationException? competingAttachException = null;
        var first = new ReentrantLegacyNavMenuNode(parentNode =>
        {
            if (parentNode is not null)
            {
                competingAttachException = Should.Throw<InvalidOperationException>(() =>
                    competingOwner.Entries.Add(second));
            }
        });
        var target = new NavMenuNode();
        var targetEntries = target.Entries.ShouldBeOfType<NavMenuEntryCollection>();

        targetEntries.AddRange([first, second]);

        target.Entries.ShouldBe([first, second]);
        competingOwner.Entries.ShouldBeEmpty();
        competingAttachException.ShouldNotBeNull();
    }

    [Fact]
    public void Add_Commits_Custom_Descendant_Ownership_Before_Parent_Callbacks()
    {
        var descendant = new NavMenuNode();
        var competingOwner = new NavMenuGroup();
        InvalidOperationException? competingAttachException = null;
        var wrapper = new ReentrantLegacyNavMenuNode(parentNode =>
        {
            if (parentNode is not null)
            {
                competingAttachException = Should.Throw<InvalidOperationException>(() =>
                    competingOwner.Entries.Add(descendant));
            }
        });
        wrapper.MutableChildren.Add(descendant);
        var target = new NavMenuNode();

        target.Entries.Add(wrapper);

        target.Entries.ShouldBe([wrapper]);
        competingOwner.Entries.ShouldBeEmpty();
        competingAttachException.ShouldNotBeNull();
    }

    [Fact]
    public void BuiltIn_Owners_Do_Not_Subscribe_To_Descendant_BuiltIn_Entry_Collections()
    {
        const int depth = 20;
        var nodes = new List<NavMenuNode> { new() };
        for (var index = 1; index < depth; index++)
        {
            var child = new NavMenuNode();
            nodes[^1].Entries.Add(child);
            nodes.Add(child);
        }

        nodes.Sum(node => GetNestedSourceSubscriptionCount(node.Entries)).ShouldBe(0);
    }

    [Fact]
    public void Entry_Collection_Rejects_Custom_Wrappers_Sharing_A_BuiltIn_Descendant_Before_Root_Attachment()
    {
        var shared = new NavMenuNode();
        var firstWrapper = new ObservableLegacyNavMenuNode();
        var secondWrapper = new ObservableLegacyNavMenuNode();
        firstWrapper.MutableChildren.Add(shared);
        secondWrapper.MutableChildren.Add(shared);
        var owner = new NavMenuNode();
        owner.Entries.Add(firstWrapper);

        var exception = Should.Throw<InvalidOperationException>(() => owner.Entries.Add(secondWrapper));

        exception.Message.ShouldContain(nameof(NavMenuNode));
        owner.Entries.ShouldBe([firstWrapper]);
    }

    [Fact]
    public void Offline_Custom_Source_Acquires_And_Releases_BuiltIn_Descendant_Ownership()
    {
        var shared = new NavMenuNode();
        var wrapper = new ObservableLegacyNavMenuNode();
        var firstOwner = new NavMenuNode();
        var secondOwner = new NavMenuGroup();
        firstOwner.Entries.Add(wrapper);

        wrapper.MutableChildren.Add(shared);
        Should.Throw<InvalidOperationException>(() => secondOwner.Entries.Add(shared));

        wrapper.MutableChildren.Remove(shared);
        secondOwner.Entries.Add(shared);

        secondOwner.Entries.ShouldBe([shared]);
    }

    [Fact]
    public void Observable_Custom_Source_Rejects_A_Dynamic_Cycle_Back_To_Its_BuiltIn_Owner()
    {
        var owner = new NavMenuNode();
        var wrapper = new ObservableLegacyNavMenuNode();
        owner.Entries.Add(wrapper);

        var exception = Should.Throw<InvalidOperationException>(() => wrapper.MutableChildren.Add(owner));

        exception.Message.ShouldContain("cycle");
        owner.TryGetStructuralOwner(out _).ShouldBeFalse();
    }

    [Fact]
    public void Observable_Custom_Source_Rejects_A_Dynamic_Cycle_Back_To_A_BuiltIn_Ancestor()
    {
        var root = new NavMenuNode();
        var child = new NavMenuNode();
        var wrapper = new ObservableLegacyNavMenuNode();
        root.Entries.Add(child);
        child.Entries.Add(wrapper);

        var exception = Should.Throw<InvalidOperationException>(() => wrapper.MutableChildren.Add(root));

        exception.Message.ShouldContain("cycle");
        root.TryGetStructuralOwner(out _).ShouldBeFalse();
    }

    [Fact]
    public void Legacy_Custom_Node_Uses_Children_As_The_Default_Entry_Source()
    {
        var child = new LegacyNavMenuNode();
        var owner = new LegacyNavMenuNode();
        owner.MutableChildren.Add(child);

        ((INavMenuNode)owner).Entries.ShouldBe([child]);
    }

    [Fact]
    public void BuiltIn_Entry_And_Children_Collections_Satisfy_Avalonia12_ItemsSource_Contracts()
    {
        var node = new NavMenuNode();

        node.Entries.ShouldBeAssignableTo<IList>();
        node.Children.ShouldBeAssignableTo<IList>();
        node.Entries.ShouldBeAssignableTo<INotifyCollectionChanged>();
        node.Children.ShouldBeAssignableTo<INotifyCollectionChanged>();
    }

    private static int GetNestedSourceSubscriptionCount(IList<INavMenuEntry> entries)
    {
        var coordinatorField = typeof(NavMenuEntryCollection).GetField(
            "_ownershipCoordinator",
            BindingFlags.Instance | BindingFlags.NonPublic);
        coordinatorField.ShouldNotBeNull();
        var coordinator = coordinatorField!.GetValue(entries);
        coordinator.ShouldNotBeNull();

        var subscriptionsField = typeof(NavMenuEntryOwnershipCoordinator).GetField(
            "_sourceSubscriptions",
            BindingFlags.Instance | BindingFlags.NonPublic);
        subscriptionsField.ShouldNotBeNull();
        var subscriptions = subscriptionsField!.GetValue(coordinator).ShouldBeAssignableTo<IDictionary>();
        return subscriptions!.Count;
    }

    private sealed class LegacyNavMenuNode : INavMenuNode
    {
        public object? Header => null;
        public IDataTemplate? HeaderTemplate => null;
        public EntityKey? ItemKey => null;
        public PathIcon? Icon => null;
        public bool IsEnabled => true;
        public ITreeNode<INavMenuNode>? ParentNode { get; private set; }
        public List<INavMenuNode> MutableChildren { get; } = [];
        public IEnumerable<INavMenuNode> Children => MutableChildren;

        public void UpdateParentNode(INavMenuNode? parentNode)
        {
            ParentNode = parentNode;
        }
    }

    private sealed class ObservableLegacyNavMenuNode : INavMenuNode
    {
        public object? Header => null;
        public IDataTemplate? HeaderTemplate => null;
        public EntityKey? ItemKey => null;
        public PathIcon? Icon => null;
        public bool IsEnabled => true;
        public ITreeNode<INavMenuNode>? ParentNode { get; private set; }
        public ObservableCollection<INavMenuNode> MutableChildren { get; } = [];
        public IEnumerable<INavMenuNode> Children => MutableChildren;

        public void UpdateParentNode(INavMenuNode? parentNode)
        {
            ParentNode = parentNode;
        }
    }

    private sealed class ReentrantLegacyNavMenuNode(Action<INavMenuNode?> parentNodeChanged) : INavMenuNode
    {
        public object? Header => null;
        public IDataTemplate? HeaderTemplate => null;
        public EntityKey? ItemKey => null;
        public PathIcon? Icon => null;
        public bool IsEnabled => true;
        public ITreeNode<INavMenuNode>? ParentNode { get; private set; }
        public ObservableCollection<INavMenuNode> MutableChildren { get; } = [];
        public IEnumerable<INavMenuNode> Children => MutableChildren;

        public void UpdateParentNode(INavMenuNode? parentNode)
        {
            ParentNode = parentNode;
            parentNodeChanged(parentNode);
        }
    }
}
