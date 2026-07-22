namespace AtomUI.Controls.Data;

internal sealed class ListCollectionViewNode
{
    public ListCollectionEntry? Entry { get; }
    public GroupListItemData? GroupHeader { get; }
    public bool IsGroupHeader => GroupHeader is not null;
    public object? Item => Entry is not null ? Entry.Item : GroupHeader;

    private ListCollectionViewNode(ListCollectionEntry? entry, GroupListItemData? groupHeader)
    {
        Entry       = entry;
        GroupHeader = groupHeader;
    }

    public static ListCollectionViewNode ForEntry(ListCollectionEntry entry)
        => new(entry, null);

    public static ListCollectionViewNode ForGroupHeader(GroupListItemData header)
        => new(null, header);
}
