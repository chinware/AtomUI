using AtomUI.Utils;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

public class Popup : AbstractPopup
{
   public static readonly DirectProperty<Popup, BoxShadows> MaskShadowsProperty =
      AvaloniaProperty.RegisterDirect<Popup, BoxShadows>(
         nameof(MaskShadows),
         o => o.MaskShadows,
         (o, v) => o.MaskShadows = v);

   private BoxShadows _maskShadows;

   public BoxShadows MaskShadows
   {
      get => _maskShadows;
      set => SetAndRaise(MaskShadowsProperty, ref _maskShadows, value);
   }
   
   // private PopupShadowLayer _shadowLayer;

   public Popup()
   {
      // _shadowLayer = new PopupShadowLayer();
      // _shadowLayer.AttachToTarget(this);
      // // TODO 是否需要释放
      // BindUtils.RelayBind(this, "MaskShadows", _shadowLayer);
      // SetupPopup();
   }

   private void SetupPopup()
   {
      // _popup.IsLightDismissEnabled = false;
      // _popup.Opened += HandlePopupOpened;
   }

   private void HandlePopupOpened(object? sender, EventArgs args)
   {
      // _shadowLayer.ShowShadows();
   }
}