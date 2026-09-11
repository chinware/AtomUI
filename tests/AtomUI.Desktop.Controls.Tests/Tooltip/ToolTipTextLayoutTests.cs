using System.Xml.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomUIWindow = AtomUI.Desktop.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Tooltip;

public class ToolTipTextLayoutTests
{
    private const double ToolTipMaxWidth = 250;

    static ToolTipTextLayoutTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Long_Text_Tip_Wraps_Within_MaxWidth()
    {
        var host = new Button();
        ToolTip.SetTip(host, string.Join(' ', Enumerable.Repeat("long tooltip text", 40)));

        var window = ShowInWindow(host);
        try
        {
            ToolTip.SetIsOpen(host, true);
            Dispatcher.UIThread.RunJobs();

            var textBlock = FindTipTextBlock(host);
            textBlock.TextWrapping.ShouldBe(TextWrapping.Wrap, "长文本应在 ToolTipMaxWidth 内换行而不是被裁剪");

            var singleLineHeight = MeasureSingleLineHeight(textBlock);
            textBlock.DesiredSize.Height.ShouldBeGreaterThan(singleLineHeight * 1.5,
                "换行后内容高度应明显超过单行高度");
            textBlock.DesiredSize.Width.ShouldBeLessThanOrEqualTo(ToolTipMaxWidth,
                "换行后内容宽度不应超过 ToolTipMaxWidth");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void TextWrapping_And_TextTrimming_Attached_Properties_Have_Antd_Aligned_Defaults()
    {
        var host = new Button();

        ToolTip.GetTextWrapping(host).ShouldBe(TextWrapping.Wrap);
        ToolTip.GetTextTrimming(host).ShouldBe(TextTrimming.None);
    }

    [Fact]
    public void TextWrapping_Attached_Property_Updates_Opened_ToolTip_Live()
    {
        var host = new Button();
        ToolTip.SetTip(host, string.Join(' ', Enumerable.Repeat("long tooltip text", 40)));

        var window = ShowInWindow(host);
        try
        {
            ToolTip.SetIsOpen(host, true);
            Dispatcher.UIThread.RunJobs();

            var textBlock = FindTipTextBlock(host);
            textBlock.TextWrapping.ShouldBe(TextWrapping.Wrap);

            ToolTip.SetTextWrapping(host, TextWrapping.NoWrap);
            Dispatcher.UIThread.RunJobs();

            textBlock.TextWrapping.ShouldBe(TextWrapping.NoWrap,
                "打开后修改附加属性应通过活绑定生效，而不是打开时的一次性快照");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void TextTrimming_Attached_Property_Updates_Opened_ToolTip_Live()
    {
        var host = new Button();
        ToolTip.SetTip(host, string.Join(' ', Enumerable.Repeat("long tooltip text", 40)));

        var window = ShowInWindow(host);
        try
        {
            ToolTip.SetIsOpen(host, true);
            Dispatcher.UIThread.RunJobs();

            var textBlock = FindTipTextBlock(host);
            textBlock.TextTrimming.ShouldBe(TextTrimming.None);

            ToolTip.SetTextTrimming(host, TextTrimming.CharacterEllipsis);
            Dispatcher.UIThread.RunJobs();

            textBlock.TextTrimming.ShouldBe(TextTrimming.CharacterEllipsis);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Theme_Connects_TextWrapping_On_ContentPresenter()
    {
        var document = XDocument.Load(GetRepoFile("src/AtomUI.Desktop.Controls/Tooltip/Themes/ToolTipTheme.axaml"));
        XNamespace av = "https://github.com/avaloniaui";

        var presenter = document.Descendants(av + "ContentPresenter")
                                .Single(element => (string?)element.Attribute("Name") == "PART_ContentPresenter");

        presenter.Attribute("TextBlock.TextWrapping").ShouldNotBeNull(
            "主题契约必须给出换行行为，否则 ToolTipMaxWidth 只会裁剪长文本").Value.ShouldBe("Wrap");
    }

    private static Avalonia.Controls.TextBlock FindTipTextBlock(Control host)
    {
        var toolTip = host.GetValue(ToolTip.ToolTipProperty);
        toolTip.ShouldNotBeNull();
        toolTip.Classes.Contains(ToolTipPseudoClass.Open).ShouldBeTrue();

        var presenter = toolTip.GetVisualDescendants()
                               .OfType<Avalonia.Controls.Presenters.ContentPresenter>()
                               .FirstOrDefault(p => p.Name == "PART_ContentPresenter");
        presenter.ShouldNotBeNull(string.Join(", ",
            toolTip.GetVisualDescendants().Select(v => v.GetType().Name)));
        // headless 下 child 可能尚未实现化；UpdateChild 是幂等的
        presenter.UpdateChild();
        presenter.Child.ShouldBeOfType<Avalonia.Controls.TextBlock>();
        return (Avalonia.Controls.TextBlock)presenter.Child!;
    }

    private static double MeasureSingleLineHeight(Avalonia.Controls.TextBlock tipTextBlock)
    {
        var reference = new Avalonia.Controls.TextBlock
        {
            Text     = "long tooltip text",
            FontSize = tipTextBlock.FontSize
        };
        reference.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        return reference.DesiredSize.Height;
    }

    private static AtomUIWindow ShowInWindow(Control content)
    {
        // headless 平台没有原生 popup 窗口实现，弹层只能走 overlay host
        ToolTip.SetIsUseOverlayHost(content, true);
        var window = new AtomUIWindow
        {
            Width   = 800,
            Height  = 600,
            Content = content
        };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        return window;
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
