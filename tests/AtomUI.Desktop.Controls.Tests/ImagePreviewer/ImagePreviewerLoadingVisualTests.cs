using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.ImagePreviewer;

public class ImagePreviewerLoadingVisualTests
{
    [Fact]
    public void Single_ImagePreviewer_Template_Relays_Cover_Size_To_Cover_Control()
    {
        var theme = ReadRepoFile("src/AtomUI.Desktop.Controls/ImagePreviewer/Themes/ImagePreviewerTheme.axaml");

        theme.ShouldContain("Width=\"{TemplateBinding CoverWidth}\"");
        theme.ShouldContain("Height=\"{TemplateBinding CoverHeight}\"");
    }

    [Fact]
    public void Cover_Default_Loading_Uses_Image_Skeleton()
    {
        var theme = ReadRepoFile("src/AtomUI.Desktop.Controls/ImagePreviewer/Themes/ImagePreviewerCoverTheme.axaml");

        theme.ShouldContain("Name=\"PART_LoadingPresenter\"");
        theme.ShouldContain("MinWidth=\"{atom:ImagePreviewerTokenResource CoverImageWidth}\"");
        theme.ShouldContain("MinHeight=\"{atom:ImagePreviewerTokenResource CoverImageWidth}\"");
        theme.ShouldContain("atom:SkeletonImage");
        theme.ShouldContain("IsActive=\"True\"");
    }

    [Fact]
    public void Default_Error_Text_Is_Localized_For_Cover_And_Viewer()
    {
        var coverTheme  = ReadRepoFile("src/AtomUI.Desktop.Controls/ImagePreviewer/Themes/ImagePreviewerCoverTheme.axaml");
        var viewerTheme = ReadRepoFile("src/AtomUI.Desktop.Controls/ImagePreviewer/Themes/ImageViewerTheme.axaml");
        var enUS        = ReadRepoFile("src/AtomUI.Desktop.Controls/ImagePreviewer/Localization/en_US.cs");
        var zhCN        = ReadRepoFile("src/AtomUI.Desktop.Controls/ImagePreviewer/Localization/zh_CN.cs");
        var zhTW        = ReadRepoFile("src/AtomUI.Desktop.Controls/ImagePreviewer/Localization/zh_TW.cs");

        coverTheme.ShouldNotContain("Image load failed");
        viewerTheme.ShouldNotContain("Image load failed");
        coverTheme.ShouldContain("Name=\"PART_ErrorPresenter\"");
        coverTheme.ShouldContain("MinWidth=\"{atom:ImagePreviewerTokenResource CoverImageWidth}\"");
        coverTheme.ShouldContain("MinHeight=\"{atom:ImagePreviewerTokenResource CoverImageWidth}\"");
        coverTheme.ShouldContain("{atom:ImagePreviewerLangResource ImageLoadFailed}");
        viewerTheme.ShouldContain("{atom:ImagePreviewerLangResource ImageLoadFailed}");
        enUS.ShouldContain("public const string ImageLoadFailed");
        zhCN.ShouldContain("public const string ImageLoadFailed");
        zhTW.ShouldContain("public const string ImageLoadFailed");
    }

    [Fact]
    public void Default_Error_Icon_Uses_Prominent_Size_For_Cover_And_Viewer()
    {
        var coverTheme  = ReadRepoFile("src/AtomUI.Desktop.Controls/ImagePreviewer/Themes/ImagePreviewerCoverTheme.axaml");
        var viewerTheme = ReadRepoFile("src/AtomUI.Desktop.Controls/ImagePreviewer/Themes/ImageViewerTheme.axaml");

        coverTheme.ShouldContain("<Setter Property=\"Width\" Value=\"48\" />");
        coverTheme.ShouldContain("<Setter Property=\"Height\" Value=\"48\" />");
        viewerTheme.ShouldContain("<Setter Property=\"Width\" Value=\"48\" />");
        viewerTheme.ShouldContain("<Setter Property=\"Height\" Value=\"48\" />");
    }

    [Fact]
    public void Dialog_ImageViewer_Paints_Dialog_Background_To_Cover_Csd_State_Transition_Frames()
    {
        var dialogSource = ReadRepoFile("src/AtomUI.Desktop.Controls/ImagePreviewer/ImagePreviewerDialog.cs");
        var viewerTheme  = ReadRepoFile("src/AtomUI.Desktop.Controls/ImagePreviewer/Themes/ImageViewerTheme.axaml");

        dialogSource.ShouldContain("viewer[!ImageViewer.BackgroundProperty]");
        dialogSource.ShouldContain("this[!BackgroundProperty]");
        viewerTheme.ShouldContain("<Panel Background=\"{TemplateBinding Background}\">");
    }

    private static string ReadRepoFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(candidate))
            {
                return File.ReadAllText(candidate);
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"Could not find repository file: {relativePath}");
    }
}
