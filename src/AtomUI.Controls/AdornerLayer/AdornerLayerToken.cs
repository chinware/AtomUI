using AtomUI.Theme.TokenSystem;
using Avalonia;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class AdornerLayerToken : AbstractControlDesignToken
{
    public const string ID = "AdornerLayer";
    
    public AdornerLayerToken()
        : base(ID)
    {
    }
    
    public Thickness FocusVisualMargin { get; set; }

    protected internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        FocusVisualMargin = new Thickness(0);
    }
}