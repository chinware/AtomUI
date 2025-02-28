using System.Diagnostics;
using System.Reactive.Disposables;
using Avalonia;

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

    public static void DisposeTokenBindings(this StyledElement control)
    {
        var consumer = control as ITokenResourceConsumer;
        Debug.Assert(consumer != null, $"{control.GetType()} is not ITokenResourceConsumer");
        consumer.TokenBindingsDisposable?.Dispose();
    }
}