// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics;
using Avalonia;

namespace AtomUI.Controls;

internal partial class DataGridColumnHeader
{
    internal static readonly DirectProperty<DataGridColumnHeader, bool> CanUserFilterProperty =
        AvaloniaProperty.RegisterDirect<DataGridColumnHeader, bool>(
            nameof(CanUserFilter),
            o => o.CanUserFilter,
            (o, v) => o.CanUserFilter = v);
    
    private bool _canUserFilter = false;
    internal bool CanUserFilter
    {
        get => _canUserFilter;
        set => SetAndRaise(CanUserFilterProperty, ref _canUserFilter, value);
    }
    
    internal void InvokeProcessSort()
    {
        Debug.Assert(OwningGrid != null);
        if (OwningGrid.WaitForLostFocus(InvokeProcessSort))
        {
            return;
        }
        if (OwningGrid.CommitEdit(DataGridEditingUnit.Row, exitEditingMode: true))
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(ProcessSort);
        }
    }

    internal void ProcessSort()
    {
        
    }
}