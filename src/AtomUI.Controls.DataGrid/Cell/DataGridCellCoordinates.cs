namespace AtomUI.Controls;

internal class DataGridCellCoordinates
{
    public DataGridCellCoordinates(int columnIndex, int slot)
    {
        ColumnIndex = columnIndex;
        Slot        = slot;
    }

    public DataGridCellCoordinates(DataGridCellCoordinates dataGridCellCoordinates) : this(
        dataGridCellCoordinates.ColumnIndex, dataGridCellCoordinates.Slot)
    {
    }

    public int ColumnIndex { get; set; }

    public int Slot { get; set; }

    public override bool Equals(object? o)
    {
        if (o is DataGridCellCoordinates dataGridCellCoordinates)
        {
            return dataGridCellCoordinates.ColumnIndex == ColumnIndex && dataGridCellCoordinates.Slot == Slot;
        }

        return false;
    }

#if DEBUG
        public override string ToString()
        {
            return "DataGridCellCoordinates {ColumnIndex = " + ColumnIndex.ToString(CultureInfo.CurrentCulture) +
                   ", Slot = " + Slot.ToString(CultureInfo.CurrentCulture) + "}";
        }
#endif
}