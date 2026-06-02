using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia.Interactivity;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Form;

public partial class FormDynamicShowCase : ReactiveUserControl<FormViewModel>
{
    private static int s_formGid = 3;

    public FormDynamicShowCase()
    {
        InitializeComponent();
    }

    private void HandleAddFormItem(object? sender, RoutedEventArgs args)
    {
        var formItem = CreatePassengerFormItem();
        var insertIndex = 0;
        for (var i = 0; i < DynamicForm.Items.Count; ++i)
        {
            var item = DynamicForm.Items[i];
            if (item is FormActionsItem)
            {
                insertIndex = i;
                break;
            }
        }

        insertIndex = Math.Max(0, insertIndex);
        DynamicForm.Items.Insert(insertIndex, formItem);
    }

    private void HandleAddFormItemAtHead(object? sender, RoutedEventArgs args)
    {
        DynamicForm.Items.Insert(0, CreatePassengerFormItem());
    }

    private static FormItem CreatePassengerFormItem()
    {
        var id = s_formGid++;
        return new FormItem
        {
            FieldName = $"Passengers_{id}",
            LabelText = $"passengers_{id}",
            Content   = new AtomUILineEdit(),
            Validators = new List<IFormValidator>()
            {
                new FormStringNotEmptyValidator()
                {
                    Message = "Please input passenger's name or delete this field!",
                }
            }
        };
    }
}
