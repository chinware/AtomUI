using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using AtomUI.Utils;
using Avalonia.Collections;

namespace AtomUI.Controls.Data;

public abstract class ListSortDescription : IListSortDescription
{
    public virtual string? PropertyPath => null;
    
    public virtual ListSortDirection Direction => ListSortDirection.Ascending;
    
    public bool HasPropertyPath => !string.IsNullOrEmpty(PropertyPath);
    
    public abstract IComparer<object> Comparer { get; }

    public virtual IOrderedEnumerable<object> OrderBy(IEnumerable<object> seq)
    {
        return seq.OrderBy(o => o, Comparer);
    }

    public virtual IOrderedEnumerable<object> ThenBy(IOrderedEnumerable<object> seq)
    {
        return seq.ThenBy(o => o, Comparer);
    }

    public virtual ListSortDescription SwitchSortDirection()
    {
        return this;
    }

    internal virtual void Initialize(Type itemType)
    {
        Initialize(itemType, null, RuntimeFeature.IsDynamicCodeSupported);
    }

    internal virtual void Initialize(Type itemType, bool isDynamicCodeSupported)
    {
        Initialize(itemType, null, isDynamicCodeSupported);
    }

    internal virtual void Initialize(Type itemType, IDataMemberAccessorDescriptor? dataMemberAccessorDescriptor)
    {
        Initialize(itemType, dataMemberAccessorDescriptor, RuntimeFeature.IsDynamicCodeSupported);
    }

    internal virtual void Initialize(Type itemType,
                                     IDataMemberAccessorDescriptor? dataMemberAccessorDescriptor,
                                     bool isDynamicCodeSupported)
    {
    }

    private interface IPropertyPathAccessor
    {
        Type? ValueType { get; }

        IComparer? Comparer { get; }

        void Initialize(Type itemType);

        object? GetValue(object instance);
    }

    private sealed class DataMemberPropertyPathAccessor : IPropertyPathAccessor
    {
        private readonly IDataMemberAccessor _accessor;

        public Type ValueType => _accessor.ValueType;

        public IComparer? Comparer => _accessor.Comparer;

        public DataMemberPropertyPathAccessor(IDataMemberAccessor accessor)
        {
            _accessor = accessor;
        }

        public void Initialize(Type itemType)
        {
        }

        public object? GetValue(object instance)
        {
            return _accessor.GetValue(instance);
        }
    }

    private sealed class RuntimeReflectionPropertyPathAccessor : IPropertyPathAccessor
    {
        private readonly string _propertyPath;
        private Type? _valueType;

        public Type? ValueType => _valueType;

        public IComparer? Comparer => null;

        public RuntimeReflectionPropertyPathAccessor(string propertyPath)
        {
            _propertyPath = propertyPath;
        }

        [UnconditionalSuppressMessage("Trimming", "IL2026",
            Justification = "Legacy reflection path is created only when dynamic code is available; NativeAOT requires generated data member accessors.")]
        public void Initialize(Type itemType)
        {
            _valueType = itemType.GetNestedPropertyType(_propertyPath);
        }

        [UnconditionalSuppressMessage("Trimming", "IL2026",
            Justification = "Legacy reflection path is created only when dynamic code is available; NativeAOT requires generated data member accessors.")]
        public object? GetValue(object instance)
        {
            if (_valueType == null)
            {
                Initialize(instance.GetType());
                if (_valueType == null)
                {
                    return null;
                }
            }

            object? propertyValue =
                TypeHelper.GetNestedPropertyValue(instance, _propertyPath, _valueType, out Exception? exception);
            if (exception != null)
            {
                throw exception;
            }

            return propertyValue;
        }
    }
    
    public static ListSortDescription FromPath(string propertyPath, ListSortDirection direction = ListSortDirection.Ascending, CultureInfo? culture = null)
    {
        return new ListPathSortDescription(propertyPath, direction, null, culture);
    }

    public static ListSortDescription FromPath(string propertyPath, ListSortDirection direction, IComparer comparer)
    {
        return new ListPathSortDescription(propertyPath, direction, comparer, null);
    }

    public static ListSortDescription FromComparer(IComparer comparer, ListSortDirection direction = ListSortDirection.Ascending)
    {
        return new ListComparerSortDescription(comparer, direction);
    }

    /// <summary>
    /// Creates a comparer class that takes in a CultureInfo as a parameter,
    /// which it will use when comparing strings.
    /// </summary>
    private class CultureSensitiveComparer : Comparer<object>
    {
        /// <summary>
        /// Private accessor for the CultureInfo of our comparer
        /// </summary>
        private CultureInfo _culture;

        /// <summary>
        /// Creates a comparer which will respect the CultureInfo
        /// that is passed in when comparing strings.
        /// </summary>
        /// <param name="culture">The CultureInfo to use in string comparisons</param>
        public CultureSensitiveComparer(CultureInfo? culture)
        {
            _culture = culture ?? CultureInfo.InvariantCulture;
        }

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to or greater than the other.
        /// </summary>
        /// <param name="x">first item to compare</param>
        /// <param name="y">second item to compare</param>
        /// <returns>Negative number if x is less than y, zero if equal, and a positive number if x is greater than y</returns>
        /// <remarks>
        /// Compares the 2 items using the specified CultureInfo for string and using the default object comparer for all other objects.
        /// </remarks>
        public override int Compare(object? x, object? y)
        {
            if (x == null)
            {
                if (y != null)
                {
                    return -1;
                }

                return 0;
            }

            if (y == null)
            {
                return 1;
            }

            // at this point x and y are not null
            if (x is string stringX && y is string stringY)
            {
                return _culture.CompareInfo.Compare(stringX, stringY);
            }

            return Default.Compare(x, y);
        }
    }

    private class ListPathSortDescription : ListSortDescription
    {
        private readonly ListSortDirection _direction;
        private readonly string _propertyPath;
        private readonly Lazy<CultureSensitiveComparer> _cultureSensitiveComparer;
        private readonly Lazy<IComparer<object>> _comparer;
        private readonly IComparer? _explicitComparer;
        private Type? _propertyType;
        private IComparer? _activeComparer;
        private IComparer<object?>? _activeComparerTyped;
        private IPropertyPathAccessor? _propertyAccessor;

        private IComparer<object?>? InternalComparer
        {
            get
            {
                if (_activeComparerTyped == null && _activeComparer != null)
                {
                    if (_activeComparer is IComparer<object?> c)
                    {
                        _activeComparerTyped = c;
                    }
                    else
                    {
                        _activeComparerTyped = Comparer<object?>.Create((x, y) => _activeComparer.Compare(x, y));
                    }
                }

                return _activeComparerTyped;
            }
        }

        public override string PropertyPath => _propertyPath;
        public override IComparer<object> Comparer => _comparer.Value;
        public override ListSortDirection Direction => _direction;

        public ListPathSortDescription(string propertyPath, ListSortDirection direction, IComparer? internalComparer,
                                       CultureInfo? culture)
        {
            _propertyPath = propertyPath;
            _direction    = direction;
            _cultureSensitiveComparer = new Lazy<CultureSensitiveComparer>(() =>
                new CultureSensitiveComparer(culture ?? CultureInfo.CurrentCulture));
            _explicitComparer = internalComparer;
            _activeComparer   = internalComparer;
            _comparer         = new Lazy<IComparer<object>>(() => Comparer<object>.Create(Compare));
        }

        private ListPathSortDescription(ListPathSortDescription inner, ListSortDirection direction)
        {
            _propertyPath             = inner._propertyPath;
            _direction                = direction;
            _propertyType             = inner._propertyType;
            _cultureSensitiveComparer = inner._cultureSensitiveComparer;
            _explicitComparer         = inner._explicitComparer;
            _activeComparer           = inner._activeComparer;
            _activeComparerTyped      = inner._activeComparerTyped;
            _propertyAccessor         = inner._propertyAccessor;

            _comparer = new Lazy<IComparer<object>>(() => Comparer<object>.Create(Compare));
        }

        private object? GetValue(object? o)
        {
            if (o == null)
            {
                return null;
            }

            if (HasPropertyPath)
            {
                EnsurePropertyAccessor(o.GetType(), null, RuntimeFeature.IsDynamicCodeSupported);
                return _propertyAccessor?.GetValue(o);
            }

            if (_propertyType == o.GetType())
            {
                return o;
            }
               
            return null;
        }

        private IComparer? GetComparerForType(Type type)
        {
            if (type == typeof(string))
            {
                return _cultureSensitiveComparer.Value;
            }
            return GetComparerForNotStringType(type);
        }

        internal static IComparer? GetComparerForNotStringType(Type type)
        {
            return DataMemberRuntimeComparerFactory.CreateForNotStringType(type);
        }

        private void EnsurePropertyAccessor(Type itemType,
                                            IDataMemberAccessorDescriptor? dataMemberAccessorDescriptor,
                                            bool isDynamicCodeSupported)
        {
            if (_propertyAccessor != null || !HasPropertyPath)
            {
                return;
            }

            IPropertyPathAccessor propertyAccessor =
                CreatePropertyAccessor(itemType, dataMemberAccessorDescriptor, isDynamicCodeSupported);
            propertyAccessor.Initialize(itemType);
            _propertyAccessor = propertyAccessor;
            _propertyType     = propertyAccessor.ValueType;
            SetActiveComparer(GetComparerForAccessor(propertyAccessor));
        }

        private IPropertyPathAccessor CreatePropertyAccessor(Type itemType,
                                                             IDataMemberAccessorDescriptor? dataMemberAccessorDescriptor,
                                                             bool isDynamicCodeSupported)
        {
            if (TryResolveDataMemberAccessor(itemType, dataMemberAccessorDescriptor, out var accessor))
            {
                return new DataMemberPropertyPathAccessor(accessor);
            }

            if (!isDynamicCodeSupported)
            {
                throw new InvalidOperationException(
                    $"AOT-safe data member accessor for path '{_propertyPath}' on type '{itemType.FullName}' was not found. " +
                    $"Mark the data type with [{nameof(GenerateDataMemberAccessorsAttribute)}] or pass an {nameof(IDataMemberAccessorDescriptor)}.");
            }

            return new RuntimeReflectionPropertyPathAccessor(_propertyPath);
        }

        private bool TryResolveDataMemberAccessor(Type itemType,
                                                  IDataMemberAccessorDescriptor? dataMemberAccessorDescriptor,
                                                  out IDataMemberAccessor accessor)
        {
            if (dataMemberAccessorDescriptor != null &&
                IsDescriptorCompatible(dataMemberAccessorDescriptor, itemType) &&
                dataMemberAccessorDescriptor.TryGetAccessor(_propertyPath, out accessor!))
            {
                return true;
            }

            if (DataMemberAccessorRegistry.TryGet(itemType, out var descriptor) &&
                descriptor.TryGetAccessor(_propertyPath, out accessor!))
            {
                return true;
            }

            accessor = null!;
            return false;
        }

        private static bool IsDescriptorCompatible(IDataMemberAccessorDescriptor descriptor, Type itemType)
        {
            return descriptor.DataType == itemType || descriptor.DataType.IsAssignableFrom(itemType);
        }

        private IComparer? GetComparerForAccessor(IPropertyPathAccessor accessor)
        {
            if (_explicitComparer != null)
            {
                return _explicitComparer;
            }

            if (accessor.ValueType == typeof(string))
            {
                return _cultureSensitiveComparer.Value;
            }

            return accessor.Comparer ?? (accessor.ValueType == null ? null : GetComparerForType(accessor.ValueType));
        }

        private void SetActiveComparer(IComparer? comparer)
        {
            if (!ReferenceEquals(_activeComparer, comparer))
            {
                _activeComparer      = comparer;
                _activeComparerTyped = null;
            }
        }

        private void EnsurePropertyType(object? x, object? y, bool isDynamicCodeSupported)
        {
            if (_propertyAccessor != null || !HasPropertyPath)
            {
                return;
            }

            if (x != null)
            {
                EnsurePropertyAccessor(x.GetType(), null, isDynamicCodeSupported);
                return;
            }

            if (y != null)
            {
                EnsurePropertyAccessor(y.GetType(), null, isDynamicCodeSupported);
            }
        }

        private int Compare(object? x, object? y)
        {
            int result = 0;

            EnsurePropertyType(x, y, RuntimeFeature.IsDynamicCodeSupported);

            var v1 = GetValue(x);
            var v2 = GetValue(y);

            result = _activeComparer?.Compare(v1, v2) ?? 0;

            if (Direction == ListSortDirection.Descending)
            {
                return -result;
            }
            return result;
        }

        internal override void Initialize(Type itemType,
                                          IDataMemberAccessorDescriptor? dataMemberAccessorDescriptor,
                                          bool isDynamicCodeSupported)
        {
            base.Initialize(itemType, dataMemberAccessorDescriptor, isDynamicCodeSupported);

            if (HasPropertyPath)
            {
                EnsurePropertyAccessor(itemType, dataMemberAccessorDescriptor, isDynamicCodeSupported);
            }
            else
            {
                _propertyType ??= itemType;
                SetActiveComparer(_explicitComparer ?? GetComparerForType(_propertyType));
            }
        }

        public override IOrderedEnumerable<object> OrderBy(IEnumerable<object> seq)
        {
            if (Direction == ListSortDirection.Descending)
            {
                return seq.OrderByDescending(GetValue, InternalComparer);
            }
            return seq.OrderBy(GetValue, InternalComparer);
        }

        public override IOrderedEnumerable<object> ThenBy(IOrderedEnumerable<object> seq)
        {
            if (Direction == ListSortDirection.Descending)
            {
                return seq.ThenByDescending(GetValue, InternalComparer);
            }
            return seq.ThenBy(GetValue, InternalComparer);
        }

        public override ListSortDescription SwitchSortDirection()
        {
            var newDirection = _direction == ListSortDirection.Ascending
                ? ListSortDirection.Descending
                : ListSortDirection.Ascending;
            return new ListPathSortDescription(this, newDirection);
        }
    }
}

public class ListComparerSortDescription : ListSortDescription
{
    private readonly IComparer _innerComparer;
    private readonly ListSortDirection _direction;
    private readonly IComparer<object> _comparer;

    public IComparer SourceComparer => _innerComparer;
    public override IComparer<object> Comparer => _comparer;
    public override ListSortDirection Direction => _direction;
    public ListComparerSortDescription(IComparer comparer, ListSortDirection direction)
    {
        _innerComparer = comparer;
        _direction     = direction;
        _comparer      = Comparer<object>.Create((x, y) => Compare(x, y));
    }

    private int Compare(object x, object y)
    {
        int result = _innerComparer.Compare(x, y);

        if (Direction == ListSortDirection.Descending)
        {
            return -result;
        }

        return result;
    }
    
    public override ListSortDescription SwitchSortDirection()
    {
        var newDirection = _direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
        return new ListComparerSortDescription(_innerComparer, newDirection);
    }
}

public class SortDescriptionList : AvaloniaList<IListSortDescription> {}
