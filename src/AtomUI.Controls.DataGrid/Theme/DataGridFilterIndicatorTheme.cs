using AtomUI.IconPkg;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class DataGridFilterIndicatorTheme : BaseControlTheme
{
    public const string FramePart = "PART_Frame";
    public const string IconPresenterPart = "PART_IconPresenter";
    
    public DataGridFilterIndicatorTheme()
        : base(typeof(DataGridFilterIndicator))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<DataGridFilterIndicator>((columnHeader, scope) =>
        {
            var frame = new Border()
            {
                Name = FramePart
            };
            var iconPresenter = new IconPresenter()
            {
                Name = IconPresenterPart,
            };
            CreateTemplateParentBinding(iconPresenter, IconPresenter.IconProperty, DataGridFilterIndicator.IconProperty);
            frame.Child = iconPresenter;
            return frame;
        });
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        
        var iconStyle = new Style(selector => selector.Nesting().Template().Name(IconPresenterPart).Descendant().OfType<Icon>());
        iconStyle.Add(Icon.WidthProperty, SharedTokenKey.IconSizeSM);
        iconStyle.Add(Icon.HeightProperty, SharedTokenKey.IconSizeSM);
        commonStyle.Add(iconStyle);
        
        Add(commonStyle);
    }
}