using AtomUI.Controls.Commons;

namespace AtomUI.Desktop.Controls;

public class RibbonBadge : AbstractRibbonBadge
{
    public RibbonBadge()
    {
    }
    
    private protected override AbstractRibbonBadgeAdorner CreateBadgeAdorner()
    {
        if (_ribbonBadgeAdorner is null)
        {
            _ribbonBadgeAdorner = new RibbonBadgeAdorner();
            SetupTokenBindings();
            if (RibbonColor is not null)
            {
                SetupRibbonColor(RibbonColor);
            }
        }

        return _ribbonBadgeAdorner;
    }
}
