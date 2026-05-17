using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

internal class PopupConfirmFlyout : Flyout
{
    internal WeakReference<PopupConfirm> PopupConfirmRef { get; set; }

    public PopupConfirmFlyout(PopupConfirm popupConfirm)
    {
        PopupConfirmRef = new WeakReference<PopupConfirm>(popupConfirm);
        this[!ShouldUseOverlayPopupProperty] = popupConfirm[!FlyoutHost.ShouldUseOverlayPopupProperty];
    }

    protected override Control CreatePresenter()
    {
        var presenter = (FlyoutPresenter)base.CreatePresenter();
        if (PopupConfirmRef.TryGetTarget(out var popupConfirm))
        {
            var popupConfirmContainer = new PopupConfirmContainer(popupConfirm);
            popupConfirmContainer[!PopupConfirmContainer.OkTextProperty] = popupConfirm[!PopupConfirm.OkTextProperty];
            popupConfirmContainer[!PopupConfirmContainer.CancelTextProperty] = popupConfirm[!PopupConfirm.CancelTextProperty];
            popupConfirmContainer[!PopupConfirmContainer.OkButtonTypeProperty] = popupConfirm[!PopupConfirm.OkButtonTypeProperty];
            popupConfirmContainer[!PopupConfirmContainer.IsShowCancelButtonProperty] =
                popupConfirm[!PopupConfirm.IsShowCancelButtonProperty];
            popupConfirmContainer[!PopupConfirmContainer.TitleProperty] = popupConfirm[!PopupConfirm.TitleProperty];
            popupConfirmContainer[!PopupConfirmContainer.ConfirmStatusProperty] =
                popupConfirm[!PopupConfirm.ConfirmStatusProperty];
            popupConfirmContainer[!PopupConfirmContainer.IconProperty] = popupConfirm[!PopupConfirm.IconProperty];
            popupConfirmContainer[!PopupConfirmContainer.ConfirmContentProperty] =
                popupConfirm[!PopupConfirm.ConfirmContentProperty];
            popupConfirmContainer[!PopupConfirmContainer.ConfirmContentTemplateProperty] =
                popupConfirm[!PopupConfirm.ConfirmContentTemplateProperty];
            
            presenter.Content = popupConfirmContainer;
        }
        return presenter;
    }
}
