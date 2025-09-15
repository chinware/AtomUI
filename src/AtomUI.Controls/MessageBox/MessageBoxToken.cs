using AtomUI.Theme.TokenSystem;

namespace AtomUI.Controls;

[ControlDesignToken]
public class MessageBoxToken : AbstractControlDesignToken
{
    public const string ID = "MessageBox";
    
    /// <summary>
    /// Style Icon 的大小
    /// </summary>
    public double StyleIconSize { get; set; }
    
    public MessageBoxToken()
        : base(ID)
    {
    }

    public override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        StyleIconSize = SharedToken.SizeLG * 1.2;
    }
}