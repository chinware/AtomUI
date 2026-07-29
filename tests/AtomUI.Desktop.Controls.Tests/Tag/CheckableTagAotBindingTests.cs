using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Tag;

public class CheckableTagAotBindingTests
{
    [Fact]
    public void ItemsPanel_Uses_Compiled_Ancestor_Bindings()
    {
        var source = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/Tag/Themes/CheckableTagItemsControlTheme.axaml"));

        source.ShouldNotContain("{Binding ");
        source.ShouldContain("{CompiledBinding $parent[atom:CheckableTagItemsControl].Orientation}");
        source.ShouldContain("{CompiledBinding $parent[atom:CheckableTagItemsControl].ItemSpacing}");
        source.ShouldContain("{CompiledBinding $parent[atom:CheckableTagItemsControl].LineSpacing}");
    }

    private static string GetRepoFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(candidate))
            {
                return candidate;
            }
            directory = directory.Parent;
        }

        return Path.Combine(AppContext.BaseDirectory, relativePath);
    }
}
