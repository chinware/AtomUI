using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AtomUI.Desktop.Controls;
using AtomUIGallery.ShowCases.ImagePreviewer;
using AtomUI.Toolkits.GalleryBase.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

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
        source.ShouldContain("<gallery:GalleryShowCaseHost");
        source.ShouldNotContain("GalleryStickyTabsHost");
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
        source.ShouldContain("<gallery:GalleryShowCaseHost.SemanticPartsContentTemplate>");
        source.ShouldContain("Name=\"ImagePreviewerSemanticPreview\"");
        source.ShouldContain("SemanticOwnerType=\"{x:Type atom:ImagePreviewer}\"");
        source.ShouldContain("Name=\"ImagePreviewerSemanticOwner\"");
        source.ShouldContain("Name=\"ImageGroupPreviewerSemanticPreview\"");
        source.ShouldContain("SemanticOwnerType=\"{x:Type atom:ImageGroupPreviewer}\"");
        source.ShouldContain("Name=\"ImageGroupPreviewerSemanticOwner\"");
        CountOccurrences(source, "<gallery:SemanticPartDescription").ShouldBe(18);
        source.ShouldContain("Path=\"image\"");
        source.ShouldContain("Path=\"cover\"");
        source.ShouldContain("Path=\"popup.root\"");
        source.ShouldContain("Path=\"popup.mask\"");
        source.ShouldContain("Path=\"popup.body\"");
        source.ShouldContain("Path=\"popup.footer\"");
        source.ShouldContain("Path=\"popup.actions\"");
        source.ShouldContain("Path=\"popup.close\"");
        CountShowCaseItemElements(source).ShouldBe(10);
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(10);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(10);
        CountOccurrences(source, "DataTemplate x:DataType=\"vm:ImagePreviewerViewModel\"").ShouldBe(11);
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
        source.ShouldContain("CoverIndex=\"{Binding RapidCurrentIndex}\"");
        source.ShouldContain("ImageSwitchMode=\"{Binding RapidSwitchMode}\"");
        source.ShouldContain("ImagePreviewerShowCaseLangResource RapidSwitchTitle");
        source.ShouldContain("ImagePreviewerShowCaseLangResource SamePathReplacementTitle");
        source.ShouldContain("ItemsSource=\"{Binding SamePathImages}\"");
        source.ShouldContain("Command=\"{Binding ReplaceSamePathCommand}\"");
        source.ShouldNotContain("SourceUri=\"");
        source.ShouldNotContain("SourceUris=\"");
        source.ShouldNotContain("FallbackSourceUri=\"");
        source.ShouldNotContain("MaxConcurrentLoads=");
        source.ShouldContain("PreloadCount=\"1\"");
        source.ShouldContain("ImagePreviewerShowCaseLangResource SemanticPartStyleTitle");
        source.ShouldContain("ImagePreviewerShowCaseLangResource SemanticPartStyleDescription");
        source.ShouldContain("SourceKey=\"imagepreviewer-semantic-part\"");
        source.ShouldContain("Padding=\"4\"");
        source.ShouldContain("CornerRadius=\"8\"");
        source.ShouldContain("BorderThickness=\"2\"");
        source.ShouldContain("BorderBrush=\"#A594F9\"");
        source.ShouldContain("ClipToBounds=\"True\"");
        source.ShouldContain("ItemsSource=\"{Binding GrayscaleImages}\"");
        // 上游 styles.image 的 borderRadius 经专用 Semantic Style 落到 image 部件，不得回退代码定制。
        // 属性必须写限定名 Border.CornerRadius（AddOwner 的 public 声明方）：非限定名会解析到 internal
        // renderer 类型自身的属性字段，运行时抛 FieldAccessException（跨程序集 ldsfld 访问器检查）。
        source.ShouldContain("<atom:ImagePreviewerImageStyle x:SetterTargetType=\"atom:ImagePreviewRenderer\">");
        CountOccurrences(source, "<atom:ImagePreviewerImageStyle x:SetterTargetType=\"atom:ImagePreviewRenderer\">").ShouldBe(2);
        CountOccurrences(source, "<Setter Property=\"Border.CornerRadius\" Value=\"4\" />").ShouldBe(2);
        source.ShouldNotContain("<Setter Property=\"CornerRadius\" Value=\"4\" />");
        source.ShouldContain("Classes=\"semantic-styles-demo\"");
        source.ShouldContain("Classes=\"semantic-styles-filled-demo\"");
        source.ShouldNotContain(".semantic-image");
        viewModelCs.ShouldContain("ImagePreviewItem");
        viewModelCs.ShouldContain("ImageSource.Parse");
        viewModelCs.ShouldContain("GrayscaleImages");
        viewModelCs.ShouldContain("ApplyGrayscale");
        viewModelCs.ShouldContain("SKColorFilter.CreateColorMatrix");
        viewModelCs.ShouldContain("new FileImageSource(_replacementFilePath!)");
        viewModelCs.ShouldContain("File.Move(nextPath, _replacementFilePath, overwrite: true)");
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

    [Fact]
    public void ImagePreviewer_ShowCase_Semantic_Example_Materializes_Without_Access_Violation()
    {
        // Semantic Style 经 x:SetterTargetType 在 internal ImagePreviewRenderer 上解析
        // CornerRadiusProperty；internal 属性字段会让 XAML 编译器生成的 ldsfld 在运行时抛
        // FieldAccessException（跨程序集访问器检查），必须实例化真实页面走一遍 deferred 物化。
        AvaloniaTestApp.EnsureInitialized();
        var page = new ImagePreviewerShowCase();

        var window = new AvaloniaWindow { Width = 900, Height = 800, Content = page };
        window.Show();
        page.ApplyTemplate();
        window.UpdateLayout();
        Dispatcher.UIThread.RunJobs();

        try
        {
            var showcaseItems = page.GetVisualDescendants().OfType<ShowCaseItem>().ToList();
            showcaseItems.ShouldNotBeEmpty();

            foreach (var item in showcaseItems.Where(i => i.IsDeferredContentEnabled))
            {
                item.MaterializeDeferredContent();
            }
            window.UpdateLayout();
            Dispatcher.UIThread.RunJobs();

            // 物化本身不抛 FieldAccessException 即通过；再校验语义示例（两个 semantic-styles-* 预览器）的
            // 圆角确实落到渲染器。全页有多个示例的渲染器（默认无圆角），只断言语义示例两个 owner 内的。
            var semanticOwners = page.GetVisualDescendants()
                                     .OfType<AtomUI.Desktop.Controls.ImagePreviewer>()
                                     .Where(p => p.Classes.Contains("semantic-styles-demo") ||
                                                 p.Classes.Contains("semantic-styles-filled-demo"))
                                     .ToList();
            semanticOwners.Count.ShouldBe(2);
            var renderers = semanticOwners
                .SelectMany(o => o.GetVisualDescendants())
                .OfType<Control>()
                .Where(c => c.Classes.Contains("semantic-image"))
                .ToList();
            renderers.ShouldNotBeEmpty();
            renderers.ShouldAllBe(r => r.GetValue(Border.CornerRadiusProperty).Equals(new CornerRadius(4)));
        }
        finally
        {
            window.Close();
        }
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
