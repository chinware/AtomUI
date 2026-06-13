using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Media;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Card;

public class CardViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Card";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    private IBrush? _borderlessFrameBg;

    public IBrush? BorderlessFrameBg
    {
        get => _borderlessFrameBg;
        set => this.RaiseAndSetIfChanged(ref _borderlessFrameBg, value);
    }

    private bool _isLoading;

    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    private ObservableCollection<CardApiRow>? _apiRows;
    private ObservableCollection<CardDesignTokenRow>? _designTokenRows;

    public ObservableCollection<CardApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<CardDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public CardViewModel(IScreen screen)
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
            new CardApiRow("Extra", Lang(CardShowCaseLangResourceKind.ApiPropertyExtra), "object?", "cyan", "null"),
            new CardApiRow("ExtraTemplate", Lang(CardShowCaseLangResourceKind.ApiPropertyExtraTemplate), "IDataTemplate?", "cyan", "null"),
            new CardApiRow("StyleVariant", Lang(CardShowCaseLangResourceKind.ApiPropertyStyleVariant), "CardStyleVariant", "blue", "Outlined"),
            new CardApiRow("SizeType", Lang(CardShowCaseLangResourceKind.ApiPropertySizeType), "SizeType", "blue", "Middle"),
            new CardApiRow("IsLoading", Lang(CardShowCaseLangResourceKind.ApiPropertyIsLoading), "bool", "purple", "false"),
            new CardApiRow("IsInnerMode", Lang(CardShowCaseLangResourceKind.ApiPropertyIsInnerMode), "bool", "purple", "false"),
            new CardApiRow("IsHoverable", Lang(CardShowCaseLangResourceKind.ApiPropertyIsHoverable), "bool", "purple", "false"),
            new CardApiRow("Cover", Lang(CardShowCaseLangResourceKind.ApiPropertyCover), "object?", "cyan", "null"),
            new CardApiRow("CoverTemplate", Lang(CardShowCaseLangResourceKind.ApiPropertyCoverTemplate), "IDataTemplate?", "cyan", "null"),
            new CardApiRow("Actions", Lang(CardShowCaseLangResourceKind.ApiPropertyActions), "Controls", "cyan", "empty"),
            new CardApiRow("BoxShadow", Lang(CardShowCaseLangResourceKind.ApiPropertyBoxShadow), "BoxShadows", "cyan", "token"),
            new CardApiRow("IsMotionEnabled", Lang(CardShowCaseLangResourceKind.ApiPropertyIsMotionEnabled), "bool", "purple", "true")
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
            new CardDesignTokenRow("HeaderBg", Lang(CardShowCaseLangResourceKind.TokenNameHeaderBg), Lang(CardShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CardShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CardDesignTokenRow("HeaderFontSizeLG", Lang(CardShowCaseLangResourceKind.TokenNameHeaderFontSizeLG), Lang(CardShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CardShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CardDesignTokenRow("HeaderFontSize", Lang(CardShowCaseLangResourceKind.TokenNameHeaderFontSize), Lang(CardShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CardShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CardDesignTokenRow("HeaderFontSizeSM", Lang(CardShowCaseLangResourceKind.TokenNameHeaderFontSizeSM), Lang(CardShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CardShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CardDesignTokenRow("HeaderHeightLG", Lang(CardShowCaseLangResourceKind.TokenNameHeaderHeightLG), Lang(CardShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CardShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CardDesignTokenRow("HeaderHeight", Lang(CardShowCaseLangResourceKind.TokenNameHeaderHeight), Lang(CardShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CardShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CardDesignTokenRow("HeaderHeightSM", Lang(CardShowCaseLangResourceKind.TokenNameHeaderHeightSM), Lang(CardShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CardShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CardDesignTokenRow("BodyPadding", Lang(CardShowCaseLangResourceKind.TokenNameBodyPadding), Lang(CardShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CardShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CardDesignTokenRow("HeaderPadding", Lang(CardShowCaseLangResourceKind.TokenNameHeaderPadding), Lang(CardShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CardShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CardDesignTokenRow("ActionsBg", Lang(CardShowCaseLangResourceKind.TokenNameActionsBg), Lang(CardShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CardShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CardDesignTokenRow("ActionsSpacing", Lang(CardShowCaseLangResourceKind.TokenNameActionsSpacing), Lang(CardShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CardShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CardDesignTokenRow("TabsMarginBottom", Lang(CardShowCaseLangResourceKind.TokenNameTabsMarginBottom), Lang(CardShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CardShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CardDesignTokenRow("ExtraColor", Lang(CardShowCaseLangResourceKind.TokenNameExtraColor), Lang(CardShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CardShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CardDesignTokenRow("CardShadows", Lang(CardShowCaseLangResourceKind.TokenNameCardShadows), Lang(CardShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CardShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CardDesignTokenRow("CardActionsIconSize", Lang(CardShowCaseLangResourceKind.TokenNameCardActionsIconSize), Lang(CardShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CardShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CardDesignTokenRow("CardGridItemShadows", Lang(CardShowCaseLangResourceKind.TokenNameCardGridItemShadows), Lang(CardShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CardShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(CardShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(CardShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            CardShowCaseLangResourceKind.ApiPropertyExtra              => en_US.ApiPropertyExtra,
            CardShowCaseLangResourceKind.ApiPropertyExtraTemplate      => en_US.ApiPropertyExtraTemplate,
            CardShowCaseLangResourceKind.ApiPropertyStyleVariant       => en_US.ApiPropertyStyleVariant,
            CardShowCaseLangResourceKind.ApiPropertySizeType           => en_US.ApiPropertySizeType,
            CardShowCaseLangResourceKind.ApiPropertyIsLoading          => en_US.ApiPropertyIsLoading,
            CardShowCaseLangResourceKind.ApiPropertyIsInnerMode        => en_US.ApiPropertyIsInnerMode,
            CardShowCaseLangResourceKind.ApiPropertyIsHoverable        => en_US.ApiPropertyIsHoverable,
            CardShowCaseLangResourceKind.ApiPropertyCover              => en_US.ApiPropertyCover,
            CardShowCaseLangResourceKind.ApiPropertyCoverTemplate      => en_US.ApiPropertyCoverTemplate,
            CardShowCaseLangResourceKind.ApiPropertyActions            => en_US.ApiPropertyActions,
            CardShowCaseLangResourceKind.ApiPropertyBoxShadow          => en_US.ApiPropertyBoxShadow,
            CardShowCaseLangResourceKind.ApiPropertyIsMotionEnabled    => en_US.ApiPropertyIsMotionEnabled,
            CardShowCaseLangResourceKind.TokenNameHeaderBg             => en_US.TokenNameHeaderBg,
            CardShowCaseLangResourceKind.TokenNameHeaderFontSizeLG     => en_US.TokenNameHeaderFontSizeLG,
            CardShowCaseLangResourceKind.TokenNameHeaderFontSize       => en_US.TokenNameHeaderFontSize,
            CardShowCaseLangResourceKind.TokenNameHeaderFontSizeSM     => en_US.TokenNameHeaderFontSizeSM,
            CardShowCaseLangResourceKind.TokenNameHeaderHeightLG       => en_US.TokenNameHeaderHeightLG,
            CardShowCaseLangResourceKind.TokenNameHeaderHeight         => en_US.TokenNameHeaderHeight,
            CardShowCaseLangResourceKind.TokenNameHeaderHeightSM       => en_US.TokenNameHeaderHeightSM,
            CardShowCaseLangResourceKind.TokenNameBodyPadding          => en_US.TokenNameBodyPadding,
            CardShowCaseLangResourceKind.TokenNameHeaderPadding        => en_US.TokenNameHeaderPadding,
            CardShowCaseLangResourceKind.TokenNameActionsBg            => en_US.TokenNameActionsBg,
            CardShowCaseLangResourceKind.TokenNameActionsSpacing       => en_US.TokenNameActionsSpacing,
            CardShowCaseLangResourceKind.TokenNameTabsMarginBottom     => en_US.TokenNameTabsMarginBottom,
            CardShowCaseLangResourceKind.TokenNameExtraColor           => en_US.TokenNameExtraColor,
            CardShowCaseLangResourceKind.TokenNameCardShadows          => en_US.TokenNameCardShadows,
            CardShowCaseLangResourceKind.TokenNameCardActionsIconSize  => en_US.TokenNameCardActionsIconSize,
            CardShowCaseLangResourceKind.TokenNameCardGridItemShadows  => en_US.TokenNameCardGridItemShadows,
            CardShowCaseLangResourceKind.TokenScopeComponent           => en_US.TokenScopeComponent,
            CardShowCaseLangResourceKind.TokenStatusStable             => en_US.TokenStatusStable,
            _                                                          => kind.ToString()
        };
    }
}

public sealed record CardApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record CardDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
