using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Layout;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class DataGridHeaderViewItemTheme : BaseControlTheme
{
    public const string FramePart = "PART_Frame";
    public const string ItemsPresenterPart = "PART_ItemsPresenter";
    public const string RootLayoutPart = "PART_RootLayout";
    public const string HeaderPart = "PART_Header";
    
    public DataGridHeaderViewItemTheme()
        : base(typeof(DataGridHeaderViewItem))
    {
    }
    
    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<DataGridHeaderViewItem>((groupHeader, scope) =>
        {
            var frame = new Border
            {
                Name = FramePart,
            };
            
            var rootLayout = new StackPanel
            {
                Name = RootLayoutPart,
                Orientation = Orientation.Vertical
            };

            var headerPresenter = new ContentPresenter
            {
                Name = HeaderPart,
            };
            CreateTemplateParentBinding(headerPresenter, ContentPresenter.ContentProperty, DataGridHeaderViewItem.HeaderProperty);
            CreateTemplateParentBinding(headerPresenter, ContentPresenter.ContentTemplateProperty, DataGridHeaderViewItem.HeaderTemplateProperty);
            rootLayout.Children.Add(headerPresenter);
            
            var itemsPresenter = new ItemsPresenter
            {
                Name = ItemsPresenterPart
            };
            CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.ItemsPanelProperty, DataGridHeaderViewItem.ItemsPanelProperty);
            rootLayout.Children.Add(itemsPresenter);
            frame.Child = rootLayout;
            return frame;
        });
    }
    
    protected override void BuildStyles()
    {
    }
}