using System.Reflection;
using System.Windows.Input;
using System.Xml.Linq;
using AtomUI.Theme.Resources;
using AtomUIGallery.Workspace.Views;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.Workspace;

public class WorkspaceWindowLayoutTests
{
    [Fact]
    public void Sidebar_Brand_Area_Does_Not_Show_Desktop_Gallery_Text()
    {
        var source = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/Workspace/Views/WorkspaceWindow.axaml"));
        var moduleSource = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/AtomUIGalleryModule.cs"));

        source.ShouldNotContain("Text=\"Desktop Gallery\"");
        source.ShouldNotContain("avares://AtomUIGallery/Assets/atomui-oss.svg");
        source.ShouldContain("ShellHost");
        moduleSource.ShouldContain("avares://AtomUIGallery/Assets/atomui-oss.svg");
        source.ShouldNotContain("avares://AtomUIGallery/Assets/atomui-red.svg");
    }

    [Fact]
    public void Sidebar_Navigation_Does_Not_Use_Fixed_Menu_Width()
    {
        var source = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/Workspace/Views/CaseNavigation.axaml"));

        source.ShouldNotContain("Width=\"260\"");
        source.ShouldContain("HorizontalAlignment=\"Stretch\"");
    }

    [Fact]
    public void Sidebar_Does_Not_Show_Search_Box()
    {
        var source = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/Workspace/Views/WorkspaceWindow.axaml"));

        source.ShouldContain("Name=\"ShellHost\"");
        source.ShouldNotContain("<atom:SearchEdit");
        source.ShouldNotContain("Search components...");
        source.ShouldNotContain("<workspaceviews:CaseNavigation Grid.Row=\"1\"");
        source.ShouldNotContain("<Border Grid.Row=\"2\"");
    }

    [Fact]
    public void Sidebar_Navigation_Does_Not_Reserve_Divider_Gap()
    {
        var source = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/Workspace/Views/CaseNavigation.axaml"));

        source.ShouldNotContain("Margin=\"0,0,2,0\"");
    }

    [Fact]
    public void Workspace_Draws_Navigation_Content_Separator()
    {
        var source = File.ReadAllText(GetRepoFile("src/AtomUI.Toolkits.GalleryBase/Shell/GalleryShellView.cs"));

        source.ShouldContain("Name             = \"WorkspaceNavigationSeparator\"");
        source.ShouldContain("Grid.SetColumn(_navigationSeparator, 1)");
        source.ShouldContain("Width            = 1");
        source.ShouldContain("HorizontalAlignment = HorizontalAlignment.Left");
        source.ShouldContain("SharedTokenKind.ColorBorderSecondary");
        source.ShouldContain("IsHitTestVisible = false");
        source.ShouldNotContain("BorderThickness=\"0,0,1,0\"");
    }

    [Fact]
    public void Source_Code_Drawer_Masks_Entire_Shell_Instead_Of_Content_Column()
    {
        var source = File.ReadAllText(GetRepoFile("src/AtomUI.Toolkits.GalleryBase/Shell/GalleryShellView.cs"));

        source.ShouldContain("Child = RoutedViewHost");
        source.ShouldContain("PageContent = rootLayout");
        source.ShouldContain("Content = codeDrawerHost");
        source.ShouldNotContain("PageContent = RoutedViewHost");
        source.ShouldNotContain("Child = codeDrawerHost");
    }

    [Fact]
    public void Sidebar_Navigation_Does_Not_Override_Selected_Background()
    {
        var source = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/Workspace/Views/CaseNavigation.axaml"));

        source.ShouldNotContain("<UserControl.Styles>");
        source.ShouldNotContain("BaseNavMenuItemHeader[IsSelected=True]");
        source.ShouldNotContain("IsDarkStyle=True][IsSelected=True]");
        source.ShouldNotContain("Value=\"#");
    }

    [Fact]
    public void Sidebar_Navigation_Does_Not_Couple_Dark_Menu_Style_To_Global_Dark_Mode()
    {
        var viewSource = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/Workspace/Views/CaseNavigation.axaml"));
        var codeBehindSource = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/Workspace/Views/CaseNavigation.axaml.cs"));

        viewSource.ShouldNotContain("IsDarkStyle=\"True\"");
        codeBehindSource.ShouldNotContain("IThemeManager.IsDarkThemeModeProperty");
        codeBehindSource.ShouldNotContain("NavMenu.IsDarkStyleProperty");
        codeBehindSource.ShouldContain("ShowCaseNavMenu");
    }

    [Fact]
    public void Sidebar_Width_Is_Twenty_Pixels_Narrower()
    {
        var configuration = global::AtomUIGallery.AtomUIGalleryModule.CreateConfiguration();

        configuration.Shell.SidebarWidth.ShouldBe(280);
    }

    [Fact]
    public void Workspace_Window_Has_Minimum_Width_To_Protect_Main_Content()
    {
        var source = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/Workspace/Views/WorkspaceWindow.axaml"));

        source.ShouldContain("MinWidth=\"520\"");
        source.ShouldNotContain("MinWidth=\"1040\"");
        source.ShouldNotContain("MinWidth=\"1200\"");
    }

    [Fact]
    public void Workspace_Window_Draws_TitleBar_Bottom_Separator_With_Secondary_Border_Color()
    {
        var source = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/Workspace/Views/WorkspaceWindow.axaml"));

        source.ShouldContain("Name=\"TitleBarBottomSeparator\"");
        source.ShouldContain("Height=\"1\"");
        source.ShouldContain("Background=\"{atom:SharedTokenResource ColorBorderSecondary}\"");
        source.ShouldContain("IsHitTestVisible=\"False\"");
    }

    [Fact]
    public void Workspace_Window_Menu_Keeps_Motion_And_WaveSpirit_Checks_In_Sync()
    {
        var source = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/Workspace/Views/WorkspaceWindow.axaml.cs"));

        source.ShouldContain("FindSiblingMenuItem(menuItem, WindowMenuItemKind.WaveSpirit)");
        source.ShouldContain("waveSpiritMenuItem.IsChecked = false");
        source.ShouldContain("FindSiblingMenuItem(menuItem, WindowMenuItemKind.Motion)");
        source.ShouldContain("motionMenuItem.IsChecked = true");
    }

    [Fact]
    public void Workspace_Window_Builds_Theme_Color_Radio_Items_From_The_ViewModel()
    {
        var viewSource = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/Workspace/Views/WorkspaceWindow.axaml"));
        var codeSource = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/Workspace/Views/WorkspaceWindow.axaml.cs"));

        viewSource.ShouldContain("WindowMenuItemKind.ThemeCatalog");
        viewSource.ShouldContain("<atom:MenuSeparator />");
        codeSource.ShouldContain("ViewModel.AvailableThemes");
        codeSource.ShouldContain("GroupName        = ThemeColorGroupName");
        codeSource.ShouldContain("var switchThemeCommand = new StableCommand(ViewModel.SwitchThemeCommand)");
        codeSource.ShouldContain("Command          = switchThemeCommand");
        codeSource.ShouldNotContain("Command          = ViewModel.SwitchThemeCommand");
        codeSource.ShouldContain("CommandParameter = theme.Id");
        codeSource.ShouldContain("theme.AccentColor is { } accentColor");
        codeSource.ShouldContain("Width               = 12");
        codeSource.ShouldContain("Height              = 12");
        codeSource.ShouldContain("CornerRadius        = new CornerRadius(2)");
        codeSource.ShouldContain("Background          = new SolidColorBrush(accentColor)");
        codeSource.ShouldNotContain("ReloadThemesCommand");
    }

    [Fact]
    public void Workspace_Window_Theme_Command_Does_Not_Forward_CanExecuteChanged_To_Menu_Items()
    {
        var innerCommand  = new RecordingCommand();
        var stableCommand = CreateStableThemeCommand(innerCommand);
        var raiseCount    = 0;
        stableCommand.CanExecuteChanged += (_, _) => raiseCount++;

        innerCommand.RaiseCanExecuteChanged();

        raiseCount.ShouldBe(0);
        stableCommand.CanExecute("PolarGreen").ShouldBeTrue();
        stableCommand.Execute("PolarGreen");
        innerCommand.ExecuteParameters.ShouldBe(["PolarGreen"]);
    }

    [Fact]
    public void Workspace_Window_Nests_Theme_Choices_Under_A_Localized_Settings_Submenu()
    {
        var source = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/Workspace/Views/WorkspaceWindow.axaml"));
        var document = XDocument.Parse(source);
        XNamespace atom = "https://atomui.net";
        var topLevelItems = document.Descendants(atom + "Menu")
                                    .Single()
                                    .Elements(atom + "MenuItem")
                                    .ToArray();
        var themeMenuItem = topLevelItems.Single(static item =>
            item.Attribute("Header")?.Value.Contains("MenuItemTheme}", StringComparison.Ordinal) == true);

        themeMenuItem.Attribute("Tag").ShouldBeNull();
        var children = themeMenuItem.Elements().ToArray();
        children.Length.ShouldBe(6);
        children[0].Name.ShouldBe(atom + "MenuItem");
        children[0].Attribute("Header")!.Value.ShouldContain("MenuItemThemeSettings");
        children[0].Attribute("Tag")!.Value.ShouldContain("WindowMenuItemKind.ThemeCatalog");
        children[1].Name.ShouldBe(atom + "MenuSeparator");
        children[2].Attribute("Header")!.Value.ShouldContain("MenuItemAppearance");
        children[2].Attribute("Tag").ShouldBeNull();
        var appearanceItems = children[2].Elements(atom + "MenuItem").ToArray();
        appearanceItems.Length.ShouldBe(3);
        appearanceItems[0].Attribute("Header")!.Value.ShouldContain("MenuItemLightMode");
        appearanceItems[0].Attribute("ToggleType")!.Value.ShouldBe("Radio");
        appearanceItems[0].Attribute("Tag")!.Value.ShouldContain("WindowMenuItemKind.LightMode");
        appearanceItems[1].Attribute("Header")!.Value.ShouldContain("MenuItemDarkMode");
        appearanceItems[1].Attribute("ToggleType")!.Value.ShouldBe("Radio");
        appearanceItems[1].Attribute("Tag")!.Value.ShouldContain("WindowMenuItemKind.DarkMode");
        appearanceItems[2].Attribute("Header")!.Value.ShouldContain("MenuItemFollowSystem");
        appearanceItems[2].Attribute("ToggleType")!.Value.ShouldBe("Radio");
        appearanceItems[2].Attribute("Tag")!.Value.ShouldContain("WindowMenuItemKind.FollowSystem");
        children[3].Attribute("Tag")!.Value.ShouldContain("WindowMenuItemKind.Compact");
        children[4].Attribute("Tag")!.Value.ShouldContain("WindowMenuItemKind.Motion");
        children[5].Attribute("Tag")!.Value.ShouldContain("WindowMenuItemKind.WaveSpirit");
    }

    [Theory]
    [InlineData("en-US.xlf", "MenuItemThemeSettings", "Theme Settings")]
    [InlineData("en-US.xlf", "MenuItemAppearance", "Appearance")]
    [InlineData("en-US.xlf", "MenuItemLightMode", "Light Mode")]
    [InlineData("en-US.xlf", "MenuItemFollowSystem", "Follow System")]
    [InlineData("zh-CN.xlf", "MenuItemThemeSettings", "主题设置")]
    [InlineData("zh-CN.xlf", "MenuItemAppearance", "外观模式")]
    [InlineData("zh-CN.xlf", "MenuItemLightMode", "明亮模式")]
    [InlineData("zh-CN.xlf", "MenuItemFollowSystem", "跟随系统")]
    [InlineData("zh-TW.xlf", "MenuItemThemeSettings", "主題設定")]
    [InlineData("zh-TW.xlf", "MenuItemAppearance", "外觀模式")]
    [InlineData("zh-TW.xlf", "MenuItemLightMode", "明亮模式")]
    [InlineData("zh-TW.xlf", "MenuItemFollowSystem", "跟隨系統")]
    public void Workspace_Window_Localizes_The_Theme_Settings_Submenu(
        string fileName,
        string resourceName,
        string expectedText)
    {
        var localization = XliffTestDocument.Read(
            $"controlgallery/AtomUIGallery/Workspace/Localization/WorkspaceWindowLang/{fileName}");

        localization[resourceName].ShouldBe(expectedText);
    }

    [Fact]
    public void Sidebar_Footer_Shows_Website_Gitee_And_Github_Links_With_Larger_Tighter_Icons()
    {
        var moduleSource = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/AtomUIGalleryModule.cs"));
        var shellSource = File.ReadAllText(GetRepoFile("src/AtomUI.Toolkits.GalleryBase/Shell/GalleryShellView.cs"));

        moduleSource.ShouldContain("https://www.atomui.net");
        moduleSource.ShouldContain("AntDesignIconKind.GlobalOutlined");
        moduleSource.ShouldContain("https://gitee.com/chinware/AtomUI");
        moduleSource.ShouldContain("AntDesignIconKind.GiteeOutlined");
        moduleSource.ShouldContain("https://github.com/chinware/atomui");
        moduleSource.ShouldContain("AntDesignIconKind.GithubOutlined");
        shellSource.ShouldContain("Spacing     = 0");
        shellSource.ShouldContain("IconWidth   = 22");
        shellSource.ShouldContain("IconHeight  = 22");
        shellSource.ShouldNotContain("FontSize = 22");
    }

    [Fact]
    public void Sidebar_Footer_Shows_AtomUI_Version_As_Green_Tag()
    {
        var moduleSource = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/AtomUIGalleryModule.cs"));
        var shellSource = File.ReadAllText(GetRepoFile("src/AtomUI.Toolkits.GalleryBase/Shell/GalleryShellView.cs"));

        shellSource.ShouldContain("new DesktopTag");
        shellSource.ShouldContain("TagColor            = \"Green\"");
        moduleSource.ShouldContain("GalleryVersionInfo.DisplayVersion");
        shellSource.ShouldNotContain("Text = \"v0.9.8\"");
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

    private static ICommand CreateStableThemeCommand(ICommand innerCommand)
    {
        var commandType = typeof(WorkspaceWindow).GetNestedType("StableCommand", BindingFlags.NonPublic);
        commandType.ShouldNotBeNull();
        var constructor = commandType!.GetConstructor(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            binder: null,
            types: [typeof(ICommand)],
            modifiers: null);
        constructor.ShouldNotBeNull();
        return (ICommand)constructor.Invoke([innerCommand]);
    }

    private sealed class RecordingCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public List<object?> ExecuteParameters { get; } = [];

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            ExecuteParameters.Add(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
