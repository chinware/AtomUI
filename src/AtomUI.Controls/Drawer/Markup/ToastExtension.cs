using System.Diagnostics;
using AtomUI.Controls.Utils;
using Avalonia.Markup.Xaml;

namespace AtomUI.Controls;

public class ToastExtension : MarkupExtension
{
    public string? Header { get; set; }
    
    public string? Text { get; set; }
    
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return new InternalCommand(p =>
        {
            // TODO 等待静态 Toast api
            
            Trace.WriteLine("Drawer confirmed!");
        });
    }
}