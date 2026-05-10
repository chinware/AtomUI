using AtomUI.Controls.Commons;
using AtomUI.Theme;

namespace AtomUI.Desktop.Controls;

public class DotBadge : AbstractDotBadge
{
    public DotBadge()
    {
        this.RegisterTokenResourceScope(BadgeToken.ScopeProvider);
    }
    
    private protected override AbstractDotBadgeAdorner CreateDotBadgeAdorner()
    {
        if (_dotBadgeAdorner is null)
        {
            _dotBadgeAdorner = new DotBadgeAdorner();
            SetupTokenBindings();
            NotifyDecoratedTargetChanged();
            if (DotColor is not null)
            {
                ConfigureDotColor(DotColor);
            }
        }

        return _dotBadgeAdorner;
    }
}