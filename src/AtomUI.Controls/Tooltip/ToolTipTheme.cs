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
                arrowDecoratedBox.Content = new SingleLineText()
                {
                    Text                = text,
                    VerticalAlignment   = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
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
        this.Add(ToolTip.ShadowsProperty, ToolTipTokenKey.ToolTipShadows);
        this.Add(ToolTip.DefaultMarginToAnchorProperty, ToolTipTokenKey.MarginToAnchor);
        this.Add(ToolTip.MotionDurationProperty, ToolTipTokenKey.ToolTipMotionDuration);
        this.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorTransparent);

        var arrowDecoratedBoxStyle = new Style(selector => selector.Nesting().Template().OfType<ArrowDecoratedBox>());
        arrowDecoratedBoxStyle.Add(TemplatedControl.FontSizeProperty, SharedTokenKey.FontSize);
        arrowDecoratedBoxStyle.Add(Layoutable.MaxWidthProperty, ToolTipTokenKey.ToolTipMaxWidth);
        arrowDecoratedBoxStyle.Add(TemplatedControl.BackgroundProperty, ToolTipTokenKey.ToolTipBackground);
        arrowDecoratedBoxStyle.Add(TemplatedControl.ForegroundProperty, ToolTipTokenKey.ToolTipColor);
        arrowDecoratedBoxStyle.Add(Layoutable.MinHeightProperty, SharedTokenKey.ControlHeight);
        arrowDecoratedBoxStyle.Add(TemplatedControl.PaddingProperty, ToolTipTokenKey.ToolTipPadding);
        arrowDecoratedBoxStyle.Add(TemplatedControl.CornerRadiusProperty, ToolTipTokenKey.BorderRadiusOuter);
        Add(arrowDecoratedBoxStyle);
    }
}