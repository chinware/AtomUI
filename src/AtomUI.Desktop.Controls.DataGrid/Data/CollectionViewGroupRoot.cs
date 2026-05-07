// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using Avalonia.Collections;

namespace AtomUI.Desktop.Controls.Data;

internal class CollectionViewGroupRoot : DataGridCollectionViewGroupInternal, INotifyCollectionChanged
{
    /// <summary>
    /// String constant used for the Root Name
    /// </summary>
    private const string RootName = "Root";

    /// <summary>
    /// Private accessor for empty object instance
    /// </summary>
    private static readonly object UseAsItemDirectly = new object();

    /// <summary>
    /// Private accessor for the top level GroupDescription
    /// </summary>
    private static DataGridGroupDescription? _topLevelGroupDescription;

    /// <summary>
    /// Private accessor for an ObservableCollection containing group descriptions
    /// </summary>
    private readonly AvaloniaList<DataGridGroupDescription> _groupBy = new ();

    /// <summary>
    /// Indicates whether the list of items (after applying the sort and filters, if any) 
    /// is already in the correct order for grouping.
    /// </summary>
    private bool _isDataInGroupOrder;

    /// <summary>
    /// Private accessor for the owning ICollectionView
    /// </summary>
    private readonly IDataGridCollectionView _view;

    /// <summary>
    /// Raise this event when the (grouped) view changes
    /// </summary>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>
    /// Raise this event when the GroupDescriptions change
    /// </summary>
    internal event EventHandler? GroupDescriptionChanged;

    /// <summary>
    /// Initializes a new instance of the CollectionViewGroupRoot class.
    /// </summary>
    /// <param name="view">CollectionView that contains this grouping</param>
    /// <param name="isDataInGroupOrder">True if items are already in correct order for grouping</param>
    internal CollectionViewGroupRoot(IDataGridCollectionView view, bool isDataInGroupOrder)
        : base(RootName, null)
    {
        _view               = view;
        _isDataInGroupOrder = isDataInGroupOrder;
    }

    /// <summary>
    /// Gets the description of grouping, indexed by level.
    /// </summary>
    public virtual AvaloniaList<DataGridGroupDescription> GroupDescriptions => _groupBy;

    /// <summary>
    /// Gets or sets the current IComparer being used
    /// </summary>
    internal IComparer? ActiveComparer { get; set; }

    /// <summary>
    /// Gets the culture to use during sorting.
    /// </summary>
    internal CultureInfo Culture
    {
        get
        {
            Debug.Assert(_view != null, "this._view should have been set from the constructor");
            return _view.Culture;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the data is in group order
    /// </summary>
    internal bool IsDataInGroupOrder
    {
        get => _isDataInGroupOrder;
        set => _isDataInGroupOrder = value;
    }

    /// <summary>
    /// Finds the index of the specified item
    /// </summary>
    /// <param name="item">Item we are looking for</param>
    /// <param name="seed">Seed of the item we are looking for</param>
    /// <param name="comparer">Comparer used to find the item</param>
    /// <param name="low">Low range of item index</param>
    /// <param name="high">High range of item index</param>
    /// <returns>Index of the specified item</returns>
    protected override int FindIndex(object item, object seed, IComparer? comparer, int low, int high)
    {
        // root group needs to adjust the bounds of the search to exclude the new item (if any)
        if (_view is IDataGridEditableCollectionView iecv && iecv.IsAddingNew)
        {
            --high;
        }

        return base.FindIndex(item, seed, comparer, low, high);
    }

    /// <summary>
    /// Initializes the group descriptions
    /// </summary>
    internal void Initialize()
    {
        if (_topLevelGroupDescription == null)
        {
            _topLevelGroupDescription = new TopLevelGroupDescription();
        }

        InitializeGroup(this, 0, null);
    }

    /// <summary>
    /// Inserts specified item into the collection
    /// </summary>
    /// <param name="index">Index to insert into</param>
    /// <param name="item">Item to insert</param>
    /// <param name="loading">Whether we are currently loading</param>
    internal void InsertSpecialItem(int index, object item, bool loading)
    {
        ChangeCounts(item, +1);
        ProtectedItems.Insert(index, item);

        if (!loading)
        {
            int globalIndex = LeafIndexFromItem(item, index);
            HandleCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, globalIndex));
        }
    }

    /// <summary>
    /// Notify listeners that this View has changed
    /// </summary>
    /// <remarks>
    /// CollectionViews (and sub-classes) should take their filter/sort/grouping
    /// into account before calling this method to forward CollectionChanged events.
    /// </remarks>
    /// <param name="args">The NotifyCollectionChangedEventArgs to be passed to the EventHandler</param>
    public void HandleCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
        Debug.Assert(args != null, "Arguments passed in should not be null");
        CollectionChanged?.Invoke(this, args);
    }

    /// <summary>
    /// Notify host that a group description has changed somewhere in the tree
    /// </summary>
    protected override void OnGroupByChanged()
    {
        GroupDescriptionChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Remove specified item from subgroups
    /// </summary>
    /// <param name="item">Item to remove</param>
    /// <returns>Whether the operation was successful</returns>
    internal bool RemoveFromSubgroups(object item)
    {
        return RemoveFromSubgroups(item, this, 0);
    }

    /// <summary>
    /// Remove specified item from subgroups using an exhaustive search
    /// </summary>
    /// <param name="item">Item to remove</param>
    internal void RemoveItemFromSubgroupsByExhaustiveSearch(object item)
    {
        RemoveItemFromSubgroupsByExhaustiveSearch(this, item);
    }

    /// <summary>
    /// Removes specified item into the collection
    /// </summary>
    /// <param name="index">Index to remove from</param>
    /// <param name="item">Item to remove</param>
    /// <param name="loading">Whether we are currently loading</param>
    internal void RemoveSpecialItem(int index, object item, bool loading)
    {
        Debug.Assert(Object.Equals(item, ProtectedItems[index]), "RemoveSpecialItem finds inconsistent data");
        int globalIndex = -1;

        if (!loading)
        {
            globalIndex = LeafIndexFromItem(item, index);
        }

        ChangeCounts(item, -1);
        ProtectedItems.RemoveAt(index);

        if (!loading)
        {
            HandleCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, globalIndex));
        }
    }

    /// <summary>
    /// Adds specified item to subgroups
    /// </summary>
    /// <param name="item">Item to add</param>
    /// <param name="loading">Whether we are currently loading</param>
    internal void AddToSubgroups(object item, bool loading)
    {
        AddToSubgroups(item, this, 0, loading);
    }

    /// <summary>
    /// Add an item to the subgroup with the given name
    /// </summary>
    /// <param name="item">Item to add</param>
    /// <param name="group">Group to add item to</param>
    /// <param name="level">The level of grouping.</param>
    /// <param name="key">Name of subgroup to add to</param>
    /// <param name="loading">Whether we are currently loading</param>
    private void AddToSubgroup(object item, DataGridCollectionViewGroupInternal group, int level, object key,
                               bool loading)
    {
        DataGridCollectionViewGroupInternal? subgroup;
        int                                 index = (_isDataInGroupOrder) ? group.LastIndex : 0;

        // find the desired subgroup
        for (int n = group.Items.Count; index < n; ++index)
        {
            subgroup = group.Items[index] as DataGridCollectionViewGroupInternal;
            if (subgroup == null)
            {
                continue; // skip children that are not groups
            }

            if (group.GroupBy != null)
            {
                group.LastIndex = index;
                AddToSubgroups(item, subgroup, level + 1, loading);
                return;
            }
        }

        // the item didn't match any subgroups.  Create a new subgroup and add the item.
        subgroup = new DataGridCollectionViewGroupInternal(key, group);
        InitializeGroup(subgroup, level + 1, item);

        if (loading)
        {
            group.Add(subgroup);
            group.LastIndex = index;
        }
        else
        {
            // using insert will find the correct sort index to
            // place the subgroup, and will default to the last
            // position if no ActiveComparer is specified
            if (ActiveComparer != null)
            {
                group.Insert(subgroup, item, ActiveComparer);
            }
        }

        AddToSubgroups(item, subgroup, level + 1, loading);
    }

    /// <summary>
    /// Add an item to the desired subgroup(s) of the given group
    /// </summary>
    /// <param name="item">Item to add</param>
    /// <param name="group">Group to add item to</param>
    /// <param name="level">The level of grouping</param>
    /// <param name="loading">Whether we are currently loading</param>
    private void AddToSubgroups(object item, DataGridCollectionViewGroupInternal group, int level, bool loading)
    {
        object? key = GetGroupKey(item, group.GroupBy, level);

        if (key == UseAsItemDirectly)
        {
            // the item belongs to the group itself (not to any subgroups)
            if (loading)
            {
                group.Add(item);
            }
            else
            {
                if (ActiveComparer != null)
                {
                    int localIndex = group.Insert(item, item, ActiveComparer);
                    int index      = group.LeafIndexFromItem(item, localIndex);
                    HandleCollectionChanged(
                        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
                }
            }
        }
        else if (key is ICollection keyList)
        {
            // the item belongs to multiple subgroups
            foreach (object o in keyList)
            {
                AddToSubgroup(item, group, level, o, loading);
            }
        }
        else
        {
            if (key != null)
            {
                // the item belongs to one subgroup
                AddToSubgroup(item, group, level, key, loading);
            }
        }
    }

    public virtual Func<DataGridCollectionViewGroup, int, DataGridGroupDescription>? GroupBySelector { get; set; }

    /// <summary>
    /// Returns the description of how to divide the given group into subgroups
    /// </summary>
    /// <param name="group">CollectionViewGroup to get group description from</param>
    /// <param name="level">The level of grouping</param>
    /// <returns>GroupDescription of how to divide the given group</returns>
    private DataGridGroupDescription? GetGroupDescription(DataGridCollectionViewGroup group, int level)
    {
        DataGridGroupDescription?   result      = null;
        DataGridCollectionViewGroup? targetGroup = group;
        if (targetGroup == this)
        {
            targetGroup = null;
        }

        if (result == null && GroupBySelector != null && targetGroup != null)
        {
            result = GroupBySelector?.Invoke(targetGroup, level);
        }

        if (result == null && level < GroupDescriptions.Count)
        {
            result = GroupDescriptions[level];
        }

        return result;
    }

    /// <summary>
    /// Get the group name(s) for the given item
    /// </summary>
    /// <param name="item">Item to get group name for</param>
    /// <param name="groupDescription">GroupDescription for the group</param>
    /// <param name="level">The level of grouping</param>
    /// <returns>Group names for the specified item</returns>
    private object? GetGroupKey(object item, DataGridGroupDescription? groupDescription, int level)
    {
        if (groupDescription != null)
        {
            return groupDescription.GroupKeyFromItem(item, level, Culture);
        }
        return UseAsItemDirectly;
    }

    /// <summary>
    /// Initialize the given group
    /// </summary>
    /// <param name="group">Group to initialize</param>
    /// <param name="level">The level of grouping</param>
    /// <param name="seedItem">The seed item to compare with to see where to insert</param>
    private void InitializeGroup(DataGridCollectionViewGroupInternal group, int level, object? seedItem)
    {
        // set the group description for dividing the group into subgroups
        DataGridGroupDescription? groupDescription = GetGroupDescription(group, level);
        group.GroupBy = groupDescription;

        // create subgroups for each of the explicit names
        var keys = groupDescription?.GroupKeys;
        if (keys != null)
        {
            for (int k = 0, n = keys.Count; k < n; ++k)
            {
                DataGridCollectionViewGroupInternal subgroup = new DataGridCollectionViewGroupInternal(keys[k], group);
                InitializeGroup(subgroup, level + 1, seedItem);
                group.Add(subgroup);
            }
        }

        group.LastIndex = 0;
    }

    /// <summary>
    /// Remove an item from the direct children of a group.
    /// </summary>
    /// <param name="group">Group to remove item from</param>
    /// <param name="item">Item to remove</param>
    /// <returns>True if item could not be removed</returns>
    private bool RemoveFromGroupDirectly(DataGridCollectionViewGroupInternal group, object item)
    {
        int leafIndex = group.Remove(item, true);
        if (leafIndex >= 0)
        {
            HandleCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, leafIndex));
            return false;
        }
        return true;
    }

    /// <summary>
    /// Remove an item from the subgroup with the given name.
    /// </summary>
    /// <param name="item">Item to remove</param>
    /// <param name="group">Group to remove item from</param>
    /// <param name="level">The level of grouping</param>
    /// <param name="key">Name of item to remove</param>
    /// <returns>Return true if the item was not in one of the subgroups it was supposed to be.</returns>
    private bool RemoveFromSubgroup(object item, DataGridCollectionViewGroupInternal group, int level, object key)
    {
        bool                                itemIsMissing = false;
        DataGridCollectionViewGroupInternal? subGroup;

        // find the desired subgroup
        for (int index = 0, n = group.Items.Count; index < n; ++index)
        {
            subGroup = group.Items[index] as DataGridCollectionViewGroupInternal;
            if (subGroup == null)
            {
                continue; // skip children that are not groups
            }

            if (group.GroupBy != null && group.GroupBy.KeysMatch(subGroup.Key, key))
            {
                if (RemoveFromSubgroups(item, subGroup, level + 1))
                {
                    itemIsMissing = true;
                }

                return itemIsMissing;
            }
        }

        // the item didn't match any subgroups.  It should have.
        return true;
    }

    /// <summary>
    /// Remove an item from the desired subgroup(s) of the given group.
    /// </summary>
    /// <param name="item">Item to remove</param>
    /// <param name="group">Group to remove item from</param>
    /// <param name="level">The level of grouping</param>
    /// <returns>Return true if the item was not in one of the subgroups it was supposed to be.</returns>
    private bool RemoveFromSubgroups(object item, DataGridCollectionViewGroupInternal group, int level)
    {
        bool   itemIsMissing = false;
        object? key           = GetGroupKey(item, group.GroupBy, level);

        if (key == UseAsItemDirectly)
        {
            // the item belongs to the group itself (not to any subgroups)
            itemIsMissing = RemoveFromGroupDirectly(group, item);
        }
        else if (key is ICollection keyList)
        {
            // the item belongs to multiple subgroups
            foreach (object o in keyList)
            {
                if (RemoveFromSubgroup(item, group, level, o))
                {
                    itemIsMissing = true;
                }
            }
        }
        else
        {
            if (key != null)
            {
                // the item belongs to one subgroup
                if (RemoveFromSubgroup(item, group, level, key))
                {
                    itemIsMissing = true;
                }
            }
        }

        return itemIsMissing;
    }

    /// <summary>
    /// The item did not appear in one or more of the subgroups it
    /// was supposed to.  This can happen if the item's properties
    /// change so that the group names we used to insert it are
    /// different from the names used to remove it. If this happens,
    /// remove the item the hard way.
    /// </summary>
    /// <param name="group">Group to remove item from</param>
    /// <param name="item">Item to remove</param>
    private void RemoveItemFromSubgroupsByExhaustiveSearch(DataGridCollectionViewGroupInternal group, object item)
    {
        // try to remove the item from the direct children 
        // this function only returns true if it failed to remove from group directly
        // in which case we will step through and search exhaustively
        if (RemoveFromGroupDirectly(group, item))
        {
            // if that didn't work, recurse into each subgroup
            // (loop runs backwards in case an entire group is deleted)
            for (int k = group.Items.Count - 1; k >= 0; --k)
            {
                if (group.Items[k] is DataGridCollectionViewGroupInternal subgroup)
                {
                    RemoveItemFromSubgroupsByExhaustiveSearch(subgroup, item);
                }
            }
        }
    }

    /// <summary>
    /// TopLevelGroupDescription class
    /// </summary>
    private class TopLevelGroupDescription : DataGridGroupDescription
    {
        /// <summary>
        /// Initializes a new instance of the TopLevelGroupDescription class.
        /// </summary>
        public TopLevelGroupDescription()
        {
        }

        /// <summary>
        /// We have to implement this abstract method, but it should never be called
        /// </summary>
        /// <param name="item">Item to get group name from</param>
        /// <param name="level">The level of grouping</param>
        /// <param name="culture">Culture used for sorting</param>
        /// <returns>We do not return a value here</returns>
        public override object? GroupKeyFromItem(object item, int level, CultureInfo? culture)
        {
            Debug.Assert(true, "We have to implement this abstract method, but it should never be called");
            return null;
        }
    }
}