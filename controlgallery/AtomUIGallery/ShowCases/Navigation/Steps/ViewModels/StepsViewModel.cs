using System.Collections.ObjectModel;
using System.Globalization;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Steps;

public class StepsViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Steps";

    private const int InteractiveLastStepIndex = 2;

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    private int _current;

    public int Current
    {
        get => _current;
        set
        {
            this.RaiseAndSetIfChanged(ref _current, value);
            this.RaisePropertyChanged(nameof(CurrentText));
            PreviousButtonVisible = Current > 0;
            RefreshInteractiveText();
        }
    }

    public string CurrentText => Current.ToString(CultureInfo.CurrentCulture);

    public string InteractivePageContent => Current switch
    {
        0 => Lang(StepsShowCaseLangResourceKind.P2ContentFirstContent),
        1 => Lang(StepsShowCaseLangResourceKind.P2ContentSecondContent),
        2 => Lang(StepsShowCaseLangResourceKind.P2ContentLastContent),
        _ => string.Empty
    };

    private bool _previousButtonVisible;

    public bool PreviousButtonVisible
    {
        get => _previousButtonVisible;
        set => this.RaiseAndSetIfChanged(ref _previousButtonVisible, value);
    }

    private string _nextButtonText = Lang(StepsShowCaseLangResourceKind.P2ContentNext);

    public string NextButtonText
    {
        get => _nextButtonText;
        set => this.RaiseAndSetIfChanged(ref _nextButtonText, value);
    }

    private ObservableCollection<StepsApiRow>? _apiRows;
    private ObservableCollection<StepsDesignTokenRow>? _designTokenRows;

    public ObservableCollection<StepsApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<StepsDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public StepsViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

    public void ResetInteractiveStep()
    {
        Current = 0;
    }

    public void MoveToNextInteractiveStep()
    {
        if (Current < InteractiveLastStepIndex)
        {
            Current++;
        }
    }

    public void MoveToPreviousInteractiveStep()
    {
        if (Current > 0)
        {
            Current--;
        }
    }

    public void RefreshInteractiveText()
    {
        NextButtonText = Current == InteractiveLastStepIndex
            ? Lang(StepsShowCaseLangResourceKind.P2ContentDone)
            : Lang(StepsShowCaseLangResourceKind.P2ContentNext);
        this.RaisePropertyChanged(nameof(InteractivePageContent));
    }

    public void EnsureApiRows()
    {
        if (ApiRows is not null)
        {
            return;
        }

        ApiRows =
        [
            new StepsApiRow("Steps.Current", Lang(StepsShowCaseLangResourceKind.ApiPropertyCurrent), "int", "cyan", "0"),
            new StepsApiRow("Steps.Initial", Lang(StepsShowCaseLangResourceKind.ApiPropertyInitial), "int", "cyan", "0"),
            new StepsApiRow("Steps.Status", Lang(StepsShowCaseLangResourceKind.ApiPropertyStatus), "StepsStatus", "blue", "Process"),
            new StepsApiRow("Steps.Percent", Lang(StepsShowCaseLangResourceKind.ApiPropertyPercent), "double?", "cyan", "null"),
            new StepsApiRow("Steps.Type", Lang(StepsShowCaseLangResourceKind.ApiPropertyType), "StepsType", "blue", "Default"),
            new StepsApiRow("Steps.Orientation", Lang(StepsShowCaseLangResourceKind.ApiPropertyOrientation), "Orientation", "blue", "Horizontal"),
            new StepsApiRow("Steps.TitlePlacement", Lang(StepsShowCaseLangResourceKind.ApiPropertyTitlePlacement), "Orientation", "blue", "Horizontal"),
            new StepsApiRow("Steps.SizeType", Lang(StepsShowCaseLangResourceKind.ApiPropertySizeType), "SizeType", "blue", "Middle"),
            new StepsApiRow("Steps.IsItemClickable", Lang(StepsShowCaseLangResourceKind.ApiPropertyIsItemClickable), "bool", "purple", "false"),
            new StepsApiRow("Steps.IsMotionEnabled", Lang(StepsShowCaseLangResourceKind.ApiPropertyIsMotionEnabled), "bool", "purple", "token"),
            new StepsApiRow("Steps.Items", Lang(StepsShowCaseLangResourceKind.ApiPropertyItems), "IList", "cyan", "empty"),
            new StepsApiRow("Steps.ItemsSource", Lang(StepsShowCaseLangResourceKind.ApiPropertyItemsSource), "IEnumerable?", "cyan", "null"),
            new StepsApiRow("Steps.ItemTemplate", Lang(StepsShowCaseLangResourceKind.ApiPropertyItemTemplate), "IDataTemplate?", "cyan", "null"),
            new StepsApiRow("Steps.CurrentChangeRequested", Lang(StepsShowCaseLangResourceKind.ApiEventCurrentChangeRequested), "event EventHandler<StepsCurrentChangeRequestedEventArgs>?", "orange", "null"),
            new StepsApiRow("StepsItem.Header", Lang(StepsShowCaseLangResourceKind.ApiPropertyStepsItemHeader), "object?", "cyan", "null"),
            new StepsApiRow("StepsItem.HeaderTemplate", Lang(StepsShowCaseLangResourceKind.ApiPropertyStepsItemHeaderTemplate), "IDataTemplate?", "cyan", "null"),
            new StepsApiRow("StepsItem.SubHeader", Lang(StepsShowCaseLangResourceKind.ApiPropertyStepsItemSubHeader), "object?", "cyan", "null"),
            new StepsApiRow("StepsItem.SubHeaderTemplate", Lang(StepsShowCaseLangResourceKind.ApiPropertyStepsItemSubHeaderTemplate), "IDataTemplate?", "cyan", "null"),
            new StepsApiRow("StepsItem.Content", Lang(StepsShowCaseLangResourceKind.ApiPropertyStepsItemContent), "object?", "cyan", "null"),
            new StepsApiRow("StepsItem.ContentTemplate", Lang(StepsShowCaseLangResourceKind.ApiPropertyStepsItemContentTemplate), "IDataTemplate?", "cyan", "null"),
            new StepsApiRow("StepsItem.Icon", Lang(StepsShowCaseLangResourceKind.ApiPropertyStepsItemIcon), "PathIcon?", "cyan", "null"),
            new StepsApiRow("StepsItem.Status", Lang(StepsShowCaseLangResourceKind.ApiPropertyStepsItemStatus), "StepsStatus?", "blue", "null"),
            new StepsApiRow("StepsItem.IsEnabled", Lang(StepsShowCaseLangResourceKind.ApiPropertyStepsItemIsEnabled), "bool", "purple", "true")
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
            new StepsDesignTokenRow("DescriptionMaxWidth", Lang(StepsShowCaseLangResourceKind.TokenNameDescriptionMaxWidth), Lang(StepsShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(StepsShowCaseLangResourceKind.TokenStatusStable), "success"),
            new StepsDesignTokenRow("IconSize", Lang(StepsShowCaseLangResourceKind.TokenNameIconSize), Lang(StepsShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(StepsShowCaseLangResourceKind.TokenStatusStable), "success"),
            new StepsDesignTokenRow("IconFontSize", Lang(StepsShowCaseLangResourceKind.TokenNameIconFontSize), Lang(StepsShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(StepsShowCaseLangResourceKind.TokenStatusStable), "success"),
            new StepsDesignTokenRow("IconSizeSM", Lang(StepsShowCaseLangResourceKind.TokenNameIconSizeSM), Lang(StepsShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(StepsShowCaseLangResourceKind.TokenStatusStable), "success"),
            new StepsDesignTokenRow("DotSize", Lang(StepsShowCaseLangResourceKind.TokenNameDotSize), Lang(StepsShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(StepsShowCaseLangResourceKind.TokenStatusStable), "success"),
            new StepsDesignTokenRow("DotCurrentSize", Lang(StepsShowCaseLangResourceKind.TokenNameDotCurrentSize), Lang(StepsShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(StepsShowCaseLangResourceKind.TokenStatusStable), "success"),
            new StepsDesignTokenRow("DotLineThickness", Lang(StepsShowCaseLangResourceKind.TokenNameDotLineThickness), Lang(StepsShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(StepsShowCaseLangResourceKind.TokenStatusStable), "success"),
            new StepsDesignTokenRow("HorizontalHeaderMargin", Lang(StepsShowCaseLangResourceKind.TokenNameHorizontalHeaderMargin), Lang(StepsShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(StepsShowCaseLangResourceKind.TokenStatusStable), "success"),
            new StepsDesignTokenRow("VerticalItemSpacing", Lang(StepsShowCaseLangResourceKind.TokenNameVerticalItemSpacing), Lang(StepsShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(StepsShowCaseLangResourceKind.TokenStatusStable), "success"),
            new StepsDesignTokenRow("VerticalDescriptionPadding", Lang(StepsShowCaseLangResourceKind.TokenNameVerticalDescriptionPadding), Lang(StepsShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(StepsShowCaseLangResourceKind.TokenStatusStable), "success"),
            new StepsDesignTokenRow("StepsNavActiveColor", Lang(StepsShowCaseLangResourceKind.TokenNameStepsNavActiveColor), Lang(StepsShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(StepsShowCaseLangResourceKind.TokenStatusStable), "success"),
            new StepsDesignTokenRow("InlineDotSize", Lang(StepsShowCaseLangResourceKind.TokenNameInlineDotSize), Lang(StepsShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(StepsShowCaseLangResourceKind.TokenStatusStable), "success"),
            new StepsDesignTokenRow("InlineItemPadding", Lang(StepsShowCaseLangResourceKind.TokenNameInlineItemPadding), Lang(StepsShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(StepsShowCaseLangResourceKind.TokenStatusStable), "success"),
            new StepsDesignTokenRow("ProcessIconBgColor", Lang(StepsShowCaseLangResourceKind.TokenNameProcessIconBgColor), Lang(StepsShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(StepsShowCaseLangResourceKind.TokenStatusStable), "success"),
            new StepsDesignTokenRow("FinishTailColor", Lang(StepsShowCaseLangResourceKind.TokenNameFinishTailColor), Lang(StepsShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(StepsShowCaseLangResourceKind.TokenStatusStable), "success"),
            new StepsDesignTokenRow("ErrorIconBgColor", Lang(StepsShowCaseLangResourceKind.TokenNameErrorIconBgColor), Lang(StepsShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(StepsShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(StepsShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(StepsShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            StepsShowCaseLangResourceKind.P2ContentDone                         => en_US.P2ContentDone,
            StepsShowCaseLangResourceKind.P2ContentNext                         => en_US.P2ContentNext,
            StepsShowCaseLangResourceKind.P2ContentFirstContent                 => en_US.P2ContentFirstContent,
            StepsShowCaseLangResourceKind.P2ContentSecondContent                => en_US.P2ContentSecondContent,
            StepsShowCaseLangResourceKind.P2ContentLastContent                  => en_US.P2ContentLastContent,
            StepsShowCaseLangResourceKind.ApiPropertyCurrent                    => en_US.ApiPropertyCurrent,
            StepsShowCaseLangResourceKind.ApiPropertyInitial                    => en_US.ApiPropertyInitial,
            StepsShowCaseLangResourceKind.ApiPropertyStatus                     => en_US.ApiPropertyStatus,
            StepsShowCaseLangResourceKind.ApiPropertyPercent                    => en_US.ApiPropertyPercent,
            StepsShowCaseLangResourceKind.ApiPropertyType                       => en_US.ApiPropertyType,
            StepsShowCaseLangResourceKind.ApiPropertyOrientation                => en_US.ApiPropertyOrientation,
            StepsShowCaseLangResourceKind.ApiPropertyTitlePlacement             => en_US.ApiPropertyTitlePlacement,
            StepsShowCaseLangResourceKind.ApiPropertySizeType                   => en_US.ApiPropertySizeType,
            StepsShowCaseLangResourceKind.ApiPropertyIsItemClickable            => en_US.ApiPropertyIsItemClickable,
            StepsShowCaseLangResourceKind.ApiPropertyIsMotionEnabled            => en_US.ApiPropertyIsMotionEnabled,
            StepsShowCaseLangResourceKind.ApiPropertyItems                      => en_US.ApiPropertyItems,
            StepsShowCaseLangResourceKind.ApiPropertyItemsSource                => en_US.ApiPropertyItemsSource,
            StepsShowCaseLangResourceKind.ApiPropertyItemTemplate               => en_US.ApiPropertyItemTemplate,
            StepsShowCaseLangResourceKind.ApiEventCurrentChangeRequested        => en_US.ApiEventCurrentChangeRequested,
            StepsShowCaseLangResourceKind.ApiPropertyStepsItemHeader            => en_US.ApiPropertyStepsItemHeader,
            StepsShowCaseLangResourceKind.ApiPropertyStepsItemHeaderTemplate    => en_US.ApiPropertyStepsItemHeaderTemplate,
            StepsShowCaseLangResourceKind.ApiPropertyStepsItemSubHeader         => en_US.ApiPropertyStepsItemSubHeader,
            StepsShowCaseLangResourceKind.ApiPropertyStepsItemSubHeaderTemplate => en_US.ApiPropertyStepsItemSubHeaderTemplate,
            StepsShowCaseLangResourceKind.ApiPropertyStepsItemContent           => en_US.ApiPropertyStepsItemContent,
            StepsShowCaseLangResourceKind.ApiPropertyStepsItemContentTemplate   => en_US.ApiPropertyStepsItemContentTemplate,
            StepsShowCaseLangResourceKind.ApiPropertyStepsItemIcon              => en_US.ApiPropertyStepsItemIcon,
            StepsShowCaseLangResourceKind.ApiPropertyStepsItemStatus            => en_US.ApiPropertyStepsItemStatus,
            StepsShowCaseLangResourceKind.ApiPropertyStepsItemIsEnabled         => en_US.ApiPropertyStepsItemIsEnabled,
            StepsShowCaseLangResourceKind.TokenNameDescriptionMaxWidth          => en_US.TokenNameDescriptionMaxWidth,
            StepsShowCaseLangResourceKind.TokenNameIconSize                     => en_US.TokenNameIconSize,
            StepsShowCaseLangResourceKind.TokenNameIconFontSize                 => en_US.TokenNameIconFontSize,
            StepsShowCaseLangResourceKind.TokenNameIconSizeSM                   => en_US.TokenNameIconSizeSM,
            StepsShowCaseLangResourceKind.TokenNameDotSize                      => en_US.TokenNameDotSize,
            StepsShowCaseLangResourceKind.TokenNameDotCurrentSize               => en_US.TokenNameDotCurrentSize,
            StepsShowCaseLangResourceKind.TokenNameDotLineThickness             => en_US.TokenNameDotLineThickness,
            StepsShowCaseLangResourceKind.TokenNameHorizontalHeaderMargin       => en_US.TokenNameHorizontalHeaderMargin,
            StepsShowCaseLangResourceKind.TokenNameVerticalItemSpacing          => en_US.TokenNameVerticalItemSpacing,
            StepsShowCaseLangResourceKind.TokenNameVerticalDescriptionPadding   => en_US.TokenNameVerticalDescriptionPadding,
            StepsShowCaseLangResourceKind.TokenNameStepsNavActiveColor          => en_US.TokenNameStepsNavActiveColor,
            StepsShowCaseLangResourceKind.TokenNameInlineDotSize                => en_US.TokenNameInlineDotSize,
            StepsShowCaseLangResourceKind.TokenNameInlineItemPadding            => en_US.TokenNameInlineItemPadding,
            StepsShowCaseLangResourceKind.TokenNameProcessIconBgColor           => en_US.TokenNameProcessIconBgColor,
            StepsShowCaseLangResourceKind.TokenNameFinishTailColor              => en_US.TokenNameFinishTailColor,
            StepsShowCaseLangResourceKind.TokenNameErrorIconBgColor             => en_US.TokenNameErrorIconBgColor,
            StepsShowCaseLangResourceKind.TokenScopeComponent                   => en_US.TokenScopeComponent,
            StepsShowCaseLangResourceKind.TokenStatusStable                     => en_US.TokenStatusStable,
            StepsShowCaseLangResourceKind.P2TextCurrent                         => en_US.P2TextCurrent,
            _                                                                  => kind.ToString()
        };
    }
}

public sealed record StepsApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record StepsDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
