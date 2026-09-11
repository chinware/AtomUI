using System.Xml.Linq;
using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
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
        button.HostOsType.ShouldBe(OsType.Unknown);
        toggleButton.IsWindowActive.ShouldBeTrue();
        toggleButton.HostMotionEnabled.ShouldBeTrue();
        toggleButton.HostOsType.ShouldBe(OsType.Unknown);
    }

    [Fact]
    public void Windows_AddOn_Button_Uses_Full_Height_Square_Caption_Geometry()
    {
        var button = new WindowTitleBarButton
        {
            Icon = new SearchOutlined()
        };
        var titleBar = new WindowTitleBar
        {
            RightAddOn = button,
            IsWindowActive = true,
            IsMotionEnabled = false,
            Title = "Title",
            Height = 44
        };
        titleBar.SetValue(WindowTitleBar.OsTypeProperty, OsType.Windows);
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
            button.ApplyTemplate();
            host.UpdateLayout();

            button.DesiredSize.ShouldBe(new Size(44, 44));
            button.Bounds.Size.ShouldBe(new Size(44, 44));
            button.CornerRadius.ShouldBe(new CornerRadius(0));
            button.VerticalAlignment.ShouldBe(VerticalAlignment.Stretch);
            button.Cursor.ShouldNotBeNull();
            button.Cursor!.ToString().ShouldContain("Arrow");
            button.Background!
                  .ShouldBeAssignableTo<ISolidColorBrush>()
                  .Color.ShouldBe(Colors.Transparent);

            var point = button.TranslatePoint(
                new Point(button.Bounds.Width / 2, button.Bounds.Height / 2),
                host).ShouldNotBeNull();
            host.MouseMove(point);
            Dispatcher.UIThread.RunJobs();

            button.IsPointerOver.ShouldBeTrue();
            var hoverColor = button.Background!
                                   .ShouldBeAssignableTo<ISolidColorBrush>()
                                   .Color;
            hoverColor.ShouldNotBe(Colors.Transparent);

            host.MouseDown(point, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();

            button.Background!
                  .ShouldBeAssignableTo<ISolidColorBrush>()
                  .Color.ShouldNotBe(hoverColor);

            host.MouseUp(point, MouseButton.Left);
            host.MouseMove(new Point(10, 90));
            Dispatcher.UIThread.RunJobs();

            button.IsPointerOver.ShouldBeFalse();
            button.Background!
                  .ShouldBeAssignableTo<ISolidColorBrush>()
                  .Color.ShouldBe(Colors.Transparent);

            titleBar.IsWindowActive = false;
            host.MouseMove(point);
            Dispatcher.UIThread.RunJobs();

            var inactiveHoverColor = button.Background!
                                           .ShouldBeAssignableTo<ISolidColorBrush>()
                                           .Color;
            inactiveHoverColor.ShouldNotBe(Colors.Transparent);

            host.MouseDown(point, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();

            button.Background!
                  .ShouldBeAssignableTo<ISolidColorBrush>()
                  .Color.ShouldNotBe(inactiveHoverColor);

            host.MouseUp(point, MouseButton.Left);
            button.IsEnabled = false;
            Dispatcher.UIThread.RunJobs();

            button.Background!
                  .ShouldBeAssignableTo<ISolidColorBrush>()
                  .Color.ShouldBe(Colors.Transparent);

            button.IsEnabled = true;
            titleBar.IsWindowActive = true;
            host.MouseMove(new Point(10, 90));
            Dispatcher.UIThread.RunJobs();

            titleBar.SetValue(WindowTitleBar.OsTypeProperty, OsType.Linux);
            host.UpdateLayout();

            button.HostOsType.ShouldBe(OsType.Linux);
            button.DesiredSize.ShouldBe(new Size(30, 30));

            titleBar.SetValue(WindowTitleBar.OsTypeProperty, OsType.Windows);
            host.UpdateLayout();

            button.HostOsType.ShouldBe(OsType.Windows);
            button.DesiredSize.ShouldBe(new Size(44, 44));
        }
        finally
        {
            host.Close();
        }
    }

    [Fact]
    public void Windows_AddOn_Toggle_Button_Uses_Full_Height_Square_Caption_Geometry()
    {
        var button = new WindowTitleBarToggleButton
        {
            CheckedIcon = new SearchOutlined(),
            UnCheckedIcon = new SettingOutlined()
        };
        var titleBar = new WindowTitleBar
        {
            LeftAddOn = button,
            IsWindowActive = true,
            Title = "Title"
        };
        titleBar.SetValue(WindowTitleBar.OsTypeProperty, OsType.Windows);
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
            button.ApplyTemplate();
            host.UpdateLayout();

            button.DesiredSize.ShouldBe(new Size(40, 40));
            button.Bounds.Size.ShouldBe(new Size(40, 40));
            button.CornerRadius.ShouldBe(new CornerRadius(0));
            button.VerticalAlignment.ShouldBe(VerticalAlignment.Stretch);
            button.Cursor.ShouldNotBeNull();
            button.Cursor!.ToString().ShouldContain("Arrow");
            button.Background!
                  .ShouldBeAssignableTo<ISolidColorBrush>()
                  .Color.ShouldBe(Colors.Transparent);
        }
        finally
        {
            host.Close();
        }
    }

    [Fact]
    public void Non_Windows_AddOn_Buttons_Preserve_Managed_Button_Geometry()
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
            IsWindowActive = true,
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
            button.ApplyTemplate();
            toggleButton.ApplyTemplate();
            host.UpdateLayout();

            button.DesiredSize.ShouldBe(new Size(30, 30));
            toggleButton.DesiredSize.ShouldBe(new Size(30, 30));
            button.CornerRadius.ShouldNotBe(new CornerRadius(0));
            toggleButton.CornerRadius.ShouldNotBe(new CornerRadius(0));
            button.VerticalAlignment.ShouldBe(VerticalAlignment.Center);
            toggleButton.VerticalAlignment.ShouldBe(VerticalAlignment.Center);
            button.Cursor!.ToString().ShouldContain("Hand");
            toggleButton.Cursor!.ToString().ShouldContain("Hand");
        }
        finally
        {
            host.Close();
        }
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
            projectedButton.HostOsType.ShouldBe(OsType.Linux);

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
            button.HostOsType.ShouldBe(OsType.Linux);

            titleBar.LeftAddOn = null;
            host.UpdateLayout();

            button.IsWindowActive.ShouldBeTrue();
            button.HostMotionEnabled.ShouldBeTrue();
            button.HostOsType.ShouldBe(OsType.Unknown);
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
        XNamespace xaml = "http://schemas.microsoft.com/winfx/2006/xaml";
        foreach (var theme in new[] { buttonTheme, toggleTheme })
        {
            theme.Root.ShouldNotBeNull();
            theme.Root!.Name.LocalName.ShouldBe("ControlTheme");
            theme.Root.Attribute(xaml + "Class").ShouldNotBeNull();
            theme.Root.Descendants()
                 .ShouldAllBe(element => element.Attribute(xaml + "Class") == null);
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
