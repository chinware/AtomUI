using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using AtomUI.Toolkits.GalleryBase.Shell;
using AtomUIGallery.Workspace.Views;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomUIToolTip = AtomUI.Desktop.Controls.ToolTip;
using AvaloniaToolTip = Avalonia.Controls.ToolTip;
using DesktopButton = AtomUI.Desktop.Controls.Button;

namespace AtomUIGallery.Tests.Workspace;

public class CaseNavigationLayoutTests
{
    [Fact]
    public void Sidebar_NavMenu_Uses_AntDesign_Item_Margins_Without_Outer_Menu_Padding()
    {
        var source = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/Workspace/Views/CaseNavigation.axaml"));

        source.ShouldContain("Name=\"ShowCaseNavMenu\"");
        source.ShouldContain("Padding=\"0\"");
        source.ShouldContain("IsItemBackgroundEnabled=\"False\"");
        source.ShouldNotContain("<atom:NavMenu.Styles>");
        source.ShouldNotContain("InlineNavMenuItemHeader");
        source.ShouldNotContain("<Setter Property=\"Margin\" Value=\"0,0,0,4\" />");
        source.ShouldNotContain("<Setter Property=\"Margin\" Value=\"0\" />");
        source.ShouldNotContain("<atom:NavMenu.Header>");
    }

    [Fact]
    public void Sidebar_Navigation_Implements_The_Explicit_NavMenu_Host_Contract()
    {
        AvaloniaTestApp.EnsureInitialized();
        var navigation = new CaseNavigation();
        var navMenu = navigation.FindControl<NavMenu>("ShowCaseNavMenu");
        var collapseButton = GetSidebarHeaderAction(navigation);

        navMenu.ShouldNotBeNull();
        ((IGallerySidebarNavMenuHost)navigation).SidebarNavMenu.ShouldBeSameAs(navMenu);
        collapseButton.Name.ShouldBe("NavigationCollapseButton");
    }

    [Fact]
    public void Sidebar_Collapse_Button_Changes_Only_The_NavMenu_Collapsed_State()
    {
        AvaloniaTestApp.EnsureInitialized();
        var navigation = new CaseNavigation();
        var navMenu = navigation.FindControl<NavMenu>("ShowCaseNavMenu")!;
        var collapseButton = GetSidebarHeaderAction(navigation);
        var firstEntry = navMenu.Items[0];

        navMenu.IsInlineCollapsed.ShouldBeFalse();
        collapseButton.Icon.ShouldBeOfType<MenuFoldOutlined>();
        collapseButton.HorizontalAlignment.ShouldBe(HorizontalAlignment.Center);
        AutomationProperties.GetName(collapseButton).ShouldNotBeNullOrWhiteSpace();

        collapseButton.RaiseEvent(new RoutedEventArgs(DesktopButton.ClickEvent));

        navMenu.IsInlineCollapsed.ShouldBeTrue();
        navMenu.Items[0].ShouldBeSameAs(firstEntry);
        collapseButton.Icon.ShouldBeOfType<MenuUnfoldOutlined>();
        collapseButton.HorizontalAlignment.ShouldBe(HorizontalAlignment.Center);
        AutomationProperties.GetName(collapseButton).ShouldNotBeNullOrWhiteSpace();

        collapseButton.RaiseEvent(new RoutedEventArgs(DesktopButton.ClickEvent));

        navMenu.IsInlineCollapsed.ShouldBeFalse();
        navMenu.Items[0].ShouldBeSameAs(firstEntry);
        collapseButton.Icon.ShouldBeOfType<MenuFoldOutlined>();
        collapseButton.HorizontalAlignment.ShouldBe(HorizontalAlignment.Center);
    }

    [Fact]
    public void Sidebar_Collapse_Button_Does_Not_Create_A_ToolTip()
    {
        AvaloniaTestApp.EnsureInitialized();
        var navigation = new CaseNavigation();
        var collapseButton = GetSidebarHeaderAction(navigation);

        AtomUIToolTip.GetTip(collapseButton).ShouldBeNull();
        AvaloniaToolTip.GetTip(collapseButton).ShouldBeNull();

        collapseButton.RaiseEvent(new RoutedEventArgs(DesktopButton.ClickEvent));

        AtomUIToolTip.GetTip(collapseButton).ShouldBeNull();
        AvaloniaToolTip.GetTip(collapseButton).ShouldBeNull();
    }

    [Theory]
    [InlineData("en-US.xlf", "CollapseNavigation", "Collapse navigation")]
    [InlineData("en-US.xlf", "ExpandNavigation", "Expand navigation")]
    [InlineData("zh-CN.xlf", "CollapseNavigation", "收起导航")]
    [InlineData("zh-CN.xlf", "ExpandNavigation", "展开导航")]
    [InlineData("zh-TW.xlf", "CollapseNavigation", "收起導覽")]
    [InlineData("zh-TW.xlf", "ExpandNavigation", "展開導覽")]
    public void Sidebar_Collapse_Button_Has_Localized_Accessible_Text(
        string fileName,
        string resourceName,
        string expectedText)
    {
        var localization = XliffTestDocument.Read(
            $"controlgallery/AtomUIGallery/Workspace/Localization/CaseNavigationLang/{fileName}");

        localization[resourceName].ShouldBe(expectedText);
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

    private static DesktopButton GetSidebarHeaderAction(CaseNavigation navigation)
    {
        return ((IGallerySidebarNavMenuHost)navigation).SidebarHeaderAction.ShouldBeOfType<DesktopButton>();
    }
}
