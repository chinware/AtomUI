// using System.Collections;
// using System.ComponentModel;
// using System.Globalization;
// using System.Reflection;
// using AtomUI.Controls.Data;
//
// namespace AtomUI.Controls;
//
// internal class SelectOptionCollectionView : IListBoxCollectionView, IList, INotifyPropertyChanged
// {
//     #region 公共属性定义
//
//     
//
//     #endregion
//
//     #region 公共事件定义
//
//     
//
//     #endregion
//
//     #region 内部属性定义
//
//     
//
//     #endregion
//     
//     /// <summary>
//     /// CultureInfo used in this DataGridCollectionView
//     /// </summary>
//     private CultureInfo _culture;
//     
//     /// <summary>
//     /// Private accessor for the Monitor we use to prevent recursion
//     /// </summary>
//     private SimpleMonitor _currentChangedMonitor = new SimpleMonitor();
//     
//     /// <summary>
//     /// The number of requests to defer Refresh()
//     /// </summary>
//     private int _deferLevel;
//     
//     /// <summary>
//     /// Private accessor for the Filter
//     /// </summary>
//     private Func<object, bool>? _filter;
//     
//     /// <summary>
//     /// Private accessor for the CollectionViewFlags
//     /// </summary>
//     private CollectionViewFlags _flags = CollectionViewFlags.ShouldProcessCollectionChanged;
//     
//     /// <summary>
//     /// Private accessor for the Grouping data
//     /// </summary>
//     private ListBoxCollectionViewGroupRoot _group;
//     
//     /// <summary>
//     /// Private accessor for the InternalList
//     /// </summary>
//     private IList _internalList;
//     
//     /// <summary>
//     /// Keeps track of whether groups have been applied to the
//     /// collection already or not. Note that this can still be set
//     /// to false even though we specify a GroupDescription, as the 
//     /// collection may not have gone through the PrepareGroups function.
//     /// </summary>
//     private bool _isGrouping;
//     
//     /// <summary>
//     /// Private accessor for indicating whether we want to point to the temporary grouping data for calculations
//     /// </summary>
//     private bool _isUsingTemporaryGroup;
//     
//     /// <summary>
//     /// ConstructorInfo obtained from reflection for generating new items
//     /// </summary>
//     private ConstructorInfo? _itemConstructor;
//     
//     /// <summary>
//     /// Whether we have the correct ConstructorInfo information for the ItemConstructor
//     /// </summary>
//     private bool _itemConstructorIsValid;
//     
//     /// <summary>
//     /// The new item we are getting ready to add to the collection
//     /// </summary>
//     private object? _newItem;
//     
//     /// <summary>
//     /// Whether the source needs to poll for changes
//     /// (if it did not implement INotifyCollectionChanged)
//     /// </summary>
//     private bool _pollForChanges;
//     
//     /// <summary>
//     /// Private accessor for the SortDescriptions
//     /// </summary>
//     private SelectSortDescriptionCollection? _sortDescriptions;
//     
//     /// <summary>
//     /// Private accessor for the FilterDescriptions
//     /// </summary>
//     private SelectFilterDescriptionCollection? _filterDescriptions;
//     
//     [Flags]
//     private enum CollectionViewFlags
//     {
//         /// <summary>
//         /// Whether the list of items (after applying the sort and filters, if any) 
//         /// is already in the correct order for grouping. 
//         /// </summary>
//         IsDataInGroupOrder = 0x01,
//
//         /// <summary>
//         /// Whether the source collection is already sorted according to the SortDescriptions collection
//         /// </summary>
//         IsDataSorted = 0x02,
//
//         /// <summary>
//         /// Whether we should process the collection changed event
//         /// </summary>
//         ShouldProcessCollectionChanged = 0x04,
//
//         /// <summary>
//         /// Whether we need to refresh
//         /// </summary>
//         NeedsRefresh = 0x10,
//
//         /// <summary>
//         /// Whether we cache the IsEmpty value
//         /// </summary>
//         CachedIsEmpty = 0x20,
//
//         /// <summary>
//         /// Indicates whether a page index change is in process or not
//         /// </summary>
//         IsPageChanging = 0x40,
//
//         /// <summary>
//         /// Whether we need to move to another page after EndDefer
//         /// </summary>
//         IsMoveToPageDeferred = 0x100,
//
//         /// <summary>
//         /// Whether we need to update the PageSize after EndDefer
//         /// </summary>
//         IsUpdatePageSizeDeferred = 0x200
//     }
//     
//     /// <summary>
//     /// A simple monitor class to help prevent re-entrant calls
//     /// </summary>
//     private class SimpleMonitor : IDisposable
//     {
//         /// <summary>
//         /// Whether the monitor is entered
//         /// </summary>
//         private bool _entered;
//
//         /// <summary>
//         /// Gets a value indicating whether we have been entered or not
//         /// </summary>
//         public bool Busy => _entered;
//
//         /// <summary>
//         /// Sets a value indicating that we have been entered
//         /// </summary>
//         /// <returns>Boolean value indicating whether we were already entered</returns>
//         public bool Enter()
//         {
//             if (_entered)
//             {
//                 return false;
//             }
//
//             _entered = true;
//             return true;
//         }
//
//         /// <summary>
//         /// Cleanup method called when done using this class
//         /// </summary>
//         public void Dispose()
//         {
//             _entered = false;
//             GC.SuppressFinalize(this);
//         }
//     }
//     
//     private class DeferHelper : IDisposable
//     {
//         /// <summary>
//         /// Private reference to the CollectionView that created this DeferHelper
//         /// </summary>
//         private SelectOptionCollectionView? _collectionView;
//
//         /// <summary>
//         /// Initializes a new instance of the DeferHelper class
//         /// </summary>
//         /// <param name="collectionView">CollectionView that created this DeferHelper</param>
//         public DeferHelper(SelectOptionCollectionView? collectionView)
//         {
//             _collectionView = collectionView;
//         }
//
//         /// <summary>
//         /// Cleanup method called when done using this class
//         /// </summary>
//         public void Dispose()
//         {
//             if (_collectionView != null)
//             {
//                 _collectionView.EndDefer();
//                 _collectionView = null;
//             }
//
//             GC.SuppressFinalize(this);
//         }
//     }
//     
//     /// <summary>
//     /// Creates a comparer class that takes in a CultureInfo as a parameter,
//     /// which it will use when comparing strings.
//     /// </summary>
//     private class CultureSensitiveComparer : IComparer<object>
//     {
//         /// <summary>
//         /// Private accessor for the CultureInfo of our comparer
//         /// </summary>
//         private CultureInfo _culture;
//
//         /// <summary>
//         /// Creates a comparer which will respect the CultureInfo
//         /// that is passed in when comparing strings.
//         /// </summary>
//         /// <param name="culture">The CultureInfo to use in string comparisons</param>
//         public CultureSensitiveComparer(CultureInfo? culture)
//         {
//             _culture = culture ?? CultureInfo.InvariantCulture;
//         }
//
//         /// <summary>
//         /// Compares two objects and returns a value indicating whether one is less than, equal to or greater than the other.
//         /// </summary>
//         /// <param name="x">first item to compare</param>
//         /// <param name="y">second item to compare</param>
//         /// <returns>Negative number if x is less than y, zero if equal, and a positive number if x is greater than y</returns>
//         /// <remarks>
//         /// Compares the 2 items using the specified CultureInfo for string and using the default object comparer for all other objects.
//         /// </remarks>
//         public int Compare(object? x, object? y)
//         {
//             if (x == null)
//             {
//                 if (y != null)
//                 {
//                     return -1;
//                 }
//
//                 return 0;
//             }
//
//             if (y == null)
//             {
//                 return 1;
//             }
//
//             // at this point x and y are not null
//             if (x is string xString && y is string yString)
//             {
//                 return _culture.CompareInfo.Compare(xString, yString);
//             }
//             return Comparer<object>.Default.Compare(x, y);
//         }
//     }
// }