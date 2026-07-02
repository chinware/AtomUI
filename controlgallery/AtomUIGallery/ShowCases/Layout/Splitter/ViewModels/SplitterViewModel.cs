using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Splitter;

public class SplitterViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    public static EntityKey ID = "SplitterShowCase";

    public IScreen HostScreen { get; }
    public ViewModelActivator Activator { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<SplitterApiRow>? _apiRows;
    private ObservableCollection<SplitterDesignTokenRow>? _designTokenRows;

    public ObservableCollection<SplitterApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<SplitterDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public SplitterViewModel(IScreen screen)
    {
        Activator  = new ViewModelActivator();
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
            new SplitterApiRow("Orientation", Lang(SplitterShowCaseLangResourceKind.ApiPropertyOrientation), "Orientation", "blue", "Vertical"),
            new SplitterApiRow("IsLazy", Lang(SplitterShowCaseLangResourceKind.ApiPropertyIsLazy), "bool", "purple", "false"),
            new SplitterApiRow("HandleSize", Lang(SplitterShowCaseLangResourceKind.ApiPropertyHandleSize), "double", "cyan", "token"),
            new SplitterApiRow("LineThickness", Lang(SplitterShowCaseLangResourceKind.ApiPropertyLineThickness), "double", "cyan", "token"),
            new SplitterApiRow("LineCornerRadius", Lang(SplitterShowCaseLangResourceKind.ApiPropertyLineCornerRadius), "CornerRadius", "cyan", "token"),
            new SplitterApiRow("CollapsePreviousIcon", Lang(SplitterShowCaseLangResourceKind.ApiPropertyCollapsePreviousIcon), "IconTemplate?", "cyan", "null"),
            new SplitterApiRow("CollapseNextIcon", Lang(SplitterShowCaseLangResourceKind.ApiPropertyCollapseNextIcon), "IconTemplate?", "cyan", "null"),
            new SplitterApiRow("Children", Lang(SplitterShowCaseLangResourceKind.ApiPropertyChildren), "Controls", "cyan", "empty"),
            new SplitterApiRow("Splitter.Size", Lang(SplitterShowCaseLangResourceKind.ApiPropertySize), "Dimension?", "blue", "null"),
            new SplitterApiRow("Splitter.DefaultSize", Lang(SplitterShowCaseLangResourceKind.ApiPropertyDefaultSize), "Dimension?", "blue", "null"),
            new SplitterApiRow("Splitter.MinSize", Lang(SplitterShowCaseLangResourceKind.ApiPropertyMinSize), "Dimension?", "blue", "null"),
            new SplitterApiRow("Splitter.MaxSize", Lang(SplitterShowCaseLangResourceKind.ApiPropertyMaxSize), "Dimension?", "blue", "null"),
            new SplitterApiRow("Splitter.IsResizable", Lang(SplitterShowCaseLangResourceKind.ApiPropertyIsResizable), "bool", "purple", "true"),
            new SplitterApiRow("Splitter.Collapsible", Lang(SplitterShowCaseLangResourceKind.ApiPropertyCollapsible), "SplitterPanelCollapsible?", "cyan", "null"),
            new SplitterApiRow("Splitter.IsCollapsed", Lang(SplitterShowCaseLangResourceKind.ApiPropertyIsCollapsed), "bool", "purple", "false"),
            new SplitterApiRow("ResizeStarted", Lang(SplitterShowCaseLangResourceKind.ApiPropertyResizeStarted), "event", "default", "null"),
            new SplitterApiRow("ResizeDelta", Lang(SplitterShowCaseLangResourceKind.ApiPropertyResizeDelta), "event", "default", "null"),
            new SplitterApiRow("ResizeCompleted", Lang(SplitterShowCaseLangResourceKind.ApiPropertyResizeCompleted), "event", "default", "null")
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
            new SplitterDesignTokenRow("SplitBarDraggableSize", Lang(SplitterShowCaseLangResourceKind.TokenNameSplitBarDraggableSize), Lang(SplitterShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SplitterShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SplitterDesignTokenRow("SplitBarSize", Lang(SplitterShowCaseLangResourceKind.TokenNameSplitBarSize), Lang(SplitterShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SplitterShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SplitterDesignTokenRow("SplitTriggerSize", Lang(SplitterShowCaseLangResourceKind.TokenNameSplitTriggerSize), Lang(SplitterShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SplitterShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SplitterDesignTokenRow("SplitBarCollapseOffset", Lang(SplitterShowCaseLangResourceKind.TokenNameSplitBarCollapseOffset), Lang(SplitterShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SplitterShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SplitterDesignTokenRow("SplitBarCollapseOffsetNegative", Lang(SplitterShowCaseLangResourceKind.TokenNameSplitBarCollapseOffsetNegative), Lang(SplitterShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SplitterShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SplitterDesignTokenRow("SplitBarCollapseCrossOffset", Lang(SplitterShowCaseLangResourceKind.TokenNameSplitBarCollapseCrossOffset), Lang(SplitterShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SplitterShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SplitterDesignTokenRow("SplitBarHandleSize", Lang(SplitterShowCaseLangResourceKind.TokenNameSplitBarHandleSize), Lang(SplitterShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SplitterShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SplitterDesignTokenRow("HandleLineColor", Lang(SplitterShowCaseLangResourceKind.TokenNameHandleLineColor), Lang(SplitterShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SplitterShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SplitterDesignTokenRow("HandleLineHoverColor", Lang(SplitterShowCaseLangResourceKind.TokenNameHandleLineHoverColor), Lang(SplitterShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SplitterShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SplitterDesignTokenRow("HandleLineDragColor", Lang(SplitterShowCaseLangResourceKind.TokenNameHandleLineDragColor), Lang(SplitterShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SplitterShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SplitterDesignTokenRow("HandleIconColor", Lang(SplitterShowCaseLangResourceKind.TokenNameHandleIconColor), Lang(SplitterShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SplitterShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SplitterDesignTokenRow("HandleIconHoverColor", Lang(SplitterShowCaseLangResourceKind.TokenNameHandleIconHoverColor), Lang(SplitterShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SplitterShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SplitterDesignTokenRow("HandleIconPressedColor", Lang(SplitterShowCaseLangResourceKind.TokenNameHandleIconPressedColor), Lang(SplitterShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SplitterShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SplitterDesignTokenRow("HandleLineThickness", Lang(SplitterShowCaseLangResourceKind.TokenNameHandleLineThickness), Lang(SplitterShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SplitterShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SplitterDesignTokenRow("HandleIconSize", Lang(SplitterShowCaseLangResourceKind.TokenNameHandleIconSize), Lang(SplitterShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SplitterShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(SplitterShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(SplitterShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            SplitterShowCaseLangResourceKind.ApiPropertyOrientation                    => en_US.ApiPropertyOrientation,
            SplitterShowCaseLangResourceKind.ApiPropertyIsLazy                         => en_US.ApiPropertyIsLazy,
            SplitterShowCaseLangResourceKind.ApiPropertyHandleSize                     => en_US.ApiPropertyHandleSize,
            SplitterShowCaseLangResourceKind.ApiPropertyLineThickness                  => en_US.ApiPropertyLineThickness,
            SplitterShowCaseLangResourceKind.ApiPropertyLineCornerRadius               => en_US.ApiPropertyLineCornerRadius,
            SplitterShowCaseLangResourceKind.ApiPropertyCollapsePreviousIcon           => en_US.ApiPropertyCollapsePreviousIcon,
            SplitterShowCaseLangResourceKind.ApiPropertyCollapseNextIcon               => en_US.ApiPropertyCollapseNextIcon,
            SplitterShowCaseLangResourceKind.ApiPropertyChildren                       => en_US.ApiPropertyChildren,
            SplitterShowCaseLangResourceKind.ApiPropertySize                           => en_US.ApiPropertySize,
            SplitterShowCaseLangResourceKind.ApiPropertyDefaultSize                    => en_US.ApiPropertyDefaultSize,
            SplitterShowCaseLangResourceKind.ApiPropertyMinSize                        => en_US.ApiPropertyMinSize,
            SplitterShowCaseLangResourceKind.ApiPropertyMaxSize                        => en_US.ApiPropertyMaxSize,
            SplitterShowCaseLangResourceKind.ApiPropertyIsResizable                    => en_US.ApiPropertyIsResizable,
            SplitterShowCaseLangResourceKind.ApiPropertyCollapsible                    => en_US.ApiPropertyCollapsible,
            SplitterShowCaseLangResourceKind.ApiPropertyIsCollapsed                    => en_US.ApiPropertyIsCollapsed,
            SplitterShowCaseLangResourceKind.ApiPropertyResizeStarted                  => en_US.ApiPropertyResizeStarted,
            SplitterShowCaseLangResourceKind.ApiPropertyResizeDelta                    => en_US.ApiPropertyResizeDelta,
            SplitterShowCaseLangResourceKind.ApiPropertyResizeCompleted                => en_US.ApiPropertyResizeCompleted,
            SplitterShowCaseLangResourceKind.TokenNameSplitBarDraggableSize            => en_US.TokenNameSplitBarDraggableSize,
            SplitterShowCaseLangResourceKind.TokenNameSplitBarSize                     => en_US.TokenNameSplitBarSize,
            SplitterShowCaseLangResourceKind.TokenNameSplitTriggerSize                 => en_US.TokenNameSplitTriggerSize,
            SplitterShowCaseLangResourceKind.TokenNameSplitBarCollapseOffset           => en_US.TokenNameSplitBarCollapseOffset,
            SplitterShowCaseLangResourceKind.TokenNameSplitBarCollapseOffsetNegative   => en_US.TokenNameSplitBarCollapseOffsetNegative,
            SplitterShowCaseLangResourceKind.TokenNameSplitBarCollapseCrossOffset      => en_US.TokenNameSplitBarCollapseCrossOffset,
            SplitterShowCaseLangResourceKind.TokenNameSplitBarHandleSize               => en_US.TokenNameSplitBarHandleSize,
            SplitterShowCaseLangResourceKind.TokenNameHandleLineColor                  => en_US.TokenNameHandleLineColor,
            SplitterShowCaseLangResourceKind.TokenNameHandleLineHoverColor             => en_US.TokenNameHandleLineHoverColor,
            SplitterShowCaseLangResourceKind.TokenNameHandleLineDragColor              => en_US.TokenNameHandleLineDragColor,
            SplitterShowCaseLangResourceKind.TokenNameHandleIconColor                  => en_US.TokenNameHandleIconColor,
            SplitterShowCaseLangResourceKind.TokenNameHandleIconHoverColor             => en_US.TokenNameHandleIconHoverColor,
            SplitterShowCaseLangResourceKind.TokenNameHandleIconPressedColor           => en_US.TokenNameHandleIconPressedColor,
            SplitterShowCaseLangResourceKind.TokenNameHandleLineThickness              => en_US.TokenNameHandleLineThickness,
            SplitterShowCaseLangResourceKind.TokenNameHandleIconSize                   => en_US.TokenNameHandleIconSize,
            SplitterShowCaseLangResourceKind.TokenScopeComponent                       => en_US.TokenScopeComponent,
            SplitterShowCaseLangResourceKind.TokenStatusStable                         => en_US.TokenStatusStable,
            _                                                                          => kind.ToString()
        };
    }
}

public sealed record SplitterApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record SplitterDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
