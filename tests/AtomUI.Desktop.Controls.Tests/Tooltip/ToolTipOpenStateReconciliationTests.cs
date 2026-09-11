using System.Reflection;
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
    public void Popup_Pin_Opens_ToolTip_State_And_Physical_Popup()
    {
        var host = new Button();
        ToolTip.SetTip(host, "tip");

        var window = ShowInWindow(host);
        try
        {
            ToolTip.SetIsPopupPinnedOpen(host, true);
            Dispatcher.UIThread.RunJobs();

            ToolTip.GetIsOpen(host).ShouldBeTrue();
            var toolTip = host.GetValue(ToolTip.ToolTipProperty);
            toolTip.ShouldNotBeNull();
            var popup = GetPopup(toolTip);
            popup.IsPopupPinnedOpen.ShouldBeTrue();
            popup.IsOpen.ShouldBeTrue();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Popup_Pin_Rejects_IsOpen_False_Request()
    {
        var host = new Button();
        ToolTip.SetTip(host, "tip");

        var window = ShowInWindow(host);
        try
        {
            ToolTip.SetIsPopupPinnedOpen(host, true);
            Dispatcher.UIThread.RunJobs();
            var toolTip = host.GetValue(ToolTip.ToolTipProperty);
            toolTip.ShouldNotBeNull();
            var popup = GetPopup(toolTip);

            ToolTip.SetIsOpen(host, false);
            Dispatcher.UIThread.RunJobs();

            ToolTip.GetIsOpen(host).ShouldBeTrue();
            toolTip.Classes.Contains(ToolTipPseudoClass.Open).ShouldBeTrue();
            popup.IsOpen.ShouldBeTrue();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Pinned_ToolTip_Close_Request_Does_Not_Publish_A_Transient_Closed_State()
    {
        var host = new Button();
        ToolTip.SetTip(host, "tip");

        var window = ShowInWindow(host);
        try
        {
            ToolTip.SetIsPopupPinnedOpen(host, true);
            Dispatcher.UIThread.RunJobs();
            var isOpenChangeCount = 0;
            host.PropertyChanged += (_, change) =>
            {
                if (change.Property == ToolTip.IsOpenProperty)
                {
                    ++isOpenChangeCount;
                }
            };

            ToolTip.SetIsOpen(host, false);
            Dispatcher.UIThread.RunJobs();

            ToolTip.GetIsOpen(host).ShouldBeTrue();
            isOpenChangeCount.ShouldBe(0);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Unpinning_A_Pending_ToolTip_Request_Clears_IsOpen()
    {
        var host = new Button();
        ToolTip.SetTip(host, "tip");

        ToolTip.SetIsPopupPinnedOpen(host, true);
        ToolTip.GetIsOpen(host).ShouldBeTrue();

        ToolTip.SetIsPopupPinnedOpen(host, false);

        ToolTip.GetIsOpen(host).ShouldBeFalse();
    }

    [Fact]
    public void Pinned_ToolTip_Closes_When_Tip_Is_Removed_And_Reopens_When_Restored()
    {
        var host = new Button();
        ToolTip.SetTip(host, "tip");

        var window = ShowInWindow(host);
        try
        {
            ToolTip.SetIsPopupPinnedOpen(host, true);
            Dispatcher.UIThread.RunJobs();
            var originalToolTip = host.GetValue(ToolTip.ToolTipProperty);
            originalToolTip.ShouldNotBeNull();
            var originalPopup = GetPopup(originalToolTip);
            originalPopup.IsOpen.ShouldBeTrue();

            ToolTip.SetTip(host, null);
            Dispatcher.UIThread.RunJobs();

            ToolTip.GetIsPopupPinnedOpen(host).ShouldBeTrue();
            ToolTip.GetIsOpen(host).ShouldBeTrue();
            originalToolTip.Classes.Contains(ToolTipPseudoClass.Open).ShouldBeFalse();
            originalPopup.IsOpen.ShouldBeFalse();

            ToolTip.SetTip(host, "replacement");
            Dispatcher.UIThread.RunJobs();

            var replacementToolTip = host.GetValue(ToolTip.ToolTipProperty);
            replacementToolTip.ShouldNotBeNull();
            replacementToolTip.Content.ShouldBe("replacement");
            replacementToolTip.Classes.Contains(ToolTipPseudoClass.Open).ShouldBeTrue();
            GetPopup(replacementToolTip).IsOpen.ShouldBeTrue();
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

    [Fact]
    public void Pinned_Host_Detach_Closes_Popup_And_Reattach_Reopens()
    {
        var host = new Button();
        ToolTip.SetTip(host, "tip");

        var window = ShowInWindow(host);
        try
        {
            ToolTip.SetIsPopupPinnedOpen(host, true);
            Dispatcher.UIThread.RunJobs();
            var toolTip = host.GetValue(ToolTip.ToolTipProperty);
            toolTip.ShouldNotBeNull();
            toolTip.IsMotionEnabled = false;
            var popup = GetPopup(toolTip);
            popup.IsOpen.ShouldBeTrue();

            window.Content = null;
            Dispatcher.UIThread.RunJobs();

            ToolTip.GetIsPopupPinnedOpen(host).ShouldBeTrue();
            ToolTip.GetIsOpen(host).ShouldBeTrue();
            toolTip.Classes.Contains(ToolTipPseudoClass.Open).ShouldBeFalse();
            popup.IsOpen.ShouldBeFalse();

            window.Content = host;
            Dispatcher.UIThread.RunJobs();

            ToolTip.GetIsOpen(host).ShouldBeTrue();
            toolTip.Classes.Contains(ToolTipPseudoClass.Open).ShouldBeTrue();
            popup.IsOpen.ShouldBeTrue();
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

    private static Popup GetPopup(ToolTip toolTip)
    {
        var field = typeof(ToolTip).GetField("_popup", BindingFlags.Instance | BindingFlags.NonPublic);
        field.ShouldNotBeNull();
        return field.GetValue(toolTip).ShouldBeOfType<Popup>();
    }
}
