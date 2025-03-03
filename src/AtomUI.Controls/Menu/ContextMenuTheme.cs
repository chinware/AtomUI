using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
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
        return new FuncControlTemplate<ContextMenu>((contextMenu, scope) =>
        {
            var wrapper = new Border
            {
                Name = RootContainerPart
            };
            
            RegisterTokenResourceBindings(contextMenu, () =>
            {
                contextMenu.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(wrapper, Border.BackgroundProperty,
                    MenuTokenKey.MenuBgColor));
                contextMenu.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(wrapper, Layoutable.MinWidthProperty,
                    MenuTokenKey.MenuPopupMinWidth));
                contextMenu.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(wrapper, Layoutable.MaxWidthProperty,
                    MenuTokenKey.MenuPopupMaxWidth));
                contextMenu.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(wrapper, Layoutable.MinHeightProperty,
                    MenuTokenKey.MenuPopupMinHeight));
                contextMenu.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(wrapper, Layoutable.MaxHeightProperty,
                    MenuTokenKey.MenuPopupMaxHeight));
                contextMenu.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(wrapper, Decorator.PaddingProperty,
                    MenuTokenKey.MenuPopupContentPadding));
                contextMenu.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(wrapper, Border.CornerRadiusProperty,
                    MenuTokenKey.MenuPopupBorderRadius));
            });

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
    }
}