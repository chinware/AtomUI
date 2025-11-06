namespace AtomUI.Controls;

public interface IDialogAwareDataContext
{
    void NotifyAttachedToDialog(IDialog dialog);
    void NotifyDetachedFromDialog();
}