using AtomUI.Platform.Windows;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AtomUI.Controls;

internal class PopupShadowLayer : AbstractPopup, IShadowDecorator
{
   public static readonly DirectProperty<PopupShadowLayer, BoxShadows> MaskShadowsProperty =
      AvaloniaProperty.RegisterDirect<PopupShadowLayer, BoxShadows>(
         nameof(MaskShadows),
         o => o.MaskShadows,
         (o, v) => o.MaskShadows = v);

   private BoxShadows _maskShadows;

   public BoxShadows MaskShadows
   {
      get => _maskShadows;
      set => SetAndRaise(MaskShadowsProperty, ref _maskShadows, value);
   }

   public PopupShadowLayer()
   {
      WindowManagerAddShadowHint = false;
      IsLightDismissEnabled = false;
   }
   
   private Popup? _target;
   private ShadowRenderer? _shadowRenderer;

   public void AttachToTarget(Popup popup)
   {
      if (_target is not null && _target != popup) {
         // 释放资源
      }
      _target = popup;
      ConfigureShadowPopup();
   }

   public void DetachedFromTarget(Popup popup) { }
   
   public void ShowShadows() { }
   public void HideShadows() {}

   private void ConfigureShadowPopup()
   {
      var offset = CalculateOffset();
      HorizontalOffset = offset.X;
      VerticalOffset = offset.Y;
      // // 绑定资源要管理起来
      if (_target is not null) {
         BindUtils.RelayBind(_target, "Placement", this);
         BindUtils.RelayBind(_target, "PlacementGravity", this);
         BindUtils.RelayBind(_target, "PlacementAnchor", this);
         BindUtils.RelayBind(_target, "PlacementTarget", this);
         _target.Opened += HandleTargetOpened;
         _target.Closed += HandleTargetClosed;
      }
      if (_shadowRenderer is null) {
         _shadowRenderer ??= new ShadowRenderer();
         Child = _shadowRenderer;
      }
   }

   private void HandleTargetOpened(object? sender, EventArgs args)
   {
      SetupShadowRenderer();
      // Open();
   }
   
   private void HandleTargetClosed(object? sender, EventArgs args)
   {
      // Close();
   }
   
   private void SetupShadowRenderer()
   {
      var popupContent = _target!.Child;
      if (popupContent is not null) {
         // 理论上现在已经有大小了
         _shadowRenderer!.Width = popupContent.DesiredSize.Width;
         _shadowRenderer.Height = popupContent.DesiredSize.Height;
         _shadowRenderer.Shadows = MaskShadows;

         if (popupContent is IShadowMaskInfoProvider shadowMaskInfoProvider) {
            _shadowRenderer.MaskCornerRadius = shadowMaskInfoProvider.GetMaskCornerRadius();
         } else if (popupContent is BorderedStyleControl bordered) {
            _shadowRenderer.MaskCornerRadius = bordered.CornerRadius;
         } else if (popupContent is TemplatedControl templatedControl) {
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
      }
   }

   private Point CalculateOffset()
   {
      return default;
   }
}
