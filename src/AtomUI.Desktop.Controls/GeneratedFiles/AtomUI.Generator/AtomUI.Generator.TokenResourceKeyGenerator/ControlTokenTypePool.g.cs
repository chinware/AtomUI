using System.Collections.Generic;
using AtomUI.Theme;

namespace AtomUI.Theme
{
    internal class ControlTokenTypePool
    {
        internal static IList<Type> GetTokenTypes()
        {
            List<Type> tokenTypes = new List<Type>();
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.AddOnDecoratedBoxToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.AdornerLayerToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.AlertToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.ArrowDecoratedBoxToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.AvatarToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.BadgeToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.BreadcrumbToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.ButtonSpinnerToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.ButtonToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.CalendarToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.CardToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.CarouselToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.CheckBoxToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.CollapseToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.ComboBoxToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.EmptyToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.ExpanderToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.FlyoutHostToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.GroupBoxToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.LineEditToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.ListBoxToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.MarqueeLabelToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.MenuToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.MessageToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.NavMenuToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.NotificationToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.NumericUpDownToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.OptionButtonToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.PopupConfirmToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.PopupHostToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.ProgressBarToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.RadioButtonToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.RateToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.ResultToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.ScrollViewerToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.SegmentedToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.SeparatorToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.SkeletonToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.SliderToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.SpaceToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.SpinToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.SplitterToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.SplitViewToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.StepsToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.TabControlToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.TagToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.TextAreaToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.TimelineToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.ToggleSwitchToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.ToolTipToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.TreeFlyoutToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.WindowTitleBarToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.WindowToken));
            return tokenTypes;
        }
    }
}