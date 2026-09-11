using System.Diagnostics;
using System.Reflection;
using AtomUI.Build.Tasks.Isolation;
using Shouldly;
using Xunit;

namespace AtomUI.Build.Tasks.Tests;

public sealed class TaskProcessHostTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), $"atomui-task-protocol-{Guid.NewGuid():N}");

    public TaskProcessHostTests() => Directory.CreateDirectory(_root);

    [Fact]
    public void Protocol_Preserves_Literal_Items_And_Custom_Identity_Metadata()
    {
        var item = new TaskWireItem
        {
            ItemSpec = "item%3Bname", Metadata = { ["Identity"] = "type%24name", ["Link"] = "a;b&c" }
        };
        var path = Path.Combine(_root, "item.json");
        TaskWire.Write(path, item);
        var restored = TaskWire.Read<TaskWireItem>(File.ReadAllText(path));
        restored.ItemSpec.ShouldBe("item%3Bname");
        restored.GetMetadata("identity").ShouldBe("type%24name");
        restored.GetMetadata("LINK").ShouldBe("a;b&c");
    }

    [Fact]
    public void Worker_Preserves_FullPath_And_Link_Metadata_When_Generating_Theme_Assets()
    {
        var source = Path.Combine(_root, "源 & 100%; 'quoted'.axaml");
        File.WriteAllText(source, "<ResourceDictionary xmlns=\"https://github.com/avaloniaui\" />");
        var request = new TaskRequest
        {
            TaskName = nameof(GenerateThemeAssetWrappersTask),
            Properties =
            {
                ["OutputDirectory"] = Path.Combine(_root, "generated"),
                ["AssemblyName"] = "Fixture.Test",
                ["GeneratedCodePath"] = Path.Combine(_root, "generated.cs")
            },
            Items =
            {
                ["ThemeAssets"] =
                [
                    new TaskWireItem
                    {
                        ItemSpec = "logical-only.axaml", FullPath = source,
                        Metadata = { ["Link"] = "Themes/Fixture.axaml" }
                    }
                ]
            }
        };
        var response = RoundTrip(request);
        response.Success.ShouldBeTrue();
        var asset = response.Items["GeneratedAssets"].ShouldHaveSingleItem();
        asset.Metadata["Link"].ShouldStartWith("AtomUI.Generated/");
        File.ReadAllText(asset.ItemSpec).ShouldContain("avares://Fixture.Test/Themes/Fixture.axaml");
        File.ReadAllText(Path.Combine(_root, "generated.cs")).ShouldContain("namespace AtomUI.Generated.FixtureTest;");
    }

    [Fact]
    public void Worker_Preserves_Empty_Item_Lists_And_Integer_Outputs()
    {
        var response = RoundTrip(new TaskRequest
        {
            TaskName = nameof(ValidateAssemblyMetadataMarkerTask),
            Properties =
            {
                ["AssemblyPath"] = typeof(TaskProcessHost).Assembly.Location,
                ["MarkerKey"] = "Not.An.AtomUI.Marker"
            }
        });
        response.Success.ShouldBeTrue();
        response.Properties["MarkerCount"].ShouldBe("0");

        response = RoundTrip(new TaskRequest
        {
            TaskName = nameof(ResolveLinkedRegistrationSidecarCandidatesTask),
            Items = { ["SidecarCandidates"] = [], ["ReferencePaths"] = [] }
        });
        response.Success.ShouldBeTrue();
        response.Items["CanonicalSidecars"].ShouldBeEmpty();
        response.Items["ExtractableReferences"].ShouldBeEmpty();
    }

    [Fact]
    public void Worker_Returns_Failure_With_Original_Diagnostic_Code_And_File()
    {
        var missingFile = Path.Combine(_root, "missing.dll");
        var response = RoundTrip(new TaskRequest
        {
            TaskName = nameof(ValidateAssemblyMetadataMarkerTask),
            Properties = { ["AssemblyPath"] = missingFile, ["MarkerKey"] = "Any" }
        });
        response.Success.ShouldBeFalse();
        var error = response.Diagnostics.ShouldHaveSingleItem();
        error.Kind.ShouldBe("error");
        error.Code.ShouldBe("ATOMUILINK001");
        error.File.ShouldBe(missingFile);
        error.Message.ShouldContain("missing.dll");
    }

    [Fact]
    public async Task MSBuild_Can_Overwrite_And_Delete_Toolset_After_Execution()
    {
        if (!OperatingSystem.IsWindows())
            return; // Windows file sharing is the regression this fixture exercises.

        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null && !File.Exists(Path.Combine(directory.FullName, "AtomUI.slnx")))
            directory = directory.Parent;
        directory.ShouldNotBeNull();
        var script = Path.Combine(directory.FullName, "scripts/verification/verify-build-task-isolation.ps1");
        var configuration = typeof(TaskProcessHostTests).Assembly
            .GetCustomAttribute<AssemblyConfigurationAttribute>()!.Configuration;
        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = "pwsh", UseShellExecute = false, CreateNoWindow = true,
            RedirectStandardOutput = true, RedirectStandardError = true,
            ArgumentList = { "-NoProfile", "-File", script, "-Configuration", configuration }
        });
        process.ShouldNotBeNull();
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(60));
        var stdout = process.StandardOutput.ReadToEndAsync(timeout.Token);
        var stderr = process.StandardError.ReadToEndAsync(timeout.Token);
        try
        {
            await process.WaitForExitAsync(timeout.Token);
        }
        catch (OperationCanceledException)
        {
            process.Kill(entireProcessTree: true);
            throw;
        }
        process.ExitCode.ShouldBe(0, await stdout + await stderr);
    }

    private TaskResponse RoundTrip(TaskRequest request)
    {
        var path = Path.Combine(_root, "protocol.json");
        TaskWire.Write(path, request);
        var response = TaskProcessHost.Execute(TaskWire.Read<TaskRequest>(File.ReadAllText(path)));
        TaskWire.Write(path, response);
        return TaskWire.Read<TaskResponse>(File.ReadAllText(path));
    }

    public void Dispose() => Directory.Delete(_root, recursive: true);
}
