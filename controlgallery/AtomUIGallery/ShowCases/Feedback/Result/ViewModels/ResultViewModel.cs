using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Result;

public class ResultViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Result";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<ResultApiRow>? _apiRows;
    private ObservableCollection<ResultDesignTokenRow>? _designTokenRows;

    public ObservableCollection<ResultApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<ResultDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public ResultViewModel(IScreen screen)
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
            new ResultApiRow("Status", Lang(ResultShowCaseLangResourceKind.ApiPropertyStatus), "ResultStatus", "purple", "Info"),
            new ResultApiRow("Icon", Lang(ResultShowCaseLangResourceKind.ApiPropertyIcon), "PathIcon?", "cyan", "null"),
            new ResultApiRow("Header", Lang(ResultShowCaseLangResourceKind.ApiPropertyHeader), "object?", "cyan", "null"),
            new ResultApiRow("HeaderTemplate", Lang(ResultShowCaseLangResourceKind.ApiPropertyHeaderTemplate), "IDataTemplate?", "cyan", "null"),
            new ResultApiRow("SubHeader", Lang(ResultShowCaseLangResourceKind.ApiPropertySubHeader), "object?", "cyan", "null"),
            new ResultApiRow("SubHeaderTemplate", Lang(ResultShowCaseLangResourceKind.ApiPropertySubHeaderTemplate), "IDataTemplate?", "cyan", "null"),
            new ResultApiRow("Extra", Lang(ResultShowCaseLangResourceKind.ApiPropertyExtra), "object?", "cyan", "null"),
            new ResultApiRow("ExtraTemplate", Lang(ResultShowCaseLangResourceKind.ApiPropertyExtraTemplate), "IDataTemplate?", "cyan", "null"),
            new ResultApiRow("Content", Lang(ResultShowCaseLangResourceKind.ApiPropertyContent), "object?", "cyan", "null")
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
            new ResultDesignTokenRow("HeaderFontSize", Lang(ResultShowCaseLangResourceKind.TokenNameHeaderFontSize), Lang(ResultShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ResultShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ResultDesignTokenRow("SubHeaderFontSize", Lang(ResultShowCaseLangResourceKind.TokenNameSubHeaderFontSize), Lang(ResultShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ResultShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ResultDesignTokenRow("IconSize", Lang(ResultShowCaseLangResourceKind.TokenNameIconSize), Lang(ResultShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ResultShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ResultDesignTokenRow("ExtraMargin", Lang(ResultShowCaseLangResourceKind.TokenNameExtraMargin), Lang(ResultShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ResultShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ResultDesignTokenRow("ImageWidth", Lang(ResultShowCaseLangResourceKind.TokenNameImageWidth), Lang(ResultShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ResultShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ResultDesignTokenRow("ImageHeight", Lang(ResultShowCaseLangResourceKind.TokenNameImageHeight), Lang(ResultShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ResultShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ResultDesignTokenRow("ResultInfoIconColor", Lang(ResultShowCaseLangResourceKind.TokenNameResultInfoIconColor), Lang(ResultShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ResultShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ResultDesignTokenRow("ResultSuccessIconColor", Lang(ResultShowCaseLangResourceKind.TokenNameResultSuccessIconColor), Lang(ResultShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ResultShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ResultDesignTokenRow("ResultWarningIconColor", Lang(ResultShowCaseLangResourceKind.TokenNameResultWarningIconColor), Lang(ResultShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ResultShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ResultDesignTokenRow("ResultErrorIconColor", Lang(ResultShowCaseLangResourceKind.TokenNameResultErrorIconColor), Lang(ResultShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ResultShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ResultDesignTokenRow("FramePadding", Lang(ResultShowCaseLangResourceKind.TokenNameFramePadding), Lang(ResultShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ResultShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ResultDesignTokenRow("ContentPadding", Lang(ResultShowCaseLangResourceKind.TokenNameContentPadding), Lang(ResultShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ResultShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ResultDesignTokenRow("ContentMargin", Lang(ResultShowCaseLangResourceKind.TokenNameContentMargin), Lang(ResultShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ResultShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ResultDesignTokenRow("HeaderMargin", Lang(ResultShowCaseLangResourceKind.TokenNameHeaderMargin), Lang(ResultShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ResultShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ResultDesignTokenRow("StatusImageMargin", Lang(ResultShowCaseLangResourceKind.TokenNameStatusImageMargin), Lang(ResultShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ResultShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(ResultShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(ResultShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            ResultShowCaseLangResourceKind.ApiPropertyStatus                 => en_US.ApiPropertyStatus,
            ResultShowCaseLangResourceKind.ApiPropertyIcon                   => en_US.ApiPropertyIcon,
            ResultShowCaseLangResourceKind.ApiPropertyHeader                 => en_US.ApiPropertyHeader,
            ResultShowCaseLangResourceKind.ApiPropertyHeaderTemplate         => en_US.ApiPropertyHeaderTemplate,
            ResultShowCaseLangResourceKind.ApiPropertySubHeader              => en_US.ApiPropertySubHeader,
            ResultShowCaseLangResourceKind.ApiPropertySubHeaderTemplate      => en_US.ApiPropertySubHeaderTemplate,
            ResultShowCaseLangResourceKind.ApiPropertyExtra                  => en_US.ApiPropertyExtra,
            ResultShowCaseLangResourceKind.ApiPropertyExtraTemplate          => en_US.ApiPropertyExtraTemplate,
            ResultShowCaseLangResourceKind.ApiPropertyContent                => en_US.ApiPropertyContent,
            ResultShowCaseLangResourceKind.TokenNameHeaderFontSize           => en_US.TokenNameHeaderFontSize,
            ResultShowCaseLangResourceKind.TokenNameSubHeaderFontSize        => en_US.TokenNameSubHeaderFontSize,
            ResultShowCaseLangResourceKind.TokenNameIconSize                 => en_US.TokenNameIconSize,
            ResultShowCaseLangResourceKind.TokenNameExtraMargin              => en_US.TokenNameExtraMargin,
            ResultShowCaseLangResourceKind.TokenNameImageWidth               => en_US.TokenNameImageWidth,
            ResultShowCaseLangResourceKind.TokenNameImageHeight              => en_US.TokenNameImageHeight,
            ResultShowCaseLangResourceKind.TokenNameResultInfoIconColor      => en_US.TokenNameResultInfoIconColor,
            ResultShowCaseLangResourceKind.TokenNameResultSuccessIconColor   => en_US.TokenNameResultSuccessIconColor,
            ResultShowCaseLangResourceKind.TokenNameResultWarningIconColor   => en_US.TokenNameResultWarningIconColor,
            ResultShowCaseLangResourceKind.TokenNameResultErrorIconColor     => en_US.TokenNameResultErrorIconColor,
            ResultShowCaseLangResourceKind.TokenNameFramePadding             => en_US.TokenNameFramePadding,
            ResultShowCaseLangResourceKind.TokenNameContentPadding           => en_US.TokenNameContentPadding,
            ResultShowCaseLangResourceKind.TokenNameContentMargin            => en_US.TokenNameContentMargin,
            ResultShowCaseLangResourceKind.TokenNameHeaderMargin             => en_US.TokenNameHeaderMargin,
            ResultShowCaseLangResourceKind.TokenNameStatusImageMargin        => en_US.TokenNameStatusImageMargin,
            ResultShowCaseLangResourceKind.TokenScopeComponent               => en_US.TokenScopeComponent,
            ResultShowCaseLangResourceKind.TokenStatusStable                 => en_US.TokenStatusStable,
            _                                                                => kind.ToString()
        };
    }
}

public sealed record ResultApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record ResultDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
