using AtomUI.Platform.Windows;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AtomUI.Controls;

internal class PopupShadowLayer : AbstractPopup
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

   public void AttachToTarget(AbstractPopup popup)
   {
      // if (_popup is not null && _popup != popup) {
      //    // 释放资源
      // }
      //
      // _popup = popup;
      // ConfigureShadowPopup();
   }

   public void DetachedFromTarget(AbstractPopup popup) { }

   public void SetShadowMaskGeometry(Geometry geometry) { }

   public void SetShadows(BoxShadows shadows) { }

   public void ShowShadows() { }

   private void ConfigureShadowPopup()
   {
      // _shadowPopup ??= new AbstractPopup();
      // _shadowPopup.WindowManagerAddShadowHint = false;
      // _shadowPopup.IsLightDismissEnabled = true;
      // _shadowPopup.OverlayDismissEventPassThrough = true;
      // var offset = CalculateOffset();
      // _shadowPopup.HorizontalOffset = offset.X;
      // _shadowPopup.VerticalOffset = offset.Y;
      //
      // var popup = _popup!;
      // _shadowPopup.OverlayInputPassThroughElement = TopLevel.GetTopLevel(popup.PlacementTarget);
      // // 绑定资源要管理起来
      // BindUtils.RelayBind(popup, "Placement", _shadowPopup);
      // BindUtils.RelayBind(popup, "PlacementGravity", _shadowPopup);
      // BindUtils.RelayBind(popup, "PlacementAnchor", _shadowPopup);
      // BindUtils.RelayBind(popup, "PlacementTarget", _shadowPopup);
   }

   private void SetupShadowRenderer(AbstractPopup shadowPopup)
   {
      // if (_shadowRenderer is null) {
      //    _shadowRenderer ??= new ShadowRenderer();
      //    shadowPopup.Child = _shadowRenderer;
      // }
      //
      // var popupContent = _popup!.Child;
      // if (popupContent is not null) {
      //    // 理论上现在已经有大小了
      //    _shadowRenderer.Width = popupContent.DesiredSize.Width;
      //    _shadowRenderer.Height = popupContent.DesiredSize.Height;
      //    _shadowRenderer.Shadows = MaskShadows;
      //
      //    if (popupContent is IShadowMaskInfoProvider shadowMaskInfoProvider) {
      //       _shadowRenderer.MaskCornerRadius = shadowMaskInfoProvider.GetMaskCornerRadius();
      //    } else if (popupContent is BorderedStyleControl bordered) {
      //       _shadowRenderer.MaskCornerRadius = bordered.CornerRadius;
      //    } else if (popupContent is TemplatedControl templatedControl) {
      //       _shadowRenderer.MaskCornerRadius = templatedControl.CornerRadius;
      //    } else {
      //       // 利用反射判断
      //       // 先留空
      //    }
      // }
   }

   private Point CalculateOffset()
   {
      return default;
   }
}

// public class PopupShadowLayer : AvaloniaObject, IShadowDecorator
// {
//    private AbstractPopup? _popup;
//    private AbstractPopup? _shadowPopup;
//    private ShadowRenderer? _shadowRenderer;
//
//    public static readonly DirectProperty<PopupShadowLayer, BoxShadows> MaskShadowsProperty =
//       AvaloniaProperty.RegisterDirect<PopupShadowLayer, BoxShadows>(
//          nameof(MaskShadows),
//          o => o.MaskShadows,
//          (o, v) => o.MaskShadows = v);
//
//    private BoxShadows _maskShadows;
//
//    public BoxShadows MaskShadows
//    {
//       get => _maskShadows;
//       set => SetAndRaise(MaskShadowsProperty, ref _maskShadows, value);
//    }
//
//    public void AttachToTarget(AbstractPopup popup)
//    {
//       if (_popup is not null && _popup != popup) {
//          // 释放资源
//       }
//
//       _popup = popup;
//       ConfigureShadowPopup();
//    }
//
//    public void DetachedFromTarget(AbstractPopup popup) { }
//
//    public void SetShadowMaskGeometry(Geometry geometry) { }
//
//    public void SetShadows(BoxShadows shadows) { }
//
//    public void ShowShadows()
//    {
//       SetupShadowRenderer(_shadowPopup!);
//       _shadowPopup!.IsOpen = true;
//       if (_shadowPopup.Host is ContentControl host) {
//          host.Background = new SolidColorBrush(Colors.Transparent);
//          host.IsHitTestVisible = false;
//          var hostTopLevel = TopLevel.GetTopLevel(host);
//          if (hostTopLevel is WindowBase windowHost) {
//             windowHost.SetTransparentForMouseEvents(true);
//          }
//       }
//    }
//
//    public void HideShadows()
//    {
//       _shadowPopup!.IsOpen = false;
//    }
//
//    private void ConfigureShadowPopup()
//    {
//       _shadowPopup ??= new AbstractPopup();
//       _shadowPopup.WindowManagerAddShadowHint = false;
//       _shadowPopup.IsLightDismissEnabled = true;
//       _shadowPopup.OverlayDismissEventPassThrough = true;
//       var offset = CalculateOffset();
//       _shadowPopup.HorizontalOffset = offset.X;
//       _shadowPopup.VerticalOffset = offset.Y;
//
//       var popup = _popup!;
//       _shadowPopup.OverlayInputPassThroughElement = TopLevel.GetTopLevel(popup.PlacementTarget);
//       // 绑定资源要管理起来
//       BindUtils.RelayBind(popup, "Placement", _shadowPopup);
//       BindUtils.RelayBind(popup, "PlacementGravity", _shadowPopup);
//       BindUtils.RelayBind(popup, "PlacementAnchor", _shadowPopup);
//       BindUtils.RelayBind(popup, "PlacementTarget", _shadowPopup);
//    }
//
//    private void SetupShadowRenderer(AbstractPopup shadowPopup)
//    {
//       if (_shadowRenderer is null) {
//          _shadowRenderer ??= new ShadowRenderer();
//          shadowPopup.Child = _shadowRenderer;
//       }
//       
//       var popupContent = _popup!.Child;
//       if (popupContent is not null) {
//          // 理论上现在已经有大小了
//          _shadowRenderer.Width = popupContent.DesiredSize.Width;
//          _shadowRenderer.Height = popupContent.DesiredSize.Height;
//          _shadowRenderer.Shadows = MaskShadows;
//
//          if (popupContent is IShadowMaskInfoProvider shadowMaskInfoProvider) {
//             _shadowRenderer.MaskCornerRadius = shadowMaskInfoProvider.GetMaskCornerRadius();
//          } else if (popupContent is BorderedStyleControl bordered) {
//             _shadowRenderer.MaskCornerRadius = bordered.CornerRadius;
//          } else if (popupContent is TemplatedControl templatedControl) {
//             _shadowRenderer.MaskCornerRadius = templatedControl.CornerRadius;
//          } else {
//             // 利用反射判断
//             // 先留空
//          }
//       }
//    }
//
//    private Point CalculateOffset()
//    {
//       return default;
//    }
// }