using System.Diagnostics;
using System.Runtime.CompilerServices;
using AtomUI.Controls.Data;
using AtomUI.Utils;
using Avalonia.Collections;

namespace AtomUI.Desktop.Controls.Data;

public class DataGridFilterDescription
{
    private string? _propertyPath;
    private List<object>? _filterConditions;
    private Type? _propertyOwnerType;
    private Type? _propertyType;
    private IDataGridDataMemberPathAccessor? _propertyAccessor;

    internal int AccessorVersion { get; private set; }

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
                _propertyAccessor  = null;
                AccessorVersion++;
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
    
    internal void Initialize(Type itemType,
                             IDataMemberAccessorDescriptor? dataMemberAccessorDescriptor,
                             bool isDynamicCodeSupported = true)
    {
        if (!HasPropertyPath)
        {
            return;
        }

        EnsurePropertyAccessor(itemType, dataMemberAccessorDescriptor, isDynamicCodeSupported);
    }

    private void EnsurePropertyAccessor(Type itemType,
                                        IDataMemberAccessorDescriptor? dataMemberAccessorDescriptor,
                                        bool isDynamicCodeSupported)
    {
        if (_propertyOwnerType == itemType && _propertyAccessor != null)
        {
            return;
        }

        Debug.Assert(PropertyPath != null);
        _propertyAccessor = DataGridDataMemberPathAccessor.Resolve(
            PropertyPath,
            itemType,
            dataMemberAccessorDescriptor,
            isDynamicCodeSupported);
        _propertyType      = _propertyAccessor.ValueType;
        _propertyOwnerType = itemType;
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

        EnsurePropertyAccessor(o.GetType(), null, RuntimeFeature.IsDynamicCodeSupported);
        if (_propertyType != null)
        {
            return _propertyAccessor?.GetValue(o);
        }
               
        return null;
    }
}

public class DataGridFilterDescriptionCollection : AvaloniaList<DataGridFilterDescription> {}
