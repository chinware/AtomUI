using System.Globalization;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using AtomUIGallery.Localization;
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
        var formItem = new FormItem
        {
            FieldName = $"Passengers_{id}",
            Content   = new AtomUILineEdit(),
            Validators = new List<IFormValidator>()
            {
                new LocalizedPassengerNameValidator()
            }
        };
        BindPassengerLabel(formItem, id);
        return formItem;
    }

    private static void BindPassengerLabel(FormItem formItem, int id)
    {
        _ = LanguageResourceBinder.CreateBinding(
            formItem,
            FormItem.LabelTextProperty,
            FormShowCaseLangResourceKind.P3DynamicPassengerLabelFormat,
            BindingPriority.LocalValue,
            value =>
            {
                var format = value as string ?? "passengers_{0}";
                return string.Format(CultureInfo.CurrentCulture, format, id);
            });
    }

    private sealed class LocalizedPassengerNameValidator : FormStringNotEmptyValidator
    {
        public LocalizedPassengerNameValidator()
        {
            RefreshMessage();
        }

        protected override Task<bool> ValidateCoreAsync(string fieldName, object? value, CancellationToken cancellationToken)
        {
            RefreshMessage();
            return base.ValidateCoreAsync(fieldName, value, cancellationToken);
        }

        private void RefreshMessage()
        {
            Message = FormShowCaseLanguage.Get(
                FormShowCaseLangResourceKind.P2MessagePleaseInputPassengerSNameOrDeleteThisField,
                "Please input passenger's name or delete this field!");
        }
    }
}
