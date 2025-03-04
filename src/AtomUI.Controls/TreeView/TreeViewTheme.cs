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
    public const string FramePart = "PART_Frame";

    public TreeViewTheme()
        : base(typeof(TreeView))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<TreeView>((view, scope) =>
        {
            var frame = new Border
            {
                Name = FramePart
            };
            var scrollViewer = new ScrollViewer();

            CreateTemplateParentBinding(scrollViewer, ScrollViewer.AllowAutoHideProperty, ScrollViewer.AllowAutoHideProperty);
            CreateTemplateParentBinding(scrollViewer, ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollViewer.HorizontalScrollBarVisibilityProperty);
            CreateTemplateParentBinding(scrollViewer, ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollViewer.VerticalScrollBarVisibilityProperty);
            CreateTemplateParentBinding(scrollViewer, ScrollViewer.IsScrollChainingEnabledProperty, ScrollViewer.IsScrollChainingEnabledProperty);
            CreateTemplateParentBinding(scrollViewer, ScrollViewer.IsDeferredScrollingEnabledProperty, ScrollViewer.IsDeferredScrollingEnabledProperty);

            var itemsPresenter = new ItemsPresenter
            {
                Name = ItemsPresenterPart
            };

            CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.ItemsPanelProperty, ItemsControl.ItemsPanelProperty);

            itemsPresenter.RegisterInNameScope(scope);
            scrollViewer.Content = itemsPresenter;
            frame.Child          = scrollViewer;
            return frame;
        });
    }
}