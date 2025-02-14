using Avalonia.Styling;

namespace AtomUI.Theme.Styling;

public class SetterValueFactory<T> : ITemplate
    where T : class
{
    private Func<T?> _valueFactory;
    
    public SetterValueFactory(Func<T?> factory)
    {
        _valueFactory = factory;
    }
    
    public object? Build()
    {
        return _valueFactory();
    }
}