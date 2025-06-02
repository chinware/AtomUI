using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Styling;

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
            frame.RegisterInNameScope(scope);
            var rootLayout = new DataGridHeaderViewItemPanel
            {
                Name = RootLayoutPart,
            };

            var headerPresenter = new ContentPresenter
            {
                Name = HeaderPart,
            };
            CreateTemplateParentBinding(headerPresenter, ContentPresenter.ContentProperty, DataGridHeaderViewItem.HeaderProperty);
            CreateTemplateParentBinding(headerPresenter, ContentPresenter.ContentTemplateProperty, DataGridHeaderViewItem.HeaderTemplateProperty);
            rootLayout.Header = headerPresenter;
            
            var itemsPresenter = new ItemsPresenter
            {
                Name = ItemsPresenterPart
            };
            CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.ItemsPanelProperty, DataGridHeaderViewItem.ItemsPanelProperty);
            rootLayout.ItemsPresenter = itemsPresenter;
            frame.Child = rootLayout;
            return frame;
        });
    }
    
    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        var frameStyle  = new Style(selector => selector.Nesting().Template().Name(FramePart));
        frameStyle.Add(Border.BorderBrushProperty, DataGridTokenKey.TableBorderColor);

        var isLeafStyle = new Style(selector => selector.Nesting().PropertyEquals(DataGridHeaderViewItem.IsLeafProperty, true));
        var itemsPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ItemsPresenterPart));
        itemsPresenterStyle.Add(ItemsPresenter.IsVisibleProperty, false);
        isLeafStyle.Add(itemsPresenterStyle);
        commonStyle.Add(isLeafStyle);
        
        commonStyle.Add(frameStyle);
        Add(commonStyle);
    }
}