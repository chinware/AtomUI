using AtomUI.IconPkg;
using Avalonia.Controls.Templates;

namespace AtomUI.Desktop.Controls;

public interface IBreadcrumbItemData
{
    Icon? Icon { get; }
    object? Content { get; }
    Uri? NavigateUri { get; }
    object? Separator { get; }
    IDataTemplate? SeparatorTemplate { get; }
    object? NavigateContext { get; }
}