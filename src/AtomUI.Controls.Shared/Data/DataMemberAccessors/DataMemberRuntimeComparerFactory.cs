using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AtomUI.Controls.Data;

internal static class DataMemberRuntimeComparerFactory
{
    public static IComparer? CreateForNotStringType(Type type)
    {
        return CreateForNotStringType(type, RuntimeFeature.IsDynamicCodeSupported);
    }

    internal static IComparer? CreateForNotStringType(Type type, bool isDynamicCodeSupported)
    {
        var nullableType = Nullable.GetUnderlyingType(type);
        if (nullableType != null && typeof(IComparable).IsAssignableFrom(nullableType))
        {
            return Comparer<object>.Create(CompareComparableValues);
        }

        if (typeof(IComparable).IsAssignableFrom(type))
        {
            return Comparer<object>.Create(CompareComparableValues);
        }

        if (isDynamicCodeSupported)
        {
            return GetRuntimeGeneratedComparer(type);
        }

        throw new InvalidOperationException(
            $"No AOT-safe comparer is available for data member type '{type.FullName}'. Provide a generated data member accessor or an explicit comparer.");
    }

    private static int CompareComparableValues(object? x, object? y)
    {
        if (x is null)
        {
            return y is null ? 0 : -1;
        }

        if (y is null)
        {
            return 1;
        }

        return ((IComparable)x).CompareTo(y);
    }

    [UnconditionalSuppressMessage("AOT", "IL3050",
        Justification = "Called only when RuntimeFeature.IsDynamicCodeSupported. NativeAOT uses non-generic IComparable or generated accessor comparers.")]
    private static IComparer? GetRuntimeGeneratedComparer(Type type)
    {
        return (typeof(Comparer<>).MakeGenericType(type).GetProperty("Default"))?.GetValue(null, null) as IComparer;
    }
}
