using System.Collections.Generic;
using AtomUI.Theme;

namespace AtomUI.Theme
{
    internal class ControlThemePool
    {
        internal static IList<BaseControlTheme> GetControlThemes()
        {
            List<BaseControlTheme> themes = new List<BaseControlTheme>();
            themes.Add(new AtomUI.Controls.BaseOverflowMenuItemTheme());
            themes.Add(new AtomUI.Controls.BaseTabScrollViewerTheme());
            themes.Add(new AtomUI.Controls.CardTabControlTheme());
            themes.Add(new AtomUI.Controls.CardTabItemTheme());
            themes.Add(new AtomUI.Controls.CardTabStripItemTheme());
            themes.Add(new AtomUI.Controls.CardTabStripTheme());
            themes.Add(new AtomUI.Controls.CircleProgressTheme());
            themes.Add(new AtomUI.Controls.DashboardProgressTheme());
            themes.Add(new AtomUI.Controls.HyperLinkTextBlockTheme());
            themes.Add(new AtomUI.Controls.MenuFlyoutPresenterTheme());
            themes.Add(new AtomUI.Controls.ProgressBarTheme());
            themes.Add(new AtomUI.Controls.SegmentedItemTheme());
            themes.Add(new AtomUI.Controls.SegmentedTheme());
            themes.Add(new AtomUI.Controls.SeparatorTheme());
            themes.Add(new AtomUI.Controls.SliderTheme());
            themes.Add(new AtomUI.Controls.SliderThumbTheme());
            themes.Add(new AtomUI.Controls.StepsProgressBarTheme());
            themes.Add(new AtomUI.Controls.TabControlTheme());
            themes.Add(new AtomUI.Controls.TabItemTheme());
            themes.Add(new AtomUI.Controls.TabStripItemTheme());
            themes.Add(new AtomUI.Controls.TabStripTheme());
            themes.Add(new AtomUI.Controls.TimelineItemTheme());
            themes.Add(new AtomUI.Controls.TimelineTheme());
            return themes;
        }
    }
}