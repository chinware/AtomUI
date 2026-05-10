using AtomUI.Theme.TokenSystem;
using AtomUI.Theme;

namespace AtomUI.Desktop.Controls.DesignTokens
{
    public enum ColorPickerTokenKind
    {
        CheckedMarkSize,
        ColorBlockDisabledOpacity,
        ColorBlockInnerShadows,
        ColorPickerAlphaInputWidth,
        ColorPickerHandlerDarkColor,
        ColorPickerHandlerLightColor,
        ColorPickerHandlerSize,
        ColorPickerHandlerSizeLG,
        ColorPickerHandlerSizeSM,
        ColorPickerInputNumberHandleWidth,
        ColorPickerInsetShadow,
        ColorPickerPresetColorGroupPadding,
        ColorPickerPresetColorSize,
        ColorPickerPresetPanelWidth,
        ColorPickerPreviewSize,
        ColorPickerSliderSize,
        ColorPickerSliderThumbSize,
        ColorPickerSliderTrackSize,
        ColorPickerWidth,
        ColorSpectrumHeight,
        SliderContainerMargin,
        TransparentBgSize,
        TriggerPadding,
        TriggerTextMargin
    }

    public class ColorPickerTokenResourceExtension : TokenResourceExtension<ColorPickerTokenKind>
    {
        public ColorPickerTokenResourceExtension()
        {
        }

        public ColorPickerTokenResourceExtension(ColorPickerTokenKind kind) : base(kind)
        {
        }
    }
}