using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Windows.Input;
using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace AtomUIGallery.Controls;

public static class GalleryBindingUtils
{
    public static IDisposable OneWay<TSource, TValue>(
        TSource source,
        string sourcePropertyName,
        Func<TSource, TValue> valueGetter,
        AvaloniaObject target,
        AvaloniaProperty<TValue> targetProperty,
        BindingPriority priority = BindingPriority.LocalValue)
        where TSource : class, INotifyPropertyChanged
    {
        return BindUtils.RelayBind(source, sourcePropertyName, valueGetter, target, targetProperty, priority);
    }

    public static IDisposable OneWay<TSource>(
        TSource source,
        string sourcePropertyName,
        Func<TSource, object?> valueGetter,
        AvaloniaObject target,
        AvaloniaProperty targetProperty,
        BindingPriority priority = BindingPriority.LocalValue)
        where TSource : class, INotifyPropertyChanged
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourcePropertyName);
        ArgumentNullException.ThrowIfNull(valueGetter);
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(targetProperty);

        return target.Bind(
            targetProperty,
            new PropertyChangedValueObservable<TSource>(source, sourcePropertyName, valueGetter),
            priority);
    }

    public static IDisposable BindCommand(AvaloniaObject target, ICommand? command)
    {
        ArgumentNullException.ThrowIfNull(target);

        var previousCommand = target.GetValue(Button.CommandProperty);
        target.SetCurrentValue(Button.CommandProperty, command);
        return Disposable.Create(() => target.SetCurrentValue(Button.CommandProperty, previousCommand));
    }

    private sealed class PropertyChangedValueObservable<TSource> : IObservable<object?>
        where TSource : class, INotifyPropertyChanged
    {
        private readonly TSource _source;
        private readonly string _sourcePropertyName;
        private readonly Func<TSource, object?> _valueGetter;

        public PropertyChangedValueObservable(TSource source, string sourcePropertyName, Func<TSource, object?> valueGetter)
        {
            _source             = source;
            _sourcePropertyName = sourcePropertyName;
            _valueGetter        = valueGetter;
        }

        public IDisposable Subscribe(IObserver<object?> observer)
        {
            observer.OnNext(_valueGetter(_source));

            void HandlePropertyChanged(object? sender, PropertyChangedEventArgs args)
            {
                if (!string.IsNullOrEmpty(args.PropertyName) && args.PropertyName != _sourcePropertyName)
                {
                    return;
                }

                observer.OnNext(_valueGetter(_source));
            }

            _source.PropertyChanged += HandlePropertyChanged;
            return Disposable.Create(() => _source.PropertyChanged -= HandlePropertyChanged);
        }
    }
}
