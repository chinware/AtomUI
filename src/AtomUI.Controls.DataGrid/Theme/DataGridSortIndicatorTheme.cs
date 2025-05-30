using System.ComponentModel;
using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class DataGridSortIndicatorTheme : BaseControlTheme
{
    public const string AscendingPart = "PART_Ascending";
    public const string DescendingPart = "PART_Descending";

    public DataGridSortIndicatorTheme()
        : base(typeof(DataGridSortIndicator))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<DataGridSortIndicator>((indicator, scope) =>
        {
            var layout        = new StackPanel();
            var ascendingIcon = AntDesignIconPackage.CaretUpOutlined();
            ascendingIcon.Name = AscendingPart;
            layout.Children.Add(ascendingIcon);
            var descendingIcon = AntDesignIconPackage.CaretDownOutlined();
            descendingIcon.Name = DescendingPart;
            layout.Children.Add(descendingIcon);
            return layout;
        });
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        var iconsStyle  = new Style(selector => selector.Nesting().Template().Descendant().OfType<Icon>());
        iconsStyle.Add(Icon.WidthProperty, DataGridTokenKey.SortIconSize);
        iconsStyle.Add(Icon.HeightProperty, DataGridTokenKey.SortIconSize);
        iconsStyle.Add(Icon.NormalFilledBrushProperty, DataGridTokenKey.HeaderIconColor);
        iconsStyle.Add(Icon.ActiveFilledBrushProperty, DataGridTokenKey.HeaderIconHoverColor);
        iconsStyle.Add(Icon.SelectedFilledBrushProperty, SharedTokenKey.ColorPrimary);
        iconsStyle.Add(Icon.DisabledFilledBrushProperty, SharedTokenKey.ColorTextDisabled);
        commonStyle.Add(iconsStyle);
        BuildAscendingStyle(commonStyle);
        BuildDescendingStyle(commonStyle);
        Add(commonStyle);
    }

    private void BuildAscendingStyle(Style commonStyle)
    {
        var activatedStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(DataGridSortIndicator.CurrentSortDirectionProperty,
                ListSortDirection.Ascending));
        {
            var ascendingIconStyle = new Style(selector => selector.Nesting().Template().Name(AscendingPart));
            ascendingIconStyle.Add(Icon.IconModeProperty, IconMode.Selected);
            activatedStyle.Add(ascendingIconStyle);
        }
        
        commonStyle.Add(activatedStyle);
        
        var notActivatedStyle = new Style(selector => selector.Nesting().Not(x => 
            x.PropertyEquals(DataGridSortIndicator.CurrentSortDirectionProperty,
                ListSortDirection.Ascending)));
        {
            var ascendingIconStyle = new Style(selector => selector.Nesting().Template().Name(AscendingPart));
            ascendingIconStyle.Add(Icon.IconModeProperty, IconMode.Normal);
            notActivatedStyle.Add(ascendingIconStyle);
        }
        var isHoverStyle = new Style(selector => selector.Nesting().PropertyEquals(DataGridSortIndicator.IsHoverModeProperty, true));
        {
            var ascendingIconStyle = new Style(selector => selector.Nesting().Template().Name(AscendingPart));
            ascendingIconStyle.Add(Icon.IconModeProperty, IconMode.Active);
            isHoverStyle.Add(ascendingIconStyle);
        }
        notActivatedStyle.Add(isHoverStyle);
        commonStyle.Add(notActivatedStyle);
    }

    private void BuildDescendingStyle(Style commonStyle)
    {
        var activatedStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(DataGridSortIndicator.CurrentSortDirectionProperty,
                ListSortDirection.Descending));
        {
            var descendingIconStyle = new Style(selector => selector.Nesting().Template().Name(DescendingPart));
            descendingIconStyle.Add(Icon.IconModeProperty, IconMode.Selected);
            activatedStyle.Add(descendingIconStyle);
        }
        
        commonStyle.Add(activatedStyle);
        
        var notActivatedStyle = new Style(selector => selector.Nesting().Not(x => 
            x.PropertyEquals(DataGridSortIndicator.CurrentSortDirectionProperty,
                ListSortDirection.Descending)));
        {
            var descendingIconStyle = new Style(selector => selector.Nesting().Template().Name(DescendingPart));
            descendingIconStyle.Add(Icon.IconModeProperty, IconMode.Normal);
            notActivatedStyle.Add(descendingIconStyle);
        }
        var isHoverStyle = new Style(selector => selector.Nesting().PropertyEquals(DataGridSortIndicator.IsHoverModeProperty, true));
        {
            var descendingIconStyle = new Style(selector => selector.Nesting().Template().Name(DescendingPart));
            descendingIconStyle.Add(Icon.IconModeProperty, IconMode.Active);
            isHoverStyle.Add(descendingIconStyle);
        }
        notActivatedStyle.Add(isHoverStyle);
        commonStyle.Add(notActivatedStyle);
    }
}