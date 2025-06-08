namespace AtomUI.Controls;

internal interface IPopupBuddyLayer
{
    void Attach();
    void Detach();
    void AttachWithMotion(Action? aboutToStart = null,
                          Action? completedAction = null);
    void DetachWithMotion(Action? aboutToStart = null,
                          Action? completedAction = null);
}