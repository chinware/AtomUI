using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Threading;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateMessageBoxScenarios()
    {
        return
        [
            new PerfScenario("MessageBox.Closed.Information", _ => CreatePerfMessageBox(MessageBoxStyle.Information)),
            new PerfScenario("MessageBox.Closed.Confirm", _ => CreatePerfMessageBox(MessageBoxStyle.Confirm)),
            new PerfScenario("MessageBox.Closed.Normal", _ => CreatePerfMessageBox(MessageBoxStyle.Normal)),
            new PerfScenario("MessageBox.OpenWindow.Information.NoMotion", _ => CreateOpenMessageBoxHost(MessageBoxStyle.Information, DialogHostType.Window)),
            new PerfScenario("MessageBox.OpenWindow.Confirm.NoMotion", _ => CreateOpenMessageBoxHost(MessageBoxStyle.Confirm, DialogHostType.Window)),
            new PerfScenario("MessageBox.OpenWindow.Loading.NoMotion", _ => CreateOpenMessageBoxHost(MessageBoxStyle.Information, DialogHostType.Window, isLoading: true))
        ];
    }

    private static MessageBox CreatePerfMessageBox(MessageBoxStyle style, bool isLoading = false)
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

    private static Control CreateOpenMessageBoxHost(MessageBoxStyle style,
                                                    DialogHostType hostType,
                                                    bool isLoading = false)
    {
        var button = new AtomUI.Desktop.Controls.Button
        {
            Content = "Open"
        };
        var messageBox = CreatePerfMessageBox(style, isLoading);
        messageBox.PlacementTarget  = button;
        messageBox.DialogHostType   = hostType;
        messageBox.IsModal          = false;
        messageBox.IsMotionEnabled  = false;
        messageBox.IsDragMovable    = false;
        messageBox.HostMinWidth     = 300;

        var root = new StackPanel
        {
            Children =
            {
                button,
                messageBox
            }
        };

        var opened = false;
        root.AttachedToVisualTree += (_, _) =>
        {
            if (opened)
            {
                return;
            }

            opened = true;
            Dispatcher.UIThread.Post(() => messageBox.OpenAsync().GetAwaiter().GetResult());
        };

        return root;
    }
}
