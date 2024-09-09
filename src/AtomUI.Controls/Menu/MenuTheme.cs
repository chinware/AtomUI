using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class MenuTheme : BaseControlTheme
{
    public const string ItemsPresenterPart = "PART_ItemsPresenter";

    public MenuTheme()
        : base(typeof(Menu))
    {
    }

    protected override IControlTemplate? BuildControlTemplate()
    {
        return new FuncControlTemplate<Menu>((menu, scope) =>
        {
            var itemPresenter = new ItemsPresenter
            {
                Name                = ItemsPresenterPart,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment   = VerticalAlignment.Center,
                ItemsPanel = new FuncTemplate<Panel?>(() => new StackPanel
                {
                    Orientation = Orientation.Horizontal
                })
            };

            KeyboardNavigation.SetTabNavigation(itemPresenter, KeyboardNavigationMode.Continue);

            var border = new Border
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Child               = itemPresenter
            };
            CreateTemplateParentBinding(border, Decorator.PaddingProperty, TemplatedControl.PaddingProperty);
            CreateTemplateParentBinding(border, Border.BackgroundProperty, TemplatedControl.BackgroundProperty);
            CreateTemplateParentBinding(border, Border.BackgroundSizingProperty,
                TemplatedControl.BackgroundSizingProperty);
            CreateTemplateParentBinding(border, Border.BorderThicknessProperty,
                TemplatedControl.BorderThicknessProperty);
            CreateTemplateParentBinding(border, Border.BorderBrushProperty, TemplatedControl.BorderBrushProperty);
            CreateTemplateParentBinding(border, Border.CornerRadiusProperty, TemplatedControl.CornerRadiusProperty);
            return border;
        });
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Left);
        commonStyle.Add(TemplatedControl.BackgroundProperty, MenuTokenResourceKey.MenuBgColor);
        commonStyle.Add(TemplatedControl.BorderBrushProperty, GlobalTokenResourceKey.ColorBorder);
        var largeSizeType =
            new Style(selector => selector.Nesting().PropertyEquals(Menu.SizeTypeProperty, SizeType.Large));
        largeSizeType.Add(Layoutable.MinHeightProperty, GlobalTokenResourceKey.ControlHeightLG);
        largeSizeType.Add(TemplatedControl.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadius);
        commonStyle.Add(largeSizeType);

        var middleSizeType =
            new Style(selector => selector.Nesting().PropertyEquals(Menu.SizeTypeProperty, SizeType.Middle));
        middleSizeType.Add(Layoutable.MinHeightProperty, GlobalTokenResourceKey.ControlHeight);
        middleSizeType.Add(TemplatedControl.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadius);
        commonStyle.Add(middleSizeType);

        var smallSizeType =
            new Style(selector => selector.Nesting().PropertyEquals(Menu.SizeTypeProperty, SizeType.Small));
        smallSizeType.Add(Layoutable.MinHeightProperty, GlobalTokenResourceKey.ControlHeightSM);
        smallSizeType.Add(TemplatedControl.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusSM);
        commonStyle.Add(smallSizeType);
        Add(commonStyle);
    }
}