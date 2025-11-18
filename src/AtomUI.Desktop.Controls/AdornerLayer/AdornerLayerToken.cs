using AtomUI.Theme.TokenSystem;
using Avalonia;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class AdornerLayerToken : AbstractControlDesignToken
{
    public const string ID = "AdornerLayer";
    
    public AdornerLayerToken()
        : base(ID)
    {
    }
    
    public Thickness FocusVisualMargin { get; set; }

    public override void CalculateTokenValues()
    {
        base.CalculateTokenValues();
        FocusVisualMargin = new Thickness(0);
    }
}