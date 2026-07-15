using Xunit;

namespace AtomUI.Core.Tests.Theme;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class ThemeConfigProviderTestCollection
{
    public const string Name = "Theme runtime state tests";
}
