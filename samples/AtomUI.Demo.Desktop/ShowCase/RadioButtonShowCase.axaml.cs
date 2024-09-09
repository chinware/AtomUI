using Avalonia.Controls;

namespace AtomUI.Demo.Desktop.ShowCase;

public partial class RadioButtonShowCase : UserControl
{
    public RadioButtonShowCase()
    {
        CheckRadios = new List<string>
        {
            "ToggleDisabledRadioUnChecked",
            "ToggleDisabledRadioChecked"
        };
        InitializeComponent();
    }

    protected List<string> CheckRadios { get; set; }

    public static void ToggleDisabledStatus(object arg)
    {
        var btn = (arg as Button)!;
        var stackPanel = btn.Parent as StackPanel;
        var radioBtn1 = stackPanel?.FindControl<RadioButton>("ToggleDisabledRadioUnChecked");
        var radioBtn2 = stackPanel?.FindControl<RadioButton>("ToggleDisabledRadioChecked");
        if (radioBtn1 != null) radioBtn1.IsEnabled = !radioBtn1.IsEnabled;

        if (radioBtn2 != null) radioBtn2.IsEnabled = !radioBtn2.IsEnabled;
    }
}