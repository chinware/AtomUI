using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

internal class TabStripMenuItem : MenuItem
{
   #region 公共属性

   public static readonly DirectProperty<TabStripMenuItem, bool> IsClosableProperty =
      AvaloniaProperty.RegisterDirect<TabStripMenuItem, bool>(nameof(IsClosable),
                                                              o => o.IsClosable,
                                                              (o, v) => o.IsClosable = v);
   
   public static readonly RoutedEvent<RoutedEventArgs> CloseTabEvent =
      RoutedEvent.Register<Button, RoutedEventArgs>(nameof(CloseTab), RoutingStrategies.Bubble);

   private bool _isClosable = false;
   public bool IsClosable
   {
      get => _isClosable;
      set => SetAndRaise(IsClosableProperty, ref _isClosable, value);
   }
   
   public TabStripItem? TabStripItem { get; set; }

   public event EventHandler<RoutedEventArgs>? CloseTab
   {
      add => AddHandler(CloseTabEvent, value);
      remove => RemoveHandler(CloseTabEvent, value);
   }
   
   #endregion

   private IconButton? _iconButton;
   
   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      _iconButton = e.NameScope.Find<IconButton>(TabStripMenuItemTheme.ItemCloseButtonPart);
      if (_iconButton is not null) {
         _iconButton.Click += (sender, args) =>
         {
            var menu = this.FindLogicalAncestorOfType<MenuBase>();
            if (menu is not null) {
               menu.Close();
               var eventArgs = new RoutedEventArgs(CloseTabEvent);
               RaiseEvent(eventArgs);
            }
         };
      }
   }
}