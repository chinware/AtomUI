using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Separator;

public class SeparatorViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Separator";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<SeparatorApiRow>? _apiRows;
    private ObservableCollection<SeparatorDesignTokenRow>? _designTokenRows;

    public ObservableCollection<SeparatorApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<SeparatorDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public SeparatorViewModel(IScreen screen)
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
            new SeparatorApiRow("Separator.Title", Lang(SeparatorShowCaseLangResourceKind.ApiPropertyTitle), "string?", "cyan", "null"),
            new SeparatorApiRow("Separator.TitlePosition", Lang(SeparatorShowCaseLangResourceKind.ApiPropertyTitlePosition), "SeparatorTitlePosition", "blue", "Center"),
            new SeparatorApiRow("Separator.TitleColor", Lang(SeparatorShowCaseLangResourceKind.ApiPropertyTitleColor), "IBrush?", "cyan", "token"),
            new SeparatorApiRow("Separator.LineColor", Lang(SeparatorShowCaseLangResourceKind.ApiPropertyLineColor), "IBrush?", "cyan", "token"),
            new SeparatorApiRow("Separator.Orientation", Lang(SeparatorShowCaseLangResourceKind.ApiPropertyOrientation), "Orientation", "blue", "Horizontal"),
            new SeparatorApiRow("Separator.OrientationMargin", Lang(SeparatorShowCaseLangResourceKind.ApiPropertyOrientationMargin), "double", "cyan", "NaN"),
            new SeparatorApiRow("Separator.Variant", Lang(SeparatorShowCaseLangResourceKind.ApiPropertyVariant), "SeparatorVariant", "blue", "Solid"),
            new SeparatorApiRow("Separator.LineWidth", Lang(SeparatorShowCaseLangResourceKind.ApiPropertyLineWidth), "double", "cyan", "1"),
            new SeparatorApiRow("Separator.IsPlain", Lang(SeparatorShowCaseLangResourceKind.ApiPropertyIsPlain), "bool", "purple", "false"),
            new SeparatorApiRow("Separator.SizeType", Lang(SeparatorShowCaseLangResourceKind.ApiPropertySizeType), "SizeType", "blue", "Middle"),
            new SeparatorApiRow("VerticalSeparator.Orientation", Lang(SeparatorShowCaseLangResourceKind.ApiPropertyVerticalSeparatorOrientation), "Orientation", "blue", "Vertical")
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
            new SeparatorDesignTokenRow("TextPaddingInline", Lang(SeparatorShowCaseLangResourceKind.TokenNameTextPaddingInline), Lang(SeparatorShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SeparatorShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SeparatorDesignTokenRow("OrientationMarginPercent", Lang(SeparatorShowCaseLangResourceKind.TokenNameOrientationMarginPercent), Lang(SeparatorShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SeparatorShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SeparatorDesignTokenRow("VerticalMarginInline", Lang(SeparatorShowCaseLangResourceKind.TokenNameVerticalMarginInline), Lang(SeparatorShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SeparatorShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SeparatorDesignTokenRow("HorizontalMarginBlockSM", Lang(SeparatorShowCaseLangResourceKind.TokenNameHorizontalMarginBlockSM), Lang(SeparatorShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SeparatorShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SeparatorDesignTokenRow("HorizontalMarginBlock", Lang(SeparatorShowCaseLangResourceKind.TokenNameHorizontalMarginBlock), Lang(SeparatorShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SeparatorShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SeparatorDesignTokenRow("HorizontalMarginBlockLG", Lang(SeparatorShowCaseLangResourceKind.TokenNameHorizontalMarginBlockLG), Lang(SeparatorShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SeparatorShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SeparatorDesignTokenRow("HorizontalWithTextGutterMargin", Lang(SeparatorShowCaseLangResourceKind.TokenNameHorizontalWithTextGutterMargin), Lang(SeparatorShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SeparatorShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(SeparatorShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(SeparatorShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            SeparatorShowCaseLangResourceKind.ApiPropertyTitle                        => en_US.ApiPropertyTitle,
            SeparatorShowCaseLangResourceKind.ApiPropertyTitlePosition                => en_US.ApiPropertyTitlePosition,
            SeparatorShowCaseLangResourceKind.ApiPropertyTitleColor                   => en_US.ApiPropertyTitleColor,
            SeparatorShowCaseLangResourceKind.ApiPropertyLineColor                    => en_US.ApiPropertyLineColor,
            SeparatorShowCaseLangResourceKind.ApiPropertyOrientation                  => en_US.ApiPropertyOrientation,
            SeparatorShowCaseLangResourceKind.ApiPropertyOrientationMargin            => en_US.ApiPropertyOrientationMargin,
            SeparatorShowCaseLangResourceKind.ApiPropertyVariant                      => en_US.ApiPropertyVariant,
            SeparatorShowCaseLangResourceKind.ApiPropertyLineWidth                    => en_US.ApiPropertyLineWidth,
            SeparatorShowCaseLangResourceKind.ApiPropertyIsPlain                      => en_US.ApiPropertyIsPlain,
            SeparatorShowCaseLangResourceKind.ApiPropertySizeType                     => en_US.ApiPropertySizeType,
            SeparatorShowCaseLangResourceKind.ApiPropertyVerticalSeparatorOrientation => en_US.ApiPropertyVerticalSeparatorOrientation,
            SeparatorShowCaseLangResourceKind.TokenNameTextPaddingInline              => en_US.TokenNameTextPaddingInline,
            SeparatorShowCaseLangResourceKind.TokenNameOrientationMarginPercent       => en_US.TokenNameOrientationMarginPercent,
            SeparatorShowCaseLangResourceKind.TokenNameVerticalMarginInline           => en_US.TokenNameVerticalMarginInline,
            SeparatorShowCaseLangResourceKind.TokenNameHorizontalMarginBlockSM        => en_US.TokenNameHorizontalMarginBlockSM,
            SeparatorShowCaseLangResourceKind.TokenNameHorizontalMarginBlock          => en_US.TokenNameHorizontalMarginBlock,
            SeparatorShowCaseLangResourceKind.TokenNameHorizontalMarginBlockLG        => en_US.TokenNameHorizontalMarginBlockLG,
            SeparatorShowCaseLangResourceKind.TokenNameHorizontalWithTextGutterMargin => en_US.TokenNameHorizontalWithTextGutterMargin,
            SeparatorShowCaseLangResourceKind.TokenScopeComponent                     => en_US.TokenScopeComponent,
            SeparatorShowCaseLangResourceKind.TokenStatusStable                       => en_US.TokenStatusStable,
            _                                                                         => kind.ToString()
        };
    }
}

public sealed record SeparatorApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record SeparatorDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
