using System.Globalization;

namespace AtomUI.Controls;

internal record DataGridCellCoordinates
{
    public DataGridCellCoordinates(int columnIndex, int slot)
    {
        ColumnIndex = columnIndex;
        Slot        = slot;
    }

    public DataGridCellCoordinates(DataGridCellCoordinates dataGridCellCoordinates)
    {
        ColumnIndex = dataGridCellCoordinates.ColumnIndex;
        Slot        = dataGridCellCoordinates.Slot;
    }

    public int ColumnIndex { get; init;  }

    public int Slot { get; init; }
    
#if DEBUG
        public override string ToString()
        {
            return "DataGridCellCoordinates {ColumnIndex = " + ColumnIndex.ToString(CultureInfo.CurrentCulture) +
                   ", Slot = " + Slot.ToString(CultureInfo.CurrentCulture) + "}";
        }
#endif
}