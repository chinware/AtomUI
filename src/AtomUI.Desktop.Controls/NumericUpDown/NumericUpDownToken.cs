using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Theme;
using AtomUI.Theme.Tokens;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class NumericUpDownToken : ButtonSpinnerToken
{
    public new const string ID = "NumericUpDown";
    
    public NumericUpDownToken()
        : base(ID)
    {
    }
    
}