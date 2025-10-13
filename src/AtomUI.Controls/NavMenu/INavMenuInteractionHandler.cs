namespace AtomUI.Controls;

public interface INavMenuInteractionHandler
{
    /// <summary>
    /// Attaches the interaction handler to a menu.
    /// </summary>
    /// <param name="menu">The menu.</param>
    void Attach(NavMenuBase menu);

    /// <summary>
    /// Detaches the interaction handler from the attached menu.
    /// </summary>
    void Detach(NavMenuBase menu);
    
    /// <summary>
    /// Select the specified NavMenuItem
    /// </summary>
    /// <param name="navMenuItem"></param>
    void Select(NavMenuItem navMenuItem);
}