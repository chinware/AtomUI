using AtomUI.Theme.Styling;
using Avalonia.Controls.Primitives;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class NodeSwitcherButtonTheme : ToggleIconButtonTheme
{
    public NodeSwitcherButtonTheme()
        : base(typeof(NodeSwitcherButton))
    {
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();

        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(TemplatedControl.BackgroundProperty, GlobalTokenResourceKey.ColorTransparent);
        commonStyle.Add(TemplatedControl.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadius);

        var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
        hoverStyle.Add(TemplatedControl.BackgroundProperty, TreeViewTokenResourceKey.NodeHoverBg);
        commonStyle.Add(hoverStyle);
        Add(commonStyle);
    }
}