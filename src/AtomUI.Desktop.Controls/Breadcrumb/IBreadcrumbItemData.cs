using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace AtomUI.Desktop.Controls;

public interface IBreadcrumbItemData
{
    PathIcon? Icon { get; }
    object? Content { get; }
    Uri? NavigateUri { get; }
    object? Separator { get; }
    IDataTemplate? SeparatorTemplate { get; }
    object? NavigateContext { get; }
}