// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Controls.Utils;

public class DataGridFrozenGrid : Grid
{
    public static readonly StyledProperty<bool> IsFrozenProperty =
        AvaloniaProperty.RegisterAttached<DataGridFrozenGrid, Control, bool>("IsFrozen");

    /// <summary>
    /// Gets a value that indicates whether the grid is frozen.
    /// </summary>
    /// <param name="element">
    /// The object to get the <see cref="P:AtomUI.Controls.Primitives.DataGridFrozenGrid.IsFrozen" /> value from.
    /// </param>
    /// <returns>true if the grid is frozen; otherwise, false. The default is true.</returns>
    public static bool GetIsFrozen(Control element)
    {
        return element.GetValue(IsFrozenProperty);
    }

    /// <summary>
    /// Sets a value that indicates whether the grid is frozen.
    /// </summary>
    /// <param name="element">The object to set the <see cref="P:AtomUI.Controls.Primitives.DataGridFrozenGrid.IsFrozen" /> value on.</param>
    /// <param name="value">true if <paramref name="element" /> is frozen; otherwise, false.</param>
    /// <exception cref="T:System.ArgumentNullException"><paramref name="element" /> is null.</exception>
    public static void SetIsFrozen(Control element, bool value)
    {
        element.SetValue(IsFrozenProperty, value);
    }
}