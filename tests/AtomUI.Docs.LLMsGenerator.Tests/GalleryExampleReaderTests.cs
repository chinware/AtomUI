using AtomUI.Docs.LLMsGenerator.Reader;
using Shouldly;
using Xunit;

namespace AtomUI.Docs.LLMsGenerator.Tests;

public class GalleryExampleReaderTests
{
    [Fact]
    public void ReaderSupportsSpecificShowCaseFileInSharedGalleryDirectory()
    {
        const string galleryPath =
            "controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Views/LineEditShowCase.axaml";

        var markdown = GalleryExampleReader.ReadMarkdown(TestRepository.RootPath, galleryPath, "OtpLineEdit");

        markdown.ShouldContain("SourceKey：`line-edit-otp-two-way`");
        markdown.ShouldContain("SourceKey：`line-edit-otp-form`");
        markdown.ShouldContain("SourceKey：`line-edit-otp-ant-design`");
        markdown.ShouldNotContain("SourceKey：`line-edit-basic`");
    }
}
