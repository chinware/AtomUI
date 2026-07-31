using System.Reflection;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Flyouts;

public class FlyoutResourceBindingLifetimeTests
{
    [Fact]
    public void Flyout_Does_Not_Discard_Global_Token_Binding_Disposables()
    {
        var source = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Flyouts/Flyout.cs"));

        Regex.IsMatch(source, @"^\s*TokenResourceBinder\.CreateGlobalTokenBinding\(this,.*;\s*$", RegexOptions.Multiline)
             .ShouldBeFalse("global resource bindings subscribe to application styles and must have a release path");
    }

    [Fact]
    public void Flyout_Has_Explicit_Global_Resource_Binding_Lifecycle()
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;

        typeof(Flyout).GetMethod("EnsureGlobalResourceBindings", flags).ShouldNotBeNull();
        typeof(Flyout).GetMethod("ReleaseGlobalResourceBindings", flags).ShouldNotBeNull();
    }

    private static string GetRepoFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"Could not find repository file: {relativePath}");
    }
}
