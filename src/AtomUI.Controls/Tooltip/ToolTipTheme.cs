using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class ToolTipTheme : BaseControlTheme
{
    public const string ToolTipContainerPart = "PART_ToolTipContainer";

    public ToolTipTheme()
        : base(typeof(ToolTip))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<ToolTip>((tip, scope) =>
        {
            var arrowDecoratedBox = new ArrowDecoratedBox
            {
                Name = ToolTipContainerPart
            };
            if (tip.Content is string text)
            {
                arrowDecoratedBox.Content = new TextBlock
                {
                    Text                = text,
                    VerticalAlignment   = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    TextWrapping        = TextWrapping.Wrap
                };
            }
            else if (tip.Content is Control control)
            {
                arrowDecoratedBox.Content = control;
            }

            BindUtils.RelayBind(tip, ToolTip.IsShowArrowEffectiveProperty, arrowDecoratedBox, ArrowDecoratedBox.IsShowArrowProperty);
            arrowDecoratedBox.RegisterInNameScope(scope);
            return arrowDecoratedBox;
        });
    }

    protected override void BuildStyles()
    {
        this.Add(ToolTip.ShadowsProperty, ToolTipTokenResourceKey.ToolTipShadows);
        this.Add(ToolTip.DefaultMarginToAnchorProperty, ToolTipTokenResourceKey.MarginToAnchor);
        this.Add(ToolTip.MotionDurationProperty, ToolTipTokenResourceKey.ToolTipMotionDuration);
        this.Add(TemplatedControl.BackgroundProperty, GlobalTokenResourceKey.ColorTransparent);

        var arrowDecoratedBoxStyle = new Style(selector => selector.Nesting().Template().OfType<ArrowDecoratedBox>());
        arrowDecoratedBoxStyle.Add(TemplatedControl.FontSizeProperty, GlobalTokenResourceKey.FontSize);
        arrowDecoratedBoxStyle.Add(Layoutable.MaxWidthProperty, ToolTipTokenResourceKey.ToolTipMaxWidth);
        arrowDecoratedBoxStyle.Add(TemplatedControl.BackgroundProperty, ToolTipTokenResourceKey.ToolTipBackground);
        arrowDecoratedBoxStyle.Add(TemplatedControl.ForegroundProperty, ToolTipTokenResourceKey.ToolTipColor);
        arrowDecoratedBoxStyle.Add(Layoutable.MinHeightProperty, GlobalTokenResourceKey.ControlHeight);
        arrowDecoratedBoxStyle.Add(TemplatedControl.PaddingProperty, ToolTipTokenResourceKey.ToolTipPadding);
        arrowDecoratedBoxStyle.Add(TemplatedControl.CornerRadiusProperty, ToolTipTokenResourceKey.BorderRadiusOuter);
        Add(arrowDecoratedBoxStyle);
    }
}