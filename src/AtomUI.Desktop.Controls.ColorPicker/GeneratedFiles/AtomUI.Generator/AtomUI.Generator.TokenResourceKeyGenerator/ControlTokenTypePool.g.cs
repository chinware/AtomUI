using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using AtomUI.Theme;

namespace AtomUI.Theme
{
    internal class ControlTokenTypePool
    {
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(AtomUI.Desktop.Controls.ColorPickerToken))]
        internal static IList<Type> GetTokenTypes()
        {
            List<Type> tokenTypes = new List<Type>(1);
            tokenTypes.Add(typeof(AtomUI.Desktop.Controls.ColorPickerToken));
            return tokenTypes;
        }
    }
}