using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using Avalonia;
using Avalonia.Threading;
using AtomUIGallery.Localization;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ColorPicker;

public class ColorPickerViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "ColorPicker";
    
    public IScreen HostScreen { get; }
    
    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<ColorPickerApiRow>? _apiRows;
    private ObservableCollection<ColorPickerDesignTokenRow>? _designTokenRows;

    public ObservableCollection<ColorPickerApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<ColorPickerDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public ColorPickerViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

    public void EnsureApiRows()
    {
        if (ApiRows is not null)
        {
            return;
        }

        ApiRows =
        [
            new ColorPickerApiRow("DefaultValue", Lang(ColorPickerShowCaseLangResourceKind.ApiPropertyDefaultValue), "Color? / LinearGradientBrush?", "cyan", "null"),
            new ColorPickerApiRow("Value", Lang(ColorPickerShowCaseLangResourceKind.ApiPropertyValue), "Color? / LinearGradientBrush?", "cyan", "null"),
            new ColorPickerApiRow("Format", Lang(ColorPickerShowCaseLangResourceKind.ApiPropertyFormat), "ColorFormat", "blue", "Hex"),
            new ColorPickerApiRow("IsAlphaEnabled", Lang(ColorPickerShowCaseLangResourceKind.ApiPropertyIsAlphaEnabled), "bool", "green", "true"),
            new ColorPickerApiRow("IsTextVisible", Lang(ColorPickerShowCaseLangResourceKind.ApiPropertyIsTextVisible), "bool", "green", "false"),
            new ColorPickerApiRow("IsClearEnabled", Lang(ColorPickerShowCaseLangResourceKind.ApiPropertyIsClearEnabled), "bool", "green", "false"),
            new ColorPickerApiRow("SizeType", Lang(ColorPickerShowCaseLangResourceKind.ApiPropertySizeType), "SizeType", "blue", "Middle"),
            new ColorPickerApiRow("TriggerType", Lang(ColorPickerShowCaseLangResourceKind.ApiPropertyTriggerType), "FlyoutTriggerType", "blue", "Click"),
            new ColorPickerApiRow("ValueSyncStrategy", Lang(ColorPickerShowCaseLangResourceKind.ApiPropertyValueSyncStrategy), "ColorPickerValueSyncMode", "blue", "Immediate"),
            new ColorPickerApiRow("IsPaletteGroupEnabled", Lang(ColorPickerShowCaseLangResourceKind.ApiPropertyIsPaletteGroupEnabled), "bool", "green", "false")
        ];
    }

    public void EnsureDesignTokenRows()
    {
        if (DesignTokenRows is not null)
        {
            return;
        }

        DesignTokenRows =
        [
            new ColorPickerDesignTokenRow("ColorPickerWidth", Lang(ColorPickerShowCaseLangResourceKind.TokenNameColorPickerWidth), Lang(ColorPickerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ColorPickerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ColorPickerDesignTokenRow("ColorSpectrumHeight", Lang(ColorPickerShowCaseLangResourceKind.TokenNameColorSpectrumHeight), Lang(ColorPickerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ColorPickerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ColorPickerDesignTokenRow("ColorPickerHandlerSize", Lang(ColorPickerShowCaseLangResourceKind.TokenNameColorPickerHandlerSize), Lang(ColorPickerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ColorPickerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ColorPickerDesignTokenRow("ColorPickerSliderTrackSize", Lang(ColorPickerShowCaseLangResourceKind.TokenNameColorPickerSliderTrackSize), Lang(ColorPickerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ColorPickerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ColorPickerDesignTokenRow("ColorPickerPresetColorSize", Lang(ColorPickerShowCaseLangResourceKind.TokenNameColorPickerPresetColorSize), Lang(ColorPickerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ColorPickerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ColorPickerDesignTokenRow("ColorPickerPresetPanelWidth", Lang(ColorPickerShowCaseLangResourceKind.TokenNameColorPickerPresetPanelWidth), Lang(ColorPickerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ColorPickerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ColorPickerDesignTokenRow("TriggerPadding", Lang(ColorPickerShowCaseLangResourceKind.TokenNameTriggerPadding), Lang(ColorPickerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ColorPickerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ColorPickerDesignTokenRow("TriggerTextMargin", Lang(ColorPickerShowCaseLangResourceKind.TokenNameTriggerTextMargin), Lang(ColorPickerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ColorPickerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ColorPickerDesignTokenRow("ColorBlockDisabledOpacity", Lang(ColorPickerShowCaseLangResourceKind.TokenNameColorBlockDisabledOpacity), Lang(ColorPickerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ColorPickerShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(ColorPickerShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(ColorPickerShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            ColorPickerShowCaseLangResourceKind.ApiPropertyDefaultValue                => en_US.ApiPropertyDefaultValue,
            ColorPickerShowCaseLangResourceKind.ApiPropertyValue                       => en_US.ApiPropertyValue,
            ColorPickerShowCaseLangResourceKind.ApiPropertyFormat                      => en_US.ApiPropertyFormat,
            ColorPickerShowCaseLangResourceKind.ApiPropertyIsAlphaEnabled              => en_US.ApiPropertyIsAlphaEnabled,
            ColorPickerShowCaseLangResourceKind.ApiPropertyIsTextVisible               => en_US.ApiPropertyIsTextVisible,
            ColorPickerShowCaseLangResourceKind.ApiPropertyIsClearEnabled              => en_US.ApiPropertyIsClearEnabled,
            ColorPickerShowCaseLangResourceKind.ApiPropertySizeType                    => en_US.ApiPropertySizeType,
            ColorPickerShowCaseLangResourceKind.ApiPropertyTriggerType                 => en_US.ApiPropertyTriggerType,
            ColorPickerShowCaseLangResourceKind.ApiPropertyValueSyncStrategy           => en_US.ApiPropertyValueSyncStrategy,
            ColorPickerShowCaseLangResourceKind.ApiPropertyIsPaletteGroupEnabled       => en_US.ApiPropertyIsPaletteGroupEnabled,
            ColorPickerShowCaseLangResourceKind.TokenNameColorPickerWidth              => en_US.TokenNameColorPickerWidth,
            ColorPickerShowCaseLangResourceKind.TokenNameColorSpectrumHeight           => en_US.TokenNameColorSpectrumHeight,
            ColorPickerShowCaseLangResourceKind.TokenNameColorPickerHandlerSize        => en_US.TokenNameColorPickerHandlerSize,
            ColorPickerShowCaseLangResourceKind.TokenNameColorPickerSliderTrackSize    => en_US.TokenNameColorPickerSliderTrackSize,
            ColorPickerShowCaseLangResourceKind.TokenNameColorPickerPresetColorSize    => en_US.TokenNameColorPickerPresetColorSize,
            ColorPickerShowCaseLangResourceKind.TokenNameColorPickerPresetPanelWidth   => en_US.TokenNameColorPickerPresetPanelWidth,
            ColorPickerShowCaseLangResourceKind.TokenNameTriggerPadding                => en_US.TokenNameTriggerPadding,
            ColorPickerShowCaseLangResourceKind.TokenNameTriggerTextMargin             => en_US.TokenNameTriggerTextMargin,
            ColorPickerShowCaseLangResourceKind.TokenNameColorBlockDisabledOpacity     => en_US.TokenNameColorBlockDisabledOpacity,
            ColorPickerShowCaseLangResourceKind.TokenScopeComponent                    => en_US.TokenScopeComponent,
            ColorPickerShowCaseLangResourceKind.TokenStatusStable                      => en_US.TokenStatusStable,
            _                                                                          => kind.ToString()
        };
    }
}

public sealed record ColorPickerApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record ColorPickerDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
