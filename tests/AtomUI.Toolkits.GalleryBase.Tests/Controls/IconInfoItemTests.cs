using AtomUI.Controls;
using AtomUI.Toolkits.GalleryBase.Controls;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Shouldly;
using Xunit;

namespace AtomUI.Toolkits.GalleryBase.Tests.Controls;

public class IconInfoItemTests
{
    static IconInfoItemTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void IconInfoItem_Uses_Standard_Button_And_Motion_Contracts()
    {
        var item = new IconInfoItem();

        item.ShouldBeAssignableTo<Button>();
        item.ShouldBeAssignableTo<IMotionAwareControl>();
    }

    [Fact]
    public void IconInfoItem_Raises_Standard_Button_Click_Event()
    {
        var item       = new IconInfoItem();
        var button     = item.ShouldBeAssignableTo<Button>();
        var clickCount = 0;
        button.Click += (_, _) => clickCount++;

        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, button));

        clickCount.ShouldBe(1);
    }

    [Fact]
    public void IconInfoItem_Exposes_Icon_Name_To_Automation()
    {
        var item = new IconInfoItem
        {
            IconName = "HomeOutlined"
        };
        var window = new Window
        {
            Content = item
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var peer = ControlAutomationPeer.CreatePeerForElement(item);

            peer.ShouldNotBeNull();
            peer.GetName().ShouldBe(item.IconName);
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    [Fact]
    public void IconInfoItem_Theme_Defines_Interactive_Visual_States()
    {
        var source = ReadRepoFile(
            "src/AtomUI.Toolkits.GalleryBase/Controls/Themes/IconInfoItemTheme.axaml");

        source.ShouldContain("<Setter Property=\"Cursor\" Value=\"Hand\" />");
        source.ShouldContain("<Setter Property=\"IsMotionEnabled\" Value=\"{atom:SharedTokenResource EnableMotion}\" />");
        source.ShouldContain("Selector=\"^:pointerover:not(:disabled)\"");
        source.ShouldContain("ColorBgTextHover");
        source.ShouldContain("Selector=\"^:pressed:not(:disabled)\"");
        source.ShouldContain("ColorBgTextActive");
        source.ShouldContain("Selector=\"^:focus-visible");
        source.ShouldContain("Selector=\"^:disabled\"");
        source.ShouldContain("<Setter Property=\"Cursor\" Value=\"Arrow\" />");
        source.ShouldContain("Selector=\"^:disabled /template/ atom|IconPresenter\"");
        source.ShouldContain("Selector=\"^[IsMotionEnabled=True]\"");
        source.ShouldContain("SolidColorBrushTransition Property=\"Background\"");
    }

    private static string ReadRepoFile(string relativePath)
    {
        var path = GetRepoFile(relativePath);
        File.Exists(path).ShouldBeTrue($"Expected repository file to exist: {relativePath}");
        return File.ReadAllText(path);
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

        return Path.Combine(AppContext.BaseDirectory, relativePath);
    }
}
