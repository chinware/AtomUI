using System.ComponentModel;
using Avalonia.Collections;

namespace AtomUI.Controls.Data;

public abstract class ListBoxCollectionViewGroup : INotifyPropertyChanged
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
    
    internal abstract ListBoxCollectionViewGroupInternal? Parent { get; }
    internal abstract ListBoxGroupDescription? GroupBy { get; set; }

    protected ListBoxCollectionViewGroup(object key)
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