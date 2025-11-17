// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics;
using System.Globalization;
using AtomUI.Utils;
using Avalonia.Data.Converters;

namespace AtomUI.Controls.Data;

public class DataGridPathGroupDescription : DataGridGroupDescription
{
    private string _propertyPath;
    private Type? _propertyType;
    private IValueConverter? _valueConverter;
    private StringComparison _stringComparison = StringComparison.Ordinal;

    public DataGridPathGroupDescription(string propertyPath)
    {
        _propertyPath = propertyPath;
    }

    public override object? GroupKeyFromItem(object item, int level, CultureInfo culture)
    {
        object? GetKey(object? o)
        {
            if (o == null)
            {
                return null;
            }
            
            _propertyType ??= GetPropertyType(o);
            Debug.Assert(_propertyType != null);
            return InvokePath(o, _propertyPath, _propertyType);
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

    private Type? GetPropertyType(object o)
    {
        return o.GetType().GetNestedPropertyType(_propertyPath);
    }

    private static object? InvokePath(object item, string propertyPath, Type propertyType)
    {
        object? propertyValue =
            TypeHelper.GetNestedPropertyValue(item, propertyPath, propertyType, out var exception);
        if (exception != null)
        {
            throw exception;
        }

        return propertyValue;
    }
}