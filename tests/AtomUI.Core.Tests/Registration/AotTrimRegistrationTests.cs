using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Registration;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Registration;

public sealed class AotTrimRegistrationTests
{
    [Fact]
    public void Generated_Registration_Uses_The_Linker_Feature_Switch()
    {
        var property = typeof(AotTrimRegistration).GetProperty(
            nameof(AotTrimRegistration.IsEnabled),
            BindingFlags.Public | BindingFlags.Static);

        property.ShouldNotBeNull();
        var attribute = property.GetCustomAttribute<FeatureSwitchDefinitionAttribute>();
        attribute.ShouldNotBeNull();
        attribute.SwitchName.ShouldBe("AtomUI.AotTrimRegistration.Enabled");
    }

    [Fact]
    public void Generated_Registration_Feature_Switch_Is_Cached_Once()
    {
        var field = typeof(AotTrimRegistration).GetField(
            "s_isEnabled",
            BindingFlags.NonPublic | BindingFlags.Static);

        field.ShouldNotBeNull();
        field.IsInitOnly.ShouldBeTrue();
        field.FieldType.ShouldBe(typeof(bool));
    }

    [Fact]
    public void Generated_Registration_Is_Disabled_When_The_Switch_Is_Not_Set()
    {
        AppContext.TryGetSwitch(
            "AtomUI.AotTrimRegistration.Enabled",
            out var configuredValue).ShouldBeFalse();
        configuredValue.ShouldBeFalse();
        AotTrimRegistration.IsEnabled.ShouldBeFalse();
    }
}
