using Avalonia.Controls;
using Avalonia.Threading;
using Shouldly;
using Xunit;
using AtomUIWindow = AtomUI.Desktop.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Tooltip;

public class ToolTipOpenStateReconciliationTests
{
    static ToolTipOpenStateReconciliationTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void IsOpen_Set_Before_Attach_Opens_After_Attach()
    {
        var host = new Button();
        ToolTip.SetTip(host, "tip");
        ToolTip.SetIsOpen(host, true);

        var window = ShowInWindow(host);
        try
        {
            ToolTip.GetIsOpen(host).ShouldBeTrue();
            var toolTip = host.GetValue(ToolTip.ToolTipProperty);
            toolTip.ShouldNotBeNull();
            toolTip.Classes.Contains(ToolTipPseudoClass.Open).ShouldBeTrue();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Tip_Set_After_IsOpen_Still_Opens()
    {
        var host = new Button();
        ToolTip.SetIsOpen(host, true);
        ToolTip.GetIsOpen(host).ShouldBeTrue("Tip 未就绪不得重置 IsOpen");

        ToolTip.SetTip(host, "tip");

        var window = ShowInWindow(host);
        try
        {
            ToolTip.GetIsOpen(host).ShouldBeTrue();
            var toolTip = host.GetValue(ToolTip.ToolTipProperty);
            toolTip.ShouldNotBeNull();
            toolTip.Classes.Contains(ToolTipPseudoClass.Open).ShouldBeTrue();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void ToolTipOpening_Cancel_Writes_Back_IsOpen_And_Does_Not_Open()
    {
        var host = new Button();
        ToolTip.SetTip(host, "tip");
        ToolTip.AddToolTipOpeningHandler(host, (_, e) => e.Cancel = true);

        var window = ShowInWindow(host);
        try
        {
            ToolTip.SetIsOpen(host, true);

            ToolTip.GetIsOpen(host).ShouldBeFalse();
            var toolTip = host.GetValue(ToolTip.ToolTipProperty);
            if (toolTip is not null)
            {
                toolTip.Classes.Contains(ToolTipPseudoClass.Open).ShouldBeFalse();
            }
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void IsOpen_False_Closes_Opened_ToolTip()
    {
        var host = new Button();
        ToolTip.SetTip(host, "tip");

        var window = ShowInWindow(host);
        try
        {
            ToolTip.SetIsOpen(host, true);
            var toolTip = host.GetValue(ToolTip.ToolTipProperty);
            toolTip.ShouldNotBeNull();
            toolTip.Classes.Contains(ToolTipPseudoClass.Open).ShouldBeTrue();

            // 关闭动效会延迟 Closed 事件；测试关注状态收敛而非动效时序
            toolTip.IsMotionEnabled = false;
            ToolTip.SetIsOpen(host, false);

            toolTip.Classes.Contains(ToolTipPseudoClass.Open).ShouldBeFalse();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Host_Detach_Closes_Popup_But_Keeps_IsOpen_And_Reattach_Reopens()
    {
        var host = new Button();
        ToolTip.SetTip(host, "tip");

        var window = ShowInWindow(host);
        try
        {
            ToolTip.SetIsOpen(host, true);
            var toolTip = host.GetValue(ToolTip.ToolTipProperty);
            toolTip.ShouldNotBeNull();
            toolTip.Classes.Contains(ToolTipPseudoClass.Open).ShouldBeTrue();

            // 关闭动效会延迟 Closed 事件；测试关注状态收敛而非动效时序
            toolTip.IsMotionEnabled = false;
            window.Content = null;
            Dispatcher.UIThread.RunJobs();

            ToolTip.GetIsOpen(host).ShouldBeTrue("detach 只关闭物理弹层，不清除期望状态");
            toolTip.Classes.Contains(ToolTipPseudoClass.Open).ShouldBeFalse();

            window.Content = host;
            Dispatcher.UIThread.RunJobs();

            ToolTip.GetIsOpen(host).ShouldBeTrue();
            toolTip.Classes.Contains(ToolTipPseudoClass.Open).ShouldBeTrue("重新挂入后由调和流程重开");
        }
        finally
        {
            window.Close();
        }
    }

    private static AtomUIWindow ShowInWindow(Control content)
    {
        // headless 平台没有原生 popup 窗口实现，弹层只能走 overlay host
        ToolTip.SetIsUseOverlayHost(content, true);
        var window = new AtomUIWindow
        {
            Width   = 300,
            Height  = 160,
            Content = content
        };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        return window;
    }
}
