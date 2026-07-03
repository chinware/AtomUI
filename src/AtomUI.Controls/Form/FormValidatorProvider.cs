using System.Collections;
using Avalonia.Collections;
using Avalonia.Markup.Xaml;
using Avalonia.Metadata;

namespace AtomUI.Controls;

public class FormValidatorProvider : MarkupExtension, IList<IFormValidator>
{
    private readonly AvaloniaList<IFormValidator> _items = new()
    {
        ResetBehavior = ResetBehavior.Remove
    };

    [Content]
    public IList Items => _items;

    public int Count => _items.Count;
    public bool IsReadOnly => false;

    public IFormValidator this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }

    public void Add(IFormValidator item)
    {
        _items.Add(item);
    }

    public void Clear()
    {
        _items.Clear();
    }

    public bool Contains(IFormValidator item)
    {
        return _items.Contains(item);
    }

    public void CopyTo(IFormValidator[] array, int arrayIndex)
    {
        _items.CopyTo(array, arrayIndex);
    }

    public IEnumerator<IFormValidator> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    public int IndexOf(IFormValidator item)
    {
        return _items.IndexOf(item);
    }

    public void Insert(int index, IFormValidator item)
    {
        _items.Insert(index, item);
    }

    public bool Remove(IFormValidator item)
    {
        return _items.Remove(item);
    }

    public void RemoveAt(int index)
    {
        _items.RemoveAt(index);
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return _items;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
