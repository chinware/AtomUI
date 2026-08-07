namespace AtomUI.Localization.Build;

internal static class XliffTranslationTarget
{
    internal static bool IsPublishable(XliffUnitModel unit)
    {
        if (unit is null)
        {
            throw new ArgumentNullException(nameof(unit));
        }
        return !string.IsNullOrWhiteSpace(unit.Target) &&
               unit.TargetState is "translated" or "reviewed" or "final" &&
               !RequiresAction(unit.TargetSubState);
    }

    private static bool RequiresAction(string? subState)
    {
        if (subState is null || string.IsNullOrWhiteSpace(subState))
        {
            return false;
        }

        var normalized = subState.Trim();
        var namespaceSeparator = normalized.LastIndexOf(':');
        if (namespaceSeparator >= 0)
        {
            normalized = normalized.Substring(namespaceSeparator + 1);
        }

        return normalized.StartsWith("needs-", StringComparison.OrdinalIgnoreCase);
    }
}
