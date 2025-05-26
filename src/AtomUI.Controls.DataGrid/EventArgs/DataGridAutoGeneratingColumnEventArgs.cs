using System.ComponentModel;

namespace AtomUI.Controls;

/// <summary>
/// Provides data for the <see cref="E:AtomUI.Controls.DataGrid.AutoGeneratingColumn" /> event. 
/// </summary>
public class DataGridAutoGeneratingColumnEventArgs : CancelEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="T:AtomUI.Controls.DataGridAutoGeneratingColumnEventArgs" /> class.
    /// </summary>
    /// <param name="propertyName">
    /// The name of the property bound to the generated column.
    /// </param>
    /// <param name="propertyType">
    /// The <see cref="T:System.Type" /> of the property bound to the generated column.
    /// </param>
    /// <param name="column">
    /// The generated column.
    /// </param>
    public DataGridAutoGeneratingColumnEventArgs(string propertyName, Type propertyType, DataGridColumn column)
    {
        Column       = column;
        PropertyName = propertyName;
        PropertyType = propertyType;
    }

    /// <summary>
    /// Gets the generated column.
    /// </summary>
    public DataGridColumn Column { get; set; }

    /// <summary>
    /// Gets the name of the property bound to the generated column.
    /// </summary>
    public string PropertyName { get; private set; }

    /// <summary>
    /// Gets the <see cref="T:System.Type" /> of the property bound to the generated column.
    /// </summary>
    public Type PropertyType { get; private set; }
}