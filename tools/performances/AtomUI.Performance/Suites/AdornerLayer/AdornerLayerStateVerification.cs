using AtomUI;
using AtomUI.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunAdornerLayerStateVerification()
    {
        var failures = new List<string>();
        VerifyScopeAwareAdornerClearsTargetOnExplicitDetach(failures);
        VerifyScopeAwareAdornerClearsTargetOnVisualDetach(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("AdornerLayer state verification passed.");
            return true;
        }

        Console.Error.WriteLine("AdornerLayer state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }

        return false;
    }

    private static void VerifyScopeAwareAdornerClearsTargetOnExplicitDetach(ICollection<string> failures)
    {
        var target = new Border
        {
            Width      = 80,
            Height     = 24,
            Background = Brushes.Transparent
        };
        var adorner = new Border
        {
            Width      = 16,
            Height     = 16,
            Background = Brushes.Transparent
        };
        var host = CreateScopeAwareAdornerHost(target);

        using var realized = RealizeControl(host);
        ScopeAwareAdornerLayer.SetAdorner(target, adorner);
        RefreshLayout(realized.Window);

        var layer = ScopeAwareAdornerLayer.GetLayer(target);
        Expect(layer != null,
            "ScopeAwareAdornerLayer should be available for realized target.",
            failures);
        Expect(ReferenceEquals(adorner.GetVisualParent(), layer),
            "Adorner should attach to the scope-aware layer.",
            failures);
        Expect(ReferenceEquals(ScopeAwareAdornerLayer.GetAdornedElement(adorner), target),
            "Attached adorner should point at its adorned target.",
            failures);

        ScopeAwareAdornerLayer.SetAdorner(target, null);
        RefreshLayout(realized.Window);

        Expect(adorner.GetVisualParent() == null,
            "Detached adorner should be removed from the layer.",
            failures);
        Expect(ScopeAwareAdornerLayer.GetAdornedElement(adorner) == null,
            "Detached adorner should clear its adorned target reference.",
            failures);

        ScopeAwareAdornerLayer.SetAdorner(target, adorner);
        RefreshLayout(realized.Window);
        Expect(ReferenceEquals(adorner.GetVisualParent(), layer),
            "Reattached adorner should attach back to the same layer.",
            failures);
        Expect(ReferenceEquals(ScopeAwareAdornerLayer.GetAdornedElement(adorner), target),
            "Reattached adorner should restore its adorned target reference.",
            failures);
    }

    private static void VerifyScopeAwareAdornerClearsTargetOnVisualDetach(ICollection<string> failures)
    {
        var target = new Border
        {
            Width      = 80,
            Height     = 24,
            Background = Brushes.Transparent
        };
        var adorner = new Border
        {
            Width      = 16,
            Height     = 16,
            Background = Brushes.Transparent
        };
        var content = new Panel();
        content.Children.Add(target);
        var host = new VisualLayerManager
        {
            Child = content
        };

        using var realized = RealizeControl(host);
        ScopeAwareAdornerLayer.SetAdorner(target, adorner);
        RefreshLayout(realized.Window);
        Expect(adorner.GetVisualParent() is ScopeAwareAdornerLayer,
            "Adorner should attach before target detaches.",
            failures);

        content.Children.Remove(target);
        RefreshLayout(realized.Window);
        Expect(adorner.GetVisualParent() == null,
            "Adorner should be removed when target detaches from visual tree.",
            failures);
        Expect(ScopeAwareAdornerLayer.GetAdornedElement(adorner) == null,
            "Adorner should clear target reference when target detaches from visual tree.",
            failures);

        content.Children.Add(target);
        RefreshLayout(realized.Window);
        Expect(adorner.GetVisualParent() is ScopeAwareAdornerLayer,
            "Adorner should reattach when target returns to visual tree.",
            failures);
        Expect(ReferenceEquals(ScopeAwareAdornerLayer.GetAdornedElement(adorner), target),
            "Reattached adorner should point at target again.",
            failures);
    }

    private static VisualLayerManager CreateScopeAwareAdornerHost(Control target)
    {
        return new VisualLayerManager
        {
            Child = new Panel
            {
                Children =
                {
                    target
                }
            }
        };
    }
}
