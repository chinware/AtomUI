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
public partial class DotBadge : AbstractDotBadge
{
    public DotBadge()
    {
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
