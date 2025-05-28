using AtomUI.Theme;
using AtomUI.Theme.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class DataGridRowTheme : BaseControlTheme
{
    public const string BottomGridLinePart = "PART_BottomGridLine";
    public const string CellsPresenterPart = "PART_CellsPresenter";
    public const string DetailsPresenterPart = "PART_DetailsPresenter";
    public const string FramePart = "PART_Frame";
    public const string RowHeaderPart = "PART_RowHeader";
    
    public DataGridRowTheme() : base(typeof(DataGridRow))
    {
    }
}