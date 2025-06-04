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

        {
            var iconButtonStyle = new Style(selector => selector.Nesting().Template().Name(IconPresenterPart));
            iconButtonStyle.Add(IconButton.IconWidthProperty, SharedTokenKey.IconSizeSM);
            iconButtonStyle.Add(IconButton.IconHeightProperty, SharedTokenKey.IconSizeSM);
            iconButtonStyle.Add(IconButton.PaddingProperty, DataGridTokenKey.FilterIndicatorPadding);
            iconButtonStyle.Add(IconButton.CornerRadiusProperty, SharedTokenKey.BorderRadius);
            iconButtonStyle.Add(IconButton.DisabledIconBrushProperty, SharedTokenKey.ColorTextDisabled);
            commonStyle.Add(iconButtonStyle);
        }
        var isFilterActivatedStyle = new Style(selector => selector.Nesting().PropertyEquals(DataGridFilterIndicator.IsFilterActivatedProperty, true));
        {
            var iconButtonStyle = new Style(selector => selector.Nesting().Template().Name(IconPresenterPart));
            iconButtonStyle.Add(IconButton.NormalIconBrushProperty, SharedTokenKey.ColorPrimary);
            iconButtonStyle.Add(IconButton.ActiveIconBrushProperty, SharedTokenKey.ColorPrimary);
            iconButtonStyle.Add(IconButton.SelectedIconBrushProperty, SharedTokenKey.ColorPrimary);
            isFilterActivatedStyle.Add(iconButtonStyle);
        }
        commonStyle.Add(isFilterActivatedStyle);
        var isNormalStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(DataGridFilterIndicator.IsFilterActivatedProperty, false));

        {
            var iconButtonStyle = new Style(selector => selector.Nesting().Template().Name(IconPresenterPart));
            iconButtonStyle.Add(IconButton.NormalIconBrushProperty, DataGridTokenKey.HeaderIconColor);
            iconButtonStyle.Add(IconButton.ActiveIconBrushProperty, DataGridTokenKey.HeaderIconHoverColor);
            iconButtonStyle.Add(IconButton.SelectedIconBrushProperty, DataGridTokenKey.HeaderIconHoverColor);
      
            isNormalStyle.Add(iconButtonStyle);
        }
 
        commonStyle.Add(isNormalStyle);
        Add(commonStyle);
    }
}