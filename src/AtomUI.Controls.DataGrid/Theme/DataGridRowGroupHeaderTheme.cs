using AtomUI.Theme;
using AtomUI.Theme.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class DataGridRowGroupHeaderTheme : BaseControlTheme
{
    public const string ExpanderButtonPart = "PART_ExpanderButton";
    public const string IndentSpacerPart = "PART_IndentSpacer";
    public const string ItemCountElementPart = "PART_ItemCountElement";
    public const string PropertyNameElementPart = "PART_PropertyNameElement";
    
    public DataGridRowGroupHeaderTheme()
        : base(typeof(DataGridRowGroupHeader))
    {
    }
    
    
}