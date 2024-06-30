using System.Reactive.Disposables;
using System.Reflection;
using AtomUI.Media;
using AtomUI.Platform.Windows;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Media;

namespace AtomUI.Controls;

internal class PopupShadowLayer : LiteWindow, IShadowDecorator
{
   public static readonly StyledProperty<BoxShadows> MaskShadowsProperty =
      Border.BoxShadowProperty.AddOwner<PopupShadowLayer>();

   public BoxShadows MaskShadows
   {
      get => GetValue(MaskShadowsProperty);
      set => SetValue(MaskShadowsProperty, value);
   }
   
   private static readonly FieldInfo ManagedPopupPositionerPopupInfo;

   static PopupShadowLayer()
   {
      ManagedPopupPositionerPopupInfo = typeof(ManagedPopupPositioner).GetField("_popup",
         BindingFlags.Instance | BindingFlags.NonPublic)!;
   }

   private Popup? _target;
   private ShadowRenderer? _shadowRenderer;
   private CompositeDisposable? _compositeDisposable;
   private IManagedPopupPositionerPopup? _managedPopupPositionerPopup;
   private TopLevel? _topLevel;
   private bool _isOpened = false;

   public PopupShadowLayer(TopLevel topLevel)
      : base(topLevel, topLevel.PlatformImpl?.CreatePopup()!)
   {
      _topLevel = topLevel;
      Background = new SolidColorBrush(Colors.Transparent);
      if (this is WindowBase window) {
         window.SetTransparentForMouseEvents(true);
      }

      if (PlatformImpl?.PopupPositioner is ManagedPopupPositioner managedPopupPositioner) {
         _managedPopupPositionerPopup =
            ManagedPopupPositionerPopupInfo.GetValue(managedPopupPositioner) as IManagedPopupPositionerPopup;
      }
   }

   public void AttachToTarget(Popup popup)
   {
      _target = popup;
      ConfigureShadowPopup();
   }

   public void DetachedFromTarget(Popup popup) { }

   private void ConfigureShadowPopup()
   {
      if (_target is not null) {
         _target.Opened += HandleTargetOpened;
         _target.Closed += HandleTargetClosed;
      }

      if (_shadowRenderer is null) {
         _shadowRenderer ??= new ShadowRenderer();
         SetChild(_shadowRenderer);
      }
   }

   private void HandleTargetOpened(object? sender, EventArgs args)
   {
      SetupShadowRenderer();
      Open();
   }

   private void Open()
   {
      if (_isOpened) {
         return;
      }
      
      _compositeDisposable = new CompositeDisposable();
      if (_topLevel is Avalonia.Controls.Window window && window.PlatformImpl != null) {
         
      }

      if (_target?.Host is PopupRoot popupRoot) {
         popupRoot.PositionChanged += TargetPopupPositionChanged;
      }

      _compositeDisposable.Add(Disposable.Create(this, state =>
      {
         state.SetChild(null);
         Hide();
         ((ISetLogicalParent)state).SetParent(null);
         Dispose();
      }));
      ((ISetLogicalParent)this).SetParent(_target);
      SetupPositionAndSize();
      _isOpened = true;
      Show();
   }

   private void TargetPopupPositionChanged(object? sender, PixelPointEventArgs e)
   {
      SetupPositionAndSize();
   }

   private static IDisposable SubscribeToEventHandler<T, TEventHandler>(T target, TEventHandler handler,
                                                                        Action<T, TEventHandler> subscribe,
                                                                        Action<T, TEventHandler> unsubscribe)
   {
      subscribe(target, handler);
      return Disposable.Create((unsubscribe, target, handler),
                               state => state.unsubscribe(state.target, state.handler));
   }

   private void HandleTargetClosed(object? sender, EventArgs args)
   {
      if (_target is not null) {
         _target.Opened -= HandleTargetOpened;
         _target.Closed -= HandleTargetClosed;
      }

      _compositeDisposable?.Dispose();
      _compositeDisposable = null;
   }

   private void SetupShadowRenderer()
   {
      if (_target?.Child is not null && _shadowRenderer is not null) {
         // 理论上现在已经有大小了
         var content = _target?.Child!;
         var rendererSize = CalculateShadowRendererSize(content);
         _shadowRenderer.Shadows = MaskShadows;
         _shadowRenderer.Width = rendererSize.Width;
         _shadowRenderer.Height = rendererSize.Height;

         if (content is IShadowMaskInfoProvider shadowMaskInfoProvider) {
            _shadowRenderer.MaskCornerRadius = shadowMaskInfoProvider.GetMaskCornerRadius();
         } else if (content is BorderedStyleControl bordered) {
            _shadowRenderer.MaskCornerRadius = bordered.CornerRadius;
         } else if (content is TemplatedControl templatedControl) {
            _shadowRenderer.MaskCornerRadius = templatedControl.CornerRadius;
         }
      }
   }

   private Size CalculateShadowRendererSize(Control content)
   {
      var shadowThickness = MaskShadows.Thickness();
      var targetWidth = content.DesiredSize.Width + shadowThickness.Left + shadowThickness.Right;
      var targetHeight = content.DesiredSize.Height + shadowThickness.Top + shadowThickness.Bottom;
      return new Size(targetWidth, targetHeight);
   }

   private void SetupPositionAndSize()
   {
      if (_target?.Host is PopupRoot popupRoot) {
         var impl = popupRoot.PlatformImpl!;
         var targetPosition = impl.Position;
         double offsetX = targetPosition.X;
         double offsetY = targetPosition.Y;
         double scaling = _managedPopupPositionerPopup!.Scaling;
         var shadowThickness = MaskShadows.Thickness();
         offsetX -= shadowThickness.Left * scaling;
         offsetY -= shadowThickness.Top * scaling;
         _managedPopupPositionerPopup?.MoveAndResize(new Point(offsetX, offsetY),
                                                     new Size(_shadowRenderer!.Width, _shadowRenderer.Height));
      }
   }
}