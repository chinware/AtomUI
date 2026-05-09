using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Theme;
using AtomUI.Theme.TokenSystem;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class MessageBoxToken : AbstractControlDesignToken
{
    public const string ID = "MessageBox";

    public static readonly ControlTokenResourceScopeProvider ScopeProvider = new(ID);

    /// <summary>
    /// Style Icon 的大小
    /// </summary>
    public double StyleIconSize { get; set; }

    public MessageBoxToken()
        : base(ID)
    {
    }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        StyleIconSize = SharedToken.SizeLG * 1.2;
    }

    protected override Type GetTokenKindType() => typeof(MessageBoxTokenKind);
}
