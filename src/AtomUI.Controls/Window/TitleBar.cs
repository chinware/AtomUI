namespace AtomUI.Controls;

using AvaloniaTitleBar = Avalonia.Controls.Chrome.TitleBar;

public class TitleBar : AvaloniaTitleBar
{
    protected override Type StyleKeyOverride { get; } = typeof(AvaloniaTitleBar);
}