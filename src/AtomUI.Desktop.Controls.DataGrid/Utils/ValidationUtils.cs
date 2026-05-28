// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel.DataAnnotations;
using Avalonia.Data;

namespace AtomUI.Desktop.Controls.Utils;

internal static class ValidationUtils
{
    /// <summary>
    /// Searches a ValidationResult for the specified target member name.  If the target is null
    /// or empty, this method will return true if there are no member names at all.
    /// </summary>
    /// <param name="validationResult">ValidationResult to search.</param>
    /// <param name="target">Member name to search for.</param>
    /// <returns>True if found.</returns>
    public static bool ContainsMemberName(this ValidationResult validationResult, string target)
    {
        var memberNames = validationResult.MemberNames;
        if (memberNames is IReadOnlyList<string> memberNameList)
        {
            int count = memberNameList.Count;
            if (count == 0)
            {
                return string.IsNullOrEmpty(target);
            }

            for (int i = 0; i < count; i++)
            {
                if (string.Equals(target, memberNameList[i]))
                {
                    return true;
                }
            }
            return false;
        }

        if (TryGetMemberNameCount(memberNames, out int knownCount) && knownCount == 0)
        {
            return string.IsNullOrEmpty(target);
        }

        int memberNameCount = 0;
        foreach (string memberName in memberNames)
        {
            if (string.Equals(target, memberName))
            {
                return true;
            }
            memberNameCount++;
        }
        return (memberNameCount == 0 && string.IsNullOrEmpty(target));
    }

    /// <summary>
    /// Finds an equivalent ValidationResult if one exists.
    /// </summary>
    /// <param name="collection">ValidationResults to search through.</param>
    /// <param name="target">ValidationResult to find.</param>
    /// <returns>Equal ValidationResult if found, null otherwise.</returns>
    public static ValidationResult? FindEqualValidationResult(this ICollection<ValidationResult> collection, ValidationResult target)
    {
        if (collection is IReadOnlyList<ValidationResult> validationResultList)
        {
            for (int i = 0, count = validationResultList.Count; i < count; i++)
            {
                var oldValidationResult = validationResultList[i];
                if (oldValidationResult.ErrorMessage == target.ErrorMessage &&
                    MemberNamesMatch(oldValidationResult.MemberNames, target.MemberNames))
                {
                    return oldValidationResult;
                }
            }
            return null;
        }

        foreach (ValidationResult oldValidationResult in collection)
        {
            if (oldValidationResult.ErrorMessage == target.ErrorMessage &&
                MemberNamesMatch(oldValidationResult.MemberNames, target.MemberNames))
            {
                return oldValidationResult;
            }
        }
        return null;
    }

    private static bool MemberNamesMatch(IEnumerable<string> oldMemberNames, IEnumerable<string> targetMemberNames)
    {
        if (oldMemberNames is IReadOnlyList<string> oldList && targetMemberNames is IReadOnlyList<string> targetList)
        {
            int count = oldList.Count;
            if (count != targetList.Count)
            {
                return false;
            }

            for (int i = 0; i < count; i++)
            {
                if (oldList[i] != targetList[i])
                {
                    return false;
                }
            }
            return true;
        }

        if (TryGetMemberNameCount(oldMemberNames, out int oldCount) &&
            TryGetMemberNameCount(targetMemberNames, out int targetCount) &&
            oldCount != targetCount)
        {
            return false;
        }

        using IEnumerator<string> oldEnumerator = oldMemberNames.GetEnumerator();
        using IEnumerator<string> targetEnumerator = targetMemberNames.GetEnumerator();
        while (true)
        {
            bool movedOld    = oldEnumerator.MoveNext();
            bool movedTarget = targetEnumerator.MoveNext();

            if (!movedOld || !movedTarget)
            {
                return movedOld == movedTarget;
            }

            if (oldEnumerator.Current != targetEnumerator.Current)
            {
                return false;
            }
        }
    }

    private static bool TryGetMemberNameCount(IEnumerable<string> memberNames, out int count)
    {
        switch (memberNames)
        {
            case ICollection<string> collection:
                count = collection.Count;
                return true;
            case IReadOnlyCollection<string> collection:
                count = collection.Count;
                return true;
            default:
                count = 0;
                return false;
        }
    }

    public static bool IsValid(this ValidationResult? result)
    {
        return result == null || result == ValidationResult.Success;
    }

    public static IEnumerable<Exception> UnpackException(Exception? exception)
    {
        if (exception == null)
        {
            return Array.Empty<Exception>();
        }

        if (exception is not AggregateException aggregate)
        {
            return exception is BindingChainException
                ? Array.Empty<Exception>()
                : new[] { exception };
        }

        List<Exception>? exceptions = null;
        foreach (var innerException in aggregate.InnerExceptions)
        {
            if (innerException is BindingChainException)
            {
                continue;
            }

            exceptions ??= new List<Exception>(aggregate.InnerExceptions.Count);
            exceptions.Add(innerException);
        }

        return exceptions ?? (IEnumerable<Exception>)Array.Empty<Exception>();
    }

    public static object? UnpackDataValidationException(Exception? exception)
    {
        if (exception is DataValidationException dataValidationException)
        {
            return dataValidationException.ErrorData;
        }

        return exception;
    }

    /// <summary>
    /// Determines whether the collection contains an equivalent ValidationResult
    /// </summary>
    /// <param name="collection">ValidationResults to search through</param>
    /// <param name="target">ValidationResult to search for</param>
    /// <returns></returns>
    public static bool ContainsEqualValidationResult(this ICollection<ValidationResult> collection, ValidationResult target)
    {
        return collection.FindEqualValidationResult(target) != null;
    }

    /// <summary>
    /// Adds a new ValidationResult to the collection if an equivalent does not exist.
    /// </summary>
    /// <param name="collection">ValidationResults to search through</param>
    /// <param name="value">ValidationResult to add</param>
    public static void AddIfNew(this ICollection<ValidationResult> collection, ValidationResult value)
    {
        if (!collection.ContainsEqualValidationResult(value))
        {
            collection.Add(value);
        }
    }

    private static bool ExceptionsMatch(Exception e1, Exception e2)
    {
        return e1.Message == e2.Message;
    }
    public static void AddExceptionIfNew(this ICollection<Exception> collection, Exception value)
    {
        if (collection is IReadOnlyList<Exception> exceptionList)
        {
            for (int i = 0, count = exceptionList.Count; i < count; i++)
            {
                if (ExceptionsMatch(exceptionList[i], value))
                {
                    return;
                }
            }

            collection.Add(value);
            return;
        }

        foreach (var exception in collection)
        {
            if (ExceptionsMatch(exception, value))
            {
                return;
            }
        }

        collection.Add(value);
    }

    /// <summary>
    /// Performs an action and catches any non-critical exceptions.
    /// </summary>
    /// <param name="action">Action to perform</param>
    public static void CatchNonCriticalExceptions(Action action)
    {
        try
        {
            action();
        }
        catch (Exception exception)
        {
            if (IsCriticalException(exception))
            {
                throw;
            }
            // Catch any non-critical exceptions
        }
    }

    /// <summary>
    /// Determines if the specified exception is un-recoverable.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns>True if the process cannot be recovered from the exception.</returns>
    public static bool IsCriticalException(Exception exception)
    {
        return (exception is OutOfMemoryException) ||
               (exception is StackOverflowException) ||
               (exception is AccessViolationException) ||
               (exception is ThreadAbortException);
    }
}
