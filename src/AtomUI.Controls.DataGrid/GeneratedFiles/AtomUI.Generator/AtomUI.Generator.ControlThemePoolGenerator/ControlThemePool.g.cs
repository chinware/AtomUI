using System.Collections.Generic;
using AtomUI.Theme;

namespace AtomUI.Theme
{
    internal class ControlThemePool
    {
        internal static IList<BaseControlTheme> GetControlThemes()
        {
            List<BaseControlTheme> themes = new List<BaseControlTheme>();
            themes.Add(new AtomUI.Controls.DataGridCellTheme());
            themes.Add(new AtomUI.Controls.DataGridColumnHeaderTheme());
            themes.Add(new AtomUI.Controls.DataGridRowGroupHeaderTheme());
            themes.Add(new AtomUI.Controls.DataGridRowHeaderTheme());
            themes.Add(new AtomUI.Controls.DataGridRowTheme());
            themes.Add(new AtomUI.Controls.DataGridSortIndicatorTheme());
            themes.Add(new AtomUI.Controls.DataGridTheme());
            themes.Add(new AtomUI.Controls.DataGridTopLeftColumnHeaderTheme());
            return themes;
        }
    }
}