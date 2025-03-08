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

internal abstract class BaseNavMenuTheme : BaseControlTheme
{
    public const string ItemsPresenterPart = "PART_ItemsPresenter";

    public BaseNavMenuTheme(Type targetType) : base(targetType)
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<NavMenu>((navMenu, scope) => { return BuildMenuContent(navMenu, scope); });
    }

    protected abstract Control BuildMenuContent(NavMenu navMenu, INameScope scope);

    protected Control BuildItemPresenter(bool isHorizontal, INameScope scope)
    {
        var itemPresenter = new ItemsPresenter
        {
            Name = ItemsPresenterPart
        };

        itemPresenter.ItemsPanel = new FuncTemplate<Panel?>(() => new StackPanel
        {
            Orientation = isHorizontal ? Orientation.Horizontal : Orientation.Vertical
        });

        KeyboardNavigation.SetTabNavigation(itemPresenter, KeyboardNavigationMode.Continue);

        var itemFrame = new Border
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Child               = itemPresenter
        };

        CreateTemplateParentBinding(itemFrame, Decorator.PaddingProperty, TemplatedControl.PaddingProperty);
        CreateTemplateParentBinding(itemFrame, Border.BackgroundProperty, TemplatedControl.BackgroundProperty);
        CreateTemplateParentBinding(itemFrame, Border.BackgroundSizingProperty,
            TemplatedControl.BackgroundSizingProperty);
        CreateTemplateParentBinding(itemFrame, Border.BorderThicknessProperty,
            TemplatedControl.BorderThicknessProperty);
        CreateTemplateParentBinding(itemFrame, Border.BorderBrushProperty, TemplatedControl.BorderBrushProperty);
        CreateTemplateParentBinding(itemFrame, Border.CornerRadiusProperty, TemplatedControl.CornerRadiusProperty);
        return itemFrame;
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorBorderSecondary);
        Add(commonStyle);
    }
}