using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Media;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ProgressBar;

public class ProgressBarViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "ProgressBar";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    private ObservableCollection<ProgressBarApiRow>? _apiRows;
    private ObservableCollection<ProgressBarDesignTokenRow>? _designTokenRows;

    public ObservableCollection<ProgressBarApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<ProgressBarDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    private LinearGradientBrush _twoStopsGradientStrokeColor;

    public LinearGradientBrush TwoStopsGradientStrokeColor
    {
        get => _twoStopsGradientStrokeColor;
        set => this.RaiseAndSetIfChanged(ref _twoStopsGradientStrokeColor, value);
    }

    private LinearGradientBrush _threeStopsGradientStrokeColor;

    public LinearGradientBrush ThreeStopsGradientStrokeColor
    {
        get => _threeStopsGradientStrokeColor;
        set => this.RaiseAndSetIfChanged(ref _threeStopsGradientStrokeColor, value);
    }

    private List<IBrush> _stepsChunkBrushes;

    public List<IBrush> StepsChunkBrushes
    {
        get => _stepsChunkBrushes;
        set => this.RaiseAndSetIfChanged(ref _stepsChunkBrushes, value);
    }

    private PercentPosition _innerStartPercentPosition;

    public PercentPosition InnerStartPercentPosition
    {
        get => _innerStartPercentPosition;
        set => this.RaiseAndSetIfChanged(ref _innerStartPercentPosition, value);
    }

    private PercentPosition _innerCenterPercentPosition;

    public PercentPosition InnerCenterPercentPosition
    {
        get => _innerCenterPercentPosition;
        set => this.RaiseAndSetIfChanged(ref _innerCenterPercentPosition, value);
    }

    private PercentPosition _innerEndPercentPosition;

    public PercentPosition InnerEndPercentPosition
    {
        get => _innerEndPercentPosition;
        set => this.RaiseAndSetIfChanged(ref _innerEndPercentPosition, value);
    }

    private PercentPosition _outerStartPercentPosition;

    public PercentPosition OuterStartPercentPosition
    {
        get => _outerStartPercentPosition;
        set => this.RaiseAndSetIfChanged(ref _outerStartPercentPosition, value);
    }

    private PercentPosition _outerCenterPercentPosition;

    public PercentPosition OuterCenterPercentPosition
    {
        get => _outerCenterPercentPosition;
        set => this.RaiseAndSetIfChanged(ref _outerCenterPercentPosition, value);
    }

    private PercentPosition _outerEndPercentPosition;

    public PercentPosition OuterEndPercentPosition
    {
        get => _outerEndPercentPosition;
        set => this.RaiseAndSetIfChanged(ref _outerEndPercentPosition, value);
    }

    private double _progressValue = 30;

    public double ProgressValue
    {
        get => _progressValue;
        set => this.RaiseAndSetIfChanged(ref _progressValue, value);
    }

    private string? _toggleDisabledText;

    public string? ToggleDisabledText
    {
        get => _toggleDisabledText;
        set => this.RaiseAndSetIfChanged(ref _toggleDisabledText, value);
    }

    private bool _toggleStatus;

    public bool ToggleStatus
    {
        get => _toggleStatus;
        set => this.RaiseAndSetIfChanged(ref _toggleStatus, value);
    }

    public ProgressBarViewModel(IScreen screen)
    {
        HostScreen = screen;
        _twoStopsGradientStrokeColor = new LinearGradientBrush
        {
            GradientStops =
            {
                new GradientStop(Color.Parse("#108ee9"), 0),
                new GradientStop(Color.Parse("#87d068"), 1)
            }
        };
        _threeStopsGradientStrokeColor = new LinearGradientBrush
        {
            GradientStops =
            {
                new GradientStop(Color.Parse("#87d068"), 0),
                new GradientStop(Color.Parse("#ffe58f"), 0.5),
                new GradientStop(Color.Parse("#ffccc7"), 1)
            }
        };
        _stepsChunkBrushes = new List<IBrush>
        {
            new SolidColorBrush(Colors.Green),
            new SolidColorBrush(Colors.Green),
            new SolidColorBrush(Colors.Red)
        };

        _innerStartPercentPosition = new PercentPosition
        {
            IsInner   = true,
            Alignment = LinePercentAlignment.Start
        };
        _innerCenterPercentPosition = new PercentPosition
        {
            IsInner   = true,
            Alignment = LinePercentAlignment.Center
        };
        _innerEndPercentPosition = new PercentPosition
        {
            IsInner   = true,
            Alignment = LinePercentAlignment.End
        };

        _outerStartPercentPosition = new PercentPosition
        {
            IsInner   = false,
            Alignment = LinePercentAlignment.Start
        };
        _outerCenterPercentPosition = new PercentPosition
        {
            IsInner   = false,
            Alignment = LinePercentAlignment.Center
        };
        _outerEndPercentPosition = new PercentPosition
        {
            IsInner   = false,
            Alignment = LinePercentAlignment.End
        };
        _toggleStatus       = true;
        _toggleDisabledText = "Disable";

        AddProgressValue = ReactiveCommand.Create(AddProgressValueImpl);
        SubProgressValue = ReactiveCommand.Create(SubProgressValueImpl);
        ToggleEnabledStatus = ReactiveCommand.Create(ToggleEnabledStatusImpl);
    }

    public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> AddProgressValue { get; }
    public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> SubProgressValue { get; }
    public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> ToggleEnabledStatus { get; }

    public void EnsureApiRows()
    {
        if (ApiRows is not null)
        {
            return;
        }

        ApiRows =
        [
            new ProgressBarApiRow("Value", Lang(ProgressBarShowCaseLangResourceKind.ApiPropertyValue), "double", "cyan", "0"),
            new ProgressBarApiRow("Minimum", Lang(ProgressBarShowCaseLangResourceKind.ApiPropertyMinimum), "double", "cyan", "0"),
            new ProgressBarApiRow("Maximum", Lang(ProgressBarShowCaseLangResourceKind.ApiPropertyMaximum), "double", "cyan", "100"),
            new ProgressBarApiRow("IsIndeterminate", Lang(ProgressBarShowCaseLangResourceKind.ApiPropertyIsIndeterminate), "bool", "green", "false"),
            new ProgressBarApiRow("IsProgressInfoVisible", Lang(ProgressBarShowCaseLangResourceKind.ApiPropertyIsProgressInfoVisible), "bool", "green", "true"),
            new ProgressBarApiRow("ProgressTextFormat", Lang(ProgressBarShowCaseLangResourceKind.ApiPropertyProgressTextFormat), "string?", "cyan", "null"),
            new ProgressBarApiRow("StrokeBrush", Lang(ProgressBarShowCaseLangResourceKind.ApiPropertyStrokeBrush), "IBrush?", "cyan", "null"),
            new ProgressBarApiRow("TrailColor", Lang(ProgressBarShowCaseLangResourceKind.ApiPropertyTrailColor), "Color?", "cyan", "null"),
            new ProgressBarApiRow("StrokeLineCap", Lang(ProgressBarShowCaseLangResourceKind.ApiPropertyStrokeLineCap), "PenLineCap", "blue", "Round"),
            new ProgressBarApiRow("SizeType", Lang(ProgressBarShowCaseLangResourceKind.ApiPropertySizeType), "SizeType", "blue", "Middle"),
            new ProgressBarApiRow("Status", Lang(ProgressBarShowCaseLangResourceKind.ApiPropertyStatus), "ProgressStatus", "blue", "Normal"),
            new ProgressBarApiRow("IndicatorThickness", Lang(ProgressBarShowCaseLangResourceKind.ApiPropertyIndicatorThickness), "double?", "cyan", "null"),
            new ProgressBarApiRow("SuccessThreshold", Lang(ProgressBarShowCaseLangResourceKind.ApiPropertySuccessThreshold), "double?", "cyan", "null"),
            new ProgressBarApiRow("SuccessStrokeBrush", Lang(ProgressBarShowCaseLangResourceKind.ApiPropertySuccessStrokeBrush), "IBrush?", "cyan", "null"),
            new ProgressBarApiRow("Orientation", Lang(ProgressBarShowCaseLangResourceKind.ApiPropertyOrientation), "Orientation", "blue", "Horizontal"),
            new ProgressBarApiRow("PercentPosition", Lang(ProgressBarShowCaseLangResourceKind.ApiPropertyPercentPosition), "PercentPosition?", "cyan", "null"),
            new ProgressBarApiRow("Steps", Lang(ProgressBarShowCaseLangResourceKind.ApiPropertySteps), "int", "cyan", "3"),
            new ProgressBarApiRow("StepsStrokeBrush", Lang(ProgressBarShowCaseLangResourceKind.ApiPropertyStepsStrokeBrush), "IReadOnlyList<IBrush>?", "cyan", "null"),
            new ProgressBarApiRow("StepCount", Lang(ProgressBarShowCaseLangResourceKind.ApiPropertyStepCount), "int", "cyan", "0"),
            new ProgressBarApiRow("StepGap", Lang(ProgressBarShowCaseLangResourceKind.ApiPropertyStepGap), "double", "cyan", "2"),
            new ProgressBarApiRow("DashboardGapPosition", Lang(ProgressBarShowCaseLangResourceKind.ApiPropertyDashboardGapPosition), "DashboardGapPosition", "blue", "Top"),
            new ProgressBarApiRow("GapDegree", Lang(ProgressBarShowCaseLangResourceKind.ApiPropertyGapDegree), "double", "cyan", "75")
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
            new ProgressBarDesignTokenRow("DefaultColor", Lang(ProgressBarShowCaseLangResourceKind.TokenNameDefaultColor), Lang(ProgressBarShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ProgressBarShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ProgressBarDesignTokenRow("RemainingColor", Lang(ProgressBarShowCaseLangResourceKind.TokenNameRemainingColor), Lang(ProgressBarShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ProgressBarShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ProgressBarDesignTokenRow("CircleTextColor", Lang(ProgressBarShowCaseLangResourceKind.TokenNameCircleTextColor), Lang(ProgressBarShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ProgressBarShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ProgressBarDesignTokenRow("LineBorderRadius", Lang(ProgressBarShowCaseLangResourceKind.TokenNameLineBorderRadius), Lang(ProgressBarShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ProgressBarShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ProgressBarDesignTokenRow("CircleMinimumTextFontSize", Lang(ProgressBarShowCaseLangResourceKind.TokenNameCircleMinimumTextFontSize), Lang(ProgressBarShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ProgressBarShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ProgressBarDesignTokenRow("CircleMinimumIconSize", Lang(ProgressBarShowCaseLangResourceKind.TokenNameCircleMinimumIconSize), Lang(ProgressBarShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ProgressBarShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ProgressBarDesignTokenRow("LineInfoIconSize", Lang(ProgressBarShowCaseLangResourceKind.TokenNameLineInfoIconSize), Lang(ProgressBarShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ProgressBarShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ProgressBarDesignTokenRow("LineInfoIconSizeSM", Lang(ProgressBarShowCaseLangResourceKind.TokenNameLineInfoIconSizeSM), Lang(ProgressBarShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ProgressBarShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ProgressBarDesignTokenRow("LineExtraInfoMargin", Lang(ProgressBarShowCaseLangResourceKind.TokenNameLineExtraInfoMargin), Lang(ProgressBarShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ProgressBarShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ProgressBarDesignTokenRow("LineProgressPadding", Lang(ProgressBarShowCaseLangResourceKind.TokenNameLineProgressPadding), Lang(ProgressBarShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ProgressBarShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private void AddProgressValueImpl()
    {
        var value = ProgressValue;
        value         += 10;
        ProgressValue =  Math.Min(value, 100);
    }

    private void SubProgressValueImpl()
    {
        var value = ProgressValue;
        value         -= 10;
        ProgressValue =  Math.Max(value, 0);
    }

    private void ToggleEnabledStatusImpl()
    {
        ToggleStatus = !ToggleStatus;
        if (ToggleStatus)
        {
            ToggleDisabledText = "Disable";
        }
        else
        {
            ToggleDisabledText = "Enable";
        }
    }

    private static string Lang(ProgressBarShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(ProgressBarShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            ProgressBarShowCaseLangResourceKind.ApiPropertyValue                     => en_US.ApiPropertyValue,
            ProgressBarShowCaseLangResourceKind.ApiPropertyMinimum                   => en_US.ApiPropertyMinimum,
            ProgressBarShowCaseLangResourceKind.ApiPropertyMaximum                   => en_US.ApiPropertyMaximum,
            ProgressBarShowCaseLangResourceKind.ApiPropertyIsIndeterminate           => en_US.ApiPropertyIsIndeterminate,
            ProgressBarShowCaseLangResourceKind.ApiPropertyIsProgressInfoVisible     => en_US.ApiPropertyIsProgressInfoVisible,
            ProgressBarShowCaseLangResourceKind.ApiPropertyProgressTextFormat        => en_US.ApiPropertyProgressTextFormat,
            ProgressBarShowCaseLangResourceKind.ApiPropertyStrokeBrush               => en_US.ApiPropertyStrokeBrush,
            ProgressBarShowCaseLangResourceKind.ApiPropertyTrailColor                => en_US.ApiPropertyTrailColor,
            ProgressBarShowCaseLangResourceKind.ApiPropertyStrokeLineCap             => en_US.ApiPropertyStrokeLineCap,
            ProgressBarShowCaseLangResourceKind.ApiPropertySizeType                  => en_US.ApiPropertySizeType,
            ProgressBarShowCaseLangResourceKind.ApiPropertyStatus                    => en_US.ApiPropertyStatus,
            ProgressBarShowCaseLangResourceKind.ApiPropertyIndicatorThickness        => en_US.ApiPropertyIndicatorThickness,
            ProgressBarShowCaseLangResourceKind.ApiPropertySuccessThreshold          => en_US.ApiPropertySuccessThreshold,
            ProgressBarShowCaseLangResourceKind.ApiPropertySuccessStrokeBrush        => en_US.ApiPropertySuccessStrokeBrush,
            ProgressBarShowCaseLangResourceKind.ApiPropertyOrientation               => en_US.ApiPropertyOrientation,
            ProgressBarShowCaseLangResourceKind.ApiPropertyPercentPosition           => en_US.ApiPropertyPercentPosition,
            ProgressBarShowCaseLangResourceKind.ApiPropertySteps                     => en_US.ApiPropertySteps,
            ProgressBarShowCaseLangResourceKind.ApiPropertyStepsStrokeBrush          => en_US.ApiPropertyStepsStrokeBrush,
            ProgressBarShowCaseLangResourceKind.ApiPropertyStepCount                 => en_US.ApiPropertyStepCount,
            ProgressBarShowCaseLangResourceKind.ApiPropertyStepGap                   => en_US.ApiPropertyStepGap,
            ProgressBarShowCaseLangResourceKind.ApiPropertyDashboardGapPosition      => en_US.ApiPropertyDashboardGapPosition,
            ProgressBarShowCaseLangResourceKind.ApiPropertyGapDegree                 => en_US.ApiPropertyGapDegree,
            ProgressBarShowCaseLangResourceKind.TokenNameDefaultColor                => en_US.TokenNameDefaultColor,
            ProgressBarShowCaseLangResourceKind.TokenNameRemainingColor              => en_US.TokenNameRemainingColor,
            ProgressBarShowCaseLangResourceKind.TokenNameCircleTextColor             => en_US.TokenNameCircleTextColor,
            ProgressBarShowCaseLangResourceKind.TokenNameLineBorderRadius            => en_US.TokenNameLineBorderRadius,
            ProgressBarShowCaseLangResourceKind.TokenNameCircleMinimumTextFontSize   => en_US.TokenNameCircleMinimumTextFontSize,
            ProgressBarShowCaseLangResourceKind.TokenNameCircleMinimumIconSize       => en_US.TokenNameCircleMinimumIconSize,
            ProgressBarShowCaseLangResourceKind.TokenNameLineInfoIconSize            => en_US.TokenNameLineInfoIconSize,
            ProgressBarShowCaseLangResourceKind.TokenNameLineInfoIconSizeSM          => en_US.TokenNameLineInfoIconSizeSM,
            ProgressBarShowCaseLangResourceKind.TokenNameLineExtraInfoMargin         => en_US.TokenNameLineExtraInfoMargin,
            ProgressBarShowCaseLangResourceKind.TokenNameLineProgressPadding         => en_US.TokenNameLineProgressPadding,
            ProgressBarShowCaseLangResourceKind.TokenScopeComponent                  => en_US.TokenScopeComponent,
            ProgressBarShowCaseLangResourceKind.TokenStatusStable                    => en_US.TokenStatusStable,
            _                                                                        => kind.ToString()
        };
    }
}

public sealed record ProgressBarApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record ProgressBarDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
