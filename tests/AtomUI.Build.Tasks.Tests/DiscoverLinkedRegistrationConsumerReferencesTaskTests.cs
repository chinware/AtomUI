using AtomUI.Build.Tasks;
using Shouldly;
using Xunit;

namespace AtomUI.Build.Tasks.Tests;

public sealed class DiscoverLinkedRegistrationConsumerReferencesTaskTests
{
    [Fact]
    public void Only_References_To_Known_Control_Package_Assemblies_Are_Selected()
    {
        using var fixture = new LinkedRegistrationAssemblyFixture();
        var consumerPath = fixture.CompileConsumer("Fixture.Consumer", callsEntry: true);
        var unrelatedPath = fixture.CompileUnrelated("Fixture.Unrelated");
        var task = new DiscoverLinkedRegistrationConsumerReferencesTask
        {
            BuildEngine = new RecordingBuildEngine(),
            ReferencePaths =
            [
                new TestTaskItem(fixture.PackageAssemblyPath),
                new TestTaskItem(consumerPath),
                new TestTaskItem(unrelatedPath)
            ],
            CatalogSidecars = [new TestTaskItem(fixture.CatalogSidecarPath)]
        };

        task.Execute().ShouldBeTrue();
        var consumer = task.ConsumerReferences.ShouldHaveSingleItem();
        consumer.ItemSpec.ShouldBe(consumerPath);
        consumer.GetMetadata("AtomUILinkedAssemblyName").ShouldBe("Fixture.Consumer");
    }
}
