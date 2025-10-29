namespace AtomUI.Controls.Data;

internal class ListBoxDefaultFilter
{
    private readonly WeakReference<IListBoxCollectionView> CollectionViewRef;
    
    public ListBoxDefaultFilter(IListBoxCollectionView collectionView)
    {
        CollectionViewRef = new WeakReference<IListBoxCollectionView>(collectionView);
    }
    
    public bool Filter(object value)
    {
        if (CollectionViewRef.TryGetTarget(out var collectionView))
        {
            var filterDescriptions = collectionView.FilterDescriptions;
            if (filterDescriptions == null || filterDescriptions.Count == 0)
            {
                return true;
            }
            
            foreach (var filterDescription in filterDescriptions)
            {
                if (!filterDescription.FilterBy(value))
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }
    
    public static implicit operator Func<object, bool>(ListBoxDefaultFilter filter)
    {
        return filter.Filter;
    }
}