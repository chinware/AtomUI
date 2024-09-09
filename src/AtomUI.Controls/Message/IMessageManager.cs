namespace AtomUI.Controls;

public interface IMessageManager
{
   public void Show(IMessage message, string[]? classes = null);
}