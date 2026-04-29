using AtomUI.Controls.Primitives;

namespace AtomUI.Desktop.Controls;

public interface INavMenu : INavMenuElement
{
    /// <summary>
    /// Gets or sets the currently selected submenu item.
    /// </summary>
    INavMenuNode? SelectedItem { get; }

    /// <summary>
    /// List of paths opened by default
    /// </summary>
    IList<TreeNodePath>? DefaultOpenPaths { get; set; }

    /// <summary>
    /// Default selected path
    /// </summary>
    TreeNodePath? DefaultSelectedPath { get; set; }

    /// <summary>
    /// Close all nodes
    /// </summary>
    void Close();
}