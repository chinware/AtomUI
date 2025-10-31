using System.Diagnostics;
using AtomUI.Utils;
using Avalonia.Collections;

namespace AtomUI.Controls.Data;

public class ListFilterDescription
{
    public string? PropertyPath { get; set; }
    public bool HasPropertyPath => !string.IsNullOrEmpty(PropertyPath);
    public StringComparison ComparisonType { get; } = StringComparison.InvariantCultureIgnoreCase;
    
    public List<object> FilterConditions { get; set; } = new ();
    public Func<object, object, bool>? Filter { get; set; }
    
    public virtual bool FilterBy(object record)
    {
        foreach (var filterValue in FilterConditions)
        {
            var value = GetValue(record);
            // 为空就不比较
            if (value != null)
            {
                if (Filter != null)
                {
                    return Filter(value, filterValue);
                }
                // 默认按照字符串来比较
                var stringValue = value.ToString();
                if (!string.IsNullOrEmpty(stringValue) && filterValue is string stringFilterValue)
                {
                    return stringValue.Contains(stringFilterValue, ComparisonType);
                }
            }
        }

        return false;
    }
    
    private Type? GetPropertyType(object o)
    {
        return o.GetType().GetNestedPropertyType(PropertyPath);
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
        var type = GetPropertyType(o);
        if (HasPropertyPath)
        {
            Debug.Assert(PropertyPath != null);
            if (type != null)
            {
                return InvokePath(o, PropertyPath, type);
            }
        }
    
        if (type == o.GetType())
        {
            return o;
        }
               
        return null;
    }
}

public class ListFilterDescriptionList : AvaloniaList<ListFilterDescription> {}