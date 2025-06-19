using AtomUI.Controls.Themes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AtomUI.Controls;

internal class BaseOverflowMenuItem : MenuItem
{
    #region 公共属性

    public static readonly DirectProperty<BaseOverflowMenuItem, bool> IsClosableProperty =
        AvaloniaProperty.RegisterDirect<BaseOverflowMenuItem, bool>(nameof(IsClosable),
            o => o.IsClosable,
            (o, v) => o.IsClosable = v);

    public static readonly RoutedEvent<CloseTabRequestEventArgs> CloseTabEvent =
        RoutedEvent.Register<Button, CloseTabRequestEventArgs>(nameof(CloseTab), RoutingStrategies.Bubble);

    private bool _isClosable;

    public bool IsClosable
    {
        get => _isClosable;
        set => SetAndRaise(IsClosableProperty, ref _isClosable, value);
    }

    public event EventHandler<CloseTabRequestEventArgs>? CloseTab
    {
        add => AddHandler(CloseTabEvent, value);
        remove => RemoveHandler(CloseTabEvent, value);
    }

    #endregion

    private IconButton? _iconButton;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _iconButton = e.NameScope.Find<IconButton>(OverflowMenuItemThemeConstants.ItemCloseButtonPart);
        if (_iconButton is not null)
        {
            _iconButton.Click += (sender, args) => { NotifyCloseRequest(); };
        }
    }

    protected virtual void NotifyCloseRequest()
    {
    }
}

internal class CloseTabRequestEventArgs : RoutedEventArgs
{
    public CloseTabRequestEventArgs(RoutedEvent routedEvent, object tabItem)
        : base(routedEvent)
    {
        TabItem = tabItem;
    }

    public object TabItem { get; }
}