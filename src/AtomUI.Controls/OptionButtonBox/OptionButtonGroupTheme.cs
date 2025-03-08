using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class OptionButtonGroupTheme : BaseControlTheme
{
    public const string FramePart = "PART_Frame";
    public const string ItemsPresenterPart = "PART_ItemsPresenter";
    
    public OptionButtonGroupTheme() : base(typeof(OptionButtonGroup))
    {
    }
    
    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<OptionButtonGroup>((group, scope) =>
        {
            var frame = new Border
            {
                Name         = FramePart,
                ClipToBounds = true
            };
            var itemsPresenter = new ItemsPresenter
            {
                Name = ItemsPresenterPart
            };
            itemsPresenter.RegisterInNameScope(scope);
            frame.Child = itemsPresenter;

            CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.ItemsPanelProperty,
                ItemsControl.ItemsPanelProperty);
            CreateTemplateParentBinding(frame, Border.BorderThicknessProperty,
                OptionButtonGroup.EffectiveBorderThicknessProperty);
            CreateTemplateParentBinding(frame, Border.BorderBrushProperty,
                TemplatedControl.BorderBrushProperty);
            CreateTemplateParentBinding(frame, Border.CornerRadiusProperty,
                TemplatedControl.CornerRadiusProperty);

            return frame;
        });
    }
    
    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        
        var largeSizeStyle =
            new Style(selector =>
                selector.Nesting().PropertyEquals(OptionButtonGroup.SizeTypeProperty, SizeType.Large));
        largeSizeStyle.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadiusLG);
        largeSizeStyle.Add(Layoutable.MaxHeightProperty, SharedTokenKey.ControlHeightLG);
        commonStyle.Add(largeSizeStyle);

        var middleSizeStyle =
            new Style(
                selector => selector.Nesting().PropertyEquals(OptionButtonGroup.SizeTypeProperty, SizeType.Middle));
        middleSizeStyle.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadius);
        middleSizeStyle.Add(Layoutable.MaxHeightProperty, SharedTokenKey.ControlHeight);
        commonStyle.Add(middleSizeStyle);

        var smallSizeStyle =
            new Style(selector =>
                selector.Nesting().PropertyEquals(OptionButtonGroup.SizeTypeProperty, SizeType.Small));
        smallSizeStyle.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadiusSM);
        smallSizeStyle.Add(Layoutable.MaxHeightProperty, SharedTokenKey.ControlHeightSM);
        commonStyle.Add(smallSizeStyle);

        commonStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorBorder);
        commonStyle.Add(OptionButtonGroup.SelectedOptionBorderColorProperty, SharedTokenKey.ColorPrimary);
        commonStyle.Add(TemplatedControl.BorderThicknessProperty, SharedTokenKey.BorderThickness);
        
        Add(commonStyle);
    }
}