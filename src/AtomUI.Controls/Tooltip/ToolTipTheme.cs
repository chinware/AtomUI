using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
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
        return new FuncControlTemplate<ToolTip>((tooltip, scope) =>
        {
            ResetTokenResourceBindings(tooltip);
            var arrowDecoratedBox = new ArrowDecoratedBox
            {
                Name = ToolTipContainerPart,
            };
            CreateTemplateParentBinding(arrowDecoratedBox, ArrowDecoratedBox.ContentProperty, ToolTip.ContentProperty);
            CreateTemplateParentBinding(arrowDecoratedBox, ArrowDecoratedBox.ContentTemplateProperty,
                ToolTip.ContentTemplateProperty);

            arrowDecoratedBox.RegisterInNameScope(scope);
            return arrowDecoratedBox;
        });
    }

    protected override void BuildStyles()
    {
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