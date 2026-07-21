using System.Linq;
using AtomUI.Controls.Primitives;
using AtomUI.Desktop.Controls.Tests.Window;
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
    [InlineData(DrawerPlacement.Left, true)]
    [InlineData(DrawerPlacement.Top, true)]
    [InlineData(DrawerPlacement.Right, true)]
    [InlineData(DrawerPlacement.Bottom, true)]
    [InlineData(DrawerPlacement.Left, false)]
    [InlineData(DrawerPlacement.Top, false)]
    [InlineData(DrawerPlacement.Right, false)]
    [InlineData(DrawerPlacement.Bottom, false)]
    public void TopLevel_Drawer_Uses_The_Window_Visible_Frame(
        DrawerPlacement placement,
        bool isCsdEnabled)
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
            window.IsCsdEnabled          = isCsdEnabled;
            Dispatcher.UIThread.RunJobs();

            // The visible frame belongs to the client surface and must remain
            // covered by the Drawer mask. Only the transparent shadow buffer
            // is excluded from the Drawer root bounds.
            window.VisibleFrameBorderThickness = new Thickness(3, 5, 7, 9);

            drawer.IsOpen = true;
            Dispatcher.UIThread.RunJobs();

            var layer = ScopeAwareAdornerLayer.GetLayer(drawer);
            layer.ShouldNotBeNull();
            var container = layer.GetVisualDescendants().OfType<DrawerContainer>().Single();

            container.Margin.ShouldBe(shadow);
            container.CornerRadius.ShouldBe(cornerRadius);

            var mask = container.GetVisualDescendants()
                                .OfType<Border>()
                                .Single(border => border.Name == "PART_Mask");
            var maskOrigin = mask.TranslatePoint(default, window).ShouldNotBeNull();
            var visibleFrameBounds = WindowVisualLayerClip.CalculateClipBounds(
                window.Bounds.Size,
                shadow);
            maskOrigin.X.ShouldBe(visibleFrameBounds.X, 0.001);
            maskOrigin.Y.ShouldBe(visibleFrameBounds.Y, 0.001);
            mask.Bounds.Size.ShouldBe(visibleFrameBounds.Size);

            window.CornerRadius = default;
            Dispatcher.UIThread.RunJobs();
            container.CornerRadius.ShouldBe(default);

            var visibleFrameInsets = new Thickness(
                shadow.Left + window.VisibleFrameBorderThickness.Left,
                shadow.Top + window.VisibleFrameBorderThickness.Top,
                shadow.Right + window.VisibleFrameBorderThickness.Right,
                shadow.Bottom + window.VisibleFrameBorderThickness.Bottom);
            var expectedReferenceSize = placement is DrawerPlacement.Top or DrawerPlacement.Bottom
                ? Math.Max(0, window.Bounds.Height - visibleFrameInsets.Top - visibleFrameInsets.Bottom)
                : Math.Max(0, window.Bounds.Width - visibleFrameInsets.Left - visibleFrameInsets.Right);
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

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Available_Drawn_Decorations_Drawer_Host_Is_Used_Regardless_Of_Csd_State(
        bool isCsdEnabled)
    {
        var drawer = new AtomUI.Desktop.Controls.Drawer
        {
            Content         = new TextBlock { Text = "Body" },
            IsMotionEnabled = false,
            Width           = 1,
            Height          = 1
        };

        var window = CreateAtomWindow(drawer);
        var drawnHost = new ScopeAwareAdornerLayer
        {
            Name = "PART_DrawerOverlayLayerHost",
            LayerHost = window
        };
        Action? restoreDecorations = null;
        try
        {
            window.IsCsdEnabled = isCsdEnabled;
            restoreDecorations = DrawnDecorationsTestHost.Install(window, drawnHost);
            Dispatcher.UIThread.RunJobs();

            drawer.IsOpen = true;
            Dispatcher.UIThread.RunJobs();

            var container = drawnHost.Children.OfType<DrawerContainer>().Single();

            container.GetVisualParent().ShouldBeSameAs(drawnHost);

            drawer.IsOpen = false;
            Dispatcher.UIThread.RunJobs();
            drawnHost.Children.OfType<DrawerContainer>().ShouldBeEmpty();
        }
        finally
        {
            if (drawer.IsOpen)
            {
                drawer.IsOpen = false;
                Dispatcher.UIThread.RunJobs();
            }
            restoreDecorations?.Invoke();
            window.Close();
        }
    }

    [Fact]
    public void Drawer_Inside_An_Existing_Scope_Layer_Reuses_The_Containing_Layer()
    {
        var openOn = new Border();
        var drawer = new AtomUI.Desktop.Controls.Drawer
        {
            OpenOn = openOn
        };
        var layer = new ScopeAwareAdornerLayer
        {
            LayerHost = openOn,
            Children = { drawer }
        };

        try
        {
            ScopeAwareAdornerLayer.GetLayer(drawer).ShouldBeSameAs(layer);
        }
        finally
        {
            layer.Children.Remove(drawer);
        }
    }

    [Fact]
    public void Nested_Drawer_Reuses_The_Parent_Drawer_Layer_And_Releases_In_Order()
    {
        var childDrawer = new AtomUI.Desktop.Controls.Drawer
        {
            Content         = new TextBlock { Text = "Child body" },
            IsMotionEnabled = false,
            Width           = 1,
            Height          = 1
        };
        var parentDrawer = new AtomUI.Desktop.Controls.Drawer
        {
            Content = new StackPanel
            {
                Children =
                {
                    new TextBlock { Text = "Parent body" },
                    childDrawer
                }
            },
            IsMotionEnabled = false,
            Width           = 1,
            Height          = 1
        };
        childDrawer.SetValue(
            AtomUI.Desktop.Controls.Drawer.IsMotionEnabledProperty,
            false,
            BindingPriority.Animation);
        parentDrawer.SetValue(
            AtomUI.Desktop.Controls.Drawer.IsMotionEnabledProperty,
            false,
            BindingPriority.Animation);

        var window = CreateAtomWindow(parentDrawer);
        try
        {
            parentDrawer.IsOpen = true;
            Dispatcher.UIThread.RunJobs();

            var layer = ScopeAwareAdornerLayer.GetLayer(parentDrawer).ShouldNotBeNull();
            AtomUI.Desktop.Controls.Drawer.GetDrawer(childDrawer).ShouldBeSameAs(parentDrawer);
            childDrawer.OpenOn.ShouldBeSameAs(window);
            ScopeAwareAdornerLayer.GetLayer(childDrawer).ShouldBeSameAs(layer);

            childDrawer.IsOpen = true;
            Dispatcher.UIThread.RunJobs();
            layer.Children.OfType<DrawerContainer>().Count().ShouldBe(2);

            childDrawer.IsOpen = false;
            Dispatcher.UIThread.RunJobs();
            layer.Children.OfType<DrawerContainer>().Count().ShouldBe(1);

            parentDrawer.IsOpen = false;
            Dispatcher.UIThread.RunJobs();
            layer.Children.OfType<DrawerContainer>().ShouldBeEmpty();
        }
        finally
        {
            if (childDrawer.IsOpen)
            {
                childDrawer.IsOpen = false;
                Dispatcher.UIThread.RunJobs();
            }
            if (parentDrawer.IsOpen)
            {
                parentDrawer.IsOpen = false;
                Dispatcher.UIThread.RunJobs();
            }
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
