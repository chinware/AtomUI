using AtomUI.Theme.DesignTokens;
using Avalonia;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal sealed class AdornerLayerToken : AbstractControlDesignToken
{

    public AdornerLayerToken()

    {
    }
    
    public Thickness FocusVisualMargin { get; set; }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        FocusVisualMargin = new Thickness(0);
    }
    
}
