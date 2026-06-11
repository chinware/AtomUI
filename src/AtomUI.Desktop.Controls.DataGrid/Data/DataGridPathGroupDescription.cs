// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Globalization;
using System.Runtime.CompilerServices;
using AtomUI.Controls.Data;
using AtomUI.Utils;
using Avalonia.Data.Converters;

namespace AtomUI.Desktop.Controls.Data;

public class DataGridPathGroupDescription : DataGridGroupDescription
{
    private string _propertyPath;
    private Type? _propertyOwnerType;
    private Type? _propertyType;
    private IDataGridDataMemberPathAccessor? _propertyAccessor;
    private IValueConverter? _valueConverter;
    private StringComparison _stringComparison = StringComparison.Ordinal;

    public DataGridPathGroupDescription(string propertyPath)
    {
        _propertyPath = propertyPath;
    }

    public DataGridPathGroupDescription(string propertyPath, IDataMemberAccessorDescriptor? dataMemberAccessorDescriptor)
        : this(propertyPath)
    {
        _dataMemberAccessorDescriptor = dataMemberAccessorDescriptor;
    }

    private readonly IDataMemberAccessorDescriptor? _dataMemberAccessorDescriptor;

    public override object? GroupKeyFromItem(object item, int level, CultureInfo culture)
    {
        object? GetKey(object? o)
        {
            if (o == null)
            {
                return null;
            }
            
            EnsurePropertyAccessor(o.GetType(), _dataMemberAccessorDescriptor, RuntimeFeature.IsDynamicCodeSupported);
            if (_propertyType != null)
            {
                return _propertyAccessor?.GetValue(o);
            }
            return null;
        }

        var key = GetKey(item);
        if (key == null)
        {
            key = item;
        }
        var valueConverter = ValueConverter;
        if (valueConverter != null)
        {
            key = valueConverter.Convert(key, typeof(object), level, culture);
        }
        return key;
    }

    public override bool KeysMatch(object groupKey, object itemKey)
    {
        if (groupKey is string k1 && itemKey is string k2)
        {
            return string.Equals(k1, k2, _stringComparison);
        }
        return base.KeysMatch(groupKey, itemKey);
    }

    public override string PropertyName => _propertyPath;

    public IValueConverter? ValueConverter
    {
        get => _valueConverter;
        set => _valueConverter = value;
    }

    internal void Initialize(Type itemType,
                             IDataMemberAccessorDescriptor? dataMemberAccessorDescriptor,
                             bool isDynamicCodeSupported = true)
    {
        EnsurePropertyAccessor(itemType, dataMemberAccessorDescriptor ?? _dataMemberAccessorDescriptor, isDynamicCodeSupported);
    }

    private void EnsurePropertyAccessor(Type itemType,
                                        IDataMemberAccessorDescriptor? dataMemberAccessorDescriptor,
                                        bool isDynamicCodeSupported)
    {
        if (_propertyOwnerType == itemType && _propertyAccessor != null)
        {
            return;
        }

        _propertyAccessor = DataGridDataMemberPathAccessor.Resolve(
            _propertyPath,
            itemType,
            dataMemberAccessorDescriptor,
            isDynamicCodeSupported);
        _propertyType      = _propertyAccessor.ValueType;
        _propertyOwnerType = itemType;
    }
}
