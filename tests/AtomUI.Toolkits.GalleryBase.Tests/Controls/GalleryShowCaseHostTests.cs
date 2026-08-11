using AtomUI.Toolkits.GalleryBase.Controls;
using AtomUI.Localization;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Headless;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;

namespace AtomUI.Toolkits.GalleryBase.Tests.Controls;

public class GalleryShowCaseHostTests
{
    public GalleryShowCaseHostTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Semantic_Content_Is_Not_Materialized_Until_The_Semantic_Tab_Is_Selected()
    {
        var buildCount = 0;
        var host = new GalleryShowCaseHost
        {
            Header          = new Border(),
            ExamplesContent = new Border(),
            SemanticPartsContentTemplate = new FuncDataTemplate<object?>(
                (_, _) =>
                {
                    buildCount++;
                    return new SemanticPartPreview
                    {
                        PreviewContent    = new AtomUI.Desktop.Controls.Button { Content = "Semantic Button" },
                        SemanticOwnerType = typeof(AtomUI.Desktop.Controls.Button)
                    };
                })
        };

        ShowInWindow(host, () =>
        {
            buildCount.ShouldBe(0);
            host.IsSemanticPartsContentMaterialized.ShouldBeFalse();

            host.Measure(new(900, 700));
            host.Arrange(new(0, 0, 900, 700));
            Dispatcher.UIThread.RunJobs();

            buildCount.ShouldBe(0);
            host.IsSemanticPartsContentMaterialized.ShouldBeFalse();

            host.SelectedTab = GalleryShowCaseTab.SemanticParts;
            Dispatcher.UIThread.RunJobs();

            buildCount.ShouldBe(1);
            host.IsSemanticPartsContentMaterialized.ShouldBeTrue();
            host.GetVisualDescendants().OfType<SemanticPartPreview>().Count().ShouldBe(1);

            host.SelectedTab = GalleryShowCaseTab.Examples;
            Dispatcher.UIThread.RunJobs();

            buildCount.ShouldBe(1);
            host.GetVisualDescendants().OfType<SemanticPartPreview>().ShouldBeEmpty();

            host.SelectedTab = GalleryShowCaseTab.SemanticParts;
            Dispatcher.UIThread.RunJobs();

            buildCount.ShouldBe(1);
            host.GetVisualDescendants().OfType<SemanticPartPreview>().Count().ShouldBe(1);
        });
    }

    [Fact]
    public void Host_Without_Semantic_Template_Does_Not_Create_Tab_Strip()
    {
        var host = new GalleryShowCaseHost
        {
            Header          = new Border(),
            ExamplesContent = new Border()
        };

        ShowInWindow(host, () =>
        {
            host.HasSemanticParts.ShouldBeFalse();
            host.GetVisualDescendants()
                .OfType<AtomUI.Desktop.Controls.TabStrip>()
                .ShouldBeEmpty();
        });
    }

    [Fact]
    public void Language_Change_Updates_Tab_Labels_Without_Materializing_Semantic_Content()
    {
        var languageManager = Avalonia.Application.Current.ShouldNotBeNull()
                                                .GetLanguageManager().ShouldNotBeNull();
        var originalLanguage = languageManager.Current.CurrentLanguage;
        var buildCount = 0;
        var host = new GalleryShowCaseHost
        {
            Header          = new Border(),
            ExamplesContent = new Border(),
            SemanticPartsContentTemplate = new FuncDataTemplate<object?>(
                (_, _) =>
                {
                    buildCount++;
                    return new SemanticPartPreview();
                })
        };

        try
        {
            languageManager.ChangeLanguage(LanguageTags.EnUS);
            Dispatcher.UIThread.RunJobs();

            ShowInWindow(host, () =>
            {
                var tabStrip = host.GetVisualDescendants()
                                   .OfType<AtomUI.Desktop.Controls.TabStrip>()
                                   .Single();
                var tabs = tabStrip.Items.OfType<AtomUI.Desktop.Controls.TabStripItem>().ToArray();

                tabs.Select(static tab => tab.Content).ShouldBe(["Examples", "Semantic Parts"]);

                languageManager.ChangeLanguage(LanguageTags.ZhCN);
                Dispatcher.UIThread.RunJobs();

                tabs.Select(static tab => tab.Content).ShouldBe(["示例", "语义部件"]);
                buildCount.ShouldBe(0);
                host.IsSemanticPartsContentMaterialized.ShouldBeFalse();
                host.GetVisualDescendants().OfType<SemanticPartPreview>().ShouldBeEmpty();
            });
        }
        finally
        {
            languageManager.ChangeLanguage(originalLanguage);
            Dispatcher.UIThread.RunJobs();
        }
    }

    [Fact]
    public void Reattach_Rebuilds_Released_Semantic_Content_When_The_Tab_Remains_Selected()
    {
        var buildCount = 0;
        var host = new GalleryShowCaseHost
        {
            ExamplesContent = new Border(),
            SemanticPartsContentTemplate = new FuncDataTemplate<object?>(
                (_, _) =>
                {
                    buildCount++;
                    return new SemanticPartPreview
                    {
                        PreviewContent    = new AtomUI.Desktop.Controls.Button { Content = "Semantic Button" },
                        SemanticOwnerType = typeof(AtomUI.Desktop.Controls.Button)
                    };
                })
        };
        var window = new Window
        {
            Width   = 900,
            Height  = 700,
            Content = host
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            host.SelectedTab = GalleryShowCaseTab.SemanticParts;
            Dispatcher.UIThread.RunJobs();
            buildCount.ShouldBe(1);

            window.Content = null;
            Dispatcher.UIThread.RunJobs();
            host.IsSemanticPartsContentMaterialized.ShouldBeFalse();

            window.Content = host;
            Dispatcher.UIThread.RunJobs();

            buildCount.ShouldBe(2);
            host.IsSemanticPartsContentMaterialized.ShouldBeTrue();
            host.GetVisualDescendants().OfType<SemanticPartPreview>().Count().ShouldBe(1);
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    [Fact]
    public void Initial_Attach_Reuses_The_Navigation_Built_For_The_Semantic_Template()
    {
        var host = new GalleryShowCaseHost
        {
            ExamplesContent = new Border(),
            SemanticPartsContentTemplate = new FuncDataTemplate<object?>(
                (_, _) => new SemanticPartPreview())
        };
        var navigation = host.NavigationContent.ShouldNotBeNull();

        ShowInWindow(host, () => host.NavigationContent.ShouldBeSameAs(navigation));
    }

    [Fact]
    public void Detach_Releases_The_Materialized_Preview_And_Demo_Control()
    {
        var (host, previewReference, demoReference) = CreateDetachedSemanticContentReferences();

        for (var attempt = 0; attempt < 3; attempt++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            Dispatcher.UIThread.RunJobs();
        }

        previewReference.IsAlive.ShouldBeFalse();
        demoReference.IsAlive.ShouldBeFalse();
        GC.KeepAlive(host);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static (GalleryShowCaseHost Host, WeakReference Preview, WeakReference Demo)
        CreateDetachedSemanticContentReferences()
    {
        var host = new GalleryShowCaseHost
        {
            ExamplesContent = new Border(),
            SemanticPartsContentTemplate = new FuncDataTemplate<object?>(
                (_, _) => new SemanticPartPreview
                {
                    PreviewContent    = new AtomUI.Desktop.Controls.Button { Content = "Semantic Button" },
                    SemanticOwnerType = typeof(AtomUI.Desktop.Controls.Button)
                })
        };
        var window = new Window
        {
            Width   = 900,
            Height  = 700,
            Content = host
        };

        window.Show();
        Dispatcher.UIThread.RunJobs();
        host.SelectedTab = GalleryShowCaseTab.SemanticParts;
        Dispatcher.UIThread.RunJobs();

        var preview = host.GetVisualDescendants().OfType<SemanticPartPreview>().Single();
        var demo = preview.PreviewContent.ShouldNotBeNull();
        var previewReference = new WeakReference(preview);
        var demoReference = new WeakReference(demo);

        window.Content = null;
        Dispatcher.UIThread.RunJobs();
        host.ActiveContent.ShouldBeSameAs(host.ExamplesContent);
        host.GetVisualDescendants().OfType<SemanticPartPreview>().ShouldBeEmpty();
        host.GetLogicalDescendants().OfType<SemanticPartPreview>().ShouldBeEmpty();
        window.Close();
        Dispatcher.UIThread.RunJobs();

        return (host, previewReference, demoReference);
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new Window
        {
            Width   = 900,
            Height  = 700,
            Content = content
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            assertion();
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }
}
