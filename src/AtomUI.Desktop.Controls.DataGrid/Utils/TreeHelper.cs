// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Avalonia;
using Avalonia.Input;

namespace AtomUI.Desktop.Controls.Utils;

internal static class TreeHelper
{
    /// <summary>
    /// Walks the visual tree to determine if the currently focused element is contained within
    /// a parent AvaloniaObject.  The FocusManager's Current property is used to determine
    /// the currently focused element, which is updated synchronously.
    /// </summary>
    /// <param name="element">Parent Visual</param>
    /// <returns>True if the currently focused element is within the visual tree of the parent</returns>
    internal static bool ContainsFocusedElement(this Visual? element)
    {
        return element is InputElement { IsKeyboardFocusWithin: true };
    }
}