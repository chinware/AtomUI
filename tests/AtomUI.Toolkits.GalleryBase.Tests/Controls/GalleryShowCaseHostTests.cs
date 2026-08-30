using System.Xml.Linq;
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

            // 双内容槽契约：切回 Examples 后语义内容隐藏挂载，不再销毁重建。
            buildCount.ShouldBe(1);
            var cachedPreview = host.GetVisualDescendants()
                                    .OfType<SemanticPartPreview>()
                                    .Single();
            cachedPreview.IsEffectivelyVisible.ShouldBeFalse();
            host.IsSemanticPartsContentMaterialized.ShouldBeTrue();

            host.SelectedTab = GalleryShowCaseTab.SemanticParts;
            Dispatcher.UIThread.RunJobs();

            buildCount.ShouldBe(1);
            host.GetVisualDescendants().OfType<SemanticPartPreview>().Count().ShouldBe(1);
            cachedPreview.IsEffectivelyVisible.ShouldBeTrue();
        });
    }

    [Fact]
    public void Semantic_Content_Can_Contain_Multiple_Independent_Previews()
    {
        var buildCount = 0;
        var firstPreview = new SemanticPartPreview
        {
            PreviewContent    = new AtomUI.Desktop.Controls.Button { Content = "First" },
            SemanticOwnerType = typeof(AtomUI.Desktop.Controls.Button)
        };
        var secondPreview = new SemanticPartPreview
        {
            PreviewContent    = new AtomUI.Desktop.Controls.Button { Content = "Second" },
            SemanticOwnerType = typeof(AtomUI.Desktop.Controls.Button)
        };
        var host = new GalleryShowCaseHost
        {
            Header          = new Border(),
            ExamplesContent = new Border(),
            SemanticPartsContentTemplate = new FuncDataTemplate<object?>(
                (_, _) =>
                {
                    buildCount++;
                    return new StackPanel
                    {
                        Children =
                        {
                            firstPreview,
                            secondPreview
                        }
                    };
                })
        };

        ShowInWindow(host, () =>
        {
            buildCount.ShouldBe(0);
            firstPreview.IsPreviewActive.ShouldBeFalse();
            secondPreview.IsPreviewActive.ShouldBeFalse();

            host.SelectedTab = GalleryShowCaseTab.SemanticParts;
            Dispatcher.UIThread.RunJobs();

            buildCount.ShouldBe(1);
            host.GetVisualDescendants().OfType<SemanticPartPreview>().Count().ShouldBe(2);
            firstPreview.IsPreviewActive.ShouldBeTrue();
            secondPreview.IsPreviewActive.ShouldBeTrue();

            host.SelectedTab = GalleryShowCaseTab.Examples;
            Dispatcher.UIThread.RunJobs();

            // 双内容槽契约：切回 Examples 后两个 preview 隐藏挂载并停用。
            buildCount.ShouldBe(1);
            host.GetVisualDescendants().OfType<SemanticPartPreview>().Count().ShouldBe(2);
            firstPreview.IsEffectivelyVisible.ShouldBeFalse();
            secondPreview.IsEffectivelyVisible.ShouldBeFalse();
            firstPreview.IsPreviewActive.ShouldBeFalse();
            secondPreview.IsPreviewActive.ShouldBeFalse();
        });

        Should.Throw<ObjectDisposedException>(() => firstPreview.ActivatePreview());
        Should.Throw<ObjectDisposedException>(() => secondPreview.ActivatePreview());
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
    public void Switching_Back_To_Examples_Keeps_Semantic_Content_Attached_But_Hidden()
    {
        var host = new GalleryShowCaseHost
        {
            Header          = new Border(),
            ExamplesContent = new Border(),
            SemanticPartsContentTemplate = new FuncDataTemplate<object?>(
                (_, _) => new SemanticPartPreview
                {
                    PreviewContent    = new AtomUI.Desktop.Controls.Button { Content = "Semantic Button" },
                    SemanticOwnerType = typeof(AtomUI.Desktop.Controls.Button)
                })
        };

        ShowInWindow(host, () =>
        {
            var examplesContent = host.ExamplesContent.ShouldNotBeNull().ShouldBeAssignableTo<Control>();

            host.SelectedTab = GalleryShowCaseTab.SemanticParts;
            Dispatcher.UIThread.RunJobs();

            var semanticContent = host.SemanticPartsContent.ShouldNotBeNull().ShouldBeAssignableTo<Control>();
            semanticContent.IsAttachedToVisualTree().ShouldBeTrue();
            semanticContent.IsEffectivelyVisible.ShouldBeTrue();
            examplesContent.IsAttachedToVisualTree().ShouldBeTrue();
            examplesContent.IsEffectivelyVisible.ShouldBeFalse();

            host.SelectedTab = GalleryShowCaseTab.Examples;
            Dispatcher.UIThread.RunJobs();

            // 双内容槽基础设施：切回 Examples 不再整树 detach 语义内容，
            // 而是隐藏挂载，消除来回切换的整树 attach/detach 卡顿。
            semanticContent.IsAttachedToVisualTree().ShouldBeTrue();
            semanticContent.IsEffectivelyVisible.ShouldBeFalse();
            examplesContent.IsEffectivelyVisible.ShouldBeTrue();
            host.IsSemanticPartsContentMaterialized.ShouldBeTrue();
        });
    }

    [Fact]
    public void Theme_Declares_Dual_Tab_Content_Presenters()
    {
        var themePath = FindRepoFile(
            "src/AtomUI.Toolkits.GalleryBase/Controls/Themes/GalleryShowCaseHostTheme.axaml");
        var document = System.Xml.Linq.XDocument.Load(themePath, System.Xml.Linq.LoadOptions.SetLineInfo);
        var names = document.Descendants()
                            .Attributes()
                            .Where(static attribute => attribute.Name.LocalName == "Name")
                            .Select(static attribute => attribute.Value)
                            .ToArray();

        names.ShouldContain("PART_ExamplesContentHost");
        names.ShouldContain("PART_SemanticPartsContentHost");

        // 双内容槽由宿主 C# 驱动：内容层 TemplatedParent 会被呈现器重绑定为
        // GalleryStickyTabsHost，模板内 TemplateBinding 解析不到宿主属性。
        var markup = File.ReadAllText(themePath);
        markup.ShouldNotContain("TemplateBinding ExamplesContent");
        markup.ShouldNotContain("TemplateBinding SemanticPartsContent");
        markup.ShouldNotContain("TemplateBinding ActiveContent");
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
        host.SemanticPartsContent.ShouldBeNull();
        host.GetVisualDescendants().OfType<SemanticPartPreview>().ShouldBeEmpty();
        host.GetLogicalDescendants().OfType<SemanticPartPreview>().ShouldBeEmpty();
        window.Close();
        Dispatcher.UIThread.RunJobs();

        return (host, previewReference, demoReference);
    }

    private static string FindRepoFile(string relativePath)
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

        throw new FileNotFoundException($"Could not locate repository file '{relativePath}'.");
    }

    private static void ShowInWindow(Control content, Action assertion)    {
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
