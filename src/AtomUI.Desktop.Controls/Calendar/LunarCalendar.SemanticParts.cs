using AtomUI.Theme;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Desktop.Controls;

[SemanticPart(
    "header",
    SelectorClass = "semantic-header",
    ContractType = typeof(TemplatedControl),
    Since = "6.0")]
[SemanticPart(
    "body",
    SelectorClass = "semantic-body",
    ContractType = typeof(DockPanel),
    Since = "6.0")]
[SemanticPart(
    "content",
    SelectorClass = "semantic-content",
    ContractType = typeof(TemplatedControl),
    Since = "6.0")]
[SemanticPart(
    "item",
    SelectorClass = "semantic-item",
    SelectorRoute = "/template/ .semantic-content > .semantic-scope-body > .semantic-scope-cells > .semantic-item",
    ContractType = typeof(TemplatedControl),
    Cardinality = SemanticPartCardinality.Multiple,
    Since = "6.0",
    RuntimeCreated = true)]
[SemanticPart(
    "itemContent",
    SelectorClass = "semantic-item-content",
    SelectorRoute = "/template/ .semantic-content > .semantic-scope-body > .semantic-scope-cells > .semantic-item /template/ .semantic-item-content",
    ContractType = typeof(ContentControl),
    Cardinality = SemanticPartCardinality.Multiple,
    Since = "6.0",
    RuntimeCreated = true)]
public partial class LunarCalendar
{
}
