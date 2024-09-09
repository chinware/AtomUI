using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace AtomUI.Controls;

public class MenuFlyoutPresenter : MenuBase, IShadowMaskInfoProvider
{
   #region 公共属性定义

   public static readonly StyledProperty<bool> IsShowArrowProperty =
      ArrowDecoratedBox.IsShowArrowProperty.AddOwner<MenuFlyoutPresenter>();

   public static readonly StyledProperty<ArrowPosition> ArrowPositionProperty =
      ArrowDecoratedBox.ArrowPositionProperty.AddOwner<MenuFlyoutPresenter>();

   public static readonly RoutedEvent<FlyoutMenuItemClickedEventArgs> MenuItemClickedEvent =
      RoutedEvent.Register<DropdownButton, FlyoutMenuItemClickedEventArgs>(
         nameof(MenuItemClicked),
         RoutingStrategies.Bubble);

   /// <summary>
   /// 是否显示指示箭头
   /// </summary>
   public bool IsShowArrow
   {
      get => GetValue(IsShowArrowProperty);
      set => SetValue(IsShowArrowProperty, value);
   }

   /// <summary>
   /// 箭头渲染的位置
   /// </summary>
   public ArrowPosition ArrowPosition
   {
      get => GetValue(ArrowPositionProperty);
      set => SetValue(ArrowPositionProperty, value);
   }

   public event EventHandler<FlyoutMenuItemClickedEventArgs>? MenuItemClicked
   {
      add => AddHandler(MenuItemClickedEvent, value);
      remove => RemoveHandler(MenuItemClickedEvent, value);
   }

   public WeakReference<MenuFlyout>? MenuFlyout { get; set; }
   
   #endregion

   private ArrowDecoratedBox? _arrowDecoratedBox;

   public MenuFlyoutPresenter()
      : base(new DefaultMenuInteractionHandler(true)) { }

   public MenuFlyoutPresenter(IMenuInteractionHandler menuInteractionHandler)
      : base(menuInteractionHandler)
   {
   }

   public override void Close()
   {
      // DefaultMenuInteractionHandler calls this
      if (MenuFlyout is not null) {
         if (MenuFlyout.TryGetTarget(out var menuFlyout)) {
            SelectedIndex = -1;
            menuFlyout.Hide();
         }
      }
   }
   
   protected override void ContainerForItemPreparedOverride(Control container, object? item, int index)
   {
      base.ContainerForItemPreparedOverride(container, item, index);
      if (container is MenuItem menuItem) {
         BindMenuItemClickedRecursive(menuItem);
      }
   }
   
   private void BindMenuItemClickedRecursive(MenuItem menuItem)
   {
      foreach (var childItem in menuItem.Items) {
         if (childItem is MenuItem childMenuItem) {
            BindMenuItemClickedRecursive(childMenuItem);
         }
      }
      // 绑定自己
      menuItem.Click += HandleMenuItemClicked;
   }

   private void ClearMenuItemClickedRecursive(MenuItem menuItem)
   { 
      foreach (var childItem in menuItem.Items) {
         if (childItem is MenuItem childMenuItem) {
            ClearMenuItemClickedRecursive(childMenuItem);
         }
      }
      // 绑定自己
      menuItem.Click -= HandleMenuItemClicked;
      
   }

   protected override void ClearContainerForItemOverride(Control container)
   {
      base.ClearContainerForItemOverride(container);
      if (container is MenuItem menuItem) {
         ClearMenuItemClickedRecursive(menuItem);
      }
   }

   private void HandleMenuItemClicked(object? sender, RoutedEventArgs args)
   {
      if (sender is MenuItem menuItem) {
         var ev = new FlyoutMenuItemClickedEventArgs(MenuItemClickedEvent, menuItem);
         RaiseEvent(ev);
      }
   }

   public override void Open()
   {
      throw new NotSupportedException("Use MenuFlyout.ShowAt(Control) instead");
   }

   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      _arrowDecoratedBox = e.NameScope.Find<ArrowDecoratedBox>(MenuFlyoutPresenterTheme.RootContainerPart);
   }

   protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnDetachedFromVisualTree(e);

      foreach (var i in LogicalChildren) {
         if (i is MenuItem menuItem) {
            menuItem.IsSubMenuOpen = false;
         }
      }
   }

   public CornerRadius GetMaskCornerRadius()
   {
      if (_arrowDecoratedBox is not null) {
         return _arrowDecoratedBox.CornerRadius;
      }

      return new CornerRadius(0);
   }

   public Rect GetMaskBounds()
   {
      if (_arrowDecoratedBox is not null) {
         var contentRect = _arrowDecoratedBox.GetContentRect(Bounds.Size);
         var adjustedPos = _arrowDecoratedBox.TranslatePoint(contentRect.Position, this) ?? default;
         return new Rect(adjustedPos, contentRect.Size);
      }

      return Bounds;
   }
}

public class FlyoutMenuItemClickedEventArgs : RoutedEventArgs
{
   /// <summary>
   /// 当前鼠标点击的菜单项
   /// </summary>
   public MenuItem Item { get; }

   public FlyoutMenuItemClickedEventArgs(RoutedEvent routedEvent, MenuItem menuItem)
      : base(routedEvent)
   {
      Item = menuItem;
   }
}