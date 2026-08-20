using AtomUI.Controls.Primitives;
using AtomUI.Desktop.Controls.Tests.Window;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;

using AvaloniaVisualLayerManager = Avalonia.Controls.Primitives.VisualLayerManager;

namespace AtomUI.Desktop.Controls.Tests.Dialog;

public class DialogContentPopupLayeringTests
{
    static DialogContentPopupLayeringTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Dialog_Layer_Is_Wrapped_In_A_Popup_Capable_Scope_In_Every_Host_Path()
    {
        RunOnUIThread(() =>
        {
            var (window, _, presenter) = CreateDialogFixture(new AtomUI.Desktop.Controls.ComboBox());
            try
            {
                window.Show();
                Dispatcher.UIThread.RunJobs();
                WaitWithDispatcherPump(presenter.ShowAsync(CancellationToken.None).AsTask());
                Dispatcher.UIThread.RunJobs();

                var dialogLayer = presenter.Parent.ShouldBeOfType<DialogOverlayLayer>();
                var scope = dialogLayer.GetVisualParent().ShouldBeOfType<AvaloniaVisualLayerManager>();
                scope.Child.ShouldBeSameAs(dialogLayer);
                // 弹层作用域尺寸与宿主层同步(headless 走 TopLevel popup layer 宿主路径)
                scope.Bounds.Size.ShouldBe(window.ClientSize);

                WaitWithDispatcherPump(presenter.CloseAsync().AsTask());
                WaitWithDispatcherPump(presenter.DisposeAsync().AsTask());

                presenter.Parent.ShouldBeNull();
                dialogLayer.Parent.ShouldBeNull();
                scope.Parent.ShouldBeNull();
            }
            finally
            {
                window.Close();
            }
        });
    }

    [Fact]
    public void ComboBox_Dropdown_In_Dialog_Content_Is_Hosted_Above_The_Dialog_Layer()
    {
        RunOnUIThread(() =>
        {
            var comboBox = new AtomUI.Desktop.Controls.ComboBox { Width = 160 };
            comboBox.Items.Add(new AtomUI.Desktop.Controls.ComboBoxItem { Content = "1" });
            comboBox.Items.Add(new AtomUI.Desktop.Controls.ComboBoxItem { Content = "2" });
            var (window, _, presenter) = CreateDialogFixture(comboBox);
            try
            {
                window.Show();
                Dispatcher.UIThread.RunJobs();
                WaitWithDispatcherPump(presenter.ShowAsync(CancellationToken.None).AsTask());
                Dispatcher.UIThread.RunJobs();

                comboBox.SetValue(AtomUI.Desktop.Controls.ComboBox.IsDropDownOpenProperty, true);
                Dispatcher.UIThread.RunJobs();
                AvaloniaHeadlessPlatform.ForceRenderTimerTick(1);
                Dispatcher.UIThread.RunJobs();

                comboBox.IsDropDownOpen.ShouldBeTrue();

                var root = ((Visual)window).GetSelfAndVisualAncestors().Last();
                var popupHost = root.GetVisualDescendants()
                                    .OfType<OverlayPopupHost>()
                                    .LastOrDefault(host => host.GetLogicalAncestors()
                                                               .OfType<AtomUI.Desktop.Controls.ComboBox>()
                                                               .Contains(comboBox))
                                    .ShouldNotBeNull();

                var dialogLayer = presenter.Parent.ShouldBeOfType<DialogOverlayLayer>();
                var scope = dialogLayer.GetVisualParent().ShouldBeOfType<AvaloniaVisualLayerManager>();

                // popup host 的最近 VisualLayerManager 祖先必须是 Dialog 弹层作用域,
                // 而不是窗口内容区的 VisualLayerManager
                popupHost.GetVisualAncestors()
                         .OfType<AvaloniaVisualLayerManager>()
                         .First()
                         .ShouldBeSameAs(scope);

                // 作用域内 popup overlay layer 的 z 序高于 Child(DialogOverlayLayer),渲染在 presenter 之上
                var popupLayer = popupHost.GetVisualParent().ShouldBeAssignableTo<Canvas>()!;
                popupLayer.ZIndex.ShouldBeGreaterThan(dialogLayer.ZIndex);
            }
            finally
            {
                WaitWithDispatcherPump(presenter.CloseAsync().AsTask());
                WaitWithDispatcherPump(presenter.DisposeAsync().AsTask());
                window.Close();
            }
        });
    }

    private static (AtomUI.Desktop.Controls.Window Window, Border PlacementTarget, OverlayDialogPresenter Presenter)
        CreateDialogFixture(Control content)
    {
        var placementTarget = new Border { Width = 100, Height = 40 };
        var root = new ScopeAwareOverlayLayerPanel
        {
            Children = { placementTarget }
        };
        var window = new AtomUI.Desktop.Controls.Window
        {
            Width = 640,
            Height = 480,
            Content = root
        };
        var dialog = new AtomUI.Desktop.Controls.Dialog
        {
            Content = content,
            IsModal = true,
            IsMotionEnabled = false,
            HostWidth = 320,
            HostHeight = 220
        };
        var presenter = new OverlayDialogPresenter(dialog, placementTarget);
        return (window, placementTarget, presenter);
    }

    private static void WaitWithDispatcherPump(Task task)
    {
        var timeoutAt = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(5);
        while (!task.IsCompleted && DateTimeOffset.UtcNow < timeoutAt)
        {
            Dispatcher.UIThread.RunJobs();
            Thread.Sleep(1);
        }

        task.IsCompleted.ShouldBeTrue();
        task.GetAwaiter().GetResult();
    }

    private static void RunOnUIThread(Action action)
    {
        Dispatcher.UIThread.Invoke(action);
    }
}
