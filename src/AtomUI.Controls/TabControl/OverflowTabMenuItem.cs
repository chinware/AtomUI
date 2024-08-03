using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace AtomUI.Controls;

internal class OverflowTabMenuItem : MenuItem
{
   #region 公共属性

   public static readonly DirectProperty<OverflowTabMenuItem, bool> IsClosableProperty =
      AvaloniaProperty.RegisterDirect<OverflowTabMenuItem, bool>(nameof(IsClosable),
                                                              o => o.IsClosable,
                                                              (o, v) => o.IsClosable = v);
   
   public static readonly RoutedEvent<CloseTabRequestEventArgs> CloseTabEvent =
      RoutedEvent.Register<Button, CloseTabRequestEventArgs>(nameof(CloseTab), RoutingStrategies.Bubble);

   private bool _isClosable = false;
   public bool IsClosable
   {
      get => _isClosable;
      set => SetAndRaise(IsClosableProperty, ref _isClosable, value);
   }
   
   public TabStripItem? TabStripItem { get; set; }

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
      _iconButton = e.NameScope.Find<IconButton>(OverflowTabMenuItemTheme.ItemCloseButtonPart);
      if (_iconButton is not null) {
         _iconButton.Click += (sender, args) =>
         {
            if (Parent is MenuBase menu) {
               var eventArgs = new CloseTabRequestEventArgs(CloseTabEvent, TabStripItem!);
               RaiseEvent(eventArgs);
               Dispatcher.UIThread.Post(() =>
               {
                  menu.Close();
               });
            }
         };
      }
   }
}

internal class CloseTabRequestEventArgs : RoutedEventArgs
{
   public CloseTabRequestEventArgs(RoutedEvent routedEvent, TabStripItem stripItem)
      : base(routedEvent)
   {
      TabStripItem = stripItem;
   }
   public TabStripItem TabStripItem { get; }
}