using AtomUI.Controls.Data;

namespace AtomUI.Desktop.Controls;

internal class CascaderViewFilterListItemData : ListItemData, ICascaderItemInfo
{
    public IList<ICascaderOption>? ExpandItems { get; set; }
    public string Path => Content?.ToString() ?? string.Empty;
}
