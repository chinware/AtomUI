namespace AtomUI.Controls.Message;

public interface IMessageManager
{
   public void Show(IMessage message, string[]? classes = null);
}