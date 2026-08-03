using System.Xml.Linq;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Theme;

public class ThemeSelectorBoundaryTests
{
    [Fact]
    public void Production_Theme_Selectors_Do_Not_Cross_Multiple_Template_Boundaries()
    {
        var repositoryRoot = GetRepositoryRoot();
        var sourceRoot     = Path.Combine(repositoryRoot, "src");
        var violations     = new List<string>();

        foreach (var path in Directory.EnumerateFiles(sourceRoot, "*.axaml", SearchOption.AllDirectories))
        {
            var document = XDocument.Load(path);
            foreach (var style in document.Descendants().Where(element => element.Name.LocalName == "Style"))
            {
                var selector = style.Attribute("Selector")?.Value;
                if (string.IsNullOrWhiteSpace(selector))
                {
                    continue;
                }

                foreach (var branch in selector.Split(','))
                {
                    if (CountTemplateBoundaries(branch) > 1)
                    {
                        violations.Add($"{Path.GetRelativePath(repositoryRoot, path)}: {branch.Trim()}");
                    }
                }
            }
        }

        violations.Count.ShouldBe(0, string.Join(Environment.NewLine, violations));
    }

    private static int CountTemplateBoundaries(string selector)
    {
        var count = 0;
        var index = 0;
        while ((index = selector.IndexOf("/template/", index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += "/template/".Length;
        }

        return count;
    }

    private static string GetRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "AtomUI.slnx")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Unable to locate repository root.");
    }
}
