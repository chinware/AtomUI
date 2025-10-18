﻿using AtomUI.Controls.Primitives;
using Avalonia.Rendering;

namespace AtomUI.Controls;

internal interface INavMenu : INavMenuElement
{
    /// <summary>
    /// Gets or sets the currently selected submenu item.
    /// </summary>
    INavMenuItem? SelectedItem { get; }
    
    /// <summary>
    /// Gets the menu interaction handler.
    /// </summary>
    INavMenuInteractionHandler? InteractionHandler { get; }
    
    /// <summary>
    /// Gets a value indicating whether the menu is open.
    /// </summary>
    bool IsOpen { get; }
    
    /// <summary>
    /// Gets the root of the visual tree, if the control is attached to a visual tree.
    /// </summary>
    IRenderRoot? VisualRoot { get; }
    
    /// <summary>
    /// List of paths opened by default
    /// </summary>
    IList<TreeNodePath>? DefaultOpenPaths { get; set; }
    
    /// <summary>
    /// Default selected path
    /// </summary>
    TreeNodePath? DefaultSelectedPath { get; set; }
}