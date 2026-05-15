using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunBadgeStateVerification()
    {
        var failures = new List<string>();
        VerifyHiddenCountBadgeDoesNotCreateAdorner(failures);
        VerifyZeroVisibilityCanToggleAtRuntime(failures);
        VerifyDotBadgeTargetNoTextSkipsLabel(failures);
        VerifyBadgeVisibilityLifecycle(failures);
        VerifyRibbonBadgeVisibilityLifecycle(failures);
        VerifyBadgeDecoratedTargetSwitch(failures);
        VerifyBadgeStandaloneTargetModeSwitches(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Badge state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Badge state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyHiddenCountBadgeDoesNotCreateAdorner(ICollection<string> failures)
    {
        var targetBadge = new CountBadge
        {
            Count           = 0,
            DecoratedTarget = CreateBadgeTarget()
        };
        using (RealizeControl(targetBadge))
        {
            Expect(GetPrivateField(targetBadge, "AtomUI.Controls.Commons.AbstractCountBadge", "_badgeAdorner") == null,
                "Hidden target CountBadge should not create _badgeAdorner.",
                failures);
            Expect(CountVisualByTypeName(targetBadge, "CountBadgeAdorner") == 0,
                "Hidden target CountBadge should not create CountBadgeAdorner visuals.",
                failures);
        }

        var standaloneBadge = new CountBadge
        {
            Count = 0
        };
        using (RealizeControl(standaloneBadge))
        {
            Expect(GetPrivateField(standaloneBadge, "AtomUI.Controls.Commons.AbstractCountBadge", "_badgeAdorner") == null,
                "Hidden standalone CountBadge should not create _badgeAdorner.",
                failures);
            Expect(CountVisualByTypeName(standaloneBadge, "CountBadgeAdorner") == 0,
                "Hidden standalone CountBadge should not create CountBadgeAdorner visuals.",
                failures);
        }
    }

    private static void VerifyDotBadgeTargetNoTextSkipsLabel(ICollection<string> failures)
    {
        var dotBadge = new DotBadge
        {
            DecoratedTarget = CreateBadgeTarget()
        };
        using (RealizeControl(dotBadge))
        {
            Expect(CountVisualByTypeName(dotBadge, "DotBadgeAdorner") == 0,
                "Target DotBadge adorner should live in AdornerLayer, not under the DotBadge visual subtree.",
                failures);
            var adorner = GetPrivateField(dotBadge, "AtomUI.Controls.Commons.AbstractDotBadge", "_dotBadgeAdorner") as Control;
            if (adorner != null)
            {
                Expect(CountVisualByTypeName(adorner, "Label") == 0,
                    "Target DotBadge without text should not create Label visuals.",
                    failures);
            }
        }
    }

    private static void VerifyZeroVisibilityCanToggleAtRuntime(ICollection<string> failures)
    {
        var countBadge = new CountBadge
        {
            Count           = 0,
            IsMotionEnabled = false
        };

        using var realized = RealizeControl(countBadge);
        Expect(GetPrivateField(countBadge, "AtomUI.Controls.Commons.AbstractCountBadge", "_badgeAdorner") == null,
            "Zero CountBadge should start without _badgeAdorner when IsZeroVisible is false.",
            failures);

        countBadge.SetCurrentValue(CountBadge.IsZeroVisibleProperty, true);
        RefreshLayout(realized.Window);
        Expect(GetPrivateField(countBadge, "AtomUI.Controls.Commons.AbstractCountBadge", "_badgeAdorner") != null,
            "Zero CountBadge should create _badgeAdorner after IsZeroVisible becomes true.",
            failures);
        Expect(countBadge.BadgeIsVisible,
            "Zero CountBadge should restore BadgeIsVisible after IsZeroVisible becomes true.",
            failures);

        countBadge.SetCurrentValue(CountBadge.IsZeroVisibleProperty, false);
        RefreshLayout(realized.Window);
        Expect(GetPrivateField(countBadge, "AtomUI.Controls.Commons.AbstractCountBadge", "_adornerLayer") == null,
            "Zero CountBadge should clear _adornerLayer after IsZeroVisible becomes false.",
            failures);
        Expect(!countBadge.BadgeIsVisible,
            "Zero CountBadge should clear BadgeIsVisible after IsZeroVisible becomes false.",
            failures);
    }

    private static void VerifyBadgeVisibilityLifecycle(ICollection<string> failures)
    {
        var countBadge = new CountBadge
        {
            Count           = 5,
            IsMotionEnabled = false
        };

        using (var realized = RealizeControl(countBadge))
        {
            Expect(GetPrivateField(countBadge, "AtomUI.Controls.Commons.AbstractCountBadge", "_badgeAdorner") != null,
                "Visible CountBadge should create _badgeAdorner.",
                failures);

            countBadge.SetCurrentValue(CountBadge.BadgeIsVisibleProperty, false);
            RefreshLayout(realized.Window);
            Expect(GetPrivateField(countBadge, "AtomUI.Controls.Commons.AbstractCountBadge", "_adornerLayer") == null,
                "Hidden CountBadge should clear _adornerLayer.",
                failures);

            countBadge.SetCurrentValue(CountBadge.BadgeIsVisibleProperty, true);
            RefreshLayout(realized.Window);
            Expect(GetPrivateField(countBadge, "AtomUI.Controls.Commons.AbstractCountBadge", "_badgeAdorner") != null,
                "Re-shown CountBadge should recreate or reuse _badgeAdorner.",
                failures);
        }

        Expect(GetPrivateField(countBadge, "AtomUI.Controls.Commons.AbstractCountBadge", "_adornerLayer") == null,
            "Detached CountBadge should clear _adornerLayer.",
            failures);
    }

    private static void VerifyBadgeDecoratedTargetSwitch(ICollection<string> failures)
    {
        var firstTarget = CreateBadgeTarget();
        var secondTarget = CreateBadgeTarget();
        var countBadge = new CountBadge
        {
            Count           = 5,
            IsMotionEnabled = false,
            DecoratedTarget = firstTarget
        };

        using var realized = RealizeControl(countBadge);
        countBadge.SetCurrentValue(CountBadge.DecoratedTargetProperty, secondTarget);
        RefreshLayout(realized.Window);

        Expect(firstTarget.GetVisualParent() == null,
            "Old CountBadge decorated target should be detached after DecoratedTarget changes.",
            failures);
        Expect(secondTarget.GetVisualParent() == countBadge,
            "New CountBadge decorated target should be attached to CountBadge.",
            failures);
    }

    private static void VerifyRibbonBadgeVisibilityLifecycle(ICollection<string> failures)
    {
        var ribbonBadge = new RibbonBadge
        {
            Text = "Ribbon"
        };

        using var realized = RealizeControl(ribbonBadge);
        Expect(CountVisualByTypeName(ribbonBadge, "RibbonBadgeAdorner") == 1,
            "Visible standalone RibbonBadge should create RibbonBadgeAdorner.",
            failures);

        ribbonBadge.SetCurrentValue(RibbonBadge.BadgeIsVisibleProperty, false);
        RefreshLayout(realized.Window);
        Expect(CountVisualByTypeName(ribbonBadge, "RibbonBadgeAdorner") == 0,
            "Hidden standalone RibbonBadge should detach RibbonBadgeAdorner.",
            failures);
        Expect(!ribbonBadge.IsVisible,
            "Hidden standalone RibbonBadge should clear IsVisible.",
            failures);

        ribbonBadge.SetCurrentValue(RibbonBadge.BadgeIsVisibleProperty, true);
        RefreshLayout(realized.Window);
        Expect(CountVisualByTypeName(ribbonBadge, "RibbonBadgeAdorner") == 1,
            "Re-shown standalone RibbonBadge should reattach RibbonBadgeAdorner.",
            failures);
        Expect(ribbonBadge.IsVisible,
            "Re-shown standalone RibbonBadge should restore IsVisible.",
            failures);
    }

    private static void VerifyBadgeStandaloneTargetModeSwitches(ICollection<string> failures)
    {
        VerifyCountBadgeStandaloneTargetModeSwitch(failures);
        VerifyDotBadgeStandaloneTargetModeSwitch(failures);
        VerifyRibbonBadgeStandaloneTargetModeSwitch(failures);
    }

    private static void VerifyCountBadgeStandaloneTargetModeSwitch(ICollection<string> failures)
    {
        var countBadge = new CountBadge
        {
            Count           = 5,
            IsMotionEnabled = false
        };

        using var realized = RealizeControl(countBadge);
        Expect(CountVisualByTypeName(countBadge, "CountBadgeAdorner") == 1,
            "Standalone CountBadge should attach CountBadgeAdorner under itself.",
            failures);

        var target = CreateBadgeTarget();
        countBadge.SetCurrentValue(CountBadge.DecoratedTargetProperty, target);
        RefreshLayout(realized.Window);
        Expect(CountVisualByTypeName(countBadge, "CountBadgeAdorner") == 0,
            "Target CountBadge should detach standalone CountBadgeAdorner before using AdornerLayer.",
            failures);
        Expect(target.GetVisualParent() == countBadge,
            "Target CountBadge should attach DecoratedTarget under itself.",
            failures);

        countBadge.SetCurrentValue(CountBadge.DecoratedTargetProperty, null);
        RefreshLayout(realized.Window);
        Expect(target.GetVisualParent() == null,
            "CountBadge target should be detached after switching back to standalone mode.",
            failures);
        Expect(CountVisualByTypeName(countBadge, "CountBadgeAdorner") == 1,
            "CountBadge should reattach CountBadgeAdorner after switching back to standalone mode.",
            failures);
    }

    private static void VerifyDotBadgeStandaloneTargetModeSwitch(ICollection<string> failures)
    {
        var dotBadge = new DotBadge
        {
            IsMotionEnabled = false,
            Text            = "Active"
        };

        using var realized = RealizeControl(dotBadge);
        Expect(CountVisualByTypeName(dotBadge, "DotBadgeAdorner") == 1,
            "Standalone DotBadge should attach DotBadgeAdorner under itself.",
            failures);
        Expect(CountVisualByTypeName(dotBadge, "Label") == 1,
            "Standalone DotBadge with text should create one Label.",
            failures);

        var target = CreateBadgeTarget();
        dotBadge.SetCurrentValue(DotBadge.DecoratedTargetProperty, target);
        RefreshLayout(realized.Window);
        Expect(CountVisualByTypeName(dotBadge, "DotBadgeAdorner") == 0,
            "Target DotBadge should detach standalone DotBadgeAdorner before using AdornerLayer.",
            failures);
        var adorner = GetPrivateField(dotBadge, "AtomUI.Controls.Commons.AbstractDotBadge", "_dotBadgeAdorner") as Control;
        if (adorner is not null)
        {
            Expect(CountVisualByTypeName(adorner, "Label") == 0,
                "Target DotBadge should detach standalone text Label.",
                failures);
        }

        dotBadge.SetCurrentValue(DotBadge.DecoratedTargetProperty, null);
        RefreshLayout(realized.Window);
        Expect(target.GetVisualParent() == null,
            "DotBadge target should be detached after switching back to standalone mode.",
            failures);
        Expect(CountVisualByTypeName(dotBadge, "DotBadgeAdorner") == 1,
            "DotBadge should reattach DotBadgeAdorner after switching back to standalone mode.",
            failures);
        Expect(CountVisualByTypeName(dotBadge, "Label") == 1,
            "Standalone DotBadge text Label should be restored after switching back.",
            failures);
    }

    private static void VerifyRibbonBadgeStandaloneTargetModeSwitch(ICollection<string> failures)
    {
        var ribbonBadge = new RibbonBadge
        {
            Text = "Ribbon"
        };

        using var realized = RealizeControl(ribbonBadge);
        Expect(CountVisualByTypeName(ribbonBadge, "RibbonBadgeAdorner") == 1,
            "Standalone RibbonBadge should attach RibbonBadgeAdorner under itself.",
            failures);

        var target = CreateRibbonTarget();
        ribbonBadge.SetCurrentValue(RibbonBadge.DecoratedTargetProperty, target);
        RefreshLayout(realized.Window);
        Expect(CountVisualByTypeName(ribbonBadge, "RibbonBadgeAdorner") == 0,
            "Target RibbonBadge should detach standalone RibbonBadgeAdorner before using AdornerLayer.",
            failures);
        Expect(target.GetVisualParent() == ribbonBadge,
            "Target RibbonBadge should attach DecoratedTarget under itself.",
            failures);

        ribbonBadge.SetCurrentValue(RibbonBadge.DecoratedTargetProperty, null);
        RefreshLayout(realized.Window);
        Expect(target.GetVisualParent() == null,
            "RibbonBadge target should be detached after switching back to standalone mode.",
            failures);
        Expect(CountVisualByTypeName(ribbonBadge, "RibbonBadgeAdorner") == 1,
            "RibbonBadge should reattach RibbonBadgeAdorner after switching back to standalone mode.",
            failures);
    }
}
