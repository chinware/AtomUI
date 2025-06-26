namespace AtomUI.Controls;

/// <summary>
/// Handles user interaction for TreeView.
/// </summary>
public interface ITreeViewInteractionHandler
{
    /// <summary>
    /// Attaches the interaction handler to a menu.
    /// </summary>
    /// <param name="treeView">The treeView.</param>
    void Attach(TreeView treeView);

    /// <summary>
    /// Detaches the interaction handler from the attached treeView.
    /// </summary>
    void Detach(TreeView treeView);
}