using System.Xml.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Window;

public class WindowTitleBarTokenTests
{
    static WindowTitleBarTokenTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Caption_Button_Padding_Uses_Default_Two_Size_Units()
    {
        var source = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/WindowTitleBar/WindowTitleBarToken.cs"));

        source.ShouldContain("CaptionButtonPadding = new Thickness(SharedToken.SizeUnit * 2)");
    }

    [Fact]
    public void Windows_Caption_Button_Icons_Use_Native_Size()
    {
        var source = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/WindowTitleBar/WindowTitleBarToken.cs"));

        source.ShouldContain("CaptionButtonIconSize       = SharedToken.IconSize;");
        source.ShouldContain("WindowsCaptionIconSize      = 11;");
    }

    [Fact]
    public void Title_Bar_Content_Padding_Has_No_Vertical_Inset()
    {
        var source = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/WindowTitleBar/WindowTitleBarToken.cs"));

        source.ShouldContain("TitleBarPadding             = new Thickness(LogoAndTitleSpacing * 1.8, 0)");
    }

    [Fact]
    public void Windows_Title_Bar_Theme_Leaves_Close_Button_Flush_To_The_Right_Edge()
    {
        var document = XDocument.Load(GetRepoFile(
            "src/AtomUI.Desktop.Controls/WindowTitleBar/Themes/WindowTitleBarTheme.axaml"));

        var windowsStyle = document.Descendants()
                                   .Single(element =>
                                       element.Name.LocalName == "Style" &&
                                       (string?)element.Attribute("Selector") ==
                                       "^[OsType=Windows]");
        var template = windowsStyle.Descendants()
                                     .Single(element => element.Name.LocalName == "ControlTemplate");
        var frame = template.Elements().Single();
        var captionButtonGroup = FindTemplatePart(template, "PART_CaptionButtonGroup");
        var leftPaddingConverter = FindResource(document.Root!, "WindowTitleBarLeftPaddingConverter");
        var layoutPanel = frame.Elements()
                               .Single(element => element.Name.LocalName == "WindowTitleBarLayoutPanel");
        var trailing = layoutPanel.Elements()
                                  .Single(element =>
                                      element.Attributes().Any(attribute =>
                                          attribute.Name.LocalName == "WindowTitleBarLayoutPanel.Role" &&
                                          attribute.Value == "Trailing"));

        frame.Name.LocalName.ShouldBe("Border");
        frame.Attribute("Name")?.Value.ShouldBe("Frame");
        frame.Attribute("Background")?.Value.ShouldBe("{TemplateBinding Background}");
        frame.Attribute("Padding").ShouldBeNull();
        layoutPanel.Attribute("Padding")?.Value.ShouldBe(
            "{TemplateBinding Padding, Converter={StaticResource WindowTitleBarLeftPaddingConverter}}");
        leftPaddingConverter.Name.LocalName.ShouldBe("BorderThicknessFilterConverter");
        leftPaddingConverter.Attribute("Left")?.Value.ShouldNotBe("False");
        leftPaddingConverter.Attribute("Right")?.Value.ShouldBe("False");
        leftPaddingConverter.Attribute("Top")?.Value.ShouldBe("False");
        leftPaddingConverter.Attribute("Bottom")?.Value.ShouldBe("False");
        captionButtonGroup.Ancestors().ShouldContain(frame);
        captionButtonGroup.Parent.ShouldBe(trailing);
        captionButtonGroup.ElementsAfterSelf().ShouldBeEmpty();
    }

    [Fact]
    public void Shared_Window_Chrome_Metrics_Do_Not_Use_Speculative_Os_Branches()
    {
        var windowTokenSource = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/Window/WindowToken.cs"));
        var titleBarTokenSource = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/WindowTitleBar/WindowTitleBarToken.cs"));

        windowTokenSource.ShouldContain("TitleBarHeight               = 40;");
        windowTokenSource.ShouldContain("FullscreenHeaderFramePadding = new Thickness(24, 0);");
        windowTokenSource.ShouldNotContain("GetPlatformTitleBarHeight");
        windowTokenSource.ShouldNotContain("GetPlatformFullscreenHeaderPadding");
        windowTokenSource.ShouldNotContain("OperatingSystem.");

        titleBarTokenSource.ShouldContain("Height                      = 40;");
        titleBarTokenSource.ShouldContain("FullscreenCaptionButtonSize = 24;");
        titleBarTokenSource.ShouldContain("HeaderHorizontalSpacing     = 8;");
        titleBarTokenSource.ShouldNotContain("GetPlatformTitleBarHeight");
        titleBarTokenSource.ShouldNotContain("GetPlatformFullscreenCaptionButtonSize");
        titleBarTokenSource.ShouldNotContain("GetPlatformHeaderHorizontalSpacing");
        titleBarTokenSource.ShouldNotContain("OperatingSystem.");
    }

    [Fact]
    public void Linux_Caption_Button_Background_Is_Inset_Without_Changing_Button_Layout_Size()
    {
        var buttonTheme = XDocument.Load(GetRepoFile(
            "src/AtomUI.Desktop.Controls/WindowTitleBar/Themes/CaptionButtonTheme.axaml"));
        var groupTheme = XDocument.Load(GetRepoFile(
            "src/AtomUI.Desktop.Controls/WindowTitleBar/Themes/CaptionButtonGroupTheme.axaml"));

        var frame = buttonTheme.Descendants()
                               .Single(element => (string?)element.Attribute("Name") == "PART_Frame");
        var contentFrame = frame.ElementsAfterSelf().Single();
        var linuxStyle = groupTheme.Descendants()
                                   .Single(element =>
                                       element.Name.LocalName == "Style" &&
                                       (string?)element.Attribute("Selector") ==
                                       "^ /template/ atom|CaptionButton" &&
                                       element.Ancestors().Any(ancestor =>
                                           ancestor.Name.LocalName == "Style" &&
                                           (string?)ancestor.Attribute("Selector") == "^[OsType=Linux]"));
        var insetSetter = linuxStyle.Elements()
                                    .Single(element => element.Name.LocalName == "Setter");

        frame.Parent.ShouldNotBeNull();
        frame.Parent.Name.LocalName.ShouldBe("Panel");
        frame.Attribute("Padding").ShouldBeNull();
        contentFrame.Name.LocalName.ShouldBe("Border");
        contentFrame.Attribute("Padding")?.Value.ShouldBe("{TemplateBinding Padding}");
        insetSetter.Attribute("Property")?.Value.ShouldBe("BackgroundInset");
        insetSetter.Attribute("Value")?.Value.ShouldBe("2");
        linuxStyle.Ancestors()
                  .ShouldContain(element =>
                      element.Name.LocalName == "Style" &&
                      (string?)element.Attribute("Selector") == "^[OsType=Linux]");
    }

    [Fact]
    public void Linux_Caption_Button_Background_Inset_Is_Applied_At_Runtime()
    {
        var group = new CaptionButtonGroup();
        group.SetValue(CaptionButtonGroup.OsTypeProperty, OsType.Linux);
        Application.Current!.TryFindResource(typeof(CaptionButtonGroup), out var resource).ShouldBeTrue();
        group.Theme = resource.ShouldBeAssignableTo<ControlTheme>();

        var host = new Avalonia.Controls.Window
        {
            Width   = 400,
            Height  = 100,
            Content = group
        };

        try
        {
            host.Show();
            group.ApplyTemplate();
            host.UpdateLayout();

            var buttons = group.GetVisualDescendants().OfType<CaptionButton>().ToList();
            buttons.ShouldNotBeEmpty();

            foreach (var button in buttons)
            {
                button.ApplyTemplate();
                var frame = button.GetVisualDescendants()
                                  .OfType<Border>()
                                  .Single(border => border.Name == "PART_Frame");

                button.BackgroundInset.ShouldBe(new Thickness(2));
                frame.Margin.ShouldBe(new Thickness(2));
            }
        }
        finally
        {
            host.Close();
        }
    }

    [Fact]
    public void Windows_Caption_Buttons_Use_Dedicated_Token_Colored_Geometries()
    {
        var iconResources = XDocument.Load(GetRepoFile(
            "src/AtomUI.Desktop.Controls/WindowTitleBar/Themes/WindowsCaptionIconGeometries.axaml"));
        var groupTheme = XDocument.Load(GetRepoFile(
            "src/AtomUI.Desktop.Controls/WindowTitleBar/Themes/CaptionButtonGroupTheme.axaml"));
        var buttonTheme = XDocument.Load(GetRepoFile(
            "src/AtomUI.Desktop.Controls/WindowTitleBar/Themes/CaptionButtonTheme.axaml"));
        var themeResources = XDocument.Load(GetRepoFile(
            "src/AtomUI.Desktop.Controls/WindowTitleBar/Themes/WindowTitleBarThemes.axaml"));

        var expectedResources = new[]
        {
            "WindowCaptionMinimizeIconGeometry",
            "WindowCaptionMaximizeIconGeometry",
            "WindowCaptionRestoreIconGeometry",
            "WindowCaptionCloseIconGeometry"
        };
        var geometryKeys = iconResources.Root!
                                            .Elements()
                                            .Where(element => element.Name.LocalName == "StreamGeometry")
                                            .Select(element => element.Attributes()
                                                                      .Single(attribute =>
                                                                          attribute.Name.LocalName == "Key")
                                                                      .Value)
                                            .ToList();

        geometryKeys.ShouldBe(expectedResources, ignoreOrder: true);
        iconResources.ToString().ShouldNotContain("#020202");

        var windowsStyle = groupTheme.Descendants()
                                     .Single(element =>
                                         element.Name.LocalName == "Style" &&
                                         (string?)element.Attribute("Selector") == "^[OsType=Windows]" &&
                                         element.Descendants().Any(descendant =>
                                             descendant.Name.LocalName == "PathIcon"));
        foreach (var resourceKey in expectedResources)
        {
            windowsStyle.Descendants()
                        .ShouldContain(element =>
                            element.Name.LocalName == "PathIcon" &&
                            (string?)element.Attribute("Data") == $"{{StaticResource {resourceKey}}}");
        }

        windowsStyle.ToString().ShouldNotContain("MinusOutlined");
        windowsStyle.ToString().ShouldNotContain("WindowMaximizedOutlined");
        windowsStyle.ToString().ShouldNotContain("WindowRestoreOutlined");
        windowsStyle.ToString().ShouldNotContain("WindowCloseOutlined");
        groupTheme.ToString().ShouldContain("WindowTitleBarTokenResource WindowsCaptionIconSize");
        var extendedActionButtons = windowsStyle.Descendants()
                                                .Where(element =>
                                                    element.Name.LocalName == "WindowsCaptionButton" &&
                                                    (string?)element.Attribute("Classes") == "extended-action")
                                                .Select(element => (string?)element.Attribute("Name"))
                                                .ToList();
        extendedActionButtons.ShouldBe(
            ["PART_FullScreenButton", "PART_PinButton"],
            ignoreOrder: true);

        groupTheme.Descendants()
                  .Single(element =>
                      element.Name.LocalName == "Style" &&
                      (string?)element.Attribute("Selector") ==
                      "^[OsType=Windows] /template/ atom|WindowsCaptionButton.extended-action")
                  .ToString()
                  .ShouldContain("WindowTitleBarTokenResource CaptionButtonIconSize");
        buttonTheme.ToString().ShouldContain("WindowTitleBarTokenResource ActiveColor");
        buttonTheme.ToString().ShouldContain("WindowTitleBarTokenResource InactiveColor");

        var includes = themeResources.Root!
                                     .Element(themeResources.Root.Name.Namespace + "ResourceDictionary.MergedDictionaries")!
                                     .Elements()
                                     .Select(element => (string?)element.Attribute("Source"))
                                     .ToList();
        includes.IndexOf("WindowsCaptionIconGeometries.axaml")
                .ShouldBeLessThan(includes.IndexOf("CaptionButtonGroupTheme.axaml"));
    }

    [Fact]
    public void Window_State_Changes_Invalidate_Stale_Maximize_Hover()
    {
        var groupSource = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/WindowTitleBar/CaptionButtonGroup.cs"));
        var buttonSource = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/WindowTitleBar/WindowsCaptionButton.cs"));
        var themeSource = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/WindowTitleBar/Themes/WindowsCaptionButtonTheme.axaml"));

        groupSource.ShouldContain("InvalidateWindowsCaptionButtonPointerOverVisualStates();");
        groupSource.ShouldContain("InvalidateWindowsCaptionButtonPointerOverVisualState(_maximizeButton);");
        groupSource.ShouldContain("windowsCaptionButton.InvalidatePointerOverVisualState();");
        buttonSource.ShouldContain("IsPointerOverSuppressed = IsPointerOver;");
        buttonSource.ShouldContain("protected override void OnPointerMoved(PointerEventArgs e)");
        themeSource.ShouldContain("^[IsPointerOverSuppressed=False]:pointerover");
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

    private static XElement FindTemplatePart(XElement root, string partName)
    {
        var part = root.Descendants()
                       .SingleOrDefault(element => (string?)element.Attribute("Name") == partName);

        part.ShouldNotBeNull();
        return part;
    }

    private static XElement FindResource(XElement root, string resourceKey)
    {
        var resource = root.Descendants()
                           .SingleOrDefault(element =>
                               element.Attributes().Any(attribute =>
                                   attribute.Name.LocalName == "Key" &&
                                   attribute.Value == resourceKey));

        resource.ShouldNotBeNull();
        return resource;
    }
}
