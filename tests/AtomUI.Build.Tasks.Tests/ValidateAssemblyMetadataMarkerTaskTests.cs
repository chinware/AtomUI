using System.Reflection;
using AtomUI.Build.Tasks;
using Shouldly;
using Xunit;

[assembly: AssemblyMetadata("AtomUI.Test.SingleMarker", "AtomUI.Build.Tasks.Tests")]
[assembly: AssemblyMetadata("AtomUI.Test.DuplicateMarker", "First")]
[assembly: AssemblyMetadata("AtomUI.Test.DuplicateMarker", "Second")]

namespace AtomUI.Build.Tasks.Tests;

public sealed class ValidateAssemblyMetadataMarkerTaskTests : IDisposable
{
    private readonly string _directory = Path.Combine(
        Path.GetTempPath(),
        $"atomui-plan-marker-tests-{Guid.NewGuid():N}");

    public ValidateAssemblyMetadataMarkerTaskTests()
    {
        Directory.CreateDirectory(_directory);
    }

    [Fact]
    public void Execute_Accepts_Exactly_One_Matching_Assembly_Metadata_Marker()
    {
        var task = CreateTask("AtomUI.Test.SingleMarker");

        task.Execute().ShouldBeTrue();

        task.MarkerCount.ShouldBe(1);
        task.BuildEngine.ShouldBeAssignableTo<RecordingBuildEngine>().Errors.ShouldBeEmpty();
    }

    [Fact]
    public void Execute_Reports_Zero_When_The_Marker_Is_Missing()
    {
        var task = CreateTask("AtomUI.Test.MissingMarker");

        task.Execute().ShouldBeTrue();

        task.MarkerCount.ShouldBe(0);
        task.BuildEngine.ShouldBeAssignableTo<RecordingBuildEngine>().Errors.ShouldBeEmpty();
    }

    [Fact]
    public void Execute_Counts_Duplicate_Markers()
    {
        var task = CreateTask("AtomUI.Test.DuplicateMarker");

        task.Execute().ShouldBeTrue();

        task.MarkerCount.ShouldBe(2);
        task.BuildEngine.ShouldBeAssignableTo<RecordingBuildEngine>().Errors.ShouldBeEmpty();
    }

    [Fact]
    public void Execute_Fails_With_Linked_Diagnostic_For_An_Unreadable_Assembly()
    {
        var assemblyPath = Path.Combine(_directory, "Invalid.dll");
        File.WriteAllText(assemblyPath, "not a managed assembly");
        var task = new ValidateAssemblyMetadataMarkerTask
        {
            BuildEngine = new RecordingBuildEngine(),
            AssemblyPath = assemblyPath,
            MarkerKey = "AtomUI.Test.SingleMarker"
        };

        task.Execute().ShouldBeFalse();

        var error = task.BuildEngine.ShouldBeAssignableTo<RecordingBuildEngine>().Errors.ShouldHaveSingleItem();
        error.Code.ShouldBe("ATOMUILINK001");
        error.Message.ShouldNotBeNull();
        error.Message.ShouldContain(assemblyPath);
    }

    public void Dispose()
    {
        Directory.Delete(_directory, recursive: true);
    }

    private static ValidateAssemblyMetadataMarkerTask CreateTask(string markerKey)
    {
        return new ValidateAssemblyMetadataMarkerTask
        {
            BuildEngine = new RecordingBuildEngine(),
            AssemblyPath = typeof(ValidateAssemblyMetadataMarkerTaskTests).Assembly.Location,
            MarkerKey = markerKey
        };
    }
}
