using System.Collections.ObjectModel;
using System.Globalization;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Theme.Language;
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

    private int _currentStep;

    public int CurrentStep
    {
        get => _currentStep;
        set
        {
            this.RaiseAndSetIfChanged(ref _currentStep, value);
            this.RaisePropertyChanged(nameof(CurrentStepText));
            PreviousButtonVisible = CurrentStep > 0;
            RefreshInteractiveButtonText();
        }
    }

    public string CurrentStepText => CurrentStep.ToString(CultureInfo.CurrentCulture);

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
        CurrentStep            = 0;
        PreviousButtonVisible = false;
        RefreshInteractiveButtonText();
    }

    public void MoveToNextInteractiveStep()
    {
        if (CurrentStep < InteractiveLastStepIndex)
        {
            CurrentStep++;
        }

        RefreshInteractiveButtonText();
    }

    public void MoveToPreviousInteractiveStep()
    {
        if (CurrentStep > 0)
        {
            CurrentStep--;
        }

        RefreshInteractiveButtonText();
    }

    public void RefreshInteractiveButtonText()
    {
        NextButtonText = CurrentStep == InteractiveLastStepIndex
            ? Lang(StepsShowCaseLangResourceKind.P2ContentDone)
            : Lang(StepsShowCaseLangResourceKind.P2ContentNext);
    }

    public void EnsureApiRows()
    {
        if (ApiRows is not null)
        {
            return;
        }

        ApiRows =
        [
            new StepsApiRow("Steps.CurrentStep", Lang(StepsShowCaseLangResourceKind.ApiPropertyCurrentStep), "int", "cyan", "0"),
            new StepsApiRow("Steps.InitialStep", Lang(StepsShowCaseLangResourceKind.ApiPropertyInitialStep), "int", "cyan", "-1"),
            new StepsApiRow("Steps.ProgressValue", Lang(StepsShowCaseLangResourceKind.ApiPropertyProgressValue), "double", "cyan", "0"),
            new StepsApiRow("Steps.CurrentStepStatus", Lang(StepsShowCaseLangResourceKind.ApiPropertyCurrentStepStatus), "StepsItemStatus", "blue", "Process"),
            new StepsApiRow("Steps.Orientation", Lang(StepsShowCaseLangResourceKind.ApiPropertyOrientation), "Orientation", "blue", "Horizontal"),
            new StepsApiRow("Steps.LabelPlacement", Lang(StepsShowCaseLangResourceKind.ApiPropertyLabelPlacement), "Orientation", "blue", "Horizontal"),
            new StepsApiRow("Steps.SizeType", Lang(StepsShowCaseLangResourceKind.ApiPropertySizeType), "SizeType", "blue", "Middle"),
            new StepsApiRow("Steps.ItemIndicatorType", Lang(StepsShowCaseLangResourceKind.ApiPropertyItemIndicatorType), "StepsItemIndicatorType", "blue", "Default"),
            new StepsApiRow("Steps.Style", Lang(StepsShowCaseLangResourceKind.ApiPropertyStyle), "StepsStyle", "blue", "Default"),
            new StepsApiRow("Steps.IsItemClickable", Lang(StepsShowCaseLangResourceKind.ApiPropertyIsItemClickable), "bool", "purple", "false"),
            new StepsApiRow("Steps.IsShowItemProgress", Lang(StepsShowCaseLangResourceKind.ApiPropertyIsShowItemProgress), "bool", "purple", "false"),
            new StepsApiRow("Steps.ContentTemplate", Lang(StepsShowCaseLangResourceKind.ApiPropertyContentTemplate), "IDataTemplate?", "cyan", "null"),
            new StepsApiRow("Steps.CurrentContent", Lang(StepsShowCaseLangResourceKind.ApiPropertyCurrentContent), "object?", "cyan", "read-only"),
            new StepsApiRow("StepsItem.SubHeader", Lang(StepsShowCaseLangResourceKind.ApiPropertyStepsItemSubHeader), "object?", "cyan", "null"),
            new StepsApiRow("StepsItem.Description", Lang(StepsShowCaseLangResourceKind.ApiPropertyStepsItemDescription), "object?", "cyan", "null"),
            new StepsApiRow("StepsItem.Icon", Lang(StepsShowCaseLangResourceKind.ApiPropertyStepsItemIcon), "PathIcon?", "cyan", "null"),
            new StepsApiRow("StepsItem.Status", Lang(StepsShowCaseLangResourceKind.ApiPropertyStepsItemStatus), "StepsItemStatus", "blue", "Process")
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
            new StepsDesignTokenRow("StepsProgressSize", Lang(StepsShowCaseLangResourceKind.TokenNameStepsProgressSize), Lang(StepsShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(StepsShowCaseLangResourceKind.TokenStatusStable), "success"),
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
            StepsShowCaseLangResourceKind.ApiPropertyCurrentStep                => en_US.ApiPropertyCurrentStep,
            StepsShowCaseLangResourceKind.ApiPropertyInitialStep                => en_US.ApiPropertyInitialStep,
            StepsShowCaseLangResourceKind.ApiPropertyProgressValue              => en_US.ApiPropertyProgressValue,
            StepsShowCaseLangResourceKind.ApiPropertyCurrentStepStatus          => en_US.ApiPropertyCurrentStepStatus,
            StepsShowCaseLangResourceKind.ApiPropertyOrientation                => en_US.ApiPropertyOrientation,
            StepsShowCaseLangResourceKind.ApiPropertyLabelPlacement             => en_US.ApiPropertyLabelPlacement,
            StepsShowCaseLangResourceKind.ApiPropertySizeType                   => en_US.ApiPropertySizeType,
            StepsShowCaseLangResourceKind.ApiPropertyItemIndicatorType          => en_US.ApiPropertyItemIndicatorType,
            StepsShowCaseLangResourceKind.ApiPropertyStyle                      => en_US.ApiPropertyStyle,
            StepsShowCaseLangResourceKind.ApiPropertyIsItemClickable            => en_US.ApiPropertyIsItemClickable,
            StepsShowCaseLangResourceKind.ApiPropertyIsShowItemProgress         => en_US.ApiPropertyIsShowItemProgress,
            StepsShowCaseLangResourceKind.ApiPropertyContentTemplate            => en_US.ApiPropertyContentTemplate,
            StepsShowCaseLangResourceKind.ApiPropertyCurrentContent             => en_US.ApiPropertyCurrentContent,
            StepsShowCaseLangResourceKind.ApiPropertyStepsItemSubHeader         => en_US.ApiPropertyStepsItemSubHeader,
            StepsShowCaseLangResourceKind.ApiPropertyStepsItemDescription       => en_US.ApiPropertyStepsItemDescription,
            StepsShowCaseLangResourceKind.ApiPropertyStepsItemIcon              => en_US.ApiPropertyStepsItemIcon,
            StepsShowCaseLangResourceKind.ApiPropertyStepsItemStatus            => en_US.ApiPropertyStepsItemStatus,
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
            StepsShowCaseLangResourceKind.TokenNameStepsProgressSize            => en_US.TokenNameStepsProgressSize,
            StepsShowCaseLangResourceKind.TokenNameInlineDotSize                => en_US.TokenNameInlineDotSize,
            StepsShowCaseLangResourceKind.TokenNameInlineItemPadding            => en_US.TokenNameInlineItemPadding,
            StepsShowCaseLangResourceKind.TokenNameProcessIconBgColor           => en_US.TokenNameProcessIconBgColor,
            StepsShowCaseLangResourceKind.TokenNameFinishTailColor              => en_US.TokenNameFinishTailColor,
            StepsShowCaseLangResourceKind.TokenNameErrorIconBgColor             => en_US.TokenNameErrorIconBgColor,
            StepsShowCaseLangResourceKind.TokenScopeComponent                   => en_US.TokenScopeComponent,
            StepsShowCaseLangResourceKind.TokenStatusStable                     => en_US.TokenStatusStable,
            StepsShowCaseLangResourceKind.P2TextCurrentStep                    => en_US.P2TextCurrentStep,
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
