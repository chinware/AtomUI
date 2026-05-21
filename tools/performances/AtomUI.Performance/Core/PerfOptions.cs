namespace AtomUI.Performance;

internal sealed record PerfOptions(
    int Count,
    string Suite,
    string? MarkdownOutputPath,
    bool VerifyAccessories,
    bool VerifyEffectiveBrushes,
    bool VerifyAddonStates,
    bool VerifyAntDesignMetadata,
    bool VerifyIconHiddenSlots,
    bool VerifyIconProviderCache,
    bool VerifyButtonStates,
    bool VerifySpaceStates,
    bool VerifySelectStates,
    bool VerifyAutoCompleteStates,
    bool VerifyAvatarStates,
    bool VerifyBadgeStates,
    bool VerifyButtonSpinnerStates,
    bool VerifyCardStates,
    bool VerifyCarouselStates,
    bool VerifyCascaderStates,
    bool VerifyCheckBoxStates,
    bool VerifyCollapseStates,
    bool VerifyComboBoxStates,
    bool VerifyDatePickerStates,
    bool VerifyDescriptionsStates,
    bool VerifyDialogStates,
    bool VerifyDrawerStates,
    bool VerifyExpanderStates,
    bool VerifyEmptyStates,
    bool VerifyFloatButtonStates,
    bool VerifyFlyoutStates,
    bool VerifyFormStates,
    bool VerifyGroupBoxStates,
    bool VerifyImagePreviewerStates,
    bool VerifyInputStates,
    bool VerifyListBoxStates,
    bool VerifyListViewStates,
    bool VerifyMarqueeLabelStates,
    bool VerifyMentionsStates,
    bool VerifyMenuStates,
    bool VerifyNavMenuStates,
    bool VerifyMessageStates,
    bool VerifyMessageBoxStates,
    bool VerifyNotificationStates,
    bool VerifyNumericUpDownStates,
    bool VerifyOptionButtonGroupStates,
    bool VerifyPaginationStates,
    bool VerifyPopupConfirmStates,
    bool VerifyProgressBarStates,
    bool VerifyRadioButtonStates,
    bool VerifyQRCodeStates,
    bool VerifyRateStates,
    bool VerifyResultStates,
    bool VerifyScrollViewerStates,
    bool VerifySkeletonStates,
    bool VerifySegmentedStates,
    bool VerifySpinStates,
    bool VerifySwitchStates,
    bool VerifyTabControlStates,
    bool VerifyStepsStates,
    bool VerifyStatisticStates,
    bool VerifySplitViewStates,
    bool VerifySplitterStates,
    bool VerifySliderStates,
    bool VerifyTreeViewStates,
    bool VerifyTextBlockStates)
{
    public static PerfOptions Parse(string[] args)
    {
        var count                  = Program.DefaultCount;
        var suite                  = "addon";
        string? markdownOutput     = null;
        var verifyAccessories      = false;
        var verifyEffectiveBrushes = false;
        var verifyAddonStates      = false;
        var verifyAntDesignMetadata = false;
        var verifyIconHiddenSlots  = false;
        var verifyIconProviderCache = false;
        var verifyButtonStates    = false;
        var verifySpaceStates     = false;
        var verifySelectStates    = false;
        var verifyAutoCompleteStates = false;
        var verifyAvatarStates    = false;
        var verifyBadgeStates     = false;
        var verifyButtonSpinnerStates = false;
        var verifyCardStates      = false;
        var verifyCarouselStates  = false;
        var verifyCascaderStates  = false;
        var verifyCheckBoxStates  = false;
        var verifyCollapseStates  = false;
        var verifyComboBoxStates  = false;
        var verifyDatePickerStates = false;
        var verifyDescriptionsStates = false;
        var verifyDialogStates = false;
        var verifyDrawerStates = false;
        var verifyExpanderStates = false;
        var verifyEmptyStates = false;
        var verifyFloatButtonStates = false;
        var verifyFlyoutStates = false;
        var verifyFormStates = false;
        var verifyGroupBoxStates = false;
        var verifyImagePreviewerStates = false;
        var verifyInputStates = false;
        var verifyListBoxStates = false;
        var verifyListViewStates = false;
        var verifyMarqueeLabelStates = false;
        var verifyMentionsStates = false;
        var verifyMenuStates = false;
        var verifyNavMenuStates = false;
        var verifyMessageStates = false;
        var verifyMessageBoxStates = false;
        var verifyNotificationStates = false;
        var verifyNumericUpDownStates = false;
        var verifyOptionButtonGroupStates = false;
        var verifyPaginationStates = false;
        var verifyPopupConfirmStates = false;
        var verifyProgressBarStates = false;
        var verifyRadioButtonStates = false;
        var verifyQRCodeStates = false;
        var verifyRateStates = false;
        var verifyResultStates = false;
        var verifyScrollViewerStates = false;
        var verifySkeletonStates = false;
        var verifySegmentedStates = false;
        var verifySpinStates = false;
        var verifySwitchStates = false;
        var verifyTabControlStates = false;
        var verifyStepsStates = false;
        var verifyStatisticStates = false;
        var verifySplitViewStates = false;
        var verifySplitterStates = false;
        var verifySliderStates = false;
        var verifyTreeViewStates = false;
        var verifyTextBlockStates = false;

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--count" when i + 1 < args.Length && int.TryParse(args[i + 1], out var parsedCount):
                    count = parsedCount;
                    i++;
                    break;
                case "--markdown" when i + 1 < args.Length:
                    markdownOutput = args[i + 1];
                    i++;
                    break;
                case "--suite" when i + 1 < args.Length:
                    suite = args[i + 1];
                    i++;
                    break;
                case "--verify-accessories":
                    verifyAccessories = true;
                    break;
                case "--verify-effective-brushes":
                    verifyEffectiveBrushes = true;
                    break;
                case "--verify-addon-states":
                    verifyAddonStates = true;
                    break;
                case "--verify-antdesign-metadata":
                    verifyAntDesignMetadata = true;
                    break;
                case "--verify-icon-hidden-slots":
                    verifyIconHiddenSlots = true;
                    break;
                case "--verify-icon-provider-cache":
                    verifyIconProviderCache = true;
                    break;
                case "--verify-button-states":
                    verifyButtonStates = true;
                    break;
                case "--verify-space-states":
                    verifySpaceStates = true;
                    break;
                case "--verify-select-states":
                    verifySelectStates = true;
                    break;
                case "--verify-autocomplete-states":
                    verifyAutoCompleteStates = true;
                    break;
                case "--verify-avatar-states":
                    verifyAvatarStates = true;
                    break;
                case "--verify-badge-states":
                    verifyBadgeStates = true;
                    break;
                case "--verify-buttonspinner-states":
                    verifyButtonSpinnerStates = true;
                    break;
                case "--verify-card-states":
                    verifyCardStates = true;
                    break;
                case "--verify-carousel-states":
                    verifyCarouselStates = true;
                    break;
                case "--verify-cascader-states":
                    verifyCascaderStates = true;
                    break;
                case "--verify-checkbox-states":
                    verifyCheckBoxStates = true;
                    break;
                case "--verify-collapse-states":
                    verifyCollapseStates = true;
                    break;
                case "--verify-combobox-states":
                    verifyComboBoxStates = true;
                    break;
                case "--verify-datepicker-states":
                    verifyDatePickerStates = true;
                    break;
                case "--verify-descriptions-states":
                    verifyDescriptionsStates = true;
                    break;
                case "--verify-dialog-states":
                    verifyDialogStates = true;
                    break;
                case "--verify-drawer-states":
                    verifyDrawerStates = true;
                    break;
                case "--verify-expander-states":
                    verifyExpanderStates = true;
                    break;
                case "--verify-empty-states":
                    verifyEmptyStates = true;
                    break;
                case "--verify-floatbutton-states":
                    verifyFloatButtonStates = true;
                    break;
                case "--verify-flyout-states":
                    verifyFlyoutStates = true;
                    break;
                case "--verify-form-states":
                    verifyFormStates = true;
                    break;
                case "--verify-groupbox-states":
                    verifyGroupBoxStates = true;
                    break;
                case "--verify-imagepreviewer-states":
                    verifyImagePreviewerStates = true;
                    break;
                case "--verify-input-states":
                    verifyInputStates = true;
                    break;
                case "--verify-listbox-states":
                    verifyListBoxStates = true;
                    break;
                case "--verify-listview-states":
                    verifyListViewStates = true;
                    break;
                case "--verify-marqueelabel-states":
                    verifyMarqueeLabelStates = true;
                    break;
                case "--verify-mentions-states":
                    verifyMentionsStates = true;
                    break;
                case "--verify-menu-states":
                    verifyMenuStates = true;
                    break;
                case "--verify-navmenu-states":
                    verifyNavMenuStates = true;
                    break;
                case "--verify-message-states":
                    verifyMessageStates = true;
                    break;
                case "--verify-messagebox-states":
                    verifyMessageBoxStates = true;
                    break;
                case "--verify-notification-states":
                    verifyNotificationStates = true;
                    break;
                case "--verify-numericupdown-states":
                    verifyNumericUpDownStates = true;
                    break;
                case "--verify-optionbuttongroup-states":
                    verifyOptionButtonGroupStates = true;
                    break;
                case "--verify-pagination-states":
                    verifyPaginationStates = true;
                    break;
                case "--verify-popupconfirm-states":
                    verifyPopupConfirmStates = true;
                    break;
                case "--verify-progressbar-states":
                    verifyProgressBarStates = true;
                    break;
                case "--verify-radiobutton-states":
                    verifyRadioButtonStates = true;
                    break;
                case "--verify-qrcode-states":
                    verifyQRCodeStates = true;
                    break;
                case "--verify-rate-states":
                    verifyRateStates = true;
                    break;
                case "--verify-result-states":
                    verifyResultStates = true;
                    break;
                case "--verify-scrollviewer-states":
                    verifyScrollViewerStates = true;
                    break;
                case "--verify-skeleton-states":
                    verifySkeletonStates = true;
                    break;
                case "--verify-segmented-states":
                    verifySegmentedStates = true;
                    break;
                case "--verify-spin-states":
                    verifySpinStates = true;
                    break;
                case "--verify-switch-states":
                    verifySwitchStates = true;
                    break;
                case "--verify-tabcontrol-states":
                    verifyTabControlStates = true;
                    break;
                case "--verify-steps-states":
                    verifyStepsStates = true;
                    break;
                case "--verify-statistic-states":
                    verifyStatisticStates = true;
                    break;
                case "--verify-splitview-states":
                    verifySplitViewStates = true;
                    break;
                case "--verify-splitter-states":
                    verifySplitterStates = true;
                    break;
                case "--verify-slider-states":
                    verifySliderStates = true;
                    break;
                case "--verify-treeview-states":
                    verifyTreeViewStates = true;
                    break;
                case "--verify-textblock-states":
                    verifyTextBlockStates = true;
                    break;
            }
        }

        return new PerfOptions(
            Math.Max(1, count),
            suite,
            markdownOutput,
            verifyAccessories,
            verifyEffectiveBrushes,
            verifyAddonStates,
            verifyAntDesignMetadata,
            verifyIconHiddenSlots,
            verifyIconProviderCache,
            verifyButtonStates,
            verifySpaceStates,
            verifySelectStates,
            verifyAutoCompleteStates,
            verifyAvatarStates,
            verifyBadgeStates,
            verifyButtonSpinnerStates,
            verifyCardStates,
            verifyCarouselStates,
            verifyCascaderStates,
            verifyCheckBoxStates,
            verifyCollapseStates,
            verifyComboBoxStates,
            verifyDatePickerStates,
            verifyDescriptionsStates,
            verifyDialogStates,
            verifyDrawerStates,
            verifyExpanderStates,
            verifyEmptyStates,
            verifyFloatButtonStates,
            verifyFlyoutStates,
            verifyFormStates,
            verifyGroupBoxStates,
            verifyImagePreviewerStates,
            verifyInputStates,
            verifyListBoxStates,
            verifyListViewStates,
            verifyMarqueeLabelStates,
            verifyMentionsStates,
            verifyMenuStates,
            verifyNavMenuStates,
            verifyMessageStates,
            verifyMessageBoxStates,
            verifyNotificationStates,
            verifyNumericUpDownStates,
            verifyOptionButtonGroupStates,
            verifyPaginationStates,
            verifyPopupConfirmStates,
            verifyProgressBarStates,
            verifyRadioButtonStates,
            verifyQRCodeStates,
            verifyRateStates,
            verifyResultStates,
            verifyScrollViewerStates,
            verifySkeletonStates,
            verifySegmentedStates,
            verifySpinStates,
            verifySwitchStates,
            verifyTabControlStates,
            verifyStepsStates,
            verifyStatisticStates,
            verifySplitViewStates,
            verifySplitterStates,
            verifySliderStates,
            verifyTreeViewStates,
            verifyTextBlockStates);
    }
}
