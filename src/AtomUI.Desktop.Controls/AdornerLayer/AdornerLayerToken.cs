using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Theme;
using AtomUI.Theme.TokenSystem;
using Avalonia;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class AdornerLayerToken : AbstractControlDesignToken
{
    public const string ID = "AdornerLayer";
    public static readonly ControlTokenResourceScopeProvider ScopeProvider = new(ID);

    public AdornerLayerToken()
        : base(ID)
    {
    }
    
    public Thickness FocusVisualMargin { get; set; }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        FocusVisualMargin = new Thickness(0);
    }
    
    protected override Type GetTokenKindType() => typeof(AdornerLayerTokenKind);
}