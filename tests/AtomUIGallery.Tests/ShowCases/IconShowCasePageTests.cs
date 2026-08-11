using System.Security.Cryptography;
using System.Text;
using AtomUI.Desktop.Controls;
using AtomUI.Toolkits.GalleryBase.Controls;
using AtomUIGallery.ShowCases.Icon;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaButton = Avalonia.Controls.Button;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUIGallery.Tests.ShowCases;

public class IconShowCasePageTests
{
    [Fact]
    public void Icon_ShowCase_Uses_Document_Layout_With_Lazy_Icon_Galleries()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Icon/Views/IconShowCase.axaml");

        source.ShouldContain("IconShowCaseLangResource PageSubtitle");
        source.ShouldContain("IconShowCaseLangResource PageDescription");
        source.ShouldNotContain("IconShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("IconShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("IconShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("IconShowCaseLangResource ComponentCategory");
        source.ShouldContain("IconShowCaseLangResource ComponentStatusStable");
        source.ShouldContain("IconShowCaseLangResource P2HeaderOutlined");
        source.ShouldContain("IconShowCaseLangResource P2HeaderFilled");
        source.ShouldContain("IconShowCaseLangResource P2HeaderTwoTone");
        source.ShouldContain("Tag=\"Outlined\"");
        source.ShouldContain("Tag=\"Filled\"");
        source.ShouldContain("Tag=\"TwoTone\"");
        source.ShouldContain("RowDefinitions=\"Auto,Auto,*\"");
        source.ShouldContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        source.ShouldContain("Grid.Row=\"1\"");
        source.ShouldContain("<ContentControl Name=\"ScenarioContentHost\"");
        source.ShouldContain("Grid.Row=\"2\"");
        source.ShouldContain("Margin=\"28,10,28,28\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-label\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-value\"");
        CountOccurrences(source, "Classes=\"info-label\"").ShouldBe(0);
        CountOccurrences(source, "Classes=\"info-value\"").ShouldBe(0);
        source.ShouldNotContain("LineHeight=\"22\"");
        source.ShouldContain("Description=\"{gallery:IconShowCaseLangResource PageDescription}\"");
        source.ShouldNotContain("<gallery:GalleryStickyTabsHost");
        source.ShouldNotContain("<atom:ScrollViewer");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain("<gallery:IconGallery IconThemeType=");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void Icon_ShowCase_Lazy_Loads_IconGallery_For_Selected_Theme()
    {
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Icon/Views/IconShowCase.axaml.cs");

        codeBehindSource.ShouldContain("new GalleryShowCaseScenarioController");
        codeBehindSource.ShouldContain("_scenarioController.Attach(DataContext)");
        codeBehindSource.ShouldContain("_scenarioController.UpdateDataContext(DataContext)");
        codeBehindSource.ShouldContain("new IconGallery()");
        codeBehindSource.ShouldContain("IconThemeType = IconThemeType.Outlined");
        codeBehindSource.ShouldContain("IconThemeType = IconThemeType.Filled");
        codeBehindSource.ShouldContain("IconThemeType = IconThemeType.TwoTone");
        codeBehindSource.ShouldContain("VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch");
        codeBehindSource.ShouldNotContain("Height        = 640");
        codeBehindSource.ShouldContain("new GalleryShowCaseScenarioController");
        codeBehindSource.ShouldContain("_scenarioController.Detach()");
    }

    [Fact]
    public void Icon_ShowCase_Uses_Standard_Click_And_Managed_Feedback_Lifecycle()
    {
        var source = ReadRepoFile(
            "controlgallery/AtomUIGallery/ShowCases/General/Icon/Views/IconShowCase.axaml.cs");

        source.ShouldContain("AddHandler(AvaloniaButton.ClickEvent, HandleIconItemClick)");
        source.ShouldContain("await clipboard.SetTextAsync(iconName)");
        source.ShouldContain("new WindowMessageManager(topLevel)");
        source.ShouldContain("MaxItems = 1");
        source.ShouldContain("_messageManager?.Dispose()");
        source.ShouldContain("var copyRequest = ++_copyRequestSequence");
        source.ShouldContain("_copyWaitCancellation?.Cancel()");
        source.ShouldContain("var copySemaphore");
        source.ShouldContain("= _copySemaphore;");
        source.ShouldContain("await copySemaphore.WaitAsync(copyWaitCancellation.Token)");
        source.ShouldContain("copySemaphore.Release()");
        source.ShouldContain("copyRequest == _copyRequestSequence");
        source.ShouldContain("ReferenceEquals(TopLevel.GetTopLevel(this), topLevel)");
        source.ShouldContain("catch (Exception ex)");
    }

    [Fact]
    public void Icon_ShowCase_Click_Copies_Icon_Name_And_Shows_Success_Feedback()
    {
        AvaloniaTestApp.EnsureInitialized();
        var page = new IconShowCase();

        ShowInWindow(page, window =>
        {
            var item = WaitForVisual<IconInfoItem>(page);

            item.RaiseEvent(new RoutedEventArgs(AvaloniaButton.ClickEvent, item));

            WaitForClipboardText(item, item.IconName);
            var messageManager = WaitForVisual<WindowMessageManager>(window);
            messageManager.MaxItems.ShouldBe(1);
            var messageCard = WaitForVisual<MessageCard>(messageManager);
            messageCard.MessageType.ShouldBe(MessageType.Success);
            messageCard.Message.ShouldContain(item.IconName);

            window.Content = null;
            Dispatcher.UIThread.RunJobs();
            messageManager.GetVisualParent().ShouldBeNull();
        });
    }

    [Fact]
    public void Icon_ShowCase_Localization_Includes_Page_Copy()
    {
        var en = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/ShowCases/General/Icon/Localization/en-US.xlf");
        var zhCn = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/ShowCases/General/Icon/Localization/zh-CN.xlf");
        var zhTw = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/ShowCases/General/Icon/Localization/zh-TW.xlf");
        var ptBr = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/ShowCases/General/Icon/Localization/pt-BR.xlf");

        foreach (var localization in new[] { en, zhCn, zhTw, ptBr })
        {
            localization.ContainsKey("ComponentCategory").ShouldBeTrue();
            localization.ContainsKey("ComponentStatusStable").ShouldBeTrue();
            localization.ContainsKey("PageSubtitle").ShouldBeTrue();
            localization.ContainsKey("PageDescription").ShouldBeTrue();
            localization.ContainsKey("InfoNamespaceLabel").ShouldBeFalse();
            localization.ContainsKey("InfoPackageLabel").ShouldBeFalse();
            localization.ContainsKey("InfoBaseClassLabel").ShouldBeFalse();
            localization.ContainsKey("P2HeaderOutlined").ShouldBeTrue();
            localization.ContainsKey("P2HeaderFilled").ShouldBeTrue();
            localization.ContainsKey("P2HeaderTwoTone").ShouldBeTrue();
            localization.ContainsKey("IconCopySucceededFormat").ShouldBeTrue();
            localization.ContainsKey("IconCopyFailed").ShouldBeTrue();
        }
    }

    [Fact]
    public void Icon_ShowCase_Themes_Match_Approved_Gallery_Content()
    {
        var pageSource       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Icon/Views/IconShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Icon/Views/IconShowCase.axaml.cs");
        var approved         = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/IconShowCaseExamples.snapshot");

        var normalized = NormalizeIconThemeScenarios(pageSource, codeBehindSource);
        CountIconThemeScenarios(normalized).ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    private static string NormalizeIconThemeScenarios(string pageSource, string codeBehindSource)
    {
        var scenarios = new List<string>();
        AddScenarioIfPresent(scenarios, pageSource, codeBehindSource, "Outlined", "P2HeaderOutlined");
        AddScenarioIfPresent(scenarios, pageSource, codeBehindSource, "Filled", "P2HeaderFilled");
        AddScenarioIfPresent(scenarios, pageSource, codeBehindSource, "TwoTone", "P2HeaderTwoTone");
        return string.Join("\n", scenarios);
    }

    private static void AddScenarioIfPresent(
        ICollection<string> scenarios,
        string pageSource,
        string codeBehindSource,
        string theme,
        string resourceKey)
    {
        var hasOldInlineGallery =
            pageSource.Contains(resourceKey, StringComparison.Ordinal) &&
            pageSource.Contains($"IconThemeType=\"{theme}\"", StringComparison.Ordinal);

        var hasLazyGallery =
            pageSource.Contains(resourceKey, StringComparison.Ordinal) &&
            pageSource.Contains($"Tag=\"{theme}\"", StringComparison.Ordinal) &&
            codeBehindSource.Contains($"IconThemeType.{theme}", StringComparison.Ordinal);

        if (hasOldInlineGallery || hasLazyGallery)
        {
            scenarios.Add($"{theme}:{resourceKey}");
        }
    }

    private static int CountIconThemeScenarios(string source)
    {
        return source.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length;
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

    private static void ShowInWindow(Control content, Action<AvaloniaWindow> assertion)
    {
        var visualLayerManager = new VisualLayerManager
        {
            EnableAdornerLayer = true,
            Child              = content
        };
        var window = new AvaloniaWindow
        {
            Width   = 1000,
            Height  = 700,
            Content = visualLayerManager
        };

        window.Show();
        Dispatcher.UIThread.RunJobs();

        try
        {
            assertion(window);
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    private static T WaitForVisual<T>(Visual root)
        where T : Visual
    {
        for (var i = 0; i < 40; i++)
        {
            Dispatcher.UIThread.RunJobs();
            var visual = root.GetVisualDescendants().OfType<T>().FirstOrDefault();
            if (visual is not null)
            {
                return visual;
            }

            Thread.Sleep(10);
        }

        return root.GetVisualDescendants().OfType<T>().First();
    }

    private static void WaitForClipboardText(Control control, string expectedText)
    {
        var clipboard = TopLevel.GetTopLevel(control)?.Clipboard;
        clipboard.ShouldNotBeNull();

        string? actualText = null;
        for (var i = 0; i < 40; i++)
        {
            Dispatcher.UIThread.RunJobs();
            using var dataTransfer = clipboard!.TryGetInProcessDataAsync().GetAwaiter().GetResult();
            actualText = dataTransfer is null
                ? null
                : dataTransfer.TryGetValueAsync(DataFormat.Text).GetAwaiter().GetResult();
            if (actualText == expectedText)
            {
                return;
            }

            Thread.Sleep(10);
        }

        actualText.ShouldBe(expectedText);
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
