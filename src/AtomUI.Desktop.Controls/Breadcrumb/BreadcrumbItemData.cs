using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace AtomUI.Desktop.Controls;

public class BreadcrumbItemData : IBreadcrumbItemData
{
    public PathIcon? Icon { get; set; }
    public object? Content { get; set; }
    public Uri? NavigateUri { get; set; }
    public object? Separator { get; set; }
    public IDataTemplate? SeparatorTemplate { get; set; }
    public object? NavigateContext { get; set; }
}