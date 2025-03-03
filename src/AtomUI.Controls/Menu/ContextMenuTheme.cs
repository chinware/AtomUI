using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class ContextMenuTheme : BaseControlTheme
{
    public const string ItemsPresenterPart = "PART_ItemsPresenter";
    public const string RootContainerPart = "PART_RootContainer";

    public ContextMenuTheme()
        : base(typeof(ContextMenu))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<ContextMenu>((contextMenu, scope) =>
        {
            var wrapper = new Border
            {
                Name = RootContainerPart
            };

            var scrollViewer = new MenuScrollViewer();
            var itemsPresenter = new ItemsPresenter
            {
                Name = ItemsPresenterPart
            };
            CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.ItemsPanelProperty,
                ItemsControl.ItemsPanelProperty);
            KeyboardNavigation.SetTabNavigation(itemsPresenter, KeyboardNavigationMode.Continue);
            Grid.SetIsSharedSizeScope(itemsPresenter, true);
            scrollViewer.Content = itemsPresenter;
            wrapper.Child        = scrollViewer;
            return wrapper;
        });
    }

    protected override void BuildStyles()
    {
        this.Add(TemplatedControl.BackgroundProperty, Brushes.Transparent);
        this.Add(TemplatedControl.CornerRadiusProperty, MenuTokenKey.MenuPopupBorderRadius);

        var rootContainerStyle = new Style(selector => selector.Nesting().Template().Name(RootContainerPart));
        rootContainerStyle.Add(Border.BackgroundProperty, MenuTokenKey.MenuBgColor);
        rootContainerStyle.Add(Layoutable.MinWidthProperty, MenuTokenKey.MenuPopupMinWidth);
        rootContainerStyle.Add(Layoutable.MaxWidthProperty, MenuTokenKey.MenuPopupMaxWidth);
        rootContainerStyle.Add(Layoutable.MinHeightProperty, MenuTokenKey.MenuPopupMinHeight);
        rootContainerStyle.Add(Layoutable.MaxHeightProperty, MenuTokenKey.MenuPopupMaxHeight);
        rootContainerStyle.Add(Decorator.PaddingProperty, MenuTokenKey.MenuPopupContentPadding);
        rootContainerStyle.Add(Border.CornerRadiusProperty, MenuTokenKey.MenuPopupBorderRadius);
        Add(rootContainerStyle);
    }
}