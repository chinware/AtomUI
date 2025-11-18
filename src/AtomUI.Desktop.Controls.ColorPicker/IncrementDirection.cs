// This source file is adapted from the WinUI project.
// (https://github.com/microsoft/microsoft-ui-xaml)

namespace AtomUI.Desktop.Controls.Primitives;

/// <summary>
/// Defines the direction a color component should be incremented.
/// </summary>
internal enum IncrementDirection
{
    /// <summary>
    /// Decreasing in value towards zero.
    /// </summary>
    Lower,

    /// <summary>
    /// Increasing in value towards positive infinity.
    /// </summary>
    Higher,
}