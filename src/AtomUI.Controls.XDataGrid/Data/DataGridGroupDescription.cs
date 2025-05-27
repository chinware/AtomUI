using System.ComponentModel;
using System.Globalization;
using Avalonia.Collections;

namespace AtomUI.Controls.XDataGrid.Data;

public abstract class DataGridGroupDescription : INotifyPropertyChanged
{
    public AvaloniaList<object> GroupKeys { get; }

    public DataGridGroupDescription()
    {
        GroupKeys = new AvaloniaList<object>();
        GroupKeys.CollectionChanged += (sender, e) => OnPropertyChanged(new PropertyChangedEventArgs(nameof(GroupKeys)));
    }

    protected virtual event PropertyChangedEventHandler? PropertyChanged;
    event PropertyChangedEventHandler? INotifyPropertyChanged.PropertyChanged
    {
        add => PropertyChanged += value;
        remove => PropertyChanged -= value;
    }
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, e);
    }

    public virtual string PropertyName => string.Empty;
    public abstract object? GroupKeyFromItem(object item, int level, CultureInfo culture);
    public virtual bool KeysMatch(object groupKey, object itemKey)
    {
        return Equals(groupKey, itemKey);
    }
}