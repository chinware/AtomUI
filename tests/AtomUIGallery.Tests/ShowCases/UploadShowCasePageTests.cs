using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class UploadShowCasePageTests
{
    [Fact]
    public void Upload_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload/Views/UploadShowCase.axaml");

        source.ShouldContain("UploadShowCaseLangResource PageSubtitle");
        source.ShouldContain("UploadShowCaseLangResource PageDescription");
        source.ShouldNotContain("UploadShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("UploadShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("UploadShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("UploadShowCaseLangResource ComponentCategory");
        source.ShouldContain("UploadShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("UploadShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("UploadShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("UploadShowCaseLangResource ScenarioDesignToken");
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
        source.ShouldContain("Description=\"{gallery:UploadShowCaseLangResource PageDescription}\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(12);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(12);
        CountOccurrences(source, "DataTemplate x:DataType=\"vm:UploadViewModel\"").ShouldBe(12);
        source.ShouldContain("UploadShowCaseLangResource UploadByClickingTitle");
        source.ShouldContain("UploadShowCaseLangResource PicturesWallTitle");
        source.ShouldContain("UploadShowCaseLangResource FileAndDirectoryTitle");
        source.ShouldContain("UploadShowCaseLangResource ScrollableListTitle");
        source.ShouldContain("UploadShowCaseLangResource SuccessAutoRemoveTitle");
        source.ShouldContain("UploadShowCaseLangResource UploadPngOnlyTitle");
        source.ShouldContain("<atom:UploadTrigger SourceKind=\"Files\"");
        source.ShouldContain("<atom:UploadTrigger SourceKind=\"Directories\"");
        Regex.IsMatch(
            source,
            "<atom:Upload Name=\"DragAndDropUpload\"[^>]*IsMultipleEnabled=\"True\"[^>]*>.*" +
            "<atom:UploadDropZone>\\s*<atom:UploadDefaultDropArea\\s*/>\\s*</atom:UploadDropZone>",
            RegexOptions.CultureInvariant | RegexOptions.Singleline).ShouldBeTrue();
        Regex.IsMatch(
            source,
            "<atom:UploadDropZone>\\s*<atom:UploadDefaultDropArea\\s*/>\\s*</atom:UploadDropZone>",
            RegexOptions.CultureInvariant).ShouldBeTrue();
        source.ShouldContain("AllowedFileTypes=\"{Binding PngFileTypes}\"");
        source.ShouldNotContain("Accepts=");
        source.ShouldContain("Files=\"{Binding DefaultFiles}\"");
        source.ShouldContain("Files=\"{Binding ScrollableUploadFiles}\"");
        source.ShouldNotContain("DefaultTaskList");
        source.ShouldNotContain("IsUploadDirectoryEnabled");
        source.ShouldNotContain("IsShowUploadTrigger");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void Upload_ShowCase_Uses_The_Stream_Source_Metadata_Contract()
    {
        var viewSource = ReadRepoFile(
            "controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload/Views/UploadShowCase.axaml.cs");
        var viewModelSource = ReadRepoFile(
            "controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload/ViewModels/UploadViewModel.cs");

        viewSource.ShouldNotContain("FilePath");
        viewSource.ShouldContain("Path.GetExtension(fileInfo.Name)");
        viewSource.ShouldContain("fileInfo.Size ?? 0");
        viewSource.ShouldContain("fileInfo.Path ??");
        viewModelSource.ShouldContain("IReadOnlyList<FilePickerFileType> PngFileTypes");
        viewModelSource.ShouldContain("Patterns = [\"*.png\"]");
    }

    [Fact]
    public void Upload_ShowCase_Removes_Orphaned_SubShowCase_Files()
    {
        File.Exists(GetRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload/Views/UploadBasicShowCase.axaml")).ShouldBeFalse();
        File.Exists(GetRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload/Views/UploadBasicShowCase.axaml.cs")).ShouldBeFalse();
        File.Exists(GetRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload/Views/UploadPicturesShowCase.axaml")).ShouldBeFalse();
        File.Exists(GetRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload/Views/UploadPicturesShowCase.axaml.cs")).ShouldBeFalse();
        File.Exists(GetRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload/Views/UploadConstraintsShowCase.axaml")).ShouldBeFalse();
        File.Exists(GetRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload/Views/UploadConstraintsShowCase.axaml.cs")).ShouldBeFalse();
    }

    [Fact]
    public void Upload_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload/Views/UploadShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/UploadShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractUploadExampleItems(source));
        CountOccurrences(normalized, "<gallery:ShowCaseItem").ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    private static string ExtractUploadExampleItems(string source)
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
        return ShowCaseSnapshotMarkup.Normalize(StripUploadBehaviorMarkup(source));
    }

    private static string StripUploadBehaviorMarkup(string source)
    {
        var normalized = Regex.Replace(
            source,
            "\\s*UploadTransport=\"\\{Binding UploadTransport\\}\"",
            string.Empty,
            RegexOptions.CultureInvariant);

        normalized = Regex.Replace(
            normalized,
            "\\s*UploadTaskFailed=\"HandleUploadFailed\"",
            string.Empty,
            RegexOptions.CultureInvariant);

        normalized = Regex.Replace(
            normalized,
            "\\s*UploadTaskCompleted=\"HandleUploadCompleted\"",
            string.Empty,
            RegexOptions.CultureInvariant);

        normalized = Regex.Replace(
            normalized,
            "\\s*UploadTaskAboutToScheduling=\"Handle(?:Image|Png)UploadAboutToScheduling\"",
            string.Empty,
            RegexOptions.CultureInvariant);

        return normalized;
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
