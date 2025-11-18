using AtomUI.Theme.TokenSystem;

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