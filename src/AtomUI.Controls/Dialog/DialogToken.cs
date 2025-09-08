using AtomUI.Theme.TokenSystem;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class DialogToken : AbstractControlDesignToken
{
    public const string ID = "Dialog";
    
    public DialogToken()
        : base(ID)
    {
    }

    public override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
    }
}