namespace AtomUI.Desktop.Controls;

public enum DialogButtonRole
{
    /// <summary>
    /// Clicking the button causes the dialog to be accepted (e.g. OK).
    /// </summary>
    AcceptRole,
    /// <summary>
    /// Clicking the button causes the dialog to be rejected (e.g. Cancel).
    /// </summary>
    RejectRole,
    /// <summary>
    /// Clicking the button causes a destructive change (e.g. for Discarding Changes) and closes the dialog.
    /// </summary>
    DestructiveRole,
    /// <summary>
    /// Clicking the button causes changes to the elements within the dialog.
    /// </summary>
    ActionRole,
    /// <summary>
    /// The button can be clicked to request help.
    /// </summary>
    HelpRole,
    /// <summary>
    /// The button is a "Yes"-like button.
    /// </summary>
    YesRole,
    /// <summary>
    /// The button is a "No"-like button.
    /// </summary>
    NoRole,
    /// <summary>
    /// The button applies current changes.
    /// </summary>
    ApplyRole,
    /// <summary>
    /// The button resets the dialog's fields to default values.
    /// </summary>
    ResetRole,
    /// <summary>
    /// User-defined button roles
    /// </summary>
    CustomRole
}