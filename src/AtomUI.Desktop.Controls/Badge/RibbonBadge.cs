using AtomUI.Controls.Commons;
using AtomUI.Generated.AtomUI_Desktop_Controls;
using AtomUI.Theme;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

[SemanticPart(
    "indicator",
    SelectorClass = "semantic-indicator",
    ContractType = typeof(Control),
    Cardinality = SemanticPartCardinality.Optional,
    Since = "6.0",
    RuntimeCreated = true)]
[SemanticPart(
    "content",
    SelectorClass = "semantic-content",
    ContractType = typeof(TextBlock),
    Cardinality = SemanticPartCardinality.Optional,
    Since = "6.0",
    RuntimeCreated = true)]
public partial class RibbonBadge : AbstractRibbonBadge
{
    public RibbonBadge()
    {
    }
    
    private protected override AbstractRibbonBadgeAdorner CreateBadgeAdorner()
    {
        if (_ribbonBadgeAdorner is null)
        {
            _ribbonBadgeAdorner = new RibbonBadgeAdorner();
            _ribbonBadgeAdorner.Classes.Add(RibbonBadgeSemanticParts.IndicatorClass);
            SetupTokenBindings();
            if (RibbonColor is not null)
            {
                SetupRibbonColor(RibbonColor);
            }
        }

        return _ribbonBadgeAdorner;
    }
}
