using AtomUI.Theme.Styling;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class NodeSwitcherButtonTheme : ToggleIconButtonTheme
{
   public NodeSwitcherButtonTheme()
      : base(typeof(NodeSwitcherButton))
   {}
   
   protected override void BuildStyles()
   {
      base.BuildStyles();

      var commonStyle = new Style(selector => selector.Nesting());
      commonStyle.Add(NodeSwitcherButton.BackgroundProperty, GlobalResourceKey.ColorBgContainer);
      commonStyle.Add(NodeSwitcherButton.CornerRadiusProperty, GlobalResourceKey.BorderRadius);

      var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
      hoverStyle.Add(NodeSwitcherButton.BackgroundProperty, TreeViewResourceKey.NodeHoverBg);
      commonStyle.Add(hoverStyle);
      Add(commonStyle);
   }
}