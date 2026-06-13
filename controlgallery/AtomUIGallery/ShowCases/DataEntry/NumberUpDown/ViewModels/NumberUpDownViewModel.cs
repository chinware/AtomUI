using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.NumberUpDown;

public class NumberUpDownViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "NumberUpDown";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<NumberUpDownApiRow>? _apiRows;
    private ObservableCollection<NumberUpDownDesignTokenRow>? _designTokenRows;

    public ObservableCollection<NumberUpDownApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<NumberUpDownDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    private string? _stringModeValue = "0.123456789012345678901234";

    public string? StringModeValue
    {
        get => _stringModeValue;
        set => this.RaiseAndSetIfChanged(ref _stringModeValue, value);
    }

    private bool _keyboardEnabled = true;
    public bool KeyboardEnabled
    {
        get => _keyboardEnabled;
        set => this.RaiseAndSetIfChanged(ref _keyboardEnabled, value);
    }

    public NumberUpDownViewModel(IScreen screen)
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
            new NumberUpDownApiRow("Value", Lang(NumberUpDownShowCaseLangResourceKind.ApiPropertyValue), "decimal?", "cyan", "null"),
            new NumberUpDownApiRow("Minimum", Lang(NumberUpDownShowCaseLangResourceKind.ApiPropertyMinimum), "decimal", "cyan", "decimal.MinValue"),
            new NumberUpDownApiRow("Maximum", Lang(NumberUpDownShowCaseLangResourceKind.ApiPropertyMaximum), "decimal", "cyan", "decimal.MaxValue"),
            new NumberUpDownApiRow("Increment", Lang(NumberUpDownShowCaseLangResourceKind.ApiPropertyIncrement), "decimal", "cyan", "1"),
            new NumberUpDownApiRow("FormatString", Lang(NumberUpDownShowCaseLangResourceKind.ApiPropertyFormatString), "string?", "cyan", "null"),
            new NumberUpDownApiRow("IsStringMode", Lang(NumberUpDownShowCaseLangResourceKind.ApiPropertyIsStringMode), "bool", "green", "false"),
            new NumberUpDownApiRow("StringValue", Lang(NumberUpDownShowCaseLangResourceKind.ApiPropertyStringValue), "string?", "cyan", "null"),
            new NumberUpDownApiRow("IsKeyboardEnabled", Lang(NumberUpDownShowCaseLangResourceKind.ApiPropertyIsKeyboardEnabled), "bool", "green", "true"),
            new NumberUpDownApiRow("IsAllowClear", Lang(NumberUpDownShowCaseLangResourceKind.ApiPropertyIsAllowClear), "bool", "green", "false"),
            new NumberUpDownApiRow("ClearIcon", Lang(NumberUpDownShowCaseLangResourceKind.ApiPropertyClearIcon), "PathIcon?", "cyan", "CloseCircleFilled"),
            new NumberUpDownApiRow("SizeType", Lang(NumberUpDownShowCaseLangResourceKind.ApiPropertySizeType), "SizeType", "blue", "Middle"),
            new NumberUpDownApiRow("StyleVariant", Lang(NumberUpDownShowCaseLangResourceKind.ApiPropertyStyleVariant), "InputControlStyleVariant", "purple", "Outlined"),
            new NumberUpDownApiRow("Status", Lang(NumberUpDownShowCaseLangResourceKind.ApiPropertyStatus), "InputControlStatus", "purple", "Default"),
            new NumberUpDownApiRow("LeftAddOn", Lang(NumberUpDownShowCaseLangResourceKind.ApiPropertyLeftAddOn), "object?", "cyan", "null"),
            new NumberUpDownApiRow("RightAddOn", Lang(NumberUpDownShowCaseLangResourceKind.ApiPropertyRightAddOn), "object?", "cyan", "null"),
            new NumberUpDownApiRow("InnerLeftContent", Lang(NumberUpDownShowCaseLangResourceKind.ApiPropertyInnerLeftContent), "object?", "cyan", "null"),
            new NumberUpDownApiRow("InnerRightContent", Lang(NumberUpDownShowCaseLangResourceKind.ApiPropertyInnerRightContent), "object?", "cyan", "null")
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
            new NumberUpDownDesignTokenRow("ControlWidth", Lang(NumberUpDownShowCaseLangResourceKind.TokenNameControlWidth), Lang(NumberUpDownShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(NumberUpDownShowCaseLangResourceKind.TokenStatusStable), "success"),
            new NumberUpDownDesignTokenRow("HandleWidth", Lang(NumberUpDownShowCaseLangResourceKind.TokenNameHandleWidth), Lang(NumberUpDownShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(NumberUpDownShowCaseLangResourceKind.TokenStatusStable), "success"),
            new NumberUpDownDesignTokenRow("HandleIconSize", Lang(NumberUpDownShowCaseLangResourceKind.TokenNameHandleIconSize), Lang(NumberUpDownShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(NumberUpDownShowCaseLangResourceKind.TokenStatusStable), "success"),
            new NumberUpDownDesignTokenRow("HandleBg", Lang(NumberUpDownShowCaseLangResourceKind.TokenNameHandleBg), Lang(NumberUpDownShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(NumberUpDownShowCaseLangResourceKind.TokenStatusStable), "success"),
            new NumberUpDownDesignTokenRow("HandleActiveBg", Lang(NumberUpDownShowCaseLangResourceKind.TokenNameHandleActiveBg), Lang(NumberUpDownShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(NumberUpDownShowCaseLangResourceKind.TokenStatusStable), "success"),
            new NumberUpDownDesignTokenRow("HandleHoverColor", Lang(NumberUpDownShowCaseLangResourceKind.TokenNameHandleHoverColor), Lang(NumberUpDownShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(NumberUpDownShowCaseLangResourceKind.TokenStatusStable), "success"),
            new NumberUpDownDesignTokenRow("HandleBorderColor", Lang(NumberUpDownShowCaseLangResourceKind.TokenNameHandleBorderColor), Lang(NumberUpDownShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(NumberUpDownShowCaseLangResourceKind.TokenStatusStable), "success"),
            new NumberUpDownDesignTokenRow("FilledHandleBg", Lang(NumberUpDownShowCaseLangResourceKind.TokenNameFilledHandleBg), Lang(NumberUpDownShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(NumberUpDownShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(NumberUpDownShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(NumberUpDownShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            NumberUpDownShowCaseLangResourceKind.ApiPropertyValue             => en_US.ApiPropertyValue,
            NumberUpDownShowCaseLangResourceKind.ApiPropertyMinimum           => en_US.ApiPropertyMinimum,
            NumberUpDownShowCaseLangResourceKind.ApiPropertyMaximum           => en_US.ApiPropertyMaximum,
            NumberUpDownShowCaseLangResourceKind.ApiPropertyIncrement         => en_US.ApiPropertyIncrement,
            NumberUpDownShowCaseLangResourceKind.ApiPropertyFormatString      => en_US.ApiPropertyFormatString,
            NumberUpDownShowCaseLangResourceKind.ApiPropertyIsStringMode      => en_US.ApiPropertyIsStringMode,
            NumberUpDownShowCaseLangResourceKind.ApiPropertyStringValue       => en_US.ApiPropertyStringValue,
            NumberUpDownShowCaseLangResourceKind.ApiPropertyIsKeyboardEnabled => en_US.ApiPropertyIsKeyboardEnabled,
            NumberUpDownShowCaseLangResourceKind.ApiPropertyIsAllowClear      => en_US.ApiPropertyIsAllowClear,
            NumberUpDownShowCaseLangResourceKind.ApiPropertyClearIcon         => en_US.ApiPropertyClearIcon,
            NumberUpDownShowCaseLangResourceKind.ApiPropertySizeType          => en_US.ApiPropertySizeType,
            NumberUpDownShowCaseLangResourceKind.ApiPropertyStyleVariant      => en_US.ApiPropertyStyleVariant,
            NumberUpDownShowCaseLangResourceKind.ApiPropertyStatus            => en_US.ApiPropertyStatus,
            NumberUpDownShowCaseLangResourceKind.ApiPropertyLeftAddOn         => en_US.ApiPropertyLeftAddOn,
            NumberUpDownShowCaseLangResourceKind.ApiPropertyRightAddOn        => en_US.ApiPropertyRightAddOn,
            NumberUpDownShowCaseLangResourceKind.ApiPropertyInnerLeftContent  => en_US.ApiPropertyInnerLeftContent,
            NumberUpDownShowCaseLangResourceKind.ApiPropertyInnerRightContent => en_US.ApiPropertyInnerRightContent,
            NumberUpDownShowCaseLangResourceKind.TokenNameControlWidth        => en_US.TokenNameControlWidth,
            NumberUpDownShowCaseLangResourceKind.TokenNameHandleWidth         => en_US.TokenNameHandleWidth,
            NumberUpDownShowCaseLangResourceKind.TokenNameHandleIconSize      => en_US.TokenNameHandleIconSize,
            NumberUpDownShowCaseLangResourceKind.TokenNameHandleBg            => en_US.TokenNameHandleBg,
            NumberUpDownShowCaseLangResourceKind.TokenNameHandleActiveBg      => en_US.TokenNameHandleActiveBg,
            NumberUpDownShowCaseLangResourceKind.TokenNameHandleHoverColor    => en_US.TokenNameHandleHoverColor,
            NumberUpDownShowCaseLangResourceKind.TokenNameHandleBorderColor   => en_US.TokenNameHandleBorderColor,
            NumberUpDownShowCaseLangResourceKind.TokenNameFilledHandleBg      => en_US.TokenNameFilledHandleBg,
            NumberUpDownShowCaseLangResourceKind.TokenScopeComponent          => en_US.TokenScopeComponent,
            NumberUpDownShowCaseLangResourceKind.TokenStatusStable            => en_US.TokenStatusStable,
            _                                                                 => kind.ToString()
        };
    }
}

public sealed record NumberUpDownApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record NumberUpDownDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
