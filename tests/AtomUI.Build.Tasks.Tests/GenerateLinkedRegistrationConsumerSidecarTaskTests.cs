using AtomUI.Build.Tasks;
using AtomUI.LinkedRegistration.Protocol;
using Shouldly;
using Xunit;

namespace AtomUI.Build.Tasks.Tests;

public sealed class GenerateLinkedRegistrationConsumerSidecarTaskTests
{
    [Theory]
    [InlineData(true, 1)]
    [InlineData(false, 0)]
    public void Consumer_Assembly_Produces_Conservative_Package_Evidence(
        bool callsEntry,
        int expectedEntryCount)
    {
        using var fixture = new LinkedRegistrationAssemblyFixture();
        var consumerPath = fixture.CompileConsumer("Fixture.Consumer", callsEntry);
        var outputPath = fixture.GetOutputPath("Fixture.Consumer.atomui-link.json");
        var task = new GenerateLinkedRegistrationSidecarTask
        {
            BuildEngine = new RecordingBuildEngine(),
            AssemblyPath = consumerPath,
            OutputPath = outputPath,
            TargetFramework = "net10.0",
            ExtractConsumerUsage = true,
            CatalogSidecars = [new TestTaskItem(fixture.CatalogSidecarPath)]
        };

        task.Execute().ShouldBeTrue();
        LinkedRegistrationSidecarCodec.TryRead(
                File.ReadAllBytes(outputPath),
                out var sidecar,
                out var error)
            .ShouldBeTrue(error);
        sidecar.ShouldNotBeNull();
        sidecar.Packages.ShouldBeEmpty();
        sidecar.Usages.Count(static usage =>
                usage.Kind == "PackageRoot" && usage.Identity == "Fixture.AtomUI.Package")
            .ShouldBe(1);
        sidecar.Usages.Count(static usage =>
                usage.Kind == "Entry" && usage.Identity == "Fixture.AtomUI.Package")
            .ShouldBe(expectedEntryCount);
        sidecar.Fallbacks.ShouldHaveSingleItem().ShouldSatisfyAllConditions(
            fallback => fallback.PackageId.ShouldBe("Fixture.AtomUI.Package"),
            fallback => fallback.Reason.ShouldBe("ExtractedConsumerAssembly"),
            fallback => fallback.Source.ShouldBe("Fixture.Consumer.dll"));
    }

    [Fact]
    public void Delegate_Entry_Target_Is_Recovered_From_Ldftn()
    {
        using var fixture = new LinkedRegistrationAssemblyFixture();
        var consumerPath = fixture.CompileDelegateConsumer("Fixture.DelegateConsumer");
        var outputPath = fixture.GetOutputPath("Fixture.DelegateConsumer.atomui-link.json");
        var task = new GenerateLinkedRegistrationSidecarTask
        {
            BuildEngine = new RecordingBuildEngine(),
            AssemblyPath = consumerPath,
            OutputPath = outputPath,
            TargetFramework = "net10.0",
            ExtractConsumerUsage = true,
            CatalogSidecars = [new TestTaskItem(fixture.CatalogSidecarPath)]
        };

        task.Execute().ShouldBeTrue();
        LinkedRegistrationSidecarCodec.TryRead(
                File.ReadAllBytes(outputPath),
                out var sidecar,
                out var error)
            .ShouldBeTrue(error);
        sidecar.ShouldNotBeNull();
        sidecar.Usages.ShouldContain(static usage =>
            usage.Kind == "Entry" && usage.Identity == "Fixture.AtomUI.Package");
    }

    [Fact]
    public void Type_Operand_Before_Entry_Call_Does_Not_Break_Il_Decoding()
    {
        using var fixture = new LinkedRegistrationAssemblyFixture();
        var consumerPath = fixture.CompileConsumerWithTypeOperand("Fixture.TypeOperandConsumer");
        var outputPath = fixture.GetOutputPath("Fixture.TypeOperandConsumer.atomui-link.json");
        var task = new GenerateLinkedRegistrationSidecarTask
        {
            BuildEngine = new RecordingBuildEngine(),
            AssemblyPath = consumerPath,
            OutputPath = outputPath,
            TargetFramework = "net10.0",
            ExtractConsumerUsage = true,
            CatalogSidecars = [new TestTaskItem(fixture.CatalogSidecarPath)]
        };

        task.Execute().ShouldBeTrue();
        LinkedRegistrationSidecarCodec.TryRead(
                File.ReadAllBytes(outputPath),
                out var sidecar,
                out var error)
            .ShouldBeTrue(error);
        sidecar.ShouldNotBeNull();
        sidecar.Usages.ShouldContain(static usage =>
            usage.Kind == "Entry" && usage.Identity == "Fixture.AtomUI.Package");
    }
}
