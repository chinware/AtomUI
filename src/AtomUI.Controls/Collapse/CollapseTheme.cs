using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class CollapseTheme : BaseControlTheme
{
    public const string FramePart = "PART_Frame";
    public const string ItemsPresenterPart = "PART_ItemsPresenter";

    public CollapseTheme() : base(typeof(Collapse))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<Collapse>((collapse, scope) =>
        {
            var Frame = new Border
            {
                Name         = FramePart,
                ClipToBounds = true
            };
            var itemsPresenter = new ItemsPresenter
            {
                Name = ItemsPresenterPart
            };
            itemsPresenter.RegisterInNameScope(scope);
            Frame.Child = itemsPresenter;

            CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.ItemsPanelProperty,
                ItemsControl.ItemsPanelProperty);
            CreateTemplateParentBinding(Frame, Border.BorderThicknessProperty,
                Collapse.EffectiveBorderThicknessProperty);
            CreateTemplateParentBinding(Frame, Border.BorderBrushProperty,
                TemplatedControl.BorderBrushProperty);
            CreateTemplateParentBinding(Frame, Border.CornerRadiusProperty,
                TemplatedControl.CornerRadiusProperty);

            return Frame;
        });
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorBorder);
        commonStyle.Add(TemplatedControl.CornerRadiusProperty, CollapseTokenKey.CollapsePanelBorderRadius);

        Add(commonStyle);
    }
}