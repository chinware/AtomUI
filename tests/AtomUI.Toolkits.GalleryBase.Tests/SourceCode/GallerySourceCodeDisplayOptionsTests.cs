using AtomUI.Toolkits.GalleryBase.Configuration;
using AtomUI.Toolkits.GalleryBase.SourceCode;
using Shouldly;
using Xunit;

namespace AtomUI.Toolkits.GalleryBase.Tests.SourceCode;

public class GallerySourceCodeDisplayOptionsTests
{
    [Fact]
    public void SourceCodeDisplayOptionsKeepProviderAndEnabledState()
    {
        var provider = new EmptySnippetProvider();
        var options = new GalleryBaseOptions();

        options.SourceCodeDisplay.IsEnabled = true;
        options.SourceCodeDisplay.SnippetProvider = provider;

        options.SourceCodeDisplay.IsEnabled.ShouldBeTrue();
        options.SourceCodeDisplay.SnippetProvider.ShouldBeSameAs(provider);
    }

    private sealed class EmptySnippetProvider : IShowCaseCodeSnippetProvider
    {
        public bool TryGetSnippetGroup(ShowCaseCodeSnippetKey key, out ShowCaseCodeSnippetGroup group)
        {
            group = default!;
            return false;
        }
    }
}
