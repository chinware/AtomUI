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
            themes.Add(new AtomUI.Controls.ContextMenuTheme());
            themes.Add(new AtomUI.Controls.DashboardProgressTheme());
            themes.Add(new AtomUI.Controls.DatePickerFlyoutPresenterTheme());
            themes.Add(new AtomUI.Controls.DatePickerPresenterTheme());
            themes.Add(new AtomUI.Controls.DrawerContainerTheme());
            themes.Add(new AtomUI.Controls.DrawerInfoContainerTheme());
            themes.Add(new AtomUI.Controls.DualMonthRangeDatePickerPresenterTheme());
            themes.Add(new AtomUI.Controls.EmptyIndicatorTheme());
            themes.Add(new AtomUI.Controls.ExpanderTheme());
            themes.Add(new AtomUI.Controls.GroupBoxTheme());
            themes.Add(new AtomUI.Controls.HorizontalNavMenuTheme());
            themes.Add(new AtomUI.Controls.HyperLinkTextBlockTheme());
            themes.Add(new AtomUI.Controls.InlineNavMenuItemTheme());
            themes.Add(new AtomUI.Controls.ListBoxItemTheme());
            themes.Add(new AtomUI.Controls.ListBoxTheme());
            themes.Add(new AtomUI.Controls.LoadingIndicatorAdornerTheme());
            themes.Add(new AtomUI.Controls.LoadingIndicatorTheme());
            themes.Add(new AtomUI.Controls.MenuFlyoutPresenterTheme());
            themes.Add(new AtomUI.Controls.MenuItemTheme());
            themes.Add(new AtomUI.Controls.MenuScrollViewerTheme());
            themes.Add(new AtomUI.Controls.MenuSeparatorTheme());
            themes.Add(new AtomUI.Controls.MenuTheme());
            themes.Add(new AtomUI.Controls.MessageCardTheme());
            themes.Add(new AtomUI.Controls.NavMenuItemTheme());
            themes.Add(new AtomUI.Controls.NavMenuTheme());
            themes.Add(new AtomUI.Controls.NotificationCardTheme());
            themes.Add(new AtomUI.Controls.OptionButtonGroupTheme());
            themes.Add(new AtomUI.Controls.OptionButtonTheme());
            themes.Add(new AtomUI.Controls.ProgressBarTheme());
            themes.Add(new AtomUI.Controls.RangeDatePickerFlyoutPresenterTheme());
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
            themes.Add(new AtomUI.Controls.TopLevelHorizontalNavMenuItemTheme());
            themes.Add(new AtomUI.Controls.TopLevelMenuItemTheme());
            themes.Add(new AtomUI.Controls.VerticalNavMenuItemTheme());
            themes.Add(new AtomUI.Controls.VerticalNavMenuTheme());
            themes.Add(new AtomUI.Controls.WindowMessageManagerTheme());
            themes.Add(new AtomUI.Controls.WindowNotificationManagerTheme());
            return themes;
        }
    }
}