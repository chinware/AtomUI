using System.Diagnostics;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using AvaloniaButton = Avalonia.Controls.Button;

namespace AtomUIGallery.ShowCases.Icon;

public partial class IconShowCase : GalleryReactiveUserControl<IconViewModel>
{
    public const string LanguageId = nameof(IconShowCase);
    private const string OutlinedScenario = "Outlined";
    private const string FilledScenario   = "Filled";
    private const string TwoToneScenario  = "TwoTone";

    private readonly GalleryShowCaseScenarioController _scenarioController;
    private SemaphoreSlim? _copySemaphore;
    private CancellationTokenSource? _copyWaitCancellation;
    private WindowMessageManager? _messageManager;
    private int _copyRequestSequence;

    public IconShowCase()
    {
        InitializeComponent();
        _scenarioController = new GalleryShowCaseScenarioController(ScenarioTabs, ScenarioContentHost, CreateScenarioContent);
        AddHandler(AvaloniaButton.ClickEvent, HandleIconItemClick);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _copySemaphore        = new SemaphoreSlim(1, 1);
        _copyWaitCancellation = new CancellationTokenSource();
        _scenarioController.Attach(DataContext);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _copyRequestSequence++;
        _copyWaitCancellation?.Cancel();
        _copyWaitCancellation?.Dispose();
        _copyWaitCancellation = null;
        _copySemaphore        = null;
        _scenarioController.Detach();
        _messageManager?.Dispose();
        _messageManager = null;
        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        _scenarioController.UpdateDataContext(DataContext);
    }

    private static Control CreateScenarioContent(string scenario)
    {
        return scenario switch
        {
            OutlinedScenario => new IconGallery()
            {
                IconThemeType = IconThemeType.Outlined,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch
            },
            FilledScenario => new IconGallery()
            {
                IconThemeType = IconThemeType.Filled,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch
            },
            TwoToneScenario => new IconGallery()
            {
                IconThemeType = IconThemeType.TwoTone,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch
            },
            _ => throw new InvalidOperationException($"Unknown Icon scenario: {scenario}")
        };
    }

    private async void HandleIconItemClick(object? sender, RoutedEventArgs e)
    {
        if (e.Source is not IconInfoItem item || string.IsNullOrWhiteSpace(item.IconName))
        {
            return;
        }

        e.Handled = true;
        var iconName    = item.IconName;
        var copyRequest = ++_copyRequestSequence;
        var topLevel    = TopLevel.GetTopLevel(this);
        if (topLevel is null || topLevel.Clipboard is not { } clipboard)
        {
            ShowCopyFailure();
            return;
        }

        var copySemaphore        = _copySemaphore;
        var copyWaitCancellation = _copyWaitCancellation;
        if (copySemaphore is null || copyWaitCancellation is null)
        {
            return;
        }

        try
        {
            await copySemaphore.WaitAsync(copyWaitCancellation.Token);
        }
        catch (OperationCanceledException) when (copyWaitCancellation.IsCancellationRequested)
        {
            return;
        }

        try
        {
            if (!IsCurrentCopyRequest(copyRequest, topLevel))
            {
                return;
            }

            await clipboard.SetTextAsync(iconName);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to copy icon name: {ex.Message}");
            if (IsCurrentCopyRequest(copyRequest, topLevel))
            {
                ShowCopyFailure();
            }

            return;
        }
        finally
        {
            copySemaphore.Release();
        }

        if (IsCurrentCopyRequest(copyRequest, topLevel))
        {
            GetMessageManager()?.Show(new AtomUIMessage(
                content: GalleryLocalization.Format(
                    IconShowCaseLangResourceKind.IconCopySucceededFormat,
                    "Icon name copied: {0}",
                    iconName),
                type: MessageType.Success,
                expiration: TimeSpan.FromSeconds(2)));
        }
    }

    private bool IsCurrentCopyRequest(int copyRequest, TopLevel topLevel)
    {
        return copyRequest == _copyRequestSequence &&
               ReferenceEquals(TopLevel.GetTopLevel(this), topLevel);
    }

    private void ShowCopyFailure()
    {
        if (!this.IsAttachedToVisualTree())
        {
            return;
        }

        GetMessageManager()?.Show(new AtomUIMessage(
            content: GalleryLocalization.Get(
                IconShowCaseLangResourceKind.IconCopyFailed,
                "Could not copy the icon name. Please try again."),
            type: MessageType.Error,
            expiration: TimeSpan.FromSeconds(2)));
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
            MaxItems = 1
        };
        return _messageManager;
    }
}
