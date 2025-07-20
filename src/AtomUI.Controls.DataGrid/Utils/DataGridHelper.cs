// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using Avalonia;

namespace AtomUI.Controls.Utils;

internal static class DataGridHelper
{
    public static void SyncColumnProperty<T>(AvaloniaObject column, AvaloniaObject content, AvaloniaProperty<T> property)
    {
        SyncColumnProperty(column, content, property, property);
    }

    public static void SyncColumnProperty<T>(AvaloniaObject column, AvaloniaObject content, AvaloniaProperty<T> contentProperty, AvaloniaProperty<T> columnProperty)
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

    public static ListSortDirection FromSupportedDirections(DataGridSortDirections supportedDirections)
    {
        return supportedDirections switch
        {
            DataGridSortDirections.Ascending => ListSortDirection.Ascending,
            DataGridSortDirections.Descending => ListSortDirection.Descending,
            _ => throw new ArgumentOutOfRangeException($"unsupported SupportedDirections: {supportedDirections}."),
        };
    }

    public static DataGridSortDirections FromListSortDirection(ListSortDirection sortDirection)
    {
        return sortDirection switch
        {
            ListSortDirection.Ascending => DataGridSortDirections.Ascending,
            ListSortDirection.Descending => DataGridSortDirections.Descending,
            _ => throw new ArgumentOutOfRangeException($"unsupported ListSortDirection: {sortDirection}."),
        };
    }
    
    public static bool AreEqualAt3Decimals(double a, double b)
    {
        long scaledA = (long)Math.Round(a * 10000);
        long scaledB = (long)Math.Round(b * 10000);
        return scaledA == scaledB;
    }
    
    public static bool AreLessAt3Decimals(double a, double b)
    {
        long scaledA = (long)Math.Round(a * 10000);
        long scaledB = (long)Math.Round(b * 10000);
        return scaledA < scaledB;
    }
}