using AtomUI.Desktop.Controls;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunDialogStateVerification()
    {
        var failures = new List<string>();
        VerifyMessageBoxLazyDialogLifecycle(failures);
        VerifyMessageBoxOpenCloseLifecycle(failures);
        VerifyDialogOpenCloseLifecycle(failures);
        VerifyDialogButtonBoxLifecycle(failures);
        VerifyOverlayHostLazyPartsLifecycle(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Dialog state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Dialog state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyMessageBoxLazyDialogLifecycle(ICollection<string> failures)
    {
        var messageBox = CreateMessageBox(AtomUI.Desktop.Controls.MessageBoxStyle.Information);
        using var realized = RealizeControl(messageBox);

        Expect(GetPrivateField(messageBox, "AtomUI.Desktop.Controls.MessageBox", "_dialog") == null,
            "Closed MessageBox should not create an internal Dialog before the first open.",
            failures);
        Expect(CountVisualsByTypeName(messageBox, "Dialog") == 0,
            "Closed MessageBox should not keep an internal Dialog visual in its template.",
            failures);

        messageBox.ApplyTemplate();
        RefreshLayout(realized.Window);
        Expect(GetPrivateField(messageBox, "AtomUI.Desktop.Controls.MessageBox", "_dialog") == null,
            "Re-applying the closed MessageBox template should not materialize Dialog.",
            failures);
    }

    private static void VerifyMessageBoxOpenCloseLifecycle(ICollection<string> failures)
    {
        var button = new AtomUI.Desktop.Controls.Button
        {
            Content = "Open"
        };
        var messageBox = CreateMessageBox(MessageBoxStyle.Confirm);
        messageBox.PlacementTarget = button;
        messageBox.IsModal         = false;

        var root = new Avalonia.Controls.StackPanel
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
        Expect(GetPrivateField(messageBox, "AtomUI.Desktop.Controls.MessageBox", "_dialog") != null,
            "Opening MessageBox should lazily create the internal Dialog.",
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

    private static void VerifyDialogOpenCloseLifecycle(ICollection<string> failures)
    {
        var button = new AtomUI.Desktop.Controls.Button
        {
            Content = "Open"
        };
        var dialog = CreateBasicDialog();
        dialog.PlacementTarget = button;
        dialog.IsModal       = false;
        dialog.DialogHostType = DialogHostType.Window;

        var root = new Avalonia.Controls.StackPanel
        {
            Children =
            {
                button,
                dialog
            }
        };

        using var realized = RealizeControl(root);
        dialog.OpenAsync().GetAwaiter().GetResult();
        RefreshLayout(realized.Window);
        Expect(dialog.Host != null,
            "Dialog.OpenAsync should create a host.",
            failures);

        dialog.Reject();
        RefreshLayout(realized.Window);
        Expect(dialog.Host == null && !dialog.IsOpen,
            "Dialog.Reject should close the host and reset IsOpen.",
            failures);
    }

    private static void VerifyDialogButtonBoxLifecycle(ICollection<string> failures)
    {
        var buttonBox = CreateDialogButtonBox(DialogStandardButton.Ok | DialogStandardButton.Cancel);
        using var realized = RealizeControl(buttonBox);

        Expect(CountVisualsByTypeName(buttonBox, "DialogButton") == 2,
            "DialogButtonBox should create exactly the requested standard buttons.",
            failures);

        buttonBox.StandardButtons = DialogStandardButton.NoButton;
        RefreshLayout(realized.Window);
        Expect(CountVisualsByTypeName(buttonBox, "DialogButton") == 0,
            "DialogButtonBox should remove standard buttons when StandardButtons becomes NoButton.",
            failures);

        buttonBox.StandardButtons = DialogStandardButton.Ok | DialogStandardButton.Cancel;
        RefreshLayout(realized.Window);
        Expect(CountVisualsByTypeName(buttonBox, "DialogButton") == 2,
            "DialogButtonBox should recreate standard buttons without duplicates.",
            failures);

        var customButton = new DialogButton
        {
            Content = "Custom",
            Role    = DialogButtonRole.ActionRole
        };
        buttonBox.CustomButtons.Add(customButton);
        RefreshLayout(realized.Window);
        Expect(CountVisualsByTypeName(buttonBox, "DialogButton") == 3,
            "DialogButtonBox should add custom buttons through indexed collection changed sync.",
            failures);

        buttonBox.CustomButtons.Remove(customButton);
        RefreshLayout(realized.Window);
        Expect(CountVisualsByTypeName(buttonBox, "DialogButton") == 2,
            "DialogButtonBox should remove custom buttons through indexed collection changed sync.",
            failures);
    }

    private static void VerifyOverlayHostLazyPartsLifecycle(ICollection<string> failures)
    {
        var button = new AtomUI.Desktop.Controls.Button
        {
            Content = "Open"
        };
        var dialog = CreateBasicDialog();
        dialog.PlacementTarget = button;
        dialog.IsModal         = true;
        dialog.IsResizable     = false;

        var root = new Avalonia.Controls.StackPanel
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
        var openTask = dialog.OpenAsync();
        RefreshLayout(window);

        var host = dialog.Host as Avalonia.Controls.Control;
        Expect(host != null,
            "Modal Dialog.OpenAsync should create an overlay host.",
            failures);
        if (host != null)
        {
            Expect(CountVisualsByTypeName(host, "OverlayDialogResizer") == 0,
                "Non-resizable overlay host should not create OverlayDialogResizer.",
                failures);
            Expect(GetPrivateField(host, "AtomUI.Desktop.Controls.OverlayDialogHost", "_dialogMask") != null,
                "Modal overlay host should create mask on open.",
                failures);

            dialog.IsResizable = true;
            RefreshLayout(window);
            Expect(CountVisualsByTypeName(host, "OverlayDialogResizer") == 1,
                "Resizable overlay host should create OverlayDialogResizer on demand.",
                failures);

            dialog.IsResizable = false;
            RefreshLayout(window);
            Expect(CountVisualsByTypeName(host, "OverlayDialogResizer") == 0,
                "Disabling IsResizable should remove OverlayDialogResizer.",
                failures);
        }

        dialog.Reject();
        RefreshLayout(window);
        openTask.GetAwaiter().GetResult();
        window.Close();

        if (host != null)
        {
            Expect(GetPrivateField(host, "AtomUI.Desktop.Controls.OverlayDialogHost", "_dialogMask") == null,
                "Closed overlay host should release mask field.",
                failures);
            Expect(GetPrivateField(host, "AtomUI.Desktop.Controls.OverlayDialogHost", "_dialogMaskCornerRadiusBinding") == null,
                "Closed overlay host should dispose mask corner-radius binding.",
                failures);
        }
    }
}
