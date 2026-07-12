using System;
using System.IO;
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
    public void Title_Bar_Content_Padding_Has_No_Vertical_Inset()
    {
        var source = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/WindowTitleBar/WindowTitleBarToken.cs"));

        source.ShouldContain("TitleBarPadding             = new Thickness(LogoAndTitleSpacing * 1.8, 0)");
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
