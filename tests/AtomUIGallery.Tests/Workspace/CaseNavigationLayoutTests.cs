using System;
using System.IO;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.Workspace;

public class CaseNavigationLayoutTests
{
    [Fact]
    public void Sidebar_NavMenu_Uses_AntDesign_Item_Margins_Without_Outer_Menu_Padding()
    {
        var source = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/Workspace/Views/CaseNavigation.axaml"));

        source.ShouldContain("Name=\"ShowCaseNavMenu\"");
        source.ShouldContain("Padding=\"0\"");
        source.ShouldContain("IsItemBackgroundEnabled=\"False\"");
        source.ShouldNotContain("<atom:NavMenu.Styles>");
        source.ShouldNotContain("InlineNavMenuItemHeader");
        source.ShouldNotContain("<Setter Property=\"Margin\" Value=\"0,0,0,4\" />");
        source.ShouldNotContain("<Setter Property=\"Margin\" Value=\"0\" />");
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

        throw new FileNotFoundException($"Could not find repository file: {relativePath}");
    }
}
