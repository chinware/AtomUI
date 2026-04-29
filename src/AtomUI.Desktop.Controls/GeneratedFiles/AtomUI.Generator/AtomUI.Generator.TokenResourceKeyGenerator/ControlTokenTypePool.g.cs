using System.Collections.Generic;
using AtomUI.Theme;

namespace AtomUI.Theme
{
    internal class ControlTokenTypePool
    {
        internal static IList<Type> GetTokenTypes()
        {
            List<Type> tokenTypes = new List<Type>();
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.AdornerLayerToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.ArrowDecoratedBoxToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.BreadcrumbToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.ButtonToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.CheckBoxToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.MarqueeLabelToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.NavMenuToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.PopupHostToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.ProgressBarToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.RadioButtonToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.RateToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.ResultToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.ScrollViewerToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.SegmentedToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.SpaceToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.SplitterToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.ToolTipToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.WindowTitleBarToken));
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.WindowToken));
            return tokenTypes;
        }
    }
}