namespace AtomUI.Desktop.Controls;

public interface IDialogAwareDataContext
{
    void NotifyAttachedToDialog(IDialog dialog);
    public void NotifyDetachedFromDialog() {}
    public void NotifyClosed() {}
}