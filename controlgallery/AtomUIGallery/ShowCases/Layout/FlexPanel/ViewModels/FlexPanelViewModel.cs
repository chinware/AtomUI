using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.FlexPanel;

public class FlexPanelViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "FlexPanelShowCase";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<FlexPanelApiRow>? _apiRows;
    private ObservableCollection<FlexPanelDesignTokenRow>? _designTokenRows;

    public ObservableCollection<FlexPanelApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<FlexPanelDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public FlexPanelViewModel(IScreen screen)
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
            new FlexPanelApiRow("Direction", Lang(FlexPanelShowCaseLangResourceKind.ApiPropertyDirection), "FlexDirection", "cyan", "Row"),
            new FlexPanelApiRow("Wrap", Lang(FlexPanelShowCaseLangResourceKind.ApiPropertyWrap), "FlexWrap", "cyan", "NoWrap"),
            new FlexPanelApiRow("JustifyContent", Lang(FlexPanelShowCaseLangResourceKind.ApiPropertyJustifyContent), "JustifyContent", "cyan", "FlexStart"),
            new FlexPanelApiRow("AlignItems", Lang(FlexPanelShowCaseLangResourceKind.ApiPropertyAlignItems), "AlignItems", "cyan", "Stretch"),
            new FlexPanelApiRow("AlignContent", Lang(FlexPanelShowCaseLangResourceKind.ApiPropertyAlignContent), "AlignContent", "cyan", "Stretch"),
            new FlexPanelApiRow("ColumnSpacing", Lang(FlexPanelShowCaseLangResourceKind.ApiPropertyColumnSpacing), "double", "cyan", "0"),
            new FlexPanelApiRow("RowSpacing", Lang(FlexPanelShowCaseLangResourceKind.ApiPropertyRowSpacing), "double", "cyan", "0"),
            new FlexPanelApiRow("Flex.Grow", Lang(FlexPanelShowCaseLangResourceKind.ApiPropertyFlexGrow), "double", "cyan", "0"),
            new FlexPanelApiRow("Flex.Shrink", Lang(FlexPanelShowCaseLangResourceKind.ApiPropertyFlexShrink), "double", "cyan", "1"),
            new FlexPanelApiRow("Flex.Basis", Lang(FlexPanelShowCaseLangResourceKind.ApiPropertyFlexBasis), "FlexBasis", "cyan", "Auto"),
            new FlexPanelApiRow("Flex.Order", Lang(FlexPanelShowCaseLangResourceKind.ApiPropertyFlexOrder), "int", "orange", "0"),
            new FlexPanelApiRow("Flex.AlignSelf", Lang(FlexPanelShowCaseLangResourceKind.ApiPropertyFlexAlignSelf), "AlignItems?", "cyan", "null")
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
            new FlexPanelDesignTokenRow("N/A", Lang(FlexPanelShowCaseLangResourceKind.TokenNameNoComponentToken), Lang(FlexPanelShowCaseLangResourceKind.TokenScopeComponent), "default", Lang(FlexPanelShowCaseLangResourceKind.TokenStatusNotApplicable), "default")
        ];
    }

    private static string Lang(FlexPanelShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(FlexPanelShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            FlexPanelShowCaseLangResourceKind.ApiPropertyDirection      => en_US.ApiPropertyDirection,
            FlexPanelShowCaseLangResourceKind.ApiPropertyWrap           => en_US.ApiPropertyWrap,
            FlexPanelShowCaseLangResourceKind.ApiPropertyJustifyContent => en_US.ApiPropertyJustifyContent,
            FlexPanelShowCaseLangResourceKind.ApiPropertyAlignItems     => en_US.ApiPropertyAlignItems,
            FlexPanelShowCaseLangResourceKind.ApiPropertyAlignContent   => en_US.ApiPropertyAlignContent,
            FlexPanelShowCaseLangResourceKind.ApiPropertyColumnSpacing  => en_US.ApiPropertyColumnSpacing,
            FlexPanelShowCaseLangResourceKind.ApiPropertyRowSpacing     => en_US.ApiPropertyRowSpacing,
            FlexPanelShowCaseLangResourceKind.ApiPropertyFlexGrow       => en_US.ApiPropertyFlexGrow,
            FlexPanelShowCaseLangResourceKind.ApiPropertyFlexShrink     => en_US.ApiPropertyFlexShrink,
            FlexPanelShowCaseLangResourceKind.ApiPropertyFlexBasis      => en_US.ApiPropertyFlexBasis,
            FlexPanelShowCaseLangResourceKind.ApiPropertyFlexOrder      => en_US.ApiPropertyFlexOrder,
            FlexPanelShowCaseLangResourceKind.ApiPropertyFlexAlignSelf  => en_US.ApiPropertyFlexAlignSelf,
            FlexPanelShowCaseLangResourceKind.TokenNameNoComponentToken => en_US.TokenNameNoComponentToken,
            FlexPanelShowCaseLangResourceKind.TokenScopeComponent       => en_US.TokenScopeComponent,
            FlexPanelShowCaseLangResourceKind.TokenStatusNotApplicable  => en_US.TokenStatusNotApplicable,
            _                                                           => kind.ToString()
        };
    }
}

public sealed record FlexPanelApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record FlexPanelDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
