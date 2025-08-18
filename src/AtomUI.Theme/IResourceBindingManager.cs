using System.Diagnostics;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;

namespace AtomUI.Theme;

public interface IResourceBindingManager
{
    CompositeDisposable? ResourceBindingsDisposable { get; set; }
}

public static class StyledElementTokenBindingsExtensions
{
    public static void AddResourceBindingDisposable(this StyledElement control, IDisposable tokenBindingsDisposable)
    {
        var consumer = control as IResourceBindingManager;
        Debug.Assert(consumer != null, $"{control.GetType()} is not IResourceBindingManager");
        consumer.ResourceBindingsDisposable ??= new CompositeDisposable();
        consumer.ResourceBindingsDisposable.Add(tokenBindingsDisposable);
    } 

    public static void RunThemeResourceBindingActions(this TemplatedControl control)
    {
        var consumer = control as IResourceBindingManager;
        Debug.Assert(consumer != null, $"{control.GetType()} is not IResourceBindingManager");
        if (((ILogical)control).IsAttachedToLogicalTree)
        {
            RunThemeResourceBindingActionsCore(control);
        }
        control.AttachedToLogicalTree -= HandleResourceConsumerAttachedToLogicalTree;
        control.AttachedToLogicalTree += HandleResourceConsumerAttachedToLogicalTree;
    }

    public static void AddThemeResourceBindingAction(this TemplatedControl control, Action action)
    {
        TokenResourceConsumerProperty.AddBindingAction(control, action);
    }

    internal static void HandleResourceConsumerAttachedToLogicalTree(object? sender, LogicalTreeAttachmentEventArgs args)
    {
        if (sender is TemplatedControl templatedControl)
        {
            RunThemeResourceBindingActionsCore(templatedControl);
        }
    }

    private static void RunThemeResourceBindingActionsCore(TemplatedControl control)
    {
        var actions = TokenResourceConsumerProperty.GetBindingActions(control);
        if (actions != null)
        {
            foreach (var action in actions)
            {
                action();
            }
        }
    }

    public static void DisposeTokenBindings(this StyledElement control)
    {
        var consumer = control as IResourceBindingManager;
        Debug.Assert(consumer != null, $"{control.GetType()} is not IResourceBindingManager");
        consumer.ResourceBindingsDisposable?.Dispose();
        consumer.ResourceBindingsDisposable = null;
    }
}

internal abstract class TokenResourceConsumerProperty : AvaloniaObject
{
    public const string ThemeTokenResourceBindingActionsPropertyName = "ThemeTokenResourceBindingActions";

    public static readonly AttachedProperty<List<Action>?> ThemeTokenResourceBindingActionsProperty =
        AvaloniaProperty.RegisterAttached<TokenResourceConsumerProperty, Control, List<Action>?>(
            ThemeTokenResourceBindingActionsPropertyName);
    
    public static List<Action>? GetBindingActions(TemplatedControl element)
    {
        Debug.Assert(element is IResourceBindingManager);
        return element.GetValue(ThemeTokenResourceBindingActionsProperty);
    }

    public static void SetBindingActions(TemplatedControl element, List<Action>? actions)
    {
        Debug.Assert(element is IResourceBindingManager);
        element.SetValue(ThemeTokenResourceBindingActionsProperty, actions);
    }
    
    public static void AddBindingAction(TemplatedControl element, Action action)
    {
        Debug.Assert(element is IResourceBindingManager);
        var actions = element.GetValue(ThemeTokenResourceBindingActionsProperty);
        if (actions == null)
        {
            actions = new List<Action>();
            element.SetValue(ThemeTokenResourceBindingActionsProperty, actions);
        }
        actions.Add(action);
    }
}