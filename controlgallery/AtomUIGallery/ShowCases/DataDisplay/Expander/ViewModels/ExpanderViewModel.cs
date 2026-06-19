using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Expander;

public class ExpanderViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Expander";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    private ExpanderIconPosition _toggleIconPosition;

    public ExpanderIconPosition ToggleIconPosition
    {
        get => _toggleIconPosition;
        set => this.RaiseAndSetIfChanged(ref _toggleIconPosition, value);
    }

    private ExpandDirection _expandDirection;

    public ExpandDirection ExpandDirection
    {
        get => _expandDirection;
        set => this.RaiseAndSetIfChanged(ref _expandDirection, value);
    }

    private ObservableCollection<ExpanderApiRow>? _apiRows;
    private ObservableCollection<ExpanderDesignTokenRow>? _designTokenRows;

    public ObservableCollection<ExpanderApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<ExpanderDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public ExpanderViewModel(IScreen screen)
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
            new ExpanderApiRow("SizeType", Lang(ExpanderShowCaseLangResourceKind.ApiPropertySizeType), "CustomizableSizeType", "blue", "Middle"),
            new ExpanderApiRow("ExpandDirection", Lang(ExpanderShowCaseLangResourceKind.ApiPropertyExpandDirection), "ExpandDirection", "blue", "Down"),
            new ExpanderApiRow("IsShowExpandIcon", Lang(ExpanderShowCaseLangResourceKind.ApiPropertyIsShowExpandIcon), "bool", "purple", "true"),
            new ExpanderApiRow("ExpandIcon", Lang(ExpanderShowCaseLangResourceKind.ApiPropertyExpandIcon), "PathIcon?", "cyan", "RightOutlined"),
            new ExpanderApiRow("AddOnContent", Lang(ExpanderShowCaseLangResourceKind.ApiPropertyAddOnContent), "object?", "cyan", "null"),
            new ExpanderApiRow("AddOnContentTemplate", Lang(ExpanderShowCaseLangResourceKind.ApiPropertyAddOnContentTemplate), "IDataTemplate?", "cyan", "null"),
            new ExpanderApiRow("IsGhostStyle", Lang(ExpanderShowCaseLangResourceKind.ApiPropertyIsGhostStyle), "bool", "purple", "false"),
            new ExpanderApiRow("IsBorderless", Lang(ExpanderShowCaseLangResourceKind.ApiPropertyIsBorderless), "bool", "purple", "false"),
            new ExpanderApiRow("TriggerType", Lang(ExpanderShowCaseLangResourceKind.ApiPropertyTriggerType), "ExpanderTriggerType", "blue", "Header"),
            new ExpanderApiRow("ExpandIconPosition", Lang(ExpanderShowCaseLangResourceKind.ApiPropertyExpandIconPosition), "ExpanderIconPosition", "blue", "Start"),
            new ExpanderApiRow("HeaderPadding", Lang(ExpanderShowCaseLangResourceKind.ApiPropertyHeaderPadding), "Thickness", "cyan", "token"),
            new ExpanderApiRow("ContentPadding", Lang(ExpanderShowCaseLangResourceKind.ApiPropertyContentPadding), "Thickness", "cyan", "token"),
            new ExpanderApiRow("IsMotionEnabled", Lang(ExpanderShowCaseLangResourceKind.ApiPropertyIsMotionEnabled), "bool", "purple", "true")
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
            new ExpanderDesignTokenRow("HeaderPadding", Lang(ExpanderShowCaseLangResourceKind.TokenNameHeaderPadding), Lang(ExpanderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ExpanderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ExpanderDesignTokenRow("HeaderPaddingSM", Lang(ExpanderShowCaseLangResourceKind.TokenNameHeaderPaddingSM), Lang(ExpanderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ExpanderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ExpanderDesignTokenRow("HeaderPaddingLG", Lang(ExpanderShowCaseLangResourceKind.TokenNameHeaderPaddingLG), Lang(ExpanderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ExpanderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ExpanderDesignTokenRow("HeaderBg", Lang(ExpanderShowCaseLangResourceKind.TokenNameHeaderBg), Lang(ExpanderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ExpanderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ExpanderDesignTokenRow("ContentPadding", Lang(ExpanderShowCaseLangResourceKind.TokenNameContentPadding), Lang(ExpanderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ExpanderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ExpanderDesignTokenRow("ContentPaddingSM", Lang(ExpanderShowCaseLangResourceKind.TokenNameContentPaddingSM), Lang(ExpanderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ExpanderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ExpanderDesignTokenRow("ContentPaddingLG", Lang(ExpanderShowCaseLangResourceKind.TokenNameContentPaddingLG), Lang(ExpanderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ExpanderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ExpanderDesignTokenRow("ContentBg", Lang(ExpanderShowCaseLangResourceKind.TokenNameContentBg), Lang(ExpanderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ExpanderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ExpanderDesignTokenRow("ExpanderBorderRadius", Lang(ExpanderShowCaseLangResourceKind.TokenNameExpanderBorderRadius), Lang(ExpanderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ExpanderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ExpanderDesignTokenRow("LeftExpandButtonHMargin", Lang(ExpanderShowCaseLangResourceKind.TokenNameLeftExpandButtonHMargin), Lang(ExpanderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ExpanderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ExpanderDesignTokenRow("RightExpandButtonHMargin", Lang(ExpanderShowCaseLangResourceKind.TokenNameRightExpandButtonHMargin), Lang(ExpanderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ExpanderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ExpanderDesignTokenRow("LeftExpandButtonVMargin", Lang(ExpanderShowCaseLangResourceKind.TokenNameLeftExpandButtonVMargin), Lang(ExpanderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ExpanderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ExpanderDesignTokenRow("RightExpandButtonVMargin", Lang(ExpanderShowCaseLangResourceKind.TokenNameRightExpandButtonVMargin), Lang(ExpanderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ExpanderShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(ExpanderShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(ExpanderShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            ExpanderShowCaseLangResourceKind.ApiPropertySizeType                 => en_US.ApiPropertySizeType,
            ExpanderShowCaseLangResourceKind.ApiPropertyExpandDirection          => en_US.ApiPropertyExpandDirection,
            ExpanderShowCaseLangResourceKind.ApiPropertyIsShowExpandIcon         => en_US.ApiPropertyIsShowExpandIcon,
            ExpanderShowCaseLangResourceKind.ApiPropertyExpandIcon               => en_US.ApiPropertyExpandIcon,
            ExpanderShowCaseLangResourceKind.ApiPropertyAddOnContent             => en_US.ApiPropertyAddOnContent,
            ExpanderShowCaseLangResourceKind.ApiPropertyAddOnContentTemplate     => en_US.ApiPropertyAddOnContentTemplate,
            ExpanderShowCaseLangResourceKind.ApiPropertyIsGhostStyle             => en_US.ApiPropertyIsGhostStyle,
            ExpanderShowCaseLangResourceKind.ApiPropertyIsBorderless             => en_US.ApiPropertyIsBorderless,
            ExpanderShowCaseLangResourceKind.ApiPropertyTriggerType              => en_US.ApiPropertyTriggerType,
            ExpanderShowCaseLangResourceKind.ApiPropertyExpandIconPosition       => en_US.ApiPropertyExpandIconPosition,
            ExpanderShowCaseLangResourceKind.ApiPropertyHeaderPadding            => en_US.ApiPropertyHeaderPadding,
            ExpanderShowCaseLangResourceKind.ApiPropertyContentPadding           => en_US.ApiPropertyContentPadding,
            ExpanderShowCaseLangResourceKind.ApiPropertyIsMotionEnabled          => en_US.ApiPropertyIsMotionEnabled,
            ExpanderShowCaseLangResourceKind.TokenNameHeaderPadding              => en_US.TokenNameHeaderPadding,
            ExpanderShowCaseLangResourceKind.TokenNameHeaderPaddingSM            => en_US.TokenNameHeaderPaddingSM,
            ExpanderShowCaseLangResourceKind.TokenNameHeaderPaddingLG            => en_US.TokenNameHeaderPaddingLG,
            ExpanderShowCaseLangResourceKind.TokenNameHeaderBg                   => en_US.TokenNameHeaderBg,
            ExpanderShowCaseLangResourceKind.TokenNameContentPadding             => en_US.TokenNameContentPadding,
            ExpanderShowCaseLangResourceKind.TokenNameContentPaddingSM           => en_US.TokenNameContentPaddingSM,
            ExpanderShowCaseLangResourceKind.TokenNameContentPaddingLG           => en_US.TokenNameContentPaddingLG,
            ExpanderShowCaseLangResourceKind.TokenNameContentBg                  => en_US.TokenNameContentBg,
            ExpanderShowCaseLangResourceKind.TokenNameExpanderBorderRadius       => en_US.TokenNameExpanderBorderRadius,
            ExpanderShowCaseLangResourceKind.TokenNameLeftExpandButtonHMargin    => en_US.TokenNameLeftExpandButtonHMargin,
            ExpanderShowCaseLangResourceKind.TokenNameRightExpandButtonHMargin   => en_US.TokenNameRightExpandButtonHMargin,
            ExpanderShowCaseLangResourceKind.TokenNameLeftExpandButtonVMargin    => en_US.TokenNameLeftExpandButtonVMargin,
            ExpanderShowCaseLangResourceKind.TokenNameRightExpandButtonVMargin   => en_US.TokenNameRightExpandButtonVMargin,
            ExpanderShowCaseLangResourceKind.TokenScopeComponent                 => en_US.TokenScopeComponent,
            ExpanderShowCaseLangResourceKind.TokenStatusStable                   => en_US.TokenStatusStable,
            _                                                                    => kind.ToString()
        };
    }

    public void HandleExpandButtonPosOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (args.Index == 0)
        {
            ToggleIconPosition = ExpanderIconPosition.Start;
        }
        else if (args.Index == 1)
        {
            ToggleIconPosition = ExpanderIconPosition.End;
        }
    }

    public void HandleExpandDirectionOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (args.Index == 0)
        {
            ExpandDirection = ExpandDirection.Down;
        }
        else if (args.Index == 1)
        {
            ExpandDirection = ExpandDirection.Up;
        }
        else if (args.Index == 2)
        {
            ExpandDirection = ExpandDirection.Left;
        }
        else if (args.Index == 3)
        {
            ExpandDirection = ExpandDirection.Right;
        }
    }
}

public sealed record ExpanderApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record ExpanderDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
