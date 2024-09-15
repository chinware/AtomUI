using AtomUI.Controls.Utils;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AtomUI.Controls;

public class CloseDrawerExtension : MarkupExtension
{
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        if (provideValueTarget?.TargetObject is not Control control)
        {
            return InvalidCommand.Default;
        }
        
        return new CloseDrawerCommand(control);
    }
}