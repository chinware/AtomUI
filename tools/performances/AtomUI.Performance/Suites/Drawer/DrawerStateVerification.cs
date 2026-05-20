using System.Diagnostics;
using System.Reflection;
using System.Threading;
using AtomUI;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunDrawerStateVerification()
    {
        var failures = new List<string>();
        VerifyDrawerClosedStateIsLazy(failures);
        VerifyDrawerOpenCloseReuseLifecycle(failures);
        VerifyDrawerDetachReleasesContainer(failures);
        VerifyDrawerPercentageSizeSubscriptionLifecycle(failures);
        VerifyDrawerTemplateSlotState(failures);
        VerifyNestedDrawerPlacementChangeKeepsChildOpen(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Drawer state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Drawer state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyDrawerClosedStateIsLazy(ICollection<string> failures)
    {
        var drawer = CreateVerificationDrawer();
        using var realized = RealizeControl(drawer);

        Expect(GetDrawerContainer(drawer) == null,
            "Closed Drawer should not create DrawerContainer before first open.",
            failures);
        Expect(CountVisualsByTypeName(realized.Window, "DrawerContainer") == 0,
            "Closed Drawer should not keep DrawerContainer in the visual tree.",
            failures);
        Expect(GetPrivateField(drawer, "AtomUI.Desktop.Controls.Drawer", "_openOnSizeChangedTarget") == null,
            "Closed Drawer should not subscribe to OpenOn.SizeChanged.",
            failures);
    }

    private static void VerifyDrawerOpenCloseReuseLifecycle(ICollection<string> failures)
    {
        var drawer = CreateVerificationDrawer();
        using var realized = RealizeControl(drawer);

        drawer.IsOpen = true;
        RefreshLayout(realized.Window);
        var firstContainer = GetDrawerContainer(drawer);
        Expect(firstContainer != null,
            "Opening Drawer should create DrawerContainer.",
            failures);
        Expect(firstContainer?.GetVisualParent() != null,
            "Opened DrawerContainer should be attached to the adorner layer.",
            failures);
        Expect(CountDrawerContainersInParent(firstContainer) == 1,
            "Opening Drawer should attach exactly one DrawerContainer.",
            failures);

        drawer.IsOpen = false;
        WaitForDrawerContainerDetached(realized.Window, firstContainer);
        Expect(firstContainer?.GetVisualParent() == null,
            "Closed DrawerContainer should be removed from the visual tree.",
            failures);
        Expect(GetPrivateField(drawer, "AtomUI.Desktop.Controls.Drawer", "_openOnSizeChangedTarget") == null,
            "Closing Drawer should detach OpenOn.SizeChanged subscription.",
            failures);

        drawer.IsOpen = true;
        RefreshLayout(realized.Window);
        var secondContainer = GetDrawerContainer(drawer);
        Expect(ReferenceEquals(firstContainer, secondContainer),
            "Drawer should reuse the materialized container after close.",
            failures);
        Expect(CountDrawerContainersInParent(secondContainer) == 1,
            "Second open should not duplicate DrawerContainer.",
            failures);
    }

    private static void VerifyDrawerDetachReleasesContainer(ICollection<string> failures)
    {
        var drawer = CreateVerificationDrawer();
        Control? container;
        using (var realized = RealizeControl(drawer))
        {
            drawer.IsOpen = true;
            RefreshLayout(realized.Window);
            container = GetDrawerContainer(drawer);
            Expect(container != null,
                "Opening Drawer before detach should create DrawerContainer.",
                failures);
        }

        Expect(GetDrawerContainer(drawer) == null,
            "Detached Drawer should clear its DrawerContainer field.",
            failures);
        Expect(container?.GetVisualParent() == null,
            "Detached DrawerContainer should not keep a visual parent.",
            failures);
        if (container != null)
        {
            Expect(GetPrivateField(container, "AtomUI.Desktop.Controls.DrawerContainer", "_infoContainer") == null,
                "Released DrawerContainer should clear DrawerInfoContainer reference.",
                failures);
        }
        Expect(GetPrivateField(drawer, "AtomUI.Desktop.Controls.Drawer", "_openOnSizeChangedTarget") == null,
            "Detached Drawer should not retain OpenOn.SizeChanged target.",
            failures);
    }

    private static void VerifyDrawerPercentageSizeSubscriptionLifecycle(ICollection<string> failures)
    {
        var host = new Border
        {
            Width  = 400,
            Height = 300
        };
        var drawer = CreateVerificationDrawer();
        drawer.OpenOn     = host;
        drawer.Placement  = AtomUI.Desktop.Controls.DrawerPlacement.Right;
        drawer.DialogSize = new Dimension(50, DimensionUnitType.Percentage);

        var root = new StackPanel
        {
            Children =
            {
                host,
                drawer
            }
        };

        using var realized = RealizeControl(root);
        drawer.IsOpen = true;
        RefreshLayout(realized.Window);
        Expect(ReferenceEquals(GetPrivateField(drawer, "AtomUI.Desktop.Controls.Drawer", "_openOnSizeChangedTarget"), host),
            "Percentage Drawer should subscribe to OpenOn.SizeChanged only while open.",
            failures);
        Expect(Math.Abs(GetDrawerEffectiveDialogSize(drawer) - 200) < 0.5,
            $"Percentage Drawer should resolve initial width from OpenOn bounds, actual {GetDrawerEffectiveDialogSize(drawer):0.###}.",
            failures);

        host.Width = 600;
        RefreshLayout(realized.Window);
        Expect(Math.Abs(GetDrawerEffectiveDialogSize(drawer) - 300) < 0.5,
            $"Percentage Drawer should update size after OpenOn resize, actual {GetDrawerEffectiveDialogSize(drawer):0.###}.",
            failures);

        drawer.IsOpen = false;
        RefreshLayout(realized.Window);
        Expect(GetPrivateField(drawer, "AtomUI.Desktop.Controls.Drawer", "_openOnSizeChangedTarget") == null,
            "Closed percentage Drawer should detach OpenOn.SizeChanged subscription.",
            failures);
    }

    private static void VerifyDrawerTemplateSlotState(ICollection<string> failures)
    {
        var noMaskDrawer = CreateVerificationDrawer();
        noMaskDrawer.IsShowMask = false;
        using (var realized = RealizeControl(noMaskDrawer))
        {
            noMaskDrawer.IsOpen = true;
            RefreshLayout(realized.Window);
            var container = GetDrawerContainer(noMaskDrawer);
            var mask = container == null ? null : FindVisualByName<Border>(container, "PART_Mask");
            Expect(mask != null && !mask.IsVisible,
                "Drawer with IsShowMask=false should keep mask hidden.",
                failures);
        }

        var noCloseDrawer = CreateVerificationDrawer();
        noCloseDrawer.IsShowCloseButton = false;
        using (var realized = RealizeControl(noCloseDrawer))
        {
            noCloseDrawer.IsOpen = true;
            RefreshLayout(realized.Window);
            var container = GetDrawerContainer(noCloseDrawer);
            var infoContainer = GetDrawerInfoContainer(container);
            Expect(ReadInternalBoolProperty(infoContainer, "IsShowCloseButton") == false,
                "Drawer with IsShowCloseButton=false should sync the hidden close button state to DrawerInfoContainer.",
                failures);

            noCloseDrawer.IsShowCloseButton = true;
            RefreshLayout(realized.Window);
            Expect(ReadInternalBoolProperty(infoContainer, "IsShowCloseButton") == true,
                "Drawer should sync visible close button state when IsShowCloseButton becomes true.",
                failures);
        }

        var slotDrawer = CreateVerificationDrawer();
        using (var realized = RealizeControl(slotDrawer))
        {
            slotDrawer.IsOpen = true;
            RefreshLayout(realized.Window);
            var container = GetDrawerContainer(slotDrawer);
            var infoContainer = GetDrawerInfoContainer(container);
            Expect(ReadInternalBoolProperty(infoContainer, "HasExtra") == false,
                "Drawer without Extra should sync HasExtra=false.",
                failures);
            Expect(ReadInternalBoolProperty(infoContainer, "HasFooter") == false,
                "Drawer without Footer should sync HasFooter=false.",
                failures);

            slotDrawer.Extra  = new AtomUI.Desktop.Controls.Button { Content = "Extra" };
            slotDrawer.Footer = new AtomUI.Desktop.Controls.Button { Content = "Footer" };
            RefreshLayout(realized.Window);
            Expect(ReadInternalBoolProperty(infoContainer, "HasExtra") == true,
                "Drawer should sync HasExtra=true when Extra is assigned.",
                failures);
            Expect(ReadInternalBoolProperty(infoContainer, "HasFooter") == true,
                "Drawer should sync HasFooter=true when Footer is assigned.",
                failures);

            slotDrawer.Extra  = null;
            slotDrawer.Footer = null;
            RefreshLayout(realized.Window);
            Expect(ReadInternalBoolProperty(infoContainer, "HasExtra") == false,
                "Removed Extra should sync HasExtra=false.",
                failures);
            Expect(ReadInternalBoolProperty(infoContainer, "HasFooter") == false,
                "Removed Footer should sync HasFooter=false.",
                failures);
        }
    }

    private static void VerifyNestedDrawerPlacementChangeKeepsChildOpen(ICollection<string> failures)
    {
        var childDrawer = CreateVerificationDrawer();
        childDrawer.Title     = "Two-level Drawer";
        childDrawer.Placement = AtomUI.Desktop.Controls.DrawerPlacement.Right;

        var parentDrawer = CreateVerificationDrawer();
        parentDrawer.Title     = "First-level Drawer";
        parentDrawer.Placement = AtomUI.Desktop.Controls.DrawerPlacement.Right;
        parentDrawer.Content   = new StackPanel
        {
            Children =
            {
                new AtomUI.Desktop.Controls.TextBlock { Text = "Some contents..." },
                new AtomUI.Desktop.Controls.Button { Content = "Two-level drawer" },
                childDrawer
            }
        };

        using var realized = RealizeControlInVisualLayerManager(parentDrawer);
        parentDrawer.IsOpen = true;
        RefreshLayout(realized.Window);
        RefreshLayout(realized.Window);

        var parentContainer = GetDrawerContainer(parentDrawer);
        var parentInfoContainer = GetDrawerInfoContainer(parentContainer) as Control;
        Expect(parentContainer?.GetVisualParent() != null,
            "Nested parent DrawerContainer should be attached before placement switch.",
            failures);
        Expect(childDrawer.IsAttachedToVisualTree(),
            "Nested child Drawer should be attached through the parent Drawer content.",
            failures);
        childDrawer.IsOpen = true;
        RefreshLayout(realized.Window);
        var childContainer = GetDrawerContainer(childDrawer);
        Expect(childContainer?.GetVisualParent() != null,
            "Nested child DrawerContainer should open through the real visual tree.",
            failures);
        Expect(GetRenderOffsetX(parentInfoContainer) < 0,
            "Right-placement parent Drawer should be pushed left when child Drawer is open.",
            failures);

        var parentContentPanel = parentDrawer.Content as StackPanel;
        if (parentContentPanel != null)
        {
            var childIndex = parentContentPanel.Children.IndexOf(childDrawer);
            if (childIndex >= 0)
            {
                parentContentPanel.Children.RemoveAt(childIndex);
                parentContentPanel.Children.Insert(childIndex, childDrawer);
                RefreshLayout(realized.Window);
                Expect(ReferenceEquals(childContainer, GetDrawerContainer(childDrawer)),
                    "Transient child Drawer detach/reattach should keep the open DrawerContainer instance.",
                    failures);
                Expect(childContainer?.GetVisualParent() != null,
                    "Transient child Drawer detach/reattach should keep the child DrawerContainer attached.",
                    failures);
                Expect(GetRenderOffsetX(parentInfoContainer) < 0,
                    "Transient child Drawer detach/reattach should keep the parent Drawer pushed.",
                    failures);
            }
        }

        parentDrawer.Placement = AtomUI.Desktop.Controls.DrawerPlacement.Left;
        childDrawer.Placement  = AtomUI.Desktop.Controls.DrawerPlacement.Left;
        RefreshLayout(realized.Window);
        Expect(parentContainer?.GetVisualParent() != null,
            "Nested parent DrawerContainer should remain attached after Right->Left switch.",
            failures);
        Expect(childDrawer.IsOpen,
            "Nested child Drawer should keep IsOpen=true after Right->Left switch.",
            failures);
        Expect(childContainer?.GetVisualParent() != null,
            "Nested child DrawerContainer should remain attached after Right->Left switch.",
            failures);
        Expect(GetRenderOffsetX(parentInfoContainer) > 0,
            "Left-placement parent Drawer should be pushed right when child Drawer remains open.",
            failures);

        parentDrawer.Placement = AtomUI.Desktop.Controls.DrawerPlacement.Right;
        childDrawer.Placement  = AtomUI.Desktop.Controls.DrawerPlacement.Right;
        RefreshLayout(realized.Window);
        Expect(childDrawer.IsOpen,
            "Nested child Drawer should keep IsOpen=true after Left->Right switch.",
            failures);
        Expect(childContainer?.GetVisualParent() != null,
            "Nested child DrawerContainer should remain attached after Left->Right switch.",
            failures);
        Expect(GetRenderOffsetX(parentInfoContainer) < 0,
            "Right-placement parent Drawer should restore left push after switching back.",
            failures);

        childDrawer.IsOpen = false;
        RefreshLayout(realized.Window);
        Expect(Math.Abs(GetRenderOffsetX(parentInfoContainer)) < 0.001,
            "Parent Drawer push transform should be restored after nested child closes.",
            failures);
    }

    private static Drawer CreateVerificationDrawer()
    {
        var drawer = new Drawer
        {
            Title           = "Verification Drawer",
            DialogSize      = new Dimension(50, DimensionUnitType.Percentage),
            Content         = new StackPanel
            {
                Children =
                {
                    new AtomUI.Desktop.Controls.TextBlock { Text = "Some contents..." }
                }
            }
        };
        drawer.SetValue(Drawer.IsMotionEnabledProperty, false, BindingPriority.Animation);
        return drawer;
    }

    private static Control? GetDrawerContainer(Drawer drawer)
    {
        return GetPrivateField(drawer, "AtomUI.Desktop.Controls.Drawer", "_container") as Control;
    }

    private static int CountDrawerContainersInParent(Control? drawerContainer)
    {
        return drawerContainer?.GetVisualParent() is Panel parent
            ? parent.Children.OfType<Control>().Count(control => control.GetType().Name == "DrawerContainer")
            : 0;
    }

    private static double GetDrawerEffectiveDialogSize(Drawer drawer)
    {
        return GetPrivateField(drawer, "AtomUI.Desktop.Controls.Drawer", "_effectiveDialogSize") is double value
            ? value
            : 0;
    }

    private static object? GetDrawerInfoContainer(Control? container)
    {
        return container == null
            ? null
            : GetPrivateField(container, "AtomUI.Desktop.Controls.DrawerContainer", "_infoContainer");
    }

    private static double GetRenderOffsetX(Control? control)
    {
        return control?.RenderTransform?.Value.M31 ?? 0;
    }

    private static bool? ReadInternalBoolProperty(object? target, string propertyName)
    {
        if (target == null)
        {
            return null;
        }

        var value = target.GetType()
                          .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                          ?.GetValue(target);
        return value is bool boolValue ? boolValue : null;
    }

    private static void WaitForDrawerContainerDetached(Avalonia.Controls.Window window, Control? container)
    {
        if (container == null)
        {
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        while (container.GetVisualParent() != null && stopwatch.Elapsed < TimeSpan.FromSeconds(2))
        {
            Thread.Sleep(20);
            RefreshLayout(window);
        }
    }

    private static RealizedScenario RealizeControlInVisualLayerManager(Control control)
    {
        var manager = new VisualLayerManager
        {
            Child = control
        };
        var window = new Avalonia.Controls.Window
        {
            Width         = MeasureSize.Width,
            Height        = 900,
            Content       = manager,
            ShowInTaskbar = false
        };

        window.Show();
        Dispatcher.UIThread.RunJobs();
        window.Measure(MeasureSize);
        window.Arrange(ArrangeRect);
        window.UpdateLayout();
        Dispatcher.UIThread.RunJobs();

        return new RealizedScenario(window, [control]);
    }
}
