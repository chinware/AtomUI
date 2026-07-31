using AtomUI.Controls.Commons;
using AtomUI.Data;
using AtomUI.Desktop.Controls.DesignTokens;

namespace AtomUI.Desktop.Controls;

public class MarqueeLabel : AbstractMarqueeLabel
{
    public MarqueeLabel()
    {
        TokenResourceBinder.CreateTokenBinding(this, CycleSpaceProperty, MarqueeLabelTokenKind.CycleSpace);
        TokenResourceBinder.CreateTokenBinding(this, MoveSpeedProperty, MarqueeLabelTokenKind.DefaultSpeed);
    }
}