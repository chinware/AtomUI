using AtomUI.Data;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace AtomUI.Controls;

public class Popup : AbstractPopup
{
   public static readonly StyledProperty<BoxShadows> MaskShadowsProperty =
      Border.BoxShadowProperty.AddOwner<Popup>();

   public BoxShadows MaskShadows
   {
      get => GetValue(MaskShadowsProperty);
      set => SetValue(MaskShadowsProperty, value);
   }
   
   private PopupShadowLayer _shadowLayer;
   private GlobalTokenBinder _globalTokenBinder;
   private bool _initialized;

   public Popup()
   {
      IsLightDismissEnabled = false;
      _shadowLayer = new PopupShadowLayer();
      _shadowLayer.AttachToTarget(this);
      // TODO 是否需要释放
      BindUtils.RelayBind(this, MaskShadowsProperty, _shadowLayer);
      _globalTokenBinder = new GlobalTokenBinder();
   }
   
   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (!_initialized) {
          _globalTokenBinder.AddGlobalBinding(this, MaskShadowsProperty, GlobalResourceKey.BoxShadowsSecondary);
         _initialized = true;
      }
   }
}