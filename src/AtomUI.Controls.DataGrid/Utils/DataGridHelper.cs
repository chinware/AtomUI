// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Avalonia;

namespace AtomUI.Controls.Utils;

internal static class DataGridHelper
{
    internal static void SyncColumnProperty<T>(AvaloniaObject column, AvaloniaObject content, AvaloniaProperty<T> property)
    {
        SyncColumnProperty(column, content, property, property);
    }

    internal static void SyncColumnProperty<T>(AvaloniaObject column, AvaloniaObject content, AvaloniaProperty<T> contentProperty, AvaloniaProperty<T> columnProperty)
    {
        if (!column.IsSet(columnProperty))
        {
            content.ClearValue(contentProperty);
        }
        else
        {
            content.SetValue(contentProperty, column.GetValue(columnProperty));
        }
    }
}