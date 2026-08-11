using AtomUI.Toolkits.GalleryBase.Controls;
using AtomUI.Localization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Presenters;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomUIButton = AtomUI.Desktop.Controls.Button;
using AtomUITag = AtomUI.Desktop.Controls.Tag;
using AtomUIToolTip = AtomUI.Desktop.Controls.ToolTip;
using AtomUIWindow = AtomUI.Desktop.Controls.Window;

namespace AtomUI.Toolkits.GalleryBase.Tests.Controls;

public class SemanticPartPreviewTests
{
    public SemanticPartPreviewTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Activate_Builds_Descriptor_Items_Without_Creating_A_Highlight_Session()
    {
        var button = new AtomUIButton
        {
            Content   = "Semantic Button",
            IsLoading = true
        };
        var preview = CreatePreview(button);

        preview.Items.ShouldBeEmpty();
        preview.ActiveHighlightSession.ShouldBeNull();
        preview.CodeViewer.ShouldBeNull();

        using var context = ShowInWindow(preview);
        preview.ActivatePreview();

        preview.Items.Select(static item => item.Path)
               .ShouldBe(["root", "content", "icon"]);
        preview.ActiveHighlightSession.ShouldBeNull();
        preview.CodeViewer.ShouldBeNull();
        AdornerLayer.GetAdornerLayer(button).ShouldNotBeNull()
                    .Children.OfType<SemanticPartAdorner>().ShouldBeEmpty();
    }

    [Fact]
    public void Effective_Part_Uses_Pin_Over_Hover_And_Deactivate_Releases_The_Session()
    {
        var button = new AtomUIButton
        {
            Content   = "Semantic Button",
            IsLoading = true
        };
        var preview = CreatePreview(button);

        using var context = ShowInWindow(preview);
        preview.ActivatePreview();
        var root = preview.Items.Single(static item => item.Path == "root");
        var content = preview.Items.Single(static item => item.Path == "content");
        var icon = preview.Items.Single(static item => item.Path == "icon");

        preview.SetHoveredPart(icon, true);
        Dispatcher.UIThread.RunJobs();
        preview.EffectivePart.ShouldBe(icon);
        preview.ActiveHighlightSession.ShouldNotBeNull();

        preview.TogglePinnedPart(content);
        Dispatcher.UIThread.RunJobs();
        preview.EffectivePart.ShouldBe(content);
        content.IsPinned.ShouldBeTrue();

        preview.SetHoveredPart(root, true);
        Dispatcher.UIThread.RunJobs();
        preview.EffectivePart.ShouldBe(content);

        preview.DeactivatePreview();
        Dispatcher.UIThread.RunJobs();
        preview.ActiveHighlightSession.ShouldBeNull();
        AdornerLayer.GetAdornerLayer(button).ShouldNotBeNull()
                    .Children.OfType<SemanticPartAdorner>().ShouldBeEmpty();
    }

    [Fact]
    public void Info_Lazily_Creates_The_Code_Viewer_Without_Target_Resolution()
    {
        var button = new AtomUIButton
        {
            Content = "Semantic Button"
        };
        var preview = CreatePreview(button);

        using var context = ShowInWindow(preview);
        preview.ActivatePreview();
        var content = preview.Items.Single(static item => item.Path == "content");
        preview.CodeViewer.ShouldBeNull();

        preview.ShowPartInfo(content);

        preview.CodeViewer.ShouldNotBeNull();
        var code = preview.CodeViewer.CodeText.ShouldNotBeNull();
        code.ShouldContain("xmlns:atom=\"using:AtomUI.Desktop.Controls\"");
        code.ShouldContain("Selector=\"atom|Button /template/ .semantic-content\"");
        code.ShouldContain(".semantic-content");
        code.ShouldContain("/template/");
        code.ShouldContain("x:SetterTargetType=\"contract:ContentPresenter\"");
        code.ShouldNotContain("xmlns:owner=");
        code.ShouldNotContain(":is(");

        preview.ShowPartInfo(preview.Items.Single(static item => item.Path == "root"));
        var rootCode = preview.CodeViewer.CodeText.ShouldNotBeNull();
        rootCode.ShouldContain("Selector=\"atom|Button\"");
        rootCode.ShouldContain("x:SetterTargetType=\"atom:Button\"");
        preview.ActiveHighlightSession.ShouldBeNull();
    }

    [Fact]
    public void Template_Uses_A_Single_Flat_Inspection_Panel()
    {
        var preview = CreatePreview(new AtomUIButton
        {
            Content = "Semantic Button"
        });

        using var context = ShowInWindow(preview);
        preview.ActivatePreview();
        preview.ApplyTemplate();
        Dispatcher.UIThread.RunJobs();

        var frame = preview.GetVisualDescendants()
                           .OfType<Border>()
                           .Single(static border => border.Name == "PART_InspectionPanel");
        frame.VerticalAlignment.ShouldBe(VerticalAlignment.Top);
        frame.BorderThickness.ShouldBe(new Thickness(1));
        frame.CornerRadius.ShouldBe(default);

        var layout = preview.GetVisualDescendants()
                            .OfType<SemanticPartPreviewLayoutPanel>()
                            .Single(static panel => panel.Name == "PART_Layout");
        layout.Spacing.ShouldBe(0);
        layout.Children.Count.ShouldBe(2);

        var previewStage = layout.Children[0].ShouldBeOfType<Border>();
        previewStage.Name.ShouldBe("PART_PreviewStage");
        previewStage.Background.ShouldBeNull();
        previewStage.BorderThickness.ShouldBe(default);
        previewStage.CornerRadius.ShouldBe(default);

        var partsPane = layout.Children[1].ShouldBeOfType<Border>();
        partsPane.Name.ShouldBe("PART_PartsPane");
        partsPane.BorderThickness.ShouldBe(new Thickness(1, 0, 0, 0));
        partsPane.CornerRadius.ShouldBe(default);

        var rows = preview.GetVisualDescendants()
                          .OfType<SemanticPartPreviewItemControl>()
                          .ToArray();
        rows.Length.ShouldBe(3);
        foreach (var row in rows)
        {
            var rowBorder = row.GetVisualDescendants()
                               .OfType<Border>()
                               .Single(static border => border.Name == "PART_Row");
            rowBorder.BorderThickness.ShouldBe(new Thickness(0, 0, 0, 1));
            rowBorder.CornerRadius.ShouldBe(default);
            AtomUIToolTip.GetTip(rowBorder).ShouldBeNull();
        }

        preview.GetVisualDescendants().OfType<AtomUITag>().Count().ShouldBe(3);
        var persistentTexts = rows.SelectMany(static row => row.GetVisualDescendants().OfType<TextBlock>())
                                  .Select(static textBlock => textBlock.Text)
                                  .Where(static text => text is not null)
                                  .ToArray();
        persistentTexts.ShouldNotContain(".semantic-content");
        persistentTexts.ShouldNotContain(nameof(ContentPresenter));
    }

    [Fact]
    public void Info_Lazily_Separates_Technical_Metadata_From_The_Main_Row()
    {
        var preview = CreatePreview(new AtomUIButton
        {
            Content = "Semantic Button"
        });

        using var context = ShowInWindow(preview);
        preview.ActivatePreview();
        var content = preview.Items.Single(static item => item.Path == "content");

        preview.ShowPartInfo(content);
        Dispatcher.UIThread.RunJobs();

        var infoContent = preview.InfoContent.ShouldNotBeNull();
        infoContent.ShouldNotBeOfType<GalleryCodeViewer>();
        var metadataValues = infoContent.GetVisualDescendants()
                                        .OfType<TextBlock>()
                                        .Select(static textBlock => textBlock.Text)
                                        .Where(static text => text is not null)
                                        .ToArray();
        metadataValues.ShouldContain(content.Selector);
        metadataValues.ShouldContain(content.ContractType);
        metadataValues.ShouldContain(content.Cardinality);
        metadataValues.ShouldContain(content.Customization);
        var metadataGrid = infoContent.GetVisualDescendants()
                                      .OfType<Grid>()
                                      .Single();
        metadataGrid.Children
                    .OfType<TextBlock>()
                    .ShouldAllBe(static textBlock =>
                        textBlock.VerticalAlignment == VerticalAlignment.Center);
        infoContent.GetVisualDescendants()
                   .OfType<GalleryCodeViewer>()
                   .ShouldBeEmpty();
        preview.CodeViewer.ShouldNotBeNull();
        preview.ActiveHighlightSession.ShouldBeNull();
    }

    [Fact]
    public void Code_Example_Is_Hosted_Below_The_Two_Column_Layout_At_Full_Width()
    {
        var preview = CreatePreview(new AtomUIButton
        {
            Content = "Semantic Button"
        });

        using var context = ShowInWindow(preview);
        preview.ActivatePreview();
        preview.ShowPartInfo(preview.Items.Single(static item => item.Path == "content"));
        preview.ApplyTemplate();
        Dispatcher.UIThread.RunJobs();

        var frame = preview.GetVisualDescendants()
                           .OfType<Border>()
                           .Single(static border => border.Name == "PART_InspectionPanel");
        var rootLayout = frame.Child.ShouldBeOfType<Grid>();
        rootLayout.RowDefinitions.Count.ShouldBe(2);

        var layout = rootLayout.Children
                               .OfType<SemanticPartPreviewLayoutPanel>()
                               .Single(static panel => panel.Name == "PART_Layout");
        var partsPane = layout.Children[1].ShouldBeOfType<Border>();
        var infoPresenter = partsPane.GetVisualDescendants()
                                     .OfType<ContentPresenter>()
                                     .Single(static presenter => presenter.Name == "PART_InfoContent");
        var codePresenter = rootLayout.Children
                                      .OfType<ContentPresenter>()
                                      .Single(static presenter => presenter.Name == "PART_CodeContent");

        Grid.GetRow(layout).ShouldBe(0);
        Grid.GetRow(codePresenter).ShouldBe(1);
        infoPresenter.Bounds.Width.ShouldBe(
            partsPane.Bounds.Width - partsPane.BorderThickness.Left - partsPane.BorderThickness.Right,
            0.01);
        codePresenter.Bounds.Width.ShouldBe(layout.Bounds.Width, 0.01);
        codePresenter.Bounds.Width.ShouldBeGreaterThan(partsPane.Bounds.Width);
        infoPresenter.GetVisualDescendants()
                     .OfType<GalleryCodeViewer>()
                     .ShouldBeEmpty();
        codePresenter.GetVisualDescendants()
                     .OfType<GalleryCodeViewer>()
                     .ShouldHaveSingleItem();
    }

    [Fact]
    public void Activate_Rejects_A_Description_For_An_Unknown_Part_Path()
    {
        var preview = CreatePreview(new AtomUIButton { Content = "Semantic Button" });
        preview.PartDescriptions.Add(new SemanticPartDescription
        {
            Path        = "missing",
            Description = "Unknown"
        });

        Should.Throw<InvalidOperationException>(() => preview.ActivatePreview())
              .Message.ShouldContain("missing");
    }

    [Fact]
    public void Activate_Uses_The_Current_Language_For_Fallback_Descriptions_Without_Row_Status_Tooltips()
    {
        var languageManager = Application.Current.ShouldNotBeNull()
                                         .GetLanguageManager().ShouldNotBeNull();
        var button = new AtomUIButton { Content = "Semantic Button" };
        var preview = new SemanticPartPreview
        {
            PreviewContent    = button,
            SemanticOwnerType = typeof(AtomUIButton)
        };

        try
        {
            languageManager.ChangeLanguage(LanguageTags.ZhCN);
            using var context = ShowInWindow(preview);
            preview.ActivatePreview();

            var root = preview.Items.Single(static item => item.Path == "root");
            root.Description.ShouldBe("控件根区域。");

            preview.SetHoveredPart(root, true);
            Dispatcher.UIThread.RunJobs();

            var rootRow = preview.GetVisualDescendants()
                                 .OfType<SemanticPartPreviewItemControl>()
                                 .Single(row => ReferenceEquals(row.Item, root));
            var rowBorder = rootRow.GetVisualDescendants()
                                   .OfType<Border>()
                                   .Single(static border => border.Name == "PART_Row");
            AtomUIToolTip.GetTip(rowBorder).ShouldBeNull();
        }
        finally
        {
            languageManager.ChangeLanguage(LanguageTags.EnUS);
            Dispatcher.UIThread.RunJobs();
        }
    }

    private static SemanticPartPreview CreatePreview(AtomUIButton button)
    {
        var preview = new SemanticPartPreview
        {
            PreviewContent    = button,
            SemanticOwnerType = typeof(AtomUIButton)
        };
        preview.PartDescriptions.Add(new SemanticPartDescription
        {
            Path        = "root",
            Description = "The Button root."
        });
        preview.PartDescriptions.Add(new SemanticPartDescription
        {
            Path        = "icon",
            Description = "The icon region."
        });
        preview.PartDescriptions.Add(new SemanticPartDescription
        {
            Path        = "content",
            Description = "The content region."
        });
        return preview;
    }

    private static WindowContext ShowInWindow(SemanticPartPreview preview)
    {
        var window = new AtomUIWindow
        {
            Width   = 1000,
            Height  = 700,
            Content = preview
        };

        window.Show();
        Dispatcher.UIThread.RunJobs();
        return new WindowContext(window);
    }

    private sealed class WindowContext : IDisposable
    {
        private readonly AtomUIWindow _window;

        public WindowContext(AtomUIWindow window)
        {
            _window = window;
        }

        public void Dispose()
        {
            _window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }
}
