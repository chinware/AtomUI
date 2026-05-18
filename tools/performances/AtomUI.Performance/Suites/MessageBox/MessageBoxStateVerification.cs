using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunMessageBoxStateVerification()
    {
        var failures = new List<string>();
        VerifyMessageBoxStyleIconLifecycle(failures);
        VerifyMessageBoxOpenCloseReleaseLifecycle(failures);
        VerifyDialogLoadingContentLifecycle(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("MessageBox state verification passed.");
            return true;
        }

        Console.Error.WriteLine("MessageBox state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyMessageBoxStyleIconLifecycle(ICollection<string> failures)
    {
        var button = new AtomUI.Desktop.Controls.Button
        {
            Content = "Open"
        };
        var messageBox = CreatePerfMessageBox(MessageBoxStyle.Information);
        messageBox.PlacementTarget = button;
        messageBox.IsModal         = false;
        messageBox.IsMotionEnabled = false;

        var root = new StackPanel
        {
            Children =
            {
                button,
                messageBox
            }
        };

        var window = new AtomUI.Desktop.Controls.Window
        {
            Width         = 800,
            Height        = 600,
            Content       = root,
            ShowInTaskbar = false
        };

        window.Show();
        RefreshLayout(window);
        messageBox.OpenAsync().GetAwaiter().GetResult();
        RefreshLayout(window);

        Expect(messageBox.Icon?.GetType().Name == "InfoCircleFilled",
            "Information MessageBox should create the information icon on first open.",
            failures);

        messageBox.Style = MessageBoxStyle.Normal;
        RefreshLayout(window);
        Expect(messageBox.Icon == null,
            "Normal MessageBox should clear the style-provided icon.",
            failures);

        messageBox.Style = MessageBoxStyle.Warning;
        RefreshLayout(window);
        Expect(messageBox.Icon?.GetType().Name == "ExclamationCircleFilled",
            "Warning MessageBox should create the warning icon after switching from Normal.",
            failures);

        var customIcon = new CloseCircleFilled();
        messageBox.Icon  = customIcon;
        messageBox.Style = MessageBoxStyle.Success;
        RefreshLayout(window);
        Expect(ReferenceEquals(customIcon, messageBox.Icon),
            "A local custom MessageBox.Icon should not be overwritten by style icon updates.",
            failures);

        messageBox.Cancel();
        RefreshLayout(window);
        window.Close();
    }

    private static void VerifyMessageBoxOpenCloseReleaseLifecycle(ICollection<string> failures)
    {
        var button = new AtomUI.Desktop.Controls.Button
        {
            Content = "Open"
        };
        var messageBox = CreatePerfMessageBox(MessageBoxStyle.Confirm);
        messageBox.PlacementTarget = button;
        messageBox.IsModal         = false;
        messageBox.IsMotionEnabled = false;

        var root = new StackPanel
        {
            Children =
            {
                button,
                messageBox
            }
        };

        var window = new AtomUI.Desktop.Controls.Window
        {
            Width         = 800,
            Height        = 600,
            Content       = root,
            ShowInTaskbar = false
        };

        window.Show();
        RefreshLayout(window);
        Expect(GetPrivateField(messageBox, "AtomUI.Desktop.Controls.MessageBox", "_dialog") == null,
            "Closed MessageBox should not materialize Dialog before open.",
            failures);

        messageBox.OpenAsync().GetAwaiter().GetResult();
        RefreshLayout(window);
        Expect(GetPrivateField(messageBox, "AtomUI.Desktop.Controls.MessageBox", "_dialog") != null,
            "MessageBox should materialize Dialog on first open.",
            failures);

        messageBox.Cancel();
        RefreshLayout(window);
        Expect(!messageBox.IsOpen,
            "MessageBox.Cancel should close and reset IsOpen.",
            failures);

        window.Close();
        RefreshLayout(window);
        Expect(GetPrivateField(messageBox, "AtomUI.Desktop.Controls.MessageBox", "_dialog") == null,
            "Detached MessageBox should release the lazy Dialog.",
            failures);
    }

    private static void VerifyDialogLoadingContentLifecycle(ICollection<string> failures)
    {
        var button = new AtomUI.Desktop.Controls.Button
        {
            Content = "Open"
        };
        var dialog = CreateBasicDialog();
        dialog.PlacementTarget  = button;
        dialog.IsModal          = false;
        dialog.IsMotionEnabled  = false;
        dialog.DialogHostType   = DialogHostType.Overlay;
        dialog.IsLoading        = false;

        var root = new StackPanel
        {
            Children =
            {
                button,
                dialog
            }
        };

        var window = new AtomUI.Desktop.Controls.Window
        {
            Width         = 800,
            Height        = 600,
            Content       = root,
            ShowInTaskbar = false
        };

        window.Show();
        RefreshLayout(window);
        dialog.OpenAsync().GetAwaiter().GetResult();
        RefreshLayout(window);

        var host = dialog.Host as Control;
        Expect(host != null,
            "Opening overlay Dialog should create a visual host.",
            failures);
        if (host != null)
        {
            Expect(CountVisualsByTypeName(host, "Skeleton") == 0,
                "Non-loading Dialog content should not create Skeleton visuals.",
                failures);

            dialog.IsLoading = true;
            RefreshLayout(window);
            Expect(CountVisualsByTypeName(host, "Skeleton") == 1,
                "Loading Dialog content should create Skeleton visuals on demand.",
                failures);

            dialog.IsLoading = false;
            RefreshLayout(window);
            Expect(CountVisualsByTypeName(host, "Skeleton") == 0,
                "Disabling Dialog loading should release Skeleton visuals.",
                failures);

            dialog.IsLoading = true;
            RefreshLayout(window);
            Expect(CountVisualsByTypeName(host, "Skeleton") == 1,
                "Re-enabling Dialog loading should recreate Skeleton visuals.",
                failures);
        }

        dialog.Reject();
        RefreshLayout(window);
        window.Close();
        if (host != null)
        {
            Expect(CountVisualsByTypeName(host, "Skeleton") == 0,
                "Detached Dialog loading presenter should release Skeleton visuals.",
                failures);
        }
    }
}
