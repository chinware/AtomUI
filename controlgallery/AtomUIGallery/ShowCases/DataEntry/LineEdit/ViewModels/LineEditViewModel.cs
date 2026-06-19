using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.LineEdit;

public class LineEditViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "LineEdit";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<LineEditApiRow>? _apiRows;
    private ObservableCollection<LineEditDesignTokenRow>? _designTokenRows;

    public ObservableCollection<LineEditApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<LineEditDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public LineEditViewModel(IScreen screen)
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
            new LineEditApiRow("PlaceholderText", Lang(LineEditShowCaseLangResourceKind.ApiPropertyPlaceholderText), "string?", "cyan", "null"),
            new LineEditApiRow("Text", Lang(LineEditShowCaseLangResourceKind.ApiPropertyText), "string?", "cyan", "null"),
            new LineEditApiRow("IsAllowClear", Lang(LineEditShowCaseLangResourceKind.ApiPropertyIsAllowClear), "bool", "green", "false"),
            new LineEditApiRow("SizeType", Lang(LineEditShowCaseLangResourceKind.ApiPropertySizeType), "CustomizableSizeType", "blue", "Middle"),
            new LineEditApiRow("StyleVariant", Lang(LineEditShowCaseLangResourceKind.ApiPropertyStyleVariant), "InputControlStyleVariant", "purple", "Outlined"),
            new LineEditApiRow("Status", Lang(LineEditShowCaseLangResourceKind.ApiPropertyStatus), "InputControlStatus", "purple", "Default"),
            new LineEditApiRow("LeftAddOn", Lang(LineEditShowCaseLangResourceKind.ApiPropertyLeftAddOn), "object?", "cyan", "null"),
            new LineEditApiRow("RightAddOn", Lang(LineEditShowCaseLangResourceKind.ApiPropertyRightAddOn), "object?", "cyan", "null"),
            new LineEditApiRow("InnerLeftContent", Lang(LineEditShowCaseLangResourceKind.ApiPropertyInnerLeftContent), "object?", "cyan", "null"),
            new LineEditApiRow("InnerRightContent", Lang(LineEditShowCaseLangResourceKind.ApiPropertyInnerRightContent), "object?", "cyan", "null"),
            new LineEditApiRow("RevealPassword", Lang(LineEditShowCaseLangResourceKind.ApiPropertyRevealPassword), "bool", "green", "false"),
            new LineEditApiRow("SearchEdit.SearchButtonStyle", Lang(LineEditShowCaseLangResourceKind.ApiPropertySearchButtonStyle), "SearchEditButtonStyle", "purple", "Default"),
            new LineEditApiRow("SearchEdit.SearchButtonText", Lang(LineEditShowCaseLangResourceKind.ApiPropertySearchButtonText), "string?", "cyan", "null"),
            new LineEditApiRow("SearchEdit.IsOperating", Lang(LineEditShowCaseLangResourceKind.ApiPropertyIsOperating), "bool", "green", "false"),
            new LineEditApiRow("TextArea.Lines", Lang(LineEditShowCaseLangResourceKind.ApiPropertyLines), "int", "green", "2"),
            new LineEditApiRow("TextArea.IsAutoSize", Lang(LineEditShowCaseLangResourceKind.ApiPropertyIsAutoSize), "bool", "green", "false"),
            new LineEditApiRow("IsShowCount", Lang(LineEditShowCaseLangResourceKind.ApiPropertyIsShowCount), "bool", "green", "false"),
            new LineEditApiRow("TextArea.IsResizable", Lang(LineEditShowCaseLangResourceKind.ApiPropertyIsResizable), "bool", "green", "false")
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
            new LineEditDesignTokenRow("LineEdit.InputFontSize", Lang(LineEditShowCaseLangResourceKind.TokenNameInputFontSize), Lang(LineEditShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(LineEditShowCaseLangResourceKind.TokenStatusStable), "success"),
            new LineEditDesignTokenRow("LineEdit.InputFontSizeLG", Lang(LineEditShowCaseLangResourceKind.TokenNameInputFontSizeLG), Lang(LineEditShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(LineEditShowCaseLangResourceKind.TokenStatusStable), "success"),
            new LineEditDesignTokenRow("LineEdit.InputFontSizeSM", Lang(LineEditShowCaseLangResourceKind.TokenNameInputFontSizeSM), Lang(LineEditShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(LineEditShowCaseLangResourceKind.TokenStatusStable), "success"),
            new LineEditDesignTokenRow("TextArea.FontSize", Lang(LineEditShowCaseLangResourceKind.TokenNameFontSize), Lang(LineEditShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(LineEditShowCaseLangResourceKind.TokenStatusStable), "success"),
            new LineEditDesignTokenRow("TextArea.FontSizeLG", Lang(LineEditShowCaseLangResourceKind.TokenNameFontSizeLG), Lang(LineEditShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(LineEditShowCaseLangResourceKind.TokenStatusStable), "success"),
            new LineEditDesignTokenRow("TextArea.FontSizeSM", Lang(LineEditShowCaseLangResourceKind.TokenNameFontSizeSM), Lang(LineEditShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(LineEditShowCaseLangResourceKind.TokenStatusStable), "success"),
            new LineEditDesignTokenRow("TextArea.ResizeHandleSize", Lang(LineEditShowCaseLangResourceKind.TokenNameResizeHandleSize), Lang(LineEditShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(LineEditShowCaseLangResourceKind.TokenStatusStable), "success"),
            new LineEditDesignTokenRow("TextArea.ResizeIndicatorLineColor", Lang(LineEditShowCaseLangResourceKind.TokenNameResizeIndicatorLineColor), Lang(LineEditShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(LineEditShowCaseLangResourceKind.TokenStatusStable), "success"),
            new LineEditDesignTokenRow("TextArea.RightAddOnPadding", Lang(LineEditShowCaseLangResourceKind.TokenNameRightAddOnPadding), Lang(LineEditShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(LineEditShowCaseLangResourceKind.TokenStatusStable), "success"),
            new LineEditDesignTokenRow("TextArea.RightAddOnPaddingSM", Lang(LineEditShowCaseLangResourceKind.TokenNameRightAddOnPaddingSM), Lang(LineEditShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(LineEditShowCaseLangResourceKind.TokenStatusStable), "success"),
            new LineEditDesignTokenRow("TextArea.RightAddOnPaddingLG", Lang(LineEditShowCaseLangResourceKind.TokenNameRightAddOnPaddingLG), Lang(LineEditShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(LineEditShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(LineEditShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(LineEditShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            LineEditShowCaseLangResourceKind.ApiPropertyPlaceholderText          => en_US.ApiPropertyPlaceholderText,
            LineEditShowCaseLangResourceKind.ApiPropertyText                     => en_US.ApiPropertyText,
            LineEditShowCaseLangResourceKind.ApiPropertyIsAllowClear             => en_US.ApiPropertyIsAllowClear,
            LineEditShowCaseLangResourceKind.ApiPropertySizeType                 => en_US.ApiPropertySizeType,
            LineEditShowCaseLangResourceKind.ApiPropertyStyleVariant             => en_US.ApiPropertyStyleVariant,
            LineEditShowCaseLangResourceKind.ApiPropertyStatus                   => en_US.ApiPropertyStatus,
            LineEditShowCaseLangResourceKind.ApiPropertyLeftAddOn                => en_US.ApiPropertyLeftAddOn,
            LineEditShowCaseLangResourceKind.ApiPropertyRightAddOn               => en_US.ApiPropertyRightAddOn,
            LineEditShowCaseLangResourceKind.ApiPropertyInnerLeftContent         => en_US.ApiPropertyInnerLeftContent,
            LineEditShowCaseLangResourceKind.ApiPropertyInnerRightContent        => en_US.ApiPropertyInnerRightContent,
            LineEditShowCaseLangResourceKind.ApiPropertyRevealPassword           => en_US.ApiPropertyRevealPassword,
            LineEditShowCaseLangResourceKind.ApiPropertySearchButtonStyle        => en_US.ApiPropertySearchButtonStyle,
            LineEditShowCaseLangResourceKind.ApiPropertySearchButtonText         => en_US.ApiPropertySearchButtonText,
            LineEditShowCaseLangResourceKind.ApiPropertyIsOperating              => en_US.ApiPropertyIsOperating,
            LineEditShowCaseLangResourceKind.ApiPropertyLines                    => en_US.ApiPropertyLines,
            LineEditShowCaseLangResourceKind.ApiPropertyIsAutoSize               => en_US.ApiPropertyIsAutoSize,
            LineEditShowCaseLangResourceKind.ApiPropertyIsShowCount              => en_US.ApiPropertyIsShowCount,
            LineEditShowCaseLangResourceKind.ApiPropertyIsResizable              => en_US.ApiPropertyIsResizable,
            LineEditShowCaseLangResourceKind.TokenNameInputFontSize              => en_US.TokenNameInputFontSize,
            LineEditShowCaseLangResourceKind.TokenNameInputFontSizeLG            => en_US.TokenNameInputFontSizeLG,
            LineEditShowCaseLangResourceKind.TokenNameInputFontSizeSM            => en_US.TokenNameInputFontSizeSM,
            LineEditShowCaseLangResourceKind.TokenNameFontSize                   => en_US.TokenNameFontSize,
            LineEditShowCaseLangResourceKind.TokenNameFontSizeLG                 => en_US.TokenNameFontSizeLG,
            LineEditShowCaseLangResourceKind.TokenNameFontSizeSM                 => en_US.TokenNameFontSizeSM,
            LineEditShowCaseLangResourceKind.TokenNameResizeHandleSize           => en_US.TokenNameResizeHandleSize,
            LineEditShowCaseLangResourceKind.TokenNameResizeIndicatorLineColor   => en_US.TokenNameResizeIndicatorLineColor,
            LineEditShowCaseLangResourceKind.TokenNameRightAddOnPadding          => en_US.TokenNameRightAddOnPadding,
            LineEditShowCaseLangResourceKind.TokenNameRightAddOnPaddingSM        => en_US.TokenNameRightAddOnPaddingSM,
            LineEditShowCaseLangResourceKind.TokenNameRightAddOnPaddingLG        => en_US.TokenNameRightAddOnPaddingLG,
            LineEditShowCaseLangResourceKind.TokenScopeComponent                 => en_US.TokenScopeComponent,
            LineEditShowCaseLangResourceKind.TokenStatusStable                   => en_US.TokenStatusStable,
            _                                                                    => kind.ToString()
        };
    }
}

public sealed record LineEditApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record LineEditDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
