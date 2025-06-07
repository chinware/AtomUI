namespace AtomUI.Controls;

internal interface IPopupBuddyLayer
{
    void Attach();
    void Detach();
    void AttachWithMotion(Action? aboutToStart = null,
                          Action? completedAction = null,
                          CancellationToken cancellationToken = default);
    void DetachWithMotion(Action? aboutToStart = null,
                          Action? completedAction = null,
                          CancellationToken cancellationToken = default);
}