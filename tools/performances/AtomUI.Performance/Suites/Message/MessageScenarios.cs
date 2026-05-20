using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateMessageScenarios()
    {
        return
        [
            new PerfScenario("MessageCard.Information.NoMotion", _ => CreateMessageCard(MessageType.Information, isMotionEnabled: false)),
            new PerfScenario("MessageCard.Success.NoMotion", _ => CreateMessageCard(MessageType.Success, isMotionEnabled: false)),
            new PerfScenario("MessageCard.Warning.NoMotion", _ => CreateMessageCard(MessageType.Warning, isMotionEnabled: false)),
            new PerfScenario("MessageCard.Error.NoMotion", _ => CreateMessageCard(MessageType.Error, isMotionEnabled: false)),
            new PerfScenario("MessageCard.Loading.NoMotion", _ => CreateMessageCard(MessageType.Loading, isMotionEnabled: false)),
            new PerfScenario("WindowMessageManager.Empty.Closed", _ => CreateWindowMessageManager()),
            new PerfScenario("WindowMessageManager.Show.Single.NoMotion", _ => new MessageManagerShowHost(1)),
            new PerfScenario("WindowMessageManager.Show.MaxItems.NoMotion", _ => new MessageManagerShowHost(12))
        ];
    }

    private static MessageCard CreateMessageCard(MessageType type, bool isMotionEnabled)
    {
        return new MessageCard
        {
            Message         = "Hello, AtomUI/Avalonia!",
            MessageType     = type,
            IsMotionEnabled = isMotionEnabled
        };
    }

    private static WindowMessageManager CreateWindowMessageManager()
    {
        return new WindowMessageManager(null)
        {
            MaxItems = 10
        };
    }

    private sealed class MessageManagerShowHost : Border
    {
        private readonly int _messageCount;
        private WindowMessageManager? _messageManager;
        private bool _isShown;

        public MessageManagerShowHost(int messageCount)
        {
            _messageCount = messageCount;
            _messageManager = new WindowMessageManager(null)
            {
                IsMotionEnabled = false,
                MaxItems = 5
            };
            Child = _messageManager;
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            Dispatcher.UIThread.Post(ShowMessages, DispatcherPriority.Loaded);
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
            _messageManager?.Dispose();
            _messageManager = null;
        }

        private void ShowMessages()
        {
            if (_isShown || _messageManager is null)
            {
                return;
            }

            _isShown = true;
            for (var i = 0; i < _messageCount; i++)
            {
                _messageManager.Show(new Message(
                    type: i % 2 == 0 ? MessageType.Information : MessageType.Success,
                    content: $"Message {i}",
                    expiration: TimeSpan.Zero));
            }
        }
    }
}
