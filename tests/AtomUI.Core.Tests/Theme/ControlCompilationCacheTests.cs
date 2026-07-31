using AtomUI.Theme.Compilation;
using AtomUI.Theme.Configuration;
using AtomUI.Theme.Schema;
using AtomUI.Theme.DesignTokens;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

public class ControlCompilationCacheTests
{
    [Fact]
    public async Task Different_Snapshots_Reuse_Only_Equivalent_Control_Compilations()
    {
        var button = CreateControl("Button");
        var input = CreateControl("Input");
        var registry = TypedThemeSnapshotCacheTests.CreateRegistry([button, input]);
        registry.TryGetControl(button.Identity, out button).ShouldBeTrue();
        registry.TryGetControl(input.Identity, out input).ShouldBeTrue();
        var buttonConfig = new NormalizedControlThemeConfig(
            button.Identity,
            ControlAlgorithmMode.Disabled,
            Array.Empty<ThemeAlgorithmDescriptor>(),
            Array.Empty<NormalizedTokenValue>(),
            [CreateControlToken(button, "48")]);
        var cache = new ThemeSnapshotCache();
        var compiler = new ThemeCompiler();

        var baseline = await TypedThemeSnapshotCacheTests.CompileAsync(cache,
            TypedThemeSnapshotCacheTests.CreateInput(registry),
            compiler);
        var changed = await TypedThemeSnapshotCacheTests.CompileAsync(cache,
            TypedThemeSnapshotCacheTests.CreateInput(registry, controls: [buttonConfig]),
            compiler);

        baseline.Success.ShouldBeTrue();
        changed.Success.ShouldBeTrue();
        changed.Snapshot!.Controls[button.Slot]
               .ShouldNotBeSameAs(baseline.Snapshot!.Controls[button.Slot]);
        changed.Snapshot.Controls[input.Slot]
               .ShouldBeSameAs(baseline.Snapshot.Controls[input.Slot]);
        cache.ControlCount.ShouldBe(3);
        cache.ControlRetainedBytes.ShouldBeGreaterThan(0);
    }

    private static NormalizedTokenValue CreateControlToken(
        ControlTokenDescriptor control,
        string value)
    {
        var descriptor = control.OwnTokens.ShouldHaveSingleItem();
        var parsed = descriptor.Parse(value);
        return new NormalizedTokenValue(descriptor, parsed, descriptor.Format(parsed));
    }

    private static ControlTokenDescriptor CreateControl(string id)
    {
        var token = new TokenDescriptor(
            "Height",
            0,
            TokenStage.Control,
            typeof(double),
            $"{id}.Height",
            static value => ThemeTokenValueParser.Parse<double>(value),
            static value => ThemeTokenValueFormatter.Format((double)value!),
            static builder => ((CacheControlToken)builder).Height,
            static (builder, value) => ((CacheControlToken)builder).Height = (double)value!,
            static builder => ((CacheControlToken)builder).Height);
        return new ControlTokenDescriptor(
            new ControlTokenIdentity("AtomUI", id),
            [token],
            () => new CacheControlToken(id),
            static (builder, _) => ((CacheControlToken)builder).Height = 40);
    }

    private sealed class CacheControlToken(string id) : AbstractControlDesignToken(id)
    {
        public double Height { get; set; }
    }
}
