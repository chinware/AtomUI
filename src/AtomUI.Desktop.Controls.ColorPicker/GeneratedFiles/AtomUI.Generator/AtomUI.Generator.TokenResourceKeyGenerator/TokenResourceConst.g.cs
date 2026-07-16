using AtomUI.Theme.TokenSystem;
using AtomUI.Theme;
using AtomUI.Theme.Resources;
using AtomUI.Theme.Styling;

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

    public sealed class ColorPickerTokenSharedTokenResourceExtension : ControlSharedTokenResourceExtension
    {
        public ColorPickerTokenSharedTokenResourceExtension(SharedTokenKind kind) : base("AtomUI", "ColorPicker", kind)
        {
        }
    }
}