using System.Diagnostics;
using AtomUI.Utils;
using Avalonia.Collections;

namespace AtomUI.Desktop.Controls.Data;

public class DataGridFilterDescription
{
    private string? _propertyPath;
    private List<object>? _filterConditions;
    private Type? _propertyOwnerType;
    private Type? _propertyType;

    public string? PropertyPath
    {
        get => _propertyPath;
        set
        {
            if (!string.Equals(_propertyPath, value, StringComparison.Ordinal))
            {
                _propertyPath      = value;
                _propertyOwnerType = null;
                _propertyType      = null;
            }
        }
    }

    public bool HasPropertyPath => !string.IsNullOrEmpty(PropertyPath);
    public StringComparison ComparisonType { get; } = StringComparison.InvariantCultureIgnoreCase;
    
    public List<object> FilterConditions
    {
        get => _filterConditions ??= new List<object>();
        set => _filterConditions = value;
    }

    public Func<object, object, bool>? Filter { get; set; }
    
    public virtual bool FilterBy(object record)
    {
        var filterConditions = _filterConditions;
        if (filterConditions == null || filterConditions.Count == 0)
        {
            return false;
        }

        var value = GetValue(record);
        // 为空就不比较
        if (value == null)
        {
            return false;
        }

        if (Filter != null)
        {
            return Filter(value, filterConditions[0]);
        }

        // 默认按照字符串来比较
        var stringValue = value.ToString();
        if (string.IsNullOrEmpty(stringValue))
        {
            return false;
        }

        foreach (var filterValue in filterConditions)
        {
            if (filterValue is string stringFilterValue &&
                stringValue.Contains(stringFilterValue, ComparisonType))
            {
                return true;
            }
        }

        return false;
    }
    
    private Type? GetPropertyType(object o)
    {
        var ownerType = o.GetType();
        if (_propertyOwnerType != ownerType)
        {
            _propertyType      = ownerType.GetNestedPropertyType(PropertyPath);
            _propertyOwnerType = ownerType;
        }

        return _propertyType;
    }
    
    private static object? InvokePath(object item, string propertyPath, Type propertyType)
    {
        object? propertyValue =
            TypeHelper.GetNestedPropertyValue(item, propertyPath, propertyType, out Exception? exception);
        if (exception != null)
        {
            throw exception;
        }

        return propertyValue;
    }
    
    private object? GetValue(object? o)
    {
        if (o == null)
        {
            return null;
        }
        if (!HasPropertyPath)
        {
            return o;
        }

        var type = GetPropertyType(o);
        if (type != null)
        {
            Debug.Assert(PropertyPath != null);
            return InvokePath(o, PropertyPath, type);
        }
               
        return null;
    }
}

public class DataGridFilterDescriptionCollection : AvaloniaList<DataGridFilterDescription> {}
