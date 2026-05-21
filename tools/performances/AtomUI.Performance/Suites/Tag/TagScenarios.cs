using AtomUI.Desktop.Controls;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateTagScenarios()
    {
        return
        [
            new PerfScenario("Tag.Default.NoText", _ => new Tag()),
            new PerfScenario("Tag.Default.Text", _ => new Tag { Text = "Tag" }),
            new PerfScenario("Tag.Closable", _ => new Tag { Text = "Closable", IsClosable = true }),
            new PerfScenario("Tag.PresetColor", _ => new Tag { Text = "blue", TagColor = "blue" }),
            new PerfScenario("Tag.StatusColor", _ => new Tag { Text = "Success", TagColor = "Success" }),
            new PerfScenario("Tag.CustomColor", _ => new Tag { Text = "#f50", TagColor = "#f50" }),
            new PerfScenario("Tag.WithIcon", _ => new Tag
            {
                Text = "Icon",
                Icon = new AtomUI.Icons.AntDesign.SearchOutlined()
            }),
            new PerfScenario("Tag.PresetColor.Closable", _ => new Tag
            {
                Text       = "magenta",
                TagColor   = "magenta",
                IsClosable = true
            })
        ];
    }
}
