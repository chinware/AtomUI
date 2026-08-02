using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Theme;

public class TokenResourceBindingLifetimeTests
{
    private static readonly Regex DiscardedBindingPattern = new(
        @"^\s*(?:TokenResourceBinder\.Create(?:Global|Control)TokenBinding\(.*|this\.Configure(?:Motion|WaveSpirit)BindingStyle\(\));\s*$",
        RegexOptions.Compiled);

    [Fact]
    public void Runtime_Token_Bindings_Are_Not_Discarded()
    {
        var sourceRoot = Path.Combine(GetRepositoryRoot(), "src");
        var offenders = Directory
                        .EnumerateFiles(sourceRoot, "*.cs", SearchOption.AllDirectories)
                        .Where(path => !path.Contains("/GeneratedFiles/", StringComparison.Ordinal))
                        .SelectMany(path => File.ReadLines(path)
                                                .Select((line, index) => new
                                                {
                                                    Path = path,
                                                    Line = line,
                                                    Number = index + 1
                                                }))
                        .Where(item => DiscardedBindingPattern.IsMatch(item.Line))
                        .Select(item => $"{Path.GetRelativePath(GetRepositoryRoot(), item.Path)}:{item.Number}")
                        .ToArray();

        offenders.ShouldBeEmpty(
            "runtime Token bindings subscribe to resource hosts and must have an explicit release path");
    }

    private static string GetRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "AtomUI.slnx")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Unable to locate repository root.");
    }
}
