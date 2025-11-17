using AtomUI.Theme.TokenSystem;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class NumericUpDownToken : ButtonSpinnerToken
{
    public new const string ID = "NumericUpDown";

    public NumericUpDownToken()
        : base(ID)
    {
    }
}