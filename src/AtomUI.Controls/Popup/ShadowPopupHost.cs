using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

public class ShadowPopupHost : LiteWindow
{
   public static readonly StyledProperty<bool> IsOpenProperty = AvaloniaProperty.Register<Popup, bool>(nameof(IsOpen));
   
   public bool IsOpen
   {
      get => GetValue(IsOpenProperty);
      set => SetValue(IsOpenProperty, value);
   }
   
   private CompositeDisposable? _compositeDisposable;
   private PopupRoot _popupRoot;

   public ShadowPopupHost(PopupRoot popupRoot)
      : base(popupRoot, popupRoot.PlatformImpl!)
   {
      _popupRoot = popupRoot;
   }

   private void Open()
   {
      if (_compositeDisposable != null) {
         return;
      }

      Topmost = true;
      // SubscribeToEventHandler<PopupRoot, EventHandler<PixelPointEventArgs>>(parentPopupRoot, ParentPopupPositionChanged,
      //                                                                       (x, handler) => x.PositionChanged += handler,
      //                                                                       (x, handler) => x.PositionChanged -= handler).DisposeWith(handlerCleanup);
      //
      // if (parentPopupRoot.Parent is Popup popup)
      // {
      //    SubscribeToEventHandler<Popup, EventHandler<EventArgs>>(popup, ParentClosed,
      //                                                            (x, handler) => x.Closed += handler,
      //                                                            (x, handler) => x.Closed -= handler).DisposeWith(handlerCleanup);
      // }
      _compositeDisposable = new CompositeDisposable();
      _compositeDisposable.Add(Disposable.Create(this, state =>
      {
         state.SetChild(null);
         Hide();
         ((ISetLogicalParent)state).SetParent(null);
         Dispose();
      }));
      Show();
   }

   private static IDisposable SubscribeToEventHandler<T, TEventHandler>(T target, TEventHandler handler,
                                                                        Action<T, TEventHandler> subscribe,
                                                                        Action<T, TEventHandler> unsubscribe)
   {
      subscribe(target, handler);
      return Disposable.Create((unsubscribe, target, handler), state => state.unsubscribe(state.target, state.handler));
   }
}