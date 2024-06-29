using System.Reactive.Disposables;
using System.Reflection;
using AtomUI.Media;
using AtomUI.Platform.Windows;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Media;

namespace AtomUI.Controls;

internal class PopupShadowLayer : AbstractPopup, IShadowDecorator
{
   public static readonly StyledProperty<BoxShadows> MaskShadowsProperty = 
         Border.BoxShadowProperty.AddOwner<PopupShadowLayer>();

   public BoxShadows MaskShadows
   {
      get => GetValue(MaskShadowsProperty);
      set => SetValue(MaskShadowsProperty, value);
   }

   private static readonly FieldInfo ManagedPopupPositionerPopupInfo;
   private IManagedPopupPositionerPopup? _managedPopupPositionerPopup;

   static PopupShadowLayer()
   {
      ManagedPopupPositionerPopupInfo = typeof(ManagedPopupPositioner).GetField("_popup", 
         BindingFlags.Instance | BindingFlags.NonPublic)!;
   }

   public PopupShadowLayer()
   {
      WindowManagerAddShadowHint = false;
      IsLightDismissEnabled = false;
      _compositeDisposable = new CompositeDisposable();
   }
   
   private Popup? _target;
   private ShadowRenderer? _shadowRenderer;
   private CompositeDisposable _compositeDisposable;

   public void AttachToTarget(Popup popup)
   {
      if (_target is not null && _target != popup) {
         // 释放资源
         _compositeDisposable.Dispose();
      }
      _target = popup;
      ConfigureShadowPopup();
   }

   public void DetachedFromTarget(Popup popup) { }
   
   public void ShowShadows() { }
   public void HideShadows() {}

   private void ConfigureShadowPopup()
   {
      var offset = CalculatePopupOffset();
      HorizontalOffset = offset.X;
      VerticalOffset = offset.Y;
      // // 绑定资源要管理起来
      if (_target is not null) {
         _target.Opened += HandleTargetOpened;
         _target.Closed += HandleTargetClosed;
         SetupRelayBindings();
      }
      if (_shadowRenderer is null) {
         _shadowRenderer ??= new ShadowRenderer();
         Child = _shadowRenderer;
      }
   }

   private void SetupRelayBindings()
   {
      if (_target is not null) {
         // _compositeDisposable.Add(BindUtils.RelayBind(_target, PlacementProperty, this));
         // _compositeDisposable.Add(BindUtils.RelayBind(_target, PlacementGravityProperty, this));
         // _compositeDisposable.Add(BindUtils.RelayBind(_target, PlacementAnchorProperty, this));
         _compositeDisposable.Add(BindUtils.RelayBind(_target, PlacementTargetProperty, this));
      }
   }

   private void HandleTargetOpened(object? sender, EventArgs args)
   {
      SetupShadowRenderer();
      Open();
   }
   
   private void HandleTargetClosed(object? sender, EventArgs args)
   {
      _compositeDisposable.Dispose();
      Close();
   }

   protected override void NotifyClosed()
   {
      base.NotifyClosed();
      _managedPopupPositionerPopup = null;
   }
   
   private void SetupShadowRenderer()
   {
      SetupRelayBindings();
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

   protected override void NotifyPopupHostCreated(IPopupHost popupHost)
   {
      base.NotifyPopupHostCreated(popupHost);
      if (popupHost is WindowBase window) {
         window.Background = new SolidColorBrush(Colors.Transparent);
         window.SetTransparentForMouseEvents(true);
         if (_managedPopupPositionerPopup is null) {
            if (popupHost is PopupRoot popupRoot) {
               if (popupRoot.PlatformImpl?.PopupPositioner is ManagedPopupPositioner managedPopupPositioner) {
                  _managedPopupPositionerPopup = ManagedPopupPositionerPopupInfo.GetValue(managedPopupPositioner) as IManagedPopupPositionerPopup;
               }
            }
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

   private Point CalculatePopupOffset()
   {
      return default;
   }
   
   protected internal override void NotifyPopupHostPositionUpdated(IPopupHost popupHost, Control placementTarget)
   {
      base.NotifyPopupHostPositionUpdated(popupHost, placementTarget);
      popupHost.ConfigurePosition(placementTarget,
                                  PlacementMode.Pointer,
                                  offset: new Point(-40, 1),
                                  anchor: PopupAnchor.Top,
                                  gravity: PopupGravity.Top);
   }
}
