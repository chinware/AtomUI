using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia.Layout;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateSeparatorScenarios()
    {
        return
        [
            new PerfScenario("Separator.Default", _ => new Separator()),
            new PerfScenario("Separator.WithTitle", _ => new Separator { Title = "Section" }),
            new PerfScenario("Separator.Dashed", _ => new Separator { Variant = SeparatorVariant.Dashed }),
            new PerfScenario("Separator.Dotted", _ => new Separator { Variant = SeparatorVariant.Dotted }),
            new PerfScenario("Separator.Vertical", _ => new VerticalSeparator()),
            new PerfScenario("Separator.WithTitle.Plain", _ => new Separator { Title = "Plain", IsPlain = true })
        ];
    }
}
