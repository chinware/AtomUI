using System.Diagnostics;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;

namespace AtomUI.Theme;

public interface ITokenResourceConsumer
{
    CompositeDisposable? TokenBindingsDisposable { get; }
}

public static class StyledElementTokenBindingsExtensions
{
    public static void AddTokenBindingDisposable(this StyledElement control, IDisposable tokenBindingsDisposable)
    {
        var consumer = control as ITokenResourceConsumer;
        Debug.Assert(consumer != null, $"{control.GetType()} is not ITokenResourceConsumer");
        Debug.Assert(consumer.TokenBindingsDisposable != null,
            $"The TokenBindingsDisposable of ITokenResourceConsumer {control.GetType()} is null.");
        consumer.TokenBindingsDisposable.Add(tokenBindingsDisposable);
    }

    public static void RunThemeTokenBindingActions(this TemplatedControl control)
    {
        var consumer = control as ITokenResourceConsumer;
        Debug.Assert(consumer != null, $"{control.GetType()} is not ITokenResourceConsumer");
        if (((ILogical)control).IsAttachedToLogicalTree)
        {
            RunThemeTokenBindingActionsCore(control);
        }
        control.AttachedToLogicalTree -= HandleTokenResourceConsumerAttachedToLogicalTree;
        control.AttachedToLogicalTree += HandleTokenResourceConsumerAttachedToLogicalTree;
    }

    internal static void HandleTokenResourceConsumerAttachedToLogicalTree(object? sender, LogicalTreeAttachmentEventArgs args)
    {
        if (sender is TemplatedControl templatedControl)
        {
            RunThemeTokenBindingActionsCore(templatedControl);
        }
    }

    private static void RunThemeTokenBindingActionsCore(TemplatedControl control)
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
        var consumer = control as ITokenResourceConsumer;
        Debug.Assert(consumer != null, $"{control.GetType()} is not ITokenResourceConsumer");
        consumer.TokenBindingsDisposable?.Dispose();
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
        Debug.Assert(element is ITokenResourceConsumer);
        return element.GetValue(ThemeTokenResourceBindingActionsProperty);
    }

    public static void SetBindingActions(TemplatedControl element, List<Action>? actions)
    {
        Debug.Assert(element is ITokenResourceConsumer);
        element.SetValue(ThemeTokenResourceBindingActionsProperty, actions);
    }
    
    public static void AddBindingAction(TemplatedControl element, Action action)
    {
        Debug.Assert(element is ITokenResourceConsumer);
        var actions = element.GetValue(ThemeTokenResourceBindingActionsProperty);
        if (actions == null)
        {
            actions = new List<Action>();
            element.SetValue(ThemeTokenResourceBindingActionsProperty, actions);
        }
        actions.Add(action);
    }
}