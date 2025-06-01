using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class DataGridHeaderViewTheme : BaseControlTheme
{
    public const string ItemsPresenterPart = "PART_ItemsPresenter";
    public const string FramePart = "PART_Frame";
    
    public DataGridHeaderViewTheme()
        : base(typeof(DataGridHeaderView))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<DataGridHeaderView>((headerView, scope) =>
        {
            var frame = new Border
            {
                Name = FramePart,
                ClipToBounds = true,
            };
            CreateTemplateParentBinding(frame, Border.BackgroundProperty, DataGridHeaderView.BackgroundProperty);
            CreateTemplateParentBinding(frame, Border.CornerRadiusProperty, DataGridHeaderView.CornerRadiusProperty);
            var itemsPresenter = new ItemsPresenter
            {
                Name = ItemsPresenterPart
            };
            CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.ItemsPanelProperty, DataGridHeaderView.ItemsPanelProperty);
            itemsPresenter.RegisterInNameScope(scope);
            frame.Child = itemsPresenter;
            return frame;
        });
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(DataGridHeaderView.ForegroundProperty, DataGridTokenKey.TableHeaderTextColor);
        commonStyle.Add(DataGridHeaderView.BackgroundProperty, DataGridTokenKey.TableHeaderBg);
        commonStyle.Add(DataGridHeaderView.FontWeightProperty, SharedTokenKey.FontWeightStrong);
        Add(commonStyle);
    }
}