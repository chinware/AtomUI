using System.ComponentModel;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Data;

namespace AtomUI.Data;

public static class BindUtils
{
    public static IDisposable RelayBind(AvaloniaObject source, 
                                        AvaloniaProperty sourceProperty, 
                                        AvaloniaObject target,
                                        AvaloniaProperty? targetProperty = null,
                                        BindingMode mode = BindingMode.Default,
                                        BindingPriority priority = BindingPriority.LocalValue)
    {
        targetProperty ??= sourceProperty;
        var registry = AvaloniaPropertyRegistry.Instance;
        if (!sourceProperty.IsAttached)
        {
            if (!registry.IsRegistered(source.GetType(), sourceProperty))
            {
                throw new ArgumentException($"Relay source property is not registered for: {source.GetType().Name}.");
            }
        }

        if (!targetProperty.IsAttached)
        {
            if (!registry.IsRegistered(target.GetType(), targetProperty))
            {
                throw new ArgumentException($"Relay target property is not registered for: {target.GetType().Name}.");
            }
        }

        mode = ResolveBindingMode(target, targetProperty, mode);
        return mode switch
        {
            BindingMode.OneWay         => target.Bind(targetProperty, source.GetObservable(sourceProperty), priority),
            BindingMode.TwoWay         => TwoWayRelayBind(source, sourceProperty, target, targetProperty, priority),
            BindingMode.OneWayToSource => OneWayToSourceRelayBind(source, sourceProperty, target, targetProperty),
            BindingMode.OneTime        => OneTimeRelayBind(source, sourceProperty, target, targetProperty, priority),
            _                          => throw new NotSupportedException($"{nameof(RelayBind)} does not support {mode}.")
        };
    }

    private static BindingMode ResolveBindingMode(AvaloniaObject target, AvaloniaProperty targetProperty, BindingMode mode)
    {
        return mode == BindingMode.Default
            ? targetProperty.GetMetadata(target).DefaultBindingMode
            : mode;
    }

    private static IDisposable TwoWayRelayBind(
        AvaloniaObject source,
        AvaloniaProperty sourceProperty,
        AvaloniaObject target,
        AvaloniaProperty targetProperty,
        BindingPriority priority)
    {
        var sourceToTarget = target.Bind(targetProperty, source.GetObservable(sourceProperty), priority);
        var targetToSource = SubscribeChangesAfterInitialValue(
            target,
            targetProperty,
            value => SetBindingSourceValue(source, sourceProperty, value));

        return Disposable.Create(() =>
        {
            targetToSource.Dispose();
            sourceToTarget.Dispose();
        });
    }

    private static IDisposable OneWayToSourceRelayBind(
        AvaloniaObject source,
        AvaloniaProperty sourceProperty,
        AvaloniaObject target,
        AvaloniaProperty targetProperty)
    {
        return SubscribeChangesAfterInitialValue(
            target,
            targetProperty,
            value => SetBindingSourceValue(source, sourceProperty, value));
    }

    private static IDisposable OneTimeRelayBind(
        AvaloniaObject source,
        AvaloniaProperty sourceProperty,
        AvaloniaObject target,
        AvaloniaProperty targetProperty,
        BindingPriority priority)
    {
        var disposable = target.SetValue(targetProperty, source.GetValue(sourceProperty), priority);
        if (disposable is not null)
        {
            return disposable;
        }

        return priority == BindingPriority.LocalValue || targetProperty.IsDirect
            ? Disposable.Create(() => target.ClearValue(targetProperty))
            : Disposable.Empty;
    }
    
    [Obsolete("String-path relay binding is not AOT-safe. Use an AvaloniaProperty overload or the strongly typed getter overload.", true)]
    public static IDisposable RelayBind(object source,
                                        string sourcePropertyName,
                                        AvaloniaObject target,
                                        AvaloniaProperty targetProperty,
                                        BindingMode mode = BindingMode.Default,
                                        BindingPriority priority = BindingPriority.LocalValue)
    {
        throw new NotSupportedException(
            "String-path relay binding is not AOT-safe. Use an AvaloniaProperty overload or the strongly typed getter overload.");
    }

    public static IDisposable RelayBind<TSource, TResult>(TSource source,
                                                          string sourcePropertyName,
                                                          Func<TSource, TResult> valueGetter,
                                                          AvaloniaObject target,
                                                          AvaloniaProperty<TResult> targetProperty,
                                                          BindingPriority priority = BindingPriority.LocalValue)
        where TSource : class
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourcePropertyName);
        ArgumentNullException.ThrowIfNull(valueGetter);

        var registry = AvaloniaPropertyRegistry.Instance;
        if (!targetProperty.IsAttached && !registry.IsRegistered(target.GetType(), targetProperty))
        {
            throw new ArgumentException($"Relay target property is not registered for: {target.GetType().Name}.");
        }

        return target.Bind(
            targetProperty,
            new PropertyChangedValueObservable<TSource, TResult>(source, sourcePropertyName, valueGetter),
            priority);
    }

    public static IDisposable RelayBind<TSource, TResult>(AvaloniaObject source,
                                                          AvaloniaProperty<TSource> sourceProperty,
                                                          AvaloniaObject target,
                                                          AvaloniaProperty<TResult> targetProperty,
                                                          Func<TSource, TResult> converter,
                                                          BindingPriority priority = BindingPriority.LocalValue)
    {
        var registry = AvaloniaPropertyRegistry.Instance;
        if (!registry.IsRegistered(source.GetType(), sourceProperty))
        {
            throw new ArgumentException($"Relay source property is not registered for: {source.GetType().Name}.");
        }

        if (!registry.IsRegistered(target.GetType(), targetProperty))
        {
            throw new ArgumentException($"Relay target property is not registered for: {target.GetType().Name}.");
        }

        return target.Bind(targetProperty, source.GetObservable(sourceProperty, converter), priority);
    }

    public static IDisposable RelayBind<TResult>(AvaloniaObject source,
                                                 AvaloniaProperty sourceProperty,
                                                 AvaloniaObject target,
                                                 AvaloniaProperty<TResult> targetProperty,
                                                 Func<object?, TResult> converter,
                                                 BindingPriority priority = BindingPriority.LocalValue)
    {
        var registry = AvaloniaPropertyRegistry.Instance;
        if (!registry.IsRegistered(source.GetType(), sourceProperty))
        {
            throw new ArgumentException($"Relay source property is not registered for: {source.GetType().Name}.");
        }

        if (!registry.IsRegistered(target.GetType(), targetProperty))
        {
            throw new ArgumentException($"Relay target property is not registered for: {target.GetType().Name}.");
        }

        return target.Bind(targetProperty, source.GetObservable(sourceProperty, converter), priority);
    }

    private static IDisposable SubscribeChangesAfterInitialValue(
        AvaloniaObject source,
        AvaloniaProperty property,
        Action<object?> onChanged)
    {
        var isInitialValue = true;
        return source.GetObservable(property).Subscribe(value =>
        {
            if (isInitialValue)
            {
                isInitialValue = false;
                return;
            }

            onChanged(value);
        });
    }

    private static void SetBindingSourceValue(AvaloniaObject source, AvaloniaProperty sourceProperty, object? value)
    {
        if (sourceProperty.IsReadOnly)
        {
            return;
        }

        source.SetValue(sourceProperty, value, BindingPriority.LocalValue);
    }

    private sealed class PropertyChangedValueObservable<TSource, TResult> : IObservable<object?>
        where TSource : class
    {
        private readonly TSource _source;
        private readonly string _sourcePropertyName;
        private readonly Func<TSource, TResult> _valueGetter;

        public PropertyChangedValueObservable(
            TSource source,
            string sourcePropertyName,
            Func<TSource, TResult> valueGetter)
        {
            _source             = source;
            _sourcePropertyName = sourcePropertyName;
            _valueGetter        = valueGetter;
        }

        public IDisposable Subscribe(IObserver<object?> observer)
        {
            observer.OnNext(_valueGetter(_source));

            if (_source is not INotifyPropertyChanged notifyPropertyChanged)
            {
                return Disposable.Empty;
            }

            void HandlePropertyChanged(object? sender, PropertyChangedEventArgs args)
            {
                if (!string.IsNullOrEmpty(args.PropertyName) && args.PropertyName != _sourcePropertyName)
                {
                    return;
                }

                observer.OnNext(_valueGetter(_source));
            }

            notifyPropertyChanged.PropertyChanged += HandlePropertyChanged;
            return Disposable.Create(() =>
                notifyPropertyChanged.PropertyChanged -= HandlePropertyChanged);
        }
    }
}
