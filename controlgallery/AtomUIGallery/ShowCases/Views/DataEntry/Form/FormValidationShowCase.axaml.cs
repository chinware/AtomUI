using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUIGallery.ShowCases.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

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
        GetMessageManager()?.Show(new Message(
            type: MessageType.Success,
            content: "Submit success!"
        ));
    }

    private void HandleNoBlockFormValidated(object? sender, FormValidatedEventArgs args)
    {
        if (args.Result == FormValidateResult.Error)
        {
            GetMessageManager()?.Show(new Message(
                type: MessageType.Error,
                content: "Submit failed!"
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
