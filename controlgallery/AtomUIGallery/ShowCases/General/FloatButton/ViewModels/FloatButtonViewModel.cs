using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.FloatButton;

public class FloatButtonViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "FloatButton";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<FloatButtonApiRow>? _apiRows;
    private ObservableCollection<FloatButtonDesignTokenRow>? _designTokenRows;

    public ObservableCollection<FloatButtonApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<FloatButtonDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }
    
    private bool _isOpened;

    public bool IsOpened
    {
        get => _isOpened;
        set => this.RaiseAndSetIfChanged(ref _isOpened, value);
    }

    public FloatButtonViewModel(IScreen screen)
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
            new FloatButtonApiRow("Placement", Lang(FloatButtonShowCaseLangResourceKind.ApiPropertyPlacement), "FloatButtonPlacement", "blue", "BottomRight"),
            new FloatButtonApiRow("FloatOffsetX", Lang(FloatButtonShowCaseLangResourceKind.ApiPropertyFloatOffsetX), "double", "cyan", "0"),
            new FloatButtonApiRow("FloatOffsetY", Lang(FloatButtonShowCaseLangResourceKind.ApiPropertyFloatOffsetY), "double", "cyan", "0"),
            new FloatButtonApiRow("Icon", Lang(FloatButtonShowCaseLangResourceKind.ApiPropertyIcon), "PathIcon?", "cyan", "FileTextOutlined"),
            new FloatButtonApiRow("Tooltip", Lang(FloatButtonShowCaseLangResourceKind.ApiPropertyTooltip), "string?", "cyan", "null"),
            new FloatButtonApiRow("TooltipColor", Lang(FloatButtonShowCaseLangResourceKind.ApiPropertyTooltipColor), "Color?", "cyan", "null"),
            new FloatButtonApiRow("Description", Lang(FloatButtonShowCaseLangResourceKind.ApiPropertyDescription), "object?", "cyan", "null"),
            new FloatButtonApiRow("ButtonType", Lang(FloatButtonShowCaseLangResourceKind.ApiPropertyButtonType), "FloatButtonType", "blue", "Default"),
            new FloatButtonApiRow("Shape", Lang(FloatButtonShowCaseLangResourceKind.ApiPropertyShape), "FloatButtonShape", "blue", "Circle"),
            new FloatButtonApiRow("Href", Lang(FloatButtonShowCaseLangResourceKind.ApiPropertyHref), "Uri?", "cyan", "null"),
            new FloatButtonApiRow("IsMotionEnabled", Lang(FloatButtonShowCaseLangResourceKind.ApiPropertyIsMotionEnabled), "bool", "green", "true"),
            new FloatButtonApiRow("IsBadgeEnabled", Lang(FloatButtonShowCaseLangResourceKind.ApiPropertyIsBadgeEnabled), "bool", "green", "false"),
            new FloatButtonApiRow("IsDotBadge", Lang(FloatButtonShowCaseLangResourceKind.ApiPropertyIsDotBadge), "bool", "green", "false"),
            new FloatButtonApiRow("BadgeCount", Lang(FloatButtonShowCaseLangResourceKind.ApiPropertyBadgeCount), "int", "cyan", "0"),
            new FloatButtonApiRow("BadgeColor", Lang(FloatButtonShowCaseLangResourceKind.ApiPropertyBadgeColor), "string?", "cyan", "null"),
            new FloatButtonApiRow("BadgeOverflowCount", Lang(FloatButtonShowCaseLangResourceKind.ApiPropertyBadgeOverflowCount), "int", "cyan", "99"),
            new FloatButtonApiRow("Trigger", Lang(FloatButtonShowCaseLangResourceKind.ApiPropertyTrigger), "FloatButtonGroupTrigger", "blue", "Default"),
            new FloatButtonApiRow("MenuPlacement", Lang(FloatButtonShowCaseLangResourceKind.ApiPropertyMenuPlacement), "FloatButtonGroupMenuPlacement", "blue", "Top"),
            new FloatButtonApiRow("IsOpen", Lang(FloatButtonShowCaseLangResourceKind.ApiPropertyIsOpen), "bool", "green", "false"),
            new FloatButtonApiRow("ToTopDuration", Lang(FloatButtonShowCaseLangResourceKind.ApiPropertyToTopDuration), "TimeSpan", "cyan", "00:00:00.450"),
            new FloatButtonApiRow("Target", Lang(FloatButtonShowCaseLangResourceKind.ApiPropertyTarget), "ScrollViewer?", "cyan", "null"),
            new FloatButtonApiRow("VisibilityHeight", Lang(FloatButtonShowCaseLangResourceKind.ApiPropertyVisibilityHeight), "double", "cyan", "400")
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
            new FloatButtonDesignTokenRow("FloatButtonSize", Lang(FloatButtonShowCaseLangResourceKind.TokenNameFloatButtonSize), Lang(FloatButtonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(FloatButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new FloatButtonDesignTokenRow("FloatButtonIconSize", Lang(FloatButtonShowCaseLangResourceKind.TokenNameFloatButtonIconSize), Lang(FloatButtonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(FloatButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new FloatButtonDesignTokenRow("SquareBadgeOffset", Lang(FloatButtonShowCaseLangResourceKind.TokenNameSquareBadgeOffset), Lang(FloatButtonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(FloatButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new FloatButtonDesignTokenRow("CircleBadgeOffset", Lang(FloatButtonShowCaseLangResourceKind.TokenNameCircleBadgeOffset), Lang(FloatButtonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(FloatButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new FloatButtonDesignTokenRow("PrimaryColor", Lang(FloatButtonShowCaseLangResourceKind.TokenNamePrimaryColor), Lang(FloatButtonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(FloatButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new FloatButtonDesignTokenRow("DescriptionLineHeight", Lang(FloatButtonShowCaseLangResourceKind.TokenNameDescriptionLineHeight), Lang(FloatButtonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(FloatButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new FloatButtonDesignTokenRow("FloatOffsetX", Lang(FloatButtonShowCaseLangResourceKind.TokenNameFloatOffsetX), Lang(FloatButtonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(FloatButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new FloatButtonDesignTokenRow("FloatOffsetY", Lang(FloatButtonShowCaseLangResourceKind.TokenNameFloatOffsetY), Lang(FloatButtonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(FloatButtonShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(FloatButtonShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(FloatButtonShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            FloatButtonShowCaseLangResourceKind.ApiPropertyPlacement             => en_US.ApiPropertyPlacement,
            FloatButtonShowCaseLangResourceKind.ApiPropertyFloatOffsetX          => en_US.ApiPropertyFloatOffsetX,
            FloatButtonShowCaseLangResourceKind.ApiPropertyFloatOffsetY          => en_US.ApiPropertyFloatOffsetY,
            FloatButtonShowCaseLangResourceKind.ApiPropertyIcon                  => en_US.ApiPropertyIcon,
            FloatButtonShowCaseLangResourceKind.ApiPropertyTooltip               => en_US.ApiPropertyTooltip,
            FloatButtonShowCaseLangResourceKind.ApiPropertyTooltipColor          => en_US.ApiPropertyTooltipColor,
            FloatButtonShowCaseLangResourceKind.ApiPropertyDescription           => en_US.ApiPropertyDescription,
            FloatButtonShowCaseLangResourceKind.ApiPropertyButtonType            => en_US.ApiPropertyButtonType,
            FloatButtonShowCaseLangResourceKind.ApiPropertyShape                 => en_US.ApiPropertyShape,
            FloatButtonShowCaseLangResourceKind.ApiPropertyHref                  => en_US.ApiPropertyHref,
            FloatButtonShowCaseLangResourceKind.ApiPropertyIsMotionEnabled       => en_US.ApiPropertyIsMotionEnabled,
            FloatButtonShowCaseLangResourceKind.ApiPropertyIsBadgeEnabled        => en_US.ApiPropertyIsBadgeEnabled,
            FloatButtonShowCaseLangResourceKind.ApiPropertyIsDotBadge            => en_US.ApiPropertyIsDotBadge,
            FloatButtonShowCaseLangResourceKind.ApiPropertyBadgeCount            => en_US.ApiPropertyBadgeCount,
            FloatButtonShowCaseLangResourceKind.ApiPropertyBadgeColor            => en_US.ApiPropertyBadgeColor,
            FloatButtonShowCaseLangResourceKind.ApiPropertyBadgeOverflowCount    => en_US.ApiPropertyBadgeOverflowCount,
            FloatButtonShowCaseLangResourceKind.ApiPropertyTrigger               => en_US.ApiPropertyTrigger,
            FloatButtonShowCaseLangResourceKind.ApiPropertyMenuPlacement         => en_US.ApiPropertyMenuPlacement,
            FloatButtonShowCaseLangResourceKind.ApiPropertyIsOpen                => en_US.ApiPropertyIsOpen,
            FloatButtonShowCaseLangResourceKind.ApiPropertyToTopDuration         => en_US.ApiPropertyToTopDuration,
            FloatButtonShowCaseLangResourceKind.ApiPropertyTarget                => en_US.ApiPropertyTarget,
            FloatButtonShowCaseLangResourceKind.ApiPropertyVisibilityHeight      => en_US.ApiPropertyVisibilityHeight,
            FloatButtonShowCaseLangResourceKind.TokenNameFloatButtonSize         => en_US.TokenNameFloatButtonSize,
            FloatButtonShowCaseLangResourceKind.TokenNameFloatButtonIconSize     => en_US.TokenNameFloatButtonIconSize,
            FloatButtonShowCaseLangResourceKind.TokenNameSquareBadgeOffset       => en_US.TokenNameSquareBadgeOffset,
            FloatButtonShowCaseLangResourceKind.TokenNameCircleBadgeOffset       => en_US.TokenNameCircleBadgeOffset,
            FloatButtonShowCaseLangResourceKind.TokenNamePrimaryColor            => en_US.TokenNamePrimaryColor,
            FloatButtonShowCaseLangResourceKind.TokenNameDescriptionLineHeight   => en_US.TokenNameDescriptionLineHeight,
            FloatButtonShowCaseLangResourceKind.TokenNameFloatOffsetX            => en_US.TokenNameFloatOffsetX,
            FloatButtonShowCaseLangResourceKind.TokenNameFloatOffsetY            => en_US.TokenNameFloatOffsetY,
            FloatButtonShowCaseLangResourceKind.TokenScopeComponent              => en_US.TokenScopeComponent,
            FloatButtonShowCaseLangResourceKind.TokenStatusStable                => en_US.TokenStatusStable,
            _                                                                    => kind.ToString()
        };
    }
}

public sealed record FloatButtonApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record FloatButtonDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
