using Avalonia;

namespace AtomUI.Desktop.Controls;

internal static class CustomizableSizeLayoutHelper
{
    public static bool TryCalculateCustomContentHeight(
        CustomizableSizeType sizeType,
        double controlHeight,
        Thickness contentPadding,
        Thickness borderThickness,
        double minContentHeight,
        out double contentHeight)
    {
        contentHeight = double.NaN;
        if (!CanUseCustomMetrics(sizeType, controlHeight, minContentHeight))
        {
            return false;
        }

        var availableHeight = CalculateAvailableContentHeight(controlHeight, contentPadding, borderThickness);
        contentHeight = Math.Max(availableHeight, minContentHeight);
        return true;
    }

    public static bool ShouldUseCompactVerticalPadding(
        CustomizableSizeType sizeType,
        double controlHeight,
        Thickness contentPadding,
        Thickness borderThickness,
        double minContentHeight)
    {
        if (!CanUseCustomMetrics(sizeType, controlHeight, minContentHeight))
        {
            return false;
        }

        return CalculateAvailableContentHeight(controlHeight, contentPadding, borderThickness) < minContentHeight;
    }

    private static bool CanUseCustomMetrics(
        CustomizableSizeType sizeType,
        double controlHeight,
        double minContentHeight)
    {
        return sizeType == CustomizableSizeType.Custom &&
               IsUsablePositiveNumber(controlHeight) &&
               IsUsablePositiveNumber(minContentHeight);
    }

    private static double CalculateAvailableContentHeight(
        double controlHeight,
        Thickness contentPadding,
        Thickness borderThickness)
    {
        return controlHeight -
               contentPadding.Top -
               contentPadding.Bottom -
               borderThickness.Top -
               borderThickness.Bottom;
    }

    private static bool IsUsablePositiveNumber(double value)
    {
        return !double.IsNaN(value) &&
               !double.IsInfinity(value) &&
               value > 0;
    }
}
