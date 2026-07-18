using System;
using System.IO;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.TreeSelectControl;

public class TreeSelectThemeContractTests
{
    [Fact]
    public void TreeSelect_Multiple_Empty_Small_Uses_Small_Content_Padding()
    {
        var source = ReadRepoFile("src/AtomUI.Desktop.Controls/TreeSelect/Themes/TreeSelectAddOnDecoratedBoxTheme.axaml");

        Regex.IsMatch(
                source,
                """
                <Style Selector="\^\[IsSelectionEmpty=True\]">[\s\S]*?<Style Selector="\^\[SizeType=Small\]">[\s\S]*?<Setter Property="Padding"\s+Value="\{atom:SelectTokenResource PaddingSM\}" />
                """,
                RegexOptions.CultureInvariant)
            .ShouldBeTrue("empty multiple TreeSelect should use small select padding when SizeType is Small.");
    }

    private static string ReadRepoFile(string relativePath)
    {
        var path = GetRepoFile(relativePath);
        File.Exists(path).ShouldBeTrue($"Expected repository file to exist: {relativePath}");
        return File.ReadAllText(path);
    }

    private static string GetRepoFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(candidate) || Directory.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        return Path.Combine(AppContext.BaseDirectory, relativePath);
    }
}
