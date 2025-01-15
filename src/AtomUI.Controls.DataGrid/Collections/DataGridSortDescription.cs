// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.ComponentModel;
using System.Globalization;
using AtomUI.Controls.Utils;
using Avalonia.Collections;

namespace AtomUI.Controls.Collections;

public abstract class DataGridSortDescription
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

    public virtual DataGridSortDescription SwitchSortDirection()
    {
        return this;
    }

    internal virtual void Initialize(Type itemType)
    {
    }

    private static object? InvokePath(object item, string propertyPath, Type propertyType)
    {
        var propertyValue = TypeHelper.GetNestedPropertyValue(item, propertyPath, propertyType, out var exception);
        if (exception != null)
        {
            throw exception;
        }

        return propertyValue;
    }

    private class CultureSensitiveComparer : Comparer<object>
    {
        private CultureInfo _culture;

        public CultureSensitiveComparer(CultureInfo? culture)
        {
            _culture = culture ?? CultureInfo.InvariantCulture;
        }

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

            if (x is string && y is string)
            {
                return _culture.CompareInfo.Compare((string)x, (string)y);
            }

            return Default.Compare(x, y);
        }
    }

    private class DataGridPathSortDescription : DataGridSortDescription
    {
        private readonly ListSortDirection _direction;
        private readonly string _propertyPath;
        private readonly Lazy<CultureSensitiveComparer> _cultureSensitiveComparer;
        private readonly Lazy<IComparer<object>> _comparer;
        private Type? _propertyType;
        private IComparer? _internalComparer;
        private IComparer<object?>? _internalComparerTyped;

        private IComparer<object?>? InternalComparer
        {
            get
            {
                if (_internalComparerTyped == null && _internalComparer != null)
                {
                    if (_internalComparer is IComparer<object?> c)
                    {
                        _internalComparerTyped = c;
                    }
                    else
                    {
                        _internalComparerTyped = Comparer<object?>.Create((x, y) => _internalComparer.Compare(x, y));
                    }
                }

                return _internalComparerTyped;
            }
        }

        public override string PropertyPath => _propertyPath;
        public override IComparer<object> Comparer => _comparer.Value;
        public override ListSortDirection Direction => _direction;

        public DataGridPathSortDescription(string propertyPath, ListSortDirection direction, IComparer? internalComparer,
                                           CultureInfo? culture)
        {
            _propertyPath = propertyPath;
            _direction    = direction;
            _cultureSensitiveComparer = new Lazy<CultureSensitiveComparer>(() =>
                new CultureSensitiveComparer(culture ?? CultureInfo.CurrentCulture));
            _internalComparer = internalComparer;
            _comparer         = new Lazy<IComparer<object>>(() => Comparer<object>.Create((x, y) => Compare(x, y)));
        }

        private DataGridPathSortDescription(DataGridPathSortDescription inner, ListSortDirection direction)
        {
            _propertyPath             = inner._propertyPath;
            _direction                = direction;
            _propertyType             = inner._propertyType;
            _cultureSensitiveComparer = inner._cultureSensitiveComparer;
            _internalComparer         = inner._internalComparer;
            _internalComparerTyped    = inner._internalComparerTyped;

            _comparer = new Lazy<IComparer<object>>(() => Comparer<object>.Create((x, y) => Compare(x, y)));
        }

        private object? GetValue(object? o)
        {
            if (o == null)
            {
                return null;
            }

            if (HasPropertyPath)
            {
                return InvokePath(o, _propertyPath, _propertyType!);
            }

            if (_propertyType == o.GetType())
            {
                return o;
            }

            return null;
        }

        private IComparer GetComparerForType(Type type)
        {
            if (type == typeof(string))
                return _cultureSensitiveComparer.Value;
            else
                return GetComparerForNotStringType(type);
        }

        internal static IComparer GetComparerForNotStringType(Type type)
        {
            if (System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported == false)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                    type.GetGenericArguments()[0].IsAssignableTo(typeof(IComparable)))
                {
                    return Comparer<object?>.Create((x, y) =>
                    {
                        if (x == null)
                        {
                            return y == null ? 0 : -1;
                        }
                        return (x as IComparable)!.CompareTo(y);
                    });
                }
                else if (type.IsAssignableTo(typeof(IComparable)))
                {
                    //enum should be here
                    return Comparer<object>.Create((x, y) => (x as IComparable)!.CompareTo(y));
                }
                else
                {
                    return Comparer<object>.Create((x, y) => 0); //avoid using reflection to avoid crash on AOT
                }
            }
            return (typeof(Comparer<>).MakeGenericType(type).GetProperty("Default")!.GetValue(null, null) as
                IComparer)!;
        }

        private Type? GetPropertyType(object o)
        {
            return o.GetType().GetNestedPropertyType(_propertyPath);
        }

        private int Compare(object? x, object? y)
        {
            int result = 0;

            if (_propertyType == null)
            {
                if (x != null)
                {
                    _propertyType = GetPropertyType(x);
                }

                if (_propertyType == null && y != null)
                {
                    _propertyType = GetPropertyType(y);
                }
            }

            var v1 = GetValue(x);
            var v2 = GetValue(y);

            if (_propertyType != null && _internalComparer == null)
            {
                _internalComparer = GetComparerForType(_propertyType);
            }
            result = _internalComparer?.Compare(v1, v2) ?? 0;

            if (Direction == ListSortDirection.Descending)
            {
                return -result;
            }
            return result;
        }

        internal override void Initialize(Type itemType)
        {
            base.Initialize(itemType);

            if (_propertyType == null)
            {
                _propertyType = itemType.GetNestedPropertyType(_propertyPath);
            }

            if (_internalComparer == null && _propertyType != null)
            {
                _internalComparer = GetComparerForType(_propertyType);
            }
        }

        public override IOrderedEnumerable<object> OrderBy(IEnumerable<object> seq)
        {
            if (Direction == ListSortDirection.Descending)
            {
                return seq.OrderByDescending(o => GetValue(o), InternalComparer);
            }
            return seq.OrderBy(o => GetValue(o), InternalComparer);
        }

        public override IOrderedEnumerable<object> ThenBy(IOrderedEnumerable<object> seq)
        {
            if (Direction == ListSortDirection.Descending)
            {
                return seq.ThenByDescending(o => GetValue(o), InternalComparer);
            }
            else
            {
                return seq.ThenBy(o => GetValue(o), InternalComparer);
            }
        }

        public override DataGridSortDescription SwitchSortDirection()
        {
            var newDirection = _direction == ListSortDirection.Ascending
                ? ListSortDirection.Descending
                : ListSortDirection.Ascending;
            return new DataGridPathSortDescription(this, newDirection);
        }
    }

    public static DataGridSortDescription FromPath(string propertyPath,
                                                   ListSortDirection direction = ListSortDirection.Ascending,
                                                   CultureInfo? culture = null)
    {
        return new DataGridPathSortDescription(propertyPath, direction, null, culture);
    }

    public static DataGridSortDescription FromPath(string propertyPath, ListSortDirection direction, IComparer comparer)
    {
        return new DataGridPathSortDescription(propertyPath, direction, comparer, null);
    }

    public static DataGridSortDescription FromComparer(IComparer comparer,
                                                       ListSortDirection direction = ListSortDirection.Ascending)
    {
        return new DataGridComparerSortDescription(comparer, direction);
    }
}

public class DataGridComparerSortDescription : DataGridSortDescription
{
    private readonly IComparer _innerComparer;
    private readonly ListSortDirection _direction;
    private readonly IComparer<object> _comparer;

    public IComparer SourceComparer => _innerComparer;
    public override IComparer<object> Comparer => _comparer;
    public override ListSortDirection Direction => _direction;

    public DataGridComparerSortDescription(IComparer comparer, ListSortDirection direction)
    {
        _innerComparer = comparer;
        _direction     = direction;
        _comparer      = Comparer<object>.Create((x, y) => Compare(x, y));
    }

    private int Compare(object x, object y)
    {
        int result = _innerComparer.Compare(x, y);

        if (Direction == ListSortDirection.Descending)
            return -result;
        else
            return result;
    }

    public override DataGridSortDescription SwitchSortDirection()
    {
        var newDirection = _direction == ListSortDirection.Ascending
            ? ListSortDirection.Descending
            : ListSortDirection.Ascending;
        return new DataGridComparerSortDescription(_innerComparer, newDirection);
    }
}

public class DataGridSortDescriptionCollection : AvaloniaList<DataGridSortDescription>
{
}