using AtomUI.Theme.TokenSystem;

namespace AtomUI.Controls.MessageBox;

[ControlDesignToken]
public class MessageBoxToken : AbstractControlDesignToken
{
    public const string ID = "MessageBox";
    
    public MessageBoxToken()
        : base(ID)
    {
    }

    public override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
    }
}