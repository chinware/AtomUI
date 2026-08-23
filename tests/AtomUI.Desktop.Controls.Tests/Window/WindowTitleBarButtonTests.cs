using System.Xml.Linq;
using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Styling;
using Avalonia.VisualTree;
using AtomUI.Icons.AntDesign;
using Shouldly;
using Xunit;

using AtomUIWindow = AtomUI.Desktop.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Window;

public class WindowTitleBarButtonTests
{
    static WindowTitleBarButtonTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Public_AddOn_Controls_Reuse_The_Icon_Button_Family()
    {
        var button = new WindowTitleBarButton();
        var toggleButton = new WindowTitleBarToggleButton();

        button.ShouldBeAssignableTo<IconButton>();
        toggleButton.ShouldBeAssignableTo<ToggleIconButton>();
        button.IsWindowActive.ShouldBeTrue();
        button.HostMotionEnabled.ShouldBeTrue();
        toggleButton.IsWindowActive.ShouldBeTrue();
        toggleButton.HostMotionEnabled.ShouldBeTrue();
    }

    [Fact]
    public void AddOn_Content_Receives_TitleBar_Host_State()
    {
        var button = new WindowTitleBarButton();
        var titleBar = new WindowTitleBar
        {
            LeftAddOn = button,
            IsWindowActive = false,
            IsMotionEnabled = false,
            Title = "Title"
        };
        titleBar.SetValue(WindowTitleBar.OsTypeProperty, OsType.Linux);
        titleBar.Theme = Application.Current!
            .FindResource(typeof(WindowTitleBar))
            .ShouldBeAssignableTo<ControlTheme>();

        var host = new Avalonia.Controls.Window
        {
            Width = 400,
            Height = 100,
            Content = titleBar
        };

        try
        {
            host.Show();
            titleBar.ApplyTemplate();
            host.UpdateLayout();

            var projectedButton = titleBar.GetVisualDescendants()
                                          .OfType<WindowTitleBarButton>()
                                          .Single();

            projectedButton.ShouldBeSameAs(button);
            projectedButton.IsWindowActive.ShouldBeFalse();
            projectedButton.HostMotionEnabled.ShouldBeFalse();

            titleBar.IsWindowActive = true;
            titleBar.IsMotionEnabled = true;
            host.UpdateLayout();

            projectedButton.IsWindowActive.ShouldBeTrue();
            projectedButton.HostMotionEnabled.ShouldBeTrue();
        }
        finally
        {
            host.Close();
        }
    }

    [Fact]
    public void Toggle_AddOn_Preserves_Checked_State_And_Icon_Pair()
    {
        var checkedIcon   = new SearchOutlined();
        var uncheckedIcon = new SettingOutlined();
        var toggleButton = new WindowTitleBarToggleButton
        {
            CheckedIcon   = checkedIcon,
            UnCheckedIcon = uncheckedIcon,
            IsChecked     = false
        };
        toggleButton.Theme = Application.Current!
            .FindResource(typeof(WindowTitleBarToggleButton))
            .ShouldBeAssignableTo<ControlTheme>();

        var host = new Avalonia.Controls.Window
        {
            Width   = 180,
            Height  = 80,
            Content = toggleButton
        };

        try
        {
            host.Show();
            toggleButton.ApplyTemplate();
            host.UpdateLayout();

            var presenters = toggleButton.GetVisualDescendants()
                                          .OfType<IconPresenter>()
                                          .Where(presenter => presenter.Name is not null)
                                          .ToDictionary(presenter => presenter.Name!);
            presenters["PART_CheckedIconPresenter"].Icon.ShouldBeSameAs(checkedIcon);
            presenters["PART_UnCheckedIconPresenter"].Icon.ShouldBeSameAs(uncheckedIcon);
            presenters["PART_CheckedIconPresenter"].IsVisible.ShouldBeFalse();
            presenters["PART_UnCheckedIconPresenter"].IsVisible.ShouldBeTrue();

            toggleButton.IsChecked = true;
            host.UpdateLayout();

            presenters["PART_CheckedIconPresenter"].IsVisible.ShouldBeTrue();
            presenters["PART_UnCheckedIconPresenter"].IsVisible.ShouldBeFalse();
            toggleButton.IsChecked.ShouldBe(true);
        }
        finally
        {
            host.Close();
        }
    }

    [Fact]
    public void AddOn_Content_Returns_To_Standalone_Defaults_When_Removed()
    {
        var button = new WindowTitleBarButton();
        var titleBar = new WindowTitleBar
        {
            LeftAddOn      = button,
            IsWindowActive = false,
            IsMotionEnabled = false,
            Title          = "Title"
        };
        titleBar.SetValue(WindowTitleBar.OsTypeProperty, OsType.Linux);
        titleBar.Theme = Application.Current!
            .FindResource(typeof(WindowTitleBar))
            .ShouldBeAssignableTo<ControlTheme>();

        var host = new Avalonia.Controls.Window
        {
            Width   = 400,
            Height  = 100,
            Content = titleBar
        };

        try
        {
            host.Show();
            titleBar.ApplyTemplate();
            host.UpdateLayout();

            button.IsWindowActive.ShouldBeFalse();
            button.HostMotionEnabled.ShouldBeFalse();

            titleBar.LeftAddOn = null;
            host.UpdateLayout();

            button.IsWindowActive.ShouldBeTrue();
            button.HostMotionEnabled.ShouldBeTrue();
        }
        finally
        {
            host.Close();
        }
    }

    [Fact]
    public void AddOn_Pointer_Input_Does_Not_Bubble_As_TitleBar_Double_Click()
    {
        var button = new WindowTitleBarButton
        {
            Icon = new SearchOutlined()
        };
        var toggleButton = new WindowTitleBarToggleButton
        {
            CheckedIcon = new SearchOutlined(),
            UnCheckedIcon = new SettingOutlined()
        };
        var titleBar = new WindowTitleBar
        {
            LeftAddOn = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Children = { button, toggleButton }
            },
            Title = "Title"
        };
        titleBar.SetValue(WindowTitleBar.OsTypeProperty, OsType.Linux);
        titleBar.Theme = Application.Current!
            .FindResource(typeof(WindowTitleBar))
            .ShouldBeAssignableTo<ControlTheme>();

        var maximizeRequests = 0;
        titleBar.MaximizeWindowRequested += (_, _) => maximizeRequests++;

        var host = new AtomUIWindow
        {
            IsTitleBarVisible = false,
            Content = titleBar
        };

        try
        {
            host.Show();
            titleBar.ApplyTemplate();
            host.UpdateLayout();

            RaiseDoubleClick(button);
            RaiseDoubleClick(toggleButton);

            maximizeRequests.ShouldBe(0);
        }
        finally
        {
            host.Close();
        }
    }

    [Fact]
    public void AddOn_Themes_Are_Registered_As_Independent_Assets_And_Stay_Out_Of_System_Caption_Contract()
    {
        Application.Current!
            .FindResource(typeof(WindowTitleBarButton))
            .ShouldBeAssignableTo<ControlTheme>();
        Application.Current!
            .FindResource(typeof(WindowTitleBarToggleButton))
            .ShouldBeAssignableTo<ControlTheme>();

        var manifest = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/GeneratedFiles/AtomUI.Generator/AtomUI.Generator.ThemeAssetManifestGenerator/GeneratedControlThemeAssetManifest.g.cs"));
        manifest.ShouldContain("WindowTitleBarButtonTheme.axaml");
        manifest.ShouldContain("WindowTitleBarToggleButtonTheme.axaml");

        var buttonTheme = XDocument.Load(GetRepoFile(
            "src/AtomUI.Desktop.Controls/WindowTitleBar/Themes/WindowTitleBarButtonTheme.axaml"));
        var toggleTheme = XDocument.Load(GetRepoFile(
            "src/AtomUI.Desktop.Controls/WindowTitleBar/Themes/WindowTitleBarToggleButtonTheme.axaml"));
        foreach (var theme in new[] { buttonTheme, toggleTheme })
        {
            theme.ToString().ShouldNotContain("CaptionButtonAction");
            theme.ToString().ShouldNotContain("ElementRole");
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

    private static void RaiseDoubleClick(Control source)
    {
        var pointer = new Pointer(
            Pointer.GetNextFreeId(),
            PointerType.Mouse,
            true);
        source.RaiseEvent(new PointerPressedEventArgs(
            source,
            pointer,
            source,
            default,
            0,
            new PointerPointProperties(
                RawInputModifiers.LeftMouseButton,
                PointerUpdateKind.LeftButtonPressed),
            KeyModifiers.None,
            2));
        source.RaiseEvent(new PointerReleasedEventArgs(
            source,
            pointer,
            source,
            default,
            1,
            new PointerPointProperties(
                RawInputModifiers.None,
                PointerUpdateKind.LeftButtonReleased),
            KeyModifiers.None,
            MouseButton.Left));
    }
}
