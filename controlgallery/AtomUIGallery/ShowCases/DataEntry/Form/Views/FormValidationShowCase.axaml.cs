using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AtomUIGallery.Localization;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Form;

public partial class FormValidationShowCase : ReactiveUserControl<FormViewModel>
{
    private WindowMessageManager? _messageManager;

    public FormValidationShowCase()
    {
        InitializeComponent();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _messageManager?.Dispose();
        _messageManager = null;
    }

    private void HandleFillClicked(object? sender, RoutedEventArgs args)
    {
        var formValues = new FormValues();
        formValues.Add("url", "https://taobao.com/");
        NoBlockRuleForm.SetFormValues(formValues);
    }

    private void HandleNoBlockFormSubmitted(object? sender, FormSubmittedEventArgs args)
    {
        GetMessageManager()?.Show(new AtomUIMessage(
            type: MessageType.Success,
            content: FormShowCaseLanguage.Get(FormShowCaseLangResourceKind.P3SubmitSuccessMessage,
                "Submit success!")
        ));
    }

    private void HandleNoBlockFormValidated(object? sender, FormValidatedEventArgs args)
    {
        if (args.Result == FormValidateResult.Error)
        {
            GetMessageManager()?.Show(new AtomUIMessage(
                type: MessageType.Error,
                content: FormShowCaseLanguage.Get(FormShowCaseLangResourceKind.P3SubmitFailedMessage,
                    "Submit failed!")
            ));
        }
    }

    private WindowMessageManager? GetMessageManager()
    {
        if (_messageManager is not null)
        {
            return _messageManager;
        }

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null)
        {
            return null;
        }

        _messageManager = new WindowMessageManager(topLevel)
        {
            MaxItems = 10
        };
        return _messageManager;
    }
}
