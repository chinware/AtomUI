using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Segmented;

public class SegmentedViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Segmented";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<SegmentedApiRow>? _apiRows;
    private ObservableCollection<SegmentedDesignTokenRow>? _designTokenRows;

    public ObservableCollection<SegmentedApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<SegmentedDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public SegmentedViewModel(IScreen screen)
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
            new SegmentedApiRow("SizeType", Lang(SegmentedShowCaseLangResourceKind.ApiPropertySizeType), "CustomizableSizeType", "blue", "Middle"),
            new SegmentedApiRow("IsExpanding", Lang(SegmentedShowCaseLangResourceKind.ApiPropertyIsExpanding), "bool", "green", "false"),
            new SegmentedApiRow("IsMotionEnabled", Lang(SegmentedShowCaseLangResourceKind.ApiPropertyIsMotionEnabled), "bool", "green", "EnableMotion"),
            new SegmentedApiRow("SelectedIndex", Lang(SegmentedShowCaseLangResourceKind.ApiPropertySelectedIndex), "int", "green", "0"),
            new SegmentedApiRow("SelectedItem", Lang(SegmentedShowCaseLangResourceKind.ApiPropertySelectedItem), "object?", "cyan", "first item"),
            new SegmentedApiRow("SelectionChanged", Lang(SegmentedShowCaseLangResourceKind.ApiEventSelectionChanged), "event", "purple", "-"),
            new SegmentedApiRow("SegmentedItem.Icon", Lang(SegmentedShowCaseLangResourceKind.ApiPropertyIcon), "PathIcon?", "cyan", "null"),
            new SegmentedApiRow("SegmentedItem.IsSelected", Lang(SegmentedShowCaseLangResourceKind.ApiPropertyIsSelected), "bool", "green", "false")
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
            new SegmentedDesignTokenRow("TrackPadding", Lang(SegmentedShowCaseLangResourceKind.TokenNameTrackPadding), Lang(SegmentedShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SegmentedShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SegmentedDesignTokenRow("TrackBg", Lang(SegmentedShowCaseLangResourceKind.TokenNameTrackBg), Lang(SegmentedShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SegmentedShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SegmentedDesignTokenRow("ItemColor", Lang(SegmentedShowCaseLangResourceKind.TokenNameItemColor), Lang(SegmentedShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SegmentedShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SegmentedDesignTokenRow("ItemHoverColor", Lang(SegmentedShowCaseLangResourceKind.TokenNameItemHoverColor), Lang(SegmentedShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SegmentedShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SegmentedDesignTokenRow("ItemHoverBg", Lang(SegmentedShowCaseLangResourceKind.TokenNameItemHoverBg), Lang(SegmentedShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SegmentedShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SegmentedDesignTokenRow("ItemActiveBg", Lang(SegmentedShowCaseLangResourceKind.TokenNameItemActiveBg), Lang(SegmentedShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SegmentedShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SegmentedDesignTokenRow("ItemSelectedBg", Lang(SegmentedShowCaseLangResourceKind.TokenNameItemSelectedBg), Lang(SegmentedShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SegmentedShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SegmentedDesignTokenRow("ItemSelectedColor", Lang(SegmentedShowCaseLangResourceKind.TokenNameItemSelectedColor), Lang(SegmentedShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SegmentedShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SegmentedDesignTokenRow("ItemMinHeightLG", Lang(SegmentedShowCaseLangResourceKind.TokenNameItemMinHeightLG), Lang(SegmentedShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SegmentedShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SegmentedDesignTokenRow("ItemMinHeight", Lang(SegmentedShowCaseLangResourceKind.TokenNameItemMinHeight), Lang(SegmentedShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SegmentedShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SegmentedDesignTokenRow("ItemMinHeightSM", Lang(SegmentedShowCaseLangResourceKind.TokenNameItemMinHeightSM), Lang(SegmentedShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SegmentedShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(SegmentedShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(SegmentedShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            SegmentedShowCaseLangResourceKind.ApiPropertySizeType          => en_US.ApiPropertySizeType,
            SegmentedShowCaseLangResourceKind.ApiPropertyIsExpanding       => en_US.ApiPropertyIsExpanding,
            SegmentedShowCaseLangResourceKind.ApiPropertyIsMotionEnabled   => en_US.ApiPropertyIsMotionEnabled,
            SegmentedShowCaseLangResourceKind.ApiPropertySelectedIndex     => en_US.ApiPropertySelectedIndex,
            SegmentedShowCaseLangResourceKind.ApiPropertySelectedItem      => en_US.ApiPropertySelectedItem,
            SegmentedShowCaseLangResourceKind.ApiEventSelectionChanged     => en_US.ApiEventSelectionChanged,
            SegmentedShowCaseLangResourceKind.ApiPropertyIcon              => en_US.ApiPropertyIcon,
            SegmentedShowCaseLangResourceKind.ApiPropertyIsSelected        => en_US.ApiPropertyIsSelected,
            SegmentedShowCaseLangResourceKind.TokenNameTrackPadding        => en_US.TokenNameTrackPadding,
            SegmentedShowCaseLangResourceKind.TokenNameTrackBg             => en_US.TokenNameTrackBg,
            SegmentedShowCaseLangResourceKind.TokenNameItemColor           => en_US.TokenNameItemColor,
            SegmentedShowCaseLangResourceKind.TokenNameItemHoverColor      => en_US.TokenNameItemHoverColor,
            SegmentedShowCaseLangResourceKind.TokenNameItemHoverBg         => en_US.TokenNameItemHoverBg,
            SegmentedShowCaseLangResourceKind.TokenNameItemActiveBg        => en_US.TokenNameItemActiveBg,
            SegmentedShowCaseLangResourceKind.TokenNameItemSelectedBg      => en_US.TokenNameItemSelectedBg,
            SegmentedShowCaseLangResourceKind.TokenNameItemSelectedColor   => en_US.TokenNameItemSelectedColor,
            SegmentedShowCaseLangResourceKind.TokenNameItemMinHeightLG     => en_US.TokenNameItemMinHeightLG,
            SegmentedShowCaseLangResourceKind.TokenNameItemMinHeight       => en_US.TokenNameItemMinHeight,
            SegmentedShowCaseLangResourceKind.TokenNameItemMinHeightSM     => en_US.TokenNameItemMinHeightSM,
            SegmentedShowCaseLangResourceKind.TokenScopeComponent          => en_US.TokenScopeComponent,
            SegmentedShowCaseLangResourceKind.TokenStatusStable            => en_US.TokenStatusStable,
            _                                                              => kind.ToString()
        };
    }
}

public sealed record SegmentedApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record SegmentedDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
