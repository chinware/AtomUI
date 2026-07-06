using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reactive;
using AtomUI.Controls;
using AtomUI.Data;
using Avalonia;
using Avalonia.Media;
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
    private Color? _boundColorValue = Color.Parse("#1677ff");
    private LinearGradientBrush? _boundGradientValue = CreateGradient("#108ee9", "#87d068");

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

    public Color? BoundColorValue
    {
        get => _boundColorValue;
        set
        {
            if (_boundColorValue == value)
            {
                return;
            }

            this.RaiseAndSetIfChanged(ref _boundColorValue, value);
            this.RaisePropertyChanged(nameof(BoundColorValueText));
        }
    }

    public string BoundColorValueText => BoundColorValue?.ToString() ?? "-";

    public LinearGradientBrush? BoundGradientValue
    {
        get => _boundGradientValue;
        set
        {
            if (ReferenceEquals(_boundGradientValue, value))
            {
                return;
            }

            this.RaiseAndSetIfChanged(ref _boundGradientValue, value);
            this.RaisePropertyChanged(nameof(BoundGradientValueText));
        }
    }

    public string BoundGradientValueText => FormatGradientValue(BoundGradientValue);

    public ReactiveCommand<Unit, Unit> SetBoundColorValueCommand { get; }

    public ReactiveCommand<Unit, Unit> ClearBoundColorValueCommand { get; }

    public ReactiveCommand<Unit, Unit> SetBoundGradientValueCommand { get; }

    public ReactiveCommand<Unit, Unit> ClearBoundGradientValueCommand { get; }

    public ColorPickerViewModel(IScreen screen)
    {
        HostScreen                         = screen;
        SetBoundColorValueCommand          = ReactiveCommand.Create(SetBoundColorValue);
        ClearBoundColorValueCommand        = ReactiveCommand.Create(ClearBoundColorValue);
        SetBoundGradientValueCommand       = ReactiveCommand.Create(SetBoundGradientValue);
        ClearBoundGradientValueCommand     = ReactiveCommand.Create(ClearBoundGradientValue);
    }

    private void SetBoundColorValue()
    {
        BoundColorValue = Color.Parse("#722ed1");
    }

    private void ClearBoundColorValue()
    {
        BoundColorValue = null;
    }

    private void SetBoundGradientValue()
    {
        BoundGradientValue = CreateGradient("#f5222d", "#faad14");
    }

    private void ClearBoundGradientValue()
    {
        BoundGradientValue = null;
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
            new ColorPickerApiRow("SizeType", Lang(ColorPickerShowCaseLangResourceKind.ApiPropertySizeType), "CustomizableSizeType", "blue", "Middle"),
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
            ColorPickerShowCaseLangResourceKind.ValueBindingTitle                      => en_US.ValueBindingTitle,
            ColorPickerShowCaseLangResourceKind.ValueBindingDescription                => en_US.ValueBindingDescription,
            ColorPickerShowCaseLangResourceKind.P2TextColorValue                       => en_US.P2TextColorValue,
            ColorPickerShowCaseLangResourceKind.P2TextGradientValue                    => en_US.P2TextGradientValue,
            ColorPickerShowCaseLangResourceKind.P2ContentSetColor                      => en_US.P2ContentSetColor,
            ColorPickerShowCaseLangResourceKind.P2ContentSetGradient                   => en_US.P2ContentSetGradient,
            ColorPickerShowCaseLangResourceKind.P2ContentClearColor                    => en_US.P2ContentClearColor,
            ColorPickerShowCaseLangResourceKind.P2ContentClearGradient                 => en_US.P2ContentClearGradient,
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

    private static LinearGradientBrush CreateGradient(string startColor, string endColor)
    {
        return new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0.5, RelativeUnit.Relative),
            EndPoint   = new RelativePoint(1, 0.5, RelativeUnit.Relative),
            GradientStops =
            [
                new GradientStop(Color.Parse(startColor), 0),
                new GradientStop(Color.Parse(endColor), 1)
            ]
        };
    }

    private static string FormatGradientValue(LinearGradientBrush? brush)
    {
        if (brush?.GradientStops is not { Count: > 0 } stops)
        {
            return "-";
        }

        return string.Join(" → ", stops.Select(stop =>
            string.Format(CultureInfo.CurrentCulture, "{0} {1:0.#}%", stop.Color, stop.Offset * 100)));
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
