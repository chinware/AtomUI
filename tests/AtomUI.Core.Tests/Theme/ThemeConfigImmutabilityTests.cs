using AtomUI.Theme.Configuration;
using AtomUI.Theme.Schema;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

public class ThemeConfigImmutabilityTests
{
    [Fact]
    public void Build_Defensively_Copies_All_Nested_Input()
    {
        var algorithms = new[] { "Default", "Dark" };
        var controlAlgorithms = new[] { "Default", "Compact" };
        var identity = new ControlTokenIdentity("AtomUI", "Button");
        var controlBuilder = new ControlThemeConfigBuilder()
                             .WithAlgorithm(ControlAlgorithmMode.Custom)
                             .WithAlgorithms(controlAlgorithms)
                             .WithToken("ColorPrimary", "#1677FF");
        var configBuilder = new ThemeConfigBuilder()
                            .WithAlgorithms(algorithms)
                            .WithToken("BorderRadius", "6")
                            .WithControl(identity, controlBuilder.Build());

        var config = configBuilder.Build();

        algorithms[0] = "Changed";
        controlAlgorithms[0] = "Changed";
        controlBuilder.WithToken("ColorPrimary", "#000000");
        configBuilder.WithToken("BorderRadius", "99");

        config.Algorithms.ShouldBe(["Default", "Dark"]);
        config.Tokens["BorderRadius"].ShouldBe("6");
        config.Controls[identity].Algorithms.ShouldBe(["Default", "Compact"]);
        config.Controls[identity].Tokens["ColorPrimary"].ShouldBe("#1677FF");
    }

    [Fact]
    public void Default_Config_Is_An_Immutable_Inheriting_Value()
    {
        var config = new ThemeConfigBuilder().Build();

        config.Inherit.ShouldBeTrue();
        config.Algorithms.ShouldBeNull();
        config.Tokens.ShouldBeEmpty();
        config.Controls.ShouldBeEmpty();
    }
}
