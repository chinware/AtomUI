using Avalonia.Input;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

public interface INavMenuElement : IInputElement, ILogical
{
    /// <summary>
    /// Gets or sets the currently selected submenu item.
    /// </summary>
    INavMenuItem? SelectedItem { get; set; }
    
    /// <summary>
    /// Gets the submenu items.
    /// </summary>
    IEnumerable<INavMenuItem> SubItems { get; }
    
    /// <summary>
    /// Opens the menu or menu item.
    /// </summary>
    void Open();
    
    /// <summary>
    /// Closes the menu or menu item.
    /// </summary>
    void Close();
}