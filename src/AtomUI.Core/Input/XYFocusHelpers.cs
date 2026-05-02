using Avalonia.Input;
using Avalonia.VisualTree;

namespace AtomUI.Input;

/// <summary>
/// Helper methods for XY focus navigation.
/// Extracted from Avalonia 12 internal XYFocusHelpers class.
/// </summary>
internal static class XYFocusHelpers
{
    /// <summary>
    /// Determines whether XY navigation is allowed for the specified input element and key device type.
    /// </summary>
    /// <param name="visual">The input element to check.</param>
    /// <param name="keyDeviceType">The type of key device (Keyboard, Gamepad, Remote, or null for programmatic).</param>
    /// <returns>True if XY navigation is allowed; otherwise, false.</returns>
    internal static bool IsAllowedXYNavigationMode(this InputElement visual, KeyDeviceType? keyDeviceType)
    {
        return IsAllowedXYNavigationMode(XYFocus.GetNavigationModes(visual), keyDeviceType);
    }

    /// <summary>
    /// Determines whether XY navigation is allowed for the specified navigation modes and key device type.
    /// </summary>
    /// <param name="modes">The XY focus navigation modes.</param>
    /// <param name="keyDeviceType">The type of key device (Keyboard, Gamepad, Remote, or null for programmatic).</param>
    /// <returns>True if XY navigation is allowed; otherwise, false.</returns>
    private static bool IsAllowedXYNavigationMode(XYFocusNavigationModes modes, KeyDeviceType? keyDeviceType)
    {
        return keyDeviceType switch
        {
            null => !modes.Equals(XYFocusNavigationModes.Disabled), // programmatic input, allow any subtree except Disabled.
            KeyDeviceType.Keyboard => modes.HasFlag(XYFocusNavigationModes.Keyboard),
            KeyDeviceType.Gamepad => modes.HasFlag(XYFocusNavigationModes.Gamepad),
            KeyDeviceType.Remote => modes.HasFlag(XYFocusNavigationModes.Remote),
            _ => throw new ArgumentOutOfRangeException(nameof(keyDeviceType), keyDeviceType, null)
        };
    }

    /// <summary>
    /// Finds the root element for XY focus search starting from the specified visual.
    /// </summary>
    /// <param name="visual">The starting input element.</param>
    /// <param name="keyDeviceType">The type of key device.</param>
    /// <returns>The root element for XY focus search, or null if not found.</returns>
    internal static InputElement? FindXYSearchRoot(this InputElement visual, KeyDeviceType? keyDeviceType)
    {
        InputElement candidate = visual;
        InputElement? candidateParent = visual.FindAncestorOfType<InputElement>();

        while (candidateParent is not null && candidateParent.IsAllowedXYNavigationMode(keyDeviceType))
        {
            candidate = candidateParent;
            candidateParent = candidate.FindAncestorOfType<InputElement>();
        }

        return candidate;
    }
}
