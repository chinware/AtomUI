using AtomUI.Theme.Algorithms;
using AtomUI.Theme.Configuration;
using AtomUI.Theme.Schema;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

public class ThemeConfigMergerTests
{
    [Fact]
    public void Merge_Inherits_Parent_And_Applies_Local_Precedence()
    {
        var schema = ThemeConfigTestSchema.Create();
        var defaults = Normalize(schema, false, [ThemeAlgorithm.Default], ("Alpha", "1"));
        var parent = Normalize(schema, true, [ThemeAlgorithm.Compact], ("Alpha", "2"), ("Beta", "3"));
        var local = Normalize(schema, true, null, ("Beta", "4"));

        var result = ThemeConfigMerger.Merge(defaults, parent, local);

        result.EffectiveConfig.Algorithms.Select(static item => item.Algorithm).ShouldBe([ThemeAlgorithm.Compact]);
        Value(result.EffectiveConfig, "Alpha").ShouldBe(2d);
        Value(result.EffectiveConfig, "Beta").ShouldBe(4d);
        result.ChangeSet.AlgorithmsChanged.ShouldBeFalse();
        result.ChangeSet.GlobalTokensChanged.ShouldBeTrue();
    }

    [Fact]
    public void Merge_Inherit_False_Restarts_From_Defaults_But_Changes_Are_Relative_To_Parent()
    {
        var schema = ThemeConfigTestSchema.Create();
        var defaults = Normalize(schema, false, [ThemeAlgorithm.Default], ("Alpha", "1"));
        var parent = Normalize(schema, true, [ThemeAlgorithm.Compact], ("Alpha", "2"), ("Beta", "3"));
        var local = Normalize(schema, false, null, ("Beta", "4"));

        var result = ThemeConfigMerger.Merge(defaults, parent, local);

        result.EffectiveConfig.Algorithms.Select(static item => item.Algorithm).ShouldBe([ThemeAlgorithm.Default]);
        Value(result.EffectiveConfig, "Alpha").ShouldBe(1d);
        Value(result.EffectiveConfig, "Beta").ShouldBe(4d);
        result.ChangeSet.AlgorithmsChanged.ShouldBeTrue();
        result.ChangeSet.GlobalTokensChanged.ShouldBeTrue();
    }

    [Fact]
    public void Merge_Empty_Algorithm_List_Explicitly_Replaces_Parent_With_Default()
    {
        var schema = ThemeConfigTestSchema.Create();
        var defaults = Normalize(schema, false, [ThemeAlgorithm.Default]);
        var parent = Normalize(schema, true, [ThemeAlgorithm.Compact]);
        var local = Normalize(schema, true, []);

        var result = ThemeConfigMerger.Merge(defaults, parent, local);

        result.EffectiveConfig.Algorithms.Select(static item => item.Algorithm).ShouldBe([ThemeAlgorithm.Default]);
        result.ChangeSet.AlgorithmsChanged.ShouldBeTrue();
    }

    [Theory]
    [InlineData((int)ControlAlgorithmMode.Unspecified, (int)ControlAlgorithmMode.Custom)]
    [InlineData((int)ControlAlgorithmMode.Disabled, (int)ControlAlgorithmMode.Disabled)]
    [InlineData((int)ControlAlgorithmMode.Global, (int)ControlAlgorithmMode.Global)]
    [InlineData((int)ControlAlgorithmMode.Custom, (int)ControlAlgorithmMode.Custom)]
    public void Merge_Resolves_All_Control_Algorithm_States(
        int localModeValue,
        int expectedModeValue)
    {
        var localMode = (ControlAlgorithmMode)localModeValue;
        var expectedMode = (ControlAlgorithmMode)expectedModeValue;
        var schema = ThemeConfigTestSchema.Create();
        var defaults = Normalize(schema, false, [ThemeAlgorithm.Default]);
        var parent = NormalizeControl(schema, true, ControlAlgorithmMode.Custom, [ThemeAlgorithm.Compact], "20");
        var localAlgorithms = localMode == ControlAlgorithmMode.Custom ? new[] { ThemeAlgorithm.Default } : null;
        var local = NormalizeControl(schema, true, localMode, localAlgorithms, "32");

        var result = ThemeConfigMerger.Merge(defaults, parent, local);

        var button = result.EffectiveConfig.Controls.Single();
        button.AlgorithmMode.ShouldBe(expectedMode);
        button.OwnTokens.Single(item => item.Descriptor.Name == "Height").Value.ShouldBe(32d);
        button.Algorithms.Select(static item => item.Algorithm).ShouldBe(
            expectedMode == ControlAlgorithmMode.Custom
                ? localMode == ControlAlgorithmMode.Custom
                    ? [ThemeAlgorithm.Default]
                    : [ThemeAlgorithm.Compact]
                : Array.Empty<ThemeAlgorithm>());
    }

    [Fact]
    public void Merge_Produces_Deterministic_Changed_Control_Order_And_NoOp_Result()
    {
        var schema = ThemeConfigTestSchema.Create();
        var defaults = Normalize(schema, false, [ThemeAlgorithm.Default]);
        var parent = NormalizeControl(schema, true, ControlAlgorithmMode.Disabled, null, "20");
        var local = NormalizeControl(schema, true, ControlAlgorithmMode.Unspecified, null, "32");

        var changed = ThemeConfigMerger.Merge(defaults, parent, local);
        var noOpConfig = Normalize(new ThemeConfigBuilder().WithInherit(true).Build(), schema);
        var noOp = ThemeConfigMerger.Merge(defaults, changed.EffectiveConfig, noOpConfig);

        changed.ChangeSet.ChangedControls.ShouldBe([ThemeConfigTestSchema.ButtonIdentity]);
        changed.ChangeSet.IsEmpty.ShouldBeFalse();
        noOp.ChangeSet.IsEmpty.ShouldBeTrue();
        noOp.EffectiveConfig.ShouldBe(changed.EffectiveConfig);
        noOp.EffectiveConfig.Fingerprint.ShouldBe(changed.EffectiveConfig.Fingerprint);
    }

    private static NormalizedThemeConfig Normalize(
        ThemeSchemaRegistry schema,
        bool inherit,
        ThemeAlgorithm[]? algorithms,
        params (string Name, string Value)[] tokens)
    {
        var builder = new ThemeConfigBuilder().WithInherit(inherit);
        if (algorithms is not null)
        {
            builder.WithAlgorithms(algorithms);
        }

        foreach (var token in tokens)
        {
            builder.WithToken(token.Name, token.Value);
        }

        return Normalize(builder.Build(), schema);
    }

    private static NormalizedThemeConfig NormalizeControl(
        ThemeSchemaRegistry schema,
        bool inherit,
        ControlAlgorithmMode mode,
        ThemeAlgorithm[]? algorithms,
        string height)
    {
        var controlBuilder = new ControlThemeConfigBuilder()
                             .WithAlgorithm(mode)
                             .WithToken("Height", height);
        if (algorithms is not null)
        {
            controlBuilder.WithAlgorithms(algorithms);
        }

        var config = new ThemeConfigBuilder()
                     .WithInherit(inherit)
                     .WithControl(ThemeConfigTestSchema.ButtonIdentity, controlBuilder.Build())
                     .Build();
        return Normalize(config, schema);
    }

    private static NormalizedThemeConfig Normalize(ThemeConfig config, ThemeSchemaRegistry schema)
    {
        return ThemeConfigNormalizer.Normalize(config, schema).Config.ShouldNotBeNull();
    }

    private static double Value(NormalizedThemeConfig config, string name)
    {
        return (double)config.GlobalTokens.Single(item => item.Descriptor.Name == name).Value!;
    }
}
