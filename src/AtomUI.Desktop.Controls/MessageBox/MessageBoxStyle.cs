namespace AtomUI.Desktop.Controls;

public enum MessageBoxStyle
{
    /// <summary>
    /// the message box does not have any icon.
    /// </summary>
    Normal,
    /// <summary>
    /// an icon indicating that the message is asking a question.
    /// </summary>
    Confirm,
    /// <summary>
    /// an icon indicating that the message is nothing out of the ordinary.
    /// </summary>
    Information,
    /// <summary>
    /// an icon indicating that the message is a success.
    /// </summary>
    Success,
    /// <summary>
    /// an icon indicating that the message is a warning, but can be dealt with.
    /// </summary>
    Warning,
    /// <summary>
    /// an icon indicating that the message represents a critical problem.
    /// </summary>
    Error
}