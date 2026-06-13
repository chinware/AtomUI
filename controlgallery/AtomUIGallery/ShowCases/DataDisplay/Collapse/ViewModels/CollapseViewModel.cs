using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Collapse;

public class CollapseViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Collapse";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    private CollapseExpandIconPosition _collapseExpandIconPosition;

    public CollapseExpandIconPosition CollapseExpandIconPosition
    {
        get => _collapseExpandIconPosition;
        set => this.RaiseAndSetIfChanged(ref _collapseExpandIconPosition, value);
    }

    private ObservableCollection<CollapseApiRow>? _apiRows;
    private ObservableCollection<CollapseDesignTokenRow>? _designTokenRows;

    public ObservableCollection<CollapseApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<CollapseDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public CollapseViewModel(IScreen screen)
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
            new CollapseApiRow("SizeType", Lang(CollapseShowCaseLangResourceKind.ApiPropertySizeType), "SizeType", "blue", "Middle"),
            new CollapseApiRow("IsGhostStyle", Lang(CollapseShowCaseLangResourceKind.ApiPropertyIsGhostStyle), "bool", "purple", "false"),
            new CollapseApiRow("IsBorderless", Lang(CollapseShowCaseLangResourceKind.ApiPropertyIsBorderless), "bool", "purple", "false"),
            new CollapseApiRow("IsAccordion", Lang(CollapseShowCaseLangResourceKind.ApiPropertyIsAccordion), "bool", "purple", "false"),
            new CollapseApiRow("TriggerType", Lang(CollapseShowCaseLangResourceKind.ApiPropertyTriggerType), "CollapseTriggerType", "blue", "Header"),
            new CollapseApiRow("ExpandIconPosition", Lang(CollapseShowCaseLangResourceKind.ApiPropertyExpandIconPosition), "CollapseExpandIconPosition", "blue", "Start"),
            new CollapseApiRow("IsMotionEnabled", Lang(CollapseShowCaseLangResourceKind.ApiPropertyIsMotionEnabled), "bool", "purple", "true"),
            new CollapseApiRow("ItemHeaderPadding", Lang(CollapseShowCaseLangResourceKind.ApiPropertyItemHeaderPadding), "Thickness", "cyan", "token"),
            new CollapseApiRow("ItemContentPadding", Lang(CollapseShowCaseLangResourceKind.ApiPropertyItemContentPadding), "Thickness", "cyan", "token"),
            new CollapseApiRow("CollapseItem.IsSelected", Lang(CollapseShowCaseLangResourceKind.ApiPropertyIsSelected), "bool", "purple", "false"),
            new CollapseApiRow("CollapseItem.IsShowExpandIcon", Lang(CollapseShowCaseLangResourceKind.ApiPropertyIsShowExpandIcon), "bool", "purple", "true"),
            new CollapseApiRow("CollapseItem.ExpandIcon", Lang(CollapseShowCaseLangResourceKind.ApiPropertyExpandIcon), "PathIcon?", "cyan", "RightOutlined"),
            new CollapseApiRow("CollapseItem.AddOnContent", Lang(CollapseShowCaseLangResourceKind.ApiPropertyAddOnContent), "object?", "cyan", "null"),
            new CollapseApiRow("CollapseItem.AddOnContentTemplate", Lang(CollapseShowCaseLangResourceKind.ApiPropertyAddOnContentTemplate), "IDataTemplate?", "cyan", "null"),
            new CollapseApiRow("CollapseItem.HeaderPadding", Lang(CollapseShowCaseLangResourceKind.ApiPropertyHeaderPadding), "Thickness", "cyan", "token"),
            new CollapseApiRow("CollapseItem.ContentPadding", Lang(CollapseShowCaseLangResourceKind.ApiPropertyContentPadding), "Thickness", "cyan", "token")
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
            new CollapseDesignTokenRow("HeaderPadding", Lang(CollapseShowCaseLangResourceKind.TokenNameHeaderPadding), Lang(CollapseShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CollapseShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CollapseDesignTokenRow("HeaderBg", Lang(CollapseShowCaseLangResourceKind.TokenNameHeaderBg), Lang(CollapseShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CollapseShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CollapseDesignTokenRow("ContentPadding", Lang(CollapseShowCaseLangResourceKind.TokenNameContentPadding), Lang(CollapseShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CollapseShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CollapseDesignTokenRow("ContentBg", Lang(CollapseShowCaseLangResourceKind.TokenNameContentBg), Lang(CollapseShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CollapseShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CollapseDesignTokenRow("CollapseHeaderPaddingSM", Lang(CollapseShowCaseLangResourceKind.TokenNameCollapseHeaderPaddingSM), Lang(CollapseShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CollapseShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CollapseDesignTokenRow("CollapseHeaderPaddingLG", Lang(CollapseShowCaseLangResourceKind.TokenNameCollapseHeaderPaddingLG), Lang(CollapseShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CollapseShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CollapseDesignTokenRow("CollapseContentPaddingSM", Lang(CollapseShowCaseLangResourceKind.TokenNameCollapseContentPaddingSM), Lang(CollapseShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CollapseShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CollapseDesignTokenRow("CollapseContentPaddingLG", Lang(CollapseShowCaseLangResourceKind.TokenNameCollapseContentPaddingLG), Lang(CollapseShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CollapseShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CollapseDesignTokenRow("CollapsePanelBorderRadius", Lang(CollapseShowCaseLangResourceKind.TokenNameCollapsePanelBorderRadius), Lang(CollapseShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CollapseShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CollapseDesignTokenRow("LeftExpandButtonMargin", Lang(CollapseShowCaseLangResourceKind.TokenNameLeftExpandButtonMargin), Lang(CollapseShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CollapseShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CollapseDesignTokenRow("RightExpandButtonMargin", Lang(CollapseShowCaseLangResourceKind.TokenNameRightExpandButtonMargin), Lang(CollapseShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CollapseShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(CollapseShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(CollapseShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            CollapseShowCaseLangResourceKind.ApiPropertySizeType                 => en_US.ApiPropertySizeType,
            CollapseShowCaseLangResourceKind.ApiPropertyIsGhostStyle             => en_US.ApiPropertyIsGhostStyle,
            CollapseShowCaseLangResourceKind.ApiPropertyIsBorderless             => en_US.ApiPropertyIsBorderless,
            CollapseShowCaseLangResourceKind.ApiPropertyIsAccordion              => en_US.ApiPropertyIsAccordion,
            CollapseShowCaseLangResourceKind.ApiPropertyTriggerType              => en_US.ApiPropertyTriggerType,
            CollapseShowCaseLangResourceKind.ApiPropertyExpandIconPosition       => en_US.ApiPropertyExpandIconPosition,
            CollapseShowCaseLangResourceKind.ApiPropertyIsMotionEnabled          => en_US.ApiPropertyIsMotionEnabled,
            CollapseShowCaseLangResourceKind.ApiPropertyItemHeaderPadding        => en_US.ApiPropertyItemHeaderPadding,
            CollapseShowCaseLangResourceKind.ApiPropertyItemContentPadding       => en_US.ApiPropertyItemContentPadding,
            CollapseShowCaseLangResourceKind.ApiPropertyIsSelected               => en_US.ApiPropertyIsSelected,
            CollapseShowCaseLangResourceKind.ApiPropertyIsShowExpandIcon         => en_US.ApiPropertyIsShowExpandIcon,
            CollapseShowCaseLangResourceKind.ApiPropertyExpandIcon               => en_US.ApiPropertyExpandIcon,
            CollapseShowCaseLangResourceKind.ApiPropertyAddOnContent             => en_US.ApiPropertyAddOnContent,
            CollapseShowCaseLangResourceKind.ApiPropertyAddOnContentTemplate     => en_US.ApiPropertyAddOnContentTemplate,
            CollapseShowCaseLangResourceKind.ApiPropertyHeaderPadding            => en_US.ApiPropertyHeaderPadding,
            CollapseShowCaseLangResourceKind.ApiPropertyContentPadding           => en_US.ApiPropertyContentPadding,
            CollapseShowCaseLangResourceKind.TokenNameHeaderPadding              => en_US.TokenNameHeaderPadding,
            CollapseShowCaseLangResourceKind.TokenNameHeaderBg                   => en_US.TokenNameHeaderBg,
            CollapseShowCaseLangResourceKind.TokenNameContentPadding             => en_US.TokenNameContentPadding,
            CollapseShowCaseLangResourceKind.TokenNameContentBg                  => en_US.TokenNameContentBg,
            CollapseShowCaseLangResourceKind.TokenNameCollapseHeaderPaddingSM    => en_US.TokenNameCollapseHeaderPaddingSM,
            CollapseShowCaseLangResourceKind.TokenNameCollapseHeaderPaddingLG    => en_US.TokenNameCollapseHeaderPaddingLG,
            CollapseShowCaseLangResourceKind.TokenNameCollapseContentPaddingSM   => en_US.TokenNameCollapseContentPaddingSM,
            CollapseShowCaseLangResourceKind.TokenNameCollapseContentPaddingLG   => en_US.TokenNameCollapseContentPaddingLG,
            CollapseShowCaseLangResourceKind.TokenNameCollapsePanelBorderRadius  => en_US.TokenNameCollapsePanelBorderRadius,
            CollapseShowCaseLangResourceKind.TokenNameLeftExpandButtonMargin     => en_US.TokenNameLeftExpandButtonMargin,
            CollapseShowCaseLangResourceKind.TokenNameRightExpandButtonMargin    => en_US.TokenNameRightExpandButtonMargin,
            CollapseShowCaseLangResourceKind.TokenScopeComponent                 => en_US.TokenScopeComponent,
            CollapseShowCaseLangResourceKind.TokenStatusStable                   => en_US.TokenStatusStable,
            _                                                                    => kind.ToString()
        };
    }

    public void HandleExpandButtonPosOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (args.Index == 0)
        {
            CollapseExpandIconPosition = CollapseExpandIconPosition.Start;
        }
        else if (args.Index == 1)
        {
            CollapseExpandIconPosition = CollapseExpandIconPosition.End;
        }
    }
}

public sealed record CollapseApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record CollapseDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
