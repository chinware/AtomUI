using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class ImagePreviewerShowCasePageTests
{
    [Fact]
    public void ImagePreviewer_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source      = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/ImagePreviewer/Views/ImagePreviewerShowCase.axaml");
        var viewModelCs = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/ImagePreviewer/ViewModels/ImagePreviewerViewModel.cs");

        source.ShouldContain("ImagePreviewerShowCaseLangResource PageSubtitle");
        source.ShouldContain("ImagePreviewerShowCaseLangResource PageDescription");
        source.ShouldNotContain("ImagePreviewerShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("ImagePreviewerShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("ImagePreviewerShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("ImagePreviewerShowCaseLangResource ComponentCategory");
        source.ShouldContain("ImagePreviewerShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("ImagePreviewerShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("ImagePreviewerShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("ImagePreviewerShowCaseLangResource ScenarioDesignToken");
        source.ShouldNotContain("Tag=\"Examples\"");
        source.ShouldNotContain("Tag=\"Api\"");
        source.ShouldNotContain("Tag=\"DesignToken\"");
        source.ShouldContain("<gallery:GalleryStickyTabsHost");
        source.ShouldContain("StickyContentPadding=\"28,0,28,0\"");
        source.ShouldNotContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        source.ShouldNotContain("<ContentControl Name=\"ScenarioContentHost\">");
        source.ShouldContain("Name=\"ExamplesContent\"");
        source.ShouldContain("IsScrollEnabled=\"False\"");
        source.ShouldContain("IsDeferredLoadingEnabled=\"True\"");
        source.ShouldContain("InitialDeferredLoadItemCount=\"4\"");
        source.ShouldContain("DeferredLoadBatchSize=\"2\"");
        source.ShouldContain("ContentMargin=\"28,10,28,28\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-label\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-value\"");
        CountOccurrences(source, "Classes=\"info-label\"").ShouldBe(0);
        CountOccurrences(source, "Classes=\"info-value\"").ShouldBe(0);
        source.ShouldNotContain("LineHeight=\"22\"");
        source.ShouldContain("Description=\"{gallery:ImagePreviewerShowCaseLangResource PageDescription}\"");
        CountShowCaseItemElements(source).ShouldBe(8);
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(8);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(8);
        CountOccurrences(source, "DataTemplate x:DataType=\"vm:ImagePreviewerViewModel\"").ShouldBe(8);
        source.ShouldContain("ImagePreviewerShowCaseLangResource BasicUsageTitle");
        source.ShouldContain("ImagePreviewerShowCaseLangResource RemoteImageLoadingTitle");
        source.ShouldContain("ImagePreviewerShowCaseLangResource TwentyRemoteImagesTitle");
        source.ShouldContain("ImagePreviewerShowCaseLangResource MultipleImagePreviewTitle");
        source.ShouldContain("IsOccupyEntireRow=\"True\"");
        source.ShouldContain("ItemsSource=\"{Binding DefaultImages}\"");
        source.ShouldContain("ItemsSource=\"{Binding RemoteImages}\"");
        source.ShouldNotContain("<atom:SkeletonImage IsActive=\"True\"");
        source.ShouldContain("ItemsSource=\"{Binding FallbackImages}\"");
        source.ShouldContain("ItemsSource=\"{Binding ThreeImages}\"");
        source.ShouldContain("CoverIndex=\"1\"");
        source.ShouldNotContain("CoverSource");
        source.ShouldContain("ItemsSource=\"{Binding TwoImages}\"");
        source.ShouldContain("ItemsSource=\"{Binding TwentyRemoteImages}\"");
        source.ShouldContain("ItemsSource=\"{Binding RapidImages}\"");
        source.ShouldContain("ImageSwitchMode=\"{Binding RapidSwitchMode}\"");
        source.ShouldContain("ImagePreviewerShowCaseLangResource RapidSwitchTitle");
        source.ShouldNotContain("SourceUri=\"");
        source.ShouldNotContain("SourceUris=\"");
        source.ShouldNotContain("FallbackSourceUri=\"");
        source.ShouldNotContain("MaxConcurrentLoads=");
        source.ShouldContain("PreloadCount=\"1\"");
        viewModelCs.ShouldContain("ImagePreviewItem");
        viewModelCs.ShouldContain("ImageLoadSource.FromUri");
        viewModelCs.ShouldNotContain("IImagePreviewSource");
        viewModelCs.ShouldNotContain("UriImagePreviewSource");
        viewModelCs.ShouldContain("https://zos.alipayobjects.com/rmsportal/jkjgkEfvpUPVyRjUImniVslZfWPnJuuZ.png");
        viewModelCs.ShouldContain("TwentyRemoteImages");
        viewModelCs.ShouldContain("https://picsum.photos/id/20/600/400");
        viewModelCs.ShouldContain("https://picsum.photos/id/39/600/400");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void ImagePreviewer_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/ImagePreviewer/Views/ImagePreviewerShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/ImagePreviewerShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractImagePreviewerExampleItems(source));
        CountOccurrences(normalized, "<gallery:ShowCaseItem").ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    private static string ExtractImagePreviewerExampleItems(string source)
    {
        const string firstItemMarker  = "<gallery:ShowCaseItem";
        const string panelCloseMarker = "</gallery:ShowCasePanel>";

        var firstItemStart = source.IndexOf(firstItemMarker, StringComparison.Ordinal);
        firstItemStart.ShouldBeGreaterThanOrEqualTo(0);

        var panelCloseStart = source.IndexOf(panelCloseMarker, firstItemStart, StringComparison.Ordinal);
        panelCloseStart.ShouldBeGreaterThan(firstItemStart);

        return source[firstItemStart..panelCloseStart];
    }

    private static string NormalizeMarkup(string source)
    {
        return ShowCaseSnapshotMarkup.Normalize(source);
    }

    private static int CountShowCaseItemElements(string source)
    {
        return Regex.Matches(source, @"<gallery:ShowCaseItem(\s|>)").Count;
    }

    private static string ComputeSha256(string source)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(source));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private static string ReadSnapshotHash(string source)
    {
        return source
            .Split('\n')
            .First(line => line.StartsWith("sha256:", StringComparison.Ordinal))
            .Split(':', 2)[1]
            .Trim();
    }

    private static int ReadSnapshotCount(string source)
    {
        return int.Parse(source
            .Split('\n')
            .First(line => line.StartsWith("count:", StringComparison.Ordinal))
            .Split(':', 2)[1]
            .Trim());
    }

    private static int CountOccurrences(string source, string value)
    {
        var count      = 0;
        var startIndex = 0;
        while (true)
        {
            var matchIndex = source.IndexOf(value, startIndex, StringComparison.Ordinal);
            if (matchIndex < 0)
            {
                return count;
            }

            count++;
            startIndex = matchIndex + value.Length;
        }
    }

    private static string ReadRepoFile(string relativePath)
    {
        var path = GetRepoFile(relativePath);
        File.Exists(path).ShouldBeTrue($"Expected repository file to exist: {relativePath}");
        return File.ReadAllText(path);
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

        return Path.Combine(AppContext.BaseDirectory, relativePath);
    }
}
