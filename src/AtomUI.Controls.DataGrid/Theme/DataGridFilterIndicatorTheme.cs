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
            var frame = new Border
            {
                Name = FramePart
            };
            var iconPresenter = new IconButton()
            {
                Name = IconPresenterPart,
                IsEnableHoverEffect = true
            };
            CreateTemplateParentBinding(iconPresenter, IconButton.IconProperty, DataGridFilterIndicator.IconProperty);
            frame.Child = iconPresenter;
            return frame;
        });
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        
        var iconButtonStyle = new Style(selector => selector.Nesting().Template().Name(IconPresenterPart));
        iconButtonStyle.Add(IconButton.NormalIconBrushProperty, DataGridTokenKey.HeaderIconColor);
        iconButtonStyle.Add(IconButton.ActiveIconBrushProperty, DataGridTokenKey.HeaderIconHoverColor);
        iconButtonStyle.Add(IconButton.SelectedIconBrushProperty, DataGridTokenKey.HeaderIconHoverColor);
        iconButtonStyle.Add(IconButton.DisabledIconBrushProperty, SharedTokenKey.ColorTextDisabled);
        iconButtonStyle.Add(IconButton.IconWidthProperty, SharedTokenKey.IconSizeSM);
        iconButtonStyle.Add(IconButton.IconHeightProperty, SharedTokenKey.IconSizeSM);
        iconButtonStyle.Add(IconButton.PaddingProperty, DataGridTokenKey.FilterIndicatorPadding);
        iconButtonStyle.Add(IconButton.CornerRadiusProperty, SharedTokenKey.BorderRadius);
        commonStyle.Add(iconButtonStyle);
        
        Add(commonStyle);
    }
}