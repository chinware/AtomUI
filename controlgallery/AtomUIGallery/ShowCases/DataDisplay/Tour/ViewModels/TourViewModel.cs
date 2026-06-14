using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Tour;

public class TourViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Tour";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private bool _basicCaseTourOpened;

    public bool BasicCaseTourOpened
    {
        get => _basicCaseTourOpened;
        set => this.RaiseAndSetIfChanged(ref _basicCaseTourOpened, value);
    }

    private bool _nonMaskTourOpened;

    public bool NonMaskTourOpened
    {
        get => _nonMaskTourOpened;
        set => this.RaiseAndSetIfChanged(ref _nonMaskTourOpened, value);
    }

    private bool _placementTourOpened;

    public bool PlacementTourOpened
    {
        get => _placementTourOpened;
        set => this.RaiseAndSetIfChanged(ref _placementTourOpened, value);
    }

    private bool _customIndicatorTourOpened;

    public bool CustomIndicatorTourOpened
    {
        get => _customIndicatorTourOpened;
        set => this.RaiseAndSetIfChanged(ref _customIndicatorTourOpened, value);
    }

    private bool _customMaskTourOpened;

    public bool CustomMaskTourOpened
    {
        get => _customMaskTourOpened;
        set => this.RaiseAndSetIfChanged(ref _customMaskTourOpened, value);
    }

    private bool _customGapTourOpened;

    public bool CustomGapTourOpened
    {
        get => _customGapTourOpened;
        set => this.RaiseAndSetIfChanged(ref _customGapTourOpened, value);
    }

    private double _customGapRadius = 2;

    public double CustomGapRadius
    {
        get => _customGapRadius;
        set => this.RaiseAndSetIfChanged(ref _customGapRadius, value);
    }

    private double _customGapOffsetX;

    public double CustomGapOffsetX
    {
        get => _customGapOffsetX;
        set => this.RaiseAndSetIfChanged(ref _customGapOffsetX, value);
    }

    private double _customGapOffsetY;

    public double CustomGapOffsetY
    {
        get => _customGapOffsetY;
        set => this.RaiseAndSetIfChanged(ref _customGapOffsetY, value);
    }

    private IList<ITourStepOption>? _basicCaseSteps;

    public IList<ITourStepOption>? BasicCaseSteps
    {
        get => _basicCaseSteps;
        set => this.RaiseAndSetIfChanged(ref _basicCaseSteps, value);
    }

    private bool _customActionTourOpened;

    public bool CustomActionTourOpened
    {
        get => _customActionTourOpened;
        set => this.RaiseAndSetIfChanged(ref _customActionTourOpened, value);
    }

    private ObservableCollection<TourApiRow>? _apiRows;
    private ObservableCollection<TourDesignTokenRow>? _designTokenRows;

    public ObservableCollection<TourApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<TourDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public TourViewModel(IScreen screen)
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
            new TourApiRow("StepsSource", Lang(TourShowCaseLangResourceKind.ApiPropertyStepsSource), "IEnumerable<ITourStepOption>?", "cyan", "null"),
            new TourApiRow("ItemTemplate", Lang(TourShowCaseLangResourceKind.ApiPropertyItemTemplate), "IDataTemplate?", "cyan", "null"),
            new TourApiRow("IsOpen", Lang(TourShowCaseLangResourceKind.ApiPropertyIsOpen), "bool", "green", "false"),
            new TourApiRow("CurrentIndex", Lang(TourShowCaseLangResourceKind.ApiPropertyCurrentIndex), "int", "green", "-1"),
            new TourApiRow("StepCount", Lang(TourShowCaseLangResourceKind.ApiPropertyStepCount), "int", "green", "0"),
            new TourApiRow("Placement", Lang(TourShowCaseLangResourceKind.ApiPropertyPlacement), "TourPlacementMode", "blue", "Center"),
            new TourApiRow("IsShowMask", Lang(TourShowCaseLangResourceKind.ApiPropertyIsShowMask), "bool", "green", "true"),
            new TourApiRow("MaskColor", Lang(TourShowCaseLangResourceKind.ApiPropertyMaskColor), "IBrush?", "cyan", "null"),
            new TourApiRow("StyleType", Lang(TourShowCaseLangResourceKind.ApiPropertyStyleType), "TourStyleType", "blue", "Default"),
            new TourApiRow("IsArrowVisible", Lang(TourShowCaseLangResourceKind.ApiPropertyIsArrowVisible), "bool", "green", "true"),
            new TourApiRow("IsPointAtCenter", Lang(TourShowCaseLangResourceKind.ApiPropertyIsPointAtCenter), "bool", "green", "false"),
            new TourApiRow("CloseIcon", Lang(TourShowCaseLangResourceKind.ApiPropertyCloseIcon), "IIconTemplate?", "cyan", "null"),
            new TourApiRow("Indicator", Lang(TourShowCaseLangResourceKind.ApiPropertyIndicator), "TourIndicator?", "cyan", "null"),
            new TourApiRow("IsDisabledInteraction", Lang(TourShowCaseLangResourceKind.ApiPropertyIsDisabledInteraction), "bool", "green", "false"),
            new TourApiRow("IsScrollIntoView", Lang(TourShowCaseLangResourceKind.ApiPropertyIsScrollIntoView), "bool", "green", "true"),
            new TourApiRow("GapOffsetX", Lang(TourShowCaseLangResourceKind.ApiPropertyGapOffsetX), "double", "green", "6"),
            new TourApiRow("GapOffsetY", Lang(TourShowCaseLangResourceKind.ApiPropertyGapOffsetY), "double", "green", "6"),
            new TourApiRow("GapRadius", Lang(TourShowCaseLangResourceKind.ApiPropertyGapRadius), "double", "green", "2"),
            new TourApiRow("TourStep.Target", Lang(TourShowCaseLangResourceKind.ApiPropertyStepTarget), "Control?", "cyan", "null"),
            new TourApiRow("TourStep.Title", Lang(TourShowCaseLangResourceKind.ApiPropertyStepTitle), "object?", "cyan", "null"),
            new TourApiRow("TourStep.Description", Lang(TourShowCaseLangResourceKind.ApiPropertyStepDescription), "object?", "cyan", "null"),
            new TourApiRow("TourStep.Cover", Lang(TourShowCaseLangResourceKind.ApiPropertyStepCover), "object?", "cyan", "null")
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
            new TourDesignTokenRow("CloseBtnSize", Lang(TourShowCaseLangResourceKind.TokenNameCloseBtnSize), Lang(TourShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TourShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TourDesignTokenRow("PrimaryPrevBtnBg", Lang(TourShowCaseLangResourceKind.TokenNamePrimaryPrevBtnBg), Lang(TourShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TourShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TourDesignTokenRow("PrimaryNextBtnHoverBg", Lang(TourShowCaseLangResourceKind.TokenNamePrimaryNextBtnHoverBg), Lang(TourShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TourShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TourDesignTokenRow("TourViewMinWidth", Lang(TourShowCaseLangResourceKind.TokenNameTourViewMinWidth), Lang(TourShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TourShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TourDesignTokenRow("TourViewMinHeight", Lang(TourShowCaseLangResourceKind.TokenNameTourViewMinHeight), Lang(TourShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TourShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TourDesignTokenRow("HeaderColor", Lang(TourShowCaseLangResourceKind.TokenNameHeaderColor), Lang(TourShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TourShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(TourShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(TourShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            TourShowCaseLangResourceKind.ApiPropertyStepsSource            => en_US.ApiPropertyStepsSource,
            TourShowCaseLangResourceKind.ApiPropertyItemTemplate           => en_US.ApiPropertyItemTemplate,
            TourShowCaseLangResourceKind.ApiPropertyIsOpen                 => en_US.ApiPropertyIsOpen,
            TourShowCaseLangResourceKind.ApiPropertyCurrentIndex           => en_US.ApiPropertyCurrentIndex,
            TourShowCaseLangResourceKind.ApiPropertyStepCount              => en_US.ApiPropertyStepCount,
            TourShowCaseLangResourceKind.ApiPropertyPlacement              => en_US.ApiPropertyPlacement,
            TourShowCaseLangResourceKind.ApiPropertyIsShowMask             => en_US.ApiPropertyIsShowMask,
            TourShowCaseLangResourceKind.ApiPropertyMaskColor              => en_US.ApiPropertyMaskColor,
            TourShowCaseLangResourceKind.ApiPropertyStyleType              => en_US.ApiPropertyStyleType,
            TourShowCaseLangResourceKind.ApiPropertyIsArrowVisible         => en_US.ApiPropertyIsArrowVisible,
            TourShowCaseLangResourceKind.ApiPropertyIsPointAtCenter        => en_US.ApiPropertyIsPointAtCenter,
            TourShowCaseLangResourceKind.ApiPropertyCloseIcon              => en_US.ApiPropertyCloseIcon,
            TourShowCaseLangResourceKind.ApiPropertyIndicator              => en_US.ApiPropertyIndicator,
            TourShowCaseLangResourceKind.ApiPropertyIsDisabledInteraction  => en_US.ApiPropertyIsDisabledInteraction,
            TourShowCaseLangResourceKind.ApiPropertyIsScrollIntoView       => en_US.ApiPropertyIsScrollIntoView,
            TourShowCaseLangResourceKind.ApiPropertyGapOffsetX             => en_US.ApiPropertyGapOffsetX,
            TourShowCaseLangResourceKind.ApiPropertyGapOffsetY             => en_US.ApiPropertyGapOffsetY,
            TourShowCaseLangResourceKind.ApiPropertyGapRadius              => en_US.ApiPropertyGapRadius,
            TourShowCaseLangResourceKind.ApiPropertyStepTarget             => en_US.ApiPropertyStepTarget,
            TourShowCaseLangResourceKind.ApiPropertyStepTitle              => en_US.ApiPropertyStepTitle,
            TourShowCaseLangResourceKind.ApiPropertyStepDescription        => en_US.ApiPropertyStepDescription,
            TourShowCaseLangResourceKind.ApiPropertyStepCover              => en_US.ApiPropertyStepCover,
            TourShowCaseLangResourceKind.TokenNameCloseBtnSize             => en_US.TokenNameCloseBtnSize,
            TourShowCaseLangResourceKind.TokenNamePrimaryPrevBtnBg         => en_US.TokenNamePrimaryPrevBtnBg,
            TourShowCaseLangResourceKind.TokenNamePrimaryNextBtnHoverBg    => en_US.TokenNamePrimaryNextBtnHoverBg,
            TourShowCaseLangResourceKind.TokenNameTourViewMinWidth         => en_US.TokenNameTourViewMinWidth,
            TourShowCaseLangResourceKind.TokenNameTourViewMinHeight        => en_US.TokenNameTourViewMinHeight,
            TourShowCaseLangResourceKind.TokenNameHeaderColor              => en_US.TokenNameHeaderColor,
            TourShowCaseLangResourceKind.TokenScopeComponent               => en_US.TokenScopeComponent,
            TourShowCaseLangResourceKind.TokenStatusStable                 => en_US.TokenStatusStable,
            _                                                              => kind.ToString()
        };
    }
}

public sealed record TourApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record TourDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
