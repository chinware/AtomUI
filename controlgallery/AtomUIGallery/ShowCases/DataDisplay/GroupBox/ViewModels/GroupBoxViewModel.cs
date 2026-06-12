using System.Collections.ObjectModel;
using AtomUI;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.GroupBox;

public class GroupBoxViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "GroupBox";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    private ObservableCollection<GroupBoxApiRow>? _apiRows;
    private ObservableCollection<GroupBoxDesignTokenRow>? _designTokenRows;

    public ObservableCollection<GroupBoxApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<GroupBoxDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public GroupBoxViewModel(IScreen screen)
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
            new GroupBoxApiRow("HeaderTitle", Lang(GroupBoxShowCaseLangResourceKind.ApiPropertyHeaderTitle), "string?", "cyan", "null"),
            new GroupBoxApiRow("HeaderTitleColor", Lang(GroupBoxShowCaseLangResourceKind.ApiPropertyHeaderTitleColor), "IBrush?", "cyan", "ColorText"),
            new GroupBoxApiRow("HeaderIcon", Lang(GroupBoxShowCaseLangResourceKind.ApiPropertyHeaderIcon), "PathIcon?", "cyan", "null"),
            new GroupBoxApiRow("HeaderTitlePosition", Lang(GroupBoxShowCaseLangResourceKind.ApiPropertyHeaderTitlePosition), "GroupBoxTitlePosition", "blue", "Left"),
            new GroupBoxApiRow("HeaderFontSize", Lang(GroupBoxShowCaseLangResourceKind.ApiPropertyHeaderFontSize), "double", "green", "FontSize"),
            new GroupBoxApiRow("HeaderFontStyle", Lang(GroupBoxShowCaseLangResourceKind.ApiPropertyHeaderFontStyle), "FontStyle", "blue", "Normal"),
            new GroupBoxApiRow("HeaderFontWeight", Lang(GroupBoxShowCaseLangResourceKind.ApiPropertyHeaderFontWeight), "FontWeight", "blue", "Normal")
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
            new GroupBoxDesignTokenRow("ContentPadding", Lang(GroupBoxShowCaseLangResourceKind.TokenNameContentPadding), Lang(GroupBoxShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(GroupBoxShowCaseLangResourceKind.TokenStatusStable), "success"),
            new GroupBoxDesignTokenRow("HeaderContainerMargin", Lang(GroupBoxShowCaseLangResourceKind.TokenNameHeaderContainerMargin), Lang(GroupBoxShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(GroupBoxShowCaseLangResourceKind.TokenStatusStable), "success"),
            new GroupBoxDesignTokenRow("HeaderContentPadding", Lang(GroupBoxShowCaseLangResourceKind.TokenNameHeaderContentPadding), Lang(GroupBoxShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(GroupBoxShowCaseLangResourceKind.TokenStatusStable), "success"),
            new GroupBoxDesignTokenRow("HeaderIconMargin", Lang(GroupBoxShowCaseLangResourceKind.TokenNameHeaderIconMargin), Lang(GroupBoxShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(GroupBoxShowCaseLangResourceKind.TokenStatusStable), "success"),
            new GroupBoxDesignTokenRow("OrientationMarginPercent", Lang(GroupBoxShowCaseLangResourceKind.TokenNameOrientationMarginPercent), Lang(GroupBoxShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(GroupBoxShowCaseLangResourceKind.TokenStatusStable), "success"),
            new GroupBoxDesignTokenRow("TextPaddingInline", Lang(GroupBoxShowCaseLangResourceKind.TokenNameTextPaddingInline), Lang(GroupBoxShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(GroupBoxShowCaseLangResourceKind.TokenStatusStable), "success"),
            new GroupBoxDesignTokenRow("VerticalMarginInline", Lang(GroupBoxShowCaseLangResourceKind.TokenNameVerticalMarginInline), Lang(GroupBoxShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(GroupBoxShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(GroupBoxShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(GroupBoxShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            GroupBoxShowCaseLangResourceKind.ApiPropertyHeaderTitle             => en_US.ApiPropertyHeaderTitle,
            GroupBoxShowCaseLangResourceKind.ApiPropertyHeaderTitleColor        => en_US.ApiPropertyHeaderTitleColor,
            GroupBoxShowCaseLangResourceKind.ApiPropertyHeaderIcon              => en_US.ApiPropertyHeaderIcon,
            GroupBoxShowCaseLangResourceKind.ApiPropertyHeaderTitlePosition     => en_US.ApiPropertyHeaderTitlePosition,
            GroupBoxShowCaseLangResourceKind.ApiPropertyHeaderFontSize          => en_US.ApiPropertyHeaderFontSize,
            GroupBoxShowCaseLangResourceKind.ApiPropertyHeaderFontStyle         => en_US.ApiPropertyHeaderFontStyle,
            GroupBoxShowCaseLangResourceKind.ApiPropertyHeaderFontWeight        => en_US.ApiPropertyHeaderFontWeight,
            GroupBoxShowCaseLangResourceKind.TokenNameContentPadding            => en_US.TokenNameContentPadding,
            GroupBoxShowCaseLangResourceKind.TokenNameHeaderContainerMargin     => en_US.TokenNameHeaderContainerMargin,
            GroupBoxShowCaseLangResourceKind.TokenNameHeaderContentPadding      => en_US.TokenNameHeaderContentPadding,
            GroupBoxShowCaseLangResourceKind.TokenNameHeaderIconMargin          => en_US.TokenNameHeaderIconMargin,
            GroupBoxShowCaseLangResourceKind.TokenNameOrientationMarginPercent  => en_US.TokenNameOrientationMarginPercent,
            GroupBoxShowCaseLangResourceKind.TokenNameTextPaddingInline         => en_US.TokenNameTextPaddingInline,
            GroupBoxShowCaseLangResourceKind.TokenNameVerticalMarginInline      => en_US.TokenNameVerticalMarginInline,
            GroupBoxShowCaseLangResourceKind.TokenScopeComponent                => en_US.TokenScopeComponent,
            GroupBoxShowCaseLangResourceKind.TokenStatusStable                  => en_US.TokenStatusStable,
            _                                                                   => kind.ToString()
        };
    }
}

public sealed record GroupBoxApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record GroupBoxDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
