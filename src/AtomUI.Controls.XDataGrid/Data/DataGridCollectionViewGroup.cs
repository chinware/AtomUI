// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using Avalonia.Collections;

namespace AtomUI.Controls.XDataGrid.Data;

public abstract class DataGridCollectionViewGroup : INotifyPropertyChanged
{
    private int _itemCount;

    public object Key { get; }
    public int ItemCount => _itemCount;
    public IAvaloniaReadOnlyList<object> Items => ProtectedItems;

    protected AvaloniaList<object> ProtectedItems { get; }

    protected int ProtectedItemCount
    {
        get => _itemCount;
        set
        {
            _itemCount = value;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(ItemCount)));
        }
    }

    internal abstract DataGridCollectionViewGroupInternal? Parent { get; }
    internal abstract DataGridGroupDescription? GroupBy { get; set; }

    protected DataGridCollectionViewGroup(object key)
    {
        Key            = key;
        ProtectedItems = new AvaloniaList<object>();
    }

    public abstract bool IsBottomLevel { get; }

    protected virtual event PropertyChangedEventHandler? PropertyChanged;

    event PropertyChangedEventHandler? INotifyPropertyChanged.PropertyChanged
    {
        add =>  PropertyChanged += value;

        remove => PropertyChanged -= value;
    }

    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, e);
    }
}