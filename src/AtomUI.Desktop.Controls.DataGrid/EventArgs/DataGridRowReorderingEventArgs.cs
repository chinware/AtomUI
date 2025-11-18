using System.ComponentModel;

namespace AtomUI.Desktop.Controls;

public class DataGridRowReorderingEventArgs : CancelEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="T:AtomUI.Desktop.Controls.DataGridRowReorderingEventArgs" /> class.
    /// </summary>
    /// <param name="dataGridRow"></param>
    public DataGridRowReorderingEventArgs(DataGridRow dataGridRow)
    {
        Row = dataGridRow;
    }

    /// <summary>
    /// The row being moved.
    /// </summary>
    public DataGridRow Row { get; private set; }
}