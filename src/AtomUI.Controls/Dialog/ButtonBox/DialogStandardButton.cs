using System.ComponentModel;

namespace AtomUI.Controls;

[Flags]
public enum DialogStandardButton
{
    /// <summary>
    /// An invalid button.
    /// </summary>
    NoButton = 0x00000000,
    /// <summary>
    /// An "OK" button defined with the AcceptRole.
    /// </summary>
    Ok = 0x00000001,
    /// <summary>
    /// An "Open" button defined with the AcceptRole.
    /// </summary>
    Open = 0x00000002,
    /// <summary>
    /// A "Save" button defined with the AcceptRole.
    /// </summary>
    Save = 0x00000004,
    /// <summary>
    /// A "Cancel" button defined with the RejectRole.
    /// </summary>
    Cancel = 0x00000008,
    /// <summary>
    /// A "Close" button defined with the RejectRole.
    /// </summary>
    Close = 0x00000010,
    /// <summary>
    /// A "Discard" or "Don't Save" button, depending on the platform, defined with the DestructiveRole.
    /// </summary>
    Discard = 0x00000020,
    /// <summary>
    /// An "Apply" button defined with the ApplyRole.
    /// </summary>
    Apply = 0x00000040,
    /// <summary>
    /// A "Reset" button defined with the ResetRole.
    /// </summary>
    Reset = 0x00000080,
    /// <summary>
    /// A "Reload" button defined with the ResetRole.
    /// </summary>
    Reload = 0x00000100,
    /// <summary>
    /// A "Restore Defaults" button defined with the ResetRole.
    /// </summary>
    RestoreDefaults = 0x00000200,
    /// <summary>
    /// A "Help" button defined with the HelpRole.
    /// </summary>
    Help = 0x00000400,
    /// <summary>
    /// A "Save All" button defined with the AcceptRole.
    /// </summary>
    SaveAll = 0x00000800,
    /// <summary>
    /// A "Yes" button defined with the YesRole.
    /// </summary>
    Yes = 0x00001000,
    /// <summary>
    /// A "Yes to All" button defined with the YesRole.
    /// </summary>
    YesToAll = 0x00002000,
    /// <summary>
    /// A "No" button defined with the NoRole.
    /// </summary>
    No = 0x00004000,
    /// <summary>
    /// A "No to All" button defined with the NoRole.
    /// </summary>
    NoToAll = 0x00008000,
    /// <summary>
    /// An "Abort" button defined with the RejectRole.
    /// </summary>
    Abort = 0x00010000,
    /// <summary>
    /// A "Retry" button defined with the AcceptRole.
    /// </summary>
    Retry = 0x00020000,
    /// <summary>
    /// An "Ignore" button defined with the AcceptRole.
    /// </summary>
    Ignore = 0x00040000,
}

public readonly struct DialogStandardButtons : IEquatable<DialogStandardButtons>
{
    public DialogStandardButton ButtonFlags { get; }

    public DialogStandardButtons(DialogStandardButton buttonFlags)
    {
        ButtonFlags = buttonFlags;
    }
    
    public static bool operator ==(DialogStandardButtons left, DialogStandardButtons right) => left.Equals(right);
    public static bool operator !=(DialogStandardButtons left, DialogStandardButtons right) => !left.Equals(right);

    public bool HasFlag(DialogStandardButton flag)
    {
        return ButtonFlags.HasFlag(flag);
    }
    
    public bool Equals(DialogStandardButtons other)
    {
        return ButtonFlags == other.ButtonFlags;
    }
    public override bool Equals(object? obj) => obj is DialogStandardButtons other && Equals(other);
    public override int GetHashCode() => ButtonFlags.GetHashCode();
    public override string ToString() => ButtonFlags.ToString();
    
    public static DialogStandardButtons Parse(string s)
    {
        var converter = new EnumConverter(typeof(DialogStandardButton));
        var dialogButton = converter.ConvertFromInvariantString(s) as DialogStandardButton?;
        if (dialogButton == null)
        {
            throw new InvalidOperationException("Invalid DialogStandardButton format.");
        }
        return new DialogStandardButtons(dialogButton.Value);
    }
    
    public static implicit operator DialogStandardButtons(DialogStandardButton flags) => new DialogStandardButtons(flags);
        
    public int Count
    {
        get
        {
            int count = 0;
            foreach (DialogStandardButton value in Enum.GetValues(typeof(DialogStandardButton)))
            {
                if (value != DialogStandardButton.NoButton && HasFlag(value))
                {
                    count++;
                }
            }
            return count;
        }
    }
}