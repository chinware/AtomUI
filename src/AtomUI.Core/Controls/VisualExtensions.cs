using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public static class VisualExtensions
{
    public static T? FindChildOfType<T>(this Visual? visual) where T : class
    {
        if (visual == null)
        {
            return null;
        }
           
        IEnumerable<Visual> visualChildren = visual.GetVisualChildren();
        foreach (var child in visualChildren)
        {
            if (child is T target)
            {
                return target;
            }
        }
        return null;
    }
    
    public static bool HasChild(this Visual? visual, Visual searchedChild)
    {
        if (visual == null)
        {
            return false;
        }

        IEnumerable<Visual> visualChildren = visual.GetVisualChildren();
        foreach (var child in visualChildren)
        {
            if (child == searchedChild)
            {
                return true;
            }
        }
        return false;
    }

    public static void SubscribeAncestorIsVisible(
        this Visual visual,
        Action<bool> handler,
        CompositeDisposable disposable)
    {
        var current = visual.GetVisualParent();
        while (current is not null)
        {
            if (current is Control control)
            {
                control.GetObservable(Visual.IsVisibleProperty)
                       .Subscribe(handler)
                       .DisposeWith(disposable);
            }

            if (current is TopLevel)
            {
                break;
            }

            current = current.GetVisualParent();
        }
    }
}