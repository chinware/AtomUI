using System.Linq;
using AtomUI.Controls.Primitives;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Media.Transformation;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AtomUI.MotionScene;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;
using AtomUIWindow = AtomUI.Desktop.Controls.Window;

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

    [Fact]
    public void Reopening_Without_Motion_Clears_Completed_Close_Transform()
    {
        var drawer = new AtomUI.Desktop.Controls.Drawer
        {
            Content = new TextBlock { Text = "Body" },
            Width   = 1,
            Height  = 1
        };
        drawer.SetValue(AtomUI.Desktop.Controls.Drawer.IsMotionEnabledProperty, false, BindingPriority.Animation);

        var window = CreateWindow(drawer);
        try
        {
            drawer.IsOpen = true;
            Dispatcher.UIThread.RunJobs();

            var layer = ScopeAwareAdornerLayer.GetLayer(drawer);
            layer.ShouldNotBeNull();
            var container = layer.GetVisualDescendants().OfType<DrawerContainer>().Single();
            var motionActor = container.GetVisualDescendants().OfType<BaseMotionActor>().Single();

            drawer.IsOpen = false;
            Dispatcher.UIThread.RunJobs();
            drawer.IsAttachedToVisualTree().ShouldBeTrue();
            container.GetVisualParent().ShouldBeNull();

            var transformBuilder = new TransformOperations.Builder(1);
            transformBuilder.AppendTranslate(240, 0);
            motionActor.MotionTransformOperations = transformBuilder.Build();

            drawer.IsOpen = true;
            Dispatcher.UIThread.RunJobs();

            container.GetVisualParent().ShouldNotBeNull();
            var reopenedMotionActor = container.GetVisualDescendants().OfType<BaseMotionActor>().Single();
            reopenedMotionActor.ShouldBeSameAs(motionActor);
            reopenedMotionActor.MotionTransformOperations.ShouldBeNull();
            reopenedMotionActor.RenderTransform.ShouldNotBeNull();
            reopenedMotionActor.RenderTransform.Value.M31.ShouldBe(0, 0.001);
            reopenedMotionActor.RenderTransform.Value.M32.ShouldBe(0, 0.001);
        }
        finally
        {
            window.Close();
        }
    }

    [Theory]
    [InlineData(DrawerPlacement.Left)]
    [InlineData(DrawerPlacement.Top)]
    [InlineData(DrawerPlacement.Right)]
    [InlineData(DrawerPlacement.Bottom)]
    public void TopLevel_Drawer_Uses_Csd_Visible_Frame(
        DrawerPlacement placement)
    {
        var drawer = new AtomUI.Desktop.Controls.Drawer
        {
            Content         = new TextBlock { Text = "Body" },
            DialogSize      = new Dimension(50, DimensionUnitType.Percentage),
            IsMotionEnabled = false,
            Placement       = placement,
            Width           = 1,
            Height          = 1
        };

        var window = CreateAtomWindow(drawer);
        try
        {
            var shadow = new Thickness(12, 18, 24, 30);
            var cornerRadius = new CornerRadius(9, 11, 13, 15);
            window.FrameShadowThickness = shadow;
            window.CornerRadius         = cornerRadius;
            window.IsCsdEnabled          = true;
            Dispatcher.UIThread.RunJobs();

            drawer.IsOpen = true;
            Dispatcher.UIThread.RunJobs();

            var layer = ScopeAwareAdornerLayer.GetLayer(drawer);
            layer.ShouldNotBeNull();
            var container = layer.GetVisualDescendants().OfType<DrawerContainer>().Single();

            container.Margin.ShouldBe(shadow);
            container.CornerRadius.ShouldBe(cornerRadius);

            window.CornerRadius = default;
            Dispatcher.UIThread.RunJobs();
            container.CornerRadius.ShouldBe(default);

            var expectedReferenceSize = placement is DrawerPlacement.Top or DrawerPlacement.Bottom
                ? Math.Max(0, window.Bounds.Height - shadow.Top - shadow.Bottom)
                : Math.Max(0, window.Bounds.Width - shadow.Left - shadow.Right);
            drawer.EffectiveDialogSize.ShouldBe(expectedReferenceSize * 0.5, 0.001);

            container.IsMotionEnabled = false;
            drawer.IsOpen = false;
            Dispatcher.UIThread.RunJobs();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void X11_Style_Drawer_Does_Not_Change_Its_Host_Margin()
    {
        var drawer = new AtomUI.Desktop.Controls.Drawer
        {
            Content         = new TextBlock { Text = "Body" },
            IsMotionEnabled = false,
            Width           = 1,
            Height          = 1
        };

        var window = CreateAtomWindow(drawer);
        try
        {
            window.FrameShadowThickness = new Thickness(20);
            window.IsCsdEnabled          = false;
            Dispatcher.UIThread.RunJobs();

            drawer.IsOpen = true;
            Dispatcher.UIThread.RunJobs();

            var layer = ScopeAwareAdornerLayer.GetLayer(drawer);
            layer.ShouldNotBeNull();
            var container = layer.GetVisualDescendants().OfType<DrawerContainer>().Single();

            container.Margin.ShouldBe(default);
            container.CornerRadius.ShouldBe(default);
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

    private static AtomUIWindow CreateAtomWindow(Control content)
    {
        var window = new AtomUIWindow
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
