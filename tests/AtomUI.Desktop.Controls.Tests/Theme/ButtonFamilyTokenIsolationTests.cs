using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Theme;

public class ButtonFamilyTokenIsolationTests
{
    [Theory]
    [InlineData(
        "src/AtomUI.Desktop.Controls/Buttons/Themes/DropdownButtonTheme.axaml",
        "DropdownButtonTokenResource")]
    [InlineData(
        "src/AtomUI.Desktop.Controls/Buttons/Themes/HyperLinkButtonTheme.axaml",
        "HyperLinkButtonTokenResource")]
    [InlineData(
        "src/AtomUI.Desktop.Controls/Buttons/Themes/SplitButtonTheme.axaml",
        "SplitButtonTokenResource")]
    [InlineData(
        "src/AtomUI.Desktop.Controls/FloatButton/Themes/AbstractFloatButtonTheme.axaml",
        "FloatButtonTokenResource")]
    public void Independent_Button_Family_Owners_Consume_Only_Their_Own_Token_Family(
        string relativePath,
        string ownTokenResource)
    {
        var source = File.ReadAllText(Path.Combine(GetRepositoryRoot(), relativePath));

        source.ShouldContain(ownTokenResource);
        source.ShouldNotContain("{atom:ButtonTokenResource ");
        source.ShouldNotContain("{atom:PopupTokenResource ");
    }

    [Fact]
    public void DropdownButton_Does_Not_Inherit_Button_Theme_Token_Semantics()
    {
        var source = File.ReadAllText(Path.Combine(
            GetRepositoryRoot(),
            "src/AtomUI.Desktop.Controls/Buttons/Themes/DropdownButtonTheme.axaml"));

        source.ShouldNotContain("<atom:ButtonTheme");
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
