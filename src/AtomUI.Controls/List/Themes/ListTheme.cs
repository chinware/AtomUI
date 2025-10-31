using AtomUI.Controls.Converters;
using Avalonia.Styling;

namespace AtomUI.Controls.Themes;

internal class ListTheme : ControlTheme
{
    public static readonly ListPaginationVisibilityConvertor TopPaginationVisibilityConvertor =
        new()
        {
            IsTop = true
        };
    
    public static readonly ListPaginationVisibilityConvertor BottomPaginationVisibilityConvertor =
        new()
        {
            IsTop = false
        };
}