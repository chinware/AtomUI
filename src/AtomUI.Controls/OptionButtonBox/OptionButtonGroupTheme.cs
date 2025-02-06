using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class OptionButtonGroupTheme : BaseControlTheme
{
    public const string MainContainerPart = "PART_MainContainer";

    public OptionButtonGroupTheme()
        : base(typeof(OptionButtonGroup))
    {
    }

    protected override IControlTemplate? BuildControlTemplate()
    {
        return new FuncControlTemplate<OptionButtonGroup>((group, scope) =>
        {
            var layout = new StackPanel
            {
                Name         = MainContainerPart,
                Orientation  = Orientation.Horizontal,
                ClipToBounds = true
            };
            layout.RegisterInNameScope(scope);
            return layout;
        });
    }

    protected override void BuildStyles()
    {
        var largeSizeStyle =
            new Style(selector =>
                selector.Nesting().PropertyEquals(OptionButtonGroup.SizeTypeProperty, SizeType.Large));
        largeSizeStyle.Add(TemplatedControl.CornerRadiusProperty, DesignTokenKey.BorderRadiusLG);
        largeSizeStyle.Add(Layoutable.MaxHeightProperty, DesignTokenKey.ControlHeightLG);
        Add(largeSizeStyle);

        var middleSizeStyle =
            new Style(
                selector => selector.Nesting().PropertyEquals(OptionButtonGroup.SizeTypeProperty, SizeType.Middle));
        middleSizeStyle.Add(TemplatedControl.CornerRadiusProperty, DesignTokenKey.BorderRadius);
        middleSizeStyle.Add(Layoutable.MaxHeightProperty, DesignTokenKey.ControlHeight);
        Add(middleSizeStyle);

        var smallSizeStyle =
            new Style(selector =>
                selector.Nesting().PropertyEquals(OptionButtonGroup.SizeTypeProperty, SizeType.Small));
        smallSizeStyle.Add(TemplatedControl.CornerRadiusProperty, DesignTokenKey.BorderRadiusSM);
        smallSizeStyle.Add(Layoutable.MaxHeightProperty, DesignTokenKey.ControlHeightSM);
        Add(smallSizeStyle);

        this.Add(TemplatedControl.BorderBrushProperty, DesignTokenKey.ColorBorder);
        this.Add(OptionButtonGroup.SelectedOptionBorderColorProperty, DesignTokenKey.ColorPrimary);
        this.Add(TemplatedControl.BorderThicknessProperty, DesignTokenKey.BorderThickness);
    }
}