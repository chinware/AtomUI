using AtomUI.Controls.Commons;
using AtomUI.Theme;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

[SemanticPart(
    "indicator",
    SelectorClass = "semantic-indicator",
    ContractType = typeof(Control),
    Cardinality = SemanticPartCardinality.Optional,
    CrossVisualRoot = true,
    Since = "6.0",
    RuntimeCreated = true)]
public partial class CountBadge : AbstractCountBadge
{
    public CountBadge()
    {
    }
    
    private protected override AbstractCountBadgeAdorner CreateBadgeAdorner()
    {
        if (_badgeAdorner is null)
        {
            _badgeAdorner = new CountBadgeAdorner();
            SetupTokenBindings();
            NotifyDecoratedTargetChanged();
            if (BadgeColor is not null)
            {
                ConfigureDotColor(BadgeColor);
            }
        }

        return _badgeAdorner;
    }
}
