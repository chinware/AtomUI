using System.Xml.Linq;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.ImagePreviewer;

public class ImagePreviewerTitleBarThemeTests
{
    [Fact]
    public void ImagePreviewer_Linux_TitleBar_Applies_Frame_Padding_To_Caption_Buttons()
    {
        var document = XDocument.Load(GetRepoFile(
            "src/AtomUI.Desktop.Controls/ImagePreviewer/Themes/ImagePreviewerTitleBarTheme.axaml"));

        var linuxStyle = document.Descendants()
                                 .Single(element =>
                                     element.Name.LocalName == "Style" &&
                                     (string?)element.Attribute("Selector") == "^[OsType=Linux]");
        var template = linuxStyle.Descendants()
                                 .Single(element => element.Name.LocalName == "ControlTemplate");
        var frame = template.Elements().Single();
        var captionButtonGroup = FindTemplatePart(template, "PART_CaptionButtonGroup");

        frame.Name.LocalName.ShouldBe("Border");
        frame.Attribute("Name")?.Value.ShouldBe("Frame");
        frame.Attribute("Padding")?.Value.ShouldBe("{TemplateBinding Padding}");
        captionButtonGroup.Ancestors().ShouldContain(frame);
        captionButtonGroup.Parent.ShouldNotBeNull();
        captionButtonGroup.Parent.Name.LocalName.ShouldBe("DockPanel");
        captionButtonGroup.Parent.Parent.ShouldBe(frame);
    }

    [Fact]
    public void ImagePreviewer_TitleBar_Template_Centers_Title_Outside_LeftAddOn_Flow()
    {
        var document = XDocument.Load(GetRepoFile(
            "src/AtomUI.Desktop.Controls/ImagePreviewer/Themes/ImagePreviewerTitleBarTheme.axaml"));

        var templates = document.Descendants()
                                .Where(element => element.Name.LocalName == "ControlTemplate")
                                .ToList();

        templates.Count.ShouldBeGreaterThanOrEqualTo(3);

        foreach (var template in templates)
        {
            var leftAddOnPresenter = FindTemplatePart(template, "PART_LeftAddOn");
            var titlePresenter     = FindTemplatePart(template, "PART_ContentPresenter");

            titlePresenter.Attribute("HorizontalAlignment")?.Value.ShouldBe("Center");
            titlePresenter.Attribute("Content")?.Value.ShouldBe("{TemplateBinding Title}");
            titlePresenter.Attribute("IsHitTestVisible")?.Value.ShouldBe("False");
            titlePresenter.Attribute("DockPanel.Dock").ShouldBeNull();
            titlePresenter.Parent.ShouldNotBeNull();
            titlePresenter.Parent.Name.LocalName.ShouldBe("StackPanel");
            titlePresenter.Parent.Parent.ShouldNotBeNull();
            titlePresenter.Parent.Parent.Name.LocalName.ShouldBe("Panel");

            var leftAddOnStack = FindNearestAncestor(leftAddOnPresenter, "StackPanel");
            var titleStack     = FindNearestAncestor(titlePresenter, "StackPanel");

            if (leftAddOnStack is not null && titleStack is not null)
            {
                titleStack.ShouldNotBe(leftAddOnStack);
            }
        }
    }

    [Fact]
    public void ImagePreviewer_TitleBar_Template_Places_Explicit_Icon_Before_Title_In_Title_Group()
    {
        var document = XDocument.Load(GetRepoFile(
            "src/AtomUI.Desktop.Controls/ImagePreviewer/Themes/ImagePreviewerTitleBarTheme.axaml"));

        var templates = document.Descendants()
                                .Where(element => element.Name.LocalName == "ControlTemplate")
                                .ToList();

        templates.Count.ShouldBeGreaterThanOrEqualTo(3);

        foreach (var template in templates)
        {
            var titleLayout    = FindTemplatePart(template, "PART_TitleLayout");
            var iconPresenter  = FindTemplatePart(template, "PART_IconPresenter");
            var titlePresenter = FindTemplatePart(template, "PART_ContentPresenter");

            titleLayout.Name.LocalName.ShouldBe("StackPanel");
            titleLayout.Attribute("Orientation")?.Value.ShouldBe("Horizontal");
            titleLayout.Attribute("HorizontalAlignment")?.Value.ShouldBe("Center");
            titleLayout.Attribute("Spacing")?.Value.ShouldBe("{atom:WindowTitleBarTokenResource LogoAndTitleSpacing}");

            iconPresenter.Name.LocalName.ShouldBe("IconPresenter");
            iconPresenter.Attribute("Icon")?.Value.ShouldBe("{TemplateBinding Icon}");
            iconPresenter.Attribute("IsVisible")?.Value.ShouldBe("{TemplateBinding Icon, Converter={x:Static ObjectConverters.IsNotNull}}");
            iconPresenter.Parent.ShouldBe(titleLayout);
            titlePresenter.Parent.ShouldBe(titleLayout);
            iconPresenter.ElementsAfterSelf().ShouldContain(titlePresenter);
        }

        document.ToString().ShouldNotContain("PART_Logo");
        document.ToString().ShouldNotContain("TemplateBinding Logo");
        document.ToString().ShouldNotContain("IsEffectiveLogoVisible");
    }

    [Fact]
    public void ImagePreviewer_TitleBar_Toolbar_Uses_Same_Navigation_Icons_As_Side_Nav()
    {
        var document = XDocument.Load(GetRepoFile(
            "src/AtomUI.Desktop.Controls/ImagePreviewer/Themes/ImagePreviewToolbarTheme.axaml"));

        var previousButton = FindTemplatePart(document.Root!, "PART_PreviousButton");
        var nextButton     = FindTemplatePart(document.Root!, "PART_NextButton");

        previousButton.Attribute("Icon")?.Value.ShouldBe("{antdicons:AntDesignIconProvider LeftOutlined}");
        nextButton.Attribute("Icon")?.Value.ShouldBe("{antdicons:AntDesignIconProvider RightOutlined}");
    }

    private static XElement FindTemplatePart(XElement root, string partName)
    {
        var part = root.Descendants()
                       .SingleOrDefault(element => (string?)element.Attribute("Name") == partName);

        part.ShouldNotBeNull();
        return part;
    }

    private static XElement? FindNearestAncestor(XElement element, string localName)
    {
        return element.Ancestors()
                      .FirstOrDefault(ancestor => ancestor.Name.LocalName == localName);
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

        throw new FileNotFoundException($"Could not find repository file: {relativePath}");
    }
}
