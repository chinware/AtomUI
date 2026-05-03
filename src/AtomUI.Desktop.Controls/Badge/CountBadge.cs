using AtomUI.Controls.Commons;
using AtomUI.Theme;

namespace AtomUI.Desktop.Controls;

public class CountBadge : AbstractCountBadge
{
    public CountBadge()
    {
        this.RegisterTokenResourceScope(BadgeToken.ScopeProvider);
    }
    
    private protected override AbstractCountBadgeAdorner CreateBadgeAdorner()
    {
        if (_badgeAdorner is null)
        {
            _badgeAdorner = new CountBadgeAdorner();
            SetupTokenBindings();
            HandleDecoratedTargetChanged();
            if (BadgeColor is not null)
            {
                ConfigureDotColor(BadgeColor);
            }
        }

        return _badgeAdorner;
    }
}