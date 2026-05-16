using AtomUI.Desktop.Controls;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateDialogScenarios()
    {
        return
        [
            new PerfScenario("DialogButtonBox.OkCancel", _ => CreateDialogButtonBox(DialogStandardButton.Ok | DialogStandardButton.Cancel)),
            new PerfScenario("DialogButtonBox.AllStandard", _ => CreateDialogButtonBox(
                DialogStandardButton.Ok |
                DialogStandardButton.Open |
                DialogStandardButton.Save |
                DialogStandardButton.Cancel |
                DialogStandardButton.Close |
                DialogStandardButton.Discard |
                DialogStandardButton.Apply |
                DialogStandardButton.Reset |
                DialogStandardButton.Help |
                DialogStandardButton.Yes |
                DialogStandardButton.No)),
            new PerfScenario("DialogButtonBox.Custom", _ => CreateCustomDialogButtonBox()),
            new PerfScenario("MessageBox.Closed.Information", _ => CreateMessageBox(MessageBoxStyle.Information)),
            new PerfScenario("MessageBox.Closed.Confirm", _ => CreateMessageBox(MessageBoxStyle.Confirm)),
            new PerfScenario("MessageBox.Closed.Loading", _ => CreateMessageBox(MessageBoxStyle.Information, isLoading: true))
        ];
    }

    private static Dialog CreateBasicDialog(bool isLoading = false)
    {
        return new Dialog
        {
            Title                     = isLoading ? "Loading Modal" : "Basic Modal",
            IsModal                   = true,
            IsResizable               = false,
            IsDragMovable             = true,
            IsMaximizable             = false,
            StandardButtons           = DialogStandardButton.Cancel | DialogStandardButton.Ok,
            DefaultStandardButton     = DialogStandardButton.Ok,
            HorizontalStartupLocation = DialogHorizontalAnchor.Center,
            VerticalStartupLocation   = DialogVerticalAnchor.Center,
            HostMinWidth              = 300,
            IsLoading                 = isLoading,
            Content                   = CreateDialogContent()
        };
    }

    private static MessageBox CreateMessageBox(MessageBoxStyle style, bool isLoading = false)
    {
        return new MessageBox
        {
            Title       = style == MessageBoxStyle.Confirm
                ? "Do you want to delete these items?"
                : "This is a notification message",
            Style       = style,
            IsLoading   = isLoading,
            HostMinWidth = 300,
            Content     = "Some descriptions"
        };
    }

    private static DialogButtonBox CreateDialogButtonBox(DialogStandardButtons standardButtons)
    {
        return new DialogButtonBox
        {
            StandardButtons       = standardButtons,
            DefaultStandardButton = DialogStandardButton.Ok
        };
    }

    private static DialogButtonBox CreateCustomDialogButtonBox()
    {
        var buttonBox = CreateDialogButtonBox(DialogStandardButton.Ok | DialogStandardButton.Cancel);
        buttonBox.CustomButtons.Add(new DialogButton
        {
            Content = "Custom",
            Role    = DialogButtonRole.ActionRole
        });
        return buttonBox;
    }

    private static string CreateDialogContent()
    {
        return "Some contents...";
    }
}
