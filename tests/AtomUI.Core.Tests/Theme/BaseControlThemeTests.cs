using System.Reflection;
using AtomUI.Theme;
using AtomUI.Theme.Resources;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

public class BaseControlThemeTests
{
    [Fact]
    public void Template_Parent_Bindings_Do_Not_Accept_String_Paths()
    {
        var stringPathOverloads = typeof(BaseControlTheme)
                                  .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                                  .Where(static method =>
                                      method.Name == "CreateTemplateParentBinding" &&
                                      method.GetParameters().Any(static parameter =>
                                          parameter.ParameterType == typeof(string)))
                                  .ToArray();

        stringPathOverloads.ShouldBeEmpty();
    }
}
