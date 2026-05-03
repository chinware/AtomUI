using AtomUI.Controls.Commons;
using AtomUI.Theme;

namespace AtomUI.Desktop.Controls;

public class RibbonBadge : AbstractRibbonBadge
{
    public RibbonBadge()
    {
        this.RegisterTokenResourceScope(BadgeToken.ScopeProvider);
    }
    
    private protected override AbstractRibbonBadgeAdorner CreateBadgeAdorner()
    {
        if (_ribbonBadgeAdorner is null)
        {
            _ribbonBadgeAdorner = new RibbonBadgeAdorner();
            SetupTokenBindings();
            HandleDecoratedTargetChanged();
            if (RibbonColor is not null)
            {
                SetupRibbonColor(RibbonColor);
            }
        }

        return _ribbonBadgeAdorner;
    }
}