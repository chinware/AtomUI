using AtomUI.Core.Tests.Theme;
using AtomUI.Theme;
using AtomUI.Theme.Compilation;
using AtomUI.Theme.Configuration;
using AtomUI.Theme.Tokens;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme.Performance;

[Collection(ThemeConfigProviderTestCollection.Name)]
public class ThemeScopeAndTransactionPerformanceTests
{
    [Fact]
    public void Ten_Nested_Scopes_Keep_One_Stable_Context_And_Resource_Provider_Per_Scope()
    {
        var manager = CreateInitializedManager();
        var providers = new List<ThemeConfigProvider>(10);
        Control child = new Border();
        for (var index = 9; index >= 0; index--)
        {
            var provider = new ThemeConfigProvider
            {
                Config = new ThemeConfigBuilder()
                         .WithToken(nameof(DesignToken.BorderRadius), (index + 1).ToString())
                         .Build(),
                Child = child
            };
            providers.Insert(0, provider);
            child = provider;
        }

        using var root = new LogicalRoot();
        root.SetValue(ThemeScope.ContextProperty, manager.RootContext);
        root.Child = child;

        var contexts = providers
                       .Select(provider => provider.GetValue(ThemeScope.ContextProperty).ShouldNotBeNull())
                       .ToArray();
        contexts.Distinct(ReferenceEqualityComparer.Instance).Count().ShouldBe(10);
        foreach (var provider in providers)
        {
            provider.Resources.MergedDictionaries
                    .OfType<AtomUI.Theme.Resources.ThemeTokenResourceProvider>()
                    .ShouldHaveSingleItem();
        }
        manager.ScopeGraph.CaptureAll().Nodes.Count.ShouldBe(10);
        contexts[^1].Snapshot.Global<CornerRadius>(nameof(DesignToken.BorderRadius))
                .ShouldBe(new CornerRadius(10));
    }

    [Fact]
    public async Task One_Hundred_Pending_Updates_Coalesce_To_The_Latest_Request()
    {
        var firstPrepared = CreatePrepared("Theme-0", "#1677ff");
        var latestPrepared = CreatePrepared("Theme-100", "#722ed1");
        var firstStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseFirst = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var preparedThemes = new List<string>();
        var manager = new ThemeManager(static () => true, async (request, _, _) =>
        {
            preparedThemes.Add(request.ThemeId);
            if (request.ThemeId == "Theme-0")
            {
                firstStarted.SetResult();
                await releaseFirst.Task;
                return firstPrepared;
            }
            return latestPrepared;
        });

        var transitions = new List<Task<ThemeTransitionResult>>(101)
        {
            ApplyAsync(manager, Request("Theme-0"))
        };
        await firstStarted.Task;
        for (var index = 1; index <= 100; index++)
        {
            transitions.Add(ApplyAsync(manager, Request($"Theme-{index}")));
        }
        releaseFirst.SetResult();

        var results = await Task.WhenAll(transitions);

        results.Take(100).ShouldAllBe(result => result.Status == ThemeTransitionStatus.Superseded);
        results[^1].Status.ShouldBe(ThemeTransitionStatus.Committed);
        preparedThemes.ShouldBe(["Theme-0", "Theme-100"]);
        manager.CurrentTheme!.ThemeId.ShouldBe("Theme-100");
    }

    [Fact]
    public async Task Scope_Detach_During_Prepare_Rejects_The_Stale_Topology()
    {
        await HeadlessTestApp.RunAsync(async () =>
        {
            var initial = CreatePrepared("Initial", "#1677ff");
            var next = CreatePrepared("Next", "#52c41a");
            var nextStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var releaseNext = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var manager = new ThemeManager(static () => true, async (request, _, _) =>
            {
                if (request.ThemeId == "Next")
                {
                    nextStarted.SetResult();
                    await releaseNext.Task;
                    return next;
                }
                return initial;
            });
            (await ApplyAsync(manager, Request("Initial"))).Status.ShouldBe(ThemeTransitionStatus.Committed);
            using var root = new LogicalRoot();
            root.SetValue(ThemeScope.ContextProperty, manager.RootContext);
            root.Child = new ThemeConfigProvider
            {
                Config = new ThemeConfigBuilder().Build(),
                Child = new Border()
            };

            var transition = ApplyAsync(manager, Request("Next"));
            await nextStarted.Task;
            root.Child = null;
            releaseNext.SetResult();

            (await transition).Status.ShouldBe(ThemeTransitionStatus.Superseded);
            manager.CurrentTheme!.ThemeId.ShouldBe("Initial");
        });
    }

    private static ThemeManager CreateInitializedManager()
    {
        var prepared = CreatePrepared("Initial", "#1677ff");
        var manager = new ThemeManager(
            static () => true,
            (_, _, _) => ValueTask.FromResult(prepared));
        manager.ApplyThemeAsync(new ThemeRequest("Initial", null, ThemeTransitionReason.Startup))
               .GetAwaiter()
               .GetResult()
               .Status.ShouldBe(ThemeTransitionStatus.Committed);
        return manager;
    }

    private static ThemeTransactionPreparation CreatePrepared(string themeId, string color)
    {
        var registry = TypedThemeSnapshotCacheTests.CreateRegistry();
        registry.TryGetGlobalToken(nameof(DesignToken.ColorPrimary), out var descriptor).ShouldBeTrue();
        var parsed = descriptor!.Parse(color);
        var token = new NormalizedTokenValue(
            descriptor,
            parsed,
            descriptor.Format(parsed));
        var input = TypedThemeSnapshotCacheTests.CreateInput(registry, globalTokens: [token]);
        var snapshot = new ThemeCompiler().Compile(input).Snapshot!;
        return ThemeTransactionPreparation.Succeeded(
            snapshot,
            ThemeSnapshotCacheKey.Create(input));
    }

    private static ThemeRequest Request(string themeId)
    {
        return new ThemeRequest(themeId, null, ThemeTransitionReason.UserRequest);
    }

    private static Task<ThemeTransitionResult> ApplyAsync(
        ThemeManager manager,
        ThemeRequest request)
    {
        return manager.ApplyThemeAsync(request, TestContext.Current.CancellationToken);
    }

    private sealed class LogicalRoot : Decorator, ILogicalRoot, IDisposable
    {
        public void Dispose()
        {
            Child = null;
            ClearValue(ThemeScope.ContextProperty);
        }
    }
}
