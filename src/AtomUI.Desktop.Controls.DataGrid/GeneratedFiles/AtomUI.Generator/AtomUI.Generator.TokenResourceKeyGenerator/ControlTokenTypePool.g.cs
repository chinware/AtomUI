using System.Collections.Generic;
using AtomUI.Theme;

namespace AtomUI.Theme
{
    internal class ControlTokenTypePool
    {
        internal static IList<Type> GetTokenTypes()
        {
            List<Type> tokenTypes = new List<Type>(1);
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.DataGridToken));
            return tokenTypes;
        }
    }
}