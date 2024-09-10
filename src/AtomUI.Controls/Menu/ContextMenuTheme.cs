using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

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
        return new FuncControlTemplate<ContextMenu>((menu, scope) =>
        {
            var wrapper = new Border
            {
                Name = RootContainerPart
            };
            TokenResourceBinder.CreateTokenBinding(wrapper, Border.BackgroundProperty,
                MenuTokenResourceKey.MenuBgColor);
            TokenResourceBinder.CreateTokenBinding(wrapper, Layoutable.MinWidthProperty,
                MenuTokenResourceKey.MenuPopupMinWidth);
            TokenResourceBinder.CreateTokenBinding(wrapper, Layoutable.MaxWidthProperty,
                MenuTokenResourceKey.MenuPopupMaxWidth);
            TokenResourceBinder.CreateTokenBinding(wrapper, Layoutable.MinHeightProperty,
                MenuTokenResourceKey.MenuPopupMinHeight);
            TokenResourceBinder.CreateTokenBinding(wrapper, Layoutable.MaxHeightProperty,
                MenuTokenResourceKey.MenuPopupMaxHeight);
            TokenResourceBinder.CreateTokenBinding(wrapper, Decorator.PaddingProperty,
                MenuTokenResourceKey.MenuPopupContentPadding);
            TokenResourceBinder.CreateTokenBinding(wrapper, Border.CornerRadiusProperty,
                MenuTokenResourceKey.MenuPopupBorderRadius);

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
        this.Add(TemplatedControl.BackgroundProperty, new SolidColorBrush(Colors.Transparent));
        this.Add(TemplatedControl.CornerRadiusProperty, MenuTokenResourceKey.MenuPopupBorderRadius);
    }
}