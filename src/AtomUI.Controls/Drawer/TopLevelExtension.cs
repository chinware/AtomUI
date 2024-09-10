using Avalonia.Controls;
using Avalonia.Data;

namespace AtomUI.Controls;

public class TopLevelExtension : Binding
{
    public object ProvideValue(IServiceProvider serviceProvider)
    {
        return new Binding()
        {
            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor)
            {
                AncestorType = typeof(TopLevel),
            }
        };
    }
}