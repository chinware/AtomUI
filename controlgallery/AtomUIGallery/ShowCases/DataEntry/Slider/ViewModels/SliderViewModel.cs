using System.Collections.ObjectModel;
using System.Globalization;
using System.Reactive;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Slider;

public class SliderViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Slider";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<SliderApiRow>? _apiRows;
    private ObservableCollection<SliderDesignTokenRow>? _designTokenRows;
    private List<SliderMark>? _sliderMarks;
    private SliderRangeValue _boundRangeValue = new()
    {
        StartValue = 20,
        EndValue   = 60
    };

    public ObservableCollection<SliderApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<SliderDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public List<SliderMark>? SliderMarks
    {
        get => _sliderMarks;
        set => this.RaiseAndSetIfChanged(ref _sliderMarks, value);
    }

    private bool _normalEnabled = true;

    public bool NormalEnabled
    {
        get => _normalEnabled;
        set => this.RaiseAndSetIfChanged(ref _normalEnabled, value);
    }

    public SliderRangeValue BoundRangeValue
    {
        get => _boundRangeValue;
        set
        {
            if (_boundRangeValue == value)
            {
                return;
            }

            this.RaiseAndSetIfChanged(ref _boundRangeValue, value);
            this.RaisePropertyChanged(nameof(BoundRangeValueText));
        }
    }

    public string BoundRangeValueText => string.Format(
        CultureInfo.CurrentCulture,
        "{0:0.#} - {1:0.#}",
        BoundRangeValue.StartValue,
        BoundRangeValue.EndValue);

    public ReactiveCommand<Unit, Unit> SetBoundRangeValueCommand { get; }

    public ReactiveCommand<Unit, Unit> ClearBoundRangeValueCommand { get; }

    public SliderViewModel(IScreen screen)
    {
        HostScreen                   = screen;
        SetBoundRangeValueCommand    = ReactiveCommand.Create(SetBoundRangeValue);
        ClearBoundRangeValueCommand  = ReactiveCommand.Create(ClearBoundRangeValue);
    }

    private void SetBoundRangeValue()
    {
        BoundRangeValue = new SliderRangeValue
        {
            StartValue = 35,
            EndValue   = 85
        };
    }

    private void ClearBoundRangeValue()
    {
        BoundRangeValue = default;
    }

    public void EnsureApiRows()
    {
        if (ApiRows is not null)
        {
            return;
        }

        ApiRows =
        [
            new SliderApiRow("Minimum", Lang(SliderShowCaseLangResourceKind.ApiPropertyMinimum), "double", "cyan", "0"),
            new SliderApiRow("Maximum", Lang(SliderShowCaseLangResourceKind.ApiPropertyMaximum), "double", "cyan", "100"),
            new SliderApiRow("Value", Lang(SliderShowCaseLangResourceKind.ApiPropertyValue), "double", "cyan", "0"),
            new SliderApiRow("SmallChange", Lang(SliderShowCaseLangResourceKind.ApiPropertySmallChange), "double", "cyan", "1"),
            new SliderApiRow("LargeChange", Lang(SliderShowCaseLangResourceKind.ApiPropertyLargeChange), "double", "cyan", "10"),
            new SliderApiRow("Orientation", Lang(SliderShowCaseLangResourceKind.ApiPropertyOrientation), "Orientation", "blue", "Horizontal"),
            new SliderApiRow("IsDirectionReversed", Lang(SliderShowCaseLangResourceKind.ApiPropertyIsDirectionReversed), "bool", "purple", "false"),
            new SliderApiRow("IsSnapToTickEnabled", Lang(SliderShowCaseLangResourceKind.ApiPropertyIsSnapToTickEnabled), "bool", "purple", "false"),
            new SliderApiRow("TickFrequency", Lang(SliderShowCaseLangResourceKind.ApiPropertyTickFrequency), "double", "cyan", "0"),
            new SliderApiRow("RangeValue", Lang(SliderShowCaseLangResourceKind.ApiPropertyRangeValue), "SliderRangeValue", "blue", "0, 0"),
            new SliderApiRow("IsRangeMode", Lang(SliderShowCaseLangResourceKind.ApiPropertyIsRangeMode), "bool", "purple", "false"),
            new SliderApiRow("Marks", Lang(SliderShowCaseLangResourceKind.ApiPropertyMarks), "List<SliderMark>?", "cyan", "null"),
            new SliderApiRow("ValueFormatTemplate", Lang(SliderShowCaseLangResourceKind.ApiPropertyValueFormatTemplate), "string", "cyan", "{0:0}"),
            new SliderApiRow("IsIncluded", Lang(SliderShowCaseLangResourceKind.ApiPropertyIsIncluded), "bool", "purple", "true"),
            new SliderApiRow("IsMotionEnabled", Lang(SliderShowCaseLangResourceKind.ApiPropertyIsMotionEnabled), "bool", "purple", "token"),
            new SliderApiRow("IsWaveSpiritEnabled", Lang(SliderShowCaseLangResourceKind.ApiPropertyIsWaveSpiritEnabled), "bool", "purple", "token")
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
            new SliderDesignTokenRow("SliderTrackSize", Lang(SliderShowCaseLangResourceKind.TokenNameSliderTrackSize), Lang(SliderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SliderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SliderDesignTokenRow("RailSize", Lang(SliderShowCaseLangResourceKind.TokenNameRailSize), Lang(SliderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SliderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SliderDesignTokenRow("MarkSize", Lang(SliderShowCaseLangResourceKind.TokenNameMarkSize), Lang(SliderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SliderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SliderDesignTokenRow("ThumbSize", Lang(SliderShowCaseLangResourceKind.TokenNameThumbSize), Lang(SliderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SliderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SliderDesignTokenRow("ThumbCircleSize", Lang(SliderShowCaseLangResourceKind.TokenNameThumbCircleSize), Lang(SliderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SliderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SliderDesignTokenRow("ThumbCircleSizeHover", Lang(SliderShowCaseLangResourceKind.TokenNameThumbCircleSizeHover), Lang(SliderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SliderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SliderDesignTokenRow("ThumbCircleBorderThickness", Lang(SliderShowCaseLangResourceKind.TokenNameThumbCircleBorderThickness), Lang(SliderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SliderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SliderDesignTokenRow("ThumbCircleBorderThicknessHover", Lang(SliderShowCaseLangResourceKind.TokenNameThumbCircleBorderThicknessHover), Lang(SliderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SliderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SliderDesignTokenRow("RailBg", Lang(SliderShowCaseLangResourceKind.TokenNameRailBg), Lang(SliderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SliderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SliderDesignTokenRow("RailHoverBg", Lang(SliderShowCaseLangResourceKind.TokenNameRailHoverBg), Lang(SliderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SliderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SliderDesignTokenRow("TrackBg", Lang(SliderShowCaseLangResourceKind.TokenNameTrackBg), Lang(SliderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SliderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SliderDesignTokenRow("TrackHoverBg", Lang(SliderShowCaseLangResourceKind.TokenNameTrackHoverBg), Lang(SliderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SliderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SliderDesignTokenRow("MarkBorderColor", Lang(SliderShowCaseLangResourceKind.TokenNameMarkBorderColor), Lang(SliderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SliderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SliderDesignTokenRow("MarkBorderColorHover", Lang(SliderShowCaseLangResourceKind.TokenNameMarkBorderColorHover), Lang(SliderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SliderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SliderDesignTokenRow("MarkBorderColorActive", Lang(SliderShowCaseLangResourceKind.TokenNameMarkBorderColorActive), Lang(SliderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SliderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SliderDesignTokenRow("ThumbCircleBorderColor", Lang(SliderShowCaseLangResourceKind.TokenNameThumbCircleBorderColor), Lang(SliderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SliderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SliderDesignTokenRow("ThumbCircleBorderHoverColor", Lang(SliderShowCaseLangResourceKind.TokenNameThumbCircleBorderHoverColor), Lang(SliderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SliderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SliderDesignTokenRow("ThumbCircleBorderActiveColor", Lang(SliderShowCaseLangResourceKind.TokenNameThumbCircleBorderActiveColor), Lang(SliderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SliderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SliderDesignTokenRow("ThumbCircleBorderColorDisabled", Lang(SliderShowCaseLangResourceKind.TokenNameThumbCircleBorderColorDisabled), Lang(SliderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SliderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SliderDesignTokenRow("ThumbOutlineColor", Lang(SliderShowCaseLangResourceKind.TokenNameThumbOutlineColor), Lang(SliderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SliderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SliderDesignTokenRow("ThumbOutlineThickness", Lang(SliderShowCaseLangResourceKind.TokenNameThumbOutlineThickness), Lang(SliderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SliderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SliderDesignTokenRow("TrackBgDisabled", Lang(SliderShowCaseLangResourceKind.TokenNameTrackBgDisabled), Lang(SliderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SliderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SliderDesignTokenRow("SliderPaddingHorizontal", Lang(SliderShowCaseLangResourceKind.TokenNameSliderPaddingHorizontal), Lang(SliderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SliderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SliderDesignTokenRow("SliderPaddingVertical", Lang(SliderShowCaseLangResourceKind.TokenNameSliderPaddingVertical), Lang(SliderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SliderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SliderDesignTokenRow("MarginPartWithMark", Lang(SliderShowCaseLangResourceKind.TokenNameMarginPartWithMark), Lang(SliderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SliderShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(SliderShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(SliderShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            SliderShowCaseLangResourceKind.ApiPropertyMinimum                         => en_US.ApiPropertyMinimum,
            SliderShowCaseLangResourceKind.ApiPropertyMaximum                         => en_US.ApiPropertyMaximum,
            SliderShowCaseLangResourceKind.ApiPropertyValue                           => en_US.ApiPropertyValue,
            SliderShowCaseLangResourceKind.ApiPropertySmallChange                     => en_US.ApiPropertySmallChange,
            SliderShowCaseLangResourceKind.ApiPropertyLargeChange                     => en_US.ApiPropertyLargeChange,
            SliderShowCaseLangResourceKind.ApiPropertyOrientation                     => en_US.ApiPropertyOrientation,
            SliderShowCaseLangResourceKind.ApiPropertyIsDirectionReversed             => en_US.ApiPropertyIsDirectionReversed,
            SliderShowCaseLangResourceKind.ApiPropertyIsSnapToTickEnabled             => en_US.ApiPropertyIsSnapToTickEnabled,
            SliderShowCaseLangResourceKind.ApiPropertyTickFrequency                   => en_US.ApiPropertyTickFrequency,
            SliderShowCaseLangResourceKind.ApiPropertyRangeValue                      => en_US.ApiPropertyRangeValue,
            SliderShowCaseLangResourceKind.ApiPropertyIsRangeMode                     => en_US.ApiPropertyIsRangeMode,
            SliderShowCaseLangResourceKind.ApiPropertyMarks                           => en_US.ApiPropertyMarks,
            SliderShowCaseLangResourceKind.ApiPropertyValueFormatTemplate             => en_US.ApiPropertyValueFormatTemplate,
            SliderShowCaseLangResourceKind.ApiPropertyIsIncluded                      => en_US.ApiPropertyIsIncluded,
            SliderShowCaseLangResourceKind.ApiPropertyIsMotionEnabled                 => en_US.ApiPropertyIsMotionEnabled,
            SliderShowCaseLangResourceKind.ApiPropertyIsWaveSpiritEnabled             => en_US.ApiPropertyIsWaveSpiritEnabled,
            SliderShowCaseLangResourceKind.RangeValueBindingTitle                     => en_US.RangeValueBindingTitle,
            SliderShowCaseLangResourceKind.RangeValueBindingDescription               => en_US.RangeValueBindingDescription,
            SliderShowCaseLangResourceKind.P2TextBoundRangeValue                      => en_US.P2TextBoundRangeValue,
            SliderShowCaseLangResourceKind.P2ContentSetRange                          => en_US.P2ContentSetRange,
            SliderShowCaseLangResourceKind.P2ContentClear                             => en_US.P2ContentClear,
            SliderShowCaseLangResourceKind.TokenNameSliderTrackSize                   => en_US.TokenNameSliderTrackSize,
            SliderShowCaseLangResourceKind.TokenNameRailSize                          => en_US.TokenNameRailSize,
            SliderShowCaseLangResourceKind.TokenNameMarkSize                          => en_US.TokenNameMarkSize,
            SliderShowCaseLangResourceKind.TokenNameThumbSize                         => en_US.TokenNameThumbSize,
            SliderShowCaseLangResourceKind.TokenNameThumbCircleSize                   => en_US.TokenNameThumbCircleSize,
            SliderShowCaseLangResourceKind.TokenNameThumbCircleSizeHover              => en_US.TokenNameThumbCircleSizeHover,
            SliderShowCaseLangResourceKind.TokenNameThumbCircleBorderThickness        => en_US.TokenNameThumbCircleBorderThickness,
            SliderShowCaseLangResourceKind.TokenNameThumbCircleBorderThicknessHover   => en_US.TokenNameThumbCircleBorderThicknessHover,
            SliderShowCaseLangResourceKind.TokenNameRailBg                            => en_US.TokenNameRailBg,
            SliderShowCaseLangResourceKind.TokenNameRailHoverBg                       => en_US.TokenNameRailHoverBg,
            SliderShowCaseLangResourceKind.TokenNameTrackBg                           => en_US.TokenNameTrackBg,
            SliderShowCaseLangResourceKind.TokenNameTrackHoverBg                      => en_US.TokenNameTrackHoverBg,
            SliderShowCaseLangResourceKind.TokenNameMarkBorderColor                   => en_US.TokenNameMarkBorderColor,
            SliderShowCaseLangResourceKind.TokenNameMarkBorderColorHover              => en_US.TokenNameMarkBorderColorHover,
            SliderShowCaseLangResourceKind.TokenNameMarkBorderColorActive             => en_US.TokenNameMarkBorderColorActive,
            SliderShowCaseLangResourceKind.TokenNameThumbCircleBorderColor            => en_US.TokenNameThumbCircleBorderColor,
            SliderShowCaseLangResourceKind.TokenNameThumbCircleBorderHoverColor       => en_US.TokenNameThumbCircleBorderHoverColor,
            SliderShowCaseLangResourceKind.TokenNameThumbCircleBorderActiveColor      => en_US.TokenNameThumbCircleBorderActiveColor,
            SliderShowCaseLangResourceKind.TokenNameThumbCircleBorderColorDisabled    => en_US.TokenNameThumbCircleBorderColorDisabled,
            SliderShowCaseLangResourceKind.TokenNameThumbOutlineColor                 => en_US.TokenNameThumbOutlineColor,
            SliderShowCaseLangResourceKind.TokenNameThumbOutlineThickness             => en_US.TokenNameThumbOutlineThickness,
            SliderShowCaseLangResourceKind.TokenNameTrackBgDisabled                   => en_US.TokenNameTrackBgDisabled,
            SliderShowCaseLangResourceKind.TokenNameSliderPaddingHorizontal           => en_US.TokenNameSliderPaddingHorizontal,
            SliderShowCaseLangResourceKind.TokenNameSliderPaddingVertical             => en_US.TokenNameSliderPaddingVertical,
            SliderShowCaseLangResourceKind.TokenNameMarginPartWithMark                => en_US.TokenNameMarginPartWithMark,
            SliderShowCaseLangResourceKind.TokenScopeComponent                        => en_US.TokenScopeComponent,
            SliderShowCaseLangResourceKind.TokenStatusStable                          => en_US.TokenStatusStable,
            _                                                                         => kind.ToString()
        };
    }
}

public sealed record SliderApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record SliderDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
