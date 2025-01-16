using System.Reactive.Linq;
using Avalonia.Data;

namespace AtomUI.Data;

internal static class InstancedBindingFactory
{
    public static InstancedBinding CreateInstancedBinding(object source,
                                                          BindingMode mode,
                                                          BindingPriority priority = BindingPriority.LocalValue)
    {
        _ = source ?? throw new ArgumentNullException(nameof(source));
        if (mode == BindingMode.OneTime)
        {
            if (source is IObservable<object?> observable)
            {
                return InstancedBinding.OneTime(observable, priority);
            }
            return InstancedBinding.OneTime(Observable.Return(source), priority);
        } else if (mode == BindingMode.OneWay)
        {
            if (source is IObservable<object?> observable)
            {
                return InstancedBinding.OneWay(observable, priority);
            }
            else
            {
                throw new ArgumentException("source type must be IObservable<object?>.");
            }
        }
        else if (mode == BindingMode.OneWayToSource)
        {
            if (source is IObserver<object?> observer)
            {
                return InstancedBinding.OneWayToSource(observer, priority);
            } 
            else
            {
                throw new ArgumentException("source type must be IObserver<object?>.");
            }
        }
        else if (mode == BindingMode.TwoWay)
        {
            if (source is IObserver<object?> observer)
            {
                return InstancedBinding.OneWayToSource(observer, priority);
            } 
            else
            {
                throw new ArgumentException("source type must be IObserver<object?>.");
            }
        }
        else
        {
            throw new ArgumentException("mode must be OneTime, OneWay or OneWayToSource.");
        }
    }
    
}