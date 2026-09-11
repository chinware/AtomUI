using AtomUI.Build.Tasks;
using AtomUI.LinkedRegistration.Protocol;
using Microsoft.Build.Framework;
using Shouldly;
using Xunit;

namespace AtomUI.Build.Tasks.Tests;

public sealed class ResolveLinkedRegistrationSidecarCandidatesTaskTests
{
    [Fact]
    public void Matching_Formal_Sidecar_Is_Canonical_And_Suppresses_Extraction()
    {
        using var fixture = new Fixture();
        var sidecar = fixture.CreateSidecar("Package");
        var task = fixture.CreateTask(
            new TestTaskItem(sidecar, ("AtomUILinkedSidecarSource", "Package")));

        task.Execute().ShouldBeTrue();
        var canonicalSidecar = task.CanonicalSidecars.ShouldHaveSingleItem();
        canonicalSidecar.ItemSpec.ShouldBe(sidecar);
        canonicalSidecar.GetMetadata("AtomUILinkedSidecarAssembly")
            .ShouldBe(fixture.AssemblyName);
        task.ExtractableReferences.ShouldBeEmpty();
    }

    [Fact]
    public void Missing_Formal_Sidecar_Produces_One_Extraction_Reference()
    {
        using var fixture = new Fixture();
        var task = fixture.CreateTask();

        task.Execute().ShouldBeTrue();
        task.CanonicalSidecars.ShouldBeEmpty();
        var extractableReference = task.ExtractableReferences.ShouldHaveSingleItem();
        extractableReference.ItemSpec.ShouldBe(fixture.ReferencePath);
        extractableReference.GetMetadata("AtomUILinkedAssemblyName")
            .ShouldBe(fixture.AssemblyName);
    }

    [Fact]
    public void Same_Assembly_And_Hash_Collapse_To_Project_Companion()
    {
        using var fixture = new Fixture();
        var packageSidecar = fixture.CreateSidecar("Package");
        var companionSidecar = fixture.CreateSidecar("ProjectCompanion");
        var task = fixture.CreateTask(
            new TestTaskItem(packageSidecar, ("AtomUILinkedSidecarSource", "Package")),
            new TestTaskItem(companionSidecar, ("AtomUILinkedSidecarSource", "ProjectCompanion")));

        task.Execute().ShouldBeTrue();
        task.CanonicalSidecars.Length.ShouldBe(1);
        task.CanonicalSidecars[0].ItemSpec.ShouldBe(companionSidecar);
        task.CanonicalSidecars[0].GetMetadata("AtomUILinkedSidecarSource")
            .ShouldBe("ProjectCompanion");
        task.ExtractableReferences.ShouldBeEmpty();
    }

    [Fact]
    public void Different_Hashes_Fail_Closed_With_All_Sources()
    {
        using var fixture = new Fixture();
        var packageSidecar = fixture.CreateSidecar("Package", "one");
        var companionSidecar = fixture.CreateSidecar("ProjectCompanion", "two");
        var task = fixture.CreateTask(
            new TestTaskItem(packageSidecar, ("AtomUILinkedSidecarSource", "Package")),
            new TestTaskItem(companionSidecar, ("AtomUILinkedSidecarSource", "ProjectCompanion")));

        task.Execute().ShouldBeFalse();
        task.CanonicalSidecars.ShouldBeEmpty();
        task.ExtractableReferences.ShouldBeEmpty();
        fixture.BuildEngine.Errors.Count.ShouldBe(1);
        fixture.BuildEngine.Errors[0].Code.ShouldBe("ATOMUILINK005");
        var message = fixture.BuildEngine.Errors[0].Message.ShouldNotBeNull();
        message.ShouldContain(fixture.AssemblyName);
        message.ShouldContain("Package");
        message.ShouldContain("ProjectCompanion");
        message.ShouldContain("sha256:");
    }

    [Fact]
    public void Unrelated_Formal_Sidecar_Does_Not_Enter_The_Current_Compilation()
    {
        using var fixture = new Fixture();
        var unrelatedPath = fixture.CreateSidecar("Package", assemblyName: "AtomUI.Unrelated");
        var task = fixture.CreateTask(
            new TestTaskItem(unrelatedPath, ("AtomUILinkedSidecarSource", "Package")));

        task.Execute().ShouldBeTrue();
        task.CanonicalSidecars.ShouldBeEmpty();
        task.ExtractableReferences.Length.ShouldBe(1);
    }

    [Fact]
    public void Non_AtomUI_Reference_Is_Not_Extracted()
    {
        using var fixture = new Fixture();
        var task = fixture.CreateTaskForReference(typeof(ITask).Assembly.Location);

        task.Execute().ShouldBeTrue();
        task.CanonicalSidecars.ShouldBeEmpty();
        task.ExtractableReferences.ShouldBeEmpty();
    }

    [Fact]
    public void Candidate_Source_Metadata_Is_Required()
    {
        using var fixture = new Fixture();
        var task = fixture.CreateTask(new TestTaskItem(fixture.CreateSidecar("MissingSource")));

        task.Execute().ShouldBeFalse();
        task.CanonicalSidecars.ShouldBeEmpty();
        task.ExtractableReferences.ShouldBeEmpty();
        fixture.BuildEngine.Errors.ShouldHaveSingleItem().Code.ShouldBe("ATOMUILINK005");
    }

    private sealed class Fixture : IDisposable
    {
        private readonly string _directory = Path.Combine(
            Path.GetTempPath(),
            "AtomUI-LinkedSidecarTests-" + Guid.NewGuid().ToString("N"));

        internal Fixture()
        {
            Directory.CreateDirectory(_directory);
            ReferencePath = typeof(ResolveLinkedRegistrationSidecarCandidatesTask).Assembly.Location;
            AssemblyName = typeof(ResolveLinkedRegistrationSidecarCandidatesTask).Assembly.GetName().Name!;
            BuildEngine = new RecordingBuildEngine();
        }

        internal string ReferencePath { get; }

        internal string AssemblyName { get; }

        internal RecordingBuildEngine BuildEngine { get; }

        internal ResolveLinkedRegistrationSidecarCandidatesTask CreateTask(
            params ITaskItem[] candidates)
        {
            return CreateTaskForReference(ReferencePath, candidates);
        }

        internal ResolveLinkedRegistrationSidecarCandidatesTask CreateTaskForReference(
            string referencePath,
            params ITaskItem[] candidates)
        {
            return new ResolveLinkedRegistrationSidecarCandidatesTask
            {
                BuildEngine = BuildEngine,
                SidecarCandidates = candidates,
                ReferencePaths = [new TestTaskItem(referencePath)]
            };
        }

        internal string CreateSidecar(
            string source,
            string variation = "same",
            string? assemblyName = null)
        {
            var sidecar = new LinkedRegistrationSidecar
            {
                Producer = "AtomUI.Build.Tasks.Tests." + variation,
                Assembly = new LinkedSidecarAssembly
                {
                    Name = assemblyName ?? AssemblyName,
                    TargetFramework = "net10.0"
                },
                Packages = [],
                Usages = [],
                Fallbacks = []
            };
            var path = Path.Combine(
                _directory,
                source + "-" + variation + ".atomui-link.json");
            File.WriteAllBytes(path, LinkedRegistrationSidecarCodec.Write(sidecar));
            return path;
        }

        public void Dispose()
        {
            if (Directory.Exists(_directory))
            {
                Directory.Delete(_directory, recursive: true);
            }
        }
    }
}
