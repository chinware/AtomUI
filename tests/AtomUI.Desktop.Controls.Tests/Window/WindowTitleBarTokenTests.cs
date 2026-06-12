using System;
using System.IO;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Window;

public class WindowTitleBarTokenTests
{
    [Fact]
    public void Caption_Button_Padding_Uses_Default_Two_Size_Units()
    {
        var source = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/WindowTitleBar/WindowTitleBarToken.cs"));

        source.ShouldContain("CaptionButtonPadding = new Thickness(SharedToken.SizeUnit * 2)");
    }

    [Fact]
    public void Title_Bar_Content_Padding_Has_No_Vertical_Inset()
    {
        var source = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/WindowTitleBar/WindowTitleBarToken.cs"));

        source.ShouldContain("TitleBarPadding             = new Thickness(LogoAndTitleSpacing * 1.8, 0)");
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
