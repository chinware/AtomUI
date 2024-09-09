using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

[TemplatePart(WindowNotificationManagerTheme.ItemsPart, typeof(Panel))]
public class WindowMessageManager : TemplatedControl, IMessageManager
{
   /// <summary>
   ///     Defines the <see cref="Position" /> property.
   /// </summary>
   public static readonly StyledProperty<NotificationPosition> PositionProperty =
        AvaloniaProperty.Register<WindowNotificationManager, NotificationPosition>(
            nameof(Position), NotificationPosition.TopRight);

   /// <summary>
   ///     Defines the <see cref="MaxItems" /> property.
   /// </summary>
   public static readonly StyledProperty<int> MaxItemsProperty =
        AvaloniaProperty.Register<WindowNotificationManager, int>(nameof(MaxItems), 5);

    private IList? _items;

    static WindowMessageManager()
    {
        HorizontalAlignmentProperty.OverrideDefaultValue<WindowMessageManager>(HorizontalAlignment.Stretch);
        VerticalAlignmentProperty.OverrideDefaultValue<WindowMessageManager>(VerticalAlignment.Stretch);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="WindowNotificationManager" /> class.
    /// </summary>
    /// <param name="host">The TopLevel that will host the control.</param>
    public WindowMessageManager(TopLevel? host)
    {
        if (host is not null) InstallFromTopLevel(host);
    }

    /// <summary>
    ///     Defines which corner of the screen notifications can be displayed in.
    /// </summary>
    /// <seealso cref="NotificationPosition" />
    public NotificationPosition Position
    {
        get => GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }

    /// <summary>
    ///     Defines the maximum number of notifications visible at once.
    /// </summary>
    public int MaxItems
    {
        get => GetValue(MaxItemsProperty);
        set => SetValue(MaxItemsProperty, value);
    }

    /// <summary>
    ///     Shows a Notification
    /// </summary>
    /// <param name="message">the content of the message</param>
    /// <param name="classes">style classes to apply</param>
    public async void Show(IMessage message, string[]? classes = null)
    {
        var expiration = message.Expiration;
        var onClose    = message.OnClose;
        Dispatcher.UIThread.VerifyAccess();

        var messageControl = new MessageCard
        {
            Icon        = message.Icon,
            Message     = message.Content,
            MessageType = message.Type
        };

        // Add style classes if any
        if (classes != null)
            foreach (var @class in classes)
                messageControl.Classes.Add(@class);

        messageControl.MessageClosed += (sender, args) =>
        {
            onClose?.Invoke();

            _items?.Remove(sender);
        };

        Dispatcher.UIThread.Post(() =>
        {
            _items?.Add(messageControl);

            if (_items?.OfType<MessageCard>().Count(i => !i.IsClosing) > MaxItems)
                _items.OfType<MessageCard>().First(i => !i.IsClosing).Close();
        });

        if (expiration == TimeSpan.Zero) return;

        await Task.Delay(expiration);
        messageControl.Close();
    }

    /// <inheritdoc />
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        var itemsControl = e.NameScope.Find<Panel>("PART_Items");
        _items = itemsControl?.Children;
    }

    /// <summary>
    ///     Installs the <see cref="WindowNotificationManager" /> within the <see cref="AdornerLayer" />
    /// </summary>
    private void InstallFromTopLevel(TopLevel topLevel)
    {
        topLevel.TemplateApplied += TopLevelOnTemplateApplied;
        var adorner = topLevel.FindDescendantOfType<VisualLayerManager>()?.AdornerLayer;
        if (adorner is not null)
        {
            adorner.Children.Add(this);
            AdornerLayer.SetAdornedElement(this, adorner);
        }
    }

    private void TopLevelOnTemplateApplied(object? sender, TemplateAppliedEventArgs e)
    {
        if (Parent is AdornerLayer adornerLayer)
        {
            adornerLayer.Children.Remove(this);
            AdornerLayer.SetAdornedElement(this, null);
        }

        // Reinstall notification manager on template reapplied.
        var topLevel = (TopLevel)sender!;
        topLevel.TemplateApplied -= TopLevelOnTemplateApplied;
        InstallFromTopLevel(topLevel);
    }
}