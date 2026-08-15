using AtomUI.Registration;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Registration;

public sealed class ControlPackageRegistrationEntryAttributeTests
{
    [Fact]
    public void Attribute_Is_A_Single_Method_Level_Declaration()
    {
        typeof(ControlPackageRegistrationEntryAttribute).IsSealed.ShouldBeTrue();

        var usage = typeof(ControlPackageRegistrationEntryAttribute)
                    .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
                    .Cast<AttributeUsageAttribute>()
                    .ShouldHaveSingleItem();
        usage.ValidOn.ShouldBe(AttributeTargets.Method);
        usage.AllowMultiple.ShouldBeFalse();
        usage.Inherited.ShouldBeFalse();
    }
}
