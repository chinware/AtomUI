using Avalonia.Controls.Platform;
using Avalonia.Input;
using Avalonia.Rendering;

namespace AtomUI.Controls.NavMenu;

internal interface INavMenu : INavMenuElement, IInputElement
{
    /// <summary>
    /// Gets the menu interaction handler.
    /// </summary>
    INavMenuInteractionHandler InteractionHandler { get; }
    
    /// <summary>
    /// Gets a value indicating whether the menu is open.
    /// </summary>
    bool IsOpen { get; }
    
    /// <summary>
    /// Gets the root of the visual tree, if the control is attached to a visual tree.
    /// </summary>
    IRenderRoot? VisualRoot { get; }
}