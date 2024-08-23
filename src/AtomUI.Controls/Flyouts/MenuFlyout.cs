using System.Collections;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reflection;
using AtomUI.Data;
using AtomUI.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Metadata;
using Avalonia.Styling;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public class MenuFlyout : Flyout
{
   private static readonly MethodInfo SetItemsSourceMethodInfo;

   static MenuFlyout()
   {
      SetItemsSourceMethodInfo = typeof(ItemCollection).GetMethod("SetItemsSource", BindingFlags.Instance | BindingFlags.NonPublic)!;
   }
   
   public MenuFlyout()
   {
      var itemCollectionType = typeof(ItemCollection);
      Items = (ItemCollection)Activator.CreateInstance(itemCollectionType, true)!;
   }

   /// <summary>
   /// Defines the <see cref="ItemsSource"/> property
   /// </summary>
   public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
      AvaloniaProperty.Register<MenuFlyout, IEnumerable?>(
         nameof(ItemsSource));

   /// <summary>
   /// Defines the <see cref="ItemTemplate"/> property
   /// </summary>
   public static readonly StyledProperty<IDataTemplate?> ItemTemplateProperty =
      AvaloniaProperty.Register<MenuFlyout, IDataTemplate?>(nameof(ItemTemplate));

   /// <summary>
   /// Defines the <see cref="ItemContainerTheme"/> property.
   /// </summary>
   public static readonly StyledProperty<ControlTheme?> ItemContainerThemeProperty =
      ItemsControl.ItemContainerThemeProperty.AddOwner<MenuFlyout>();

   [Content] public ItemCollection Items { get; }

   /// <summary>
   /// Gets or sets the items of the MenuFlyout
   /// </summary>
   public IEnumerable? ItemsSource
   {
      get => GetValue(ItemsSourceProperty);
      set => SetValue(ItemsSourceProperty, value);
   }

   /// <summary>
   /// Gets or sets the template used for the items
   /// </summary>
   public IDataTemplate? ItemTemplate
   {
      get => GetValue(ItemTemplateProperty);
      set => SetValue(ItemTemplateProperty, value);
   }

   private Classes? _classes;

   protected override Control CreatePresenter()
   {
      var presenter = new MenuFlyoutPresenter()
      {
         ItemsSource = Items,
         [!ItemsControl.ItemTemplateProperty] = this[!ItemTemplateProperty],
         [!ItemsControl.ItemContainerThemeProperty] = this[!ItemContainerThemeProperty],
      };
      BindUtils.RelayBind(this, IsShowArrowEffectiveProperty, presenter, IsShowArrowProperty);
      SetupArrowPosition(Popup, presenter);
      return presenter;
   }
   
   private void SetupArrowPosition(Popup popup, MenuFlyoutPresenter? flyoutPresenter = null)
   {
      if (flyoutPresenter is null) {
         var child = popup.Child;
         if (child is MenuFlyoutPresenter childPresenter) {
            flyoutPresenter = childPresenter;
         }
      }

      var placement = popup.Placement;
      var anchor = popup.PlacementAnchor;
      var gravity = popup.PlacementGravity;

      if (flyoutPresenter is not null) {
         var arrowPosition = PopupUtils.CalculateArrowPosition(placement, anchor, gravity);
         if (arrowPosition.HasValue) {
            flyoutPresenter.ArrowPosition = arrowPosition.Value;
         }
      }
   }
   
   protected override void OnOpening(CancelEventArgs args)
   {
      if (Popup.Child is { } presenter) {
         if (_classes != null) {
            SetPresenterClasses(presenter, FlyoutPresenterClasses);
         }
   
         if (FlyoutPresenterTheme is { } theme) {
            presenter.SetValue(Control.ThemeProperty, theme);
         }
      }
   
      base.OnOpening(args);
      
      var dismissLayer = LightDismissOverlayLayer.GetLightDismissOverlayLayer(Popup.PlacementTarget!);
      var compositeDisposable = _compositeDisposable!;
      if (dismissLayer != null) {
         dismissLayer.IsVisible = true;
         dismissLayer.InputPassThroughElement = OverlayInputPassThroughElement;
                    
         Disposable.Create(() =>
         {
            dismissLayer.IsVisible = false;
            dismissLayer.InputPassThroughElement = null;
         }).DisposeWith(compositeDisposable);
                    
         SubscribeToEventHandler<LightDismissOverlayLayer, EventHandler<PointerPressedEventArgs>>(
            dismissLayer,
            PointerPressedDismissOverlay,
            (x, handler) => x.PointerPressed += handler,
            (x, handler) => x.PointerPressed -= handler).DisposeWith(compositeDisposable);
      }
      var inputManager = AvaloniaLocator.Current.GetService<IInputManager>();
      inputManager?.Process.Subscribe(ListenForNonClientClick).DisposeWith(compositeDisposable);
   }

   private void ListenForNonClientClick(RawInputEventArgs e)
   {
      var mouse = e as RawPointerEventArgs;

      if (mouse?.Type == RawPointerEventType.NonClientLeftButtonDown) {
         Hide();
      }
   }
   
   private void PointerPressedDismissOverlay(object? sender, PointerPressedEventArgs e)
   {
      if (e.Source is Visual v && !IsChildOrThis(v)) {
         // Ensure the popup is closed if it was not closed by a pass-through event handler
         if (IsOpen) {
            Hide();
         }
      }
   }

   private bool IsChildOrThis(Visual child)
   {
      if (!IsOpen) {
         return false;
      }

      var popupHost = Popup.Host;

      Visual? root = child.GetVisualRoot() as Visual;

      while (root is IHostedVisualTreeRoot hostedRoot) {
         if (root == popupHost) {
            return true;
         }

         root = hostedRoot.Host?.GetVisualRoot() as Visual;
      }

      return false;
   }
   
   private static IDisposable SubscribeToEventHandler<T, TEventHandler>(T target, TEventHandler handler, Action<T, TEventHandler> subscribe, Action<T, TEventHandler> unsubscribe)
   {
      subscribe(target, handler);

      return Disposable.Create((unsubscribe, target, handler), state => state.unsubscribe(state.target, state.handler));
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
   {
      base.OnPropertyChanged(change);

      if (change.Property == ItemsSourceProperty) {
         SetItemsSourceMethodInfo.Invoke(Items, new object?[]{change.GetNewValue<IEnumerable?>()});
      }
   }
   
   protected override bool ShowAtCore(Control placementTarget, bool showAtPointer = false)
   {
      Popup.IsLightDismissEnabled = false;
      return base.ShowAtCore(placementTarget, showAtPointer);
   }

}