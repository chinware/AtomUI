using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Globalization;
using System.Reactive;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Rate;

public class RateViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Rate";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<RateApiRow>? _apiRows;
    private ObservableCollection<RateDesignTokenRow>? _designTokenRows;
    private IList<string>? _tooltips;
    private double _twoWayValue;
    private string? _twoWayValueSummary;

    public ObservableCollection<RateApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<RateDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public IList<string>? Tooltips
    {
        get => _tooltips;
        set => this.RaiseAndSetIfChanged(ref _tooltips, value);
    }

    public double TwoWayValue
    {
        get => _twoWayValue;
        set
        {
            if (Math.Abs(_twoWayValue - value) < double.Epsilon)
            {
                return;
            }

            this.RaiseAndSetIfChanged(ref _twoWayValue, value);
            UpdateTwoWayValueSummary();
        }
    }

    public string? TwoWayValueSummary
    {
        get => _twoWayValueSummary;
        set => this.RaiseAndSetIfChanged(ref _twoWayValueSummary, value);
    }

    private string? _activeTooltip;

    public string? ActiveTooltip
    {
        get => _activeTooltip;
        set => this.RaiseAndSetIfChanged(ref _activeTooltip, value);
    }

    public ReactiveCommand<Unit, Unit> SetFourStarsCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearTwoWayValueCommand { get; }

    public RateViewModel(IScreen screen)
    {
        HostScreen = screen;
        _twoWayValue = 2.0;
        SetFourStarsCommand      = ReactiveCommand.Create(HandleSetFourStars);
        ClearTwoWayValueCommand  = ReactiveCommand.Create(HandleClearTwoWayValue);
        UpdateTwoWayValueSummary();
    }

    public void RefreshLocalizedState()
    {
        UpdateTwoWayValueSummary();
    }

    private void HandleSetFourStars()
    {
        TwoWayValue = 4.0;
    }

    private void HandleClearTwoWayValue()
    {
        TwoWayValue = 0.0;
    }

    private void UpdateTwoWayValueSummary()
    {
        TwoWayValueSummary = string.Format(CultureInfo.CurrentCulture,
            RateShowCaseLanguage.Get(RateShowCaseLangResourceKind.P2TwoWayValueSummaryFormat, "Selected value: {0:0.#}"),
            TwoWayValue);
    }

    public void EnsureApiRows()
    {
        if (ApiRows is not null)
        {
            return;
        }

        ApiRows =
        [
            new RateApiRow("IsAllowClear", Lang(RateShowCaseLangResourceKind.ApiPropertyIsAllowClear), "bool", "purple", "true"),
            new RateApiRow("IsAllowHalf", Lang(RateShowCaseLangResourceKind.ApiPropertyIsAllowHalf), "bool", "purple", "false"),
            new RateApiRow("Character", Lang(RateShowCaseLangResourceKind.ApiPropertyCharacter), "object?", "cyan", "StarFilled"),
            new RateApiRow("StarColor", Lang(RateShowCaseLangResourceKind.ApiPropertyStarColor), "IBrush?", "cyan", "token"),
            new RateApiRow("StarBgColor", Lang(RateShowCaseLangResourceKind.ApiPropertyStarBgColor), "IBrush?", "cyan", "token"),
            new RateApiRow("Count", Lang(RateShowCaseLangResourceKind.ApiPropertyCount), "int", "cyan", "5"),
            new RateApiRow("Value", Lang(RateShowCaseLangResourceKind.ApiPropertyValue), "double", "cyan", "DefaultValue"),
            new RateApiRow("DefaultValue", Lang(RateShowCaseLangResourceKind.ApiPropertyDefaultValue), "double", "cyan", "0"),
            new RateApiRow("IsKeyboardEnabled", Lang(RateShowCaseLangResourceKind.ApiPropertyIsKeyboardEnabled), "bool", "purple", "true"),
            new RateApiRow("ToolTips", Lang(RateShowCaseLangResourceKind.ApiPropertyToolTips), "IList<string>?", "cyan", "null"),
            new RateApiRow("SizeType", Lang(RateShowCaseLangResourceKind.ApiPropertySizeType), "SizeType", "blue", "Middle"),
            new RateApiRow("IsMotionEnabled", Lang(RateShowCaseLangResourceKind.ApiPropertyIsMotionEnabled), "bool", "purple", "token"),
            new RateApiRow("ValueChanged", Lang(RateShowCaseLangResourceKind.ApiPropertyValueChanged), "event", "default", "null"),
            new RateApiRow("HoverValueChanged", Lang(RateShowCaseLangResourceKind.ApiPropertyHoverValueChanged), "event", "default", "null")
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
            new RateDesignTokenRow("StarColor", Lang(RateShowCaseLangResourceKind.TokenNameStarColor), Lang(RateShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(RateShowCaseLangResourceKind.TokenStatusStable), "success"),
            new RateDesignTokenRow("StarSize", Lang(RateShowCaseLangResourceKind.TokenNameStarSize), Lang(RateShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(RateShowCaseLangResourceKind.TokenStatusStable), "success"),
            new RateDesignTokenRow("StarSizeSM", Lang(RateShowCaseLangResourceKind.TokenNameStarSizeSM), Lang(RateShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(RateShowCaseLangResourceKind.TokenStatusStable), "success"),
            new RateDesignTokenRow("StarSizeLG", Lang(RateShowCaseLangResourceKind.TokenNameStarSizeLG), Lang(RateShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(RateShowCaseLangResourceKind.TokenStatusStable), "success"),
            new RateDesignTokenRow("StarHoverScale", Lang(RateShowCaseLangResourceKind.TokenNameStarHoverScale), Lang(RateShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(RateShowCaseLangResourceKind.TokenStatusStable), "success"),
            new RateDesignTokenRow("StarBg", Lang(RateShowCaseLangResourceKind.TokenNameStarBg), Lang(RateShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(RateShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(RateShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(RateShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            RateShowCaseLangResourceKind.ApiPropertyIsAllowClear       => en_US.ApiPropertyIsAllowClear,
            RateShowCaseLangResourceKind.ApiPropertyIsAllowHalf        => en_US.ApiPropertyIsAllowHalf,
            RateShowCaseLangResourceKind.ApiPropertyCharacter          => en_US.ApiPropertyCharacter,
            RateShowCaseLangResourceKind.ApiPropertyStarColor          => en_US.ApiPropertyStarColor,
            RateShowCaseLangResourceKind.ApiPropertyStarBgColor        => en_US.ApiPropertyStarBgColor,
            RateShowCaseLangResourceKind.ApiPropertyCount              => en_US.ApiPropertyCount,
            RateShowCaseLangResourceKind.ApiPropertyValue              => en_US.ApiPropertyValue,
            RateShowCaseLangResourceKind.ApiPropertyDefaultValue       => en_US.ApiPropertyDefaultValue,
            RateShowCaseLangResourceKind.ApiPropertyIsKeyboardEnabled  => en_US.ApiPropertyIsKeyboardEnabled,
            RateShowCaseLangResourceKind.ApiPropertyToolTips           => en_US.ApiPropertyToolTips,
            RateShowCaseLangResourceKind.ApiPropertySizeType           => en_US.ApiPropertySizeType,
            RateShowCaseLangResourceKind.ApiPropertyIsMotionEnabled    => en_US.ApiPropertyIsMotionEnabled,
            RateShowCaseLangResourceKind.ApiPropertyValueChanged       => en_US.ApiPropertyValueChanged,
            RateShowCaseLangResourceKind.ApiPropertyHoverValueChanged  => en_US.ApiPropertyHoverValueChanged,
            RateShowCaseLangResourceKind.TwoWayBindingTitle            => en_US.TwoWayBindingTitle,
            RateShowCaseLangResourceKind.TwoWayBindingDescription      => en_US.TwoWayBindingDescription,
            RateShowCaseLangResourceKind.P2ContentSetFourStars         => en_US.P2ContentSetFourStars,
            RateShowCaseLangResourceKind.P2ContentClear                => en_US.P2ContentClear,
            RateShowCaseLangResourceKind.P2TwoWayValueSummaryFormat    => en_US.P2TwoWayValueSummaryFormat,
            RateShowCaseLangResourceKind.TokenNameStarColor            => en_US.TokenNameStarColor,
            RateShowCaseLangResourceKind.TokenNameStarSize             => en_US.TokenNameStarSize,
            RateShowCaseLangResourceKind.TokenNameStarSizeSM           => en_US.TokenNameStarSizeSM,
            RateShowCaseLangResourceKind.TokenNameStarSizeLG           => en_US.TokenNameStarSizeLG,
            RateShowCaseLangResourceKind.TokenNameStarHoverScale       => en_US.TokenNameStarHoverScale,
            RateShowCaseLangResourceKind.TokenNameStarBg               => en_US.TokenNameStarBg,
            RateShowCaseLangResourceKind.TokenScopeComponent           => en_US.TokenScopeComponent,
            RateShowCaseLangResourceKind.TokenStatusStable             => en_US.TokenStatusStable,
            _                                                          => kind.ToString()
        };
    }
}

public sealed record RateApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record RateDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
