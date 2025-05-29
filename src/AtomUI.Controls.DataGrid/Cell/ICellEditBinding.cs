// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace AtomUI.Controls;

public interface ICellEditBinding
{
    bool IsValid { get; }
    IEnumerable<Exception> ValidationErrors { get; }
    IObservable<bool> ValidationChanged { get; }
    bool CommitEdit();
}