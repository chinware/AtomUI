using AtomUI.Desktop.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using System.Linq;

namespace AtomUIGallery.ShowCases.Message;

public partial class MessageShowCase : GalleryReactiveUserControl<MessageViewModel>
{
    public const string LanguageId = nameof(MessageShowCase);

    private WindowMessageManager? _messageManager;

    public MessageShowCase()
    {
        InitializeComponent();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _messageManager?.Dispose();
        _messageManager = null;
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

    }

    private void ShowSimpleMessage(object? sender, RoutedEventArgs e)
    {
        ShowMessage(new AtomUIMessage(
            Lang(MessageShowCaseLangResourceKind.P2MessageHelloAtomUIAvalonia, "Hello, AtomUI/Avalonia!")
        ));
    }

    private void ShowInfoMessage(object? sender, RoutedEventArgs e)
    {
        ShowMessage(new AtomUIMessage(
            type: MessageType.Information,
            content: Lang(MessageShowCaseLangResourceKind.P2MessageInformation, "This is an information message.")
        ));
    }

    private void ShowSuccessMessage(object? sender, RoutedEventArgs e)
    {
        ShowMessage(new AtomUIMessage(
            type: MessageType.Success,
            content: Lang(MessageShowCaseLangResourceKind.P2MessageSuccess, "This is a success message.")
        ));
    }

    private void ShowWarningMessage(object? sender, RoutedEventArgs e)
    {
        ShowMessage(new AtomUIMessage(
            type: MessageType.Warning,
            content: Lang(MessageShowCaseLangResourceKind.P2MessageWarning, "This is a warning message.")
        ));
    }

    private void ShowErrorMessage(object? sender, RoutedEventArgs e)
    {
        ShowMessage(new AtomUIMessage(
            type: MessageType.Error,
            content: Lang(MessageShowCaseLangResourceKind.P2MessageError, "This is an error message.")
        ));
    }

    private void ShowLoadingMessage(object? sender, RoutedEventArgs e)
    {
        ShowMessage(new AtomUIMessage(
            type: MessageType.Loading,
            content: Lang(MessageShowCaseLangResourceKind.P2MessageActionInProgress, "Action in progress...")
        ));
    }

    private void ShowSequentialMessage(object? sender, RoutedEventArgs e)
    {
        ShowMessage(new AtomUIMessage(
            type: MessageType.Loading,
            content: Lang(MessageShowCaseLangResourceKind.P2MessageActionInProgress, "Action in progress..."),
            expiration: TimeSpan.FromSeconds(2.5),
            onClose: () =>
            {
                ShowMessage(new AtomUIMessage(
                    type: MessageType.Success,
                    expiration: TimeSpan.FromSeconds(2.5),
                    content: Lang(MessageShowCaseLangResourceKind.P2MessageLoadingFinished, "Loading finished"),
                    onClose: () =>
                    {
                        ShowMessage(new AtomUIMessage(
                            type: MessageType.Information,
                            expiration: TimeSpan.FromSeconds(2.5),
                            content: Lang(MessageShowCaseLangResourceKind.P2MessageLoadingFinished, "Loading finished")
                        ));
                    }
                ));
            }
        ));
    }

    private void ShowMessage(AtomUIMessage message)
    {
        GetMessageManager()?.Show(message);
    }

    // 对齐上游 components/message/demo/style-class.tsx：Object style 以对象式样式弹出成功消息（绿色卡片），
    // Function style 以函数式样式弹出错误消息——上游 stylesFn 在 type === 'error' 时整卡转红，AtomUI 由
    // 卡片上的 :error 伪类分支表达。样式随消息传入（上游 styles 的等价物），不做代码侧部件属性改写。
    private void HandleShowObjectStyleMessage(object? sender, RoutedEventArgs e)
    {
        ShowStyledMessage(
            MessageType.Success,
            MessageShowCaseLangResourceKind.P2MessageObjectStyles,
            "This is a message with object styles",
            "semantic-object-style-demo");
    }

    private void HandleShowFunctionStyleMessage(object? sender, RoutedEventArgs e)
    {
        ShowStyledMessage(
            MessageType.Error,
            MessageShowCaseLangResourceKind.P2MessageFunctionStyles,
            "This is a message with function styles",
            "semantic-function-style-demo");
    }

    private void ShowStyledMessage(
        MessageType type,
        MessageShowCaseLangResourceKind resourceKind,
        string fallback,
        string styleClass)
    {
        // 与上游一致：demo 走 messageApi.open 的默认 duration = 3 秒，到期自动消失。
        // 注意保持有限时长：常驻消息在连点时会无限堆积。
        GetMessageManager()?.Show(
            new AtomUIMessage(
                type: type,
                content: Lang(resourceKind, fallback),
                expiration: TimeSpan.FromSeconds(3)),
            [styleClass]);
    }

    private static string Lang(MessageShowCaseLangResourceKind resourceKind, string fallback)
    {
        return GalleryLocalization.Get(resourceKind, fallback);
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

        // 窗口反馈层（WindowFeedbackLayer）在页面视觉树之外，页面内声明的 Style 选不中它弹出的卡片。
        // 语义样式在页面 AXAML 中以 Styles 资源声明（仍是生成的专用 Style 类），这里挂到 manager 自身
        // ——卡片是 manager 的视觉后代，按 owner 作用域命中。挂载是唯一的代码步骤，不做部件属性改写。
        if (Resources.TryGetResource("MessageSemanticStyleStyles", null, out var styles) &&
            styles is Styles semanticStyles)
        {
            _messageManager.Styles.Add(semanticStyles);
        }

        return _messageManager;
    }

    // manager 只通过 Show(IMessage) 接收消息，没有声明式 items 入口；语义预览舞台在首次加载时
    // 注入一条常驻消息（expiration = Zero），让 listContent 与卡片在预览期间一直可解析、可高亮。
    // 这是示例数据播种，不是用代码改写语义部件属性（部件定制一律走生成的专用 Style）。
    // Loaded 在每次重新挂载时都会触发，用实例引用去重，避免同一 manager 播种多条消息。
    private WindowMessageManager? _seededSemanticPreviewManager;

    private void HandleSemanticPreviewOwnerLoaded(object? sender, RoutedEventArgs e)
    {
        if (sender is not WindowMessageManager manager ||
            ReferenceEquals(_seededSemanticPreviewManager, manager))
        {
            return;
        }

        _seededSemanticPreviewManager = manager;
        manager.Show(new AtomUIMessage(
            type: MessageType.Information,
            content: Lang(MessageShowCaseLangResourceKind.P2MessageInformation, "This is an information message."),
            expiration: TimeSpan.Zero));
    }
}
