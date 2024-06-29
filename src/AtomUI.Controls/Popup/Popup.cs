using System.Reactive.Disposables;
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
   private CompositeDisposable _compositeDisposable;
   private bool _initialized;

   public Popup()
   {
      IsLightDismissEnabled = false;
      _shadowLayer = new PopupShadowLayer();
      _shadowLayer.AttachToTarget(this);
      _globalTokenBinder = new GlobalTokenBinder();
      _compositeDisposable = new CompositeDisposable();
   }
   
   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (!_initialized) {
          _globalTokenBinder.AddGlobalBinding(this, MaskShadowsProperty, GlobalResourceKey.BoxShadowsSecondary);
          _compositeDisposable.Add(BindUtils.RelayBind(this, MaskShadowsProperty, _shadowLayer));
         _initialized = true;
      }
   }
   
   protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnDetachedFromLogicalTree(e);
      _compositeDisposable.Dispose();
   }

   private PopupShadowLayer CreateShadowLayer()
   {
      return default!;
   }
}