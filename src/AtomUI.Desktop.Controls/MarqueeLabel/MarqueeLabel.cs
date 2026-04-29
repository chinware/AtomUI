using AtomUI.Controls.Commons;
using AtomUI.Data;
using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Theme;

namespace AtomUI.Desktop.Controls;

public class MarqueeLabel : AbstractMarqueeLabel
{
    public MarqueeLabel()
    {
        this.RegisterTokenResourceScope(MarqueeLabelToken.ScopeProvider);
        TokenResourceBinder.CreateTokenBinding(this, CycleSpaceProperty, MarqueeLabelTokenKind.CycleSpace);
        TokenResourceBinder.CreateTokenBinding(this, MoveSpeedProperty, MarqueeLabelTokenKind.DefaultSpeed);
    }
}