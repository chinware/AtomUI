using AtomUI.Theme;
using AtomUI.Theme.Compilation;
using AtomUI.Theme.Configuration;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

[Collection(ThemeConfigProviderTestCollection.Name)]
public class ThemeScopeGraphTests
{
    [Fact]
    public void Registration_Ids_Are_Monotonic_And_Are_Not_Reused_After_Reattach()
    {
        var manager = new ThemeManager(static () => true);
        var root = CreateRootContext(manager);
        var graph = new ThemeScopeGraph(manager);
        var provider = new ThemeConfigProvider();

        var first = graph.Register(provider, root, EmptyConfig());
        var firstId = first.RegistrationId;
        first.Dispose();
        var second = graph.Register(provider, root, EmptyConfig());

        second.RegistrationId.ShouldBeGreaterThan(firstId);
        graph.TopologyRevision.ShouldBe(3);
    }

    [Fact]
    public void Capture_Records_Parent_And_Config_Revisions_In_Topological_Order()
    {
        var manager = new ThemeManager(static () => true);
        var root = CreateRootContext(manager);
        var graph = new ThemeScopeGraph(manager);
        var parentProvider = new ThemeConfigProvider();
        var childProvider = new ThemeConfigProvider();
        using var parent = graph.Register(parentProvider, root, EmptyConfig());
        using var child = graph.Register(childProvider, parent.Context, EmptyConfig());

        graph.ReplaceConfig(child.RegistrationId, ConfigWithToken("ColorPrimary", "#ff0000"));
        var capture = graph.CaptureSubtree(parent.RegistrationId);

        capture.Nodes.Select(static node => node.Stamp.RegistrationId)
               .ShouldBe([parent.RegistrationId, child.RegistrationId]);
        capture.Nodes[0].Stamp.ParentRegistrationId.ShouldBe(0);
        capture.Nodes[1].Stamp.ParentRegistrationId.ShouldBe(parent.RegistrationId);
        capture.Nodes[1].Stamp.ConfigRevision.ShouldBe(1);
        graph.IsCurrent(capture).ShouldBeTrue();
    }

    [Fact]
    public void Topology_Changes_Invalidate_Previous_Captures()
    {
        var manager = new ThemeManager(static () => true);
        var root = CreateRootContext(manager);
        var graph = new ThemeScopeGraph(manager);
        var provider = new ThemeConfigProvider();
        var first = graph.Register(provider, root, EmptyConfig());
        var capture = graph.CaptureSubtree(first.RegistrationId);

        first.Dispose();
        using var second = graph.Register(provider, root, EmptyConfig());

        graph.IsCurrent(capture).ShouldBeFalse();
        second.RegistrationId.ShouldNotBe(capture.Nodes[0].Stamp.RegistrationId);
    }

    [Fact]
    public void Reparent_Replaces_The_Old_Edge_And_Invalidates_The_Stamp()
    {
        var manager = new ThemeManager(static () => true);
        var root = CreateRootContext(manager);
        var graph = new ThemeScopeGraph(manager);
        using var firstParent = graph.Register(new ThemeConfigProvider(), root, EmptyConfig());
        using var secondParent = graph.Register(new ThemeConfigProvider(), root, EmptyConfig());
        using var child = graph.Register(new ThemeConfigProvider(), firstParent.Context, EmptyConfig());
        var before = graph.CaptureSubtree(child.RegistrationId);

        graph.Reparent(child.RegistrationId, secondParent.Context);
        var after = graph.CaptureSubtree(child.RegistrationId);

        graph.IsCurrent(before).ShouldBeFalse();
        after.Nodes[0].Stamp.ParentRegistrationId.ShouldBe(secondParent.RegistrationId);
    }

    [Fact]
    public void Context_Keeps_One_Resource_Provider_While_Snapshots_Change()
    {
        var manager = new ThemeManager(static () => true);
        var first = CreateSnapshot();
        var second = CreateSnapshot();
        var context = new ThemeContext(manager, first, 0);
        var provider = context.ResourceProvider;
        var published = 0;
        context.Published += (_, _) => published++;

        context.Commit(second);
        context.Publish();

        context.Snapshot.ShouldBeSameAs(second);
        context.ResourceProvider.ShouldBeSameAs(provider);
        provider.Snapshot.ShouldBeSameAs(second);
        published.ShouldBe(1);
    }

    private static ThemeContext CreateRootContext(ThemeManager manager)
    {
        return new ThemeContext(manager, CreateSnapshot(), 0);
    }

    private static ThemeSnapshot CreateSnapshot()
    {
        var registry = TypedThemeSnapshotCacheTests.CreateRegistry();
        var input = TypedThemeSnapshotCacheTests.CreateInput(registry);
        return new ThemeCompiler().Compile(input).Snapshot!;
    }

    private static ThemeConfig EmptyConfig()
    {
        return new ThemeConfigBuilder().Build();
    }

    private static ThemeConfig ConfigWithToken(string name, string value)
    {
        return new ThemeConfigBuilder().WithToken(name, value).Build();
    }
}
