using System;
using System.IO;
using System.Linq;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class GalleryDataGridSelectableTextTests
{
    [Fact]
    public void Api_And_DesignToken_DataGrid_Text_Cells_Use_Selectable_Copyable_Text()
    {
        var files = Directory.GetFiles(
                                 GetRepoDirectory("controlgallery/AtomUIGallery/ShowCases"),
                                 "*DataGrid.axaml",
                                 SearchOption.AllDirectories)
                             .Where(path => path.EndsWith("ApiDataGrid.axaml", StringComparison.Ordinal) ||
                                            path.EndsWith("DesignTokenDataGrid.axaml", StringComparison.Ordinal))
                             .ToArray();

        files.ShouldNotBeEmpty();

        foreach (var file in files)
        {
            var source = File.ReadAllText(file);
            source.ShouldNotContain("<atom:TextBlock Text=\"{Binding");

            if (source.Contains("Text=\"{Binding", StringComparison.Ordinal))
            {
                source.ShouldContain("<gallery:GallerySelectableTextBlock");
            }
        }
    }

    private static string GetRepoDirectory(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        return Path.Combine(AppContext.BaseDirectory, relativePath);
    }
}
