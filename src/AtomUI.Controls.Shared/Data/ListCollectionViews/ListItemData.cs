namespace AtomUI.Controls.Data;

public class ListItemData : IListItemData
{
    public bool IsEnabled { get; set; } = true;
    public object? Content { get; set; }
    public EntityKey? ItemKey { get; init; }
    public string? Group { get; init; }
}

public class GroupListItemData : ListItemData, IGroupListItemData
{
    public bool IsGroupItem { get; set; }
}
