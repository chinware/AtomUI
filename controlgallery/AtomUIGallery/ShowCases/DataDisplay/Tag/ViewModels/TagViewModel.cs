using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Tag;

public class TagViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Tag";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<TagApiRow>? _apiRows;
    private ObservableCollection<TagDesignTokenRow>? _designTokenRows;

    public ObservableCollection<TagApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<TagDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public TagViewModel(IScreen screen)
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
            new TagApiRow("Text", Lang(TagShowCaseLangResourceKind.ApiPropertyText), "string?", "cyan", "null"),
            new TagApiRow("TagColor", Lang(TagShowCaseLangResourceKind.ApiPropertyTagColor), "string?", "cyan", "null"),
            new TagApiRow("IsClosable", Lang(TagShowCaseLangResourceKind.ApiPropertyIsClosable), "bool", "green", "false"),
            new TagApiRow("IsBordered", Lang(TagShowCaseLangResourceKind.ApiPropertyIsBordered), "bool", "green", "true"),
            new TagApiRow("Icon", Lang(TagShowCaseLangResourceKind.ApiPropertyIcon), "PathIcon?", "cyan", "null"),
            new TagApiRow("CloseIcon", Lang(TagShowCaseLangResourceKind.ApiPropertyCloseIcon), "PathIcon?", "cyan", "null"),
            new TagApiRow("Closed", Lang(TagShowCaseLangResourceKind.ApiEventClosed), "event EventHandler<RoutedEventArgs>?", "purple", "-")
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
            new TagDesignTokenRow("DefaultBg", Lang(TagShowCaseLangResourceKind.TokenNameDefaultBg), Lang(TagShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TagShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TagDesignTokenRow("DefaultColor", Lang(TagShowCaseLangResourceKind.TokenNameDefaultColor), Lang(TagShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TagShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TagDesignTokenRow("TagFontSize", Lang(TagShowCaseLangResourceKind.TokenNameTagFontSize), Lang(TagShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TagShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TagDesignTokenRow("TagLineHeight", Lang(TagShowCaseLangResourceKind.TokenNameTagLineHeight), Lang(TagShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TagShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TagDesignTokenRow("TagIconSize", Lang(TagShowCaseLangResourceKind.TokenNameTagIconSize), Lang(TagShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TagShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TagDesignTokenRow("TagCloseIconSize", Lang(TagShowCaseLangResourceKind.TokenNameTagCloseIconSize), Lang(TagShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TagShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TagDesignTokenRow("TagPadding", Lang(TagShowCaseLangResourceKind.TokenNameTagPadding), Lang(TagShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TagShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TagDesignTokenRow("TagTextPaddingInline", Lang(TagShowCaseLangResourceKind.TokenNameTagTextPaddingInline), Lang(TagShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TagShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TagDesignTokenRow("TagBorderlessBg", Lang(TagShowCaseLangResourceKind.TokenNameTagBorderlessBg), Lang(TagShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TagShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(TagShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(TagShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            TagShowCaseLangResourceKind.ApiPropertyText              => en_US.ApiPropertyText,
            TagShowCaseLangResourceKind.ApiPropertyTagColor          => en_US.ApiPropertyTagColor,
            TagShowCaseLangResourceKind.ApiPropertyIsClosable        => en_US.ApiPropertyIsClosable,
            TagShowCaseLangResourceKind.ApiPropertyIsBordered        => en_US.ApiPropertyIsBordered,
            TagShowCaseLangResourceKind.ApiPropertyIcon              => en_US.ApiPropertyIcon,
            TagShowCaseLangResourceKind.ApiPropertyCloseIcon         => en_US.ApiPropertyCloseIcon,
            TagShowCaseLangResourceKind.ApiEventClosed               => en_US.ApiEventClosed,
            TagShowCaseLangResourceKind.TokenNameDefaultBg           => en_US.TokenNameDefaultBg,
            TagShowCaseLangResourceKind.TokenNameDefaultColor        => en_US.TokenNameDefaultColor,
            TagShowCaseLangResourceKind.TokenNameTagFontSize         => en_US.TokenNameTagFontSize,
            TagShowCaseLangResourceKind.TokenNameTagLineHeight       => en_US.TokenNameTagLineHeight,
            TagShowCaseLangResourceKind.TokenNameTagIconSize         => en_US.TokenNameTagIconSize,
            TagShowCaseLangResourceKind.TokenNameTagCloseIconSize    => en_US.TokenNameTagCloseIconSize,
            TagShowCaseLangResourceKind.TokenNameTagPadding          => en_US.TokenNameTagPadding,
            TagShowCaseLangResourceKind.TokenNameTagTextPaddingInline => en_US.TokenNameTagTextPaddingInline,
            TagShowCaseLangResourceKind.TokenNameTagBorderlessBg     => en_US.TokenNameTagBorderlessBg,
            TagShowCaseLangResourceKind.TokenScopeComponent          => en_US.TokenScopeComponent,
            TagShowCaseLangResourceKind.TokenStatusStable            => en_US.TokenStatusStable,
            _                                                        => kind.ToString()
        };
    }
}

public sealed record TagApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record TagDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
