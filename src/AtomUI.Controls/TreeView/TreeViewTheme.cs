using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TreeViewTheme : BaseControlTheme
{
    public const string ItemsPresenterPart = "PART_ItemsPresenter";
    public const string FrameDecoratorPart = "PART_FrameDecorator";

    public TreeViewTheme()
        : base(typeof(TreeView))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<TreeView>((view, scope) =>
        {
            var frameDecorator = new Border
            {
                Name = FrameDecoratorPart
            };
            var scrollViewer = new ScrollViewer();

            BindUtils.RelayBind(view, ScrollViewer.AllowAutoHideProperty, scrollViewer,
                ScrollViewer.AllowAutoHideProperty);
            BindUtils.RelayBind(view, ScrollViewer.HorizontalScrollBarVisibilityProperty, scrollViewer,
                ScrollViewer.HorizontalScrollBarVisibilityProperty);
            BindUtils.RelayBind(view, ScrollViewer.VerticalScrollBarVisibilityProperty, scrollViewer,
                ScrollViewer.VerticalScrollBarVisibilityProperty);
            BindUtils.RelayBind(view, ScrollViewer.IsScrollChainingEnabledProperty, scrollViewer,
                ScrollViewer.IsScrollChainingEnabledProperty);
            BindUtils.RelayBind(view, ScrollViewer.IsDeferredScrollingEnabledProperty, scrollViewer,
                ScrollViewer.IsDeferredScrollingEnabledProperty);

            var itemsPresenter = new ItemsPresenter
            {
                Name = ItemsPresenterPart
            };

            BindUtils.RelayBind(view, ItemsControl.ItemsPanelProperty, itemsPresenter,
                ItemsPresenter.ItemsPanelProperty);
            itemsPresenter.RegisterInNameScope(scope);
            scrollViewer.Content = itemsPresenter;
            frameDecorator.Child = scrollViewer;
            return frameDecorator;
        });
    }
}