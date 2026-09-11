using AtomUI.Build.Tasks;
using Shouldly;
using Xunit;

namespace AtomUI.Build.Tasks.Tests;

public sealed class CollectAxamlUsageTaskTests : IDisposable
{
    private readonly string _directory = Path.Combine(
        Path.GetTempPath(),
        $"atomui-axaml-usage-tests-{Guid.NewGuid():N}");

    public CollectAxamlUsageTaskTests()
    {
        Directory.CreateDirectory(_directory);
    }

    [Fact]
    public void Execute_Collects_Structured_Element_Template_And_Selector_Types()
    {
        var path = Write(
            "Views/MainView.axaml",
            """
            <ResourceDictionary xmlns="https://github.com/avaloniaui"
                                xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                                xmlns:atom="using:AtomUI.Desktop.Controls">
                <ControlTheme TargetType="{x:Type atom:Button}"
                              BasedOn="{StaticResource {x:Type atom:BaseButton}}" />
                <Style Selector="atom|Button.previous-btn:pointerover" />
                <DataTemplate DataType="atom:Alert" />
                <ControlTemplate TargetType="atom:Dialog" />
                <atom:DatePicker />
            </ResourceDictionary>
            """);
        var task = CreateTask(
            new TestTaskItem(path, ("FullPath", path), ("Link", "Views/MainView.axaml")));

        task.Execute().ShouldBeTrue();

        var atomTypes = task.UsageCandidates
                            .Where(item => item.GetMetadata("TypeName")
                                               .StartsWith("AtomUI.", StringComparison.Ordinal))
                            .Select(item => item.GetMetadata("TypeName"))
                            .Distinct(StringComparer.Ordinal)
                            .OrderBy(static type => type, StringComparer.Ordinal)
                            .ToArray();
        atomTypes.ShouldBe(
        [
            "AtomUI.Desktop.Controls.Alert",
            "AtomUI.Desktop.Controls.BaseButton",
            "AtomUI.Desktop.Controls.Button",
            "AtomUI.Desktop.Controls.DatePicker",
            "AtomUI.Desktop.Controls.Dialog"
        ]);
        task.UsageCandidates.Where(item => item.GetMetadata("TypeName").StartsWith("AtomUI.", StringComparison.Ordinal))
            .ShouldAllBe(item =>
                int.Parse(item.GetMetadata("Line"), System.Globalization.CultureInfo.InvariantCulture) > 0 &&
                int.Parse(item.GetMetadata("Column"), System.Globalization.CultureInfo.InvariantCulture) > 0);
        task.Uncertainties.ShouldBeEmpty();
        File.Exists(task.OutputPath).ShouldBeTrue();
    }

    [Fact]
    public void Execute_Emits_Explicit_Roots_And_Dynamic_Source_Uncertainty_Deterministically()
    {
        var path = Write(
            "Themes/Dynamic.axaml",
            """
            <ResourceDictionary xmlns="https://github.com/avaloniaui">
                <ResourceInclude Source="{DynamicResource ThemeSource}" />
            </ResourceDictionary>
            """);
        var axaml = new TestTaskItem(
            path,
            ("FullPath", path),
            ("Link", "Themes/Dynamic.axaml"),
            ("AtomUILooseXaml", "true"));
        var task = CreateTask(axaml);
        task.UnitRoots = [new TestTaskItem("AtomUI.Desktop.Controls/DatePicker")];
        task.PackageRoots = [new TestTaskItem("Acme.Dynamic.Controls")];

        task.Execute().ShouldBeTrue();
        var firstOutput = File.ReadAllText(task.OutputPath);
        File.SetLastWriteTimeUtc(task.OutputPath, DateTime.UtcNow.AddMinutes(-1));
        var originalWriteTime = File.GetLastWriteTimeUtc(task.OutputPath);
        task.Execute().ShouldBeTrue();
        var secondOutput = File.ReadAllText(task.OutputPath);

        secondOutput.ShouldBe(firstOutput);
        File.GetLastWriteTimeUtc(task.OutputPath).ShouldBe(originalWriteTime);
        task.UsageCandidates.ShouldContain(item =>
            item.GetMetadata("Kind") == "UnitRoot" &&
            item.GetMetadata("Identity") == "AtomUI.Desktop.Controls/DatePicker");
        task.UsageCandidates.ShouldContain(item =>
            item.GetMetadata("Kind") == "PackageRoot" &&
            item.GetMetadata("Identity") == "Acme.Dynamic.Controls");
        task.Uncertainties.ShouldContain(item => item.GetMetadata("Reason") == "LooseAxaml");
        task.Uncertainties.ShouldContain(item => item.GetMetadata("Reason") == "DynamicResourceSource");
    }

    [Fact]
    public void Execute_Collects_Xml_Structures_Without_Interpreting_Escaped_Markup()
    {
        var path = Write(
            "Views/Controls.axaml",
            """
            <ResourceDictionary xmlns="https://github.com/avaloniaui"
                                xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                                xmlns:local="using:MyApplication.Controls"
                                xmlns:atom="clr-namespace:AtomUI.Desktop.Controls;assembly=AtomUI.Desktop.Controls">
                <local:AccentButton />
                <ControlTheme
                    TargetType="{x:Type atom:Button}"
                    BasedOn="{StaticResource {x:Type atom:BaseButton}}" />
                <Style
                    Selector="atom|Button /template/ local|AccentButton" />
                <DataTemplate
                    DataType="atom:Alert" />
                <ControlTemplate
                    TargetType="atom:Dialog" />
                <TextBlock Text="&lt;atom:GhostButton /&gt;" />
            </ResourceDictionary>
            """);

        var task = CreateTask(new TestTaskItem(path, ("FullPath", path), ("Link", "Views/Controls.axaml")));

        task.Execute().ShouldBeTrue();

        var types = task.UsageCandidates
                        .Select(item => item.GetMetadata("TypeName"))
                        .Where(static value => value.Length != 0)
                        .Distinct(StringComparer.Ordinal)
                        .OrderBy(static value => value, StringComparer.Ordinal)
                        .ToArray();
        types.ShouldBe(
        [
            "AtomUI.Desktop.Controls.Alert",
            "AtomUI.Desktop.Controls.BaseButton",
            "AtomUI.Desktop.Controls.Button",
            "AtomUI.Desktop.Controls.Dialog",
            "MyApplication.Controls.AccentButton"
        ]);
        types.ShouldNotContain("AtomUI.Desktop.Controls.GhostButton");
        task.Uncertainties.ShouldBeEmpty();
    }

    [Fact]
    public void Execute_Collects_The_Xml_Root_Element()
    {
        var path = Write(
            "Views/RootControl.axaml",
            """
            <atom:DatePicker xmlns:atom="using:AtomUI.Desktop.Controls" />
            """);
        var task = CreateTask(new TestTaskItem(path, ("FullPath", path), ("Link", "Views/RootControl.axaml")));

        task.Execute().ShouldBeTrue();

        task.UsageCandidates.ShouldContain(item =>
            item.GetMetadata("Kind") == "Element" &&
            item.GetMetadata("TypeName") == "AtomUI.Desktop.Controls.DatePicker");
    }

    [Fact]
    public void Execute_Recognizes_Xaml2006_Type_Through_A_Namespace_Alias()
    {
        var path = Write(
            "Views/AliasedType.axaml",
            """
            <ResourceDictionary xmlns="https://github.com/avaloniaui"
                                xmlns:xaml="http://schemas.microsoft.com/winfx/2006/xaml"
                                xmlns:atom="using:AtomUI.Desktop.Controls">
                <ControlTheme TargetType="{xaml:Type atom:Button}" />
            </ResourceDictionary>
            """);
        var task = CreateTask(new TestTaskItem(path, ("FullPath", path), ("Link", "Views/AliasedType.axaml")));

        task.Execute().ShouldBeTrue();

        task.UsageCandidates.ShouldContain(item =>
            item.GetMetadata("Kind") == "XType" &&
            item.GetMetadata("TypeName") == "AtomUI.Desktop.Controls.Button");
    }

    [Fact]
    public void Execute_Does_Not_Treat_Static_Resource_Uris_As_Dynamic()
    {
        var path = Write(
            "Themes/Includes.axaml",
            """
            <ResourceDictionary xmlns="https://github.com/avaloniaui">
                <ResourceInclude Source="PriceInputTheme.axaml" />
                <StyleInclude Source="/Assembly/Themes.axaml" />
                <ResourceInclude Source="avares://AtomUI.Desktop.Controls/Themes/Shared.axaml" />
                <StyleInclude Source="{DynamicResource ThemeSource}" />
                <ResourceInclude Source="http://[invalid-host" />
            </ResourceDictionary>
            """);
        var task = CreateTask(new TestTaskItem(path, ("FullPath", path), ("Link", "Themes/Includes.axaml")));

        task.Execute().ShouldBeTrue();

        task.Uncertainties.Length.ShouldBe(2);
        task.Uncertainties.ShouldContain(item =>
            item.GetMetadata("Value") == "{DynamicResource ThemeSource}");
        task.Uncertainties.ShouldContain(item =>
            item.GetMetadata("Value") == "http://[invalid-host");
    }

    [Fact]
    public void Execute_Emits_Raw_Uncertainties_With_Stable_Package_Identity()
    {
        var malformedPath = Write("Themes/Broken.axaml", "<ResourceDictionary>");
        var dynamicPath = Write(
            "Themes/Dynamic.axaml",
            """
            <ResourceDictionary xmlns="https://github.com/avaloniaui">
                <ResourceInclude Source="{DynamicResource ThemeSource}" />
            </ResourceDictionary>
            """);
        var loosePath = Write(
            "Themes/Loose.axaml",
            """
            <ResourceDictionary xmlns="https://github.com/avaloniaui"
                                xmlns:atom="using:AtomUI.Desktop.Controls">
                <atom:UnknownControl />
            </ResourceDictionary>
            """);
        var task = CreateTask(
            new TestTaskItem(malformedPath, ("FullPath", malformedPath), ("Link", "Themes/Broken.axaml")),
            new TestTaskItem(dynamicPath, ("FullPath", dynamicPath), ("Link", "Themes/Dynamic.axaml")),
            new TestTaskItem(
                loosePath,
                ("FullPath", loosePath),
                ("Link", "Themes/Loose.axaml"),
                ("AtomUILooseXaml", "true"),
                ("AtomUIRegistrationUnit", "AtomUI.Desktop.Controls/")));
        task.ProjectPackageId = "Acme.Project.Controls";

        task.Execute().ShouldBeTrue();

        task.UsageCandidates.ShouldContain(item =>
            item.GetMetadata("TypeName") == "AtomUI.Desktop.Controls.UnknownControl");
        task.Uncertainties.ShouldContain(item =>
            item.GetMetadata("Reason") == "MalformedXml" &&
            item.GetMetadata("PackageId") == "Acme.Project.Controls");
        task.Uncertainties.ShouldContain(item =>
            item.GetMetadata("Reason") == "DynamicResourceSource" &&
            item.GetMetadata("PackageId") == "Acme.Project.Controls");
        task.Uncertainties.ShouldContain(item =>
            item.GetMetadata("Reason") == "LooseAxaml" &&
            item.GetMetadata("PackageId") == "Acme.Project.Controls");
        task.Uncertainties.ShouldContain(item =>
            item.GetMetadata("Reason") == "MalformedOwnership" &&
            item.GetMetadata("PackageId") == "AtomUI.Desktop.Controls");
    }

    [Fact]
    public void Execute_Falls_Back_To_The_Project_Package_For_An_Invalid_Ownership_Prefix()
    {
        var path = Write(
            "Themes/Ownership.axaml",
            """
            <ResourceDictionary xmlns="https://github.com/avaloniaui" />
            """);
        var task = CreateTask(new TestTaskItem(
            path,
            ("FullPath", path),
            ("Link", "Themes/Ownership.axaml"),
            ("AtomUIRegistrationUnit", "invalid+package/")));
        task.ProjectPackageId = "Acme.Project.Controls";

        task.Execute().ShouldBeTrue();

        task.Uncertainties.ShouldContain(item =>
            item.GetMetadata("Reason") == "MalformedOwnership" &&
            item.GetMetadata("PackageId") == "Acme.Project.Controls");
    }

    public void Dispose()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, recursive: true);
        }
    }

    private CollectAxamlUsageTask CreateTask(params TestTaskItem[] items)
    {
        return new CollectAxamlUsageTask
        {
            BuildEngine = new RecordingBuildEngine(),
            AxamlFiles = items,
            ProjectDirectory = _directory,
            OutputPath = Path.Combine(_directory, "obj", "AtomUIAxamlUsage.xml")
        };
    }

    private string Write(string relativePath, string content)
    {
        var path = Path.Combine(_directory, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content);
        return path;
    }
}
