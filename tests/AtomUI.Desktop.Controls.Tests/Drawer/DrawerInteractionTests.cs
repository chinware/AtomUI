using System.Linq;
using AtomUI.Controls.Primitives;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Drawer;

public class DrawerInteractionTests
{
    static DrawerInteractionTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Drawer_Right_Button_Release_Inside_InfoContainer_Does_Not_Close()
    {
        var drawer = new AtomUI.Desktop.Controls.Drawer
        {
            Content              = new TextBlock { Text = "Body" },
            IsCloseOnMaskClick   = true,
            IsMotionEnabled      = false,
            Width                = 1,
            Height               = 1
        };

        var window = CreateWindow(drawer);
        try
        {
            drawer.IsOpen = true;
            Dispatcher.UIThread.RunJobs();

            var layer = ScopeAwareAdornerLayer.GetLayer(drawer);
            layer.ShouldNotBeNull();

            var infoContainer = layer.GetVisualDescendants()
                                     .OfType<DrawerInfoContainer>()
                                     .Single();

            RaisePointerReleased(infoContainer, MouseButton.Right, PointerUpdateKind.RightButtonReleased);
            Dispatcher.UIThread.RunJobs();

            drawer.IsOpen.ShouldBeTrue("right-clicking inside Drawer content must not be treated as a mask click.");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Drawer_Left_Button_Release_On_Mask_Closes_When_CloseOnMaskClick()
    {
        var drawer = new AtomUI.Desktop.Controls.Drawer
        {
            Content            = new TextBlock { Text = "Body" },
            IsCloseOnMaskClick = true,
            IsMotionEnabled    = false,
            Width              = 1,
            Height             = 1
        };

        var window = CreateWindow(drawer);
        try
        {
            drawer.IsOpen = true;
            Dispatcher.UIThread.RunJobs();

            var layer = ScopeAwareAdornerLayer.GetLayer(drawer);
            layer.ShouldNotBeNull();

            var mask = layer.GetVisualDescendants()
                            .OfType<Border>()
                            .Single(border => border.Name == "PART_Mask");

            RaisePointerReleased(mask, MouseButton.Left, PointerUpdateKind.LeftButtonReleased);
            Dispatcher.UIThread.RunJobs();

            drawer.IsOpen.ShouldBeFalse("left-clicking the mask should keep closing the Drawer.");
        }
        finally
        {
            window.Close();
        }
    }

    private static void RaisePointerReleased(Control source, MouseButton button, PointerUpdateKind updateKind)
    {
        source.RaiseEvent(new PointerReleasedEventArgs(
            source,
            new Pointer(Pointer.GetNextFreeId(), PointerType.Mouse, true),
            source,
            default,
            0,
            new PointerPointProperties(RawInputModifiers.None, updateKind),
            KeyModifiers.None,
            button));
    }

    private static AvaloniaWindow CreateWindow(Control content)
    {
        var window = new AvaloniaWindow
        {
            Width   = 480,
            Height  = 360,
            Content = content
        };

        window.Show();
        Dispatcher.UIThread.RunJobs();
        return window;
    }
}
