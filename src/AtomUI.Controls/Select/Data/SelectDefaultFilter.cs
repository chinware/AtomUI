namespace AtomUI.Controls.Data;

internal class SelectDefaultFilter
{
    private readonly WeakReference<ISelectOptionCollectionView> CollectionViewRef;
    
    public SelectDefaultFilter(ISelectOptionCollectionView collectionView)
    {
        CollectionViewRef = new WeakReference<ISelectOptionCollectionView>(collectionView);
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
    
    public static implicit operator Func<object, bool>(SelectDefaultFilter filter)
    {
        return filter.Filter;
    }
}