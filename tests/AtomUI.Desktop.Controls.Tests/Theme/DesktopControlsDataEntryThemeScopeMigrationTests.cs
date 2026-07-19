using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Theme;

public class DesktopControlsDataEntryThemeScopeMigrationTests
{
    private static readonly string[] ControlDirectories =
    [
        "src/AtomUI.Desktop.Controls/AutoComplete",
        "src/AtomUI.Desktop.Controls/ButtonSpinner",
        "src/AtomUI.Desktop.Controls/Cascader",
        "src/AtomUI.Desktop.Controls/CheckBox",
        "src/AtomUI.Desktop.Controls/ComboBox",
        "src/AtomUI.Desktop.Controls/DatePicker",
        "src/AtomUI.Desktop.Controls/Form",
        "src/AtomUI.Desktop.Controls/Input",
        "src/AtomUI.Desktop.Controls/Mentions",
        "src/AtomUI.Desktop.Controls/NumericUpDown",
        "src/AtomUI.Desktop.Controls/OtpLineEdit",
        "src/AtomUI.Desktop.Controls/OptionButtonGroup",
        "src/AtomUI.Desktop.Controls/RadioButton",
        "src/AtomUI.Desktop.Controls/Rate",
        "src/AtomUI.Desktop.Controls/Select",
        "src/AtomUI.Desktop.Controls/Slider",
        "src/AtomUI.Desktop.Controls/Switch",
        "src/AtomUI.Desktop.Controls/TimePicker",
        "src/AtomUI.Desktop.Controls/Transfer",
        "src/AtomUI.Desktop.Controls/TreeSelect",
        "src/AtomUI.Desktop.Controls/Upload",
        "src/AtomUI.Desktop.Controls/Primitives/AddOnDecoratedBox",
        "src/AtomUI.Desktop.Controls/Primitives/InfoPickerInput"
    ];

    private static readonly string[] ThemeDirectories =
        ControlDirectories.Select(static directory => $"{directory}/Themes").ToArray();

    [Fact]
    public void DataEntry_Controls_Remove_Legacy_Token_Scope_Registration()
    {
        foreach (var relativeDirectory in ControlDirectories)
        {
            var codeFiles = Directory.GetFiles(
                GetRepoFile(relativeDirectory),
                "*.cs",
                SearchOption.AllDirectories);

            codeFiles.ShouldNotBeEmpty();

            foreach (var codeFile in codeFiles)
            {
                var text = File.ReadAllText(codeFile);
                text.ShouldNotContain("RegisterTokenResourceScope");
                text.ShouldNotContain("ScopeProvider");
            }
        }
    }

    [Fact]
    public void DataEntry_Control_Themes_Use_Ambient_Shared_Token_Scopes()
    {
        foreach (var relativeDirectory in ThemeDirectories)
        {
            ThemeAssetScopeAssertions.AssertDirectoryUsesSharedTokenScope(relativeDirectory);
        }
    }

    private static string GetRepoFile(string relativePath)
    {
        var directory = AppContext.BaseDirectory;
        while (directory is not null)
        {
            var candidate = Path.Combine(directory, relativePath);
            if (File.Exists(candidate) || Directory.Exists(candidate))
            {
                return candidate;
            }

            directory = Directory.GetParent(directory)?.FullName;
        }

        throw new FileNotFoundException(relativePath);
    }
}
