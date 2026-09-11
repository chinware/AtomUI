using System.Collections.ObjectModel;
using System.Globalization;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia.Collections;
using Avalonia.Media;
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

    public BorderBeamViewModel(IScreen screen)
    {
        HostScreen = screen;
        ColorPresets =
        [
            new BorderBeamColorPreset(
                "Ocean",
                "Dashboard",
                "A calm blue-green accent that works well for data views and cloud tooling.",
                [
                    new BorderBeamColorPresetStop("#1677ff", 0),
                    new BorderBeamColorPresetStop("#36cfc9", 52),
                    new BorderBeamColorPresetStop("#95de64", 100)
                ]),
            new BorderBeamColorPreset(
                "Sunset",
                "Upgrade",
                "A warm highlight for upgrade prompts, featured cards, and marketing blocks.",
                [
                    new BorderBeamColorPresetStop("#ff7a45", 0),
                    new BorderBeamColorPresetStop("#ff4d4f", 49),
                    new BorderBeamColorPresetStop("#ff85c0", 100)
                ]),
            new BorderBeamColorPreset(
                "Aurora",
                "AI",
                "A vivid cool-toned beam suited for AI assistants, copilots, and automation panels.",
                [
                    new BorderBeamColorPresetStop("#7c3aed", 0),
                    new BorderBeamColorPresetStop("#06b6d4", 57),
                    new BorderBeamColorPresetStop("#67e8f9", 100)
                ]),
            new BorderBeamColorPreset(
                "Forest",
                "Recommendation",
                "A bright natural palette that feels good on recommendation and growth-oriented cards.",
                [
                    new BorderBeamColorPresetStop("#22c55e", 0),
                    new BorderBeamColorPresetStop("#a3e635", 54),
                    new BorderBeamColorPresetStop("#facc15", 100)
                ]),
            new BorderBeamColorPreset(
                "Ember",
                "Alert",
                "A high-energy warm gradient for important alerts, launch cards, and hot paths.",
                [
                    new BorderBeamColorPresetStop("#fa541c", 0),
                    new BorderBeamColorPresetStop("#ff7875", 46),
                    new BorderBeamColorPresetStop("#ffd666", 100)
                ]),
            new BorderBeamColorPreset(
                "Nebula",
                "Labs",
                "A cool purple-pink mix that fits experimental modules and product lab surfaces.",
                [
                    new BorderBeamColorPresetStop("#2f54eb", 0),
                    new BorderBeamColorPresetStop("#722ed1", 44),
                    new BorderBeamColorPresetStop("#ff85c0", 100)
                ])
        ];
        _selectedColorPreset = ColorPresets[0];
        _selectedColorStops  = _selectedColorPreset.CreateColorStops();
    }

}

public sealed record BorderBeamColorPreset(
    string Name,
    string Usage,
    string Description,
    IReadOnlyList<BorderBeamColorPresetStop> Stops)
{
    public AvaloniaList<BorderBeamColorStop> CreateColorStops()
    {
        var colorStops = new AvaloniaList<BorderBeamColorStop>();
        foreach (var stop in Stops)
        {
            colorStops.Add(new BorderBeamColorStop
            {
                Color   = Color.Parse(stop.Color),
                Percent = stop.Percent
            });
        }

        return colorStops;
    }

    public override string ToString() => Name;
}

public sealed record BorderBeamColorPresetStop(string Color, double Percent)
{
    public string Label => $"{Color} · {Percent.ToString("0.##", CultureInfo.InvariantCulture)}%";
}
