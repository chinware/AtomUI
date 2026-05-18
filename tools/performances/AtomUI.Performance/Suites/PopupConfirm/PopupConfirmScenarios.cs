using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Layout;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreatePopupConfirmScenarios()
    {
        return
        [
            new PerfScenario("PopupConfirm.Closed.Basic", _ => CreatePopupConfirm()),
            new PerfScenario("PopupConfirm.Closed.TitleOnly", _ => CreatePopupConfirm(confirmContent: null)),
            new PerfScenario("PopupConfirm.Closed.NoCancel", _ => CreatePopupConfirm(showCancelButton: false)),
            new PerfScenario("PopupConfirm.Closed.CustomIcon", _ => CreatePopupConfirm(icon: new QuestionCircleOutlined(), status: PopupConfirmStatus.Error)),
            new PerfScenario("PopupConfirm.Container.Basic", _ => CreatePopupConfirmContainer()),
            new PerfScenario("PopupConfirm.Container.TitleOnly", _ => CreatePopupConfirmContainer(confirmContent: null)),
            new PerfScenario("PopupConfirm.Container.NoCancel", _ => CreatePopupConfirmContainer(showCancelButton: false)),
            new PerfScenario("PopupConfirm.GalleryShape.PopupConfirmShowCase", _ => CreatePopupConfirmShowCaseShape())
        ];
    }

    private static PopupConfirm CreatePopupConfirm(string? confirmContent = DefaultConfirmContent,
                                                   bool showCancelButton = true,
                                                   PathIcon? icon = null,
                                                   PopupConfirmStatus status = PopupConfirmStatus.Warning,
                                                   string buttonText = "Delete",
                                                   PlacementMode placement = PlacementMode.Top,
                                                   bool shouldUseOverlayPopup = true)
    {
        return new PopupConfirm
        {
            Title                 = "Delete the task",
            ConfirmContent        = confirmContent,
            IsShowCancelButton    = showCancelButton,
            Icon                  = icon,
            ConfirmStatus         = status,
            OkText                = "Ok",
            CancelText            = "Cancel",
            Placement             = placement,
            IsArrowVisible        = true,
            ShouldUseOverlayPopup = shouldUseOverlayPopup,
            Content               = new AtomUI.Desktop.Controls.Button
            {
                ButtonType = ButtonType.Default,
                IsDanger   = true,
                Content    = buttonText
            }
        };
    }

    private static PopupConfirmContainer CreatePopupConfirmContainer(string? confirmContent = DefaultConfirmContent,
                                                                     bool showCancelButton = true,
                                                                     PathIcon? icon = null,
                                                                     PopupConfirmStatus status = PopupConfirmStatus.Warning)
    {
        var popupConfirm = CreatePopupConfirm(confirmContent, showCancelButton, icon, status);
        var container = new PopupConfirmContainer(popupConfirm)
        {
            Title              = popupConfirm.Title,
            ConfirmContent     = popupConfirm.ConfirmContent,
            IsShowCancelButton = popupConfirm.IsShowCancelButton,
            Icon               = popupConfirm.Icon,
            ConfirmStatus      = popupConfirm.ConfirmStatus,
            OkText             = popupConfirm.OkText,
            CancelText         = popupConfirm.CancelText,
            OkButtonType       = popupConfirm.OkButtonType
        };
        return container;
    }

    private static Control CreatePopupConfirmShowCaseShape()
    {
        var panel = new StackPanel
        {
            Spacing = 12
        };

        panel.Children.Add(CreatePopupConfirm(shouldUseOverlayPopup: false));
        panel.Children.Add(CreatePopupConfirm(shouldUseOverlayPopup: false));
        panel.Children.Add(CreatePlacementGrid<PopupConfirm>(placement => CreatePopupConfirm(
            placement: placement,
            buttonText: ShortPlacementLabel(placement))));
        panel.Children.Add(CreatePopupConfirm(
            icon: new QuestionCircleOutlined(),
            status: PopupConfirmStatus.Error));

        return panel;
    }

    private const string DefaultConfirmContent = "Are you sure to delete this task?";
}
