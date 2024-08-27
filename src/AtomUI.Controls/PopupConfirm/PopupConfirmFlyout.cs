using AtomUI.Data;
using Avalonia.Controls;

namespace AtomUI.Controls;

internal class PopupConfirmFlyout : Flyout
{
   internal WeakReference<PopupConfirm> PopupConfirmRef { get; set; }

   public PopupConfirmFlyout(PopupConfirm popupConfirm)
   {
      PopupConfirmRef = new WeakReference<PopupConfirm>(popupConfirm);
   }
   
   protected override Control CreatePresenter()
   {
      var presenter = new FlyoutPresenter();

      if (PopupConfirmRef.TryGetTarget(out var popupConfirm)) {
         var popupConfirmContainer = new PopupConfirmContainer();
         BindUtils.RelayBind(popupConfirm, PopupConfirm.OkTextProperty, popupConfirmContainer, PopupConfirmContainer.OkTextProperty);
         BindUtils.RelayBind(popupConfirm, PopupConfirm.CancelTextProperty, popupConfirmContainer, PopupConfirmContainer.CancelTextProperty);
         BindUtils.RelayBind(popupConfirm, PopupConfirm.OkButtonTypeProperty, popupConfirmContainer, PopupConfirmContainer.OkButtonTypeProperty);
         BindUtils.RelayBind(popupConfirm, PopupConfirm.IsShowCancelButtonProperty, popupConfirmContainer, PopupConfirmContainer.IsShowCancelButtonProperty);
         BindUtils.RelayBind(popupConfirm, PopupConfirm.TitleProperty, popupConfirmContainer, PopupConfirmContainer.TitleProperty);
         BindUtils.RelayBind(popupConfirm, PopupConfirm.ConfirmStatusProperty, popupConfirmContainer, PopupConfirmContainer.ConfirmStatusProperty);
         BindUtils.RelayBind(popupConfirm, PopupConfirm.IconProperty, popupConfirmContainer, PopupConfirmContainer.IconProperty);
         BindUtils.RelayBind(popupConfirm, PopupConfirm.ConfirmContentProperty, popupConfirmContainer, PopupConfirmContainer.ConfirmContentProperty);
         BindUtils.RelayBind(popupConfirm, PopupConfirm.ConfirmContentTemplateProperty, popupConfirmContainer, PopupConfirmContainer.ConfirmContentTemplateProperty);
         presenter.Content = popupConfirmContainer;
      }
      
      BindUtils.RelayBind(this, IsShowArrowEffectiveProperty, presenter, IsShowArrowProperty);
      
      CalculateShowArrowEffective();
      SetupArrowPosition(Popup, presenter);
      return presenter;
   }
}