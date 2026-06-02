using AtomUI.Controls;
using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class FormBasicShowCase : ReactiveUserControl<FormViewModel>
{
    public FormBasicShowCase()
    {
        InitializeComponent();
        BasicForm.InitialValues = CreateBasicFormInitialValues();
    }

    private static FormValues CreateBasicFormInitialValues()
    {
        var values = new FormValues();
        values.Add("remember", true);
        return values;
    }
}
