using System;
using System.IO;
using Avalonia.Controls;
using AtomUI.Data;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Drawer;

public class DrawerAotBindingTests
{
    [Fact]
    public void Drawer_Default_OpenOn_Does_Not_Use_One_Time_TopLevel_Lookup()
    {
        var source = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Drawer/Drawer.cs"));

        source.ShouldNotContain("TopLevel.GetTopLevel(this)");
        source.ShouldContain("BindVisualAncestor");
    }

    [Fact]
    public void BindVisualAncestor_Can_Bind_Detached_Drawer_OpenOn()
    {
        var drawer = new AtomUI.Desktop.Controls.Drawer();

        using var binding = BindUtils.BindVisualAncestor(
            drawer,
            drawer,
            AtomUI.Desktop.Controls.Drawer.OpenOnProperty,
            typeof(TopLevel),
            priority: Avalonia.Data.BindingPriority.Template);

        drawer.OpenOn.ShouldBeNull();
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
