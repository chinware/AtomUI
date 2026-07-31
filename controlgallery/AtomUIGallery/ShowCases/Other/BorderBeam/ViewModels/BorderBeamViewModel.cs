using System.Collections.ObjectModel;
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
