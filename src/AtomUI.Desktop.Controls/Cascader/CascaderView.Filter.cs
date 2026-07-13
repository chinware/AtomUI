using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

public partial class CascaderView
{
    internal static readonly DirectProperty<CascaderView, bool> IsFilteringProperty =
        AvaloniaProperty.RegisterDirect<CascaderView, bool>(nameof(IsFiltering),
            o => o.IsFiltering,
            (o, v) => o.IsFiltering = v);
    
    internal static readonly DirectProperty<CascaderView, List<CascaderViewFilterListItemData>?> FilteredPathInfosProperty =
        AvaloniaProperty.RegisterDirect<CascaderView, List<CascaderViewFilterListItemData>?>(nameof(FilteredPathInfos),
            o => o.FilteredPathInfos,
            (o, v) => o.FilteredPathInfos = v);
    
    private bool _isFiltering;

    internal bool IsFiltering
    {
        get => _isFiltering;
        set => SetAndRaise(IsFilteringProperty, ref _isFiltering, value);
    }
    
    private List<CascaderViewFilterListItemData>? _filteredPathInfos;

    internal List<CascaderViewFilterListItemData>? FilteredPathInfos
    {
        get => _filteredPathInfos;
        set => SetAndRaise(FilteredPathInfosProperty, ref _filteredPathInfos, value);
    }
    
    private List<CascaderViewFilterListItemData>? _allPathInfos;
    private CascaderViewFilterList? _filterList;
    
    public void FilterItems()
    {
        if (Filter != null && FilterValue != null && IsLoaded)
        {
            if (_allPathInfos == null)
            {
                var result = new List<CascaderViewFilterListItemData>(_options.Count);
                foreach (var item in _options)
                {
                    if (item is ICascaderOption viewOption)
                    {
                        CollectionPaths(viewOption, result);
                    }
                }
                result.Sort((lhs, rhs) =>
                {
                    var lhsStr = lhs.Content?.ToString() ?? string.Empty;
                    var rhsStr = rhs.Content?.ToString() ?? string.Empty;
                    return string.Compare(lhsStr, rhsStr, StringComparison.OrdinalIgnoreCase);
                });
                _allPathInfos = result;
            }
            
            var selector = FilterValueSelector ?? DefaultCascaderFilterValueSelector;
            var filteredPathInfos = new List<CascaderViewFilterListItemData>(_allPathInfos.Count);
            foreach (var pathInfo in _allPathInfos)
            {
                if (Filter.Filter(selector(pathInfo), FilterValue))
                {
                    filteredPathInfos.Add(pathInfo);
                }
            }
            FilteredPathInfos = filteredPathInfos;
            _filterList?.ClearCandidate();
            IsFiltering       = true;
            FilterResultCount = FilteredPathInfos.Count;
        }
        else
        {
            ClearFilter();
        }
    }

    private CascaderViewFilterListItemData GetFullPath(ICascaderOption option)
    {
        var pathDepth   = CountPathDepth(option);
        var pathHeaders = new string[pathDepth];
        var current     = option;
        var pathNodes   = new ICascaderOption[pathDepth];
        for (var i = pathDepth - 1; current != null; i--)
        {
            pathNodes[i]   = current;
            pathHeaders[i] = current.Header?.ToString() ?? string.Empty;
            current = current.ParentNode as ICascaderOption;
        }

        return new CascaderViewFilterListItemData()
        {
            Content     = string.Join('/', pathHeaders),
            ExpandItems = pathNodes,
            IsEnabled   = option.IsEnabled
        };
    }

    private static int CountPathDepth(ICascaderOption option)
    {
        var count   = 0;
        var current = option;
        while (current != null)
        {
            count++;
            current = current.ParentNode as ICascaderOption;
        }

        return count;
    }
    
    private void CollectionPaths(ICascaderOption option, List<CascaderViewFilterListItemData> result)
    {
        Debug.Assert(Filter != null);
        foreach (var childItem in option.Children)
        {
            CollectionPaths(childItem, result);
        }

        if (!option.HasChildren())
        {
            result.Add(GetFullPath(option));
        }
    }
    
    public void ClearFilter()
    {
        IsFiltering       = false;
        FilterResultCount = 0;
        SetCurrentValue(FilterValueProperty, null);
        FilteredPathInfos = null;
        _allPathInfos     = null;
        _filterList?.ClearCandidate();
    }

    internal bool TryMoveFilterCandidate(int delta)
    {
        if (!IsFiltering || _filterList == null)
        {
            return false;
        }

        return _filterList.TryMoveCandidate(delta);
    }

    internal bool TryCommitFilterCandidate()
    {
        if (!IsFiltering || _filterList == null)
        {
            return false;
        }

        var itemData = _filterList.GetCandidateOrFirstEnabledItem();
        return itemData != null && TrySelectFilterResult(itemData);
    }

    private void HandleFilterListSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_filterList?.SelectedItem is CascaderViewFilterListItemData itemData)
        {
            TrySelectFilterResult(itemData);
        }
    }

    private bool TrySelectFilterResult(CascaderViewFilterListItemData itemData)
    {
        var paths = itemData.ExpandItems;
        if (paths?.Count > 0)
        {
            var targetNode = paths[^1];
            if (!IsCheckable)
            {
                ClearFilter();
                SelectOptionFromInteraction(targetNode);
                return true;
            }
        }

        return false;
    }
}
