using System.Collections.Generic;
using AtomUI.Theme;

namespace AtomUI.Theme
{
    internal class ControlTokenTypePool
    {
        internal static IList<Type> GetTokenTypes()
        {
            List<Type> tokenTypes = new List<Type>();
            tokenTypes.Add(typeof(AtomUI.Controls.AddOnDecoratedBoxToken));
            tokenTypes.Add(typeof(AtomUI.Controls.AdornerLayerToken));
            tokenTypes.Add(typeof(AtomUI.Controls.AlertToken));
            tokenTypes.Add(typeof(AtomUI.Controls.ArrowDecoratedBoxToken));
            tokenTypes.Add(typeof(AtomUI.Controls.AvatarToken));
            tokenTypes.Add(typeof(AtomUI.Controls.BadgeToken));
            tokenTypes.Add(typeof(AtomUI.Controls.ButtonSpinnerToken));
            tokenTypes.Add(typeof(AtomUI.Controls.ButtonToken));
            tokenTypes.Add(typeof(AtomUI.Controls.CalendarToken));
            tokenTypes.Add(typeof(AtomUI.Controls.CheckBoxToken));
            tokenTypes.Add(typeof(AtomUI.Controls.ChromeToken));
            tokenTypes.Add(typeof(AtomUI.Controls.CollapseToken));
            tokenTypes.Add(typeof(AtomUI.Controls.ComboBoxToken));
            tokenTypes.Add(typeof(AtomUI.Controls.DatePickerToken));
            tokenTypes.Add(typeof(AtomUI.Controls.DrawerToken));
            tokenTypes.Add(typeof(AtomUI.Controls.EmptyIndicatorToken));
            tokenTypes.Add(typeof(AtomUI.Controls.ExpanderToken));
            tokenTypes.Add(typeof(AtomUI.Controls.FlyoutPresenterToken));
            tokenTypes.Add(typeof(AtomUI.Controls.GroupBoxToken));
            tokenTypes.Add(typeof(AtomUI.Controls.LineEditToken));
            tokenTypes.Add(typeof(AtomUI.Controls.ListBoxToken));
            tokenTypes.Add(typeof(AtomUI.Controls.LoadingIndicatorToken));
            tokenTypes.Add(typeof(AtomUI.Controls.MarqueeLabelToken));
            tokenTypes.Add(typeof(AtomUI.Controls.MenuToken));
            tokenTypes.Add(typeof(AtomUI.Controls.MessageToken));
            tokenTypes.Add(typeof(AtomUI.Controls.NavMenuToken));
            tokenTypes.Add(typeof(AtomUI.Controls.NotificationToken));
            tokenTypes.Add(typeof(AtomUI.Controls.NumericUpDownToken));
            tokenTypes.Add(typeof(AtomUI.Controls.OptionButtonToken));
            tokenTypes.Add(typeof(AtomUI.Controls.PaginationToken));
            tokenTypes.Add(typeof(AtomUI.Controls.PopupConfirmToken));
            tokenTypes.Add(typeof(AtomUI.Controls.Primitives.InfoPickerInputToken));
            tokenTypes.Add(typeof(AtomUI.Controls.ProgressBarToken));
            tokenTypes.Add(typeof(AtomUI.Controls.RadioButtonToken));
            tokenTypes.Add(typeof(AtomUI.Controls.ScrollBarToken));
            tokenTypes.Add(typeof(AtomUI.Controls.SegmentedToken));
            tokenTypes.Add(typeof(AtomUI.Controls.SeparatorToken));
            tokenTypes.Add(typeof(AtomUI.Controls.SliderToken));
            tokenTypes.Add(typeof(AtomUI.Controls.TabControlToken));
            tokenTypes.Add(typeof(AtomUI.Controls.TagToken));
            tokenTypes.Add(typeof(AtomUI.Controls.TimelineToken));
            tokenTypes.Add(typeof(AtomUI.Controls.TimePickerToken));
            tokenTypes.Add(typeof(AtomUI.Controls.ToggleSwitchToken));
            tokenTypes.Add(typeof(AtomUI.Controls.ToolTipToken));
            tokenTypes.Add(typeof(AtomUI.Controls.TreeFlyoutToken));
            tokenTypes.Add(typeof(AtomUI.Controls.TreeViewToken));
            tokenTypes.Add(typeof(AtomUI.Controls.WindowToken));
            return tokenTypes;
        }
    }
}