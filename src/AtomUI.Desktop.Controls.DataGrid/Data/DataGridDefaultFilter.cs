namespace AtomUI.Desktop.Controls.Data;

public class DataGridDefaultFilter
{
    private readonly WeakReference<IDataGridCollectionView> CollectionViewRef;

    public DataGridDefaultFilter(IDataGridCollectionView  collectionView)
    {
        CollectionViewRef = new WeakReference<IDataGridCollectionView>(collectionView);
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
    
    public static implicit operator Func<object, bool>(DataGridDefaultFilter filter)
    {
        return filter.Filter;
    }
}