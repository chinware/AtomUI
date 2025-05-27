using System.Collections.Generic;
using AtomUI.Theme;

namespace AtomUI.Theme
{
    internal class ControlThemePool
    {
        internal static IList<BaseControlTheme> GetControlThemes()
        {
            List<BaseControlTheme> themes = new List<BaseControlTheme>();
            themes.Add(new AtomUI.Controls.DataGridTheme());
            return themes;
        }
    }
}