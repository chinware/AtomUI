using AtomUI.Theme.Resources;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

[Collection(ThemeConfigProviderTestCollection.Name)]
public class TokenResourceExtensionTests
{
    [Fact]
    public void Shared_Extension_Returns_A_Global_Key_Without_Inspecting_The_Parent_Stack()
    {
        var key = new SharedTokenResourceExtension(SharedTokenKind.ColorPrimary)
                  .ProvideValue(new ThrowingServiceProvider())
                  .ShouldBeOfType<DynamicResourceExtension>()
                  .ResourceKey;

        key.ShouldBe(SharedTokenKind.ColorPrimary);
    }

    [Fact]
    public void Ambient_Control_Token_Scope_Type_Is_Removed()
    {
        File.Exists(GetRepoFile("src/AtomUI.Core/Theme/Resources/ControlTokenScope.cs"))
            .ShouldBeFalse();
    }

    private static string GetRepoFile(string relativePath)
    {
        var directory = AppContext.BaseDirectory;
        while (directory is not null)
        {
            var candidate = Path.Combine(directory, relativePath);
            if (File.Exists(candidate) || Directory.Exists(candidate))
            {
                return candidate;
            }

            directory = Directory.GetParent(directory)?.FullName;
        }

        return Path.Combine(AppContext.BaseDirectory, relativePath);
    }

    private sealed class ThrowingServiceProvider : IServiceProvider
    {
        public object? GetService(Type serviceType)
        {
            throw new InvalidOperationException($"SharedTokenResource must not request '{serviceType}'.");
        }
    }
}
