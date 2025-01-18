// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace AtomUI.Controls.Utils;

internal static class TypeHelper
{
    internal const char LeftIndexerToken = '[';
    internal const char PropertyNameSeparator = '.';
    internal const char RightIndexerToken = ']';
    internal const char LeftParenthesisToken = '(';
    internal const char RightParenthesisToken = ')';

    private static Type? FindGenericType(Type definition, Type? type)
    {
        while (type != null && type != typeof(object))
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == definition)
            {
                return type;
            }

            if (definition.IsInterface)
            {
                foreach (var type2 in type.GetInterfaces())
                {
                    var type3 = FindGenericType(definition, type2);
                    if (type3 != null)
                    {
                        return type3;
                    }
                }
            }

            type = type.BaseType;
        }

        return null;
    }

    private static PropertyInfo? FindIndexerInMembers(MemberInfo[] members, string stringIndex, out object[]? index)
    {
        index = null;
        ParameterInfo[] parameters;
        PropertyInfo?   stringIndexer = null;

        foreach (var memberInfo in members)
        {
            if (memberInfo is PropertyInfo propertyInfo)
            {
                // Only a single parameter is supported and it must be a string or Int32 value.
                parameters = propertyInfo.GetIndexParameters();
                if (parameters.Length > 1)
                {
                    continue;
                }

                if (parameters[0].ParameterType == typeof(int))
                {
                    var intIndex = -1;
                    if (int.TryParse(stringIndex.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out intIndex))
                    {
                        index = new object[] { intIndex };
                        return propertyInfo;
                    }
                }

                // If string indexer is found save it, in case there is an int indexer.
                if (parameters[0].ParameterType == typeof(string))
                {
                    index         = new object[] { stringIndex };
                    stringIndexer = propertyInfo;
                }
            }
        }

        return stringIndexer;
    }

    private static string? GetDefaultMemberName(this Type type)
    {
        var attributes = type.GetCustomAttributes(typeof(DefaultMemberAttribute), true);
        if (attributes.Length == 1)
        {
            var defaultMemberAttribute = attributes[0] as DefaultMemberAttribute;
            return defaultMemberAttribute?.MemberName;
        }

        return null;
    }

    internal static string? GetDisplayName(this Type type, string propertyPath)
    {
        var propertyInfo = type.GetNestedProperty(propertyPath);
        if (propertyInfo != null)
        {
            object[] attributes = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), true);
            if (attributes.Length > 0)
            {
                Debug.Assert(attributes.Length == 1);
                if (attributes[0] is DisplayAttribute displayAttribute)
                {
                    return displayAttribute.GetShortName();
                }
            }
        }

        return null;
    }

    internal static Type GetNonNullableType(this Type type)
    {
        if (IsNullableType(type))
        {
            return type.GetGenericArguments()[0];
        }

        return type;
    }

    internal static Type GetEnumerableItemType(this Type enumerableType)
    {
        var type = FindGenericType(typeof(IEnumerable<>), enumerableType);
        if (type != null)
        {
            return type.GetGenericArguments()[0];
        }

        return enumerableType;
    }

    private static PropertyInfo? GetNestedProperty(this Type? parentType, string propertyPath, out Exception? exception,
                                                   ref object? item)
    {
        exception = null;
        if (parentType == null || string.IsNullOrEmpty(propertyPath))
        {
            item = null;
            return null;
        }

        var           type          = parentType;
        PropertyInfo? propertyInfo  = null;
        var           propertyNames = SplitPropertyPath(propertyPath);
        for (var i = 0; i < propertyNames.Count; i++)
        {
            // if we can't find the property or it is not of the correct type,
            // treat it as a null value
            propertyInfo = type.GetPropertyOrIndexer(propertyNames[i], out var index);
            if (propertyInfo == null)
            {
                item = null;
                return null;
            }

            if (!propertyInfo.CanRead)
            {
                exception =
                    new InvalidOperationException(
                        $"The property named '{propertyNames[i]}' on type '{type.GetTypeName()}' cannot be read.");
                item = null;
                return null;
            }

            if (item != null)
            {
                item = propertyInfo.GetValue(item, index);
            }

            type = propertyInfo.PropertyType.GetNonNullableType();
        }

        return propertyInfo;
    }

    internal static string GetTypeName(this Type type)
    {
        var baseType = type.GetNonNullableType();
        var s        = baseType.Name;
        if (type != baseType)
        {
            s += '?';
        }

        return s;
    }

    internal static PropertyInfo? GetNestedProperty(this Type parentType, string propertyPath, ref object? item)
    {
        return parentType.GetNestedProperty(propertyPath, out var ex, ref item);
    }

    internal static PropertyInfo? GetNestedProperty(this Type? parentType, string propertyPath)
    {
        if (parentType != null)
        {
            object? item = null;
            return parentType.GetNestedProperty(propertyPath, ref item);
        }

        return null;
    }

    internal static PropertyInfo? GetPropertyOrIndexer(this Type type, string propertyPath, out object[]? index)
    {
        index = null;
        // Return the default value of GetProperty if the first character is not an indexer token.
        if (string.IsNullOrEmpty(propertyPath) || propertyPath[0] != LeftIndexerToken)
        {
            var property = type.GetProperty(propertyPath);
            if (property != null)
            {
                return property;
            }

            // GetProperty does not return inherited interface properties,
            // so we need to enumerate them manually.
            if (type.IsInterface)
            {
                foreach (var typeInterface in type.GetInterfaces())
                {
                    property = typeInterface.GetProperty(propertyPath);
                    if (property != null)
                    {
                        return property;
                    }
                }
            }

            return null;
        }

        if (propertyPath.Length < 2 || propertyPath[^1] != RightIndexerToken)
        {
            // Return null if the indexer does not meet the standard format (i.e. "[x]").
            return null;
        }

        var stringIndex = propertyPath.Substring(1, propertyPath.Length - 2);
        var indexer     = FindIndexerInMembers(type.GetDefaultMembers(), stringIndex, out index);
        if (indexer != null)
        {
            // We found the indexer, so return it.
            return indexer;
        }

        var elementType = type.GetElementType();
        if (elementType == null)
        {
            var genericArguments = type.GetGenericArguments();
            if (genericArguments.Length == 1)
            {
                elementType = genericArguments[0];
            }
        }

        if (elementType != null)
        {
            var genericList = typeof(List<>).MakeGenericType(elementType);
            // If the object is of type IList, try to use its default indexer.
            if (genericList.IsAssignableFrom(type))
            {
                indexer = FindIndexerInMembers(genericList.GetDefaultMembers(), stringIndex, out index);
            }
            
            var genericReadOnlyList = typeof(List<>).MakeGenericType(elementType);
            if (genericReadOnlyList.IsAssignableFrom(type))
            {
                indexer = FindIndexerInMembers(genericReadOnlyList.GetDefaultMembers(), stringIndex, out index);
            }
        }

        return indexer;
    }

    internal static List<string> SplitPropertyPath(string propertyPath)
    {
        var propertyPaths = new List<string>();
        if (!string.IsNullOrEmpty(propertyPath))
        {
            bool parenthesisOn = false;
            int  startIndex    = 0;
            for (int index = 0; index < propertyPath.Length; index++)
            {
                if (parenthesisOn)
                {
                    if (propertyPath[index] == RightParenthesisToken)
                    {
                        parenthesisOn = false;
                        startIndex    = index + 1;
                    }

                    continue;
                }

                if (propertyPath[index] == LeftParenthesisToken)
                {
                    parenthesisOn = true;
                    if (startIndex != index)
                    {
                        propertyPaths.Add(propertyPath.Substring(startIndex, index - startIndex));
                        startIndex = index + 1;
                    }
                }
                else if (propertyPath[index] == PropertyNameSeparator)
                {
                    if (startIndex != index)
                    {
                        propertyPaths.Add(propertyPath.Substring(startIndex, index - startIndex));
                    }

                    startIndex = index + 1;
                }
                else if (startIndex != index && propertyPath[index] == LeftIndexerToken)
                {
                    propertyPaths.Add(propertyPath.Substring(startIndex, index - startIndex));
                    startIndex = index;
                }
                else if (index == propertyPath.Length - 1)
                {
                    propertyPaths.Add(propertyPath.Substring(startIndex));
                }
            }
        }

        return propertyPaths;
    }

    internal static bool IsEnumerableType(this Type enumerableType)
    {
        return FindGenericType(typeof(IEnumerable<>), enumerableType) != null;
    }

    internal static bool IsNullableType(this Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    internal static bool IsNullableEnum(this Type type)
    {
        return type.IsNullableType() &&
               type.GetGenericArguments().Length == 1 &&
               type.GetGenericArguments()[0].IsEnum;
    }

    internal static string PrependDefaultMemberName(object? item, string property)
    {
        if (item != null && !string.IsNullOrEmpty(property) && property[0] == LeftIndexerToken)
        {
            // The leaf property name is an indexer, so add the default member name.
            var declaringType     = item.GetType();
            var defaultMemberName = declaringType.GetNonNullableType().GetDefaultMemberName();
            if (!string.IsNullOrEmpty(defaultMemberName))
            {
                return defaultMemberName + property;
            }
        }

        return property;
    }

    internal static string RemoveDefaultMemberName(string property)
    {
        if (!string.IsNullOrEmpty(property) && property[^1] == RightIndexerToken)
        {
            // The property is an indexer, so remove the default member name.
            int leftIndexerToken = property.IndexOf(TypeHelper.LeftIndexerToken);
            if (leftIndexerToken >= 0)
            {
                return property.Substring(leftIndexerToken);
            }
        }

        return property;
    }

    internal static bool GetIsReadOnly(this MemberInfo? memberInfo)
    {
        if (memberInfo != null)
        {
            // Check if ReadOnlyAttribute is defined on the member
            object[] attributes = memberInfo.GetCustomAttributes(typeof(ReadOnlyAttribute), true);
            if (attributes.Length > 0)
            {
                var readOnlyAttribute = attributes[0] as ReadOnlyAttribute;
                Debug.Assert(readOnlyAttribute != null);
                return readOnlyAttribute.IsReadOnly;
            }
        }

        return false;
    }

    internal static Type? GetItemType(this IEnumerable list)
    {
        var   listType = list.GetType();
        Type? itemType = null;

        // if it's a generic enumerable, we get the generic type

        // Unfortunately, if data source is fed from a bare IEnumerable, TypeHelper will report an element type of object,
        // which is not particularly interesting.  We deal with it further on.
        if (listType.IsEnumerableType())
        {
            itemType = listType.GetEnumerableItemType();
        }

        // Bare IEnumerables mean that result type will be object.  In that case, we try to get something more interesting
        if (itemType == null || itemType == typeof(object))
        {
            // We haven't located a type yet.. try a different approach.
            // Does the list have anything in it?
            var enumerator = list.GetEnumerator();
            try
            {
                if (enumerator.MoveNext() && enumerator.Current != null)
                {
                    return enumerator.Current.GetType();
                }
            }
            finally
            {
                if (enumerator is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        // if we're null at this point, give up
        return itemType;
    }

    internal static object? GetNestedPropertyValue(object? item, string propertyPath, Type propertyType,
                                                   out Exception? exception)
    {
        exception = null;

        // if the item is null, treat the property value as null
        if (item == null)
        {
            return null;
        }

        // if the propertyPath is null or empty, return the item
        if (string.IsNullOrEmpty(propertyPath))
        {
            return item;
        }

        var propertyValue = item;
        var itemType      = item.GetType();
        var propertyInfo  = itemType.GetNestedProperty(propertyPath, out exception, ref propertyValue);
        if (propertyInfo != null && propertyInfo.PropertyType != propertyType)
        {
            return null;
        }

        return propertyValue;
    }
    
    internal static Type? GetNestedPropertyType(this Type? parentType, string? propertyPath)
    {
        if (parentType == null || string.IsNullOrEmpty(propertyPath))
        {
            return parentType;
        }

        var propertyInfo = parentType.GetNestedProperty(propertyPath);
        if (propertyInfo != null)
        {
            return propertyInfo.PropertyType;
        }
        return null;
    }
}