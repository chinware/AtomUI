using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Watermark;

public class WatermarkViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Watermark";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    private ObservableCollection<WatermarkApiRow>? _apiRows;
    private ObservableCollection<WatermarkDesignTokenRow>? _designTokenRows;

    public ObservableCollection<WatermarkApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<WatermarkDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public WatermarkViewModel(IScreen screen)
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
            new WatermarkApiRow("Watermark.Glyph", Lang(WatermarkShowCaseLangResourceKind.ApiPropertyGlyph), "WatermarkGlyph?", "cyan", "null"),
            new WatermarkApiRow("WatermarkGlyph.HorizontalSpace", Lang(WatermarkShowCaseLangResourceKind.ApiPropertyHorizontalSpace), "double", "cyan", "280"),
            new WatermarkApiRow("WatermarkGlyph.VerticalSpace", Lang(WatermarkShowCaseLangResourceKind.ApiPropertyVerticalSpace), "double", "cyan", "40"),
            new WatermarkApiRow("WatermarkGlyph.HorizontalOffset", Lang(WatermarkShowCaseLangResourceKind.ApiPropertyHorizontalOffset), "double", "cyan", "0"),
            new WatermarkApiRow("WatermarkGlyph.VerticalOffset", Lang(WatermarkShowCaseLangResourceKind.ApiPropertyVerticalOffset), "double", "cyan", "0"),
            new WatermarkApiRow("WatermarkGlyph.Rotate", Lang(WatermarkShowCaseLangResourceKind.ApiPropertyRotate), "double", "cyan", "-20"),
            new WatermarkApiRow("WatermarkGlyph.Opacity", Lang(WatermarkShowCaseLangResourceKind.ApiPropertyOpacity), "double", "cyan", "0.3"),
            new WatermarkApiRow("WatermarkGlyph.IsMirrorUsed", Lang(WatermarkShowCaseLangResourceKind.ApiPropertyIsMirrorUsed), "bool", "purple", "false"),
            new WatermarkApiRow("WatermarkGlyph.IsCrossUsed", Lang(WatermarkShowCaseLangResourceKind.ApiPropertyIsCrossUsed), "bool", "purple", "true"),
            new WatermarkApiRow("TextGlyph.Text", Lang(WatermarkShowCaseLangResourceKind.ApiPropertyText), "string?", "cyan", "null"),
            new WatermarkApiRow("TextGlyph.FontSize", Lang(WatermarkShowCaseLangResourceKind.ApiPropertyFontSize), "double", "cyan", "16"),
            new WatermarkApiRow("TextGlyph.Foreground", Lang(WatermarkShowCaseLangResourceKind.ApiPropertyForeground), "IBrush", "cyan", "Black"),
            new WatermarkApiRow("ImageGlyph.Source", Lang(WatermarkShowCaseLangResourceKind.ApiPropertySource), "IImage?", "cyan", "null"),
            new WatermarkApiRow("ImageGlyph.Height", Lang(WatermarkShowCaseLangResourceKind.ApiPropertyHeight), "double", "cyan", "28")
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
            new WatermarkDesignTokenRow("N/A", Lang(WatermarkShowCaseLangResourceKind.TokenNameNoComponentToken), Lang(WatermarkShowCaseLangResourceKind.TokenScopeComponent), "default", Lang(WatermarkShowCaseLangResourceKind.TokenStatusNotApplicable), "default")
        ];
    }

    private static string Lang(WatermarkShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(WatermarkShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            WatermarkShowCaseLangResourceKind.ApiPropertyGlyph            => en_US.ApiPropertyGlyph,
            WatermarkShowCaseLangResourceKind.ApiPropertyHorizontalSpace  => en_US.ApiPropertyHorizontalSpace,
            WatermarkShowCaseLangResourceKind.ApiPropertyVerticalSpace    => en_US.ApiPropertyVerticalSpace,
            WatermarkShowCaseLangResourceKind.ApiPropertyHorizontalOffset => en_US.ApiPropertyHorizontalOffset,
            WatermarkShowCaseLangResourceKind.ApiPropertyVerticalOffset   => en_US.ApiPropertyVerticalOffset,
            WatermarkShowCaseLangResourceKind.ApiPropertyRotate           => en_US.ApiPropertyRotate,
            WatermarkShowCaseLangResourceKind.ApiPropertyOpacity          => en_US.ApiPropertyOpacity,
            WatermarkShowCaseLangResourceKind.ApiPropertyIsMirrorUsed     => en_US.ApiPropertyIsMirrorUsed,
            WatermarkShowCaseLangResourceKind.ApiPropertyIsCrossUsed      => en_US.ApiPropertyIsCrossUsed,
            WatermarkShowCaseLangResourceKind.ApiPropertyText             => en_US.ApiPropertyText,
            WatermarkShowCaseLangResourceKind.ApiPropertyFontSize         => en_US.ApiPropertyFontSize,
            WatermarkShowCaseLangResourceKind.ApiPropertyForeground       => en_US.ApiPropertyForeground,
            WatermarkShowCaseLangResourceKind.ApiPropertySource           => en_US.ApiPropertySource,
            WatermarkShowCaseLangResourceKind.ApiPropertyHeight           => en_US.ApiPropertyHeight,
            WatermarkShowCaseLangResourceKind.TokenNameNoComponentToken   => en_US.TokenNameNoComponentToken,
            WatermarkShowCaseLangResourceKind.TokenScopeComponent         => en_US.TokenScopeComponent,
            WatermarkShowCaseLangResourceKind.TokenStatusNotApplicable    => en_US.TokenStatusNotApplicable,
            _                                                             => kind.ToString()
        };
    }
}

public sealed record WatermarkApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record WatermarkDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
