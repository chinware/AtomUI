using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Media;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.BorderBeam;

public class BorderBeamViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "BorderBeam";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    public ObservableCollection<BorderBeamColorPreset> ColorPresets { get; }

    private BorderBeamColorPreset _selectedColorPreset;

    public BorderBeamColorPreset SelectedColorPreset
    {
        get => _selectedColorPreset;
        set
        {
            if (EqualityComparer<BorderBeamColorPreset>.Default.Equals(_selectedColorPreset, value))
            {
                return;
            }

            this.RaiseAndSetIfChanged(ref _selectedColorPreset, value);
            SelectedColorStops = value.CreateColorStops();
        }
    }

    private AvaloniaList<BorderBeamColorStop> _selectedColorStops;

    public AvaloniaList<BorderBeamColorStop> SelectedColorStops
    {
        get => _selectedColorStops;
        private set => this.RaiseAndSetIfChanged(ref _selectedColorStops, value);
    }

    private ObservableCollection<BorderBeamApiRow>? _apiRows;
    private ObservableCollection<BorderBeamDesignTokenRow>? _designTokenRows;

    public ObservableCollection<BorderBeamApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<BorderBeamDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public BorderBeamViewModel(IScreen screen)
    {
        HostScreen = screen;
        ColorPresets =
        [
            new BorderBeamColorPreset("Ocean", ["#1677FF", "#36CFC9", "#95DE64"]),
            new BorderBeamColorPreset("Sunset", ["#FF7A45", "#F759AB", "#FFD666"]),
            new BorderBeamColorPreset("Aurora", ["#722ED1", "#B37FEB", "#13C2C2"]),
            new BorderBeamColorPreset("Forest", ["#52C41A", "#95DE64", "#D3F261"]),
            new BorderBeamColorPreset("Ember", ["#F5222D", "#FA8C16", "#FADB14"]),
            new BorderBeamColorPreset("Nebula", ["#2F54EB", "#722ED1", "#EB2F96"])
        ];
        _selectedColorPreset = ColorPresets[0];
        _selectedColorStops  = _selectedColorPreset.CreateColorStops();
    }

    public void EnsureApiRows()
    {
        if (ApiRows is not null)
        {
            return;
        }

        ApiRows =
        [
            new BorderBeamApiRow("Color", Lang(BorderBeamShowCaseLangResourceKind.ApiPropertyColor), "Color?", "cyan", "null"),
            new BorderBeamApiRow("ColorStops", Lang(BorderBeamShowCaseLangResourceKind.ApiPropertyColorStops), "AvaloniaList<BorderBeamColorStop>", "cyan", "empty"),
            new BorderBeamApiRow("Outset", Lang(BorderBeamShowCaseLangResourceKind.ApiPropertyOutset), "Thickness?", "cyan", "null"),
            new BorderBeamApiRow("BorderThickness", Lang(BorderBeamShowCaseLangResourceKind.ApiPropertyBorderThickness), "Thickness", "cyan", "token"),
            new BorderBeamApiRow("CornerRadius", Lang(BorderBeamShowCaseLangResourceKind.ApiPropertyCornerRadius), "CornerRadius", "cyan", "token"),
            new BorderBeamApiRow("IsMotionEnabled", Lang(BorderBeamShowCaseLangResourceKind.ApiPropertyIsMotionEnabled), "bool", "purple", "true"),
            new BorderBeamApiRow("Duration", Lang(BorderBeamShowCaseLangResourceKind.ApiPropertyDuration), "TimeSpan", "green", "00:00:06"),
            new BorderBeamApiRow("BeamSize", Lang(BorderBeamShowCaseLangResourceKind.ApiPropertyBeamSize), "double", "green", "100")
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
            new BorderBeamDesignTokenRow("BeamSize", Lang(BorderBeamShowCaseLangResourceKind.TokenNameBeamSize), Lang(BorderBeamShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(BorderBeamShowCaseLangResourceKind.TokenStatusStable), "success"),
            new BorderBeamDesignTokenRow("BeamOpacity", Lang(BorderBeamShowCaseLangResourceKind.TokenNameBeamOpacity), Lang(BorderBeamShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(BorderBeamShowCaseLangResourceKind.TokenStatusStable), "success"),
            new BorderBeamDesignTokenRow("MotionDuration", Lang(BorderBeamShowCaseLangResourceKind.TokenNameMotionDuration), Lang(BorderBeamShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(BorderBeamShowCaseLangResourceKind.TokenStatusStable), "success"),
            new BorderBeamDesignTokenRow("MaxVisibleStopPercent", Lang(BorderBeamShowCaseLangResourceKind.TokenNameMaxVisibleStopPercent), Lang(BorderBeamShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(BorderBeamShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(BorderBeamShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(BorderBeamShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            BorderBeamShowCaseLangResourceKind.ApiPropertyColor                 => en_US.ApiPropertyColor,
            BorderBeamShowCaseLangResourceKind.ApiPropertyColorStops            => en_US.ApiPropertyColorStops,
            BorderBeamShowCaseLangResourceKind.ApiPropertyOutset                => en_US.ApiPropertyOutset,
            BorderBeamShowCaseLangResourceKind.ApiPropertyBorderThickness       => en_US.ApiPropertyBorderThickness,
            BorderBeamShowCaseLangResourceKind.ApiPropertyCornerRadius          => en_US.ApiPropertyCornerRadius,
            BorderBeamShowCaseLangResourceKind.ApiPropertyIsMotionEnabled       => en_US.ApiPropertyIsMotionEnabled,
            BorderBeamShowCaseLangResourceKind.ApiPropertyDuration              => en_US.ApiPropertyDuration,
            BorderBeamShowCaseLangResourceKind.ApiPropertyBeamSize              => en_US.ApiPropertyBeamSize,
            BorderBeamShowCaseLangResourceKind.TokenNameBeamSize                => en_US.TokenNameBeamSize,
            BorderBeamShowCaseLangResourceKind.TokenNameBeamOpacity             => en_US.TokenNameBeamOpacity,
            BorderBeamShowCaseLangResourceKind.TokenNameMotionDuration          => en_US.TokenNameMotionDuration,
            BorderBeamShowCaseLangResourceKind.TokenNameMaxVisibleStopPercent   => en_US.TokenNameMaxVisibleStopPercent,
            BorderBeamShowCaseLangResourceKind.TokenScopeComponent              => en_US.TokenScopeComponent,
            BorderBeamShowCaseLangResourceKind.TokenStatusStable                => en_US.TokenStatusStable,
            _                                                                    => kind.ToString()
        };
    }
}

public sealed record BorderBeamColorPreset(string Name, IReadOnlyList<string> Colors)
{
    public AvaloniaList<BorderBeamColorStop> CreateColorStops()
    {
        var colorStops = new AvaloniaList<BorderBeamColorStop>();
        var count      = Math.Max(Colors.Count - 1, 1);
        for (var i = 0; i < Colors.Count; i++)
        {
            colorStops.Add(new BorderBeamColorStop
            {
                Color   = Color.Parse(Colors[i]),
                Percent = i * 100d / count
            });
        }

        return colorStops;
    }

    public override string ToString() => Name;
}

public sealed record BorderBeamApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record BorderBeamDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
