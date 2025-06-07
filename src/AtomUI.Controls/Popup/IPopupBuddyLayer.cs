namespace AtomUI.Controls;

internal interface IPopupBuddyLayer
{
    void Attach();
    void Detach();
    Task AttachWithMotion();
    Task DetachWithMotion();
}