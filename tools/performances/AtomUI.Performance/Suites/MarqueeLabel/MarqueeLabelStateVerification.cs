using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunMarqueeLabelStateVerification()
    {
        var failures = new List<string>();

        VerifyAlertMarqueeLazyLifecycle(failures);
        VerifyAlertMarqueeDetachCleanup(failures);
        VerifyAlertMarqueeDescriptionStyle(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("MarqueeLabel state verification passed.");
            return true;
        }

        Console.Error.WriteLine("MarqueeLabel state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyAlertMarqueeLazyLifecycle(ICollection<string> failures)
    {
        var alert = CreateAlert(message: "Initial message");
        using var realized = RealizeControl(alert);

        Expect(FindVisualByName<MarqueeLabel>(alert, "MarqueeLabel") == null,
            "Alert default should not create MarqueeLabel.", failures);
        Expect(FindVisualByName<Label>(alert, "MessageLabel")?.IsVisible == true,
            "Alert default should keep MessageLabel visible.", failures);

        alert.SetCurrentValue(Alert.IsMessageMarqueeEnabledProperty, true);
        RefreshLayout(realized.Window);
        var marqueeLabel = FindVisualByName<MarqueeLabel>(alert, "MarqueeLabel");
        Expect(marqueeLabel != null,
            "Alert should create MarqueeLabel when marquee is enabled.", failures);
        Expect(marqueeLabel?.Text == "Initial message",
            $"Created MarqueeLabel should bind Message, actual '{marqueeLabel?.Text}'.", failures);
        Expect(FindVisualByName<Label>(alert, "MessageLabel")?.IsVisible == false,
            "Alert should hide MessageLabel while marquee is enabled.", failures);

        alert.SetCurrentValue(Alert.MessageProperty, "Updated message");
        RefreshLayout(realized.Window);
        Expect(marqueeLabel?.Text == "Updated message",
            $"MarqueeLabel should update when Alert.Message changes, actual '{marqueeLabel?.Text}'.", failures);

        alert.SetCurrentValue(Alert.IsMessageMarqueeEnabledProperty, false);
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<MarqueeLabel>(alert, "MarqueeLabel") == null,
            "Alert should remove MarqueeLabel when marquee is disabled.", failures);
        Expect(marqueeLabel?.GetVisualParent() == null,
            "Removed MarqueeLabel should not keep a visual parent.", failures);
        Expect(string.IsNullOrEmpty(marqueeLabel?.Text),
            $"Removed MarqueeLabel should clear its Message binding, actual '{marqueeLabel?.Text}'.", failures);
        Expect(FindVisualByName<Label>(alert, "MessageLabel")?.IsVisible == true,
            "Alert should restore MessageLabel visibility after marquee is disabled.", failures);

        alert.SetCurrentValue(Alert.MessageProperty, "Message after removal");
        RefreshLayout(realized.Window);
        Expect(marqueeLabel?.Text != "Message after removal",
            "Removed MarqueeLabel should not keep receiving Alert.Message updates.", failures);

        alert.SetCurrentValue(Alert.IsMessageMarqueeEnabledProperty, true);
        RefreshLayout(realized.Window);
        var recreated = FindVisualByName<MarqueeLabel>(alert, "MarqueeLabel");
        Expect(recreated != null && !ReferenceEquals(recreated, marqueeLabel),
            "Re-enabling marquee should create a new MarqueeLabel after cleanup.", failures);
        Expect(recreated?.Text == "Message after removal",
            $"Recreated MarqueeLabel should bind the latest message, actual '{recreated?.Text}'.", failures);
    }

    private static void VerifyAlertMarqueeDetachCleanup(ICollection<string> failures)
    {
        var alert = CreateAlert(
            message: "Detach message",
            isMessageMarqueeEnabled: true);
        MarqueeLabel? marqueeLabel;

        using (RealizeControl(alert))
        {
            marqueeLabel = FindVisualByName<MarqueeLabel>(alert, "MarqueeLabel");
            Expect(marqueeLabel != null,
                "Alert with marquee enabled should create MarqueeLabel before detach.", failures);
        }

        Expect(marqueeLabel?.GetVisualParent() == null,
            "Detached Alert should remove the created MarqueeLabel from the visual tree.", failures);
        Expect(string.IsNullOrEmpty(marqueeLabel?.Text),
            $"Detached Alert should clear the created MarqueeLabel binding, actual '{marqueeLabel?.Text}'.", failures);
    }

    private static void VerifyAlertMarqueeDescriptionStyle(ICollection<string> failures)
    {
        var alert = CreateAlert(
            message: "Styled message",
            description: "Description",
            isMessageMarqueeEnabled: true);
        using var realized = RealizeControl(alert);

        var messageLabel = FindVisualByName<Label>(alert, "MessageLabel");
        var marqueeLabel = FindVisualByName<MarqueeLabel>(alert, "MarqueeLabel");

        Expect(messageLabel != null, "Alert should materialize MessageLabel.", failures);
        Expect(marqueeLabel != null, "Alert with marquee enabled should materialize MarqueeLabel.", failures);
        if (messageLabel is null || marqueeLabel is null)
        {
            return;
        }

        Expect(Math.Abs(messageLabel.FontSize - marqueeLabel.FontSize) < 0.001,
            $"MarqueeLabel should receive the same description font size as MessageLabel, message={messageLabel.FontSize}, marquee={marqueeLabel.FontSize}.",
            failures);
        Expect(messageLabel.VerticalAlignment == marqueeLabel.VerticalAlignment,
            $"MarqueeLabel should receive the same description vertical alignment as MessageLabel, message={messageLabel.VerticalAlignment}, marquee={marqueeLabel.VerticalAlignment}.",
            failures);

        alert.SetCurrentValue(Alert.DescriptionProperty, null);
        RefreshLayout(realized.Window);
        Expect(Math.Abs(messageLabel.FontSize - marqueeLabel.FontSize) < 0.001,
            $"MarqueeLabel should keep style parity after Description is cleared, message={messageLabel.FontSize}, marquee={marqueeLabel.FontSize}.",
            failures);
        Expect(messageLabel.VerticalAlignment == marqueeLabel.VerticalAlignment,
            $"MarqueeLabel should keep vertical alignment parity after Description is cleared, message={messageLabel.VerticalAlignment}, marquee={marqueeLabel.VerticalAlignment}.",
            failures);
    }
}
