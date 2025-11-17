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
        } 
        if (mode == BindingMode.OneWay)
        {
            if (source is IObservable<object?> observable)
            {
                return InstancedBinding.OneWay(observable, priority);
            }
            throw new ArgumentException("source type must be IObservable<object?>.");
        }
        if (mode == BindingMode.OneWayToSource)
        {
            if (source is IObserver<object?> observer)
            {
                return InstancedBinding.OneWayToSource(observer, priority);
            } 
            throw new ArgumentException("source type must be IObserver<object?>.");
        }
        if (mode == BindingMode.TwoWay)
        {
            if (source is IObserver<object?> observer && source is IObservable<object?> observable)
            {
                return InstancedBinding.TwoWay(observable, observer, priority);
            } 
            throw new ArgumentException("source type must be IObserver<object?> and IObservable<object?>.");
        }
        throw new ArgumentException("mode must be OneTime, OneWay or OneWayToSource.");
    }
    
}