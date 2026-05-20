using System.Collections;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunMessageStateVerification()
    {
        var failures = new List<string>();
        VerifyMessageCardTypeState(failures);
        VerifyMessageCardCustomIcon(failures);
        VerifyMessageManagerPreTemplateShow(failures);
        VerifyMessageManagerTimerLifecycle(failures);
        VerifyMessageManagerDisposeWithoutTopLevel(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Message state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Message state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyMessageCardTypeState(ICollection<string> failures)
    {
        var card = CreateMessageCard(MessageType.Information, isMotionEnabled: false);
        using var realized = RealizeControl(card);

        Expect(HasPseudoClass(card, MessageCardPseudoClass.Information),
            "Information MessageCard should set :information.",
            failures);
        Expect(!HasPseudoClass(card, MessageCardPseudoClass.Error),
            "Information MessageCard should not set :error.",
            failures);

        card.MessageType = MessageType.Error;
        RefreshLayout(realized.Window);
        Expect(!HasPseudoClass(card, MessageCardPseudoClass.Information),
            "MessageCard should clear :information when MessageType changes.",
            failures);
        Expect(HasPseudoClass(card, MessageCardPseudoClass.Error),
            "MessageCard should set :error when MessageType changes to Error.",
            failures);
    }

    private static void VerifyMessageCardCustomIcon(ICollection<string> failures)
    {
        var customIcon = new GithubOutlined();
        var card = new MessageCard
        {
            Icon            = customIcon,
            Message         = "Custom icon",
            MessageType     = MessageType.Information,
            IsMotionEnabled = false
        };
        using var realized = RealizeControl(card);

        Expect(ReferenceEquals(customIcon, card.Icon),
            "MessageCard should keep the caller-provided custom icon.",
            failures);

        card.MessageType = MessageType.Error;
        RefreshLayout(realized.Window);
        Expect(ReferenceEquals(customIcon, card.Icon),
            "MessageCard should not replace a custom icon when MessageType changes.",
            failures);
    }

    private static void VerifyMessageManagerTimerLifecycle(ICollection<string> failures)
    {
        var manager = CreateWindowMessageManager();
        manager.IsMotionEnabled = false;
        using var realized = RealizeControl(manager);

        manager.Show(new Message(
            type: MessageType.Information,
            content: "Long running message",
            expiration: TimeSpan.FromSeconds(30)));
        RefreshLayout(realized.Window);

        Expect(GetMessageTimerCount(manager) == 1,
            "WindowMessageManager should track the auto-close timer for an active message.",
            failures);

        var card = manager.GetSelfAndVisualDescendants().OfType<MessageCard>().FirstOrDefault();
        card?.Close();
        RefreshLayout(realized.Window);
        Expect(GetMessageTimerCount(manager) == 0,
            "WindowMessageManager should dispose the auto-close timer when the message closes.",
            failures);
    }

    private static void VerifyMessageManagerPreTemplateShow(ICollection<string> failures)
    {
        var manager = CreateWindowMessageManager();
        manager.IsMotionEnabled = false;

        manager.Show(new Message(
            type: MessageType.Success,
            content: "Immediate message",
            expiration: TimeSpan.Zero));

        using var realized = RealizeControl(manager);
        RefreshLayout(realized.Window);

        Expect(manager.GetSelfAndVisualDescendants().OfType<MessageCard>().Count() == 1,
            "WindowMessageManager should preserve a message shown before its template is applied.",
            failures);
    }

    private static void VerifyMessageManagerDisposeWithoutTopLevel(ICollection<string> failures)
    {
        var pendingManager = CreateWindowMessageManager();
        pendingManager.Show(new Message(
            type: MessageType.Information,
            content: "Pending message",
            expiration: TimeSpan.Zero));
        Expect(GetPendingMessageCount(pendingManager) == 1,
            "WindowMessageManager should keep pre-template messages pending until template application.",
            failures);
        pendingManager.Dispose();
        Expect(GetPendingMessageCount(pendingManager) == 0,
            "WindowMessageManager.Dispose should clear pre-template pending messages.",
            failures);

        var manager = CreateWindowMessageManager();
        manager.IsMotionEnabled = false;
        using var realized = RealizeControl(manager);

        manager.Show(new Message(
            type: MessageType.Success,
            content: "Dispose message",
            expiration: TimeSpan.FromSeconds(30)));
        RefreshLayout(realized.Window);
        Expect(GetMessageTimerCount(manager) == 1,
            "WindowMessageManager should have a tracked timer before dispose.",
            failures);

        manager.Dispose();
        Expect(GetMessageTimerCount(manager) == 0,
            "WindowMessageManager.Dispose should clear timers even when it was not installed into a TopLevel.",
            failures);
    }

    private static int GetMessageTimerCount(WindowMessageManager manager)
    {
        return GetPrivateField(manager, "AtomUI.Desktop.Controls.WindowMessageManager", "_messageCloseTimers") is IDictionary timers
            ? timers.Count
            : 0;
    }

    private static int GetPendingMessageCount(WindowMessageManager manager)
    {
        return GetPrivateField(manager, "AtomUI.Desktop.Controls.WindowMessageManager", "_pendingMessages") is ICollection pendingMessages
            ? pendingMessages.Count
            : 0;
    }
}
