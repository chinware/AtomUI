using AtomUI.Controls.Data;

namespace AtomUI.Controls;

internal class DataGridRowGroupInfo
{
    public DataGridRowGroupInfo(
        DataGridCollectionViewGroup? collectionViewGroup,
        bool isVisible,
        int level,
        int slot,
        int lastSubItemSlot)
    {
        CollectionViewGroup = collectionViewGroup;
        IsVisible           = isVisible;
        Level               = level;
        Slot                = slot;
        LastSubItemSlot     = lastSubItemSlot;
    }

    public DataGridCollectionViewGroup? CollectionViewGroup
    {
        get;
        private set;
    }

    public int LastSubItemSlot
    {
        get;
        set;
    }

    public int Level
    {
        get;
        private set;
    }

    public int Slot
    {
        get;
        set;
    }

    public bool IsVisible
    {
        get;
        set;
    }
}