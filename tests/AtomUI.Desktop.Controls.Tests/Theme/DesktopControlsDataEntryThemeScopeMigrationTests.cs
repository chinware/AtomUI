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

    private static readonly (string Directory, string[] Extensions)[] ThemeDirectories =
    [
        ("src/AtomUI.Desktop.Controls/AutoComplete/Themes", ["AutoCompleteTokenSharedTokenResource"]),
        ("src/AtomUI.Desktop.Controls/ButtonSpinner/Themes", ["ButtonSpinnerTokenSharedTokenResource"]),
        ("src/AtomUI.Desktop.Controls/Cascader/Themes", ["CascaderTokenSharedTokenResource"]),
        ("src/AtomUI.Desktop.Controls/CheckBox/Themes", ["CheckBoxTokenSharedTokenResource"]),
        ("src/AtomUI.Desktop.Controls/ComboBox/Themes", ["ComboBoxTokenSharedTokenResource"]),
        ("src/AtomUI.Desktop.Controls/DatePicker/Themes", ["DatePickerTokenSharedTokenResource"]),
        ("src/AtomUI.Desktop.Controls/Form/Themes", ["FormTokenSharedTokenResource"]),
        ("src/AtomUI.Desktop.Controls/Input/Themes",
        [
            "LineEditTokenSharedTokenResource",
            "TextAreaTokenSharedTokenResource",
            "TextBoxTokenSharedTokenResource"
        ]),
        ("src/AtomUI.Desktop.Controls/Mentions/Themes", ["MentionsTokenSharedTokenResource"]),
        ("src/AtomUI.Desktop.Controls/NumericUpDown/Themes", ["NumericUpDownTokenSharedTokenResource"]),
        ("src/AtomUI.Desktop.Controls/OtpLineEdit/Themes", ["OtpLineEditTokenSharedTokenResource"]),
        ("src/AtomUI.Desktop.Controls/OptionButtonGroup/Themes", ["OptionButtonTokenSharedTokenResource"]),
        ("src/AtomUI.Desktop.Controls/RadioButton/Themes", ["RadioButtonTokenSharedTokenResource"]),
        ("src/AtomUI.Desktop.Controls/Rate/Themes", ["RateTokenSharedTokenResource"]),
        ("src/AtomUI.Desktop.Controls/Select/Themes", ["SelectTokenSharedTokenResource"]),
        ("src/AtomUI.Desktop.Controls/Slider/Themes", ["SliderTokenSharedTokenResource"]),
        ("src/AtomUI.Desktop.Controls/Switch/Themes", ["ToggleSwitchTokenSharedTokenResource"]),
        ("src/AtomUI.Desktop.Controls/TimePicker/Themes", ["TimePickerTokenSharedTokenResource"]),
        ("src/AtomUI.Desktop.Controls/Transfer/Themes", ["TransferTokenSharedTokenResource"]),
        ("src/AtomUI.Desktop.Controls/TreeSelect/Themes", ["TreeSelectTokenSharedTokenResource"]),
        ("src/AtomUI.Desktop.Controls/Upload/Themes", ["UploadTokenSharedTokenResource"]),
        ("src/AtomUI.Desktop.Controls/Primitives/AddOnDecoratedBox/Themes",
            ["AddOnDecoratedBoxTokenSharedTokenResource"]),
        ("src/AtomUI.Desktop.Controls/Primitives/InfoPickerInput/Themes",
            ["InfoPickerInputTokenSharedTokenResource"])
    ];

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
    public void DataEntry_Component_Themes_Use_Component_Shared_Token_Resources()
    {
        foreach (var (relativeDirectory, componentResourceExtensions) in ThemeDirectories)
        {
            var themeFiles = Directory.GetFiles(
                GetRepoFile(relativeDirectory),
                "*.axaml",
                SearchOption.AllDirectories);

            themeFiles.ShouldNotBeEmpty();

            foreach (var componentResourceExtension in componentResourceExtensions)
            {
                themeFiles.ShouldContain(file =>
                    File.ReadAllText(file).Contains(componentResourceExtension, StringComparison.Ordinal));
            }

            foreach (var themeFile in themeFiles)
            {
                File.ReadAllText(themeFile)
                    .ShouldNotContain("{atom:SharedTokenResource ");
            }
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
